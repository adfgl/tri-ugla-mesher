using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Services
{
    public sealed class IlligalEdges
    {
        readonly Stack<Edge> _stack = new Stack<Edge>();
        public int Count => _stack.Count;
        public void Push(Edge edge) => _stack.Push(edge);
        public bool TryPop(out Edge edge) => _stack.TryPop(out edge!);
    }
}
