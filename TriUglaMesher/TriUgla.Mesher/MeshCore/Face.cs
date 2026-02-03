namespace TriUgla.Mesher.MeshCore
{
    public sealed class Face : MeshElement
    {
        public Edge Edge { get; set; } = null!;
        public int Depth { get; set; } = -1;
    }
}
