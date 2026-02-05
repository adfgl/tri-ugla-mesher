using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public struct Edge
    {
        public NodeId start, end;
        public EdgeId next, prev, twin;
        public FaceId face;
        public EdgeContext context;

        public readonly bool Boundary => twin.value.IsNull;

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
