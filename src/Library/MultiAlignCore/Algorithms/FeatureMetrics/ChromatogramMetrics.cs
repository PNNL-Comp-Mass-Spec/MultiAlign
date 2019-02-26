using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.FeatureMetrics
{
    /// <summary>
    /// Scores chromatograms based on various relationships between charge state and isotopic distribution.
    /// </summary>
    public class ChromatogramMetrics
    {
        public void FitChromatograms(Chromatogram profile,
                                     BasisFunctionBase basisFunction)
        {
            var solver = new LevenburgMarquadtSolver();
            solver.BasisFunction = basisFunction.FunctionDelegate;

            var coeffs = basisFunction.Coefficients;
            solver.Solve(profile.Points, ref coeffs);

            var minScan      = profile.Points.Min(x => x.X);
            var maxScan      = profile.Points.Max(x => x.X);

            var scanRange    = Math.Abs(maxScan - minScan);
            var deltaScan    = scanRange / (scanRange * 4);
            var scan         = minScan;

            var fitPoints = new List<XYData>();
            while (scan <= maxScan)
            {
                var y = basisFunction.Evaluate(coeffs, scan);
                fitPoints.Add(new XYData(scan, y));
                scan += deltaScan;
            }
            profile.FitCoefficients = coeffs;
            profile.FitPoints       = fitPoints;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="basisFunction"></param>
        public void FitChromatograms(UMCLight feature,
                                    BasisFunctionBase basisFunction)
        {
            foreach (var charge in feature.ChargeStateChromatograms.Keys)
            {
                var gram = feature.ChargeStateChromatograms[charge];
                FitChromatograms(gram, basisFunction);
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="features"></param>
        /// <param name="basisFunction"></param>
        public void FitChromatograms(List<UMCLight> features, BasisFunctionBase basisFunction)
        {
            features.ForEach(x => FitChromatograms(x, basisFunction));
        }
        /// <summary>
        /// Scores two chromatograms using their basis functions.
        /// </summary>
        /// <param name="profileA">chromatogram A</param>
        /// <param name="profileB">chromatogram B</param>
        /// <param name="basisFunction">Function used to interpolate</param>
        /// <param name="intensityProfile"></param>
        /// <returns>R-squared value for a given linear regression between intensity matrix</returns>
        public double ScoreChromatogramIntensity(Chromatogram profileA,
                                                 Chromatogram profileB,
                                                 BasisFunctionBase basisFunction,
                                                 ref List<XYData>  intensityProfile)
        {
            if (intensityProfile == null)
                throw new ArgumentNullException("intensityProfile");

            var minScan = profileA.FitPoints.Min(x => x.X);
            var maxScan = profileA.FitPoints.Max(x => x.X);

            minScan = Math.Min(minScan, profileB.FitPoints.Min(x => x.X));
            maxScan = Math.Max(maxScan, profileB.FitPoints.Max(x => x.X));

            var deltaScan    = Math.Abs(maxScan - minScan) / 100;
            var scan         = minScan;

            var pairs = new List<XYData>();

            while (scan <= maxScan)
            {
                var x = basisFunction.Evaluate(profileA.FitCoefficients, scan);
                var y = basisFunction.Evaluate(profileB.FitCoefficients, scan);

                pairs.Add(new XYData(x, y));
                scan += deltaScan;
            }

            var linearRegression  = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.Linear);
            var solver      = new LevenburgMarquadtSolver {BasisFunction = linearRegression.FunctionDelegate};

            var coeffs                    = linearRegression.Coefficients;
            var report                 = solver.Solve(pairs, ref coeffs);

            return report.RSquared;
        }
    }
}
