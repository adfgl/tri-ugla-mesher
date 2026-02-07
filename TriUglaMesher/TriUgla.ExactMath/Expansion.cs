using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TriUgla.ExactMath
{
    public static class Expansion
    {
        /// <summary>
        /// 2^27 + 1
        /// </summary>
        public const double SPLITTER = 134217729.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compress(List<double> exp)
        {
            if (exp.Count == 0) return 0;

            double sum = 0.0;
            int write = 0;

            for (int i = 0; i < exp.Count; i++)
            {
                TwoSum(sum, exp[i], out double hi, out double lo);
                if (lo != 0.0)
                    exp[write++] = lo;
                sum = hi;
            }

            if (sum != 0.0)
                exp[write++] = sum;

            exp.RemoveRange(write, exp.Count - write);
            return write;
        }

        public static void Negate(List<double> exp)
        {
            for (int i = 0; i < exp.Count; i++)
                exp[i] = -exp[i];
        }

        public static int Sign(List<double> exp)
        {
            int n = exp.Count;
            for (int i = n - 1; i >= 0; i--)
            {
                double x = exp[i];
                if (x != 0.0)
                    return x > 0.0 ? +1 : -1;
            }
            return 0;
        }

        public static double Approximate(List<double> value)
        {
            double s = 0.0;
            for (int i = 0; i < value.Count; i++)
                s += value[i];
            return s;
        }

        public static void Add(List<double> a, List<double> b)
        {
            int n = b.Count;
            for (int i = 0; i < n; i++)
            {
                Add(a, b[i]);
            }
        }

        public static void Add(List<double> exp, double value)
        {
            if (value == 0.0)
                return;

            int n = exp.Count;
            for (int i = 0; i < n; i++)
            {
                TwoSum(exp[i], value, out value, out double low);
                exp[i] = low;
            }

            if (value != 0.0)
            {
                exp.Add(value);
            }
        }

        public static void Mul(List<double> a, List<double> b)
        {
            if (a.Count == 0 || b.Count == 0)
            {
                a.Clear();
                return;
            }

            List<double> result = new List<double>(a.Count * b.Count);

            int nb = b.Count;
            for (int j = 0; j < nb; j++)
            {
                double bj = b[j];
                if (bj == 0.0) continue;

                List<double> temp = new List<double>(a);
                Mul(temp, bj);
                Add(result, temp);
            }
            a.Clear();
            a.AddRange(result);
        }

        public static void Mul(List<double> exp, double b)
        {
            if (b == 0.0)
            {
                exp.Clear();
                return;
            }

            if (b == 1.0)
                return;

            int n = exp.Count;
            double carry = 0.0;

            for (int i = 0; i < n; i++)
            {
                TwoProd(exp[i], b, out double pHigh, out double pLow);
                TwoSum(pHigh, carry, out double sHigh, out double sLow);
                TwoSum(pLow, sLow, out double cHigh, out double cLow);
                exp[i] = cLow;
                carry = sHigh + cHigh;
            }

            if (carry != 0.0)
            {
                exp.Add(carry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TwoSum(double a, double b, out double high, out double low)
        {
            high = a + b;                 // rounded sum
            double storedB = high - a;    // part of b that made it into high
            low = (a - (high - storedB))  // part of a lost
                + (b - storedB);          // part of b lost
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Split(double a, out double high, out double low)
        {
            double c = SPLITTER * a;
            double abig = c - a;
            high = c - abig;
            low = a - high;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TwoProd(double a, double b, out double high, out double low)
        {
            high = a * b;

            Split(a, out double aHi, out double aLo);
            Split(b, out double bHi, out double bLo);

            low = ((aHi * bHi - high) + aHi * bLo + aLo * bHi) + aLo * bLo;
        }
    }
}
