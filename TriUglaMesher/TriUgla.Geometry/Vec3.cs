using System.Runtime.CompilerServices;

namespace TriUgla.Geometry
{
    public readonly struct Vec3(double x, double y, double w = 1.0) : IEquatable<Vec3>
    {
        public readonly double x = x, y = y, w = w;

        public static Vec3 Zero => new Vec3(0, 0);
        public static Vec3 One => new Vec3(1, 1);
        public static Vec3 UnitX => new Vec3(1, 0);
        public static Vec3 UnitY => new Vec3(0, 1);

        public double this[int i] => i switch
        {
            0 => x,
            1 => y,
            2 => w,
            _ => throw new IndexOutOfRangeException()
        };

        public double LengthSq
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => x * x + y * y + w * w;
        }

        public double Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Math.Sqrt(LengthSq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFinite()
            => double.IsFinite(x) && double.IsFinite(y) && double.IsFinite(w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(in Vec3 a, in Vec3 b)
            => a.x * b.x + a.y * b.y + a.w * b.w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Cross(in Vec3 a, in Vec3 b)
            => new Vec3(
                a.y * b.w - a.w * b.y,
                a.w * b.x - a.x * b.w,
                a.x * b.y - a.y * b.x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec3 Normalize()
        {
            double len = Length;
            if (len == 0.0) return Zero;
            double inv = 1.0 / len;
            return new Vec3(x * inv, y * inv, w * inv);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryNormalize(out Vec3 unit)
        {
            double lenSq = LengthSq;
            if (lenSq == 0.0)
            {
                unit = default;
                return false;
            }

            double inv = 1.0 / Math.Sqrt(lenSq);
            unit = new Vec3(x * inv, y * inv, w * inv);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceSq(in Vec3 a, in Vec3 b)
        {
            double dx = a.x - b.x;
            double dy = a.y - b.y;
            double dz = a.w - b.w;
            return dx * dx + dy * dy + dz * dz;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(in Vec3 a, in Vec3 b)
            => Math.Sqrt(DistanceSq(a, b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Min(in Vec3 a, in Vec3 b)
            => new Vec3(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.w, b.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Max(in Vec3 a, in Vec3 b)
            => new Vec3(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.w, b.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Abs(in Vec3 v)
            => new Vec3(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Lerp(in Vec3 a, in Vec3 b, double t)
            => new Vec3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.w + (b.w - a.w) * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator +(Vec3 a, Vec3 b)
            => new Vec3(a.x + b.x, a.y + b.y, a.w + b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator -(Vec3 a, Vec3 b)
            => new Vec3(a.x - b.x, a.y - b.y, a.w - b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator -(Vec3 v)
            => new Vec3(-v.x, -v.y, -v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator *(Vec3 v, double s)
            => new Vec3(v.x * s, v.y * s, v.w * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator *(double s, Vec3 v)
            => new Vec3(v.x * s, v.y * s, v.w * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator /(Vec3 v, double s)
            => new Vec3(v.x / s, v.y / s, v.w / s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator *(Vec3 a, Vec3 b)
            => new Vec3(a.x * b.x, a.y * b.y, a.w * b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator /(Vec3 a, Vec3 b)
            => new Vec3(a.x / b.x, a.y / b.y, a.w / b.w);

        public static bool operator ==(Vec3 a, Vec3 b)
            => a.x == b.x && a.y == b.y && a.w == b.w;

        public static bool operator !=(Vec3 a, Vec3 b)
            => !(a == b);

        public bool Equals(Vec3 other)
            => x.Equals(other.x) && y.Equals(other.y) && w.Equals(other.w);

        public override bool Equals(object? obj)
            => obj is Vec3 v && Equals(v);

        public override int GetHashCode()
            => HashCode.Combine(x, y, w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AlmostEquals(in Vec3 other, double eps = 1e-12)
            => Math.Abs(x - other.x) <= eps &&
               Math.Abs(y - other.y) <= eps &&
               Math.Abs(w - other.w) <= eps;

        public override string ToString()
            => $"{x:0.###} {y:0.###} {w:0.###}";
    }
}