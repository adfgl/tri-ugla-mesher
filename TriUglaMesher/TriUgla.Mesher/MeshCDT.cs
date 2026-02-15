using System;
using TriUgla.ExactMath;
using TriUgla.Geometry;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Quality;
using TriUgla.Mesher.Services;
using TriUgla.Mesher.SuperStrctures;
using TriUgla.Mesher.Topology;

namespace TriUgla.Mesher
{
    public class MeshCDT
    {
        readonly List<Constraint> _constraints = new List<Constraint>();
        readonly List<Loop> _loops = new List<Loop>();
        readonly IlligalEdges _illigals = new IlligalEdges();

        readonly Locator _locator;

        readonly NodeSplitter _nodeSplitter;
        readonly EdgeSplitter _edgeSplitter;
        readonly FaceSplitter _faceSplitter;
        readonly EdgeFlipper _edgeFlipper;

        readonly NodeInserter _nodeInserter;
        readonly NodeRemover _nodeRemover;
        readonly SpanInserter _spanInserter;
        readonly EdgeLegalizer _legalizer;
        readonly MeshRefiner _refiner;

        public MeshCDT(Rect rect, SuperStructure superStructure)
        {
            Rect = rect;
            SuperStructure = superStructure;
            SuperStructure.Build(in rect);
            Mesh = new HalfEdgeMesh(SuperStructure.Faces[0]);

            _locator = new Locator(Mesh);

            _nodeSplitter = new NodeSplitter(_illigals);
            _edgeSplitter = new EdgeSplitter(_illigals);
            _faceSplitter = new FaceSplitter(_illigals);
            _edgeFlipper = new EdgeFlipper(Predicates, _illigals);

            _legalizer = new EdgeLegalizer(_illigals, _edgeFlipper);
            _nodeInserter = new NodeInserter(_nodeSplitter, _edgeSplitter, _faceSplitter);
            _nodeRemover = new NodeRemover(Predicates, _illigals);
            _spanInserter = new SpanInserter(Predicates, _illigals, _edgeFlipper, _edgeSplitter);

            _refiner = new MeshRefiner(Predicates, _locator, _legalizer, _edgeSplitter, _nodeInserter);
        }

        public Rect Rect { get; }
        public SuperStructure SuperStructure { get; }
        public Predicates Predicates { get; } = new Predicates();

        public HalfEdgeMesh Mesh { get; }
        public IReadOnlyList<Constraint> Constraints => _constraints;
        public IReadOnlyList<Loop> Loops => _loops;

        static bool Fail(out string? reason, string message)
        {
            reason = message;
            return false;
        }

        static string CtxConstraint(Constraint c) => $"Constraint '{c.Name}'";
        static string CtxLoop(Loop l) => $"Loop '{l.Name}'";

        static string WhyNodeInvalid() => "node is invalid.";
        static string WhyNodeSuper() => "node is part of the super structure.";

        public int Refine(List<Face> faces, FaceRanker ranker, in RefineSettings settings)
        {
            return _refiner.Refine(faces, ranker, in settings);
        }

        Face? _lastFound = null;

        public HitResult Locate(double x, double y, double eps = 1e-6)
        {
            Face start = _lastFound ?? Mesh.Root;
            if (!Rect.Contains(x, y))
            {
                return HitResult.None(start, 0);
            }

            HitResult result = _locator.Locate(x, y, start, eps);
            _lastFound = result.AtFace;
            return result;
        }

        public Node? TryInsertNode(Vec4 p, out string? reason, double eps = 1e-6)
        {
            reason = null;

            HitResult result = Locate(p.x, p.y, eps);
            if (result.Kind == HitKind.None)
            {
                return Fail(out reason, "Cannot insert node: point is outside the mesh (Locate returned None).") ? null : null;
            }
              

            Node node = _nodeInserter.Insert(in result, p, NodeKind.Normal);
            if (node.Kind == NodeKind.Super)
            {
                return Fail(out reason, "Cannot insert node: insertion landed on the super structure.") ? null : null;
            }
            return node;
        }

