namespace TriUgla.Mesher.Quality
{
    public struct RefineSettings(
        int maxSteiners, 
        int faceStagnationBudget, 
        double improveEps)
    {
        public int MaxSteiners = maxSteiners;
        public int FaceStagnationBudget = faceStagnationBudget;
        public double ImproveEps = improveEps;

        public static readonly RefineSettings Default = new RefineSettings(
            maxSteiners: 1_000_000,
            faceStagnationBudget: 8,
            improveEps: 1e-4);
    }
}
