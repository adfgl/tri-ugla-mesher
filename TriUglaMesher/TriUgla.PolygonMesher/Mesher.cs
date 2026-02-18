using TriUgla.Geometry;
using TriUgla.HalfEdge;
using TriUgla.Mesher;
using TriUgla.Mesher.Quality;
using TriUgla.Mesher.SuperStrctures;

namespace TriUgla.PolygonMesher
{
    public class Mesher
    {
        readonly MeshCDT _mesh;

        public Mesher(Rect rect)
        {
            _mesh = new MeshCDT(rect, new SuperTriangleStructure());
        }

        public HalfEdgeMesh Mesh => _mesh.Mesh;

        public Mesher Refine()
        {
            FaceRanker ranker = new FaceRanker();
            ranker.Area.Weight = 0.0;
            ranker.Area.MinArea = 30;

            ranker.Angle.Weight = 1.0;
            ranker.Angle.MinAngleDeg = 22.7;

            ranker.VertexArea.Weight = 1.0;

            ranker.Edge.Weight = 0.0;
            ranker.Edge.MinEdgeLength = 100;
            ranker.Edge.MaxEdgeLength = 200;

            _mesh.Refine(ranker, new RefineSettings());
            return this;
        }

        public Node Insert(Vec4 v)
        {
            Node? inserted = _mesh.TryInsertNode(v, out string? reason);
            if (inserted != null)
            {
                return inserted;
            }
            throw new Exception(reason);
        }

        public Loop Insert(IEnumerable<Vec4> vertices, string id)
        {
            string? reason = null;
            bool failed = false;
            List<Node> nodes = new List<Node>();
            foreach (Vec4 v in vertices)
            {
                Node? node = _mesh.TryInsertNode(v, out reason);
                if (node is null)
                {
                    failed = true;
                    break;
                }
            }

            if (failed)
            {
                foreach (var item in nodes)
                {
                    _mesh.TryRemoveNode(item, out _);
                }
                throw new Exception(reason);
            }

            Loop loop = new Loop() { Name = id, Nodes = nodes };
            if (!_mesh.TryInsertLoop(loop, out reason))
            {
                throw new Exception(reason);
            }
            return loop;
        }

        public Constraint Insert(Vec4 vtx, string? id)
        {
            Node node = Insert(vtx);

            Constraint constraint = new Constraint()
            {
                Points = [new ConstraintPoint(node)
                {
                    Name = id,
                }]
            };

            if (!_mesh.TryInsertConstraint(constraint, out string? reason))
            {
                throw new Exception(reason);
            }
            return constraint;
        }

        public Constraint Insert(Vec4 from, Vec4 to, string? id)
        {
            Node fromNode = Insert(from);
            Node toNode = Insert(to);

            Constraint constraint = new Constraint()
            {
                Spans = [new ConstraintSpan(fromNode, toNode)
                {
                    Name = id,
                }]
            };

            if (!_mesh.TryInsertConstraint(constraint, out string? reason))
            {
                throw new Exception(reason);
            }
            return constraint;
        }
    }
}
