using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.MeshCore
{
    public sealed class Mesh(int capacity = 16)
    {
        public Store<Node> Nodes = new Store<Node>(capacity * 2 + 1);
        public Store<Edge> Edges = new Store<Edge>(capacity * 2 + 1);
        public Store<Face> Faces = new Store<Face>(capacity);


    }
}
