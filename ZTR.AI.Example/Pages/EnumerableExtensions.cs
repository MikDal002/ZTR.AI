using Light.GuardClauses;

namespace ZTR.AI.Example.Pages
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<double[]> CreateCartesianProduct(this List<List<double>> input)
        {
            input.MustNotBeNull();

            var currentIndexes = new int[input.Count];
            var ranges = input.Select(d => d.Count - 1).ToList();

            while (true)
            {
                // Constructing output vector
                var output = new double[input.Count];

                for (int i = 0; i < input.Count; i++)
                {
                    output[i] = input[i][currentIndexes[i]];
                }

                yield return output;


                // Checking if we should finish

                bool areIndexesAtTheEnd = true;
                for (int i = 0; i < input.Count; i++)
                {
                    areIndexesAtTheEnd = areIndexesAtTheEnd && currentIndexes[i] == ranges[i];
                    if (!areIndexesAtTheEnd) break;
                }

                if (areIndexesAtTheEnd) break;
                
                // Increasing indexed
                
                for (int i = 0; i < input.Count; i++)
                {
                    if (currentIndexes[i] == ranges[i])
                    {
                        currentIndexes[i] = 0;
                    }
                    else
                    {
                        currentIndexes[i]++;
                        break;
                    }
                }

            }

        }
    }
}