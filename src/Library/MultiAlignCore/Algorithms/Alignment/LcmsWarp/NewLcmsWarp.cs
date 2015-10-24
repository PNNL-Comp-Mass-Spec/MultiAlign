using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using MultiAlignCore.Data.Features;

    public class NewLcmsWarp
    {
        private LcmsWarpAlignmentOptions options;

        public NewLcmsWarp(LcmsWarpAlignmentOptions options)
        {
            
        }

        /// <summary>
        /// Goes through the matched features and determines the probability
        /// of each that the match is correct
        /// </summary>
        public void GetMatchProbabilities(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures, List<LcmsWarpFeatureMatch> matches)
        {
            // TODO: move this to the constructor:
            ////_subsectionMatchScores = new double[NumSections, NumBaselineSections, NumMatchesPerBaseline];
            ////for (var i = 0; i < NumSections; i++)
            ////{
            ////    for (var j = 0; j < NumBaselineSections; j++)
            ////    {
            ////        for (var k = 0; k < NumMatchesPerBaseline; k++)
            ////        {
            ////            _subsectionMatchScores[i, j, k] = MinScore;
            ////        }
            ////    }
            ////}

            int numBaselineSections = this.options.NumTimeSections * this.options.ContractionFactor;
            int numMatchesPerBaseline = this.options.ContractionFactor * this.options.ContractionFactor;

            // Calculate sections
            var aligneeSectionInfo = new LcmsWarpSectionInfo(options.NumTimeSections);
            aligneeSectionInfo.InitSections(aligneeFeatures);

            var baselineSectionInfo = new LcmsWarpSectionInfo(numBaselineSections);
            baselineSectionInfo.InitSections(baselineFeatures);

            // Sort matches into sections.
            for (int section = 0; section < this.options.NumTimeSections; section++)
            {
                var minNet = aligneeSectionInfo.GetSectionStartNet(section);
                var maxNet = aligneeSectionInfo.GetSectionEndNet(section);

                var sectionMatches = matches.Where(match => match.Net >= minNet && match.Net <= maxNet);

                // Compute section score.
                //ComputeSectionMatch(section, aligneeSectionInfo, baselineSectionInfo, uniqueFeatureToMatchMap.Values.ToList());
            }
        }

        //private void ComputeSectionMatch(int msSection, LcmsWarpSectionInfo aligneeSections, LcmsWarpSectionInfo baselineSections, List<LcmsWarpFeatureMatch> sectionMatches)
        //{
        //    var numMatchesPerBaseline = this.options.ContractionFactor * this.options.ContractionFactor;
        //    var numMatchesPerSection = baselineSections.NumSections * numMatchesPerBaseline;

        //    // Get unique features in the section

        //    for (var baselineSectionStart = 0; baselineSectionStart < baselineSections.NumSections; baselineSectionStart++)
        //    {
        //        var baselineStartNet = baselineSections.GetSectionStartNet(baselineSectionStart);
        //        var endSection = Math.Min(baselineSectionStart + numMatchesPerBaseline, baselineSections.NumSections);

        //        for (var baselineSectionEnd = baselineSectionStart; baselineSectionEnd < endSection; baselineSectionEnd++)
        //        {
        //            var baselineEndNet = baselineSections.GetSectionEndNet(baselineSectionEnd);

        //            for (var i = 0; i < sectionMatches.Count; i++)
        //            {
        //                var msFeatureIndex = _sectionUniqueFeatureIndices[i];
        //                _tempFeatureBestDelta[msFeatureIndex] = double.MaxValue;
        //                _tempFeatureBestIndex[msFeatureIndex] = -1;
        //            }

        //            // Now that we have msmsSection and matching msSection, transform the scan numbers to nets using a
        //            // transformation of the two sections, and use a temporary list to keep only the best match
        //            for (var i = 0; i < numMatchingFeatures; i++)
        //            {
        //                var match = sectionMatchingFeatures[i];
        //                var msFeatureIndex = match.FeatureIndex;
        //                var featureNet = match.Net;

        //                var transformNet = (featureNet - minNet) * (baselineEndNet - baselineStartNet);
        //                transformNet = transformNet / (maxNet - minNet) + baselineStartNet;

        //                var deltaMatch = transformNet - match.BaselineNet;
        //                if (!(Math.Abs(deltaMatch) < Math.Abs(_tempFeatureBestDelta[msFeatureIndex])))
        //                    continue;

        //                _tempFeatureBestDelta[msFeatureIndex] = deltaMatch;
        //                _tempFeatureBestIndex[msFeatureIndex] = match.BaselineFeatureIndex;
        //            }

        //            _subsectionMatchScores[msSection, baselineSectionStart, baselineSectionEnd - baselineSectionStart] = CurrentlyStoredSectionMatchScore(numUniqueFeatures);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Compute match scores for this section: log(P(match of ms section to MSMS section))
        ///// Does this within the Net Tolerance of the LCMSWarper
        ///// </summary>
        ///// <param name="numUniqueFeatures"></param>
        ///// <returns></returns>
        //private double CurrentlyStoredSectionMatchScore(int numUniqueFeatures)
        //{
        //    //Compute match scores for this section: log(P(match of ms section to MSMS section))
        //    double matchScore = 0;

        //    var lg2PiStdNetSqrd = Math.Log(2 * Math.PI * _netStd * _netStd);
        //    for (var i = 0; i < numUniqueFeatures; i++)
        //    {
        //        var msFeatureIndex = _sectionUniqueFeatureIndices[i];
        //        var featureMonoMass = _features[msFeatureIndex].MassMonoisotopic;
        //        var baselineFeatureMonoMass = _baselineFeatures[_tempFeatureBestIndex[msFeatureIndex]].MassMonoisotopic;

        //        var deltaNet = _tempFeatureBestDelta[msFeatureIndex];

        //        if (_useMass)
        //        {
        //            var massDelta = (featureMonoMass - baselineFeatureMonoMass) * 1000000 /
        //                            baselineFeatureMonoMass;
        //            var likelihood = GetMatchLikelihood(massDelta, deltaNet);
        //            matchScore += Math.Log(likelihood);
        //        }
        //        else
        //        {
        //            var calcVal = deltaNet;
        //            if (Math.Abs(deltaNet) > NetTolerance)
        //            {
        //                calcVal = NetTolerance;
        //            }
        //            matchScore -= 0.5 * (calcVal / _netStd) * (calcVal / _netStd);
        //            matchScore -= 0.5 * lg2PiStdNetSqrd;
        //        }
        //    }
        //    return matchScore;
        //}

        //private double GetFeatureMatchScore(double netStdDev)
        //{
            
        //}

        //private double GetMatchLikelihood(double massDelta, double massStdDev, double netDelta, double netStdDev)
        //{
        //    var massZ = massDelta / massStdDev;
        //    var netZ = netDelta / netStdDev;
        //    var normProb = Math.Exp(-0.5 * ((massZ * massZ) + (netZ * netZ))) / (2 * Math.PI * netStdDev * massStdDev);
        //    var likelihood = (normProb * _normalProb + ((1 - _normalProb) * _u));
        //    if (likelihood < MIN_MASS_NET_LIKELIHOOD)
        //    {
        //        likelihood = MIN_MASS_NET_LIKELIHOOD;
        //    }
        //    return likelihood;
        //}
    }
}
