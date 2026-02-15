namespace TriUgla.HalfEdge
{
    public static class ElementInvalidator
    {
        public static void Invalidate(Node node)
        {
            node.Invalid = true;
        }

        public static void Invalidate(Edge edge)
        {
            edge.Invalid = true;
        }

        public static void Invalidate(Face face)
        {
            if (face.Invalid) return;
            InvalidateEdges(face);

            Edge e0 = face.Edge;
            Edge e = e0;
            do
            {
                Node n = e.NodeStart;
                if (!TryReconnect(n))
                {
                    Invalidate(n);
                }
                e = e.Next;
            } while (e0 != e);

            face.Invalid = true;
        }

        static void InvalidateEdges(Face face)
        {
            Edge e0 = face.Edge;
            Edge e = e0;
            do
            {
                Invalidate(e);
                e = e.Next;
            } while (e0 != e);
        }

        static bool TryReconnect(Node node)
        {
            Edge e0 = node.Edge;
            Edge e = e0;
            do
            {
                if (!e.Invalid)
                {
                    node.Edge = e;
                    break;
                }
                e = e.Prev.Twin!;
            } while (e0 != e);

            return node.Edge.Invalid;
        }
    }
}
