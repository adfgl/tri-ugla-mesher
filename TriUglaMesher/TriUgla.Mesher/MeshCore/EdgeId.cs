using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public readonly struct EdgeId(Id value)
    {
        public readonly Id value = value;
    }
}