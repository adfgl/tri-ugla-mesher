public readonly struct EdgeView
{
    readonly Mesh _m = mesh;
    public readonly EdgeId id = id;

    public EdgeView(Mesh mesh, EdgeId id)
    {
        _m = mesh;
        this.id = id;
    }

    internal ref Edge Ref() => ref _m.Edges.Ref(id);

    public EdgeView Next() => new EdgeView(_m, Ref().next);
    public EdgeView Prev() => new EdgeView(_m, Ref().prev);
    public EdgeView Twin() => new EdgeView(_m, Ref().twin);
    public NodeView NodeStart() => new NodeView(_m, Ref().nodeStart);
    public NodeView NodeEnd() => new NodeView(_m, Ref().nodeEnd);
    public FaceView Face() => new FaceView(_m, Ref().face);
}