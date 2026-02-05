public readonly struct FaceView(Mesh mesh, FaceId id)
{
    public readonly FaceId id = id;
    readonly Mesh _m = mesh;

    ref Face Self() => ref _m.Faces.Ref(id);

    public EdgeView Edge() => new EdgeView(_m, Self().edge);
}