namespace TriUgla.Mesher
{
    public sealed class Constraint
    {
        public string? Name { get; set; }
        public List<ConstraintPoint> Points { get; } = new List<ConstraintPoint>();
        public List<ConstraintSpan> Spans { get; } = new List<ConstraintSpan>();
    }
}
