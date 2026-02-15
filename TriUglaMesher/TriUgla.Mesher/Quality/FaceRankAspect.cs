using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public abstract class FaceRankAspect
    {
        public double Weight { get; set; } = 1.0;
        public bool Enabled => Weight > 0.0;

        public abstract double Violation01(Face face, in FaceStats s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static double Clamp01(double v) => v <= 0.0 ? 0.0 : (v >= 1.0 ? 1.0 : v);
    }
}
