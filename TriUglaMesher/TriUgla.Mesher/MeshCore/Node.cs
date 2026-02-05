namespace TriUgla.Mesher.MeshCore
{
    public struct Node
    {
        public Vertex vertex;
        public EdgeId edge;
        public NodeContext context;

        int _stamp;

        public bool TryVisit(int stamp)
        {
            if (_stamp != stamp)
            {
                _stamp = stamp;
                return true;
            }
            return false;
        }
    }
}
