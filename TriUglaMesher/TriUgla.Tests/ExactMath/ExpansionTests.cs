using TriUgla.ExactMath;

namespace TriUgla.Tests.ExactMath
{
    public class ExpansionTests
    {
        static void AssertNondecreasingAbs(IReadOnlyList<double> e)
        {
            for (int i = 1; i < e.Count; i++)
            {
                double prev = Math.Abs(e[i - 1]);
                double cur = Math.Abs(e[i]);

                Assert.True(prev <= cur, $"Not sorted by |x| at i={i}: |{e[i - 1]}|={prev} > |{e[i]}|={cur}");
            }
        }

        [Fact]
        public void Compress_Empty_ReturnsZero()
        {
            var e = new List<double>();
            int newCnt = Expansion.Compress(e);

            Assert.Equal(0, newCnt);
            Assert.Empty(e);
        }

        [Fact]
        public void Compress_PreservesAbsOrder_ForAlreadySortedExpansion()
        {
            var e = new List<double>
            {
                1e-30, -2e-20, 3e-10, -4e0, 5e10
            };

            double before = Expansion.Approximate(e);

            int newCnt = Expansion.Compress(e);

            Assert.Equal(e.Count, newCnt);
            AssertNondecreasingAbs(e);
            Assert.Equal(before, Expansion.Approximate(e), 12);
        }

        [Fact]
        public void Compress_RemovesZeros_AndKeepsAbsOrder()
        {
            var e = new List<double>
            {
                1e-30, 0.0, -2e-20, 0.0, 3e-10, 0.0, -4.0
            };

            double before = Expansion.Approximate(e);

            int newCnt = Expansion.Compress(e);

            Assert.Equal(newCnt, e.Count);
            Assert.DoesNotContain(0.0, e);
            AssertNondecreasingAbs(e);
            Assert.Equal(before, Expansion.Approximate(e), 12);
        }

        [Fact]
        public void Compress_CombinesCarry_StillSortedByAbs()
        {
            var e = new List<double>
            {
                1.0,  
                1e16, 
                -1e16 
            };

            int newCnt = Expansion.Compress(e);
            Assert.Equal(newCnt, e.Count);
            Assert.Single(e);
            Assert.Equal(1, e[0]);
            AssertNondecreasingAbs(e);
        }

        [Fact]
        public void Add_EmptyB_DoesNothing()
        {
            var a = new List<double> { 1.0, 2.0 };
            var b = new List<double>();

            double before = Expansion.Approximate(a);

            Expansion.Add(a, b);

            Assert.Equal(before, Expansion.Approximate(a));
            Assert.Equal(2, a.Count);
        }

        [Fact]
        public void Add_EmptyA_CopiesBValue()
        {
            var a = new List<double>();
            var b = new List<double> { 1.0, 2.0, 3.0 };

            Expansion.Add(a, b);
            Assert.Equal(b.Count, a.Count);
            Assert.Equal(Expansion.Approximate(b), Expansion.Approximate(a));
        }

        [Fact]
        public void Add_PreservesNumericValue()
        {
            var a = new List<double> { 1e-30, 1.0, 1e30 };
            var b = new List<double> { -1.0, 2.5, 1e-40 };

            double beforeA = Expansion.Approximate(a);
            double valueB = Expansion.Approximate(b);

            Expansion.Add(a, b);

            Assert.Equal(beforeA + valueB, Expansion.Approximate(a), 12);
        }


        [Fact]
        public void Add_DoesNotModifyB()
        {
            var a = new List<double> { 1.0 };
            var b = new List<double> { 2.0, 3.0 };

            var bCopy = new List<double>(b);

            Expansion.Add(a, b);

            Assert.Equal(bCopy.Count, b.Count);
            for (int i = 0; i < b.Count; i++)
                Assert.Equal(bCopy[i], b[i]);
        }


        [Fact]
        public void Add_CanIncreaseSize()
        {
            var a = new List<double> { 1e16 };
            var b = new List<double> { 1.0 };

            int before = a.Count;

            Expansion.Add(a, b);

            Assert.True(a.Count >= before);
            Assert.Equal(1e16 + 1.0, Expansion.Approximate(a));
        }

