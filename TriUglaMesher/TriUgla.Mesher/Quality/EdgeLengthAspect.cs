using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public sealed class EdgeLengthAspect : FaceRankAspectBase
    {
        public double MinEdgeLength { get; set; } = 0.0;
        public double MaxEdgeLength { get; set; } = double.PositiveInfinity;

        public override double Violation01(Face face, in FaceStats s)
        {
            double vMin = 0.0;
            if (MinEdgeLength > 0.0)
            {
                double req2 = MinEdgeLength * MinEdgeLength;
                if (s.MinLen2 < req2)
                {
                    double minLen = Math.Sqrt(Math.Max(s.MinLen2, 0.0));
                    vMin = Clamp01(SafeDiv(MinEdgeLength - minLen, MinEdgeLength));
                }
            }

            double vMax = 0.0;
            if (!double.IsPositiveInfinity(MaxEdgeLength))
            {
                double req2 = MaxEdgeLength * MaxEdgeLength;
                if (s.MaxLen2 > req2)
                {
                    double maxLen = Math.Sqrt(s.MaxLen2);
                    vMax = Clamp01(SafeDiv(maxLen - MaxEdgeLength, MaxEdgeLength));
                }
            }

            return vMin > vMax ? vMin : vMax;
        }
    }
}
