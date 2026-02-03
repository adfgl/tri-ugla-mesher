namespace TriUgla.Mesher.MeshCore
{
    public sealed class Node(Vertex vertex) : MeshElement
    {
        int _constraints = 0;

        public Vertex Vertex { get; } = vertex;
        public Edge Edge { get; internal set; } = null!;
        public bool Constrained => _constraints > 0;

        public void TransmitContext(Node other)
        {
            other.Payload = Payload;
            other._constraints = _constraints;
        }

        public void AddConstraint()
        {
            _constraints++;
        }

        public bool RemoveConstraint()
        {
            if (!Constrained)
                return false;

            _constraints--;
            return true;
        }
    }
}
