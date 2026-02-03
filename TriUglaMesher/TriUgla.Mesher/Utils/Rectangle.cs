using System.Runtime.CompilerServices;

namespace TriUgla.Mesher.Utils
{
    public readonly struct Rectangle(double minX, double minY, double maxX, double maxY)
    {
        public readonly double minX = minX, minY = minY;
        public readonly double maxX = maxX, maxY = maxY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle FromTwoPoints(in Vec2 a, in Vec2 b)
        {
            double minX = a.x < b.x ? a.x : b.x;
            double maxX = a.x > b.x ? a.x : b.x;
            double minY = a.y < b.y ? a.y : b.y;
            double maxY = a.y > b.y ? a.y : b.y;
            return new Rectangle(minX, minY, maxX, maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle FromThreePoints(in Vec2 a, in Vec2 b, in Vec2 c)
        {
            double minX = a.x, maxX = a.x;
            double minY = a.y, maxY = a.y;

            double x = b.x; if (x < minX) minX = x; if (x > maxX) maxX = x;
            double y = b.y; if (y < minY) minY = y; if (y > maxY) maxY = y;

            x = c.x; if (x < minX) minX = x; if (x > maxX) maxX = x;
            y = c.y; if (y < minY) minY = y; if (y > maxY) maxY = y;

            return new Rectangle(minX, minY, maxX, maxY);
        }
    }
}
