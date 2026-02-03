namespace TriUgla.Mesher.MeshCore
{
    public sealed class Edge : MeshElement
    {
        int _constraintCount = 0;
        int _featureCount = 0;
        int _contourCount = 0;
        int _holeCount = 0;

        public Node NodeStart { get; set; } = null!;
        public Node NodeEnd => Next.NodeStart;
        public Edge Next { get; internal set; } = null!;
        public Edge Prev { get; internal set; } = null!;
        public Edge? Twin { get; internal set; } = null!;
        public Face Face { get; internal set; } = null!;
        public Node Opposite => Prev.NodeStart;

        public bool Constrained => _constraintCount > 0;
        public bool BlocksFlood => (_contourCount + _holeCount) > 0;
        public bool IsWall => Twin is not null && (BlocksFlood || Twin.BlocksFlood); 
        public double Length => Math.Sqrt()

        public void AddConstraint(EdgeKind kind)
        {
            _constraintCount++;
            NodeStart.AddConstraint();
            NodeEnd.AddConstraint();
            switch (kind)
            {
                case EdgeKind.Feature: _featureCount++; break;
                case EdgeKind.Contour: _contourCount++; break;
                case EdgeKind.Hole: _holeCount++; break;
                default: _featureCount++; break;
            }
        }

        public bool RemoveConstraint(EdgeKind kind)
        {
            if (!Constrained)
                return false;

            _constraintCount--;
            NodeStart.RemoveConstraint();
            NodeEnd.RemoveConstraint();
            switch (kind)
            {
                case EdgeKind.Feature: if (_featureCount > 0) _featureCount--; break;
                case EdgeKind.Contour: if (_contourCount > 0) _contourCount--; break;
                case EdgeKind.Hole: if (_holeCount > 0) _holeCount--; break;
            }
            return true;
        }


    }
}
