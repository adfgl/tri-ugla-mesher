namespace TriUgla.Mesher.MeshCore
{
    public readonly struct EdgeView
    {
        readonly Mesh _m;
        public readonly EdgeId id;

        public EdgeView(Mesh mesh, EdgeId id)
        {
            _m = mesh;
            this.id = id;
        }

        internal ref Edge Ref() => ref _m.Ref(id);

        public EdgeView Next() => new EdgeView(_m, Ref().next);
        public EdgeView Prev() => new EdgeView(_m, Ref().prev);
        public EdgeView Twin() => new EdgeView(_m, Ref().twin);
        public NodeView Start() => new NodeView(_m, Ref().start);
        public NodeView End() => new NodeView(_m, Ref().end);
        public FaceView Face() => new FaceView(_m, Ref().face);
    }
}