namespace MultiAlignCore.Algorithms.Regression
{
    public abstract class FitReport
    {
        /// <summary>
        /// did the solver converge on a solution
        /// </summary>
        public bool DidConverge { get; set; }

        /// <summary>
        /// coefficient of determination.  How well does the model fit the experimental data
        /// </summary>
        public double RSquared { get; set; }
    }
}
