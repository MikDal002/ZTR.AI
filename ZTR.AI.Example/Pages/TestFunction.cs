using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ZTR.AI.Algorithms.Core;
using ZTR.AI.Example.Linq;

namespace ZTR.AI.Example.Pages
{
    public record DataItem(Vector<double> X, double Y);

    public class TestFunction
    {
        public static TestFunction Sin { get; } = new()
        {
            Name = "Sin",
            Function = x => Math.Sin(x[0]),
            Step = 0.1,
            Dimensions = 1
        };

        public static TestFunction Cos { get; } = new()
        {
            Name = "Cos",
            Function = x => Math.Cos(x[0]),
            Step = 0.1,
            Dimensions = 1
        };

        public static TestFunction ShortExponental { get; } = new()
        {
            Name = "Sinus&Exponental",
            Function = x => -(x[0] + Math.Sin(x[0])) * Math.Exp(-Math.Pow(x[0], 2.0)),
            Step = 0.1,
            Dimensions = 1
        };

        public static TestFunction Sum { get; } = new()
        {
            Name = "Sum",
            Function = x => x.Sum(),
            Dimensions = 2,
            Min = -Vector.Build.Dense(2, Math.PI),
            Max = Vector.Build.Dense(2, Math.PI),

        };

        public static IReadOnlyCollection<TestFunction> AllExamples { get; } = new[]
        {
            // Single Dimensional
            Sin, Cos, ShortExponental,
            // Two-dimensional
            Sum
        };

        public string Name { get; init; } = string.Empty;

        private List<DataItem>? _drawingData;
        public IEnumerable<DataItem> DrawingData
        {
            get
            {
                if (_drawingData != null) return _drawingData;

                Min.MustBeTheSameCountAs(Max);
                var stepsPerAxis = Min.Zip(Max, (min, max) => MyLinq.Range(min, max, Step).ToList()).ToList();

                _drawingData = stepsPerAxis.CreateCartesianProduct().Select(d =>
                {
                    var doubles = Vector.Build.Dense(d);
                    return new DataItem(doubles, Function(doubles));
                }).ToList();

                return _drawingData;
            }
        }



        public Func<Vector<double>, double> Function { get; init; } = _ => 0.0;
        public double Step { get; init; } = 0.1;
        /// <summary>
        /// When is null is working for any dimension you want.
        /// </summary>
        public int Dimensions { get; init; }
        public Vector<double> Min { get; init; } = -Vector.Build.Dense(1, Math.PI);
        public Vector<double> Max { get; init; } = Vector.Build.Dense(1, Math.PI);
    }
}