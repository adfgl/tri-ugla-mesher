namespace TriUgla.Mesher.Services
{
    public sealed class VisitStamps
    {
        readonly VisitStamp _faceStamp = new VisitStamp();
        readonly VisitStamp _nodeStamp = new VisitStamp();
        readonly VisitStamp _edgeStamp = new VisitStamp();

        public VisitStamp FaceStamp => _faceStamp;
        public VisitStamp NodeStamp => _nodeStamp;
        public VisitStamp EdgeStamp => _edgeStamp;
    }
}
