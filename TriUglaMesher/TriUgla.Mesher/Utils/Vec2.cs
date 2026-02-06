using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TriUgla.Mesher.Utils
{
    public readonly struct Vec2(double x, double y)
    {
        public readonly double x = x;
        public readonly double y = y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Sub(in Vec2 a, in Vec2 b) => new Vec2(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(in Vec2 a, in Vec2 b) => a.x * b.x + a.y * b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cross(in Vec2 a, in Vec2 b) => a.x * b.y - a.y * b.x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cross(in Vec2 a, in Vec2 b, in Vec2 c)
        {
            double abx = b.x - a.x;
            double aby = b.y - a.y;
            double acx = c.x - a.x;
            double acy = c.y - a.y;
            return abx * acy - aby * acx;
        }
    }
}
