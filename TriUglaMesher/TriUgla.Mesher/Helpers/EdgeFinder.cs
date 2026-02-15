using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Helpers
{
    public static class EdgeFinder
    {
        public static Edge? FindDirected(Node a, Node b)
        {
            Edge? edge = null;

            Edge e0 = a.Edge;
            Edge e = e0;
            do
            {
                if (e.NodeEnd == b)
                {
                    edge = e;
                    break;
                }
                Edge? next = e.Prev.Twin;
                if (next is null)
                {
                    break;
                }
                e = next;
            } while (e0 != e);

            return edge;
        }

        public static Edge? Find(Node a, Node b)
        {
            Edge? edge = FindDirected(a, b);
            edge ??= FindDirected(b, a);
            return edge;
        }
    }
}
