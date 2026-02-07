using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TriUgla.Geometry
{
    public readonly struct Mat4x4(
        double m11, double m12, double m13, double m14,
        double m21, double m22, double m23, double m24,
        double m31, double m32, double m33, double m34,
        double m41, double m42, double m43, double m44)
    {
        public readonly double m11 = m11, m12 = m12, m13 = m13, m14 = m14;
        public readonly double m21 = m21, m22 = m22, m23 = m23, m24 = m24;
        public readonly double m31 = m31, m32 = m32, m33 = m33, m34 = m34;
        public readonly double m41 = m41, m42 = m42, m43 = m43, m44 = m44;

        public static Mat4x4 Identity => new Mat4x4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);

        public bool IsIdentity() =>
            m11 == 1 && m12 == 0 && m13 == 0 && m14 == 0 &&
            m21 == 0 && m22 == 1 && m23 == 0 && m24 == 0 &&
            m31 == 0 && m32 == 0 && m33 == 1 && m34 == 0 &&
            m41 == 0 && m42 == 0 && m43 == 0 && m44 == 1;

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
                        3 => m14,
                        _ => throw new IndexOutOfRangeException("Column must be 0..3."),
                    },
                    1 => col switch
                    {
                        0 => m21,
                        1 => m22,
                        2 => m23,
                        3 => m24,
                        _ => throw new IndexOutOfRangeException("Column must be 0..3."),
                    },
                    2 => col switch
                    {
                        0 => m31,
                        1 => m32,
                        2 => m33,
                        3 => m34,
                        _ => throw new IndexOutOfRangeException("Column must be 0..3."),
                    },
                    3 => col switch
                    {
                        0 => m41,
                        1 => m42,
                        2 => m43,
                        3 => m44,
                        _ => throw new IndexOutOfRangeException("Column must be 0..3."),
                    },
                    _ => throw new IndexOutOfRangeException("Row must be 0..3."),
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Mat4x4 Transpose() => new Mat4x4(
            m11, m21, m31, m41,
            m12, m22, m32, m42,
            m13, m23, m33, m43,
            m14, m24, m34, m44);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Determinant()
        {
            // Expansion by first row.
            double d1 = Det3(m22, m23, m24, m32, m33, m34, m42, m43, m44);
            double d2 = Det3(m21, m23, m24, m31, m33, m34, m41, m43, m44);
            double d3 = Det3(m21, m22, m24, m31, m32, m34, m41, m42, m44);
            double d4 = Det3(m21, m22, m23, m31, m32, m33, m41, m42, m43);

            return m11 * d1 - m12 * d2 + m13 * d3 - m14 * d4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Inverse(out Mat4x4 inverse)
        {
            double det = Determinant();
            if (det == 0.0)
            {
                inverse = default;
                return false;
            }

            double invDet = 1.0 / det;

            double c11 = +Det3(m22, m23, m24, m32, m33, m34, m42, m43, m44);
            double c12 = -Det3(m21, m23, m24, m31, m33, m34, m41, m43, m44);
            double c13 = +Det3(m21, m22, m24, m31, m32, m34, m41, m42, m44);
            double c14 = -Det3(m21, m22, m23, m31, m32, m33, m41, m42, m43);

            double c21 = -Det3(m12, m13, m14, m32, m33, m34, m42, m43, m44);
            double c22 = +Det3(m11, m13, m14, m31, m33, m34, m41, m43, m44);
            double c23 = -Det3(m11, m12, m14, m31, m32, m34, m41, m42, m44);
            double c24 = +Det3(m11, m12, m13, m31, m32, m33, m41, m42, m43);

            double c31 = +Det3(m12, m13, m14, m22, m23, m24, m42, m43, m44);
            double c32 = -Det3(m11, m13, m14, m21, m23, m24, m41, m43, m44);
            double c33 = +Det3(m11, m12, m14, m21, m22, m24, m41, m42, m44);
            double c34 = -Det3(m11, m12, m13, m21, m22, m23, m41, m42, m43);

            double c41 = -Det3(m12, m13, m14, m22, m23, m24, m32, m33, m34);
            double c42 = +Det3(m11, m13, m14, m21, m23, m24, m31, m33, m34);
            double c43 = -Det3(m11, m12, m14, m21, m22, m24, m31, m32, m34);
            double c44 = +Det3(m11, m12, m13, m21, m22, m23, m31, m32, m33);

            inverse = new Mat4x4(
                c11 * invDet, c21 * invDet, c31 * invDet, c41 * invDet,
                c12 * invDet, c22 * invDet, c32 * invDet, c42 * invDet,
                c13 * invDet, c23 * invDet, c33 * invDet, c43 * invDet,
                c14 * invDet, c24 * invDet, c34 * invDet, c44 * invDet);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat4x4 Multiply(Mat4x4 a, Mat4x4 b) => new Mat4x4(
            a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31 + a.m14 * b.m41,
            a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32 + a.m14 * b.m42,
            a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33 + a.m14 * b.m43,
            a.m11 * b.m14 + a.m12 * b.m24 + a.m13 * b.m34 + a.m14 * b.m44,

            a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31 + a.m24 * b.m41,
            a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32 + a.m24 * b.m42,
            a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33 + a.m24 * b.m43,
            a.m21 * b.m14 + a.m22 * b.m24 + a.m23 * b.m34 + a.m24 * b.m44,

            a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31 + a.m34 * b.m41,
            a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32 + a.m34 * b.m42,
            a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33 + a.m34 * b.m43,
            a.m31 * b.m14 + a.m32 * b.m24 + a.m33 * b.m34 + a.m34 * b.m44,

            a.m41 * b.m11 + a.m42 * b.m21 + a.m43 * b.m31 + a.m44 * b.m41,
            a.m41 * b.m12 + a.m42 * b.m22 + a.m43 * b.m32 + a.m44 * b.m42,
            a.m41 * b.m13 + a.m42 * b.m23 + a.m43 * b.m33 + a.m44 * b.m43,
            a.m41 * b.m14 + a.m42 * b.m24 + a.m43 * b.m34 + a.m44 * b.m44);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 Multiply(Mat4x4 m, Vec4 v) => new Vec4(
            x: v.x * m.m11 + v.y * m.m12 + v.z * m.m13 + v.w * m.m14,
            y: v.x * m.m21 + v.y * m.m22 + v.z * m.m23 + v.w * m.m24,
            z: v.x * m.m31 + v.y * m.m32 + v.z * m.m33 + v.w * m.m34,
            w: v.x * m.m41 + v.y * m.m42 + v.z * m.m43 + v.w * m.m44);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat4x4 Multiply(Mat4x4 m, double s) => new Mat4x4(
            m.m11 * s, m.m12 * s, m.m13 * s, m.m14 * s,
            m.m21 * s, m.m22 * s, m.m23 * s, m.m24 * s,
            m.m31 * s, m.m32 * s, m.m33 * s, m.m34 * s,
            m.m41 * s, m.m42 * s, m.m43 * s, m.m44 * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat4x4 Divide(Mat4x4 m, double s) => new Mat4x4(
            m.m11 / s, m.m12 / s, m.m13 / s, m.m14 / s,
            m.m21 / s, m.m22 / s, m.m23 / s, m.m24 / s,
            m.m31 / s, m.m32 / s, m.m33 / s, m.m34 / s,
            m.m41 / s, m.m42 / s, m.m43 / s, m.m44 / s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat4x4 operator *(Mat4x4 a, Mat4x4 b) => Multiply(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec4 operator *(Mat4x4 m, Vec4 v) => Multiply(m, v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat4x4 operator *(Mat4x4 m, double s) => Multiply(m, s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat4x4 operator *(double s, Mat4x4 m) => Multiply(m, s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat4x4 operator /(Mat4x4 m, double s) => Divide(m, s);

        public string ToString(int precision)
        {
            string f = $"F{precision}";
            string[] s =
            {
            m11.ToString(f), m12.ToString(f), m13.ToString(f), m14.ToString(f),
            m21.ToString(f), m22.ToString(f), m23.ToString(f), m24.ToString(f),
            m31.ToString(f), m32.ToString(f), m33.ToString(f), m34.ToString(f),
            m41.ToString(f), m42.ToString(f), m43.ToString(f), m44.ToString(f),
        };

            int w = 0;
            for (int i = 0; i < s.Length; i++)
                w = Math.Max(w, s[i].Length);

            string c(string v) => v.PadLeft(w);

            return
                $"{c(s[0])} {c(s[1])} {c(s[2])} {c(s[3])}\n" +
                $"{c(s[4])} {c(s[5])} {c(s[6])} {c(s[7])}\n" +
                $"{c(s[8])} {c(s[9])} {c(s[10])} {c(s[11])}\n" +
                $"{c(s[12])} {c(s[13])} {c(s[14])} {c(s[15])}";
        }

        public override string ToString() => ToString(2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double Det3(
            double a11, double a12, double a13,
            double a21, double a22, double a23,
            double a31, double a32, double a33)
        {
            return
                a11 * (a22 * a33 - a23 * a32) -
                a12 * (a21 * a33 - a23 * a31) +
                a13 * (a21 * a32 - a22 * a31);
        }
    }
}
