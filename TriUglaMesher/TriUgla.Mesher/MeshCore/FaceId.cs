using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public readonly struct FaceId(Id value)
    {
        public readonly Id value = value;
    }
}