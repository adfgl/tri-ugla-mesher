using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Topology
{
    public sealed class NodeSplitter(Stack<Edge> illigalEdges)
        : Splitter<Node>(illigalEdges)
    {
        public override Splitter<Node> Split(Node target, Node node)
        {
            if (node.Kind == NodeKind.Normal)
            {
                target.Kind = NodeKind.Normal;
            }
            return this;
        }
    }
}
