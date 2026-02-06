using System.Runtime.CompilerServices;
using TriUgla.Mesher.HalfEdge;
using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.Topology
{
    public static class ElementLinker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LinkNodeEdge(Node node, Edge edge)
        {
            edge.NodeStart = node;  
            node.Edge = edge;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnlinkNodeEdge(Edge edge)
        {
            Node? node = edge.NodeStart;
            if (node != null && node.Edge == edge)
                node.Edge = null!;

            edge.NodeStart = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnlinkNodeEdge(Node node)
        {
            Edge? edge = node.Edge;
            if (edge != null && edge.NodeStart == node)
                edge.NodeStart = null!;

            node.Edge = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LinkEdgeFace(Edge edge, Face face)
        {
            edge.Face = face;
            face.Edge = edge;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LinkEdgeFace(Edge a, Edge b, Edge c, Face face)
        {
            a.Face = b.Face = c.Face = face;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnlinkEdgeFace(Edge edge)
        {
            Face? face = edge.Face;
            if (face != null && face.Edge == edge)
                face.Edge = null!;

            edge.Face = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnlinkEdgeFace(Face face)
        {
            Edge? edge = face.Edge;
            if (edge != null && edge.Face == face)
                edge.Face = null!;

            face.Edge = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnlinkEdgeFace(Edge edge, Face face)
        {
            if (edge.Face == face)
                edge.Face = null!;

            if (face.Edge == edge)
                face.Edge = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LinkEdges(Edge a, Edge b)
        {
            a.Next = b;
            b.Prev = a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LinkEdges(Edge a, Edge b, Edge c)
        {
            LinkEdges(a, b);
            LinkEdges(b, c);
            LinkEdges(c, a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnlinkEdges(Edge e)
        {
            Edge? p = e.Prev;
            Edge? n = e.Next;

            if (p != null && p.Next == e) p.Next = null!;
            if (n != null && n.Prev == e) n.Prev = null!;

            e.Prev = null!;
            e.Next = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LinkEdgeTwins(Edge a, Edge b)
        {
            a.Twin = b;
            b.Twin = a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnlinkEdgeTwins(Edge e)
        {
            Edge? t = e.Twin;
            if (t == null) return;
            e.Twin = null!;
            if (t.Twin == e) t.Twin = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreTwins(Edge a, Edge b)
        {
            return a.NodeStart == b.NodeEnd && a.NodeEnd == b.NodeStart;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryLinkEdgeTwins(Edge a, Edge b)
        {
            if (AreTwins(a, b))
            {
                LinkEdgeTwins(a, b);
                return true;
            }
            return false;
        }

        public static void LinkEdgeTwins(List<Edge> list)
        {
            int n = list.Count;

            for (int i = 0; i < n; i++)
            {
                Edge ei = list[i];
                if (ei.Twin is not null) continue;

                for (int j = i + 1; j < n; j++)
                {
                    Edge ej = list[j];
                    if (ej.Twin is not null) continue;

                    if (TryLinkEdgeTwins(ei, ej))
                        break;
                }
            }
        }
    }
}
