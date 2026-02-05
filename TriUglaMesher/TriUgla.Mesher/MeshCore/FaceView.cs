public readonly struct FaceView(Mesh mesh, FaceId id)
{
    public readonly FaceId id = id;
    readonly Mesh _m = mesh;

    internal ref Face Ref() => ref _m.Faces.Ref(id);

    public EdgeView Edge() => new EdgeView(_m, Ref().edge);
}