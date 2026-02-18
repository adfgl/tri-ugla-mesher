namespace TriUgla.HalfEdge
{
    public class MeshIndexator
    {
        readonly List<Face> _faces = new List<Face>();
        readonly List<Edge> _edges = new List<Edge>();
        readonly List<Node> _nodes = new List<Node>();

        public IReadOnlyList<Face> Faces => _faces;
        public IReadOnlyList<Edge> Edges => _edges;
        public IReadOnlyList<Node> Nodes => _nodes;

        public void Clear()
        {
            _faces.Clear();
            _edges.Clear();
            _nodes.Clear();
        }

        public MeshIndexator Indexate(HalfEdgeMesh mesh)
        {
            Clear();
            Face root = mesh.Root;
            if (root is null)
            {
                return this;
            }

            FaceDeindexator faceDeindexator = new FaceDeindexator();
            mesh.ForeachFace(root, ref faceDeindexator);

            FaceIndexator faceIndexator = new FaceIndexator(_faces, _edges, _nodes);
            mesh.ForeachFace(root, ref faceIndexator);

            return this;
        }

        struct FaceDeindexator() : IFaceProcessor
        {
            public bool ProcessAndContinue(Face face)
            {
                face.Index = -1;

                Edge e0 = face.Edge;
                Edge e = e0;
                do
                {
                    e.Index = -1;
                    e.NodeStart.Index = -1;
                    e = e.Next;
                } while (e0 != e);

                return true;
            }
        }

        struct FaceIndexator(List<Face> faces, List<Edge> edges, List<Node> nodes) : IFaceProcessor
        {
            public bool ProcessAndContinue(Face face)
            {
                if (!face.Invalid && face.Kind == FaceKind.Land)
                {
                    face.Index = faces.Count;
                    faces.Add(face);

                    Edge e0 = face.Edge;
                    Edge e = e0;
                    do
                    {
                        e.Index = edges.Count;
                        edges.Add(e);

                        var node = e.NodeStart;
                        if (node.Index == -1)
                        {
                            node.Index = nodes.Count;
                            nodes.Add(node);
                        }
                        e = e.Next;
                    } while (e0 != e);
                }
                return true;
            }
        }
    }
}
