using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TriUgla.HalfEdge
{
    public sealed class FaceClassifier
    {
        readonly Queue<Seed> _queue;
        readonly Stack<Face> _stack;

        public FaceClassifier(int queueCapacity = 64, int stackCapacity = 256)
        {
            _queue = new Queue<Seed>(Math.Max(0, queueCapacity));
            _stack = new Stack<Face>(Math.Max(0, stackCapacity));
        }

        readonly struct Seed(Face face, FaceKind kind)
        {
            public readonly Face Face = face;
            public readonly FaceKind Kind = kind;
        }

        public HalfEdgeMesh Declassify(HalfEdgeMesh mesh)
        {
            _queue.Clear();
            _stack.Clear();

            int stamp = mesh.Stamps.Face.Next();
            Face root = mesh.Root;

            if (!root.TryVisit(stamp))
                return mesh;

            _stack.Push(root);

            while (_stack.Count != 0)
            {
                Face f = _stack.Pop();
                f.Kind = FaceKind.Undefined;

                ForEachNeighbour(f, (e, n) =>
                {
                    if (n.TryVisit(stamp))
                        _stack.Push(n);
                });
            }

            return mesh;
        }

        public HalfEdgeMesh Classify(HalfEdgeMesh mesh)
        {
            Declassify(mesh);

            _queue.Clear();
            _stack.Clear();

            Face outside = FindOutsideSeed(mesh);

            _queue.Enqueue(new Seed(outside, FaceKind.Outside));

            while (_queue.Count != 0)
            {
                Seed s = _queue.Dequeue();
                PropagateFrom(mesh, s.Face, s.Kind);
            }

            return mesh;
        }

        void PropagateFrom(HalfEdgeMesh mesh, Face start, FaceKind kind)
        {
            if (start.Kind == kind)
                return;

            if (start.Kind != FaceKind.Undefined)
            {
                return;
            }

            int floodStamp = mesh.Stamps.Face.Next();
            _stack.Clear();

            if (!start.TryVisit(floodStamp))
                return;

            Assign(start, kind);
            _stack.Push(start);

            while (_stack.Count != 0)
            {
                Face f = _stack.Pop();

                ForEachNeighbour(f, (e, n) =>
                {
                    FaceKind nextKind;
                    bool switches = ShouldSwitch(kind, e, out nextKind);

                    if (!switches)
                    {
                        if (n.Kind == FaceKind.Undefined && n.TryVisit(floodStamp))
                        {
                            Assign(n, kind);
                            _stack.Push(n);
                        }
                        return;
                    }

                    if (n.Kind == FaceKind.Undefined)
                    {
                        _queue.Enqueue(new Seed(n, nextKind));
                    }
                    else if (n.Kind != nextKind)
                    {
                        return;
                    }
                });
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Assign(Face f, FaceKind kind)
            => f.Kind = kind;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool ShouldSwitch(FaceKind current, Edge e, out FaceKind next)
        {
            bool hasContour = e.HasContour || (e.Twin is not null && e.Twin.HasContour);
            bool hasHole = e.HasHole || (e.Twin is not null && e.Twin.HasHole);

            if (!hasContour && !hasHole)
            {
                next = current;
                return false;
            }

            if (hasContour && hasHole)
            {
#if DEBUG
                throw new InvalidOperationException("Edge has both Contour and Hole constraints.");
#else
            next = current;
            return false;
#endif
            }

            if (hasContour)
            {
                if (current == FaceKind.Land)
                {
                    next = current;  
                    return false;
                }

                next = FaceKind.Land;
                return true;
            }
            else 
            {
                if (current == FaceKind.Lake)
                {
                    next = current;  
                    return false;
                }

                if (current == FaceKind.Outside)
                {
                    next = current;    
                    return false;
                }

                next = FaceKind.Lake;  
                return true;
            }
        }

        Face FindOutsideSeed(HalfEdgeMesh mesh)
        {
            Face root = mesh.Root;
            if (root.ContainsSuperNode) return root;

            int stamp = mesh.Stamps.Face.Next();

            _stack.Clear();
            if (!root.TryVisit(stamp))
                return root;

            _stack.Push(root);

            while (_stack.Count != 0)
            {
                Face f = _stack.Pop();
                if (f.ContainsSuperNode)
                    return f;

                ForEachNeighbour(f, (e, n) =>
                {
                    if (n.TryVisit(stamp))
                        _stack.Push(n);
                });
            }

            return root;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ForEachNeighbour(Face f, Action<Edge, Face> action)
        {
            Edge e0 = f.Edge;
            Edge e = e0;
            do
            {
                Edge? t = e.Twin;
                if (t is not null)
                    action(e, t.Face);

                e = e.Next;
            }
            while (e != e0);
        }
    }
}
