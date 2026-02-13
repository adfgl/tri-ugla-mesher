using TriUgla.ExactMath;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Topology;

namespace TriUgla.Mesher.Services
{
    public sealed class EdgeLegalizer(Predicates predicates, Stack<Edge> illigalEdges)
    {
        readonly EdgeFlipper _flipper = new EdgeFlipper(predicates, illigalEdges);

        public Stack<Face> Affected { get; } = new Stack<Face>();
        public Stack<Edge> ToLegalize { get; } = illigalEdges;
        public int Flips { get; private set; } 

        public EdgeLegalizer Legalzie()
        {
            Affected.Clear();
            while (ToLegalize.Count != 0)
            {
                Edge edge = ToLegalize.Pop();
                Affected.Push(edge.Face);
                if (_flipper.CanFlip(edge, out bool should) && should)
                {
                    _flipper.Flip(edge);
                    Flips++;
                }
            }
            return this;
        }
    }
}
