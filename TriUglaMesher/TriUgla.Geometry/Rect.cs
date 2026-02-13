using System.Runtime.CompilerServices;

namespace TriUgla.Geometry
{
    public readonly struct Rect(double minX, double minY, double maxX, double maxY)
    {
        public readonly double minX = minX, minY = minY;
        public readonly double maxX = maxX, maxY = maxY;

        public bool IsEmpty()
        {
            return minX == double.MaxValue || minY == double.MaxValue || maxX == double.MinValue || maxY == double.MinValue;
        }

        public static readonly Rect Empty =
            new Rect(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect FromTwoPoints(double minX, double minY, double maxX, double maxY)
        {
            if (minX > maxX) (minX, maxX) = (maxX, minX);
            if (minY > maxY) (minY, maxY) = (maxY, minY);
            return new Rect(minX, minY, maxX, maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(double x, double y)
            => x >= minX && x <= maxX &&
               y >= minY && y <= maxY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(in Rect r)
            => r.minX >= minX && r.maxX <= maxX &&
               r.minY >= minY && r.maxY <= maxY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in Rect r)
            => !(r.minX > maxX || r.maxX < minX ||
                 r.minY > maxY || r.maxY < minY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vec2 Center()
            => new Vec2(
                (minX + maxX) * 0.5,
                (minY + maxY) * 0.5);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Rect Expand(double margin)
            => new Rect(
                minX - margin,
                minY - margin,
                maxX + margin,
                maxY + margin);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Rect Union(in Rect r)
            => new Rect(
                minX < r.minX ? minX : r.minX,
                minY < r.minY ? minY : r.minY,
                maxX > r.maxX ? maxX : r.maxX,
                maxY > r.maxY ? maxY : r.maxY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Rect Union(in Vec2 p)
            => new Rect(
                minX < p.x ? minX : p.x,
                minY < p.y ? minY : p.y,
                maxX > p.x ? maxX : p.x,
                maxY > p.y ? maxY : p.y);
    }
}
