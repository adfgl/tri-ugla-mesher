using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TriUgla.Mesher.Utils
{
    public readonly struct Id : IEquatable<Id>
    {
        // [ generation : GenBits | index : IndexBits ]  => total 32 bits
        readonly uint _value;

        public const int IndexBits = 24;
        public const int GenerationBits = 32 - IndexBits;

        public const uint MaxIndex = (1u << IndexBits) - 1u;       
        public const uint MaxGeneration = (1u << GenerationBits) - 1u; 

        const uint IndexMask = MaxIndex;

        public static Id Null => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Id(int index, uint generation)
        {
#if DEBUG
            if ((uint)index > MaxIndex)
                throw new ArgumentOutOfRangeException(nameof(index), $"index must be <= {MaxIndex}");
            if (generation > MaxGeneration)
                throw new ArgumentOutOfRangeException(nameof(generation), $"generation must be <= {MaxGeneration}");
#endif
            _value = (generation << IndexBits) | (uint)index;
        }

        public int Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (int)(_value & IndexMask);
        }

        public uint Generation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value >> IndexBits;
        }

        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value == 0;
        }

        [Conditional("DEBUG")]
        public void DebugAssertValid()
        {
            uint idx = _value & IndexMask;
            uint gen = _value >> IndexBits;

            if (idx > MaxIndex) throw new InvalidOperationException("Packed Id has invalid index bits.");
            if (gen > MaxGeneration) throw new InvalidOperationException("Packed Id has invalid generation bits.");
        }

        public bool Equals(Id other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Id h && Equals(h);
        public override int GetHashCode() => (int)_value;

        public static bool operator ==(Id a, Id b) => a._value == b._value;
        public static bool operator !=(Id a, Id b) => a._value != b._value;

        public override string ToString()
            => IsNull ? "null" : $"{Index} (gen {Generation})";
    }
}
