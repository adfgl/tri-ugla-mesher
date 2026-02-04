namespace TriUgla.Mesher.MeshCore
{
    public sealed class Face : MeshElement
    {
        public Edge Edge { get; internal set; } = null!;
        public int Depth { get; internal set; } = -1;
        public FaceKind Kind 
        {
            get
            {

            }
        }
    }
}
