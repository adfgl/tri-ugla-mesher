using System;

namespace TriUgla.Mesher.MeshCore
{
    public struct EdgeContext
    {
        public int payload;
        public int features;
        public int contours;
        public int cavities;

        public readonly bool Constrained => features + contours + cavities > 0;
        public readonly bool BlocksFlood => (cavities + contours) > 0;
    }
}
