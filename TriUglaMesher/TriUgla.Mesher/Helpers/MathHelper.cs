using System.Runtime.CompilerServices;

namespace TriUgla.Mesher.Helpers
{
    public static class MathHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(double v, double eps)
            => v >= -eps && v <= eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cos(double cos)
        {
            if (cos > 1) cos = 1;
            else if (cos < -1) cos = -1;
            return Math.Acos(cos);
        }
    }
}
