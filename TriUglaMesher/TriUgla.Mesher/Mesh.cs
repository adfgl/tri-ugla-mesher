using TriUgla.Mesher.MeshCore;
using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher
{
    public sealed class Mesh
    {
        
        
    }

    public class MeshElement
    {
        int _visitStamp = 0;

        public int Id { get; internal set; }
        public bool Dead { get; internal set; }
        
        public bool TryVisit(int stamp)
        {
            if (_visitsStamp != stamp)
            {
                _visitsStamp = stamp;
                return true;
            }
            return false;
        }
    }
}
