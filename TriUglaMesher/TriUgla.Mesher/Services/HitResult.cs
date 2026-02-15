using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TriUgla.HalfEdge;

namespace TriUgla.Mesher.Services
{
    public readonly struct HitResult
    {
        public readonly HitKind Kind;
        public readonly Face AtFace;
        public readonly Face? Face;
        public readonly Edge? Edge;
        public readonly Node? Node;
        public readonly int Steps;
        public readonly bool Capped;

        private HitResult(
            HitKind kind,
            Face at,
            Face? f,
            Edge? e,
            Node? n,
            int steps,
            bool capped)
        {
            Kind = kind;
            AtFace = at;
            Face = f;
            Edge = e;
            Node = n;
            Steps = steps;
            Capped = capped;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitResult None(Face at, int steps, bool capped = false)
           => new HitResult(
               HitKind.None,
               at,
               null,
               null,
               null,
               steps,
               capped);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitResult FaceHit(Face at, Face f, int steps, bool capped = false)
            => new HitResult(
                HitKind.Face,
                at,
                f,
                null,
                null,
                steps,
                capped);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitResult EdgeHit(Face at, Edge e, int steps, bool capped = false)
            => new HitResult(
                HitKind.Edge,
                at,
                null,
                e,
                null,
                steps,
                capped);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitResult NodeHit(Face at, Node n, int steps, bool capped = false)
            => new HitResult(
                HitKind.Node,
                at,
                null,
                null,
                n,
                steps,
                capped);
    }
}
