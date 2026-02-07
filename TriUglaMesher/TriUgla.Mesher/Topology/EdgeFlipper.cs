using TriUgla.Mesher.HalfEdge;

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

            ElementLinker.LinkEdges(cd, db, bc);
            ElementLinker.LinkEdges(dc, ca, ad);

            ad.Face = adc;
            bc.Face = dbc;

            ElementLinker.LinkNodeEdge(a, ab);
            ElementLinker.LinkNodeEdge(b, bc);
            ElementLinker.LinkNodeEdge(c, cd);
            ElementLinker.LinkNodeEdge(d, dc);

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

            Vertex a = edge.NodeStart.Vertex;
            Vertex b = edge.Twin.Opposite.Vertex;
            Vertex c = edge.NodeEnd.Vertex;
            Vertex d = edge.Opposite.Vertex;

            //if (_predicates.Convex(a, b, c, d))
            {
                should = 1 == _predicates.InCircle(a, c, d, b);
                return true;
            }
            return false;
        }
    }
}
