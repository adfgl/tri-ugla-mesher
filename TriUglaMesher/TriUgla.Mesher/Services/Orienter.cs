using TriUgla.Mesher.MeshCore;
using TriUgla.Mesher.Utils;

namespace TriUgla.Mesher.Services
{
    public sealed class Orienter(Mesh mesh)
    {
        public Circle CircumCircle(Id face)
        {
            ref readonly Face abc = ref mesh.Faces.Ref(face);
            ref readonly Edge ab = ref mesh.Edges.Get(abc.edge);
            ref readonly Edge bc = ref mesh.Edges.Get(ab.next);

            Vec2 a = mesh.Nodes.Get(ab.start).vertex.AsVec2();
            Vec2 b = mesh.Nodes.Get(ab.end).vertex.AsVec2();
            Vec2 c = mesh.Nodes.Get(bc.end).vertex.AsVec2();

            return Circle.From3(a, b, c);
        }
    }
}
