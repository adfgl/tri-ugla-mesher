using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Helpers
{
    public sealed class SegmentQueue(int capacity = 8)
    {
        readonly Queue<Segment> _pairs = new Queue<Segment>(capacity);

        public bool TryDequeue(out Node start, out Node end)
        {
            if (_pairs.TryDequeue(out var pair))
            {
                start = pair.start;
                end = pair.end;
                return true;
            }
            start = end = null!;
            return false;
        }

        public void Enqueue(Node a, Node b)
        {
            if (ReferenceEquals(a, b)) return;
            _pairs.Enqueue(new Segment(a, b));
        }

        readonly struct Segment(Node start, Node end)
        {
            public readonly Node start = start, end = end;
        }
    }
}
