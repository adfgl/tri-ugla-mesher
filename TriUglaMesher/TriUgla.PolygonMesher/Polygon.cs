using TriUgla.Geometry;
using TriUgla.Mesher.Helpers;

namespace TriUgla.PolygonMesher
{
    public sealed class Polygon
    {
        readonly Rect _rect;
        readonly List<Edge> _edges;

        public string? Name { get; set; }

        public Polygon(List<PolygonVertex> vs, double eps)
        {
            _edges = new List<Edge>(vs.Count);

            List<Vec2> verts = new List<Vec2>(vs.Count);
            Rect rect = Rect.Empty;
            foreach (var v in vs)
            {
                if (!Duplicate(v, eps))
                {
                    verts.Add(v.pos);
                    AddEdge(new Edge(v));
                    rect = rect.Union(v.pos);
                }
            }
            double signedArea = SignedArea();
            Area = Math.Abs(signedArea);
            Clockwise = signedArea >= 0;
            Eps = eps;
            _rect = rect;
        }

        public int Count => _edges.Count;
        public PolygonVertex this[int i] => _edges[i].Start;

        public double Eps { get; set; }
        public double Area { get; }
        public bool Clockwise { get; private set; }
        public Rect Bounds => _rect;

        public Polygon EnforceClockwise()
        {
            if (!Clockwise) Flip();
            return this;
        }

        public Polygon EnforceCounterClockwise()
        {
            if (Clockwise) Flip();
            return this;
        }

        public Polygon Flip()
        {
            Clockwise = !Clockwise;
            foreach (Edge e in _edges)
            {
                e.Flip();
            }
            return this;
        }

        public bool Contains(Polygon other)
        {
            if (_rect.Contains(in other._rect))
            {
                bool contains = true;
                foreach (var item in other._edges)
                {
                    if (!Contains(item.Start.pos))
                    {
                        contains = false;
                        break;
                    }
                }
                return contains;
            }
            return false;
        }

        public bool Intersects(Polygon other)
        {
            if (_rect.Intersects(in other._rect))
            {
                foreach (Edge item1 in other._edges)
                {
                    foreach (var item2 in _edges)
                    {
                        if (item2.Intersect(item1, out _))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool Contains(in Vec2 v)
        {
            if (!_rect.Contains(v.x, v.y))
                return false;

            Vec2 rayStart = v;
            Vec2 rayEnd = new Vec2(v.y, v.x + (_rect.maxX - _rect.minX));

            int intersections = 0;
            Edge? ignore = null;
            double eps2 = Eps * Eps;
            foreach (var e in _edges)
            {
                if (e == ignore || !InBounds(e, in v))
                    continue;

                if (e.Contains(in v, Eps))
                    return true;

                if (!e.Intersect(in rayStart, in rayEnd, out Vec2 inter))
                    continue;

                intersections++;
                if (Vec2.DistanceSq(e.Start.pos, in inter) <= eps2)
                {
                    ignore = e.Prev;
                }
                else if (Vec2.DistanceSq(e.End.pos, in inter) <= eps2)
                {
                    ignore = e.Next;
                }
            }

            bool inside = intersections % 2 != 0;
            return inside;
        }

        bool InBounds(Edge e, in Vec2 v)
        {
            Vec2 start = e.Start.pos;
            Vec2 end = e.End.pos;
            Rect r = Rect.FromTwoPoints(start.x, start.y, end.x, end.y);
            return
                v.x <= r.maxX + Eps &&
                r.minY - Eps <= v.y && v.y <= r.maxY + Eps;
        }

        void AddEdge(Edge e)
        {
            Edge? last = _edges.Last();
            if (last is not null)
            {
                Link(last, e);
            }
            _edges.Add(e);
        }

        bool Duplicate(PolygonVertex v, double eps)
        {
            double eps2 = eps * eps;
            foreach (Edge item in _edges)
            {
                if (Vec2.DistanceSq(v.pos, item.Start.pos) <= eps2)
                    return true;
            }
            return false;
        }

        static void Link(Edge e0, Edge e1)
        {
            e0.Next = e1;
            e1.Prev = e0;
        }

        double SignedArea()
        {
            double sum = 0;
            foreach (Edge item in _edges)
            {
                Vec2 a = item.Start.pos;
                Vec2 b = item.End.pos;
                sum += (b.x - a.x) * (b.y + a.y);
            }
            return sum * 0.5;
        }

        class Edge(PolygonVertex pos)
        {
            public PolygonVertex Start { get; set; } = pos;
            public Edge Prev { get; set; } = null!;
            public Edge Next { get; set; } = null!;
            public PolygonVertex End => Next.Start;

            public bool Contains(in Vec2 v, double eps)
            {
                Vec2 a = Start.pos, b = End.pos;
                Vec2 ab = b - a;
                Vec2 av = v - a;

                double cross = Vec2.Cross(in ab, in av);
                double abLenSqr = ab.LengthSq;
                if (cross * cross > (eps * eps) * abLenSqr)
                    return false;

                double t = Vec2.Dot(in av, in ab);
                if (t < -eps) return false;
                if (t > abLenSqr + eps) return false;

                return true;
            }

            public bool Intersect(in Vec2 s, in Vec2 e, out Vec2 inter)
                => Intersection.Intersect(in s, in e, Start.pos, End.pos, out inter);

            public bool Intersect(Edge e, out Vec2 inter)
                => Intersect(e.Start.pos, e.End.pos, out inter);

            public void Flip()
            {
                Edge t = Prev;
                Prev = Next;
                Next = t;
            }
        }
    }
}
