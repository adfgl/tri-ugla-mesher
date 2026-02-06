using System.Runtime.CompilerServices;
using TriUgla.Mesher.HalfEdge;
using TriUgla.Mesher.Topology;
using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.Services
{
    public sealed class Locator(VisitStamps stamps, Predicates predicates)
    {
        readonly VisitStamps _stamps = stamps;
        readonly Predicates _predicates = predicates;

        public HitResult Locate(double x, double y, Face start)
        {
            int stamp = _stamps.FaceStamp.Next();
            int steps = 0;
            Face curr = start;
            curr.TryVisit(stamp);

            Vec2 pos = new Vec2(x, y);
            while (true)
            {
                steps++;
                Edge exit = FindExit(curr, pos);
                Vec2 a = exit.NodeStart.Vertex.ToVec2();
                Vec2 b = exit.NodeEnd.Vertex.ToVec2();

                int orientation = _predicates.OrientAprox(a, b, pos);
                if (orientation == 1)
                {
                    return HitResult.FaceHit(curr, curr, steps);
                }
                    
                if (orientation == 0)
                {
                    if (_predicates.Close(a, pos))
                    {
                        return HitResult.NodeHit(curr, exit.NodeStart, steps);
                    }

                    if (_predicates.Close(b, pos))
                    {
                        return HitResult.NodeHit(curr, exit.NodeEnd, steps);
                    }

                    if (Rectangle.FromTwoPoints(a, b).Contains(pos))
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

        public static Edge FindExit(Face face, Vec2 pos)
        {
            double minCross = double.MaxValue;
            Edge start, curr, best;
            start = curr = best = face.Edge;
            do
            {
                Vec2 a = curr.NodeStart.Vertex.ToVec2();
                Vec2 b = curr.NodeEnd.Vertex.ToVec2();
                double cross = Vec2.Cross(a, b, pos);
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

    public enum HitKind : byte
    {
        None,
        Face,
        Edge,
        Node
    }

    public readonly struct HitResult
    {
        public readonly HitKind Kind;
        public readonly Face At;
        public readonly Face? Face;
        public readonly Edge? Edge;
        public readonly Node? Node;
        public readonly int Steps;
        public readonly bool Capped;

        private HitResult(
            HitKind kind,
            Face at,
            Face? f,
            Edge? e,
            Node? n,
            int steps,
            bool capped)
        {
            Kind = kind;
            At = at;
            Face = f;
            Edge = e;
            Node = n;
            Steps = steps;
            Capped = capped;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitResult None(Face at, int steps, bool capped = false)
       => new HitResult(
           HitKind.None,
           at,
           null,
           null,
           null,
           steps,
           capped);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitResult FaceHit(Face at, Face f, int steps, bool capped = false)
            => new HitResult(
                HitKind.Face,
                at,
                f,
                null,
                null,
                steps,
                capped);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitResult EdgeHit(Face at, Edge e, int steps, bool capped = false)
            => new HitResult(
                HitKind.Edge,
                at,
                null,
                e,
                null,
                steps,
                capped);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitResult NodeHit(Face at, Node n, int steps, bool capped = false)
            => new HitResult(
                HitKind.Node,
                at,
                null,
                null,
                n,
                steps,
                capped);
    }
}
