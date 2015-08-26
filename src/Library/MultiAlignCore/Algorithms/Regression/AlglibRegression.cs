using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions;

namespace MultiAlignCore.Algorithms.Regression
{
    public class AlglibRegression: IRegression<double>
    {
        public FitReport Fit(IEnumerable<double> x, IEnumerable<double> y, BasisFunctionsEnum basisFunction, ref double[] coeffs)
        {

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(basisFunction);
            
            //incase the coefficients input are the wrong dimension
            var coeffCount = functionSelector.Coefficients.Count();
            if(coeffs.Length!=coeffCount)
            {
                coeffs = functionSelector.Coefficients;
            }

            var worked = EvaluateFunction(x.ToList(), y.ToList(), functionSelector, ref coeffs);

            var results = new FitReportALGLIB(worked, worked.DidConverge);
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="initialCoefficients"></param>
        /// <param name="functionChoise"></param>
        /// <returns></returns>
        protected SolverReport EvaluateFunction(List<double> x, List<double> y, BasisFunctionBase basisFunction, ref double[] coeffs)
        {
            coeffs = basisFunction.Coefficients;
            basisFunction.Scale(x);

            alglib.ndimensional_pfunc myDelegate = basisFunction.FunctionDelegate;
            var solver = new LevenburgMarquadtSolver();
            solver.BasisFunction = myDelegate;
            var worked = solver.Solve(x, y, ref coeffs);
            
            //for (int i = 0; i < x.Count; i++)
            //{

            //    // This is what we are fitting 
            //    double xValue = x[i];

            //    // This is what it should fit to
            //    double yValue = y[i];

            //    // This is the warped guy
            //    double fitValue = 0;
            //    //quadSolver2.FunctionDelegate(coeffs, new double[] { xValue }, ref fitValue, null);
            //    myDelegate.Invoke(coeffs, new double[] { xValue }, ref fitValue, null);
            //}
            //Console.WriteLine(Environment.NewLine);
            return worked;
        }
    }
}
