using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public class MeshMetrics
    {
        public MeshMetric<Edge> Angle { get; set; } = new MeshMetric<Edge>();
        public MeshMetric<Edge> EdgeLength { get; set; } = new MeshMetric<Edge>();
        public MeshMetric<Face> FaceArea { get; set; } = new MeshMetric<Face>();

        public override string ToString()
        {
            return
                $"""
            Mesh metrics
            ------------
            Angle       : {Angle.ToString("°")}
            Edge length : {EdgeLength.ToString()}
            Face area   : {FaceArea.ToString()}
            """;
        }
    }
}
