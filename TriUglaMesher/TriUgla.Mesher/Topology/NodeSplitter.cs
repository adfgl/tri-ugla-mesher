using TriUgla.HalfEdge;
using TriUgla.Mesher.Services;

namespace TriUgla.Mesher.Topology
{
    public sealed class NodeSplitter(IlligalEdges illigalEdges)
        : Splitter<Node>(illigalEdges)
    {
        protected override Splitter<Node> SplitInternal(Node target, Node node)
        {
            if (node.Kind == NodeKind.Normal)
            {
                target.Kind = NodeKind.Normal;
            }
            return this;
        }
    }
}
