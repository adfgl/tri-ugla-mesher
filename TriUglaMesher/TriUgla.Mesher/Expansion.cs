public class Expansion
{
    public void Sum(double a, double b, 
            out double stored,
            out double lost)
    {
        stored = a + b;
        double storedB = stored - a;
        lost = 
        /* lost A */ (a - (stored - storedB)) +
        /* lost B */ (b - storedB) 
    }
}