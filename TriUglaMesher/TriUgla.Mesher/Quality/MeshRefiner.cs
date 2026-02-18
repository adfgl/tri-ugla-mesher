using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using TriUgla.ExactMath;
using TriUgla.Geometry;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Helpers;
using TriUgla.Mesher.Services;
using TriUgla.Mesher.Topology;

namespace TriUgla.Mesher.Quality
{
    public sealed class MeshRefiner(
        Predicates predicates,
        Locator locator,
        EdgeLegalizer legalizer,
        EdgeSplitter edgeSplitter,
        NodeInserter nodeInserter)
    {
        readonly Predicates _predicates = predicates;
        readonly Locator _locator = locator;
        readonly EdgeLegalizer _legalizer = legalizer;
        readonly EdgeSplitter _edgeSplitter = edgeSplitter;
        readonly NodeInserter _nodeInserter = nodeInserter;

        readonly List<Edge> _edges = new List<Edge>(256);
        readonly Queue<Edge> _edgeQueue = new Queue<Edge>(64);
        readonly Queue<Face> _faceQueue = new Queue<Face>(64);
        readonly Dictionary<Face, FaceProgress> _progress = new Dictionary<Face, FaceProgress>(1024);

        public int Refine(List<Face> faces, FaceRanker ranker, in RefineSettings settings)
        {
            Clear();
            FillQueues(faces, ranker, in settings);

            int steinersInserted = 0;
            while (_edgeQueue.Count != 0 || _faceQueue.Count != 0)
            {
                if (steinersInserted >= settings.MaxSteiners)
                {
                    break;
                }

                if (_edgeQueue.TryDequeue(out var edge) && ProcessEncroachedSegment(edge))
                {
                    steinersInserted++;
                }
                else if (_faceQueue.TryDequeue(out var face) && ProcessBadFace(face, ranker, in settings))
                {
                    steinersInserted++;
                }
                DrainAffected(ranker, in settings);
            }
            return steinersInserted;
        }

        void Clear()
        {
            _faceQueue.Clear();
            _edgeQueue.Clear();
            _edges.Clear();
            _progress.Clear();
        }

        public bool ProcessBadFace(Face f, FaceRanker ranker, in RefineSettings settings)
        {
            if (!ProcessableFace(f))
            {
                return false;
            }

            double badness = ranker.Rank(f);
            if (badness <= 0)
            {
                return false;
            }

            if (!ShouldRequeueFace(f, badness, in settings))
            {
                return false;
            }

            Vec2 candidate = Candidate(f);
            HitResult hit = _locator.Locate(candidate.x, candidate.y, f, 1e-8);
            if (SkipHit(in hit))
            {
                _faceQueue.Enqueue(f);
                return false;
            }

            if (EnqueueEncroached(candidate.x, candidate.y) != 0)
            {
                _faceQueue.Enqueue(f);
                return false;
            }

            _nodeInserter.Insert(in hit, candidate, NodeKind.Steiner);
            return true;
        }

        public bool ProcessEncroachedSegment(Edge edge)
        {
            Node node = new Node(Candidate(edge), NodeKind.Steiner);

            _edgeSplitter.Split(edge, node);
            Edge edge1 = _edgeSplitter.FirstHalf;
            Edge edge2 = _edgeSplitter.SecondHalf;

            _edges.Remove(edge);
            _edges.Add(edge1);
            _edges.Add(edge2);

            if (Enchrouched(edge1))
            {
                _edgeQueue.Enqueue(edge1);
            }

            if (Enchrouched(edge2))
            {
                _edgeQueue.Enqueue(edge2);
            }
            return true;
        }

        static bool SkipHit(in HitResult hit)
        {
            if (hit.Kind == HitKind.None || hit.Kind == HitKind.Node)
            {
                return true;
            }
            if (hit.Kind == HitKind.Face && !ProcessableFace(hit.Face!))
            {
                return true;
            }
            return false;
        }

        int EnqueueEncroached(double x, double y)
        {
            int count = 0;
            foreach (Edge item in _edges)
            {
                if (Enchrouched(item, x, y) && VisibleFromInterior(item, x, y))
                {
                    _edgeQueue.Enqueue(item);
                    count++;
                }
            }
            return count;
        }

        bool VisibleFromInterior(Edge edge, double x, double y)
        {
            Node start = edge.NodeStart;
            Node end = edge.NodeEnd;

            double cx = (start.Vertex.x + end.Vertex.x) * 0.5;
            double cy = (start.Vertex.y + end.Vertex.y) * 0.5;
            foreach (Edge other in _edges)
            {
                if (edge == other ||
                    other.Contains(start) ||
                    other.Contains(end))
                {
                    continue;
                }

                Vec4 a = other.NodeStart.Vertex;
                Vec4 b = other.NodeEnd.Vertex;

                if (_predicates.Intersects(a.x, a.y, b.x, b.y, cx, cy, x, y) == 1)
                {
                    return false;
                }
            }
            return true;
        }

        void DrainAffected(FaceRanker ranker, in RefineSettings settings)
        {
            _legalizer.Legalize();

            Stack<Face> affected = _legalizer.Affected;
            while (affected.Count != 0)
            {
                Face f = affected.Pop();

                if (!ProcessableFace(f))
                {
                    continue;
                }

                double b = ranker.Rank(f);
                if (b <= 0)
                {
                    continue;
                }
                    
                if (ShouldRequeueFace(f, b, in settings))
                {
                    _faceQueue.Enqueue(f);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vec4 Candidate(Edge edge)
        {
            Vec4 start = edge.NodeStart.Vertex;
            Vec4 end = edge.NodeEnd.Vertex;
            double dx = (end.x + start.x) * 0.5;
            double dy = (end.y + start.y) * 0.5;
            return Interpolation.Interpolate(new Vec2(dx, dy), edge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vec2 Candidate(Face face)
        {
            return face.CircumCircle.center;
        }

        void FillQueues(List<Face> faces, FaceRanker ranker, in RefineSettings settings)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                Face f = faces[i];
                if (ProcessableFace(f))
                {
                    double b = ranker.Rank(f);
                    if (b > 0 && ShouldRequeueFace(f, b, in settings))
                    {
                        _faceQueue.Enqueue(f);
                    }
                }

                Edge e0 = f.Edge;
                Edge e = e0;
                do
                {
                    if (e.Constrained && Unique(e))
                    {
                        _edges.Add(e);

                        if (Enchrouched(e))
                        {
                            _edgeQueue.Enqueue(e);
                        }
                    }

                    e = e.Next;
                } while (e0 != e);
            }
        }

        public bool Enchrouched(Edge edge, double x, double y)
        {
            Vec4 start = edge.NodeStart.Vertex;
            Vec4 end = edge.NodeEnd.Vertex;
            return _predicates.InCircleDiameter(start.x, start.y, end.x, end.y, x, y) == 1;
        }

        public bool Enchrouched(Edge edge)
        {
            bool encrhouched = false;

            Edge e0 = edge.NodeStart.Edge;
            Edge e = e0;
            do
            {
                if (e != edge)
                {
                    Vec4 point = e.NodeEnd.Vertex;
                    if (Enchrouched(e, point.x, point.y))
                    {
                        encrhouched = true;
                        break;
                    }
                }

                e = e.Prev.Twin!;
            } while (e0 != e);
            return encrhouched;
        }

        bool Unique(Edge edge)
        {
            foreach (Edge s in _edges)
            {
                if (s.Contains(edge.NodeStart) && s.Contains(edge.NodeEnd))
                {
                    return false;
                }
            }
            return true;
        }

        static bool ProcessableFace(Face f) => f.Kind != FaceKind.Outside;

        bool ShouldRequeueFace(Face f, double badness, in RefineSettings settings)
        {
            if (_progress.TryGetValue(f, out FaceProgress p))
            {
                if (badness + settings.ImproveEps < p.BestBadness)
                {
                    p.BestBadness = badness;
                    p.Stagnation = 0;
                    _progress[f] = p;
                    return true;
                }

                p.Stagnation++;
                _progress[f] = p;
                return p.Stagnation <= settings.FaceStagnationBudget;
            }

            _progress.Add(f, new FaceProgress
            {
                BestBadness = badness,
                Stagnation = 0
            });
            return true;
        }

        struct FaceProgress
        {
            public double BestBadness; // lower is better
            public int Stagnation;
        }
    }
}
