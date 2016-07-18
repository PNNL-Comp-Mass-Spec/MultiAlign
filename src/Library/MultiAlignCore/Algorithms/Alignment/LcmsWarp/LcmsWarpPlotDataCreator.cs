namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using System;
    using System.Collections.Generic;
    using MultiAlignCore.Algorithms.Statistics;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Data.Features;

    public class LcmsWarpPlotDataCreator
    {
        private const double MinScore = -100000;

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

        public static Dictionary<double, int> GetMassErrorHistogram(List<LcmsWarpFeatureMatch> featureMatches, double massBinSize)
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

        /// <summary>
        /// Create alignment score heatmap.
        /// The alignment score heatmap is a two dimensional matrix containing the best alignment for a
        /// particular mapping for alignee section -> (baseline section + baseline expansion factor)
        /// </summary>
        /// <param name="alignmentScoreMatrix"></param>
        /// <param name="standardize"></param>
        /// <param name="numAligneeSections"></param>
        /// <param name="numBaselineSections"></param>
        /// <param name="expansionFactor"></param>
        /// <returns></returns>
        public static double[,] GetAlignmentHeatMap(double[,,] alignmentScoreMatrix, bool standardize, int numAligneeSections, int numBaselineSections, int expansionFactor)
        {
            var alignmentHeatMap = new double[numAligneeSections, numBaselineSections];

            for (var aligneeSection = 0; aligneeSection < numAligneeSections; aligneeSection++)
            {
                for (var baselineSection = 0; baselineSection < numBaselineSections; baselineSection++)
                {
                    var maxScore = double.MinValue;
                    for (var baselineSectionWidth = 0; baselineSectionWidth < expansionFactor; baselineSectionWidth++)
                    {
                        if (baselineSection + baselineSectionWidth >= numBaselineSections)
                        {
                            continue;
                        }
                        if (alignmentScoreMatrix[aligneeSection, baselineSection, baselineSectionWidth] > maxScore)
                        {
                            maxScore = alignmentScoreMatrix[aligneeSection, baselineSection, baselineSectionWidth];
                        }
                    }
                    alignmentHeatMap[aligneeSection, baselineSection] = maxScore;
                }
            }

            if (standardize)
            {
                StandardizeSubsectionMatchScore(alignmentHeatMap, numAligneeSections, numBaselineSections);
            }

            return alignmentHeatMap;
        }

        /// <summary>
        /// Standardizes the match scores of the subsection
        /// </summary>
        /// <param name="subsectionMatchScores"></param>
        private static void StandardizeSubsectionMatchScore(double[,] subsectionMatchScores, int numAligneeSections, int numBaselineSections)
        {
            for (var aligneeSection = 0; aligneeSection < numAligneeSections; aligneeSection++)
            {
                double sumX = 0, sumXx = 0;
                var realMinScore = double.MaxValue;
                var numPoints = 0;
                for (var baselineSection = 0; baselineSection < numBaselineSections; baselineSection++)
                {
                    var score = subsectionMatchScores[aligneeSection, baselineSection];
                    // if the score is greater than _minScore, basically
                    if (Math.Abs(score - MinScore) > double.Epsilon)
                    {
                        realMinScore = Math.Min(realMinScore, score);
                        sumX += score;
                        sumXx += score * score;
                        numPoints++;
                    }
                }
                double var = 0;
                if (numPoints > 1)
                {
                    var = (sumXx - ((sumX * sumX) / numPoints)) / (numPoints - 1);
                }
                double stDev = 1;
                double avg = 0;
                if (numPoints >= 1)
                {
                    avg = sumX / numPoints;
                }
                if (Math.Abs(var) > double.Epsilon)
                {
                    stDev = Math.Sqrt(var);
                }

                for (var baselineSection = 0; baselineSection < numBaselineSections; baselineSection++)
                {
                    var score = subsectionMatchScores[aligneeSection, baselineSection];
                    if (Math.Abs(score - MinScore) < double.Epsilon)
                    {
                        score = realMinScore;
                    }
                    if (numPoints > 1)
                    {
                        subsectionMatchScores[aligneeSection, baselineSection] = ((score - avg) / stDev);
                    }
                    else
                    {
                        subsectionMatchScores[aligneeSection, baselineSection] = 0;
                    }
                }
            }
        }
    }
}
