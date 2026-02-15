using System.Runtime.CompilerServices;

namespace TriUgla.Mesher.Quality
{
    public abstract class FaceRankAspectBase : FaceRankAspect
    {
        protected const double Eps = 1e-24;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static double SafeDiv(double num, double den) => num / Math.Max(den, Eps);
    }

}
