using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public struct Node
    {
        int _stamp;

        public Vertex vertex;
        public Id edge;

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
