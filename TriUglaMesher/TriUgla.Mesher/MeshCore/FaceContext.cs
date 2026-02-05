namespace TriUgla.Mesher.MeshCore
{
    public struct FaceContext
    {
        public int payload;
        public int depth;

        public readonly FaceKind Kind
        {
            get
            {
                if (depth == 0)
                    return FaceKind.Undefined;

                return (depth & 1) == 1
                    ? FaceKind.Land
                    : FaceKind.Lake;
            }
        }
    }
}
