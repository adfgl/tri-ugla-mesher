using System.Runtime.CompilerServices;

namespace TriUgla.Geometry
{
    public readonly struct Vec4(double x, double y, double z, double w = 1.0) : IEquatable<Vec4>
    {
        public readonly double x = x, y = y, z = z, w = w;

        public static Vec4 Zero => new Vec4(0, 0, 0);
        public static Vec4 One => new Vec4(1, 1, 1);
        public static Vec4 UnitX => new Vec4(1, 0, 0);
        public static Vec4 UnitY => new Vec4(0, 1, 0);
        public static Vec4 UnitZ => new Vec4(0, 0, 1);
        public static Vec4 UnitW => new Vec4(0, 0, 0);

        public double this[int i] => i switch
        {
            0 => x,
            1 => y,
            2 => z,
            3 => w,
            _ => throw new IndexOutOfRangeException()
        };

        public double LengthSq
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => x * x + y * y + z * z + w * w;
        }

        public double Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Math.Sqrt(LengthSq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFinite()
            => double.IsFinite(x) && double.IsFinite(y) &&
               double.IsFinite(z) && double.IsFinite(w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(in Vec4 a, in Vec4 b)
            => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec4 Normalize()
        {
            double len = Length;
            if (len == 0.0) return Zero;
            double inv = 1.0 / len;
            return new Vec4(x * inv, y * inv, z * inv, w * inv);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryNormalize(out Vec4 unit)
        {
            double lenSq = LengthSq;
            if (lenSq == 0.0)
            {
                unit = default;
                return false;
            }

            double inv = 1.0 / Math.Sqrt(lenSq);
            unit = new Vec4(x * inv, y * inv, z * inv, w * inv);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceSq(in Vec4 a, in Vec4 b)
        {
            double dx = a.x - b.x;
            double dy = a.y - b.y;
            double dz = a.z - b.z;
            double dw = a.w - b.w;
            return dx * dx + dy * dy + dz * dz + dw * dw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(in Vec4 a, in Vec4 b)
            => Math.Sqrt(DistanceSq(a, b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 Min(in Vec4 a, in Vec4 b)
            => new Vec4(
                Math.Min(a.x, b.x),
                Math.Min(a.y, b.y),
                Math.Min(a.z, b.z),
                Math.Min(a.w, b.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 Max(in Vec4 a, in Vec4 b)
            => new Vec4(
                Math.Max(a.x, b.x),
                Math.Max(a.y, b.y),
                Math.Max(a.z, b.z),
                Math.Max(a.w, b.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 Abs(in Vec4 v)
            => new Vec4(
                Math.Abs(v.x),
                Math.Abs(v.y),
                Math.Abs(v.z),
                Math.Abs(v.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 Lerp(in Vec4 a, in Vec4 b, double t)
            => new Vec4(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator +(Vec4 a, Vec4 b)
            => new Vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator -(Vec4 a, Vec4 b)
            => new Vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator -(Vec4 v)
            => new Vec4(-v.x, -v.y, -v.z, -v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator *(Vec4 v, double s)
            => new Vec4(v.x * s, v.y * s, v.z * s, v.w * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator *(double s, Vec4 v)
            => new Vec4(v.x * s, v.y * s, v.z * s, v.w * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator /(Vec4 v, double s)
            => new Vec4(v.x / s, v.y / s, v.z / s, v.w / s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator *(Vec4 a, Vec4 b)
            => new Vec4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator /(Vec4 a, Vec4 b)
            => new Vec4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);

        public static bool operator ==(Vec4 a, Vec4 b)
            => a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;

        public static bool operator !=(Vec4 a, Vec4 b)
            => !(a == b);

        public bool Equals(Vec4 other)
            => x.Equals(other.x) && y.Equals(other.y) &&
               z.Equals(other.z) && w.Equals(other.w);

        public override bool Equals(object? obj)
            => obj is Vec4 v && Equals(v);

        public override int GetHashCode()
            => HashCode.Combine(x, y, z, w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AlmostEquals(in Vec4 other, double eps = 1e-12)
            => Math.Abs(x - other.x) <= eps &&
               Math.Abs(y - other.y) <= eps &&
               Math.Abs(z - other.z) <= eps &&
               Math.Abs(w - other.w) <= eps;

        public override string ToString()
            => $"{x:0.###} {y:0.###} {z:0.###} {w:0.###}";
    }
}
