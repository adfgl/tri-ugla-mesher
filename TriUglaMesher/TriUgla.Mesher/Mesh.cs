using TriUgla.Mesher.MeshCore;
using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher
{
    public sealed class Mesh(int capacity = 16)
    {
        public Store<Node> Nodes = new Store<Node>(capacity * 2 + 1);
        public Store<Edge> Edges = new Store<Edge>(capacity * 2 + 1);
        public Store<Face> Faces = new Store<Face>(capacity);

        public ref Node Ref(NodeId id) => ref Nodes.Ref(id.value);
        public ref Edge Ref(EdgeId id) => ref Edges.Ref(id.value);
        public ref Face Ref(FaceId id) => ref Faces.Ref(id.value);

        public NodeView Node(NodeId id) => new NodeView(this, id);
        public EdgeView Edge(EdgeId id) => new EdgeView(this, id);
        public FaceView Face(FaceId id) => new FaceView(this, id);
    }
}
