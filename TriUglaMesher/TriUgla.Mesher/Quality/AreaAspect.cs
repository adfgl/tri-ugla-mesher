using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public sealed class AreaAspect : FaceRankAspectBase
    {
        public double MinArea { get; set; } = 0.0;
        public double MaxArea { get; set; } = double.PositiveInfinity;

        public override double Violation01(Face face, in FaceStats s)
        {
            double area = s.AbsArea;
            if (area <= Eps) return 0.0;

            double vMin = 0.0;
            if (MinArea > 0.0 && area < MinArea)
                vMin = Clamp01(SafeDiv(MinArea - area, MinArea));

            double vMax = 0.0;
            if (!double.IsPositiveInfinity(MaxArea) && area > MaxArea)
                vMax = Clamp01(SafeDiv(area - MaxArea, MaxArea));

            return vMin > vMax ? vMin : vMax;
        }
    }
}
