using System;
using System.Collections.Generic;
using System.Text;

namespace TriUgla.Mesher.Utils
{
    public static class GeometryHelper
    {
        public static bool Convex(Vec2 a, Vec2 b, Vec2 c, Vec2 d) =>
                Vec2.Cross(d, a, b) > 0 &&
                Vec2.Cross(a, b, c) > 0 &&
                Vec2.Cross(b, c, d) > 0 &&
                Vec2.Cross(c, d, a) > 0;
    }
}
