using System.Collections;
using System.Runtime.CompilerServices;
using Light.GuardClauses.Exceptions;

namespace ZTR.AI.Algorithms.Core
{
    public static class VectorExtensions
    {
        public static T MustBeTheSameCountAs<T>(this T parameter, T other, [CallerArgumentExpression("parameter")] string? parameterName = null, string? message = null)
            where T : ICollection
        {
            if (parameter.Count != other.Count)
            {
                Throw.Argument(parameterName, message ?? "Size of vectors must be the same");
            }

            return parameter;
        }
    }
}