using TriUgla.HalfEdge;
using TriUgla.Mesher.Services;

namespace TriUgla.Mesher
{
    public class Loop
    {
        public List<Node> Nodes { get; set; } = new List<Node>();

        public Loop Close()
        {
            if (Nodes.Count != 0 && Nodes[0] != Nodes[^1])
            {
                Nodes.Add(Nodes[0]);
            }
            return this;
        }

        public List<Edge> Edges(List<Edge> edges)
        {
            Close();
            int n = Nodes.Count;
            for (int i = 0; i < n - 1; i++)
            {
                Edge? edge = EdgeFinder.FindDirected(Nodes[i], Nodes[i + 1]);
                if (edge is null)
                {
                    throw new Exception();
                }
                edges.Add(edge);
            }
            return edges;
        }
    }
}
