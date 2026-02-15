using TriUgla.Geometry;
using TriUgla.HalfEdge;
using TriUgla.Mesher.Topology;

namespace TriUgla.Mesher.Services
{
    public sealed class NodeInserter(
        NodeSplitter nodeSplitter, 
        EdgeSplitter edgeSplitter, 
        FaceSplitter faceSplitter)
    {
        readonly NodeSplitter _nodeSplitter = nodeSplitter;
        readonly EdgeSplitter _edgeSplitter = edgeSplitter;
        readonly FaceSplitter _faceSplitter = faceSplitter;

        public Node Insert(in HitResult hit, Vec2 pos, NodeKind kind)
        {
            Vec4 v = hit.Kind switch
            {
                HitKind.Face => Interpolation.Interpolate(pos, hit.Face!),
                HitKind.Edge => Interpolation.Interpolate(pos, hit.Edge!),
                HitKind.Node => Interpolation.Interpolate(pos, hit.Node!),
                _ => throw new InvalidOperationException("Hit.None cannot be inserted.")
            };
            return Insert(in hit, v, kind);
        }

        public Node Insert(in HitResult hit, Vec4 vertex, NodeKind kind)
        {
            Node node;
            if (HitKind.Node == hit.Kind)
            {
                node = hit.Node!;
            }
            else
            {
                node = new Node(vertex, kind);
            }
            return Insert(in hit, node);
        }

        public Node Insert(in HitResult hit, Node node)
        {
            switch (hit.Kind)
            {
                case HitKind.Face:
                    _faceSplitter.Split(hit.Face!, node);
                    break;

                case HitKind.Edge:
                    _edgeSplitter.Split(hit.Edge!, node);
                    break;

                case HitKind.Node:
                    _nodeSplitter.Split(hit.Node!, node);
                    break;

                default:
                    throw new InvalidOperationException("Hit.None cannot be inserted.");
            }
            return node;
        }
    }
}
