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

            Invalidate

            
        }

        
    }

    public class ElementRemover
    {
        public void Remove(Node node)
        {
            Edge? edge = node.Edge;
            if (esge is null) return;

            node.Edge = null;
            if (ReferenceEquals(edge.Start, node))
            {
                edge.Start = null;
            }
        }

        public void Remove(Edge edge)
        {
            if (edge.Start is not null)
            {
                Remove(edge.Start);
            }
            
            Face? face = edge.Face;
            if (face is not null)
            {
                if (ReferenceEquals(face.Edge, edge))
                {
                    face.Edge = null;
                }
                edge.Face = null;
            }

            Edge? twin = edge.Twin;
            if (twin is not null)
            {
                twin.Twin = null;
                edge.Twin = null;
            }

            Edge? next = edge.Next;
            if (next is not null)
            {
                next.Prev = null;
                edge.Next = null;
            }

            Edge? prev = edge.Prev;
            if (prev is not null)
            {
                prev.Next = null;
                edge.Prev = null;
            }
        }

        void DetatchPrev(Edge? prev)
        {
            if (prev is null) return;
            prev.Next = null;
            edge.Prev = null;
        }

        void DetatchNext(Edge? next)
        {
            if (next is null) return;
            next.Prev = null;
            edge.Prev = null;
        }
    }

    public class ElementInvalidator
    {
        public void Invalidate(Node node)
        {
            node.Invalid = true;
        }

        public void Invalidate(Edge edge)
        {
            edge.Invalid = true;
        }

        public void Invalidate(Face face)
        {
            if (face.Invalid) return;
            InvalidateEdges(face);

            Edge e0 = face.Edge;
            Edge e = e0;
            do
            {
                Node n = e.Start;
                if (!TryReconnect(n))
                {
                    Invalidate(n);
                }
                e = e.Next;
            } while (!ReferenceEquals(e0, e))

            face.Invalid = true;
        }

        void InvalidateEdges(Face face)
        {
            Edge e0 = face.Edge;
            Edge e = e0;
            do
            {
                Invalidate(e);
                e = e.Next;
            } while (!ReferenceEquals(e0, e))
        }

        bool TryReconnect(Node node)
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
