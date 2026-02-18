using TriUgla.Geometry;

namespace TriUgla.HalfEdge
{
    public sealed class Face : MeshElement
    {
        public Edge Edge { get; internal set; } = null!;
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

        public Vec2 Center
        {
            get
            {
                if (Edge == null)
                {
                    return Vec2.NaN;
                }

                int n = 0;
                double cx = 0;
                double cy = 0;

                Edge e0 = Edge;
                Edge e = e0;
                do
                {
                    Vec4 v = e.NodeStart.Vertex;
                    cx += v.x;
                    cy += v.y;

                    e = e.Next;
                    n++;

                    if (n > 100)
                    {
                        return Vec2.NaN;
                    }
                } while (e != e0);

                if (n < 3)
                {
                    return Vec2.NaN;
                }

                cx /= n; 
                cy /= n;
                return new Vec2(cx, cy);
            }
        }

        public double AreaSigned
        {
            get
            {
                if (Edge == null)
                {
                    return double.NaN;
                }

                int n = 0;
                double area2 = 0;

                Edge e0 = Edge;
                Edge e = e0;
                do
                {
                    Vec4 a = e.NodeStart.Vertex;
                    Vec4 b = e.NodeEnd.Vertex;
                    area2 += a.x * b.y - a.y * b.x;
                    e = e.Next;
                    n++;

                    if (n > 100)
                    {
                        return double.NaN;
                    }
                } while (e != e0);

                if (n < 3)
                {
                    return Double.NaN;
                }
                return area2 * 0.5;
            }
        }

        public Circle CircumCircle
        {
            get
            {
                Vec4 a = Edge.NodeStart.Vertex;
                Vec4 b = Edge.Next.NodeStart.Vertex;
                Vec4 c = Edge.Prev.NodeStart.Vertex;
                return Circle.From3(
                    new Vec2(a.x, a.y), 
                    new Vec2(b.x, b.y), 
                    new Vec2(c.x, c.y));
            }
        }

        public void TransmitContextTo(Face other)
        {
            other.Invalid = Invalid;
            other.Payload = Payload;
            other.Kind = Kind;
        }
    }
}
