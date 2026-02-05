public readonly struct NodeView
{
    readonly Mesh _m;
    public readonly NodeId id;

    public NodeView(Mesh mesh, NodeId id)
    {
        _m = mesh;
        this.id = id;
    }

    ref Node Self() => ref _m.Nodes.Ref(id);
    public EdgeView Edge() => new EdgeView(_m, Self().edge);
}