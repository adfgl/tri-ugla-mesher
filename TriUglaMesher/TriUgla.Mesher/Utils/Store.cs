using System.Runtime.CompilerServices;

namespace TriUgla.Mesher.Utils
{
    public sealed class Store<T>(int capacity = 16) where T : struct
    {
        int _head = -1; 
        Slot[] _slots = new Slot[Math.Max(capacity, 0)];
        int _count;
        int _used;
        int _grewTimes;

        public int Count => _count;
        public int CapacityUsed => _used;
        public int GrewTimes => _grewTimes;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Id Add(in T item)
        {
            int index;

            if (_head >= 0)
            {
                index = _head;
                ref Slot s = ref _slots[index];

                _head = s.NextFree;    

                s.value = item;
                s.meta = 0;
            }
            else
            {
                if (_used == _slots.Length)
                    Grow();

                index = _used++;
                ref Slot s = ref _slots[index];

                s.value = item;
                s.meta = 0;        
            }

            _count++;
            return new Id(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(Id h)
        {
            int index = h.Index;
            if ((uint)index >= (uint)_used)
                return false;

            ref Slot s = ref _slots[index];

            if (!s.IsAlive)
                return false;

            s.meta = _head + 1;
            _head = index;

            _count--;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive(Id h)
        {
            int index = h.Index;
            if ((uint)index >= (uint)_used)
                return false;

            return _slots[index].IsAlive;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(Id h)
        {
            int index = h.Index;
            if ((uint)index >= (uint)_used)
                throw new InvalidOperationException("Invalid handle index.");

            ref Slot s = ref _slots[index];
            if (!s.IsAlive)
                throw new InvalidOperationException("Dead handle.");

            return ref s.value;
        }

        void Grow()
        {
            const int MaxCapacity = int.MaxValue; 

            int oldLen = _slots.Length;
            int newLen;

            if (oldLen == 0)
            {
                newLen = 4;
            }
            else
            {
                long candidate = oldLen + (oldLen >> 1);

                if (candidate <= oldLen || candidate > MaxCapacity)
                    newLen = MaxCapacity;
                else
                    newLen = (int)candidate;
            }

            if (newLen == oldLen)
                throw new InvalidOperationException("StableList capacity limit reached.");

            Array.Resize(ref _slots, newLen);
            _grewTimes++;
        }

        struct Slot
        {
            public T value;
            public int meta;

            public readonly bool IsAlive => meta == 0;
            public readonly int NextFree => meta - 1;
        }
    }
}
