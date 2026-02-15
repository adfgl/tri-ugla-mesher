using System.Runtime.CompilerServices;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Services;

namespace TriUgla.Mesher.Topology
{
    public abstract class Splitter<Target>(IlligalEdges illigalEdges)
    {
        protected readonly IlligalEdges _illigalEdges = illigalEdges;

        public int SplitCount { get; protected set; } = 0;

        public Splitter<Target> Split(Target target, Node node)
        {
            SplitCount++;
            return SplitInternal(target, node);
        }

        protected abstract Splitter<Target> SplitInternal(Target target, Node node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void MakeTwinEdges(out Edge ab, out Edge ba)
        {
            ab = new Edge();
            ba = new Edge();
            ElementLinker.LinkEdgeTwins(ab, ba);
        }
    }
}
