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
        public Id Rent(in T item)
        {
            int index;
            ref Slot s = ref Unsafe.NullRef<Slot>();

            if (_head >= 0)
            {
                index = _head;
                s = ref _slots[index];
                _head = s.nextFree;
            }
            else
            {
                if (_used == _slots.Length)
                    Grow();

                index = _used++;

#if DEBUG
                if ((uint)index > Id.MaxIndex)
                    throw new InvalidOperationException($"Store exceeded Id.MaxIndex ({Id.MaxIndex}). index={index}");
#endif
                s = ref _slots[index];

                if (s.generation == 0) s.generation = 1;
            }

            s.value = item;
            s.nextFree = -1; 

            _count++;
            return new Id(index, s.generation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(Id id)
        {
            int index = id.Index;
            if ((uint)index >= (uint)_used)
                return false;

            ref Slot s = ref _slots[index];

            if (s.nextFree >= 0 || s.generation != id.Generation)
                return false;

            s.generation = NextGeneration(s.generation);
            s.nextFree = _head;
            _head = index;

            _count--;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive(Id id)
        {
            int index = id.Index;
            if ((uint)index >= (uint)_used)
                return false;

            ref readonly Slot s = ref _slots[index];
            return s.nextFree < 0 && s.generation == id.Generation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(Id id, out T value)
        {
            int index = id.Index;
            if ((uint)index >= (uint)_used)
            {
                value = default;
                return false;
            }

            ref readonly Slot s = ref _slots[index];
            if (s.nextFree >= 0 || s.generation != id.Generation)
            {
                value = default;
                return false;
            }

            value = s.value;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Ref(Id id)
        {
            int index = id.Index;

#if DEBUG
            if ((uint)index >= (uint)_used)
                throw new InvalidOperationException($"Invalid handle index: {id} (used={_used}).");
#endif

            ref Slot s = ref _slots[index];

#if DEBUG
            if (s.nextFree >= 0)
                throw new InvalidOperationException($"Dead handle: {id}.");
            if (s.generation != id.Generation)
                throw new InvalidOperationException($"Stale handle: {id}, slot gen={s.generation}.");
#endif
            return ref s.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint NextGeneration(uint gen)
        {
            gen++;
            if (gen == 0 || gen > Id.MaxGeneration) gen = 1;
            return gen;
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
                long candidate = oldLen + (oldLen >> 1); // 1.5x
                if (candidate <= oldLen || candidate > MaxCapacity)
                    newLen = MaxCapacity;
                else
                    newLen = (int)candidate;
            }

            if (newLen == oldLen)
                throw new InvalidOperationException("Store capacity limit reached.");

            Array.Resize(ref _slots, newLen);
            _grewTimes++;
        }

        struct Slot
        {
            public T value;
            public int nextFree;
            public uint generation;
        }
    }
}
