namespace TriUgla.Mesher
{
    public sealed class Constraint
    {
        public string? Name { get; set; }
        public List<ConstraintPoint> Points { get; set; } = new List<ConstraintPoint>();
        public List<ConstraintSpan> Spans { get; set; } = new List<ConstraintSpan>();
    }
}
