using TriUgla.Mesher.MeshCore;

namespace TriUgla.Mesher.Services
{
    public sealed class FaceSplitter(Mesh mesh, Stack<EdgeId> illigals)
    {
        readonly Mesh _mesh = mesh;
        readonly Linker _linker = new Linker(mesh);
        readonly Stack<EdgeId> _illigals = illigals;

        public FaceSplitter Split(FaceId faceId, NodeId nodeId)
        {
            Face abcFace = _mesh.Ref(faceId);
            Edge abEdge = _mesh.Ref(abcFace.edge);
            Edge bcEdge = _mesh.Ref(abEdge.next);

            EdgeId ab = abcFace.edge;
            EdgeId bc = abEdge.next;
            EdgeId ca = abEdge.prev;

            NodeId a = abEdge.start;
            NodeId b = abEdge.end;
            NodeId c = bcEdge.end;
            NodeId d = nodeId;

            _linker.MakeTwins(out var ad, out var da);
            _linker.MakeTwins(out var bd, out var db);
            _linker.MakeTwins(out var cd, out var dc);

            Face bcd = new Face();
            Face cad = new Face();
            bcd.context = cad.context = abcFace.context;

            FaceId bcdId = new FaceId(_mesh.Faces.Rent(bcd));
            FaceId cadId = new FaceId(_mesh.Faces.Rent(bcd));

            _illigals.Push(ab);
            _illigals.Push(bc);
            _illigals.Push(ca);
        }
    }
}
