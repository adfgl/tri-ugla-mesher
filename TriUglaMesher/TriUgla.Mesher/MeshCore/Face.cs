using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public struct Face
    {
        int _stamp;
        public Id edge;
        public FaceContext context;

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
