using System;
using System.Collections.Generic;
using System.Text;

namespace TriUgla.Mesher.Utils
{
    public readonly struct Id : IEquatable<Id>
    {
        readonly int _index;
        public static Id Null => new Id(-1);

        public Id(int index) => _index = index;

        public int Index => _index;
        public bool IsNull => _index < 0;

        public bool Equals(Id other) => _index == other._index;
        public override bool Equals(object? obj) => obj is Id h && Equals(h);
        public override int GetHashCode() => _index;
        public static bool operator ==(Id a, Id b) => a._index == b._index;
        public static bool operator !=(Id a, Id b) => a._index != b._index;

        public override string ToString() => IsNull ? "null" : _index.ToString();
    }
}
