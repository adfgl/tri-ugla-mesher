using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public struct Edge
    {
        int _stamp;
        public Id start, end;
        public Id next, prev, twin;
        public Id face;
        public EdgeContext context;

        public readonly bool Boundary => twin.IsNull;

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
