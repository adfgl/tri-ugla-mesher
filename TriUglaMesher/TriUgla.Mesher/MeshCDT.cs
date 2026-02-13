using TriUgla.Geometry;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher
{
    public class MeshCDT
    {
        public MeshCDT()
        {
            
        }

        public Mesh Mesh { get; set; }
        public List<Constraint> Constraints { get; set; } = new List<Constraint>();
        public List<Loop> Loops { get; set; } = new List<Loop>();
    }
}
