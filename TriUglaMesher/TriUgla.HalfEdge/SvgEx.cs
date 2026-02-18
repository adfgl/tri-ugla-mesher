using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TriUgla.Geometry;

namespace TriUgla.HalfEdge
{
    public static class MeshSvg
    {
        readonly struct FaceCollector(List<Face> faces) : IFaceProcessor
        {
            public bool ProcessAndContinue(Face face)
            {
                faces.Add(face);
                return true;
            }
        }

        public static string ToSvg(
            this HalfEdgeMesh mesh,
            double width = 1000,
            double height = 1000,
            double padding = 10,
            bool preserveAspect = true,
            double strokeWidth = 0.8,
            bool fillFaces = true,
            double faceOpacity = 0.5)
        {
            if (mesh is null) throw new ArgumentNullException(nameof(mesh));
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (padding < 0) throw new ArgumentOutOfRangeException(nameof(padding));

            // 1) Collect faces + compute bounds from geometry

            List<Face> faces = new List<Face>();
            FaceCollector faceCollector = new FaceCollector(faces);
            mesh.ForeachFace(mesh.Root, ref faceCollector);

            Rect bounds = Rect.Empty;
            foreach (var item in faces)
            {
                Edge e0 = item.Edge;
                Edge e = e0;
                do
                {
                    Vec4 vtx = e.NodeStart.Vertex;
                    bounds = bounds.Union(new Vec2(vtx.x, vtx.y));
                    e = e.Next;
                } while (e0 != e);
            }

            double worldW = bounds.maxX - bounds.minX;
            double worldH = bounds.maxY - bounds.minY;
            if (worldW <= 0 || worldH <= 0)
                throw new ArgumentException("Computed mesh bounds are invalid (empty or degenerate).", nameof(mesh));

            var map = new FitMapper(bounds, width, height, padding, preserveAspect);

            var sb = new StringBuilder(1 << 20);

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" ")
              .Append("width=\"").Append(F(width)).Append("\" ")
              .Append("height=\"").Append(F(height)).Append("\" ")
              .Append("viewBox=\"0 0 ").Append(F(width)).Append(" ").Append(F(height)).Append("\">")
              .AppendLine();

            sb.AppendLine("<rect x=\"0\" y=\"0\" width=\"100%\" height=\"100%\" fill=\"white\"/>");

            // 2) Faces fill
            if (fillFaces)
            {
                sb.AppendLine("<g stroke=\"none\">");

                for (int i = 0; i < faces.Count; i++)
                {
                    Face f = faces[i];
                    if (f.Kind == FaceKind.Outside) continue;

                    // triangle vertices (adjust if your face vertex access differs)
                    Vec4 a = f.Edge.NodeStart.Vertex;
                    Vec4 b = f.Edge.Next.NodeStart.Vertex;
                    Vec4 c = f.Edge.Prev.NodeStart.Vertex;

                    (double ax, double ay) = map.WorldToSvg(a);
                    (double bx, double by) = map.WorldToSvg(b);
                    (double cx, double cy) = map.WorldToSvg(c);

                    string fill = FillColor(f.Kind);

                    sb.Append("<polygon points=\"")
                      .Append(F(ax)).Append(",").Append(F(ay)).Append(" ")
                      .Append(F(bx)).Append(",").Append(F(by)).Append(" ")
                      .Append(F(cx)).Append(",").Append(F(cy))
                      .Append("\" fill=\"").Append(fill)
                      .Append("\" fill-opacity=\"").Append(F(faceOpacity))
                      .AppendLine("\"/>");
                }

                sb.AppendLine("</g>");
            }

            // 3) Edges (dedup) with border coloring
            sb.AppendLine("<g fill=\"none\" stroke-linecap=\"round\" stroke-linejoin=\"round\">");

            var seen = new HashSet<EdgeKey>(capacity: 1 << 16);

