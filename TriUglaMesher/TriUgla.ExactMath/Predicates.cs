namespace TriUgla.ExactMath
{
    public class Predicates
    {
        const double U = 1.1102230246251565e-16;

        public bool EnforcePredicates { get; set; } = false;
        public bool AllowExactMath { get; set; } = true;
        public int ExactInCircleComputations { get; private set; }
        public int ExactOrientComputations { get; private set; }
        public int ExactPlaneOrientationComputations { get; private set; }

        bool UseExact => AllowExactMath || EnforcePredicates;

        public int PlaneSide(
            double ax, double ay, double az,
            double bx, double by, double bz,
            double cx, double cy, double cz,
            double dx, double dy, double dz)
        {
            double bax = bx - ax;
            double bay = by - ay;
            double baz = bz - az;

            double cax = cx - ax;
            double cay = cy - ay;
            double caz = cz - az;

            double dax = dx - ax;
            double day = dy - ay;
            double daz = dz - az;

            // normal = (B-A) x (C-A)
            double nx = bay * caz - baz * cay;
            double ny = baz * cax - bax * caz;
            double nz = bax * cay - bay * cax;

            // dot(normal, D-A)
            double p1 = nx * dax;
            double p2 = ny * day;
            double p3 = nz * daz;
            double det = p1 + p2 + p3;

            double errBound =
                32.0 * U * (
                    Math.Abs(p1) +
                    Math.Abs(p2) +
                    Math.Abs(p3));

            if (det > errBound) return +1;   // in front
            if (det < -errBound) return -1;  // behind

            if (UseExact)
                return PlaneSideExact(
                    ax, ay, az,
                    bx, by, bz,
                    cx, cy, cz,
                    dx, dy, dz);

            return det > 0 ? +1 : det < 0 ? -1 : 0;
        }

        public int PlaneSideExact(
            double ax, double ay, double az,
            double bx, double by, double bz,
            double cx, double cy, double cz,
            double dx, double dy, double dz)
        {
            ExactPlaneOrientationComputations++;

            double bax = bx - ax;
            double bay = by - ay;
            double baz = bz - az;

            double cax = cx - ax;
            double cay = cy - ay;
            double caz = cz - az;

            double dax = dx - ax;
            double day = dy - ay;
            double daz = dz - az;

            // We'll accumulate the determinant as an expansion
            List<double> e = new List<double>(32);

            // nx = bay*caz - baz*cay
            Expansion.TwoProd(bay, caz, out double h1, out double l1);
            Expansion.Add(e, l1);
            Expansion.Add(e, h1);

            Expansion.TwoProd(baz, cay, out double h2, out double l2);
            Expansion.Add(e, -l2);
            Expansion.Add(e, -h2);

            // p1 = nx * dax
            Expansion.Mul(e, dax); // reuse your scalar Mul

            // ny = baz*cax - bax*caz
            List<double> ey = new List<double>(8);
            Expansion.TwoProd(baz, cax, out h1, out l1);
            Expansion.Add(ey, l1);
            Expansion.Add(ey, h1);

            Expansion.TwoProd(bax, caz, out h2, out l2);
            Expansion.Add(ey, -l2);
            Expansion.Add(ey, -h2);

            Expansion.Mul(ey, day);
            Expansion.Add(e, ey);

            // nz = bax*cay - bay*cax
            List<double> ez = new List<double>(8);
            Expansion.TwoProd(bax, cay, out h1, out l1);
            Expansion.Add(ez, l1);
            Expansion.Add(ez, h1);

            Expansion.TwoProd(bay, cax, out h2, out l2);
            Expansion.Add(ez, -l2);
            Expansion.Add(ez, -h2);

            Expansion.Mul(ez, daz);
            Expansion.Add(e, ez);

            Expansion.Compress(e);
            return Expansion.Sign(e);
        }


        public bool Convex(
            double ax, double ay,
            double bx, double by,
            double cx, double cy,
            double dx, double dy)
        {
            int s1 = Orient(dx, dy, ax, ay, bx, by);
            if (s1 == 0) return false;

            int s2 = Orient(bx, by, cx, cy, dx, dy); 
            if (s2 == 0 || s2 != s1) return false;

            int s3 = Orient(cx, cy, dx, dy, ax, ay); 
            if (s3 == 0 || s3 != s1) return false;

            int s4 = Orient(dx, dy, ax, ay, bx, by); 
            if (s4 == 0 || s4 != s1) return false;

            return true;
        }

        public int InCircleDiameter(
            double ax, double ay,
            double bx, double by,
            double dx, double dy)
        {
            // dot = (dx-ax, dy-ay) x (dx-bx, dy-by)
            double dax = dx - ax;
            double day = dy - ay;
            double dbx = dx - bx;
            double dby = dy - by;

            double p1 = dax * dbx;
            double p2 = day * dby;
            double dot = p1 + p2;

            double errBound = 16.0 * U * (Math.Abs(p1) + Math.Abs(p2));

            if (dot > errBound) return -1;   
            if (dot < -errBound) return +1;  

            if (UseExact)
                return InCircleDiameterExact(ax, ay, bx, by, dx, dy);

            return dot < 0 ? +1 : dot > 0 ? -1 : 0;
        }

        public int InCircleDiameterExact(
            double ax, double ay,
            double bx, double by,
            double dx, double dy)
        {
            ExactInCircleComputations++;

            double dax = dx - ax;
            double day = dy - ay;
            double dbx = dx - bx;
            double dby = dy - by;

            // dot = dax*dbx + day*dby
            List<double> e = new List<double>(8);

            Expansion.TwoProd(dax, dbx, out double h1, out double l1);
            Expansion.Add(e, l1);
            Expansion.Add(e, h1);

            Expansion.TwoProd(day, dby, out double h2, out double l2);
            Expansion.Add(e, l2);
            Expansion.Add(e, h2);

            Expansion.Compress(e);
            int s = Expansion.Sign(e);
            return s < 0 ? +1 : s > 0 ? -1 : 0;
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
            if (UseExact)
            {
                return OrientExact(ax, ay, bx, by, cx, cy);
            }
            return cross > 0 ? +1 : cross < 0 ? -1 : 0;
        }

        public int OrientExact(
           double ax, double ay,
           double bx, double by,
           double cx, double cy)
        {
            ExactOrientComputations++;

            // (bx-ax)*(cy-ay) - (by-ay)*(cx-ax)
            double baDx = bx - ax;
            double baDy = by - ay;
            double caDx = cx - ax;
            double caDy = cy - ay;

            // cross = baDx*caDy - baDy*caDx
            List<double> e = new List<double>(8);

            // t1 = baDx * caDy
            Expansion.TwoProd(baDx, caDy, out double h1, out double l1);
            Expansion.Add(e, l1);
            Expansion.Add(e, h1);

            // t2 = baDy * caDx
            Expansion.TwoProd(baDy, caDx, out double h2, out double l2);
            Expansion.Add(e, -l2);
            Expansion.Add(e, -h2);

            Expansion.Compress(e);
            return Expansion.Sign(e);
        }

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

            if (UseExact)
            {
                return InCircleExact(ax, ay, bx, by, cx, cy, dx, dy, o);
            }
            return sdet > 0 ? +1 : sdet < 0 ? -1 : 0;
        }

        public int InCircleExact(
            double ax, double ay,
            double bx, double by,
            double cx, double cy,
            double dx, double dy,
            int o)
        {
            ExactInCircleComputations++;

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
            Expansion.Mul(termA, abdet);

            var termB = new List<double>(blift);
            Expansion.Mul(termB, bcdet);

            var termC = new List<double>(clift);
            Expansion.Mul(termC, cadet);

            Expansion.Add(termA, termB);
            Expansion.Add(termA, termC);

            Expansion.Compress(termA);

            int s = Expansion.Sign(termA);
            return s * o;
        }

        static List<double> SquareSum(double x, double y)
        {
            // returns expansion for x*x + y*y
            var e = new List<double>(8);

            Expansion.TwoProd(x, x, out double hx, out double lx);
            Expansion.Add(e, lx);
            Expansion.Add(e, hx);

            Expansion.TwoProd(y, y, out double hy, out double ly);
            Expansion.Add(e, ly);
            Expansion.Add(e, hy);

            Expansion.Compress(e);
            return e;
        }

        static List<double> TwoProdDiff(double a, double b, double c, double d)
        {
            // returns expansion for (a*b) - (c*d)
            var e = new List<double>(8);

            Expansion.TwoProd(a, b, out double h1, out double l1);
            Expansion.Add(e, l1);
            Expansion.Add(e, h1);

            Expansion.TwoProd(c, d, out double h2, out double l2);
            Expansion.Add(e, -l2);
            Expansion.Add(e, -h2);

            Expansion.Compress(e);
            return e;
        }
    }
}
