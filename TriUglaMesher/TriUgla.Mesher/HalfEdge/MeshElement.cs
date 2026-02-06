namespace TriUgla.Mesher.HalfEdge
{
    public class MeshElement
    {
        int _visitStamp = 0;

        public bool Invalid { get; internal set; }
        public int Payload { get; set; }

        public bool TryVisit(int stamp)
        {
            if (_visitStamp != stamp)
            {
                _visitStamp = stamp;
                return true;
            }
            return false;
        }
    }
}
