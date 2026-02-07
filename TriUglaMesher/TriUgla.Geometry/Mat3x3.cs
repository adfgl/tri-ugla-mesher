using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TriUgla.Geometry
{
    public readonly struct Mat3x3
    {
        public readonly double m11, m12, m13;
        public readonly double m21, m22, m23;
        public readonly double m31, m32, m33;

        public Mat3x3(
            double m11, double m12, double m13,
            double m21, double m22, double m23,
            double m31, double m32, double m33)
        {
            this.m11 = m11; this.m12 = m12; this.m13 = m13;
            this.m21 = m21; this.m22 = m22; this.m23 = m23;
            this.m31 = m31; this.m32 = m32; this.m33 = m33;
        }

        public static Mat3x3 Identity => new Mat3x3(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1);

        public bool IsIdentity() =>
            m11 == 1 && m12 == 0 && m13 == 0 &&
            m21 == 0 && m22 == 1 && m23 == 0 &&
            m31 == 0 && m32 == 0 && m33 == 1;

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
                        2 => m13,
                        _ => throw new IndexOutOfRangeException("Column must be 0..2."),
                    },
                    1 => col switch
                    {
                        0 => m21,
                        1 => m22,
                        2 => m23,
                        _ => throw new IndexOutOfRangeException("Column must be 0..2."),
                    },
                    2 => col switch
                    {
                        0 => m31,
                        1 => m32,
                        2 => m33,
                        _ => throw new IndexOutOfRangeException("Column must be 0..2."),
                    },
                    _ => throw new IndexOutOfRangeException("Row must be 0..2."),
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 Scale(double x, double y, double z)
        {
            if (x < 0 || y < 0 || z < 0)
                throw new ArgumentException("Scaling factors must be non-negative.");

            return new Mat3x3(
                x, 0, 0,
                0, y, 0,
                0, 0, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 RotationX(double rad)
        {
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            return new Mat3x3(
                1, 0, 0,
                0, c, -s,
                0, s, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 RotationY(double rad)
        {
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            return new Mat3x3(
                 c, 0, s,
                 0, 1, 0,
                -s, 0, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 RotationZ(double rad)
        {
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            return new Mat3x3(
                c, -s, 0,
                s, c, 0,
                0, 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Determinant()
        {
            return
                m11 * (m22 * m33 - m23 * m32) -
                m12 * (m21 * m33 - m23 * m31) +
                m13 * (m21 * m32 - m22 * m31);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Inverse(out Mat3x3 inverse)
        {
            double det = Determinant();
            if (det == 0.0)
            {
                inverse = default;
                return false;
            }

            double invDet = 1.0 / det;

            inverse = new Mat3x3(
                (m22 * m33 - m23 * m32) * invDet,
                (m13 * m32 - m12 * m33) * invDet,
                (m12 * m23 - m13 * m22) * invDet,

                (m23 * m31 - m21 * m33) * invDet,
                (m11 * m33 - m13 * m31) * invDet,
                (m13 * m21 - m11 * m23) * invDet,

                (m21 * m32 - m22 * m31) * invDet,
                (m12 * m31 - m11 * m32) * invDet,
                (m11 * m22 - m12 * m21) * invDet
            );

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Mat3x3 Transpose() => new Mat3x3(
            m11, m21, m31,
            m12, m22, m32,
            m13, m23, m33);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 Multiply(Mat3x3 a, Mat3x3 b) => new Mat3x3(
            a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31,
            a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32,
            a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33,

            a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31,
            a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32,
            a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33,

            a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31,
            a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32,
            a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 Multiply(Mat3x3 m, double s) => new Mat3x3(
            m.m11 * s, m.m12 * s, m.m13 * s,
            m.m21 * s, m.m22 * s, m.m23 * s,
            m.m31 * s, m.m32 * s, m.m33 * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 Divide(Mat3x3 m, double s) => new Mat3x3(
            m.m11 / s, m.m12 / s, m.m13 / s,
            m.m21 / s, m.m22 / s, m.m23 / s,
            m.m31 / s, m.m32 / s, m.m33 / s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 operator *(Mat3x3 a, Mat3x3 b) => Multiply(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 operator *(Mat3x3 m, double s) => Multiply(m, s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 operator *(double s, Mat3x3 m) => Multiply(m, s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat3x3 operator /(Mat3x3 m, double s) => Divide(m, s);

        public string ToString(int precision)
        {
            string f = $"F{precision}";
            string[] s =
            {
                m11.ToString(f), m12.ToString(f), m13.ToString(f),
                m21.ToString(f), m22.ToString(f), m23.ToString(f),
                m31.ToString(f), m32.ToString(f), m33.ToString(f),
            };

            int w = 0;
            for (int i = 0; i < s.Length; i++)
                w = Math.Max(w, s[i].Length);

            string c(string v) => v.PadLeft(w);

            return
                $"{c(s[0])} {c(s[1])} {c(s[2])}\n" +
                $"{c(s[3])} {c(s[4])} {c(s[5])}\n" +
                $"{c(s[6])} {c(s[7])} {c(s[8])}";
        }

        public override string ToString() => ToString(2);
    }
}
