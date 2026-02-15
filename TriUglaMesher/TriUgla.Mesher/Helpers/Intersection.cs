using TriUgla.Geometry;

namespace TriUgla.Mesher.Helpers
{
    public static class Intersection
    {
        public static bool Intersect(
            in Vec2 p1, in Vec2 p2,
            in Vec2 q1, in Vec2 q2,
            out Vec2 inter,
            double parallelEps = 1e-12,
            double uvEps = 1e-12)
        {
            inter = default;

            Vec2 r = p2 - p1;
            Vec2 s = q2 - q1;

            double den = Vec2.Cross(in r, in s);
            if (MathHelper.IsZero(den, parallelEps))
                return false;

            Vec2 qp = q1 - p1;

            double invDen = 1.0 / den;
            double u = Vec2.Cross(in qp, in s) * invDen;
            double v = Vec2.Cross(in qp, in r) * invDen;

            if (u < -uvEps || u > 1.0 + uvEps || v < -uvEps || v > 1.0 + uvEps)
                return false;

            u = u < 0.0 ? 0.0 : (u > 1.0 ? 1.0 : u);
            inter = new Vec2(p1.x + r.x * u, p1.y + r.y * u);
            return true;
        }
    }
}
