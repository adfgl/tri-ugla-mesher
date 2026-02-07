using TriUgla.ExactMath;
using TriUgla.HalfEdge;

namespace TriUgla.ConvexHull
{
    public class ConvexGeometry
    {
        public Predicates Predicates { get; } = new Predicates();
        public Mesh Mesh { get; }

        public ConvexGeometry(IList<Vertex> vertices)
        {
            Mesh = BuildTetrahedron(vertices, out int[] indices);
            for (int i = 0; i < vertices.Count; i++)
            {
                if (indices.Contains(i)) continue;
                Add(vertices[i]);
            }
        }

        public Mesh BuildTetrahedron(IList<Vertex> vertices, out int[] tetraIndices)
        {
            (Vertex min, Vertex max, int[] indices) = Tetrahedron.GetInitialTetrahedron(vertices);

            Node a = new Node(vertices[indices[0]], NodeKind.Normal);
            Node b = new Node(vertices[indices[1]], NodeKind.Normal);
            Node c = new Node(vertices[indices[2]], NodeKind.Normal);

            Node d = new Node(vertices[indices[3]], NodeKind.Normal);

            int sign = Predicates.PlaneSide(
                a.Vertex.x, a.Vertex.y, a.Vertex.z,
                b.Vertex.x, b.Vertex.y, b.Vertex.z,
                c.Vertex.x, c.Vertex.y, c.Vertex.z,
                d.Vertex.x, d.Vertex.y, d.Vertex.z);
            if (sign == 0)
                throw new ArgumentException("Points are coplanar; cannot build a tetrahedron.");

            if (sign > 0)
            {
                (b, c) = (c, b);
            }

            Face fABC = new Face();
            Face fACD = new Face();
            Face fADB = new Face();
            Face fBDC = new Face();

            Edge ab = new Edge(); Edge bc = new Edge(); Edge ca = new Edge();
            Edge ac = new Edge(); Edge cd = new Edge(); Edge da = new Edge();
            Edge ad = new Edge(); Edge db = new Edge(); Edge ba = new Edge();
            Edge bd = new Edge(); Edge dc = new Edge(); Edge cb = new Edge();

            ElementLinker.Link(fABC, ab, bc, ca, a, b, c);
            ElementLinker.Link(fACD, ac, cd, da, a, c, d);
            ElementLinker.Link(fADB, ad, db, ba, a, d, b);
            ElementLinker.Link(fBDC, bd, dc, cb, b, d, c);

            ElementLinker.LinkEdgeTwins(ab, ba);
            ElementLinker.LinkEdgeTwins(bc, cb);
            ElementLinker.LinkEdgeTwins(ca, ac);

            ElementLinker.LinkEdgeTwins(cd, dc);
            ElementLinker.LinkEdgeTwins(da, ad);
            ElementLinker.LinkEdgeTwins(db, bd);

            tetraIndices = indices;
            return new Mesh(fABC);
        }


        public bool Add(Vertex vertex)
        {
            List<Face> cup = ExtractCup(vertex);
            if (cup.Count == 0) return false;

            List<Edge> horizon = ExtractHorizon(cup);
            BuildNewFaces(horizon, vertex);
            RemoveCup(cup);
            return true;
        }

        List<Face> ExtractCup(Vertex vertex)
        {
            CapCollector collector = new CapCollector(vertex, Predicates);
            Mesh.ForeachFace(Mesh.Root, ref collector);
            return collector.capFaces;
        }

        void RemoveCup(List<Face> faces)
        {
            foreach (var item in faces)
            {
                Mesh.Remove(item);
            }
        }

        static void BuildNewFaces(List<Edge> horizon, Vertex vertex)
        {
            Node c = new Node(vertex, NodeKind.Normal);
            Face first, prev;
            first = prev = MakeFace(horizon[0], c);
            for (int i = 1; i < horizon.Count; i++)
            {
                Face face = MakeFace(horizon[i], c);
                LinkTwins(face, prev);
                prev = face;
            }
            LinkTwins(first, prev);
        }

        static Face MakeFace(Edge ba, Node c)
        {
            Node a = ba.NodeEnd;
            Node b = ba.NodeStart;

            Edge ab = new Edge();
            Edge bc = new Edge();
            Edge ca = new Edge();

            Face face = new Face();

            ElementLinker.Link(face, ab, bc, ca, a, b, c);
            ElementLinker.LinkEdgeTwins(ba, ab);
            return face;
        }

        static void LinkTwins(Face curr, Face prev)
        {
            Edge ab = curr.Edge.Next;
            Edge ba = prev.Edge.Prev;
            ElementLinker.LinkEdgeTwins(ab, ba);
        }

        static List<Edge> ExtractHorizon(List<Face> faces)
        {
            List<Edge> horizon = new List<Edge>(64);
            Edge? start = HorizonStart(faces);
            if (start is null)
            {
                return horizon;
            }

            Edge curr = start;
            do
            {
                Edge e0 = curr.NodeStart.Edge;
                Edge e = e0;
                do
                {
                    Edge twin = e.Twin!;
                    if (!e.Invalid && twin.Invalid && horizon[^1] != e)
                    {
                        horizon.Add(e);
                        break;
                    }
                    e = twin.Next;
                } while (e0 != e);

            } while (curr != start);

            return horizon;
        }

        static Edge? HorizonStart(List<Face> faces)
        {
            foreach (Face face in faces)
            {
                Edge e0 = face.Edge;
                Edge e = e0;
                do
                {
                    Edge? twin = e.Twin;
                    if (twin is not null && !twin.Invalid)
                    {
                        return e;
                    }

                    e = e.Next;
                } while (e0 != e);
            }
            return null;
        }

        struct CapCollector(Vertex point, Predicates predicates) : IFaceProcessor
        {
            readonly double x = point.x, y = point.y, z = point.z;
            public readonly List<Face> capFaces = new List<Face>(64);

            public readonly bool ProcessAndContinue(Face face)
            {
                Vertex a = face.Edge.NodeStart.Vertex;
                Vertex b = face.Edge.Next.NodeStart.Vertex;
                Vertex c = face.Edge.Prev.NodeStart.Vertex;

                int sign = predicates.PlaneSide(
                    a.x, a.y, a.z,
                    b.x, b.y, b.z,
                    c.x, c.y, c.z,
                    x, y, z);

                if (sign == +1)
                {
                    capFaces.Add(face);
                    ElementInvalidator.Invalidate(face);
                }
                return true;
            }
        }
    }
}
