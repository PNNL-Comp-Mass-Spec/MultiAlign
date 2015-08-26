using System;

namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions
{
    /// <summary>
    /// Basis function for the LM Algorithm using Gaussian Peak Shapes
    /// </summary>
    public class Gaussian : BasisFunctionBase
    {
        /// <summary>
        /// Evalutates the second order chebyshev polynomials
        /// </summary>
        /// <param name="c">Set of coefficients</param>
        /// <param name="x">Input variables</param>
        /// <param name="functionResult">Returned sum value of your function</param>
        /// <param name="obj">?</param>
        public override void FunctionDelegate(double[] c, double[] x, ref double functionResult, object obj)
        {
            //=a*1/(2*PI()*sigma*sigma)^0.5*EXP(-((X-Offset)^2)/2*sigma*sigma)
            //sum = height * 1 / Math.Pow((2.0 * pi * sigma * sigma), 0.5) * Math.Exp(0.5 * Math.Exp(-((x[0] - xOffset) * (x[0] - xOffset))) / (2.0 * sigma * sigma));

            functionResult = 0;

            var sigma = c[0];
            var height = c[1];
            var xOffset = c[2];

            //=a*EXP(-(X^2)/(2*sigma^2))
            functionResult = height * Math.Exp(-(Math.Pow((x[0] - xOffset), 2)) / (2.0 * sigma * sigma));
        }
    }
}
