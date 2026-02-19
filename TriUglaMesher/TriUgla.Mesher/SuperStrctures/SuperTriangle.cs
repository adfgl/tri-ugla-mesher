using TriUgla.Geometry;

namespace TriUgla.Mesher.SuperStrctures
{
    public sealed class SuperTriangleStructure(double expand = 1.1) : SuperGeometryBase(nodeCapacity: 3, faceCapacity: 1)
    {
        readonly double _expand = expand;

        protected override List<Vec4> BuildRing(in Rect b)
        {
            double dx = b.maxX - b.minX;
            double dy = b.maxY - b.minY;
            double size = Math.Max(dx, dy) * _expand;

            return new List<Vec4>(3)
            {
                new Vec4(-size, -size, 0),
                new Vec4(+size, -size, 0),
                new Vec4(0, +size, 0),
            };
        }
    }
}
