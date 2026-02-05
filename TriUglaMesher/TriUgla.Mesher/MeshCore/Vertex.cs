using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public struct Vertex(double x, double y, double z = 0, double w = 1)
    {
        public double x = x;
        public double y = y;
        public double z = z;
        public double w = w;

        public override string ToString()
            => $"{x:0.###} {y:0.###} {z:0.###} {w:0.###}";

        public readonly Vec2 AsVec2() => new Vec2(x, y);
    }
}
