using System.Runtime.CompilerServices;
using TriUgla.Geometry;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Services
{
    public sealed class Locator(HalfEdgeMesh mesh)
    {
        readonly HalfEdgeMesh _mesh = mesh;

        public HitResult Locate(double x, double y, Face faceStart, double eps)
        {
            double eps2 = eps * eps;

            int stamp = _mesh.Stamps.Face.Next();
            int steps = 0;
            Face curr = faceStart;
            curr.TryVisit(stamp);
            while (true)
            {
                steps++;
                Edge exit = FindExit(curr, x, y);
                Vec4 start = exit.NodeStart.Vertex;
                Vec4 end = exit.NodeEnd.Vertex;

                if (IsZero(Cross(start, end, x, y), eps))
                {
                    if (Close(start, x, y, eps2))
                    {
                        return HitResult.NodeHit(curr, exit.NodeStart, steps);
                    }

                    if (Close(end, x, y, eps2))
                    {
                        return HitResult.NodeHit(curr, exit.NodeEnd, steps);
                    }

                    if (Rect.FromTwoPoints(start.x, start.y, end.x, end.y).Contains(x, y))
                    {
                        return HitResult.EdgeHit(curr, exit, steps);
                    }
                }

                if (exit.Twin is null)
                {
                    return HitResult.None(curr, steps, capped: true);
                }

                Face next = exit.Twin.Face;
                if (!next.TryVisit(stamp))
                {
                    return HitResult.None(curr, steps, capped: true);
                }
                curr = next;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(double value, double eps)
            => value >= -eps && value <= eps;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Close(Vec4 a, double x, double y, double eps2)
        {
            double dx = a.x - x;
            double dy = a.y - y;
            return dx * dx + dy * dy <= eps2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cross(Vec4 a, Vec4 b, double x, double y)
        {
            double ax = x   - a.x, ay = y   - a.y;
            double bx = b.x - a.x, by = b.y - a.y;
            return ax * by - ay * bx;
        }

        public static Edge FindExit(Face face, double x, double y)
        {
            double minCross = double.MaxValue;
            Edge start, curr, best;
            start = curr = best = face.Edge;
            do
            {
                double cross = Cross(curr.NodeStart.Vertex, curr.NodeEnd.Vertex, x, y);
                if (minCross > cross)
                {
                    best = curr;
                    minCross = cross;
                }

                curr = curr.Next;

            } while (start != curr);
            return best;
        }
    }
}
