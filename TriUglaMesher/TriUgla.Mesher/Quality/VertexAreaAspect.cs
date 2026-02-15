using System;
using System.Collections.Generic;
using System.Text;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public sealed class VertexAreaAspect : FaceRankAspectBase
    {
        public double OverTolerance { get; set; } = 0.0;

        public override double Violation01(Face face, in FaceStats s)
        {
            double faceArea = s.AbsArea;
            double targetArea = s.AvgVertexArea;

            if (faceArea <= Eps || targetArea <= Eps) return 0.0;

            double allowed = targetArea * (1.0 + Math.Max(0.0, OverTolerance));
            if (faceArea <= allowed) return 0.0;

            double over = SafeDiv(faceArea - allowed, allowed);
            return Clamp01(over);
        }
    }
}
