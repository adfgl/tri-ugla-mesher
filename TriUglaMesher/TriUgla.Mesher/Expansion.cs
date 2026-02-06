public struct Expansion
{

    double[] _parts;
    int _count = 0;

    public Expansion(params double[] parts)
    {
        _parts = parts;
        _count = parts.Length;
    }

    public void Add(double value)
    {
        int n = _parts.Length;
        for (int i = 0; i < n; i++)
        {
            Sum(_parts[i], value, 
                out value,
                out double lost);
            _parts[i] = lost;
        }

        if (value != 0)
        {
            _parts.Add(value);
        }
    }

    public static void Split(double a, 
            out double high,
            out double low)
    {
        const double splitter = 134217729.0;
        double c = splitter * a;
        double abig = c - a;
        high = c - abig;
        low = a - hi;
    }

    public static void Prod(double a, double b, out double stored, out double lost)
    {
        stored = a * b;
        Split(a, out double aHi, out double aLo);
        Split(b, out double bHi, out double bLo);
        lost = ((aHi * bHi - stored) + aHi * bLo + aLo * bHi) + aLo * bLo;
    }

    public static void Sum(double a, double b, 
            out double stored,
            out double lost)
    {
        stored = a + b;
        double storedB = stored - a;
        lost = 
        /* lost A */ (a - (stored - storedB)) +
        /* lost B */ (b - storedB);
    }
}
