using ZTR.AI.Example.Linq;
using ZTR.AI.Example.Shared;

namespace ZTR.AI.Example.Pages
{
    public class SingleDimensionalExample
    {
        public static SingleDimensionalExample Sin { get; } = new()
        {
            Name = "Sin",
            Function = Math.Sin,
            Step = 0.1,
            Min = -Math.PI * 1.1,
            Max = Math.PI * 1.1
        };

        public static SingleDimensionalExample Cos { get; } = new()
        {
            Name = "Cos",
            Function = Math.Cos,
            Step = 0.1,
            Min = -Math.PI * 1.1,
            Max = Math.PI * 1.1
        };

        public static SingleDimensionalExample ShortExponental { get; } = new()
        {
            Name = "Sinus&Exponental",
            Function = x => -(x + Math.Sin(x)) * Math.Exp(-Math.Pow(x, 2.0)),
            Step = 0.1,
            Min = -Math.PI * 1.1,
            Max = Math.PI * 1.1
        };


        public string Name { get; init; } = string.Empty;
        public IEnumerable<DataItem> DrawingData =>
            MyMoreLinq.Range(Min, Max, Step).Select(d => new DataItem(d, Function(d)));

        public Func<double, double> Function { get; init; } = _ =>  0.0;
        public double Step { get; init; } = 0.1;
        public double Min { get; init; } = -Math.PI;
        public double Max { get; init; } = Math.PI;
    }
}