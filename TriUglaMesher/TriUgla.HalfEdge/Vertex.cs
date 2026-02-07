namespace TriUgla.HalfEdge
{
    public readonly struct Vertex(double x, double y, double z, double w = 1)
    {
        public readonly double x = x, y = y, z = z, w = w;

        public override string ToString()
            => $"{x:0.###} {y:0.###} {z:0.###}";
    }
}
