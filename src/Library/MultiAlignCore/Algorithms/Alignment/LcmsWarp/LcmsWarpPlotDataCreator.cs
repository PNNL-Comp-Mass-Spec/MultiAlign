using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using MultiAlignCore.Algorithms.Statistics;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Data.Features;

    public class LcmsWarpPlotDataCreator
    {
        /// <summary>
        /// Create histogram for normalized elution time error.
        /// </summary>
        /// <param name="featureMatches"></param>
        /// <param name="netBinSize"></param>
        /// <returns></returns>
        public static Dictionary<double, int> GetNetErrorHistogram(List<LcmsWarpFeatureMatch> featureMatches, double netBinSize)
        {
            var netErrors = new List<double>(featureMatches.Count);

            //var minNetVal = double.MaxValue;
            //var maxNetVal = double.MinValue;

            foreach (var match in featureMatches)
            {
                netErrors.Add(match.NetError);

                //minNetVal = Math.Min(minNetVal, match.NetError);
                //maxNetVal = Math.Min(maxNetVal, match.NetError);
            }

            return Histogram.CreateHistogram(netErrors, netBinSize);

            // TODO: Change to use MathNet.Numerics.Statistics.Histogram; but, must calculate number of bins/buckets first.
            //var numNetBins = Math.Max((int)Math.Floor((maxNetVal - minNetVal) / netBinSize), 1);

            //return new MathNet.Numerics.Statistics.Histogram(netErrors, numNetBins);
        }

        public static Dictionary<double, int> GetDriftErrorHistogram(List<LcmsWarpFeatureMatch> featureMatches, double driftBinSize)
        {
            var driftErrors = new List<double>(featureMatches.Count);

            //var minDriftVal = double.MaxValue;
            //var maxDriftVal = double.MinValue;

            foreach (var match in featureMatches)
            {
                driftErrors.Add(match.DriftError);

                //minDriftVal = Math.Min(minDriftVal, match.DriftError);
                //maxDriftVal = Math.Min(maxDriftVal, match.DriftError);
            }

            return Histogram.CreateHistogram(driftErrors, driftBinSize);

            // TODO: Change to use MathNet.Numerics.Statistics.Histogram; but, must calculate number of bins/buckets first.
            //var numDriftBins = Math.Max((int)Math.Floor((maxDriftVal - minDriftVal) / driftBinSize), 1);

            //return new MathNet.Numerics.Statistics.Histogram(driftErrors, numDriftBins);
        }

        public Dictionary<double, int> GetMassErrorHistogram(List<LcmsWarpFeatureMatch> featureMatches, double massBinSize)
        {
            var massErrors = new List<double>(featureMatches.Count);

            //var minMassVal = double.MaxValue;
            //var maxMassVal = double.MinValue;

            foreach (var match in featureMatches)
            {
                massErrors.Add(match.PpmMassError);

                //minMassVal = Math.Min(minMassVal, match.PpmMassError);
                //maxMassVal = Math.Min(maxMassVal, match.PpmMassError);
            }

            return Histogram.CreateHistogram(massErrors, massBinSize);

            // TODO: Change to use MathNet.Numerics.Statistics.Histogram; but, must calculate number of bins/buckets first.
            //var numMassBins = Math.Max((int)Math.Floor((maxMassVal - minMassVal) / massBinSize), 1);

            //return new MathNet.Numerics.Statistics.Histogram(massErrors, numMassBins);
        }
    }
}
