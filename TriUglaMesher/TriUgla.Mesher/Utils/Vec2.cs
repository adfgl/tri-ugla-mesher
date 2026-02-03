using System.Runtime.CompilerServices;

namespace TriUgla.Mesher.Utils
{
    public readonly struct Vec2(double x, double y)
    {
        public readonly double x = x;
        public readonly double y = y;

        public override string ToString() => $"{x:0.###} {y:0.###}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Length2(Vec2 a, Vec2 b) => Dot(Sub(a, b), Sub(a, b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Length(Vec2 a, Vec2 b) => Math.Sqrt(Length2(a, b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(Vec2 a, Vec2 b) => a.x * b.x + a.y * b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cross(Vec2 a, Vec2 b) => a.x * b.y - a.y * b.x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cross(Vec2 a, Vec2 b, Vec2 c) => Cross(Sub(b, a), Sub(c, a));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Sub(Vec2 a, Vec2 b) => new(a.x - b.x, a.y - b.y);
    }
}
