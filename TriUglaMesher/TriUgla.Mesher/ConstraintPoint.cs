using TriUgla.HalfEdge;

namespace TriUgla.Mesher
{
    public class ConstraintPoint(Node node)
    {
        public Node Node { get; set; } = node;  
    }
}
