using TriUgla.Geometry;

namespace TriUgla.HalfEdge
{
    public sealed class Face : MeshElement
    {
        public Edge Edge { get; internal set; } = null!;
        public int RegionId { get; internal set; }
        public int Depth { get; internal set; }
        public FaceKind Kind { get; internal set; }

        public bool ContainsSuperNode
        {
            get
            {
                bool hasSuper = false;
                Edge e0 = Edge;
                Edge e = e0;
                do
                {
                    if (e.NodeStart.Kind == NodeKind.Super)
                    {
                        hasSuper = true;
                        break;
                    }
                    e = e.Next;
                } while (e != e0);

                return hasSuper;   
            }
        }

        public Circle CircumCircle
        {
            get
            {
                Vec4 a = Edge.NodeStart.Vertex;
                Vec4 b = Edge.Next.NodeStart.Vertex;
                Vec4 c = Edge.Prev.NodeStart.Vertex;
                return Circle.From3(new Vec2(a.x, a.y), new Vec2(b.x, b.y), new Vec2(c.x, c.y));
            }
        }

        public void TransmitContextTo(Face other)
        {
            other.Invalid = Invalid;
            other.Payload = Payload;
            other.RegionId = RegionId;
            other.Depth = Depth;
            other.Kind = Kind;
        }
    }
}
