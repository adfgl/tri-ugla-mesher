using TriUgla.HalfEdge;

namespace TriUgla.Mesher
{
    public class ConstraintPoint(Node node)
    {
        public string? Name { get; set; }   
        public Node Node { get; set; } = node;  
    }
}
