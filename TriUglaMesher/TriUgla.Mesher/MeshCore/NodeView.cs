namespace TriUgla.Mesher.MeshCore
{
    public readonly struct NodeView
    {
        readonly Mesh _m;
        public readonly NodeId id;

        public NodeView(Mesh mesh, NodeId id)
        {
            _m = mesh;
            this.id = id;
        }

        internal ref Node Ref() => ref _m.Ref(id);
        public EdgeView Edge() => new EdgeView(_m, Ref().edge);
    }
}