        public bool TryRemoveNode(Node node, out string? reason)
        {
            if (node.Kind == NodeKind.Super)
                return Fail(out reason, "Cannot remove node: it is part of the super structure.");

            if (node.Constrained)
                return Fail(out reason, $"Cannot remove node: it is constrained ({node.Constraints} constraints).");

            if (node.Invalid)
                return Fail(out reason, "Cannot remove node: it is invalid.");

            if (!_nodeRemover.Remove(node))
                return Fail(out reason, "Cannot remove node: topology prevents removal (still referenced or would break invariants).");

            reason = null;
            return true;
        }

        public bool TryInsertConstraint(Constraint constraint, out string? reason)
        {
            reason = null;

            for (int i = 0; i < constraint.Spans.Count; i++)
            {
                ConstraintSpan span = constraint.Spans[i];
                Node a = span.From.Node;
                Node b = span.To.Node;

                if (a.Invalid) return Fail(out reason, $"{CtxConstraint(constraint)} span[{i}] From: {WhyNodeInvalid()}");
                if (b.Invalid) return Fail(out reason, $"{CtxConstraint(constraint)} span[{i}] To: {WhyNodeInvalid()}");

                if (a.Kind == NodeKind.Super) return Fail(out reason, $"{CtxConstraint(constraint)} span[{i}] From: {WhyNodeSuper()}");
                if (b.Kind == NodeKind.Super) return Fail(out reason, $"{CtxConstraint(constraint)} span[{i}] To: {WhyNodeSuper()}");
            }

            for (int i = 0; i < constraint.Points.Count; i++)
            {
                ConstraintPoint point = constraint.Points[i];
                Node n = point.Node;

                if (n.Invalid) return Fail(out reason, $"{CtxConstraint(constraint)} point[{i}]: {WhyNodeInvalid()}");
                if (n.Kind == NodeKind.Super) return Fail(out reason, $"{CtxConstraint(constraint)} point[{i}]: {WhyNodeSuper()}");
            }

            for (int i = 0; i < constraint.Spans.Count; i++)
            {
                ConstraintSpan span = constraint.Spans[i];
                _spanInserter.Insert(span.From.Node, span.To.Node, EdgeKind.Feature);
                _legalizer.Legalize();
            }

            for (int i = 0; i < constraint.Points.Count; i++)
                constraint.Points[i].Node.Constrain();

            _constraints.Add(constraint);
            return true;
        }


        public bool TryRemoveConstraint(Constraint constraint, out string? reason)
        {
            int index = _constraints.IndexOf(constraint);
            if (index < 0)
                return Fail(out reason, $"{CtxConstraint(constraint)} not found in mesh.");

            List<Edge> path = new List<Edge>(16);
            for (int i = 0; i < constraint.Spans.Count; i++)
            {
                ConstraintSpan span = constraint.Spans[i];

                path.Clear();
                bool any = false;

                foreach (Edge e in span.Edges(path))
                {
                    any = true;
                    if (e.Features <= 0)
                        return Fail(out reason, $"{CtxConstraint(constraint)} span[{i}]: edge in path has no Feature constraint.");
                }

                if (!any)
                    return Fail(out reason, $"{CtxConstraint(constraint)} span[{i}]: produced no edges.");
            }

            for (int i = 0; i < constraint.Points.Count; i++)
            {
                Node n = constraint.Points[i].Node;

                if (n.Kind == NodeKind.Super)
                    return Fail(out reason, $"{CtxConstraint(constraint)} point[{i}]: node is part of the super structure (cannot release).");

                if (!n.Constrained)
                    return Fail(out reason, $"{CtxConstraint(constraint)} point[{i}]: node is not constrained (nothing to release).");
            }

            for (int i = 0; i < constraint.Spans.Count; i++)
            {
                ConstraintSpan span = constraint.Spans[i];

                path.Clear();
                foreach (Edge e in span.Edges(path))
                {
                    _illigals.Push(e);
                    e.Release(EdgeKind.Feature);
                }
                _legalizer.Legalize();
            }

            for (int i = 0; i < constraint.Points.Count; i++)
                constraint.Points[i].Node.Release();

            _constraints.RemoveAt(index);
            reason = null;
            return true;
        }

