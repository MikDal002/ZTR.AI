﻿using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Light.GuardClauses.Exceptions;

namespace ZTR.AI.Algorithms.Core
{
    public static class CollectionAssertionsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNull]
        public static T MustBeTheSameCountAs<T>([NotNull]  this T parameter, [NotNull] T other, [CallerArgumentExpression("parameter")] string? parameterName = null, string? message = null)
            where T : ICollection
        {
            if (parameter.Count != other.Count)
            {
                throw new ArgumentException(parameterName, message ?? "Size of vectors must be the same");
            }

            return parameter;
        }
    }
}