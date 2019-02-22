#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Solvers;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

#endregion

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
            BasisFunction = BasisFunctionsEnum.Gaussian;
            IntegrationType = NumericalIntegrationEnum.Trapezoidal;
        }

        /// <summary>
        /// Gets or sets the basis function to use when fitting XIC profiles.
        /// </summary>
        public BasisFunctionsEnum BasisFunction { get; set; }

        /// <summary>
        /// Gets or sets the type of integration to perform.
        /// </summary>
        public NumericalIntegrationEnum IntegrationType { get; set; }

        /// <summary>
        /// Scores a feature and adjusts its abundance accordingly.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public void ScoreFeature(UMCLight feature)
        {
            // Get the basis function of interest
            var basisFunction = BasisFunctionFactory.BasisFunctionSelector(BasisFunction);
            var integrator = NumericalIntegrationFactory.CreateIntegrator(IntegrationType);


            // Evaluate every charge state XIC.
            foreach (var charge in feature.ChargeStateChromatograms.Keys)
            {
                var gram = feature.ChargeStateChromatograms[charge];
                var totalPoints = gram.Points.Count;
                if (totalPoints > 1)
                {
                    // Convert the data types, not sure why this has to be done...
                    // Should just using the XYData points
                    var x = new List<double>();
                    var y = new List<double>();
                    foreach (var xyData in gram.Points)
                    {
                        x.Add(xyData.X);
                        y.Add(xyData.Y);
                    }

                    // First solve for the function
                    var coefficients = basisFunction.Coefficients;
                    var solver = new LevenburgMarquadtSolver();
                    var report = solver.Solve(x, y, ref coefficients);
                    gram.FitPoints = new List<XYData>();

                    foreach (var datum in gram.Points)
                    {
                        var yValue = basisFunction.Evaluate(coefficients, datum.X);
                        gram.FitPoints.Add(new XYData(datum.X, yValue));
                    }

                    var totalSamples = 4*Math.Abs(gram.EndScan - gram.StartScan);

                    // Then integrate the function
                    // Let's integrate with 4x the number of scans
                    gram.Area = integrator.Integrate(basisFunction,
                        coefficients,
                        Convert.ToDouble(gram.StartScan),
                        Convert.ToDouble(gram.EndScan),
                        totalSamples);
                }
            }
            // Then calculate all of the fits for each
            foreach (var charge in feature.IsotopeChromatograms.Keys)
            {
                foreach (var gram in feature.IsotopeChromatograms[charge])
                {
                    var totalPoints = gram.Points.Count;
                    if (totalPoints > 1)
                    {
                        // Convert the data types, not sure why this has to be done...
                        // Should just using the XYData points
                        var x = new List<double>();
                        var y = new List<double>();
                        foreach (var xyData in gram.Points)
                        {
                            x.Add(xyData.X);
                            y.Add(xyData.Y);
                        }

                        // First solve for the function
                        var coefficients = basisFunction.Coefficients;
                        var solver = new LevenburgMarquadtSolver();
                        solver.Solve(x, y, ref coefficients);
                        gram.FitPoints = new List<XYData>();

                        foreach (var datum in gram.Points)
                        {
                            var yValue = basisFunction.Evaluate(coefficients, datum.X);
                            gram.FitPoints.Add(new XYData(datum.X, yValue));
                        }

                        var totalSamples = 4*Math.Abs(gram.EndScan - gram.StartScan);

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