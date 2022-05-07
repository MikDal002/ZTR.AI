using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ZTR.AI.SimulatedAnnealing.Core
{
    /// <summary>
    /// Other names of this algorithms are: Metropolis, Monte Carlo Annealing,
    /// Probabilistic hill climbing, Stochastic relaxation.
    /// </summary>
    public class SimulatedAnnealing
    {
        public SimulatedAnnealing(Func<double, double> functionToOptimze)
        {
            FunctionToOptimize = functionToOptimze;
        }

        public Func<double, double> FunctionToOptimize { get; }
        public double Result => FunctionToOptimize(0);
    }
}
