namespace TriUgla.HalfEdge
{
    public sealed class VisitStamps
    {
        public VisitStamp Node { get; } = new VisitStamp();
        public VisitStamp Edge { get; } = new VisitStamp();
        public VisitStamp Face { get; } = new VisitStamp();
    }
}
