using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using PNNLOmics.Utilities;

    public class LcmsWarpStatistics
    {
        private const int RequiredMatches = 6;

        public double Mu { get; set; }

        private double massStdDev;
        public double MassStdDev
        {
            get { return this.massStdDev; }
            set
            {
                this.massStdDev = value;
                this.Log2PiMassStdDevSq = Math.Log(2 * Math.PI * this.MassVariance);
            }
        }

        public double MassVariance
        {
            get { return this.MassStdDev * this.MassStdDev; }
        }

        /// <summary>
        /// Gets the calculations of log[2pi * (StdDev)^2]
        /// </summary>
        public double Log2PiMassStdDevSq { get; private set; }

        private double netStdDev;
        public double NetStdDev
        {
            get { return this.netStdDev; }
            set
            {
                this.netStdDev = value;
                this.Log2PiNetStdDevSq = Math.Log(2 * Math.PI * this.NetVariance);
            }
        }

        public double NetVariance
        {
            get { return this.NetStdDev * this.NetStdDev; }
        }

        /// <summary>
        /// Gets the calculations of log[2pi * (StdDev)^2]
        /// </summary>
        public double Log2PiNetStdDevSq { get; private set; }

        public double NormalProbability { get; set; }

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

            var massDeltas = new List<double>(matches.Count);
            var netDeltas = new List<double>(matches.Count);
            foreach (var match in matches)
            {
                var baselineFeature = match.BaselineFeature;
                massDeltas.Add(((baselineFeature.MassMonoisotopic - match.AligneeFeature.MassMonoisotopic) * 1000000) /
                                       match.AligneeFeature.MassMonoisotopic);
                netDeltas.Add(match.BaselineNet - match.Net);
            }

            double normalProb, u, muMass, muNet, massStd, netStd;

            MathUtilities.TwoDem(massDeltas, netDeltas, out normalProb, out u,
                out muMass, out muNet, out massStd, out netStd);

            return new LcmsWarpStatistics
            {
                MassStdDev = massStd,
                NetStdDev = netStd,
                Mu = u,
                NormalProbability = normalProb
            };
        }
    }
}