            for (int i = 0; i < faces.Count; i++)
            {
                Face f = faces[i];
                if (f.Kind == FaceKind.Outside) continue;

                Edge e0 = f.Edge;
                Edge e = e0;
                do
                {
                    // dedup undirected
                    var key = EdgeKey.From(e.NodeStart, e.NodeEnd);
                    if (!seen.Add(key))
                    {
                        e = e.Next;
                        continue;
                    }

                    // Determine kinds on both sides
                    FaceKind leftKind = e.Face.Kind;
                    FaceKind rightKind = e.Twin is null ? FaceKind.Outside : e.Twin.Face.Kind;

                    bool isBorder = e.Twin is null || leftKind != rightKind;

                    string stroke = isBorder
                        ? BorderColor(leftKind, rightKind)
                        : InteriorColor(leftKind); // same kind

                    (double x1, double y1) = map.WorldToSvg(e.NodeStart.Vertex);
                    (double x2, double y2) = map.WorldToSvg(e.NodeEnd.Vertex);

                    sb.Append("<line x1=\"").Append(F(x1)).Append("\" y1=\"").Append(F(y1))
                      .Append("\" x2=\"").Append(F(x2)).Append("\" y2=\"").Append(F(y2))
                      .Append("\" stroke=\"").Append(stroke)
                      .Append("\" stroke-width=\"").Append(F(strokeWidth))
                      .AppendLine("\"/>");

                    e = e.Next;
                }
                while (!ReferenceEquals(e, e0));
            }

            sb.AppendLine("</g>");
            sb.AppendLine("</svg>");
            return sb.ToString();
        }

        static string FillColor(FaceKind kind) => kind switch
        {
            FaceKind.Land => "#4CAF50",   // green
            FaceKind.Lake => "#42A5F5",   // blue
            _ => "#9E9E9E",               // outside gray
        };

        static string InteriorColor(FaceKind kind) => kind switch
        {
            FaceKind.Land => "#2E7D32",   // medium green
            FaceKind.Lake => "#1565C0",   // medium blue
            _ => "#616161",               // medium gray
        };

        static string BorderColor(FaceKind a, FaceKind b)
        {
            // Border belongs to whichever "non-outside" type touches it.
            // Land beats Lake beats Outside. (If you hate this policy, blame topology.)
            if (a == FaceKind.Land || b == FaceKind.Land) return "#1B5E20"; // dark green
            if (a == FaceKind.Lake || b == FaceKind.Lake) return "#0D47A1"; // dark blue
            return "#424242"; // dark gray
        }

        readonly struct FitMapper
        {
            readonly double _minX, _minY, _maxY;
            readonly double _sx, _sy;
            readonly double _offX, _offY;

            public FitMapper(in Rect world, double svgW, double svgH, double pad, bool preserveAspect)
            {
                _minX = world.minX;
                _minY = world.minY;
                _maxY = world.maxY;

                double ww = world.maxX - world.minX;
                double wh = world.maxY - world.minY;

                double availW = Math.Max(1e-12, svgW - 2 * pad);
                double availH = Math.Max(1e-12, svgH - 2 * pad);

                double sx = availW / ww;
                double sy = availH / wh;

                if (preserveAspect)
                {
                    double s = Math.Min(sx, sy);
                    _sx = s;
                    _sy = s;

                    double usedW = ww * s;
                    double usedH = wh * s;

                    _offX = pad + (availW - usedW) * 0.5;
                    _offY = pad + (availH - usedH) * 0.5;
                }
                else
                {
                    _sx = sx;
                    _sy = sy;

                    _offX = pad;
                    _offY = pad;
                }
            }

            public (double x, double y) WorldToSvg(in Vec4 p)
            {
                double x = _offX + (p.x - _minX) * _sx;
                double y = _offY + (_maxY - p.y) * _sy; // flip Y
                return (x, y);
            }
        }

        static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

        readonly struct EdgeKey : IEquatable<EdgeKey>
        {
            readonly nint _a;
            readonly nint _b;

            EdgeKey(nint a, nint b) { _a = a; _b = b; }

            public static EdgeKey From(Node a, Node b)
            {
                nint pa = (nint)System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(a);
                nint pb = (nint)System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(b);
                return pa <= pb ? new EdgeKey(pa, pb) : new EdgeKey(pb, pa);
            }

            public bool Equals(EdgeKey other) => _a == other._a && _b == other._b;
            public override bool Equals(object? obj) => obj is EdgeKey k && Equals(k);

            public override int GetHashCode()
            {
                unchecked
                {
                    long x = (long)_a;
                    long y = (long)_b;
                    return (int)(x * 486187739 + y * 16777619);
                }
            }
        }

        // Reference comparer for HashSet<Face>
        sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
            where T : class
        {
            public static readonly ReferenceEqualityComparer<T> Instance = new();

            public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
            public int GetHashCode(T obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
