using TriUgla.Mesher.MeshCore;

namespace TriUgla.Mesher.Services
{
    public sealed class Linker(Mesh mesh)
    {
        readonly Mesh _mesh = mesh;

        public void TwinAll(List<EdgeId> list)
        {
            int n = list.Count;

            for (int i = 0; i < n; i++)
            {
                EdgeId ei = list[i];
                for (int j = i + 1; j < n; j++)
                {
                    EdgeId ej = list[j];
                    if (TryTwin(ei, ej))
                    {
                        break;
                    }
                }
            }
        }

        public bool TryTwin(EdgeId a, EdgeId b)
        {
            ref Edge abEdge = ref _mesh.Ref(a);
            ref Edge baEdge = ref _mesh.Ref(b);

            if (abEdge.start.value == baEdge.end.value &&
                abEdge.end.value == baEdge.start.value)
            {
                Twin(a, b);
                return true;
            }
            return false;
        }

        public void Twin(EdgeId ab, EdgeId ba)
        {
            ref Edge abEdge = ref _mesh.Ref(ab);
            ref Edge baEdge = ref _mesh.Ref(ba);
            abEdge.twin = ba;
            baEdge.twin = ba;
        }

        public void MakeTwins(out EdgeId ab, out EdgeId ba)
        {
            ab = new EdgeId(_mesh.Edges.Rent(new Edge()));
            ba = new EdgeId(_mesh.Edges.Rent(new Edge()));
            Twin(ab, ba);
        }

        public void LinkElements(
            FaceId face,
            EdgeId edge0, EdgeId edge1, EdgeId edge2,
            NodeId node0, NodeId node1, NodeId node2)
        {
            ref Face f = ref _mesh.Ref(face);
            
            ref Edge e0 = ref _mesh.Ref(edge0);
            ref Edge e1 = ref _mesh.Ref(edge1);
            ref Edge e2 = ref _mesh.Ref(edge2);

            ref Node n0 = ref _mesh.Ref(node0);
            ref Node n1 = ref _mesh.Ref(node1);
            ref Node n2 = ref _mesh.Ref(node2);

            f.edge = edge0;

            e0.face = face;
            e0.next = edge1; 
            e0.prev = edge2;
            e0.start = node0;
            e0.end = node1;

            e1.face = face;
            e1.next = edge2;
            e1.prev = edge0;
            e1.start = node1;
            e1.end = node2;

            e2.face = face;
            e2.next = edge0;
            e2.prev = edge1;
            e2.start = node2;
            e2.end = node0;
        }
    }
}
