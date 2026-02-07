using System;
using System.Collections.Generic;
using System.Text;
using TriUgla.HalfEdge;

namespace TriUgla.ConvexHull
{
    public static class Tetrahedron
    {
        public const int NO_INDEX = -1;
       
        public static (Vertex min, Vertex max, int[] indices) GetInitialTetrahedron(IList<Vertex> pts)
        {
            (Vertex min, Vertex max) = GetFirstTwoPoints(pts, out int p1, out int p2);
            GetThird(pts, p1, p2, out int p3);
            GetForth(pts, p1, p2, p3, out int p4);

            if (p1 == p2 || p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4 || p3 == p4)
            {
                throw new InvalidOperationException("Degenerate initial tetrahedron (duplicate indices).");
            }
            return (min, max, [p1, p2, p3, p4])
        }

        public static (Vertex min, Vertex max) GetFirstTwoPoints(IList<Vertex> points, out int p1, out int p2)
        {
            double minX = double.PositiveInfinity, minY = double.PositiveInfinity, minZ = double.PositiveInfinity;
            double maxX = double.NegativeInfinity, maxY = double.NegativeInfinity, maxZ = double.NegativeInfinity;

            int minIx = 0, minIy = 0, minIz = 0;
            int maxIx = 0, maxIy = 0, maxIz = 0;

            for (int i = 0; i < points.Count; i++)
            {
                var v = points[i];

                if (v.x < minX) { minX = v.x; minIx = i; }
                if (v.y < minY) { minY = v.y; minIy = i; }
                if (v.z < minZ) { minZ = v.z; minIz = i; }

                if (v.x > maxX) { maxX = v.x; maxIx = i; }
                if (v.y > maxY) { maxY = v.y; maxIy = i; }
                if (v.z > maxZ) { maxZ = v.z; maxIz = i; }
            }

            // Pick widest axis
            double dx = maxX - minX;
            double dy = maxY - minY;
            double dz = maxZ - minZ;

            int axis = 0;
            double best = dx;
            if (dy > best) { best = dy; axis = 1; }
            if (dz > best) { best = dz; axis = 2; }

            p1 = axis == 0 ? minIx : axis == 1 ? minIy : minIz;
            p2 = axis == 0 ? maxIx : axis == 1 ? maxIy : maxIz;

            if (p1 == p2)
                throw new InvalidOperationException("LOGIC ERROR: Points are the same (all points identical or axis extent 0).");

            return (new Vertex(minX, minY, minZ), new Vertex(maxX, maxY, maxZ));
        }

        public static void GetThird(IList<Vertex> points, int p1, int p2, out int p3)
        {
            if (p1 == NO_INDEX || p2 == NO_INDEX) throw new ArgumentException();
            if (p1 == p2) throw new ArgumentException("p1 == p2");

            p3 = NO_INDEX;

            var a = points[p1];
            var b = points[p2];

            // ba = b - a
            double bax = b.x - a.x;
            double bay = b.y - a.y;
            double baz = b.z - a.z;

            double em = bax * bax + bay * bay + baz * baz; // |ba|^2
            if (em == 0.0) throw new InvalidOperationException("Degenerate: p1 and p2 are identical.");

            double maxDist = double.NegativeInfinity;

            for (int i = 0; i < points.Count; i++)
            {
                if (i == p1 || i == p2) continue;

                var p = points[i];

                // pa = p - a
                double pax = p.x - a.x;
                double pay = p.y - a.y;
                double paz = p.z - a.z;

                // |pa x ba|^2 / |ba|^2  (squared distance to line through a->b)
                double cx = pay * baz - paz * bay;
                double cy = paz * bax - pax * baz;
                double cz = pax * bay - pay * bax;

                double dist = (cx * cx + cy * cy + cz * cz) / em;

                if (dist > maxDist)
                {
                    maxDist = dist;
                    p3 = i;
                }
            }

            if (p3 == NO_INDEX)
                throw new InvalidOperationException("LOGIC ERROR: third point not found.");
        }

        public static void GetForth(IList<Vertex> points, int p1, int p2, int p3, out int p4)
        {
            p4 = NO_INDEX;

            var a = points[p1];
            var b = points[p2];
            var c = points[p3];

            // Precompute plane normal n = (b-a) x (c-a)
            double bax = b.x - a.x, bay = b.y - a.y, baz = b.z - a.z;
            double cax = c.x - a.x, cay = c.y - a.y, caz = c.z - a.z;

            double nx = bay * caz - baz * cay;
            double ny = baz * cax - bax * caz;
            double nz = bax * cay - bay * cax;

            double nn = nx * nx + ny * ny + nz * nz;
            if (nn == 0.0) throw new InvalidOperationException("Degenerate: points p1,p2,p3 are collinear.");

            double maxAbs = double.NegativeInfinity;

            for (int i = 0; i < points.Count; i++)
            {
                if (i == p1 || i == p2 || i == p3) continue;

                var p = points[i];

                // signed side = n · (p-a)
                double pax = p.x - a.x;
                double pay = p.y - a.y;
                double paz = p.z - a.z;

                double s = nx * pax + ny * pay + nz * paz;
                double abs = s >= 0 ? s : -s;

                if (abs > maxAbs)
                {
                    maxAbs = abs;
                    p4 = i;
                }
            }

            if (p4 == NO_INDEX)
                throw new InvalidOperationException("LOGIC ERROR: could not build initial tetrahedron.");
        }
    }
}
