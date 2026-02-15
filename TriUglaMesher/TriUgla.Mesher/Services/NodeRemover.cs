using TriUgla.ExactMath;
using TriUgla.Geometry;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Helpers;

namespace TriUgla.Mesher.Services
{
    public class NodeRemover(
        Predicates predicates, 
        IlligalEdges illigals)
    {
        readonly Predicates _predicates = predicates;
        readonly IlligalEdges _illigals = illigals;

        public bool Remove(Node node)
        {
            if (!CanRemove(node))
            {
                return false;
            }

            Cavity cavity = BuildCavity(node);
            DetachCavity(in cavity);
            FillCavity(in cavity);
            DrainIlligals(in cavity);

            ElementInvalidator.Invalidate(node);
            ElementLinker.UnlinkNodeEdge(node);
            return true;
        }

        void DrainIlligals(in Cavity cavity)
        {
            foreach (Edge item in cavity.Ring)
            {
                if (item.Twin is null) continue;
                _illigals.Push(item.Twin);
            }
        }

        public static bool CanRemove(Node node, out string? reason)
        {
            if (node.Invalid)
            {
                reason = $"Node is invalid [{node}].";
                return false;
            }

            if (node.Constrained)
            {
                reason = $"Node is constrained or part of edge constraint [{node}].";
                return false;
            }

            if (node.Kind == NodeKind.Super)
            {
                reason = $"Node is part of super structure [{node}].";
                return false;
            }
            reason = null;
            return true;
        }

        static Cavity BuildCavity(Node node)
        {
            Cavity cavity = new Cavity();
            Edge e0 = node.Edge;
            Edge e = e0;
            do
            {
                Edge boundary = e.Next;
                cavity.Ring.Add(boundary.Twin!);
                cavity.FacesToRemove.Add(e.Face);
                e = e.Prev.Twin!;
            } while (e0 != e);
            return cavity;
        }

        void DetachCavity(in Cavity cavity)
        {
            foreach (Face item in cavity.FacesToRemove)
            {
                ElementInvalidator.Invalidate(item);
                ElementLinker.Unlink(item);
            }
        }

        void FillCavity(in Cavity cavity)
        {
            List<Edge> edges = new List<Edge>();

            List<Vec2> verts = new List<Vec2>(cavity.Ring.Count);
            foreach (Edge item in cavity.Ring)
            {
                var vtx = item.NodeStart.Vertex;
                verts.Add(new Vec2(vtx.x, vtx.y));
                edges.Add(item);
            }

            List<int> tris = new EarClipper(_predicates).Triangulate(verts);
            for (int t = 0; t < tris.Count; t += 3)
            {
                Node a = cavity.Ring[tris[t + 0]].NodeStart;
                Node b = cavity.Ring[tris[t + 1]].NodeStart;
                Node c = cavity.Ring[tris[t + 2]].NodeStart;

                Edge ab = new Edge();
                Edge bc = new Edge();
                Edge ca = new Edge();
                Face f = new Face();

                ElementLinker.Link(f, ab, bc, ca, a, b, c);

                edges.Add(ab);
                edges.Add(bc);
                edges.Add(ca);
            }

            ElementLinker.LinkEdgeTwins(edges);
        }

        readonly struct Cavity()
        {
            public readonly List<Edge> Ring = new List<Edge>(8);
            public readonly List<Face> FacesToRemove = new List<Face>();
        }
    }
}
