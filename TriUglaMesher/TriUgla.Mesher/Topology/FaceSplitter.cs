using TriUgla.HalfEdge;

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

            MakeTwinEdges(out Edge bd, out Edge db);
            MakeTwinEdges(out Edge ad, out Edge da);
            MakeTwinEdges(out Edge cd, out Edge dc);

            Face abd = target;
            Face bcd = new Face();
            Face cad = new Face();

            ElementLinker.Link(abd, ab, bd, da, a, b, d);
            ElementLinker.Link(bcd, bc, cd, db, b, c, d);
            ElementLinker.Link(cad, ca, ad, dc, c, a, d);

            abd.TransmitContextTo(bcd);
            abd.TransmitContextTo(cad);

            _illigalEdges.Push(ab);
            _illigalEdges.Push(bc);
            _illigalEdges.Push(ca);
            return this;
        }
    }
}
