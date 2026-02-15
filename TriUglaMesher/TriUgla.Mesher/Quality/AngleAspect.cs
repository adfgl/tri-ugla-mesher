using System.Runtime.CompilerServices;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public sealed class AngleAspect : FaceRankAspectBase
    {
        public double MinAngleDeg { get; set; } = 0.0;
        public double AngleRatioThreshold { get; set; } = 0.0;

        public override double Violation01(Face face, in FaceStats s)
        {
            if (s.MinLen2 <= Eps) return 0.0;

            double r2 = face.CircumCircle.radius2;
            if (r2 <= 0.0) return 0.0;

            double ratio = Math.Sqrt(r2 / s.MinLen2);

            double thr = AngleRatioThreshold > 0.0
                ? AngleRatioThreshold
                : (MinAngleDeg > 0.0 ? AngleRatioFromMinAngleDeg(MinAngleDeg) : 0.0);

            if (thr <= 0.0 || ratio <= thr) return 0.0;

            return Clamp01(SafeDiv(ratio - thr, thr));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double AngleRatioFromMinAngleDeg(double minAngleDeg)
        {
            double theta = minAngleDeg * (Math.PI / 180.0);
            return 1.0 / (2.0 * Math.Sin(theta));
        }
    }
}
