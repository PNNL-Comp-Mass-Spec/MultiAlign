namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using System;
    using System.Collections.Generic;

    using PNNLOmics.Utilities;

    /// <summary>
    /// This class contains statistical information based on the multivariate
    /// distribution of NET and mass.
    /// </summary>
    public class LcmsWarpStatistics
    {
        /// <summary>
        /// The number of matches required for statistically significant results.
        /// </summary>
        private const int RequiredMatches = 6;

        /// <summary>
        /// The mass standard deviation.
        /// </summary>
        private double massStdDev;

        /// <summary>
        /// The NET standard deviation.
        /// </summary>
        private double netStdDev;

        /// <summary>
        /// The probability density for false hits.
        /// </summary>
        public double FalseHitProbDensity { get; set; }

        /// <summary>
        /// Gets or sets the mean of the values in the mass dimension.
        /// </summary>
        public double MassMean { get; set; }

        /// <summary>
        /// Gets or sets the mass standard deviation.
        /// </summary>
        public double MassStdDev
        {
            get { return this.massStdDev; }
            set
            {
                this.massStdDev = value;
                this.Log2PiMassStdDevSq = Math.Log(2 * Math.PI * this.MassVariance);
            }
        }

        /// <summary>
        /// Gets the variance in the mass dimension.
        /// </summary>
        [IO.Options.IgnoreOptionProperty]
        public double MassVariance
        {
            get { return this.MassStdDev * this.MassStdDev; }
        }

        /// <summary>
        /// Gets the calculations of log[2pi * (StdDev)^2]
        /// </summary>
        public double Log2PiMassStdDevSq { get; private set; }

        /// <summary>
        /// Gets or sets the mean in the NET dimension.
        /// </summary>
        public double NetMean { get; set; }

        /// <summary>
        /// Gets or sets the NET standard deviation.
        /// </summary>
        public double NetStdDev
        {
            get { return this.netStdDev; }
            set
            {
                this.netStdDev = value;
                this.Log2PiNetStdDevSq = Math.Log(2 * Math.PI * this.NetVariance);
            }
        }

        /// <summary>
        /// Gets the variance in the NET dimension.
        /// </summary>
        [IO.Options.IgnoreOptionProperty]
        public double NetVariance
        {
            get { return this.NetStdDev * this.NetStdDev; }
        }

        /// <summary>
        /// Gets the calculations of log[2pi * (StdDev)^2]
        /// </summary>
        public double Log2PiNetStdDevSq { get; private set; }

        /// <summary>
        /// Gets or sets the probability given a multivariate normal distribution.
        /// </summary>
        public double NormalProbability { get; set; }

        /// <summary>
        /// Gets or sets the slope of the line fitted to the NET feature matches.
        /// </summary>
        public double NetSlope { get; set; }

        /// <summary>
        /// Gets or sets the intercept of the line fitted to the NET feature matches.
        /// </summary>
        public double NetIntercept { get; set; }

        /// <summary>
        /// Gets or sets the R-Squared value (correlation coefficient) for the line fitted to the
        /// NET feature matches.
        /// </summary>
        public double NetRSquared { get; set; }

        /// <summary>
        /// Gets or sets the mass kurtosis.
        /// </summary>
        [IO.Options.IgnoreOptionProperty]
        public double MassKurtosis
        {
            get { return Math.Pow(this.MassMean, 4) / Math.Pow(this.MassStdDev, 4); }
        }

        /// <summary>
        /// Gets or sets the NET kurtosis.
        /// </summary>
        [IO.Options.IgnoreOptionProperty]
        public double NetKurtosis
        {
            get { return Math.Pow(this.NetMean, 4) / Math.Pow(this.NetStdDev, 4); }
        }

        /// <summary>
        /// Calculates the Standard deviations of the matches.
        /// Note: method requires more than 6 matches to produce meaningful results.
        /// </summary>
        public static LcmsWarpStatistics CalculateAndGetStatistics(List<LcmsWarpFeatureMatch> matches)
        {
            if (matches.Count <= RequiredMatches)
            {
                throw new ArgumentException(string.Format("This requires at least {0} matches to produce meaningful results", RequiredMatches));
            }

            // Calculate NET delta (difference between alignee and baseline NET).
            var massDeltas = new List<double>(matches.Count);
            var netDeltas = new List<double>(matches.Count);
            foreach (var match in matches)
            {
                var baselineFeature = match.BaselineFeature;
                massDeltas.Add(((baselineFeature.MassMonoisotopic - match.AligneeFeature.MassMonoisotopic) * 1000000) /
                                       match.AligneeFeature.MassMonoisotopic);
                netDeltas.Add(match.BaselineNet - match.Net);
            }

            // Two dimensional expectation maximization
            double normalProbability, falseHitProbDensity, muMass, muNet, massStdDev, netStdDev;
            MathUtilities.TwoDem(
                                 massDeltas,
                                 netDeltas,
                                 out normalProbability,
                                 out falseHitProbDensity,
                                 out muMass,
                                 out muNet,
                                 out massStdDev,
                                 out netStdDev) ;

            var statistics = new LcmsWarpStatistics
            {
                MassStdDev = massStdDev,
                NetStdDev = netStdDev,
                FalseHitProbDensity = falseHitProbDensity,
                NormalProbability = normalProbability
            };

            return statistics;
        }
    }
}
