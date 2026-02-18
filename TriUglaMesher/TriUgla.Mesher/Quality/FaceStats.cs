namespace TriUgla.Mesher.Quality
{
    public readonly struct FaceStats(
      double signedArea,
      double minLen2,
      double maxLen2,
      double avgVertexArea,
      double cx, double cy)
    {
        public readonly double SignedArea = signedArea;
        public readonly double MinLen2 = minLen2;
        public readonly double MaxLen2 = maxLen2;
        public readonly double AvgVertexArea = avgVertexArea;
        public readonly double Cx = cx;
        public readonly double Cy = cy;
    }
}
