using TriUgla.Mesher.MeshCore;
using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher
{
    public sealed class Mesh
    {
        public Face Root { get; set; }
        
        public bool Remove(Face face)
        {
            if (face.Dead) 
            {
                return false;
            }

            Edge e0 = face.Edge;
            Edge e = e0;
            do 
            {
                e.Dead = true;
                e = e.Next;
            } while (!ReferenceEquals(e0, e))

            
        }
    }

    public class MeshElement
    {
        int _visitStamp = 0;

        public bool Dead { get; internal set; }
        public int Payload { get; set; }

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
