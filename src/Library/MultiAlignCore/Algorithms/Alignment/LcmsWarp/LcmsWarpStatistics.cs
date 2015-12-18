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
        /// The multivariate mean of both NET and mass.
        /// </summary>
        public double Mu { get; set; }

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
        public double MassVariance
        {
            get { return this.MassStdDev * this.MassStdDev; }
        }

        /// <summary>
        /// Gets the calculations of log[2pi * (StdDev)^2]
        /// </summary>
        public double Log2PiMassStdDevSq { get; private set; }

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

            // TODO: Calculating all of this in one method is nasty and difficult to test. Break this method into several methods.
            double normalProb, u, muMass, muNet, massStd, netStd;
            MathUtilities.TwoDem(massDeltas, netDeltas, out normalProb, out u,
                out muMass, out muNet, out massStd, out netStd);

            var statistics = new LcmsWarpStatistics
            {
                MassStdDev = massStd,
                NetStdDev = netStd,
                Mu = u,
                NormalProbability = normalProb
            };

            return statistics;
        }
    }
}
