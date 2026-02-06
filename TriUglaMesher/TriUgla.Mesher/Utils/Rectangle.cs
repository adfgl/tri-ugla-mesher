using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TriUgla.Mesher.Utils
{
    public readonly struct Rectangle(double minX, double minY, double maxX, double maxY)
    {
        public readonly double minX = minX, minY = minY;
        public readonly double maxX = maxX, maxY = maxY;

        public bool IsEmpty()
        {
            return minX == double.MaxValue || minY == double.MaxValue || maxX == double.MinValue || maxY == double.MinValue;
        }

        public static readonly Rectangle Empty =
            new Rectangle(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(in Vec2 p)
            => p.x >= minX && p.x <= maxX &&
               p.y >= minY && p.y <= maxY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(in Rectangle r)
            => r.minX >= minX && r.maxX <= maxX &&
               r.minY >= minY && r.maxY <= maxY;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in Rectangle r)
            => !(r.minX > maxX || r.maxX < minX ||
                 r.minY > maxY || r.maxY < minY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vec2 Center()
            => new Vec2(
                (minX + maxX) * 0.5,
                (minY + maxY) * 0.5);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Rectangle Expand(double margin)
            => new Rectangle(
                minX - margin,
                minY - margin,
                maxX + margin,
                maxY + margin);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Rectangle Union(in Rectangle r)
            => new Rectangle(
                minX < r.minX ? minX : r.minX,
                minY < r.minY ? minY : r.minY,
                maxX > r.maxX ? maxX : r.maxX,
                maxY > r.maxY ? maxY : r.maxY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Rectangle Union(in Vec2 p)
            => new Rectangle(
                minX < p.x ? minX : p.x,
                minY < p.y ? minY : p.y,
                maxX > p.x ? maxX : p.x,
                maxY > p.y ? maxY : p.y);

    }
}
