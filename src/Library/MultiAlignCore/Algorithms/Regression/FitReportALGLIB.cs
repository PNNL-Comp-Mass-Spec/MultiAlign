using System;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt;

namespace MultiAlignCore.Algorithms.Regression
{
    /// <summary>
    /// fit report that includes information from ALGLIB
    /// </summary>
    public class FitReportALGLIB: FitReport
    {
        double AverageError{ get; set; }
        int IterationCount{ get; set; }
        Double MaxError{ get; set; }
        Double[] PerPointNoise{ get; set; }
        Double RmsError { get; set; }
        private double WeightedRmsError { get; set; }

        public FitReportALGLIB(SolverReport report, bool didConverge)
        {
            AverageError = report.AverageError;
            DidConverge = didConverge;
            IterationCount = report.IterationCount;
            MaxError = report.MaxError;
            PerPointNoise = report.PerPointNoise;
            RmsError = report.RmsError;
            RSquared = report.RSquared;
            WeightedRmsError = report.WeightedRmsError;
        }
    }
}
