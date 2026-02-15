using TriUgla.ExactMath;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Topology;

namespace TriUgla.Mesher.Services
{
    public sealed class EdgeLegalizer(IlligalEdges illigalEdges, EdgeFlipper flipper)
    {
        readonly EdgeFlipper _flipper = flipper;

        public Stack<Face> Affected { get; } = new Stack<Face>();
        public IlligalEdges ToLegalize { get; } = illigalEdges;
        public int Flips { get; private set; } 

        public EdgeLegalizer Legalize()
        {
            Affected.Clear();
            while (ToLegalize.TryPop(out Edge edge))
            {
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
