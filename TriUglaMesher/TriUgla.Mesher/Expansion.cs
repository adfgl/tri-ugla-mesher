public class Expansion
{
    public void Sum(double a, double b, 
            out double stored,
            out double lost)
    {
        stored = a + b;
        double storedB = stored - a;
        double lostA = stored - b;
        double lostB = b - bStored;
        lost = lostA + lostB;
    }
}