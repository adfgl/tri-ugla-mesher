using System.Runtime.CompilerServices;
using TriUgla.Mesher.HalfEdge;
using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.Topology
{
    public sealed class Predicates(double snapDist = 1e-8)
    {
        const double U = 1.1102230246251565e-16;

        double _snapDist = snapDist;
        double _snapDist2 = snapDist * snapDist;

        public double SnapDist
        {
            get => _snapDist;
            set
            {
                _snapDist = value;
                _snapDist2 = value * value;
            }
        }

        public bool AllowExactMath { get; set; } = true;
        public int UsedExactForInCircle { get; private set; }
        public int UsedExactForOrient { get; private set; }

        public int OrientAprox(Vec2 a, Vec2 b, Vec2 p)
        {
            Vec2 ab = Vec2.Sub(b, a);
            Vec2 ap = Vec2.Sub(p, a);

            double len2 = Vec2.Dot(in ab, in ab);
            if (len2 == 0) return 0;

            double cross = Vec2.Cross(in ab, in ap);

            double tol = _snapDist2 * len2;
            double cross2 = cross * cross;

            if (cross2 <= tol) return 0;
            return cross > 0 ? +1 : -1;
        }

        public bool Convex(Vec2 a, Vec2 b, Vec2 c, Vec2 d)
        {
            return
                Orient(d, a, b) == 1 &&
                Orient(a, b, c) == 1 &&
                Orient(b, c, d) == 1 &&
                Orient(d, d, a) == 1;
        }

        public int Orient(Vec2 a, Vec2 b, Vec2 c)
            => Orient(a.x, a.y, b.x, b.y, c.x, c.y);

        public int Orient(Edge e, Vec2 c)
           => Orient(e.NodeStart.Vertex.ToVec2(), e.NodeStart.Vertex, c);

        public bool Close(Vec2 a, Vec2 b)
        {
            double dx = a.x - b.x;
            double dy = a.y - b.y;
            return dx * dx + dy * dy <= _snapDist2;
        }

        public Edge? EntranceEdge(Node start, Node end)
        {
            Edge e0 = start.Edge;
            Edge e1 = e0;
            do
            {
                if (e1.NodeEnd == end)
                {
                    return e1;
                }

                Edge e2 = e1.Prev;
                int o1 = Orient(e0, end.Vertex);
                int o2 = Orient(e1, end.Vertex);
                if (o1 == 0 && o2 == 1 || o1 == 1 && o2 == 1)
                    return e1;

                if (o1 == 1 && o2 == 0)
                    return e2.Twin;

                if (e2.Twin is null)
                {
                    break;
                }
                e1 = e2.Twin;

            } while (e0 != e1);

            return null;
        }

     

        public int InCircle(Vertex a, Vertex b, Vertex c, Vertex d)
            => InCircle(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y);

        public int InCircle(
           double ax, double ay,
           double bx, double by,
           double cx, double cy,
           double dx, double dy)
        {
            int o = Orient(ax, ay, bx, by, cx, cy);
            if (o == 0) return 0;

            double adx = ax - dx, ady = ay - dy;
            double bdx = bx - dx, bdy = by - dy;
            double cdx = cx - dx, cdy = cy - dy;

            double alift = adx * adx + ady * ady;
            double blift = bdx * bdx + bdy * bdy;
            double clift = cdx * cdx + cdy * cdy;

            double abdet = bdx * cdy - bdy * cdx;
            double bcdet = cdx * ady - cdy * adx;
            double cadet = adx * bdy - ady * bdx;

            double det = alift * abdet + blift * bcdet + clift * cadet;

            double err =
                64.0 * U * (
                    Math.Abs(alift * abdet) +
                    Math.Abs(blift * bcdet) +
                    Math.Abs(clift * cadet)
                );

            double sdet = det * o;
            if (sdet > err) return +1;
            if (sdet < -err) return -1;

            if (!AllowExactMath)
                return sdet > 0 ? +1 : sdet < 0 ? -1 : 0;

            UsedExactForInCircle++;
            return InCircleExact(ax, ay, bx, by, cx, cy, dx, dy, o);
        }

        public static int InCircleExact(
            double ax, double ay,
            double bx, double by,
            double cx, double cy,
            double dx, double dy,
            int o)
        {
            double adx = ax - dx, ady = ay - dy;
            double bdx = bx - dx, bdy = by - dy;
            double cdx = cx - dx, cdy = cy - dy;

            // alift = adx^2 + ady^2 (as expansion)
            var alift = SquareSum(adx, ady);
            var blift = SquareSum(bdx, bdy);
            var clift = SquareSum(cdx, cdy);

            // abdet = bdx*cdy - bdy*cdx (as expansion)
            var abdet = TwoProdDiff(bdx, cdy, bdy, cdx);
            var bcdet = TwoProdDiff(cdx, ady, cdy, adx);
            var cadet = TwoProdDiff(adx, bdy, ady, bdx);

            // det = alift*abdet + blift*bcdet + clift*cadet
            var termA = new List<double>(alift);
            ExpansionMath.Mul(termA, abdet);

            var termB = new List<double>(blift);
            ExpansionMath.Mul(termB, bcdet);

            var termC = new List<double>(clift);
            ExpansionMath.Mul(termC, cadet);

            ExpansionMath.Add(termA, termB);
            ExpansionMath.Add(termA, termC);

            ExpansionMath.Compress(termA);

            int s = ExpansionMath.Sign(termA);
            return s * o;
        }

        static List<double> SquareSum(double x, double y)
        {
            // returns expansion for x*x + y*y
            var e = new List<double>(8);

            ExpansionMath.TwoProd(x, x, out double hx, out double lx);
            ExpansionMath.Add(e, lx);
            ExpansionMath.Add(e, hx);

            ExpansionMath.TwoProd(y, y, out double hy, out double ly);
            ExpansionMath.Add(e, ly);
            ExpansionMath.Add(e, hy);

            ExpansionMath.Compress(e);
            return e;
        }

        static List<double> TwoProdDiff(double a, double b, double c, double d)
        {
            // returns expansion for (a*b) - (c*d)
            var e = new List<double>(8);

            ExpansionMath.TwoProd(a, b, out double h1, out double l1);
            ExpansionMath.Add(e, l1);
            ExpansionMath.Add(e, h1);

            ExpansionMath.TwoProd(c, d, out double h2, out double l2);
            ExpansionMath.Add(e, -l2);
            ExpansionMath.Add(e, -h2);

            ExpansionMath.Compress(e);
            return e;
        }

        public int Orient(
            double ax, double ay,
            double bx, double by, 
            double cx, double cy)
        {
            double baDx = bx - ax;
            double baDy = by - ay;
            double caDx = cx - ax;
            double caDy = cy - ay;

            double p1 = baDx * caDy;
            double p2 = baDy * caDx;
            double cross = p1 - p2;

            double errBound = 16.0 * U * (Math.Abs(p1) + Math.Abs(p2));

            if (cross > errBound) return +1;
            if (cross < -errBound) return -1;
            if (AllowExactMath)
            {
                UsedExactForOrient++;
                return OrientExact(ax, ay, bx, by, cx, cy);
            }
            return cross > 0 ? +1 : cross < 0 ? -1 : 0;
        }

        public int OrientExact(
            double ax, double ay, 
            double bx, double by, 
            double cx, double cy)
        {
            // (bx-ax)*(cy-ay) - (by-ay)*(cx-ax)
            double baDx = bx - ax;
            double baDy = by - ay;
            double caDx = cx - ax;
            double caDy = cy - ay;

            // cross = baDx*caDy - baDy*caDx
            List<double> e = new List<double>(8);

            // t1 = baDx * caDy
            ExpansionMath.TwoProd(baDx, caDy, out double h1, out double l1);
            ExpansionMath.Add(e, l1);
            ExpansionMath.Add(e, h1);

            // t2 = baDy * caDx
            ExpansionMath.TwoProd(baDy, caDx, out double h2, out double l2);
            ExpansionMath.Add(e, -l2);
            ExpansionMath.Add(e, -h2);

            // Make sign reliable
            ExpansionMath.Compress(e);
            return ExpansionMath.Sign(e);
        }
    }
}
