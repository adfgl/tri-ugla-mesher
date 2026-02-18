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

            Edge e = e0;
            int n = 0;

            double area2 = 0.0;
            double minLen2 = double.MaxValue;
            double maxLen2 = 0.0;

            double sumVertexArea = 0.0;
            double cx = 0.0;
            double cy = 0.0;
            do
            {
                n++;

                Vec4 a = e.NodeStart.Vertex;
                cx += a.x;
                cy += a.y;

                Vec4 b = e.NodeEnd.Vertex;

                double len2 = e.Length2;
                if (len2 < minLen2) minLen2 = len2;
                if (len2 > maxLen2) maxLen2 = len2;

                area2 += a.x * b.y - a.y * b.x;
                sumVertexArea += a.z;
                e = e.Next;
            }
            while (e != e0);

            if (n < 3)
            {
                stats = default;
                return false;
            }

            double signedArea = area2 * 0.5;
            double avgVA = sumVertexArea / n;
            cx /= n;
            cy /= n;

            stats = new FaceStats(signedArea, minLen2, maxLen2, avgVA, cx, cy);
            return true;
        }
    }
}
