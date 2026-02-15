using TriUgla.Geometry;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Helpers;

namespace TriUgla.Mesher
{
    public class Loop
    {
        public string? Name { get; set; }
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
                    throw new Exception("");
                }
                edges.Add(edge);
            }
            return edges;
        }

        public double SignedArea()
        {
            Close();

            int n = Nodes.Count;
            if (n < 4) return 0;

            double sum = 0;
            for (int i = 0; i < n - 1; i++)
            {
                Vec4 a = Nodes[i].Vertex;
                Vec4 b = Nodes[i + 1].Vertex;
                sum += a.x * b.y - b.x * a.y;
            }
            return 0.5 * sum;
        }
    }
}
