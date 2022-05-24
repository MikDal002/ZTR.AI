namespace ZTR.AI.Example.Shared;

public static class MyMoreLinq
{
    public static IEnumerable<double> Range(double start, double stop, double step)
    {
        for (var i = start; i <= stop; i += step)
        {
            yield return i;
        }
    }
}