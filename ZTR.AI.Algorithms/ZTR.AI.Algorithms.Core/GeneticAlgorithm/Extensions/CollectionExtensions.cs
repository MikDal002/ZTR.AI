using System;
using System.Collections.Generic;
using System.Linq;

namespace ZTR.AI.Algorithms.Core.GeneticAlgorithm.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<T> Shuffle<T>(this ICollection<T> input)
    {
        var allNumbers = Enumerable.Range(0, input.Count).ToList();
        var rnd = new Random();
        for (var i = 0; i < input.Count; ++i)
        {
            var selectedNumber = rnd.Next(0, allNumbers.Count);
            yield return input.ElementAt(allNumbers[selectedNumber]);
            allNumbers.RemoveAt(selectedNumber);
        }
    }

    public static byte[] ToBytes(this IEnumerable<bool> bools2)
    {
        var bools = bools2 as List<bool> ?? bools2.ToList();
        //byte[] arr1 = Array.ConvertAll(bools, b => b ? (byte)1 : (byte)0);

        // pack (in this case, using the first bool as the lsb - if you want
        // the first bool as the msb, reverse things ;-p)
        var bytes = bools.Count / 8;
        if (bools.Count % 8 != 0) bytes++;
        var arr2 = new byte[bytes];
        int bitIndex = 0, byteIndex = 0;
        for (var i = 0; i < bools.Count; i++)
        {
            if (bools.ElementAt(i)) arr2[byteIndex] |= (byte)(1 << bitIndex);
            bitIndex++;
            if (bitIndex == 8)
            {
                bitIndex = 0;
                byteIndex++;
            }
        }

        return arr2;
    }

    //public static byte[] ToBytes(this IList<bool> bools)
    //{
    //    //byte[] arr1 = Array.ConvertAll(bools, b => b ? (byte)1 : (byte)0);

    //    // pack (in this case, using the first bool as the lsb - if you want
    //    // the first bool as the msb, reverse things ;-p)
    //    int bytes = bools.Count / 8;
    //    if (bools.Count % 8 != 0) bytes++;
    //    byte[] arr2 = new byte[bytes];
    //    int bitIndex = 0, byteIndex = 0;
    //    for (int i = 0; i < bools.Count; i++)
    //    {
    //        if (bools.ElementAt(i)) arr2[byteIndex] |= (byte) (1 << bitIndex);
    //        bitIndex++;
    //        if (bitIndex == 8)
    //        {
    //            bitIndex = 0;
    //            byteIndex++;
    //        }
    //    }

    //    return arr2;
    //}
}