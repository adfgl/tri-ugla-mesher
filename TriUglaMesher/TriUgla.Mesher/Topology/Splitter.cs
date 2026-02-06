using System.Runtime.CompilerServices;
using TriUgla.Mesher.HalfEdge;

namespace TriUgla.Mesher.Topology
{
    public abstract class Splitter<Target>(Stack<Edge> illigalEdges)
    {
        protected readonly Stack<Edge> _illigalEdges = illigalEdges;

        public abstract Splitter<Target> Split(Target target, Node node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void MakeTwinEdges(Node a, Node b, out Edge ab, out Edge ba)
        {
            ab = new Edge { NodeStart = a };
            ba = new Edge { NodeStart = b };
            ElementLinker.LinkEdgeTwins(ab, ba);
        }
    }
}
