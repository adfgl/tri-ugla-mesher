using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Quality
{
    public interface IFaceStatsCollector
    {
        bool TryCollect(Face face, out FaceStats stats);
    }
}
