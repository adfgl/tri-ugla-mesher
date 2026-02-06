using TriUgla.Mesher.HalfEdge;

namespace TriUgla.Mesher.Topology
{
    public sealed class FaceSplitter(Stack<Edge> illigalEdges) 
        : Splitter<Face>(illigalEdges)
    {
        public override Splitter<Face> Split(Face target, Node node)
        {
            Edge ab = target.Edge;
            Edge bc = ab.Next;
            Edge ca = ab.Prev;

            Node a = ab.NodeStart;
            Node b = bc.NodeStart;
            Node c = ca.NodeStart;
            Node d = node;

            MakeTwinEdges(b, d, out Edge bd, out Edge db);
            MakeTwinEdges(a, d, out Edge ad, out Edge da);
            MakeTwinEdges(c, d, out Edge cd, out Edge dc);
            d.Edge = db;

            Face abd = target;
            ElementLinker.LinkEdges(ab, bd, da);
            ElementLinker.LinkEdgeFace(ab, bd, da, abd);

            Face bcd = new Face { Edge = bc };
            ElementLinker.LinkEdges(bc, cd, db);
            ElementLinker.LinkEdgeFace(bc, cd, db, bcd);

            Face cad = new Face { Edge = ca };
            ElementLinker.LinkEdges(ca, ad, dc);
            ElementLinker.LinkEdgeFace(ca, ad, dc, cad);

            abd.TransmitContextTo(bcd);
            abd.TransmitContextTo(cad);

            _illigalEdges.Push(ab);
            _illigalEdges.Push(bc);
            _illigalEdges.Push(ca);
            return this;
        }

    }
}
