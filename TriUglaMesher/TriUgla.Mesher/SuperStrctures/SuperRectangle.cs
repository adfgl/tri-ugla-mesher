using System;
using System.Collections.Generic;
using System.Text;
using TriUgla.Geometry;

namespace TriUgla.Mesher.SuperStrctures
{
    public sealed class SuperRectangleStructure(double expand = 3.0) : SuperGeometryBase(nodeCapacity: 4, faceCapacity: 2)
    {
        readonly double _expand = expand;

        protected override List<Vec4> BuildRing(in Rect b)
        {
            double dx = b.maxX - b.minX;
            double dy = b.maxY - b.minY;
            double pad = Math.Max(dx, dy) * _expand;

            double minX = b.minX - pad;
            double minY = b.minY - pad;
            double maxX = b.maxX + pad;
            double maxY = b.maxY + pad;

            return new List<Vec4>(4)
            {
                new Vec4(minX, minY, 0),
                new Vec4(maxX, minY, 0),
                new Vec4(maxX, maxY, 0),
                new Vec4(minX, maxY, 0),
            };
        }
    }
}
