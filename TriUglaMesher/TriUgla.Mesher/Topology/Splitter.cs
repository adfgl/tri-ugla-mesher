using System.Runtime.CompilerServices;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Topology
{
    public abstract class Splitter<Target>(Stack<Edge> illigalEdges)
    {
        protected readonly Stack<Edge> _illigalEdges = illigalEdges;

        public abstract Splitter<Target> Split(Target target, Node node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void MakeTwinEdges(out Edge ab, out Edge ba)
        {
            ab = new Edge();
            ba = new Edge();
            ElementLinker.LinkEdgeTwins(ab, ba);
        }
    }
}
