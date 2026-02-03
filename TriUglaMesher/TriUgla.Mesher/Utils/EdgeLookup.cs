using System;
using System.Collections.Generic;
using System.Text;
using TriUgla.Mesher.MeshCore;

namespace TriUgla.Mesher.Utils
{
    public static class EdgeLookup
    {
        public static Edge? FindEdge(Node start, Node end)
        {
            return FindDirectedEdge(start, end) ?? FindDirectedEdge(end, start);
        }


        public static Edge? FindDirectedEdge(Node start, Node end)
        {
            Edge? edge = null;

            Edge e0 = start.Edge;
            Edge e = e0;
            do
            {
                if (ReferenceEquals(e.NodeEnd, end))
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
            } while (!ReferenceEquals(e0, e));

            return edge;
        }
    }
}
