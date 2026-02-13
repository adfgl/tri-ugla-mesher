using System.Runtime.CompilerServices;
using TriUgla.ExactMath;
using TriUgla.Geometry;

namespace TriUgla.Mesher.Services
{
    public sealed class EarClipper(Predicates predicates)
    {
        readonly Predicates _predicates = predicates;

        public List<int> Triangulate(List<Vec2> poly)
        {
            int n = poly.Count;
            List<int> tris = new List<int>(Math.Max(0, (n - 2) * 3));
            if (n < 3) return tris;

            List<int>? loop = new List<int>(n);
            for (int i = 0; i < n; i++) loop.Add(i);

            int budget = n * n;
            int iCursor = 0;
            while (loop.Count > 3 && budget-- > 0)
            {
                int m = loop.Count;

                int i0 = loop[(iCursor + m - 1) % m];
                int i1 = loop[iCursor % m];
                int i2 = loop[(iCursor + 1) % m];

                if (IsEar(poly, loop, i0, i1, i2))
                {
                    tris.Add(i0);
                    tris.Add(i1);
                    tris.Add(i2);

                    loop.RemoveAt(iCursor % m);
                    iCursor = Math.Max(iCursor - 1, 0);
                    continue;
                }

                iCursor++;
                if (iCursor >= loop.Count) iCursor = 0;
            }

            if (loop.Count == 3)
            {
                tris.Add(loop[0]);
                tris.Add(loop[1]);
                tris.Add(loop[2]);
            }

            return tris;
        }

        bool IsEar(List<Vec2> poly, List<int> loop, int i0, int i1, int i2)
        {
            Vec2 a = poly[i0];
            Vec2 b = poly[i1];
            Vec2 c = poly[i2];

            for (int k = 0; k < loop.Count; k++)
            {
                int pid = loop[k];
                if (pid == i0 || pid == i1 || pid == i2) continue;

                Vec2 p = poly[pid];
                if (PointInTriOrOnEdge(in p, in a, in b, in c))
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool PointInTriOrOnEdge(in Vec2 p, in Vec2 a, in Vec2 b, in Vec2 c)
        {
            return
                _predicates.Orient(a.x, a.y, b.x, b.y, p.x, p.y) >= 0 &&
                _predicates.Orient(b.x, b.y, c.x, c.y, p.x, p.y) >= 0 &&
                _predicates.Orient(c.x, c.y, a.x, a.y, p.x, p.y) >= 0;
        }
    }
}
