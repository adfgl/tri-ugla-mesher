using System.Runtime.CompilerServices;

namespace TriUgla.Geometry
{
    public readonly struct Vec2(double x, double y) : IEquatable<Vec2>
    {
        public readonly double x = x, y = y;

        public static Vec2 Zero => new Vec2(0, 0);
        public static Vec2 One => new Vec2(1, 1);
        public static Vec2 UnitX => new Vec2(1, 0);
        public static Vec2 UnitY => new Vec2(0, 1);

        public double this[int index] => index switch
        {
            0 => x,
            1 => y,
            _ => throw new IndexOutOfRangeException("Vec2 index must be 0 or 1."),
        };

        public double LengthSq
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => x * x + y * y;
        }

        public double Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Math.Sqrt(LengthSq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFinite()
            => double.IsFinite(x) && double.IsFinite(y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(in Vec2 a, in Vec2 b)
            => a.x * b.x + a.y * b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cross(in Vec2 a, in Vec2 b)
            => a.x * b.y - a.y * b.x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 PerpLeft(in Vec2 v) => new Vec2(-v.y, v.x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 PerpRight(in Vec2 v) => new Vec2(v.y, -v.x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec2 Normalize()
        {
            double len = Length;
            if (len == 0.0) return Zero;
            double inv = 1.0 / len;
            return new Vec2(x * inv, y * inv);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryNormalize(out Vec2 unit)
        {
            double lenSq = LengthSq;
            if (lenSq == 0.0)
            {
                unit = default;
                return false;
            }

            double inv = 1.0 / Math.Sqrt(lenSq);
            unit = new Vec2(x * inv, y * inv);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Lerp(in Vec2 a, in Vec2 b, double t)
            => new Vec2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceSq(in Vec2 a, in Vec2 b)
        {
            double dx = a.x - b.x;
            double dy = a.y - b.y;
            return dx * dx + dy * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(in Vec2 a, in Vec2 b)
            => Math.Sqrt(DistanceSq(a, b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Min(in Vec2 a, in Vec2 b)
            => new Vec2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Max(in Vec2 a, in Vec2 b)
            => new Vec2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Abs(in Vec2 v)
            => new Vec2(Math.Abs(v.x), Math.Abs(v.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.x + b.x, a.y + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator -(Vec2 v) => new Vec2(-v.x, -v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator *(Vec2 v, double s) => new Vec2(v.x * s, v.y * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator *(double s, Vec2 v) => new Vec2(v.x * s, v.y * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator /(Vec2 v, double s) => new Vec2(v.x / s, v.y / s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator *(Vec2 a, Vec2 b) => new Vec2(a.x * b.x, a.y * b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator /(Vec2 a, Vec2 b) => new Vec2(a.x / b.x, a.y / b.y);

        public static bool operator ==(Vec2 a, Vec2 b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Vec2 a, Vec2 b) => !(a == b);

        public bool Equals(Vec2 other) => x.Equals(other.x) && y.Equals(other.y);

        public override bool Equals(object? obj) => obj is Vec2 v && Equals(v);

        public override int GetHashCode() => HashCode.Combine(x, y);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AlmostEquals(in Vec2 other, double eps = 1e-12)
            => Math.Abs(x - other.x) <= eps && Math.Abs(y - other.y) <= eps;

        public override string ToString() => $"{x:0.###} {y:0.###}";
    }
}