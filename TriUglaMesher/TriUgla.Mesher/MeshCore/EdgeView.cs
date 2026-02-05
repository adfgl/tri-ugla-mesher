public readonly struct EdgeView(Mesh mesh, EdgeId id)
{
    public readonly EdgeId id = id;
    readonly Mesh _m = mesh;

    ref Edge Self() => ref _m.Edges.Ref(id);

    public EdgeView Next() => new EdgeView(_m, Self().next);
    public EdgeView Prev() => new EdgeView(_m, Self().prev);
    public EdgeView Twin() => new EdgeView(_m, Self().twin);
    public NodeView NodeStart() => new NodeView(_m, Self().nodeStart);
    public NodeView NodeEnd() => new NodeView(_m, Self().nodeEnd);
    public FaceView Face() => new FaceView(_m, Self().face);
}