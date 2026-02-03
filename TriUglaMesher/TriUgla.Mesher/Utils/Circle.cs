using System.Runtime.CompilerServices;

namespace TriUgla.Mesher.Utils
{
    public readonly struct Circle(Vec2 center, double radius2)
    {
        public readonly Vec2 center = center;
        public readonly double radius2 = radius2;

        public readonly bool Contains(in Vec2 v)
        {
            double dx = v.x - center.x;
            double dy = v.y - center.y;
            double d2 = dx * dx + dy * dy;
             
            double r2 = radius2;
            double eps = 1e-14 * (r2 + 1.0);
            return d2 < r2 - eps;
        }

        public static Circle From2(Vec2 a, Vec2 b)
        {
            double mx = (a.x + b.x) * 0.5;
            double my = (a.y + b.y) * 0.5;

            double dx = a.x - b.x;
            double dy = a.y - b.y;
            return new Circle(
                new Vec2(mx, my),
                (dx * dx + dy * dy) * 0.25);
        }

        public static Circle From3(Vec2 v1, Vec2 v2, Vec2 v3)
        {
            double dx13 = v1.x - v3.x;
            double dy13 = v1.y - v3.y;
            double dx23 = v2.x - v3.x;
            double dy23 = v2.y - v3.y;

            double s1 = -(dx13 * dx13 + dy13 * dy13);
            double s2 = -(dx23 * dx23 + dy23 * dy23);

            double det = Det2x2(dx13, dy13, dx23, dy23);
            double a = Det2x2(s1, dy13, s2, dy23) / det;
            double b = Det2x2(dx13, s1, dx23, s2) / det;

            double cx = -a * 0.5 + v3.x;
            double cy = -b * 0.5 + v3.y;

            double dx = cx - v1.x;
            double dy = cy - v1.y;
            double r2 = dx * dx + dy * dy;
            return new Circle(new Vec2(cx, cy), r2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Det2x2(
            double m11, double m12,
            double m21, double m22)
        {
            return m11 * m22 - m12 * m21;
        }
    }
}
