using TriUgla.Geometry;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Helpers;

namespace TriUgla.Mesher
{
    public class ConstraintSpan(Node from, Node to)
    {
        public string? Name { get; set; }
        public Node From { get; set; } = from;
        public Node To { get; set; } = to;

        public List<Edge> Edges(List<Edge> edges)
        {
            SegmentQueue queue = new SegmentQueue();
            queue.Enqueue(From, To);

            Vec2 dir = Direction(From, To);
            while (queue.TryDequeue(out Node start, out Node end))
            {
                Edge e0 = start.Edge;
                Edge e = e0;
                do
                {
                    if (e.NodeEnd == end)
                    {
                        edges.Add(e);
                        break;
                    }

                    if (Vec2.Dot(dir, Direction(e.NodeStart, e.NodeEnd)) >= 0.99)
                    {
                        edges.Add(e);
                        queue.Enqueue(e.NodeEnd, end);
                        break;
                    }

                    e = e.Prev.Twin!;
                } while (e != e0);
            }
            return edges;
        }

        public static Vec2 Direction(Node a, Node b)
        {
            Vec4 start = a.Vertex;
            Vec4 end = b.Vertex;

            double dx = end.x - start.x;
            double dy = end.y - start.y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            return new Vec2(dx / length, dy / length);
        }
    }
}
