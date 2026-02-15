using TriUgla.HalfEdge;
using TriUgla.Mesher.Services;

namespace TriUgla.Mesher.Topology
{
    public sealed class EdgeSplitter(IlligalEdges illigalEdges)
        : Splitter<Edge>(illigalEdges)
    {
        public Edge FirstHalf { get; private set; } = null!;
        public Edge SecondHalf { get; private set; } = null!;

        protected override Splitter<Edge> SplitInternal(Edge target, Node node)
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

            MakeTwinEdges(out Edge eb, out Edge be);
            MakeTwinEdges(out Edge ec, out Edge ce);
            MakeTwinEdges(out Edge ed, out Edge de);

            Face cae = ca.Face;
            Face ade = ea.Face;
            Face dbe = new Face();
            Face bce = new Face();

            ElementLinker.Link(cae, ca, ae, ec, c, a, e);
            ElementLinker.Link(ade, ad, de, ea, a, d, e);
            ElementLinker.Link(dbe, db, be, ed, d, b, e);
            ElementLinker.Link(bce, bc, ce, eb, b, c, e);

            cae.TransmitContextTo(bce);
            ade.TransmitContextTo(dbe);
            ae.TransmitContextTo(eb);
            be.TransmitContextTo(ea);

            _illigalEdges.Push(ad);
            _illigalEdges.Push(db);
            _illigalEdges.Push(bc);
            _illigalEdges.Push(ca);

            FirstHalf = ae;
            SecondHalf = eb;
            return this;
        }
    }
}
