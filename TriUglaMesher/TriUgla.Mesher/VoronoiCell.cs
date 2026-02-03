using System;
using System.Collections.Generic;
using System.Text;
using TriUgla.Mesher.MeshCore;

namespace TriUgla.Mesher
{
    public sealed class VoronoiCell
    {
        public List<Edge> Edges { get; set; } = new List<Edge>();
    }
}
