using TriUgla.ExactMath;
using TriUgla.Geometry;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Topology
{
    public sealed class EdgeFlipper(Predicates predicates, Stack<Edge> illigalEdges)
    {
        readonly Predicates _predicates = predicates;
        readonly Stack<Edge> _illigalEdges = illigalEdges;

        public EdgeFlipper Flip(Edge edge)
        {
            /* 
                   c                   c
                  /\                  /|\
                 /  \                / | \
                /    \              /  |  \
               /  t0  \            /   |   \
              /   →    \          /    |    \
           a +----------+ b    a +  t0 | t1  + b
              \   ←    /          \    |    /
               \  t1  /            \   |   /
                \    /              \  |  /
                 \  /                \ | /
                  \/                  \|/
                  d                    d
        */
            Edge ab = edge;
            Edge bc = ab.Next;
            Edge ca = bc.Next;

            Edge ba = ab.Twin!;
            Edge ad = ba.Next;
            Edge db = ad.Next;

            Node a = ab.NodeStart;
            Node b = bc.NodeStart;
            Node c = ca.NodeStart;
            Node d = db.NodeStart;

            Edge cd = ab;
            Edge dc = ba;

            Face adc = cd.Face;
            Face dbc = dc.Face;

            ElementLinker.Link(adc, ad, dc, ca, a, d, c);
            ElementLinker.Link(dbc, db, bc, cd, d, b, c);

            _illigalEdges.Push(ad);
            _illigalEdges.Push(db);
            return this;
        }

        public bool CanFlip(Edge edge, out bool should)
        {
            should = false;
            if (edge.Twin is null || edge.Constrained || edge.Twin.Constrained)
            {
                return false;
            }

            /* 
                   d           
                  /\                
                 /  \             
                /    \          
               /  t0  \        
              /   →    \        
           a +----------+ c   
              \   ←    /          
               \  t1  /           
                \    /              
                 \  /             
                  \/               
                  b              
            */

            Vec4 a = edge.NodeStart.Vertex;
            Vec4 b = edge.Twin.Opposite.Vertex;
            Vec4 c = edge.NodeEnd.Vertex;
            Vec4 d = edge.Opposite.Vertex;

            if (_predicates.Convex(a.x, a.y, b.x, b.y, c.x, c.y, d.x, d.y))
            {
                should = 1 == _predicates.InCircle(a.x, a.y, c.x, c.y, d.x, d.y, b.x, b.y);
                return true;
            }
            return false;
        }
    }
}
