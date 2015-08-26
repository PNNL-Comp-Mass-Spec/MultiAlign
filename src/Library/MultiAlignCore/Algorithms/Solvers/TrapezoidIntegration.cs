using System;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt;

namespace MultiAlignCore.Algorithms.Solvers
{

    /// <summary>
    /// Class for using the trapezoid rule to integrate a basis function.
    /// </summary>
    public class TrapezoidIntegration : NumericalIntegrationBase
    {
        /// <summary>
        /// Numerical integration of a basis function.
        /// </summary>
        /// <param name="basis">Function to evaluate</param>
        /// <param name="coefficients">Coefficients of a fit function</param>
        /// <param name="start">Start of the function to evaluate</param>
        /// <param name="stop">Stop of the function to evaluate</param>
        /// <param name="numberOfSamples">Total number of samples to take</param>
        /// <returns>Normalized area between start and stop of the basis function</returns>
        public override double Integrate(BasisFunctionBase basis, double[] coefficients, double start, double stop, int numberOfSamples)
        {
            double sum = 0;

            // Calculate the width an spacing of each of the trapezoids.
            var width = Math.Abs(stop - start);
            var delta = width / Convert.ToDouble(numberOfSamples);
            var x = start;

            // Get the start of the function
            sum = basis.Evaluate(coefficients, x);

            // We already evaluated the first point, now for each element within
            for (var i = 1; i < numberOfSamples; i++)
            {
                x += delta;

                var y = basis.Evaluate(coefficients, x);
                sum += (y * 2);                
            }

            // Make sure we get the end of the function
            sum += basis.Evaluate(coefficients, stop);

            // Now we normalize the definite integral for the number of trapezoids that we constructed.
            var weight = width / (2 * Convert.ToDouble(numberOfSamples));
            sum *= weight;

            return sum;
        }
    }
}
