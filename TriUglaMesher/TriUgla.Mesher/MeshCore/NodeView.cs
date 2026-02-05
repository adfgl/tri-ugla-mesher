public readonly struct NodeView(Mesh mesh, NodeId id)
{
    public readonly NodeId id = id;

    readonly Mesh _m = mesh;

    ref Node Self() => ref _m.Nodes.Ref(id);

    public EdgeView Edge() => new EdgeView(_m, Self().edge);
}