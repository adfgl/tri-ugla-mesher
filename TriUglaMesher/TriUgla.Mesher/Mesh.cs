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

        public void Invalidate(Face face)
        {
            Edge e0 = face.Edge;
            Edge e = e0;
            do
            {
                e.Invalid = true;
                e = e.Next;
            } while (!ReferenceEquals(e0, e))
        }

        public bool TryReconnect(Node node)
        {
            Edge e0 = node.Edge;
            Edge e = e0;
            do
            {
                if (!e.Invalid)
                {
                    node.Edge = e;
                    return true;
                }
                e = e.Prev.Twin!;
            } while (!ReferenceEquals(e0, e))

            return false;
        }
    }

    public class ElementRemover
    {

    }

    public class ElementInvalidator
    {
        public void Invalidate(Node node)
        {
            if (node.Invalid) return;
            node.Invalid = true;
        }

        public void Invalidate(Edge edge)
        {
            if (edge.Invalid) return;
            edge.Invalid = true;
        }

        public void Invalidate(Face face)
        {
            Edge e0 = face.Edge;
            Edge e = e0;
            do
            {
                Invalidate(e);
                e = e.Next;
            } while (!ReferenceEquals(e0, e))
        }
    }

    public class MeshElement
    {
        int _visitStamp = 0;

        public bool Invalid { get; internal set; }
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
