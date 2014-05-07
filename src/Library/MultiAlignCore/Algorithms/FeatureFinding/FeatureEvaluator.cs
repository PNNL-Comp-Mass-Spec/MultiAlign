using PNNLOmics.Algorithms.Solvers;
using PNNLOmics.Algorithms.Solvers.LevenburgMarquadt;
using PNNLOmics.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Finds an XIC based on DeconTools profile data.
    /// </summary>
    public class FeatureEvaluator
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FeatureEvaluator()
        {
            BasisFunction   = BasisFunctionsEnum.Gaussian;
            IntegrationType = NumericalIntegrationEnum.Trapezoidal;
        }

        /// <summary>
        /// Gets or sets the basis function to use when fitting XIC profiles.
        /// </summary>
        public BasisFunctionsEnum BasisFunction
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of integration to perform.
        /// </summary>
        public NumericalIntegrationEnum IntegrationType
        {
            get;
            set;
        }
        
        /// <summary>
        /// Scores a feature and adjusts its abundance accordingly.
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="scan"></param>
        /// <returns></returns>
        public void ScoreFeature(UMCLight feature)
        {
            // Get the basis function of interest
            BasisFunctionBase basisFunction     = BasisFunctionFactory.BasisFunctionSelector(BasisFunction);
            NumericalIntegrationBase integrator = NumericalIntegrationFactory.CreateIntegrator(IntegrationType);


            // Evaluate every charge state XIC.
            foreach(int charge in feature.ChargeStateChromatograms.Keys)
            {
                Chromatogram gram   = feature.ChargeStateChromatograms[charge];
                int totalPoints     = gram.Points.Count;
                if (totalPoints > 1)
                {
                    // Convert the data types, not sure why this has to be done...
                    // Should just using the XYData points
                    List<double> x = new List<double>();
                    List<double> y = new List<double>();
                    foreach (var xyData in gram.Points)
                    {
                        x.Add(xyData.X);
                        y.Add(xyData.Y);
                    }

                    // First solve for the function
                    double [] coefficients          = basisFunction.Coefficients; 
                    LevenburgMarquadtSolver solver  = new LevenburgMarquadtSolver();
                    SolverReport report             = solver.Solve(x, y, ref coefficients);
                    gram.FitPoints                  = new List<XYData>();
                
                    foreach (XYData datum in gram.Points)
                    {
                        double yValue = basisFunction.Evaluate(coefficients, datum.X);
                        gram.FitPoints.Add(new XYData(datum.X, yValue));
                    }
                    gram.FitReport  = report;

                    int totalSamples = 4 * Math.Abs(gram.EndScan - gram.StartScan);

                    // Then integrate the function
                    // Let's integrate with 4x the number of scans
                    gram.Area       = integrator.Integrate( basisFunction,
                                                            coefficients, 
                                                            Convert.ToDouble(gram.StartScan), 
                                                            Convert.ToDouble(gram.EndScan),
                                                            totalSamples);
                }
            }
            // Then calculate all of the fits for each 
            foreach (int charge in feature.IsotopeChromatograms.Keys)
            {
                foreach (Chromatogram gram in feature.IsotopeChromatograms[charge])
                {
                    int totalPoints = gram.Points.Count;
                    if (totalPoints > 1)
                    {
                        // Convert the data types, not sure why this has to be done...
                        // Should just using the XYData points
                        List<double> x = new List<double>();
                        List<double> y = new List<double>();
                        foreach (var xyData in gram.Points)
                        {
                            x.Add(xyData.X);
                            y.Add(xyData.Y);
                        }

                        // First solve for the function
                        double[] coefficients = basisFunction.Coefficients;
                        LevenburgMarquadtSolver solver = new LevenburgMarquadtSolver();
                        SolverReport report = solver.Solve(x, y, ref coefficients);
                        gram.FitPoints = new List<XYData>();

                        foreach (XYData datum in gram.Points)
                        {
                            double yValue = basisFunction.Evaluate(coefficients, datum.X);
                            gram.FitPoints.Add(new XYData(datum.X, yValue));
                        }
                        gram.FitReport = report;

                        int totalSamples = 4 * Math.Abs(gram.EndScan - gram.StartScan);

                        // Then integrate the function
                        // Let's integrate with 4x the number of scans
                        gram.Area = integrator.Integrate(basisFunction,
                                                                coefficients,
                                                                Convert.ToDouble(gram.StartScan),
                                                                Convert.ToDouble(gram.EndScan),
                                                                totalSamples);
                    }
                }
            }
        }               
    }    
}
