using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ZT.AI.Researcher
{
    public class TestFunctionProvider
    {
        public (Func<Vector<double>, double> function, Vector<double> min, Vector<double> max) GetFunction(TestFunction functionToGet)
        {
            switch (functionToGet)
            {
                case TestFunction.Sum:
                    return (x => x.Sum(), -Vector.Build.Dense(2, 5.2), Vector.Build.Dense(2, 5.2));
                case TestFunction.Step:
                    return (x => Math.Floor(x[0]) + Math.Floor(x[1]) , -Vector.Build.Dense(2, 5.2), Vector.Build.Dense(2, 5.2));
                case TestFunction.Rosenbrock:
                    return (x => Math.Pow(1 - x[0],2) + 100 * Math.Pow(x[1] - Math.Pow(x[0], 2), 2), -Vector.Build.Dense(2, 5.2), Vector.Build.Dense(2, 5.2));
                case TestFunction.Rastrigin:
                    return (
                        x => 20 + x.Sum(z => Math.Pow(z, 2) - 10 * Math.Cos(2 * Math.PI * z)), 
                        -Vector.Build.Dense(2, 5.2), Vector.Build.Dense(2, 5.2));
                case TestFunction.Schwefel:
                    return (
                        x => 418.9829 * 2 - x.Sum(z => z * Math.Sin(Math.Sqrt(Math.Abs(z)))), 
                        -Vector.Build.Dense(2, -500), Vector.Build.Dense(2, 500));
                case TestFunction.Easom:
                    return (
                        x => -Math.Cos(x[0]) * Math.Cos(x[1]) * Math.Exp(-Math.Pow(x[0] - Math.PI, 2) - Math.Pow(x[1] - Math.PI, 2)),
                        -Vector.Build.Dense(2, 100), Vector.Build.Dense(2, 100));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}