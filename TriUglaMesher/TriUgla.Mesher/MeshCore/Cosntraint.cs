using System;
using System.Collections.Generic;
using System.Text;

namespace TriUgla.Mesher.MeshCore
{
    public class Cosntraint
    {
        public List<ConstraintPoint> Points { get; } = new List<ConstraintPoint>();
        public List<ConstraintSpan> Spans { get; } = new List<ConstraintSpan>();
    }
}
