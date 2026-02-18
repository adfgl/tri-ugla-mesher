using System.Xml.Linq;

namespace TriUgla.HalfEdge
{
    public sealed class HalfEdgeMesh(Face root)
    {
        public VisitStamps Stamps { get; } = new VisitStamps();
        public Face Root { get; } = root;

        public MeshIndexator Indexate() => new MeshIndexator().Indexate(this);
        public HalfEdgeMesh Classify() => new FaceClassifier().Classify(this);

        public bool Remove(Face face)
        {
            ElementInvalidator.Invalidate(face);
            ElementLinker.Unlink(face);
            return true;
        }
 
        public int ForeachNode<T>(Node from, ref T processor) where T : INodeProcessor
        {
            int hits = 0;
            Stack<Face> stack = new Stack<Face>(64);
            int faceStamp = Stamps.Face.Next();
            int nodeStamp = Stamps.Node.Next();
            if (!from.TryVisit(faceStamp))
            {
                return hits;
            }
            stack.Push(from.Edge.Face);

            while (stack.Count > 0)
            {
                Face face = stack.Pop();

                Edge e0 = face.Edge;
                Edge e = e0;
                do
                {
                    Node n = e.NodeStart;
                    if (n.TryVisit(nodeStamp))
                    {
                        hits++;
                        if (!processor.ProcessAndContinue(n))
                        {
                            return hits;
                        }
                    }

                    if (e.Twin is not null)
                    {
                        Face neighbour = e.Twin.Face;
                        if (neighbour.TryVisit(faceStamp))
                        {
                            stack.Push(neighbour);
                        }
                    }
                    e = e.Next;
                } while (e0 != e);
            }
            return hits;
        }

        public int ForeachEdge<T>(Edge from, ref T processor) where T : IEdgeProcessor
        {
            int hits = 0;
            Stack<Face> stack = new Stack<Face>(64);
            int faceStamp = Stamps.Face.Next();
            int edgeStamp = Stamps.Edge.Next();
            if (!from.TryVisit(faceStamp))
            {
                return hits;
            }
            stack.Push(from.Face);

            while (stack.Count > 0)
            {
                Face face = stack.Pop();

                Edge e0 = face.Edge;
                Edge e = e0;
                do
                {
                    if (e.TryVisit(edgeStamp))
                    {
                        hits++;
                        if (!processor.ProcessAndContinue(e))
                        {
                            return hits;
                        }
                    }

                    if (e.Twin is not null)
                    {
                        Face neighbour = e.Twin.Face;
                        if (neighbour.TryVisit(faceStamp))
                        {
                            stack.Push(neighbour);
                        }
                    }
                    e = e.Next;
                } while (e0 != e);
            }
            return hits;
        }

        public int ForeachFace<T>(ref T processor) where T : IFaceProcessor
            => ForeachFace(Root, ref processor);

        public int ForeachFace<T>(Face from, ref T processor) where T : IFaceProcessor
        {
            int hits = 0;
            Stack<Face> stack = new Stack<Face>(64);
            int faceStamp = Stamps.Face.Next();
            if (!from.TryVisit(faceStamp))
            {
                return hits;
            }
            stack.Push(from);

            while (stack.Count > 0)
            {
                Face face = stack.Pop();
                hits++;

                if (!processor.ProcessAndContinue(face))
                {
                    break;
                }

                Edge e0 = face.Edge;
                Edge e = e0;
                do
                {
                    if (e.Twin is not null)
                    {
                        Face neighbour = e.Twin.Face;
                        if (neighbour.TryVisit(faceStamp))
                        {
                            stack.Push(neighbour);
                        }
                    }
                    e = e.Next;
                } while (e0 != e);
            }
            return hits;
        }
    }
}
