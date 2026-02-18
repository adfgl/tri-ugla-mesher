using TriUgla.Geometry;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public sealed class DefaultFaceStatsCollector : IFaceStatsCollector
    {
        public bool TryCollect(Face face, out FaceStats stats)
        {
            Edge e0 = face.Edge;
            if (e0 is null)
            {
                stats = default;
                return false;
            }

            Edge e1 = e0.Next;
            Edge e2 = e1.Next;

            double len0 = e0.Length2;
            double len1 = e1.Length2;
            double len2 = e2.Length2;

            Vec4 avg = (e0.NodeStart.Vertex + e1.NodeStart.Vertex + e2.NodeStart.Vertex) / 3.0;

            stats = new FaceStats(
                face.AreaSigned, 
                Math.Min(Math.Min(len0, len1), len2), 
                Math.Max(Math.Max(len0, len1), len2),
                avg.z,
                avg.x,
                avg.y);
            return true;
        }
    }
}
