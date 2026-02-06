namespace TriUgla.Mesher.HalfEdge
{
    public sealed class Face : MeshElement
    {
        public Edge Edge { get; internal set; } = null!;
        public int RegionId { get; internal set; }
        public int Depth { get; internal set; }

        public FaceKind Kind
        {
            get
            {
                int depth = Depth;
                if (depth == 0) return FaceKind.Undefined;
                if (depth < 0) return FaceKind.Outside;
                return (depth & 1) == 1
                    ? FaceKind.Land
                    : FaceKind.Lake;
            }
        }

        public void TransmitContextTo(Face other)
        {
            other.Invalid = Invalid;
            other.Payload = Payload;
            other.RegionId = RegionId;
            other.Depth = Depth;
        }
    }
}
