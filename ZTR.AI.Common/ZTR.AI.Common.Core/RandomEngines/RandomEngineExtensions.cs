using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ZTR.AI.Algorithms.Core;

namespace ZTR.AI.Common.Core.RandomEngines;

public static class RandomEngineExtensions
{
    /// <summary>
    /// Returns value in range from -1.0 to 1.0 using random engine.
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public static double NextDoubleFromMinusOneToOne([NotNull] this IRandomEngine random)
    {
        return random.NextDouble() * 2.0 - 1.0;
    }

    public static Vector<double> NextVectorFromRange([NotNull] this IRandomEngine randomEngine, [NotNull] Vector<double> minVector,
        [NotNull] Vector<double> maxVector)
    {
        minVector.MustBeTheSameCountAs(maxVector);

        var count = minVector.Count;
        var vectorInternal = new double[count];
        for (var i = 0; i < count; i++)
        {
            vectorInternal[i] = randomEngine.NextDoubleFromRange(minVector[i], maxVector[i]);
        }

        return Vector.Build.Dense(vectorInternal);

    }

    /// <summary>
    /// Returns value between minValue and maxValue excluding.
    /// Most part of this method is taken from KGySoft.CoreLibraries, which you can find here: https://github.com/koszeggy/KGySoft.CoreLibraries.
    /// This code is under the KGy SOFT License.
    /// Original source code: https://github.com/koszeggy/KGySoft.CoreLibraries/blob/aba2a41e0c6ead882737737f814d4c11d3f0e9f1/KGySoft.CoreLibraries/CoreLibraries/_Extensions/RandomExtensions.cs#L784
    /// The KGy SOFT License is available at https://github.com/koszeggy/KGySoft.CoreLibraries/blob/master/LICENSE
    /// </summary>
    /// <param name="random"></param>
    /// <param name="minValue">Lower boundary of random value. If minValue is negative infinity then lower boundary equals double.MinValue.</param>
    /// <param name="maxValue">Higher boundary of random value. If maxValue is positive infinity then lower boundary equals double.MaxValue.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [ExcludeFromCodeCoverage(Justification = "Because this code is taken from github and it's covered there.")]
    public static double NextDoubleFromRange([NotNull] this IRandomEngine random, double minValue, double maxValue)
    {
        if (minValue > maxValue) throw new ArgumentException($"minValue must be lower than maxValue");
        if (double.IsPositiveInfinity(minValue) && double.IsPositiveInfinity(maxValue) || double.IsNegativeInfinity(minValue) && double.IsNegativeInfinity(maxValue))
            throw new ArgumentException("minValue and maxValue cannot be infinite at the same moment.");
        
        if (double.IsNaN(minValue)) throw new ArgumentOutOfRangeException(nameof(minValue), "Min cannot be NaN");
        if (double.IsNaN(maxValue)) throw new ArgumentOutOfRangeException(nameof(minValue), "Max cannot be NaN");

        static double AdjustValue(double value) => Double.IsNegativeInfinity(value) ? Double.MinValue : (Double.IsPositiveInfinity(value) ? Double.MaxValue : value);

        minValue = AdjustValue(minValue);
        maxValue = AdjustValue(maxValue);
        if (minValue == maxValue)
            return minValue;

        var posAndNeg = minValue < 0d && maxValue > 0d;
        var minAbs = Math.Min(Math.Abs(minValue), Math.Abs(maxValue));
        var maxAbs = Math.Max(Math.Abs(minValue), Math.Abs(maxValue));

        int sign;
        if (!posAndNeg)
            sign = minValue < 0d ? -1 : 1;
        else
        {
            // if both negative and positive results are expected we select the sign based on the size of the ranges
            var sample = random.NextDouble();
            var rate = minAbs / maxAbs;
            var absMinValue = Math.Abs(minValue);
            var isNeg = absMinValue <= maxValue
                ? rate / 2d > sample
                : rate / 2d < sample;
            sign = isNeg ? -1 : 1;

            // now adjusting the limits for 0..[selected range]
            minAbs = 0d;
            maxAbs = isNeg ? absMinValue : Math.Abs(maxValue);
        }

        // Possible double exponents are -1022..1023 but we don't generate too small exponents for big ranges because
        // that would cause too many almost zero results, which are much smaller than the original NextDouble values.
        var minExponent = minAbs == 0d ? -16d : Math.Log(minAbs, 2d);
        var maxExponent = Math.Log(maxAbs, 2d);
        if (minExponent == maxExponent)
            return minValue;

        // We decrease exponents only if the given range is already small. Even lower than -1022 is no problem, the result may be 0
        if (maxExponent < minExponent)
            minExponent = maxExponent - 4;

        var result = sign * Math.Pow(2d, NextDoubleLinear(random, minExponent, maxExponent));

        // protecting ourselves against inaccurate calculations; however, in practice result is always in range.
        return result < minValue ? minValue : (result > maxValue ? maxValue : result);
    }
    
    [ExcludeFromCodeCoverage(Justification = "Because this code is taken from github and it's covered there.")]
    private static double NextDoubleLinear(IRandomEngine random, double minValue, double maxValue)
    {
        var sample = random.NextDouble();
        var result = (maxValue * sample) + (minValue * (1d - sample));

        // protecting ourselves against inaccurate calculations; occurs only near MaxValue.
        return result < minValue ? minValue : (result > maxValue ? maxValue : result);
    }
}