        public bool TryInsertLoop(Loop loop, out string? reason)
        {
            reason = null;
            loop.Close();

            int n = loop.Nodes.Count;
            if (n < 4)
                return Fail(out reason, $"{CtxLoop(loop)} invalid: must have at least 3 unique points (closed loop includes duplicate last=first).");

            double area = loop.SignedArea();
            if (area == 0)
                return Fail(out reason, $"{CtxLoop(loop)} invalid: zero area.");

            if (SelfIntersects(loop))
                return Fail(out reason, $"{CtxLoop(loop)} invalid: self-intersecting.");

            for (int i = 0; i < n - 1; i++)
            {
                Node node = loop.Nodes[i];

                if (node.Invalid)
                    return Fail(out reason, $"{CtxLoop(loop)} invalid: node[{i}] is invalid.");

                if (node.Kind == NodeKind.Super)
                    return Fail(out reason, $"{CtxLoop(loop)} invalid: node[{i}] is part of the super structure.");
            }

            EdgeKind kind = area > 0 ? EdgeKind.Contour : EdgeKind.Hole;

            for (int i = 0; i < n - 1; i++)
            {
                Node a = loop.Nodes[i];
                Node b = loop.Nodes[i + 1];

                _spanInserter.Insert(a, b, kind);
                _legalizer.Legalize();
            }

            _loops.Add(loop);
            return true;
        }

        public bool TryRemoveLoop(Loop loop, out string? reason)
        {
            int index = _loops.IndexOf(loop);
            if (index < 0)
                return Fail(out reason, $"{CtxLoop(loop)} not found in mesh.");

            loop.Close();

            double area = loop.SignedArea();
            if (area == 0)
                return Fail(out reason, $"{CtxLoop(loop)} invalid: zero area (cannot determine kind).");

            EdgeKind kind = area > 0 ? EdgeKind.Contour : EdgeKind.Hole;

            List<Edge> edges = new List<Edge>(64);
            loop.Edges(edges);

            if (edges.Count == 0)
                return Fail(out reason, $"{CtxLoop(loop)} invalid: produced no edges.");

            for (int i = 0; i < edges.Count; i++)
            {
                Edge e = edges[i];

                bool hasKind =
                    (kind == EdgeKind.Contour && e.Contours > 0) ||
                    (kind == EdgeKind.Hole && e.Holes > 0);

                if (!hasKind)
                    return Fail(out reason, $"{CtxLoop(loop)} invalid: edge[{i}] does not contain expected '{kind}' constraint.");
            }

            for (int i = 0; i < edges.Count; i++)
            {
                Edge e = edges[i];
                _illigals.Push(e);
                e.Release(kind);
            }

            _legalizer.Legalize();
            _loops.RemoveAt(index);

            reason = null;
            return true;
        }

        bool SelfIntersects(Loop loop)
        {
            loop.Close();

            int n = loop.Nodes.Count;  
            int m = n - 1;             

            if (m < 4) return false;

            for (int i = 0; i < m; i++)
            {
                Node p1 = loop.Nodes[i];
                Node p2 = loop.Nodes[i + 1];

                Vec4 a4 = p1.Vertex;
                Vec4 b4 = p2.Vertex;

                double px1 = a4.x, py1 = a4.y;
                double px2 = b4.x, py2 = b4.y;

                if (px1 == px2 && py1 == py2)
                    return true;

                for (int j = i + 1; j < m; j++)
                {
                    if (j == i + 1) continue;
                    if (i == 0 && j == m - 1) continue;

                    Node q1 = loop.Nodes[j];
                    Node q2 = loop.Nodes[j + 1];

                    if ((p1 == q1 && p2 == q2) ||
                        (p1 == q2 && p2 == q1))
                    {
                        continue;
                    }

                    Vec4 c4 = q1.Vertex;
                    Vec4 d4 = q2.Vertex;

                    double qx1 = c4.x, qy1 = c4.y;
                    double qx2 = d4.x, qy2 = d4.y;

                    if (qx1 == qx2 && qy1 == qy2)
                        return true;

                    int hit = Predicates.Intersects(px1, py1, px2, py2, qx1, qy1, qx2, qy2);
                    if (hit >= 0)
                        return true;
                }
            }

            return false;
        }
    }
}
