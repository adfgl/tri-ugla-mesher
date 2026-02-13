using TriUgla.Geometry;

namespace TriUgla.HalfEdge
{
    public sealed class Node(Vec4 vertex, NodeKind kind) : MeshElement
    {
        int _constraints = 0;

        public Vec4 Vertex = vertex;
        public NodeKind Kind { get; set; } = kind;
        public Edge Edge { get; internal set; } = null!;

        public int Constraints => _constraints;
        public bool Constrained => _constraints > 0;

        public void Constrain()
        {
            _constraints++;
        }

        public bool Release()
        {
            if (!Constrained) return false;
            _constraints--;
            return true;
        }
    }
}
