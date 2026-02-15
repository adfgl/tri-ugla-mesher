using System;
using System.Collections.Generic;
using System.Text;

namespace TriUgla.Mesher.Services
{
    public enum HitKind : byte
    {
        None,
        Face,
        Edge,
        Node
    }
}
