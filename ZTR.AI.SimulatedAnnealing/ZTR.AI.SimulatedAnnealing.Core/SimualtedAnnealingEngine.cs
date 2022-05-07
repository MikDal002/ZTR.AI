using System;

namespace ZTR.AI.SimulatedAnnealing.Core.Tests
{
    internal class SimualtedAnnealingEngine
    {
        private Func<object, double> value;

        public SimualtedAnnealingEngine(Func<object, double> value)
        {
            this.value = value;
        }
    }
}