using TriUgla.Geometry;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.SuperStrctures
{
    public abstract class SuperStructure
    {
        protected readonly List<Node> _nodes;
        protected readonly List<Edge> _edges;
        protected readonly List<Face> _faces;

        protected SuperStructure(int nodeCapacity = 8, int faceCapacity = 8)
        {
            _nodes = new List<Node>(nodeCapacity);
            _faces = new List<Face>(faceCapacity);
            _edges = new List<Edge>(faceCapacity * 3);
        }

        public IReadOnlyList<Node> Nodes => _nodes;
        public IReadOnlyList<Face> Faces => _faces;

        public void Build(in Rect bounds)
        {
            _nodes.Clear();
            _faces.Clear();
            _edges.Clear();

            List<Vec4> ring = BuildRing(bounds);
            if (ring.Count < 3) throw new InvalidOperationException("Need at least 3 ring vertices.");

            for (int i = 0; i < ring.Count; i++)
            {
                Node node = new Node(ring[i], NodeKind.Super)
                {
                    Kind = NodeKind.Super
                };
                _nodes.Add(node);
            }

            Node v0 = _nodes[0];
            for (int i = 1; i <= ring.Count - 2; i++)
            {
                Node a = v0;
                Node b = _nodes[i];
                Node c = _nodes[i + 1];

                Face f = new Face();
                Edge e0 = new Edge();
                Edge e1 = new Edge();
                Edge e2 = new Edge();

                ElementLinker.Link(f, e0, e1, e2, a, b, c);

                _faces.Add(f);
                _edges.Add(e0);
                _edges.Add(e1);
                _edges.Add(e2);
            }

            ElementLinker.LinkEdgeTwins(_edges);
        }

        protected abstract List<Vec4> BuildRing(in Rect bounds);
    }
}
