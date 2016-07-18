namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions
{
    /// <summary>
    /// Basis function for adding a fit to a linear function.
    /// </summary>
    public class Linear : BasisFunctionBase
    {
        /// <summary>
        /// Evalutates a linear point value.
        /// </summary>
        /// <param name="c">Set of coefficients</param>
        /// <param name="x">Input variables</param>
        /// <param name="functionResult">Returned sum value of your function</param>
        /// <param name="obj">?</param>
        public override void FunctionDelegate(double[] c, double[] x, ref double functionResult, object obj)
        {
            //y = mx + b
            functionResult = (c[0] * x[0]) + c[1];
        }

    }

}
