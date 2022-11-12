using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ZTR.AI.Example.Linq;
using ZTR.AI.Example.Shared;

namespace ZTR.AI.Example.Pages
{
    public class SingleDimensionalExample
    {
        public static SingleDimensionalExample Sin { get; } = new()
        {
            Name = "Sin",
            Function = x=> Math.Sin(x[0]),
            Step = 0.1
        };

        public static SingleDimensionalExample Cos { get; } = new()
        {
            Name = "Cos",
            Function = x => Math.Cos(x[0]),
            Step = 0.1
        };

        public static SingleDimensionalExample ShortExponental { get; } = new()
        {
            Name = "Sinus&Exponental",
            Function = x => -(x[0] + Math.Sin(x[0])) * Math.Exp(-Math.Pow(x[0], 2.0)),
            Step = 0.1
        };

        public static IReadOnlyCollection<SingleDimensionalExample> AllExamples { get; } = new[]
        {
            Sin, Cos,  ShortExponental,
        };

        public string Name { get; init; } = string.Empty;
        public IEnumerable<DataItem> DrawingData =>
            MyMoreLinq.Range(Min[0], Max[0], Step).Select(d => new DataItem(d, Function(Vector.Build.Dense(1, d))));

        public Func<Vector<double>, double> Function { get; init; } = _ =>  0.0;
        public double Step { get; init; } = 0.1;
        public Vector<double> Min { get; init; } = -Vector.Build.Dense(1, Math.PI);
        public Vector<double> Max { get; init; } = Vector.Build.Dense(1, Math.PI);
    }
}