using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TriUgla.Mesher.Utils
{
    public sealed class BufferStack<T> where T : struct
    {
        T[] _items;
        int _count;

        public BufferStack(int capacity = 64)
        {
            if (capacity < 1) capacity = 1;
            _items = new T[capacity];
        }

        public T this[int i] => _items[i];

        public int Count => _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => _count = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item)
        {
            int count = _count;
            EnsureCapacity(count + 1);

            _items[count] = item;
            _count = count + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push2(T a, T b)
        {
            int count = _count;
            int required = count + 2;
            EnsureCapacity(required);

            _items[count] = a;
            _items[count + 1] = b;
            _count = required;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push3(T a, T b, T c)
        {
            int count = _count;
            int required = count + 3;
            EnsureCapacity(required);

            _items[count] = a;
            _items[count + 1] = b;
            _items[count + 2] = c;
            _count = required;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push4(T a, T b, T c, T d)
        {
            int count = _count;
            int required = count + 4;
            EnsureCapacity(required);

            _items[count] = a;
            _items[count + 1] = b;
            _items[count + 2] = c;
            _items[count + 3] = d;
            _count = required;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
#if DEBUG
            if (_count == 0)
                throw new InvalidOperationException("Empty stack.");
#endif
            return _items[--_count];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EnsureCapacity(int required)
        {
            if (required > _items.Length)
                Grow(required);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Grow(int required)
        {
            int newSize = _items.Length * 2;
            if (newSize < required)
                newSize = required;

            Array.Resize(ref _items, newSize);
        }
    }
}
