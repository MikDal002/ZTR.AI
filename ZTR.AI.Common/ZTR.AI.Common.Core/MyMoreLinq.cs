using System;
using System.Collections.Generic;

namespace ZTR.AI.Example.Linq;

public static class MyMoreLinq
{
    public static IEnumerable<double> Range(double start, double stop, double step)
    {
        for (var i = start; i <= stop; i += step)
        {
            yield return i;
        }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var element in source)
        {
            action(element);
        }
    }
}