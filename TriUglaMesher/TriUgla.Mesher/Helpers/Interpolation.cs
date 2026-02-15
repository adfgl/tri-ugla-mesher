using TriUgla.Geometry;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Services
{
    public static class Interpolation
    {
        public static Vec4 Interpolate(Vec2 at, Node node)
            => Interpolate(at, node.Vertex);

        public static Vec4 Interpolate(Vec2 at, Edge edge)
            => Interpolate(at, edge.NodeStart.Vertex, edge.NodeEnd.Vertex);

        public static Vec4 Interpolate(Vec2 at, Face face)
            => Interpolate(at,
                face.Edge.NodeStart.Vertex,
                face.Edge.Next.NodeStart.Vertex,
                face.Edge.Prev.NodeStart.Vertex);

        public static Vec4 Interpolate(Vec2 at, Vec4 a)
        {
            return a;
        }

        public static Vec4 Interpolate(Vec2 at, Vec4 a, Vec4 b)
        {
            double t = Project01(at, a.AsVec2(), b.AsVec2());
            return new Vec4(at.x, at.y, Lerp(a.z, b.z, t));
        }

        public static Vec4 Interpolate(Vec2 at, Vec4 a, Vec4 b, Vec4 c)
        {
            Barycentric(at, a.AsVec2(), b.AsVec2(), c.AsVec2(), 
                out double wa, out double wb, out double wc);
            return new Vec4(at.x, at.y, wa * a.z + wb * b.z + wc * c.z);
        }

        static Vec2 AsVec2(this Vec4 v) => new Vec2(v.x, v.y);

        static double Lerp(double a, double b, double t) => a + (b - a) * t;

        static double Project01(Vec2 p, Vec2 a, Vec2 b)
        {
            Vec2 ab = b - a;
            double t = Vec2.Dot(p - a, ab) / Vec2.Dot(ab, ab);
            return Math.Clamp(t, 0.0, 1.0);
        }

        static void Barycentric(
         Vec2 p, Vec2 a, Vec2 b, Vec2 c,
         out double wa, out double wb, out double wc)
        {
            Vec2 v0 = b - a;
            Vec2 v1 = c - a;
            Vec2 v2 = p - a;

            double d00 = Vec2.Dot(v0, v0);
            double d01 = Vec2.Dot(v0, v1);
            double d11 = Vec2.Dot(v1, v1);
            double d20 = Vec2.Dot(v2, v0);
            double d21 = Vec2.Dot(v2, v1);

            double denom = d00 * d11 - d01 * d01;
            if (Math.Abs(denom) < 1e-14)
            {
                wa = wb = wc = 1.0 / 3.0;
                return;
            }

            wb = (d11 * d20 - d01 * d21) / denom;
            wc = (d00 * d21 - d01 * d20) / denom;
            wa = 1.0 - wb - wc;
        }
    }
}
