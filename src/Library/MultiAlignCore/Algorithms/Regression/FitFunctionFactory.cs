using System;

namespace MultiAlignCore.Algorithms.Regression
{
    public class FitFunctionFactory
    {
        public static Func<double, double> Create(FitFunctionTypes type)
        {
            Func<double, double> func = null;
            switch (type)
            {
                case FitFunctionTypes.Sin:
                    func = FitFunctions.Sin;
                    break;
                case FitFunctionTypes.Linear:
                    func = FitFunctions.Linear;
                    break;
                case FitFunctionTypes.Quadratic:
                    func = FitFunctions.Quadratic;
                    break;
                case FitFunctionTypes.Cubic:
                    func = FitFunctions.Cubic;
                    break;
                case FitFunctionTypes.TriCubic:
                    func = FitFunctions.Tricube;
                    break;
            }
            return func;
        }
    }
}