using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.HalfEdge
{
    public class Vertex
    {
        public double X, Y, Z;

        public double DistanceTo(Vertex other)
        {
            double dx = other.X - X;
            double dy = other.Y - Y;
            double dz = other.Z - Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
         
        public Vec2 ToVec2() => new Vec2(X, Y);

        public override string ToString() 
            => $"{X:0.###} {Y:0.###} {Z:0.###}";
    }
}
