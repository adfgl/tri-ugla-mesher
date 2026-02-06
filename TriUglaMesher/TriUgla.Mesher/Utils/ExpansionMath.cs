using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TriUgla.Mesher.Utils
{
    public static class ExpansionMath
    {
        public static int SignSafe(List<double> exp)
        {
            Compress(exp);
            return Sign(exp);
        }

        public static List<double> Sub(List<double> a, List<double> b)
        {
            int n = b.Count;
            for (int i = 0; i < n; i++)
                Add(a, -b[i]);
            return a;
        }

        public static List<double> Sub(List<double> exp, double value)
        {
            return Add(exp, -value);
        }

        public static List<double> Negate(List<double> exp)
        {
            for (int i = 0; i < exp.Count; i++)
                exp[i] = -exp[i];
            return exp;
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

        public static List<double> Add(List<double> a, List<double> b)
        {
            int n = b.Count;
            for (int i = 0; i < n; i++)
            {
                Add(a, b[i]);
            }
            return a;
        }

        public static List<double> Mul(List<double> a, List<double> b)
        {
            if (a.Count == 0 || b.Count == 0)
            {
                a.Clear();
                return a;
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

            Compress(result);
            a.Clear();
            a.AddRange(result);
            return a;
        }

        public static List<double> Add(List<double> exp, double value)
        {
            if (value == 0.0)
                return exp;

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
            return exp;
        }

        public static List<double> Mul(List<double> exp, double b)
        {
            if (b == 0.0)
            {
                exp.Clear();
                return exp;
            }

            if (b == 1.0)
                return exp;

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
                exp.Add(carry);

            return exp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compress(List<double> exp)
        {
            int n = exp.Count;
            if (n == 0) return 0;

            Span<double> a = CollectionsMarshal.AsSpan(exp);

            int bottom = ForwardSweep(a, n);
            int start = BackwardSweep(a, bottom);
            int newCnt = ShiftFront(a, start, bottom);
            if (exp.Count != newCnt)
                exp.RemoveRange(newCnt, exp.Count - newCnt);

            return newCnt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int ForwardSweep(Span<double> a, int n)
        {
            int bottom = 0;
            double val = a[0];
            for (int i = 1; i < n; i++)
            {
                TwoSum(val, a[i], out val, out double err);
                if (err != 0.0)
                {
                    a[bottom++] = err;
                }
            }

            a[bottom++] = val;
            return bottom;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BackwardSweep(Span<double> a, int bottom)
        {
            int i = bottom - 1;
            double error = a[i];
            int write = i;

            while (--i >= 0)
            {
                TwoSum(error, a[i], out double sum, out error);

                if (sum != 0.0)
                    a[write--] = sum;
            }

            a[write] = error;
            return write;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int ShiftFront(Span<double> a, int start, int bottom)
        {
            int newCount = bottom - start;
            if (start != 0)
                a.Slice(start, newCount).CopyTo(a);
            return newCount;
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
            const double splitter = 134217729.0; // 2^27 + 1
            double c = splitter * a;
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
