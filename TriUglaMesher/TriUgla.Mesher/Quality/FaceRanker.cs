using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public sealed class FaceRanker
    {
        public IFaceStatsCollector Collector { get; set; } = new DefaultFaceStatsCollector();

        public AreaAspect Area { get; } = new AreaAspect { Weight = 0.0 };
        public EdgeLengthAspect Edge { get; } = new EdgeLengthAspect { Weight = 0.0 };
        public AngleAspect Angle { get; } = new AngleAspect { Weight = 1.0 };
        public VertexAreaAspect VertexArea { get; } = new VertexAreaAspect { Weight = 1.0 };

        public double Rank(Face face)
        {
            if (!Collector.TryCollect(face, out FaceStats s))
                return 0.0;

            double worst = 0.0;
            double wMax = 0.0;

            Acc(in worst, in wMax, Area, face, in s, out worst, out wMax);
            Acc(in worst, in wMax, Edge, face, in s, out worst, out wMax);
            Acc(in worst, in wMax, Angle, face, in s, out worst, out wMax);
            Acc(in worst, in wMax, VertexArea, face, in s, out worst, out wMax);

            if (wMax <= 0.0) return 0.0;
            return Clamp01(worst / wMax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Acc(
            in double worstIn,
            in double wMaxIn,
            FaceRankAspect a,
            Face face,
            in FaceStats s,
            out double worstOut,
            out double wMaxOut)
        {
            double worst = worstIn;
            double wMax = wMaxIn;

            double w = a.Weight;
            if (w > 0.0)
            {
                double v = a.Violation01(face, in s) * w;
                if (v > worst) worst = v;
                if (w > wMax) wMax = w;
            }

            worstOut = worst;
            wMaxOut = wMax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double Clamp01(double v) => v <= 0.0 ? 0.0 : (v >= 1.0 ? 1.0 : v);
    }
}