        [Fact]
        public void Add_Zero_DoesNothing()
        {
            List<double> e = new List<double> { 1.0, 2.0, 3.0 };
            List<double> before = new List<double>(e);

            Expansion.Add(e, 0.0);

            Assert.Equal(before.Count, e.Count);
            for (int i = 0; i < e.Count; i++)
                Assert.Equal(before[i], e[i]);
        }

        [Fact]
        public void Add_Empty_AddsValueAsSingleComponent()
        {
            var e = new List<double>();

            Expansion.Add(e, 5.0);

            Assert.Single(e);
            Assert.Equal(5.0, e[0]);
        }

        [Fact]
        public void Add_AppendsCarry_WhenValueIsLostInFirstComponent()
        {
            double a = 1e16;
            double b = 1.0;

            List<double> e = [a];
            int beforeCount = e.Count;

            Expansion.Add(e, b);

            Assert.Equal(beforeCount + 1, e.Count);
            Assert.Equal(a, e[^1]);    
            Assert.Equal(b, e[0]);    
        }

        [Fact]
        public void Add_DoesNotAppend_WhenAdditionIsExact()
        {
            List<double> e = new List<double> { 0.5, 1.0 };
            int beforeCount = e.Count;

            Expansion.Add(e, -1.5);

            Assert.Equal(beforeCount, e.Count);
            Assert.Equal(0.0, e[0]);
            Assert.Equal(0.0, e[1]);
        }


        [Fact]
        public void TwoSum_ProducesNonZeroLow_AndExactReconstruction()
        {
            double a = 1e16;
            double b = 1.0;

            Expansion.TwoSum(a, b, out double high, out double low);

            Assert.Equal(a + b, high);
            Assert.Equal(low, b);
            Assert.Equal(a + b, high + low);
        }

        [Theory]
        [InlineData(1e16, -1e16)]
        [InlineData(1e100, -1e100)]
        [InlineData(1e-200, -1e-200)]
        public void TwoSum_PerfectCancellation_LowIsZero(double a, double b)
        {
            Expansion.TwoSum(a, b, out double high, out double low);

            Assert.Equal(a + b, high);
            Assert.Equal(0.0, low);
            Assert.Equal(high + low, a + b);
        }

        [Fact]
        public void Split_ClearsLowMantissaBits_InHigh()
        {
            // 1.x value with many lower mantissa bits set
            double a = BitConverter.Int64BitsToDouble(
                unchecked((long)((0x3FFUL << 52) | 0x000F_FFFF_FFFF_FFFFUL))
            );

            Expansion.Split(a, out double high, out double low);

            // Dekker split with 2^27 + 1:
            // high must have its lowest 26 mantissa bits cleared
            const int splitBits = 26;
            ulong mask = (1UL << splitBits) - 1;

            ulong highMantissa = Mantissa(high);

            Assert.Equal(0UL, highMantissa & mask);

            Assert.Equal(
                BitConverter.DoubleToInt64Bits(a),
                BitConverter.DoubleToInt64Bits(high + low)
            );
        }

        [Fact]
        public void TwoProd_ProducesNonZeroLow_AndExactReconstruction()
        {
            double a = Expansion.SPLITTER;
            double b = Expansion.SPLITTER;

            Expansion.TwoProd(a, b, out double high, out double low);

            Assert.Equal(a * b, high);   
            Assert.Equal(1.0, low);      
            Assert.Equal(a * b + 1.0, high + low);
        }

        [Theory]
        [InlineData(0.0, 123.0)]
        [InlineData(1.0, 123.0)]
        [InlineData(-1.0, 123.0)]
        [InlineData(2.0, 0.5)]      
        [InlineData(8.0, 0.125)]
        [InlineData(1.0, 1024.0)]
        public void TwoProd_ExactProducts_LowIsZero(double a, double b)
        {
            Expansion.TwoProd(a, b, out double high, out double low);

            Assert.Equal(a * b, high);
            Assert.Equal(0.0, low);
            Assert.Equal(a * b, high + low);
        }

        static ulong Mantissa(double x)
        {
            ulong bits = (ulong)BitConverter.DoubleToInt64Bits(x);
            return bits & ((1UL << 52) - 1);
        }
    }
}
