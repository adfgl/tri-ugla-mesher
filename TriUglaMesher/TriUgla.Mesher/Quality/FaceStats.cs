namespace TriUgla.Mesher.Quality
{
    public readonly struct FaceStats(
      double signedArea,
      double absArea,
      double minLen2,
      double maxLen2,
      double avgVertexArea)
    {
        public readonly double SignedArea = signedArea;
        public readonly double AbsArea = absArea;
        public readonly double MinLen2 = minLen2;
        public readonly double MaxLen2 = maxLen2;
        public readonly double AvgVertexArea = avgVertexArea;
    }
}
