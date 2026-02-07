using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TriUgla.Geometry
{
    public readonly struct Mat2x2(double m11, double m12, double m21, double m22)
    {
        public readonly double m11 = m11, m12 = m12;
        public readonly double m21 = m21, m22 = m22;

        public static Mat2x2 Identity => new Mat2x2(
            1, 0,
            0, 1);

        public bool IsIdentity() =>
            m11 == 1 && m12 == 0 &&
            m21 == 0 && m22 == 1;

        public double Get(int row, int col) => this[row, col];

        public double this[int row, int col]
        {
            get
            {
                return row switch
                {
                    0 => col switch
                    {
                        0 => m11,
                        1 => m12,
                        _ => throw new IndexOutOfRangeException("Column must be 0 or 1."),
                    },
                    1 => col switch
                    {
                        0 => m21,
                        1 => m22,
                        _ => throw new IndexOutOfRangeException("Column must be 0 or 1."),
                    },
                    _ => throw new IndexOutOfRangeException("Row must be 0 or 1."),
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 Rotation(double rad)
        {
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            return new Mat2x2(
                c, -s,
                s, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 Scale(double x, double y)
        {
            if (x < 0 || y < 0)
                throw new ArgumentException("Scaling factors must be non-negative.");

            return new Mat2x2(
                x, 0,
                0, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Determinant() => m11 * m22 - m12 * m21;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Inverse(out Mat2x2 inverse)
        {
            double det = Determinant();
            if (det == 0)
            {
                inverse = default;
                return false;
            }

            double invDet = 1.0 / det;
            inverse = new Mat2x2(
                m22 * invDet, -m12 * invDet,
               -m21 * invDet, m11 * invDet);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Mat2x2 Transpose() => new Mat2x2(
            m11, m21,
            m12, m22);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 Multiply(Mat2x2 a, Mat2x2 b) => new Mat2x2(
            a.m11 * b.m11 + a.m12 * b.m21,
            a.m11 * b.m12 + a.m12 * b.m22,

            a.m21 * b.m11 + a.m22 * b.m21,
            a.m21 * b.m12 + a.m22 * b.m22);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Multiply(Mat2x2 m, Vec2 v) => new Vec2(
            x: v.x * m.m11 + v.y * m.m12,
            y: v.x * m.m21 + v.y * m.m22);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 Multiply(Mat2x2 m, double s) => new Mat2x2(
            m.m11 * s, m.m12 * s,
            m.m21 * s, m.m22 * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 Divide(Mat2x2 m, double s) => new Mat2x2(
            m.m11 / s, m.m12 / s,
            m.m21 / s, m.m22 / s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 operator *(Mat2x2 a, Mat2x2 b) => Multiply(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator *(Mat2x2 m, Vec2 v) => Multiply(m, v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 operator *(Mat2x2 m, double s) => Multiply(m, s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 operator *(double s, Mat2x2 m) => Multiply(m, s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat2x2 operator /(Mat2x2 m, double s) => Divide(m, s);

        public string ToString(int precision)
        {
            string format = $"F{precision}";
            string s11 = m11.ToString(format);
            string s12 = m12.ToString(format);
            string s21 = m21.ToString(format);
            string s22 = m22.ToString(format);

            int w = Math.Max(Math.Max(s11.Length, s12.Length),
                             Math.Max(s21.Length, s22.Length));

            string cell(string s) => s.PadLeft(w);

            return
                $"{cell(s11)} {cell(s12)}\n" +
                $"{cell(s21)} {cell(s22)}";
        }

        public override string ToString() => ToString(2);
    }
}
