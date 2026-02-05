using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public readonly struct NodeId(Id value)
    {
        public readonly Id value = value;
    }
}