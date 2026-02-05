using TriUgla.Mesher;
using TriUgla.Mesher.MeshCore;

public readonly struct FaceView(Mesh mesh, FaceId id)
{
    public readonly FaceId id = id;
    readonly Mesh _m = mesh;

    internal ref Face Ref() => ref _m.Ref(id);

    public EdgeView Edge() => new EdgeView(_m, Ref().edge);
}