using TriUgla.Mesher.HalfEdge;

namespace TriUgla.Mesher.Topology
{
    public sealed class EdgeSplitter(Stack<Edge> illigalEdges)
        : Splitter<Edge>(illigalEdges)
    {
        public Edge FirstHalf { get; private set; } = null!;
        public Edge SecondHalf { get; private set; } = null!;

        public override Splitter<Edge> Split(Edge target, Node node)
        {
            /* 
            *        c                   c
            *        /\                  /|\
            *       /  \                / | \
            *      /    \              /  |  \
            *     /  t0  \            /t0 | t3\
            *    /   →    \          /    |    \
            * a +----------+ b    a +-----e ----+ b
            *    \   ←    /          \    |    /
            *     \  t1  /            \t1 | t2/
            *      \    /              \  |  /
            *       \  /                \ | /
            *        \/                  \|/
            *        d                    d 
            */

            Edge ab = target;
            Edge bc = ab.Next;
            Edge ca = ab.Prev;

            Edge ba = ab.Twin!;
            Edge ad = ba.Next;
            Edge db = ba.Prev;

            Node a = ab.NodeStart;
            Node b = bc.NodeStart;
            Node c = ca.NodeStart;
            Node d = db.NodeStart;
            Node e = node;

            Edge ae = ab;
            Edge ea = ba;

            MakeTwinEdges(e, b, out Edge eb, out Edge be);
            MakeTwinEdges(e, c, out Edge ec, out Edge ce);
            MakeTwinEdges(e, d, out Edge ed, out Edge de);

            e.Edge = eb;

            Face cae = ca.Face;
            ElementLinker.LinkEdges(ca, ae, ec);
            ElementLinker.LinkEdgeFace(ca, ae, ec, cae);

            Face ade = ea.Face;
            ElementLinker.LinkEdges(ad, de, ea);
            ElementLinker.LinkEdgeFace(ad, de, ea, ade);

            Face dbe = new Face { Edge = db };
            ElementLinker.LinkEdges(db, be, ed);
            ElementLinker.LinkEdgeFace(db, be, ed, dbe);

            Face bce = new Face { Edge = bc };
            ElementLinker.LinkEdges(bc, ce, eb);
            ElementLinker.LinkEdgeFace(bc, ce, eb, bce);

            cae.TransmitContextTo(bce);
            ade.TransmitContextTo(dbe);
            ae.TransmitContextTo(eb);
            be.TransmitContextTo(ea);

            _illigalEdges.Push(ad);
            _illigalEdges.Push(db);
            _illigalEdges.Push(bc);
            _illigalEdges.Push(ca);
            return this;
        }
    }
}
