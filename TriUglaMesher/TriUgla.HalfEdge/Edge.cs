using TriUgla.Geometry;

namespace TriUgla.HalfEdge
{
    public sealed class Edge : MeshElement
    {
        int _constraints = 0;
        int _features = 0;
        int _contours = 0;
        int _holes = 0;

        public Node NodeStart { get; internal set; } = null!;
        public Face Face { get; internal set; } = null!;
        public Edge Next { get; internal set; } = null!;
        public Edge Prev { get; internal set; } = null!;
        public Edge? Twin { get; internal set; }

        public Node NodeEnd => Next.NodeStart;
        public Node Opposite => Prev.NodeStart;

        public double CrossingPrice { get; set; }

        public double AngleRad
        {
            get
            {
                Vec4 a = Prev.NodeStart.Vertex;
                Vec4 b = NodeStart.Vertex;
                Vec4 c = NodeEnd.Vertex;

                double bax = a.x - b.x;
                double bay = a.y - b.y;

                double bcx = c.x - b.x;
                double bcy = c.y - b.y;

                double dot = bax * bcx + bay * bcy;
                double cross = bax * bcy - bay * bcx;
                return Math.Atan2(Math.Abs(cross), dot);
            }
        }

        public double AngleDeg => Double.RadiansToDegrees(AngleRad);

        public double Length
        {
            get
            {
                Vec4 start = NodeStart.Vertex;
                Vec4 end = NodeEnd.Vertex;
                double dx = start.x - end.x;    
                double dy = start.y - end.y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }

        public int Constraints => _constraints;
        public bool Constrained => _constraints > 0;
        public int Features => _features;
        public int Contours => _contours;
        public int Holes => _holes;

        public void TransmitContextTo(Edge other)
        {
            other.Invalid = Invalid;
            other.Payload = Payload;
            other._constraints = _constraints;
            other._features = _features;
            other._contours = _contours;
            other._holes = _holes;
        }

        public void Constrain(EdgeKind kind)
        {
            switch (kind)
            {
                case EdgeKind.Feature:
                    Constrain(ref _features);
                    break;

                case EdgeKind.Contour:
                    Constrain(ref _contours);
                    break;

                case EdgeKind.Hole:
                    Constrain(ref _holes);
                    break;

                default:
                    throw new NotImplementedException(nameof(kind));
            }
        }

        public bool Release(EdgeKind kind)
        {
            return kind switch
            {
                EdgeKind.Feature => Release(ref _features),
                EdgeKind.Contour => Release(ref _contours),
                EdgeKind.Hole => Release(ref _holes),
                _ => throw new NotImplementedException(nameof(kind)),
            };
        }

        void Constrain(ref int count)
        {
            count++;
            _constraints++;
            NodeStart.Constrain();
            NodeEnd.Constrain();
        }

        bool Release(ref int count)
        {
            if (count == 0) return false;
            count--;
            _constraints--;
            NodeStart.Release();
            NodeEnd.Release();
            return true;
        }

    }
}
