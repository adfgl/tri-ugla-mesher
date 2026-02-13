using System;
using System.Collections.Generic;
using System.Text;
using TriUgla.ExactMath;
using TriUgla.Geometry;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Helpers;
using TriUgla.Mesher.Topology;

namespace TriUgla.Mesher.Services
{
    public sealed class SpanInserter(Predicates predicates, Stack<Edge> illigals)
    {
        readonly Predicates _predicates = predicates;
        readonly Stack<Edge> _illigals = illigals;
        readonly EdgeFlipper _edgeFlipper = new EdgeFlipper(predicates, illigals);
        readonly EdgeSplitter _edgeSplitter = new EdgeSplitter(illigals);

        public bool SplitAlongTheWay { get; set; } = false;

        public List<Edge> Insert(Node a, Node b, EdgeKind kind)
        {
            List<Edge> edges = new List<Edge>(8);
            SegmentQueue queue = new SegmentQueue();
            queue.Enqueue(a, b);

            while (queue.TryDequeue(out Node start, out Node end))
            {
                Edge? entrance = FindEntrance(start, end);
                if (entrance is null)
                {
                    throw new Exception("Failed to locate entrance.");
                }

                if (entrance.NodeEnd == end)
                {
                    entrance.Constrain(kind);
                    edges.Add(entrance);
                    break;
                }

                if (Collinear(entrance, end))
                {
                    entrance.Constrain(kind);
                    queue.Enqueue(entrance.NodeEnd, end);
                    edges.Add(entrance);
                    continue;
                }

                Edge crossed = entrance.Next;
                if (ShouldFlipEdge(crossed))
                {
                    _edgeFlipper.Flip(crossed);

                    Node opp = crossed.Twin!.Opposite;
                    if (Collinear(crossed, start) && Collinear(crossed, end))
                    {
                        crossed.Constrain(kind);
                        queue.Enqueue(opp, end);
                    }
                    else
                    {
                        queue.Enqueue(start, end);
                    }
                    _illigals.Push(crossed);
                }
                else
                {
                    Node steiner = new Node(Instersection(crossed, start, end), NodeKind.SteinerEdge);

                    _edgeSplitter.Split(crossed, steiner);
                    queue.Enqueue(steiner, end);
                    EdgeFinder.FindDirected(start, steiner)!.Constrain(kind);
                }
            }
            return edges;
        }

        bool Collinear(Edge edge, Node end) => 0 == Orient(edge, end);

        bool ShouldFlipEdge(Edge e)
            => !SplitAlongTheWay && _edgeFlipper.CanFlip(e, out _);

        static Vec4 Instersection(Edge crossed, Node start, Node end)
        {
            Vec4 p1 = start.Vertex;
            Vec4 p2 = end.Vertex;
            Vec4 q1 = crossed.NodeStart.Vertex;
            Vec4 q2 = crossed.NodeEnd.Vertex;

            if (!Intersection.Intersect(
                new Vec2(p1.x, p1.y), 
                new Vec2(p2.x, p2.y), 
                new Vec2(q1.x, q1.y),
                new Vec2(q2.x, q2.y), out Vec2 inter))
            {
                throw new Exception("expected intersection");
            }
            return Interpolation.Interpolate(inter, crossed);
        }

        public int Orient(Edge edge, Node node)
        {
            Vec4 a = edge.NodeStart.Vertex;
            Vec4 b = edge.NodeEnd.Vertex;
            Vec4 c = node.Vertex;
            return _predicates.Orient(a.x, a.y, b.x, b.y, c.x, c.y);
        }

        public Edge? FindEntrance(Node start, Node end)
        {
            Edge e0 = start.Edge;
            Edge e1 = e0;
            do
            {
                Edge e2 = e1.Prev;
                int o1 = Orient(e1, end);
                int o2 = Orient(e2, end);
                if (o1 == 0 && o2 == 1 || o1 == 1 && o2 == 1)
                {
                    return e1;
                }

                Edge? twin = e2.Twin;
                if (o1 == 1 && o2 == 0)
                {
                    return twin;
                }

                if (twin is null)
                {
                    break;
                }
                e1 = twin;

            } while (e0 != e1);

            return null;
        }
    }
}
