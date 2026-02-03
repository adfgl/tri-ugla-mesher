namespace TriUgla.Mesher.MeshCore
{
    public class MeshElement
    {
        int _mark = 0;

        public int Payload { get; set; } = -1;

        public bool TryVisit(int stamp)
        {
            if (_mark != stamp)
            {
                _mark = stamp;
                return true;
            }
            return false;
        }
    }
}
