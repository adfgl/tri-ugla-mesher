using System.Runtime.CompilerServices;

namespace TriUgla.HalfEdge
{
    public sealed class VisitStamp
    {
        int _next = 1;

        public bool Overflowed { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next()
        {
            int s = _next++;
            if (s <= 0)
            {
                Overflowed = true;
                return 1;
            }

            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _next = 1;
            Overflowed = false;
        }
    }
}
