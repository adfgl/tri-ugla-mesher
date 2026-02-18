using System;
using System.Collections.Generic;
using System.Text;

namespace TriUgla.Mesher.Quality
{
    public sealed class MeshMetric<T> where T : class
    {
        public int Count { get; set; }
        public double Average { get; set; }

        public double Min { get; set; } = double.MaxValue;
        public T MinElement { get; set; } = null!;

        public double Max { get; set; } = double.MinValue;
        public T MaxElement { get; set; } = null!;

        public void Set(T element, double value)
        {
            Count++;
            Average += (value - Average) / Count;

            if (MinElement is null || Min > value)
            {
                MinElement = element;
                Min = value;
            }

            if (MaxElement is null || Max < value)
            {
                MaxElement = element;
                Max = value;
            }
        }

        public string ToString(string? unit = null)
        {
            if (Count == 0)
                return "no data";

            string u = unit is null ? "" : $" {unit}";
            return
                $"min: {Min:F2}{u}, " +
                $"avg: {Average:F2}{u}, " +
                $"max: {Max:F2}{u} " +
                $"(n={Count})";
        }
    }
}
