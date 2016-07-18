
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt;

namespace MultiAlignCore.Algorithms.Solvers
{

    /// <summary>
    /// Integration technique
    /// </summary>
    public abstract class NumericalIntegrationBase
    {
        /// <summary>
        /// Integratesa function based on the start and stop points, and the total number of samples to take.
        /// </summary>
        /// <param name="basis">Function to evaluate</param>
        /// <param name="coefficients">Coefficients of a fit function</param>
        /// <param name="start">Start of the function to evaluate</param>
        /// <param name="stop">Stop of the function to evaluate</param>
        /// <param name="numberOfSamples">Total number of samples to take</param>
        /// <returns>Normalized area between start and stop of the basis function</returns>
        public abstract double Integrate(BasisFunctionBase basis, double[] coefficients, double start, double stop, int numberOfSamples);
    }
}
