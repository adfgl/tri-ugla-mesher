namespace TriUgla.Mesher.MeshCore
{
    public struct ConstraintSpan(ConstraintPoint from, ConstraintPoint to)
    {
        public ConstraintPoint from = from, to = to;
    }
}
