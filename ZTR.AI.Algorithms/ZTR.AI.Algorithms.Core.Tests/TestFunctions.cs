using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests
{
    public static class TestFunctions
    {

        public static readonly IEnumerable<object> SimpleFunctionsWithErrorMargin = new List<object>()
        {
            new object[] {new Func<Vector<double>, double>(x =>
            {
                if (x.Any(d => d <= 0)) return 0;
                else return x.Sum();
            }), 0, 0.1 },
            //new object[] {new Func<Vector<double>, double>(x => Math.Pow(x, 2.0) ), 0, 0.2 },

            //// Non-Convex Unimodal Functions https://machinelearningmastery.com/1d-test-functions-for-function-optimization/
            //new object[] {new Func<Vector<double>, double>(x => (-(x + Math.Sin(x)) * Math.Exp(-Math.Pow(x, 2.0)))), -0.82423939785504219, 1 },
            new object[] { new Func<Vector<double>, double>(x => Math.Sin(x[0]) * Math.Sin(x[1])), -1.0, 0.1 },
            //new object[] {new Func<Vector<double>, double>(Math.Cos), -1, 0.1 },
        };
    }
}