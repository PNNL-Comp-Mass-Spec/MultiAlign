using System.Collections.Generic;
using FeatureAlignment.Data;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt
{
    public class LevenburgMarquadtSolver
    {
        public LevenburgMarquadtSolver()
        {
            DifferentialStep = 0.0001;
            Epsilon          = 0.000001;
        }

        public LevenburgMarquadtSolver(double differentialStep, double epsilon)
        {
            DifferentialStep = differentialStep;
            Epsilon = epsilon;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the amount to step when differentiating.
        /// </summary>
        public double DifferentialStep
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the basis function to call that the LM algorithm is solving for.
        /// </summary>
        public alglib.ndimensional_pfunc BasisFunction
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the tolerance to know when the Least Squares has completed.
        /// </summary>
        public double Epsilon
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Fits the X to Y for the given paired data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="coeffs"></param>
        /// <returns></returns>
        public SolverReport Solve(List<XYData> data, ref double[] coeffs)
        {
            var x = data.ConvertAll(u => u.X);
            var y = data.ConvertAll(u => u.X);

            return Solve(x, y, ref coeffs);
        }
        /// <summary>
        /// Least squares solver
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="coeffs">Guess at coefficients</param>
        /// <returns>True if solved, False if error</returns>
        public SolverReport Solve(List<double> x, List<double> y, ref double[] coeffs)
        {
            const double EPSF = 0;
            const int MAX_ITERATIONS = 400;

            var         N   = x.Count;
            var   xMatrix   = new double[N, 1];

            var         i = 0;
            foreach (var d in x)
            {
                xMatrix[i++, 0] = d;
            }

            alglib.lsfitcreatef(xMatrix,
                                y.ToArray(),
                                coeffs,
                                DifferentialStep,
                                out var state);

            // Note: lsfitsetcond has a different method signature in alglib v3.14.0
            //
            // As shown here, this is compatible with v3.10.0
            alglib.lsfitsetcond(state,
                                EPSF,
                                Epsilon,
                                MAX_ITERATIONS);

            alglib.lsfitfit(state,
                            BasisFunction,
                            null,
                            null);



            alglib.lsfitresults(state,
                                out var info,
                                out coeffs,
                                out var report);

            //Info    -   completion code:
            //        * -7    gradient verification failed.
            //                See LSFitSetGradientCheck() for more information.
            //        *  1    relative function improvement is no more than
            //                EpsF.
            //        *  2    relative step is no more than EpsX.
            //        *  4    gradient norm is no more than EpsG
            //        *  5    MaxIts steps was taken
            //        *  7    stopping conditions are too stringent,
            //                further improvement is impossible

            var converged = (info >= 1 && info < 5);

            // This is the flag in the ALGLIB library that says things were good for this kind of fit.
            var solverReport = new SolverReport(report, converged);

            return solverReport;
        }
    }

    /// <summary>
    /// Contains information about the Levenburg-Marquadt execution.
    /// </summary>
    public class SolverReport
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="report">Report from algorithm</param>
        /// <param name="didConverge"></param>
        public SolverReport(alglib.lsfitreport report, bool didConverge)
        {
            //see notes at bottom from Alglib website
            AverageError     = report.avgerror;
            DidConverge      = didConverge;
            IterationCount   = report.iterationscount;
            MaxError         = report.maxerror;
            PerPointNoise    = report.noise;
            RmsError         = report.rmserror;
            RSquared         = report.r2;
            WeightedRmsError = report.wrmserror;
        }
        /// <summary>
        /// Gets the average error.
        /// </summary>
        public double AverageError
        {
            get;
        }
        /// <summary>
        /// Gets the number of iterations the software took to converge.
        /// </summary>
        public int IterationCount
        {
            get;
        }
        /// <summary>
        /// Gets the maximum error
        /// </summary>
        public double MaxError
        {
            get;
        }
        /// <summary>
        /// Gets the RMS value
        /// </summary>
        public double RmsError
        {
            get;
        }
        /// <summary>
        /// Gets the weighted RMS value
        /// </summary>
        public double WeightedRmsError
        {
            get;
        }
        /// <summary>
        /// Gets the noise per point.
        /// </summary>
        public double[] PerPointNoise
        {
            get;
        }
        /// <summary>
        /// Gets the flag indicating whether the algorithm converged.
        /// </summary>
        public bool DidConverge
        {
            get;
        }
        /// <summary>
        /// Gets the R-squared value for the fit.
        /// </summary>
        public double RSquared
        {
            get;
        }

        /*
        Least squares fitting report. This structure contains informational fields
        which are set by fitting functions provided by this unit.

        Different functions initialize different sets of  fields,  so  you  should
        read documentation on specific function you used in order  to  know  which
        fields are initialized.

            TaskRCond       reciprocal of task's condition number
            IterationsCount number of internal iterations

            VarIdx          if user-supplied gradient contains errors  which  were
                            detected by nonlinear fitter, this  field  is  set  to
                            index  of  the  first  component  of gradient which is
                            suspected to be spoiled by bugs.

            RMSError        RMS error
            AvgError        average error
            AvgRelError     average relative error (for non-zero Y[I])
            MaxError        maximum error

            WRMSError       weighted RMS error

            CovPar          covariance matrix for parameters, filled by some solvers
            ErrPar          vector of errors in parameters, filled by some solvers
            ErrCurve        vector of fit errors -  variability  of  the  best-fit
                            curve, filled by some solvers.
            Noise           vector of per-point noise estimates, filled by
                            some solvers.
            R2              coefficient of determination (non-weighted, non-adjusted),
                            filled by some solvers.
        */
    }
}
