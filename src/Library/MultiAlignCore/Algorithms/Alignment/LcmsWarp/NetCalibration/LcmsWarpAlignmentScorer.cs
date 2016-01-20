namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp.NetCalibration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MultiAlignCore.Data;

    public class LcmsWarpAlignmentScorer
    {
        private const double MinMassNetLikelihood = 1e-4;

        /// <summary>
        /// The LcmsWarp parameters.
        /// </summary>
        private readonly LcmsWarpAlignmentOptions options;

        /// <summary>
        /// Multivariate statistical information needed for match score calculation with mass and NET.
        /// </summary>
        private readonly LcmsWarpStatistics statistics;

        /// <summary>
        /// A value that indicates whether mass difference should be accounted for
        /// when calculating a section match score.
        /// </summary>
        private readonly bool includeMassInMatchScore;

        public LcmsWarpAlignmentScorer(LcmsWarpAlignmentOptions options, bool includeMassInMatchScore, LcmsWarpStatistics statistics)
        {
            this.options = options;
            this.includeMassInMatchScore = includeMassInMatchScore;
            this.statistics = statistics;
        }

        /// <summary>
        /// Gets a matrix containing the match score between each alignee section,
        /// baseline section, and baseline section width.
        /// </summary>
        public double[,,] MatchScoreMatrix { get; private set; }

        /// <summary>
        /// Gets the matrix containing scores for every possible alignment between alignee section,
        /// baseline section, and baseline section width.
        /// </summary>
        public double[,,] AlignmentScoreMatrix { get; private set; }

        /// <summary>
        /// Goes through the matched features and determines the probability
        /// of each that the match is correct
        /// </summary>
        public LcmsWarpNetAlignmentFunction GetAlignment(LcmsWarpSectionInfo aligneeSections, LcmsWarpSectionInfo baselineSections, List<LcmsWarpFeatureMatch> matches)
        {
            var matchScores =  this.ComputeSectionMatchScoreMatrix(aligneeSections, baselineSections, matches);
            var alignmentScores = this.ComputeAlignmentScoreMatrix(matchScores);

            throw new NotImplementedException();
        }

        public double[,,] ComputeSectionMatchScoreMatrix(LcmsWarpSectionInfo aligneeSections, LcmsWarpSectionInfo baselineSections, List<LcmsWarpFeatureMatch> matches)
        {
            // Initialize score matrix.
            double[,,] subsectionMatchScores;
            this.InitializeMatchScoreMatrix(out subsectionMatchScores);

            for (int aligneeSection = 0; aligneeSection < aligneeSections.NumSections; aligneeSection++)
            {
                var aligneeMinNet = aligneeSections.GetSectionStartNet(aligneeSection);
                var aligneeMaxNet = aligneeSections.GetSectionEndNet(aligneeSection);

                // Get matches in this alignee section.
                var sectionMatches = matches.Where(match => match.Net >= aligneeMinNet && match.Net <= aligneeMaxNet);

                // Group matches by alignee feature to get only matches with unique alignee features.
                var uniqueSectionMatches = sectionMatches.GroupBy(match => match.AligneeFeature.Id).ToList();

                for (var baselineSectionStart = 0; baselineSectionStart < baselineSections.NumSections; baselineSectionStart++)
                {
                    var baselineStartNet = baselineSections.GetSectionStartNet(baselineSectionStart);
                    var endSection = Math.Min(baselineSectionStart + this.options.MaxExpansionWidth, baselineSections.NumSections);

                    // Each alignee section can expand to (ContractionFactor)^2 baseline sections, so we want to compute
                    // a match score for each possible size from 1 to (ContractionFactor)^2
                    for (var baselineSectionEnd = baselineSectionStart; baselineSectionEnd < endSection; baselineSectionEnd++)
                    {
                        var baselineEndNet = baselineSections.GetSectionEndNet(baselineSectionEnd);

                        // For each unique alignee feature match group, find the minimum NET difference between the match NET and the transformed NET.
                        var deltas = uniqueSectionMatches.Select(
                                                                 group =>
                                                                    group.Min(match => this.GetDelta(match.Net,
                                                                                                     baselineStartNet,
                                                                                                     baselineEndNet,
                                                                                                     aligneeSections)))
                                                         .ToList();

                        var selectedMatches = uniqueSectionMatches.Select(group => group.FirstOrDefault()).ToList();
                        subsectionMatchScores[aligneeSection, baselineSectionStart, baselineSectionEnd - baselineSectionStart] = this.ComputeSectionMatchScore(selectedMatches, deltas);
                    }
                }
            }

            return subsectionMatchScores;
        }

        public double[,,] ComputeAlignmentScoreMatrix(double[,,] sectionMatchScores)
        {
            double[,,] alignmentScores;
            this.InitializeMatchScoreMatrix(out alignmentScores);

            var bestPreviousIndex = new Index3D[this.options.NumTimeSections, this.options.NumBaselineSections, this.options.MaxExpansionWidth];

            // Initialize scores to - inf, best previous index to -1
            for (var i = 0; i < this.options.NumTimeSections; i++)
            {
                for (var j = 0; j < this.options.NumBaselineSections; j++)
                {
                    for (var k = 0; k < this.options.MaxExpansionWidth; k++)
                    {
                        bestPreviousIndex[i, j, k] = new Index3D();
                    }
                }
            }

            var unmatchedScore = -0.5 * this.statistics.Log2PiNetStdDevSq;
            if (this.options.NetTolerance < 3 * this.statistics.NetStdDev)
            {
                unmatchedScore -= (0.5 * 9.0);
            }
            else
            {
                unmatchedScore -= (0.5 * (this.options.NetTolerance * this.options.NetTolerance) / (this.statistics.NetStdDev * this.statistics.NetStdDev));
            }
            if (this.includeMassInMatchScore)
            {
                // Assumes that for the unmatched, the masses were also off at mass tolerance, so use the same threshold from NET
                unmatchedScore *= 2;
            }

            for (var baselineSection = 0; baselineSection < this.options.NumBaselineSections; baselineSection++)
            {
                // Assume everything that was matched was past 3 standard devs in net.
                for (var sectionWidth = 0; sectionWidth < this.options.MaxExpansionWidth; sectionWidth++)
                {
                    // no need to multiply with msSection because its 0
                    alignmentScores[0, baselineSection, sectionWidth] = 0;
                }
            }

            var numUnmatchedMsFeatures = 0;
            for (var section = 0; section < this.options.NumTimeSections; section++)
            {
                for (var sectionWidth = 0; sectionWidth < this.options.MaxExpansionWidth; sectionWidth++)
                {
                    alignmentScores[section, 0, sectionWidth] = sectionMatchScores[section, 0, sectionWidth] +
                                                                   unmatchedScore * numUnmatchedMsFeatures;
                }

                // TODO: Make numFeaturesInSections available to this method.
                ////numUnmatchedMsFeatures += _numFeaturesInSections[section];
            }

            for (var section = 1; section < this.options.NumTimeSections; section++)
            {
                for (var baselineSection = 1; baselineSection < this.options.NumBaselineSections; baselineSection++)
                {
                    for (var sectionWidth = 0; sectionWidth < this.options.MaxExpansionWidth; sectionWidth++)
                    {
                        var currentBestScore = double.MinValue;
                        var bestPreviousAlignmentIndex = new Index3D();

                        for (var previousBaselineSection = baselineSection - 1;
                            previousBaselineSection >= baselineSection - this.options.MaxExpansionWidth - this.options.MaxTimeDistortion;
                            previousBaselineSection--)
                        {
                            if (previousBaselineSection < 0)
                            {
                                break;
                            }
                            var maxWidth = baselineSection - previousBaselineSection;
                            if (maxWidth > this.options.MaxExpansionWidth)
                            {
                                maxWidth = this.options.MaxExpansionWidth;
                            }
                            var previousBaselineSectionWidth = maxWidth;

                            if (!(alignmentScores[section - 1, previousBaselineSection, previousBaselineSectionWidth - 1] > currentBestScore))
                                continue;

                            currentBestScore = alignmentScores[section - 1, previousBaselineSection, previousBaselineSectionWidth - 1];
                            bestPreviousAlignmentIndex.Set(section - 1, previousBaselineSection, previousBaselineSectionWidth - 1);
                        }
                        if (Math.Abs(currentBestScore - double.MinValue) > double.Epsilon)
                        {
                            alignmentScores[section, baselineSection, sectionWidth] = currentBestScore + sectionMatchScores[section, baselineSection, sectionWidth];
                            bestPreviousIndex[section, baselineSection, sectionWidth].Set(bestPreviousAlignmentIndex);
                        }
                        else
                        {
                            alignmentScores[section, baselineSection, sectionWidth] = double.MinValue;
                        }
                    }
                }
            }

            return alignmentScores;
        }

        /// <summary>
        /// When the alignment matrix was created, the ideal match between alignee section -> (baseline section, baseline section width)
        /// was found. This function steps through the matrix of best indices (bestPreviousIndices) and creates an LcmsWarpAlignmentMatch
        /// for each mapping of alignee section to baseline section.
        /// </summary>
        /// <param name="bestPreviousIndices">
        /// Each element of this matrix stores the best index of the best matching of the previous alignee section.
        /// </param>
        /// <returns>List of matches mapping alignee section -> baseline sections.</returns>
        public List<LcmsWarpAlignmentMatch> CalculateAlignmentFunction(double[,,] alignmentScores,
                                                                       Index3D[,,] bestPreviousIndices,
                                                                       LcmsWarpSectionInfo aligneeSections,
                                                                       LcmsWarpSectionInfo baselineSections)
        {
            // For the last alignee section, find the index of the best baseline section and baseline section width.
            var lastAligneeSection = this.options.NumTimeSections - 1;
            var bestScore = double.MinValue;
            var bestAlignmentIndex = new Index3D();
            for (var baselineSection = 0; baselineSection < this.options.NumBaselineSections; baselineSection++)
            {
                // Everything past this section would have remained unmatched.
                for (var sectionWidth = 0; sectionWidth < this.options.MaxExpansionWidth; sectionWidth++)
                {
                    var alignmentScore = alignmentScores[lastAligneeSection, baselineSection, sectionWidth];
                    if (alignmentScore > bestScore)
                    {
                        bestScore = alignmentScore;
                        bestAlignmentIndex.Set(lastAligneeSection, baselineSection, sectionWidth);
                    }
                }
            }

            // Step backwards through the alignment matrix to find the best baseline section and baseline section width for each alignee section.
            var alignmentFunction = new List<LcmsWarpAlignmentMatch>();
            for (var alignIndex = bestAlignmentIndex; alignIndex.IsValid(bestPreviousIndices); alignIndex = bestPreviousIndices[alignIndex.X, alignIndex.Y, alignIndex.Z])
            {
                var ascore = alignmentScores[alignIndex.X, alignIndex.Y, alignIndex.Z];
                var mscore = alignmentScores[alignIndex.X, alignIndex.Y, alignIndex.Z];
                alignmentFunction.Add(this.CreateMatch(alignIndex, ascore, mscore, aligneeSections, baselineSections));
            }

            alignmentFunction.Sort();
            return alignmentFunction;
        }

        /// <summary>
        /// Creates an LcmsWarpAlignment match that stores a match between an alignee section and a baseline section.
        /// </summary>
        /// <param name="index">
        /// The index of the match in the alignment matrix.
        ///     X = alignee section.
        ///     Y = start of base line section that the alignee matches to.
        ///     Z = width of baseline section that the alignee matches to.
        /// </param>
        /// <param name="alignmentScore">The alignment score of the match.</param>
        /// <param name="matchScore">The match score for the alignee to baseline section match.</param>
        /// <param name="aligneeSections">The alignee section information.</param>
        /// <param name="baselineSections">The baseline section information.</param>
        /// <returns>Object that stores a matching between an alignee section and baseline section.</returns>
        public LcmsWarpAlignmentMatch CreateMatch(Index3D index,
                                                  double alignmentScore,
                                                  double matchScore,
                                                  LcmsWarpSectionInfo aligneeSections,
                                                  LcmsWarpSectionInfo baselineSections)
        {
            var section = index.X;
            var baselineSection = index.Y;
            var baselineSectionWidth = index.Z;
            return new LcmsWarpAlignmentMatch
            {
                AlignmentScore = alignmentScore,
                MatchScore = matchScore,
                AligneeSectionStart = section,
                AligneeSectionEnd = section + 1,
                AligneeNetStart = aligneeSections.GetSectionStartNet(section),
                AligneeNetEnd = aligneeSections.GetSectionEndNet(section),
                BaselineSectionStart = baselineSection,
                BaselineSectionEnd = baselineSection + baselineSectionWidth + 1,
                BaselineNetStart = baselineSections.GetSectionStartNet(section),
                BaselineNetEnd = baselineSections.GetSectionEndNet(section + baselineSectionWidth)
            };
        }

        /// <summary>
        /// Transform the NET of a match into the baseline section and then find the difference between
        /// the feature and transformed NET.
        /// </summary>
        /// <param name="net">The NET to transform.</param>
        /// <param name="baselineStartNet">Baseline section start NET.</param>
        /// <param name="baselineEndNet">Baseline section end NET.</param>
        /// <param name="aligneeSections">The alignee section info.</param>
        /// <returns>The absolute value of the difference betwen the match NET and the transformed NET.</returns>
        private double GetDelta(
            double net,
            double baselineStartNet,
            double baselineEndNet,
            LcmsWarpSectionInfo aligneeSections)
        {
            var transformNet = (net - aligneeSections.MinNet) * (baselineEndNet - baselineStartNet);
            transformNet = transformNet / (aligneeSections.MaxNet - aligneeSections.MinNet) + baselineStartNet;
            return Math.Abs(net - transformNet);
        }

        /// <summary>
        /// Compute match scores for this section: log(P(match of alignee section to baseline section))
        /// Does this within the Net Tolerance of the LCMSWarper
        /// </summary>
        /// <param name="deltaNets"></param>
        /// <returns></returns>
        private double ComputeSectionMatchScore(List<LcmsWarpFeatureMatch> uniqueSectionMatches, List<double> deltaNets)
        {
            // Compute match scores for this section: log(P(match of ms section to MSMS section))
            double matchScore = 0;
            for (var i = 0; i < uniqueSectionMatches.Count; i++)
            {
                var match = uniqueSectionMatches[i];
                var deltaNet = deltaNets[i];
                matchScore += this.GetFeatureMatchScore(match, deltaNet);
            }
            return matchScore;
        }

        /// <summary>
        /// Calculate the match score for a alignee, baseline feature match. 
        /// </summary>
        /// <param name="match">The match to compute the score for.</param>
        /// <param name="deltaNet">
        /// The difference in NET between the alignee feature's NET and the alignee feature's NET
        /// warped to a baseline section.
        /// </param>
        /// <param name="netStdDev">The standard deviation of NETs for all alignee features.</param>
        /// <returns>The match score. </returns>
        private double GetFeatureMatchScore(LcmsWarpFeatureMatch match, double deltaNet)
        {
            var featureMonoMass = match.AligneeFeature.MassMonoisotopic;
            var baselineFeatureMonoMass = match.BaselineFeature.MassMonoisotopic;

            double matchScore = 0.0;

            if (this.includeMassInMatchScore)
            {
                var massDelta = (featureMonoMass - baselineFeatureMonoMass) * 1000000 / baselineFeatureMonoMass;
                var likelihood = this.GetMatchLikelihood(massDelta, deltaNet);
                matchScore += Math.Log(likelihood);
            }
            else
            {
                var calcVal = deltaNet;
                if (Math.Abs(deltaNet) > this.options.NetTolerance)
                {
                    calcVal = this.options.NetTolerance;
                }
                matchScore -= 0.5 * (calcVal / this.statistics.NetStdDev) * (calcVal / this.statistics.NetStdDev);
                matchScore -= 0.5 * this.statistics.Log2PiNetStdDevSq;
            }

            return matchScore;
        }

        /// <summary>
        /// Use the previously calculated statistics to calculate the likelihood score for the
        /// given mass and NET deltas.
        /// </summary>
        /// <param name="massDelta">The difference between the baseline feature mass and the alignee feature mass.</param>
        /// <param name="netDelta">The difference between the baseline feature NET and the alignee feature NET.</param>
        /// <returns>The likelihood score.</returns>
        private double GetMatchLikelihood(double massDelta, double netDelta)
        {
            var massZ = massDelta / this.statistics.MassStdDev;
            var netZ = netDelta / this.statistics.NetStdDev;
            var normProb = Math.Exp(-0.5 * ((massZ * massZ) + (netZ * netZ))) / (2 * Math.PI * this.statistics.NetStdDev * this.statistics.MassStdDev);
            var likelihood = (normProb * this.statistics.NormalProbability + ((1 - this.statistics.NormalProbability) * this.statistics.FalseHitProbDensity));
            if (likelihood < MinMassNetLikelihood)
            {
                likelihood = MinMassNetLikelihood;
            }
            return likelihood;
        }

        /// <summary>
        /// Initialize default match scores to negative infinity.
        /// </summary>
        /// <param name="subsectionMatchScores">The match score matrix to initialize.</param>
        private void InitializeMatchScoreMatrix(out double[,,] subsectionMatchScores)
        {
            subsectionMatchScores = new double[this.options.NumTimeSections, this.options.NumBaselineSections, this.options.MaxExpansionWidth];
            for (var i = 0; i < this.options.NumTimeSections; i++)
            {
                for (var j = 0; j < this.options.NumBaselineSections; j++)
                {
                    for (var k = 0; k < this.options.MaxExpansionWidth; k++)
                    {
                        subsectionMatchScores[i, j, k] = double.NegativeInfinity;
                    }
                }
            }
        }
    }
}
