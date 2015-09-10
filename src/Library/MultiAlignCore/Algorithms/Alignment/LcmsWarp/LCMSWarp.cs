using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Algorithms.Statistics;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using PNNLOmics.Utilities;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Object which performs the LCMS Warping functionality
    /// </summary>
    public sealed class LcmsWarp
    {
        #region Private values
        private readonly List<double> m_tempFeatureBestDelta;
        private readonly List<int> m_tempFeatureBestIndex;

        private readonly List<int> m_sectionUniqueFeatureIndices;
        private readonly List<int> m_numFeaturesInSections;
        private readonly List<int> m_numFeaturesInBaselineSections;

        //Interpolation m_interpolation;
        private double[] m_alignmentScore;
        private int[] m_bestPreviousIndex;

        private double m_netStd;

        private const double MIN_MASS_NET_LIKELIHOOD = 1e-4;
        private const int REQUIRED_MATCHES = 6;

        private bool m_useMass;

        // Used to control the granularity of the MSMS section size when comparing against MS Sections.
        // The number of sectiosn in the MSMS will be # of sectiosn in MS * m_maxSectionDistortion.
        // Thus each section of the MS can be compared to MSMS section wich are 1/m_maxSectionDistortion to
        // m_maxSectionDistortion times the ms section size of the chromatographic run.

        private readonly double m_minScore;

        // Mass window around which the mass tolerance is applied

        private double m_massStd;

        private double m_normalProb;
        private double m_u;
        private double m_muMass;
        private double m_muNet;
        private readonly List<LcmsWarpAlignmentMatch> m_alignmentFunc;
        private readonly List<UMCLight> m_features;
        private readonly List<UMCLight> m_baselineFeatures;
        
        
        public List<LcmsWarpFeatureMatch> m_featureMatches;

        private readonly List<double> m_subsectionMatchScores;
        // Slope and intercept calculated using likelihood score in m_subsectionMatchScores.
        // Range of scans used is the range which covers the start scan of the Q2 and the end
        // scan of the Q3 of the nets of the matched features.
        #endregion

        #region ProgressReporting

        /// <summary>
        /// Percent complete, value between 0 and 100
        /// </summary>
        public double PercentComplete { get; private set; }

        public event EventHandler<ProgressNotifierArgs> Progress;

        private void OnProgress(string message, double percentComplete)
        {
            PercentComplete = percentComplete;

            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message, percentComplete));
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the MinNet for the LCMS Baseline features
        /// </summary>
        public double MinBaselineNet { get; private set; }

        /// <summary>
        /// Gets or sets the MaxNet for the LCMS Baseline features
        /// </summary>
        public double MaxBaselineNet { get; private set; }

        /// <summary>
        /// Gets or sets the MinNet for the LCMS alignee features
        /// </summary>
        public double MinNet { get; private set; }

        /// <summary>
        /// Gets or sets the MaxNet for the LCMS alignee features
        /// </summary>
        public double MaxNet { get; private set; }

        /// <summary>
        /// Gets or sets the LCMS Mass Tolerance (in ppm)
        /// </summary>
        public double MassTolerance { get; set; }

        /// <summary>
        /// Gets or sets the LCMS Net Tolerance
        /// </summary>
        public double NetTolerance { private get; set; }
        
        /// <summary>
        /// Gets or sets the  Net Intercept of what would be a linear regression of the sets
        /// </summary>
        public double NetIntercept { get; private set; }

        /// <summary>
        /// Gets or sets the  Net Slope of what would be a linear regression of the sets
        /// </summary>
        public double NetSlope { get; private set; }

        /// <summary>
        /// Gets or sets the  R-Squared of the linear regression of the sets
        /// </summary>
        public double NetLinearRsq { get; private set; }

        /// <summary>
        /// Gets or sets the  Mass Calibration Window of the LCMS warper
        /// </summary>
        public double MassCalibrationWindow { get; set; }

        /// <summary>
        /// Gets or sets the the LCMSWarp calibration type
        /// </summary>
        public LcmsWarpCalibrationType CalibrationType { private get; set; }
        
        

        /// <summary>
        /// AutoProperty for the number of sections that the alignee features is split into
        /// </summary>
        public int NumSections { private get; set; }

        /// <summary>
        /// AutoProperty for the number of sections that the baseline is split into
        /// </summary>
        public int NumBaselineSections { get; set; }

        /// <summary>
        /// AutoProperty for the number of matches in an alignee section
        /// </summary>
        public int NumMatchesPerSection { private get; set; }

        /// <summary>
        /// AutoProperty for the number of matches in a baseline section
        /// </summary>
        public int NumMatchesPerBaseline { get; set; }

        /// <summary>
        /// Property to get the number of feature matches found
        /// </summary>
        public int NumCandidateMatches
        {
            get { return m_featureMatches.Count; }
        }


        /// <summary>
        /// AutoProperty for the Max Time distortion of the warper
        /// </summary>
        public int MaxJump { private get; set; }
        
        /// <summary>
        /// Whether or not promiscuous matches are kept in the scoring function for alignment
        /// to a MT database. 
        /// </summary>
        /// <remarks>
        /// This should be true for MTDBs because they do not have split UMCs
        /// It should be false for MS to MS alignment, otherwise all split UMCs will match to the first instance
        /// </remarks>
        public bool KeepPromiscuousMatches { get; set; }
        
        /// <summary>
        /// AutoProperty for the number of promiscuous matches above which to filter UMC matches
        /// </summary>
        public int MaxPromiscuousUmcMatches { get; set; }

        /// <summary>
        /// AutoProperty for the number of "y" bins during regression
        /// </summary>
        public int MassCalNumDeltaBins { get; set; }

        /// <summary>
        /// AutoProperty for the number of "x" bins during regression
        /// </summary>
        public int MassCalNumSlices { get; set; }

        /// <summary>
        /// AutoProperty for the Max calibration jump during regression
        /// </summary>
        public int MassCalNumJump { get; set; }

        /// <summary>
        /// AutoProperty to hold the MzCalibration object
        /// </summary>
        public LcmsWarpCombinedRegression MzRecalibration { get; private set; }

        /// <summary>
        /// AutoProperty to hold the NetCalibration object
        /// </summary>
        public LcmsWarpCombinedRegression NetRecalibration { get; private set; }

        #endregion

        private static int ByMass(UMCLight left, UMCLight right)
        {
            if (left != null) return right == null ? 1 : left.MassMonoisotopic.CompareTo(right.MassMonoisotopic);
            if (right == null)
            {
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// Public Constructor, doesn't take arguements, initializes memory space and sets it
        /// to default values
        /// </summary>
        public LcmsWarp()
        {
            m_tempFeatureBestDelta = new List<double>();
            m_tempFeatureBestIndex = new List<int>();

            m_sectionUniqueFeatureIndices = new List<int>();
            m_numFeaturesInSections = new List<int>();
            m_numFeaturesInBaselineSections = new List<int>();
            
            MzRecalibration = new LcmsWarpCombinedRegression();
            NetRecalibration = new LcmsWarpCombinedRegression();

            m_alignmentFunc = new List<LcmsWarpAlignmentMatch>();
            m_features = new List<UMCLight>();
            m_baselineFeatures = new List<UMCLight>();
            m_featureMatches = new List<LcmsWarpFeatureMatch>();
            m_subsectionMatchScores = new List<double>();

            m_useMass = false;
            MassCalibrationWindow = 50;
            MassTolerance = 20;		// ppm
            NetTolerance = 0.02;
            
            m_netStd = 0.007;
            m_alignmentScore = null;
            m_bestPreviousIndex = null;
            MaxJump = 10;
            m_massStd = 20;
            
            KeepPromiscuousMatches = false;
            MaxPromiscuousUmcMatches = 3;

            m_bestPreviousIndex = null;

            CalibrationType = LcmsWarpCalibrationType.MzRegression;
            MassCalNumDeltaBins = 100;
            MassCalNumSlices = 12;
            MassCalNumJump = 50;

            const int ztolerance = 3;
            const LcmsWarpRegressionType regType = LcmsWarpRegressionType.Central;

            MzRecalibration.SetCentralRegressionOptions(MassCalNumSlices, MassCalNumDeltaBins, MassCalNumJump,
                                                          ztolerance, regType);
            NetRecalibration.SetCentralRegressionOptions(MassCalNumSlices, MassCalNumDeltaBins, MassCalNumJump,
                                                          ztolerance, regType);

            const double outlierZScore  = 2.5;
            const int numKnots          = 12;
            MzRecalibration.SetLsqOptions(numKnots, outlierZScore);
            NetRecalibration.SetLsqOptions(numKnots, outlierZScore);

            m_minScore = -100000;
            m_muMass = 0;
            m_muNet = 0;
            m_normalProb = 0.3;
        }

        /// <summary>
        /// Method which calculates the Net slope, intercept and r squared as if
        /// a linear regression was performed
        /// </summary>
        private void CalculateNetSlopeAndIntercept()
        {
            var startNets = m_featureMatches.Select(match => match.Net).ToList();
            startNets.Sort();

            var numPoints = startNets.Count;
            if (numPoints == 0)
            {
                NetSlope = 0;
                NetIntercept = 0;
                return;
            }
            var startSection = Convert.ToInt32(((startNets[numPoints / 4] - MinNet) * NumSections) / (MaxNet - MinNet));
            var endSection = Convert.ToInt32(((startNets[(3 * numPoints) / 4] - MinNet) * NumSections) / (MaxNet - MinNet));

            if (startSection >= NumSections)
            {
                startSection = NumSections - 1;
            }
            if (endSection >= NumSections)
            {
                endSection = NumSections - 1;
            }

            double sumY = 0;
            double sumX = 0;
            double sumXy = 0;
            double sumXx = 0;
            double sumYy = 0;

            var numSumPoints = 0;
            for (var section = startSection; section <= endSection; section++)
            {
                var maxScore = double.MinValue;
                double y = 0;

                for (var baselineSection = 0; baselineSection < NumBaselineSections; baselineSection++)
                {
                    for (var sectionWidth = 0; sectionWidth < NumMatchesPerBaseline; sectionWidth++)
                    {
                        var alignmentIndex = (section * NumMatchesPerSection) + (baselineSection * NumMatchesPerBaseline) + sectionWidth;

                        if (!(m_subsectionMatchScores[alignmentIndex] > maxScore))
                            continue;

                        maxScore = m_subsectionMatchScores[alignmentIndex];
                        y = baselineSection;
                    }
                }

                var net = ((section * (MaxNet - MinNet)) / NumSections) + MinNet;
                var alignedNet = ((y * (MaxBaselineNet - MinBaselineNet)) / NumBaselineSections) + MinBaselineNet;

                sumX = sumX + net;
                sumY = sumY + alignedNet;
                sumXy = sumXy + (net * alignedNet);
                sumXx = sumXx + (net * net);
                sumYy = sumYy + (alignedNet * alignedNet);
                numSumPoints++;
            }

            NetSlope = ((numSumPoints * sumXy) - (sumX * sumY)) / ((numSumPoints * sumXx) - (sumX * sumX));
            NetIntercept = (sumY - NetSlope * sumX) / numSumPoints;

            var temp = ((numSumPoints * sumXy) - (sumX * sumY)) / Math.Sqrt(((numSumPoints * sumXx) - (sumX * sumX)) * ((numSumPoints * sumYy) - (sumY * sumY)));
            NetLinearRsq = temp * temp;
        }

        /// <summary>
        /// Compute match scores for this section: log(P(match of ms section to MSMS section))
        /// Does this within the Net Tolerance of the LCMSWarper
        /// </summary>
        /// <param name="numUniqueFeatures"></param>
        /// <returns></returns>
        private double CurrentlyStoredSectionMatchScore(int numUniqueFeatures)   
        {
            //Compute match scores for this section: log(P(match of ms section to MSMS section))
            double matchScore = 0;

            var lg2PiStdNetSqrd = Math.Log(2 * Math.PI * m_netStd * m_netStd);
            for (var i = 0; i < numUniqueFeatures; i++)
            {
                var msFeatureIndex = m_sectionUniqueFeatureIndices[i];
                var msmsFeatureIndex = m_tempFeatureBestIndex[msFeatureIndex];
                var feature = m_features[msFeatureIndex];
                var baselineFeature = m_baselineFeatures[msmsFeatureIndex];

                var deltaNet = m_tempFeatureBestDelta[msFeatureIndex];

                if (m_useMass)
                {
                    var massDelta = (feature.MassMonoisotopic - baselineFeature.MassMonoisotopic) * 1000000 / baselineFeature.MassMonoisotopic;
                    var likelihood = GetMatchLikelihood(massDelta, deltaNet);
                    matchScore += Math.Log(likelihood);
                }
                else
                {
                    if (Math.Abs(deltaNet) > NetTolerance)
                    {
                        matchScore = matchScore - 0.5 * (NetTolerance / m_netStd) * (NetTolerance / m_netStd);
                        matchScore = matchScore - 0.5 * lg2PiStdNetSqrd;
                    }
                    else
                    {
                        matchScore = matchScore - 0.5 * (deltaNet / m_netStd) * (deltaNet / m_netStd);
                        matchScore = matchScore - 0.5 * lg2PiStdNetSqrd;
                    }
                }
            }
            return matchScore;
        }

        private double GetMatchLikelihood(double massDelta, double netDelta)
        {
            var massZ = massDelta / m_massStd;
            var netZ = netDelta / m_netStd;
            var normProb = Math.Exp(-0.5 * ((massZ * massZ) + (netZ * netZ))) / (2 * Math.PI * m_netStd * m_massStd);
            var likelihood = (normProb * m_normalProb + ((1 - m_normalProb) * m_u));
            if (likelihood < MIN_MASS_NET_LIKELIHOOD)
            {
                likelihood = MIN_MASS_NET_LIKELIHOOD;
            }
            return likelihood;
        }
        
        /// <summary>
        /// Method to grab the Mass and NET statistical data from the LCMS Warper
        /// </summary>
        /// <param name="massStd"></param>
        /// <param name="netStd"></param>
        /// <param name="massMu"></param>
        /// <param name="netMu"></param>
        public void GetStatistics(out double massStd, out double netStd, out double massMu, out double netMu)
        {
            massStd = m_massStd;
            netStd  = m_netStd;
            massMu  = m_muMass;
            netMu   = m_muNet;
        }

        /// <summary>
        /// Method to, given a mz and a net, get the ppm shift based on the regression method
        /// for the LCMS warper
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="net"></param>
        /// <returns></returns>
        private double GetPpmShift(double mz, double net)
        {
            double ppmShift = 0;
            switch (CalibrationType)
            {
                case LcmsWarpCalibrationType.MzRegression:
                    ppmShift = MzRecalibration.GetPredictedValue(mz);
                    break;
                case LcmsWarpCalibrationType.ScanRegression:
                    ppmShift = NetRecalibration.GetPredictedValue(net);
                    break;
                case LcmsWarpCalibrationType.Both:
                    ppmShift = MzRecalibration.GetPredictedValue(mz);
                    ppmShift = ppmShift + NetRecalibration.GetPredictedValue(net);
                    break;
            }
            return ppmShift;
        }

        /// <summary>
        /// Updates the lists passed in with the residual data from the Warper
        /// </summary>
        /// <param name="net"></param>
        /// <param name="mz"></param>
        /// <param name="linearNet"></param>
        /// <param name="customNet"></param>
        /// <param name="linearCustomNet"></param>
        /// <param name="massError"></param>
        /// <param name="massErrorCorrected"></param>
        public void GetResiduals(ref List<double> net, ref List<double> mz, ref List<double> linearNet, ref List<double> customNet,
                                 ref List<double> linearCustomNet, ref List<double> massError, ref List<double> massErrorCorrected)
        {
            var count = m_featureMatches.Count;
            net.Capacity = count;
            mz.Capacity = count;
            linearNet.Capacity = count;
            customNet.Capacity = count;
            linearCustomNet.Capacity = count;
            massError.Capacity = count;
            massErrorCorrected.Capacity = count;

            
            foreach (var match in m_featureMatches)
            {
                var feature = m_features[match.FeatureIndex];
                var predictedLinear = (NetSlope * match.Net2) + NetIntercept;
                var ppmMassError = match.PpmMassError;
                var scanNumber = match.Net;

                // NET
                net.Add(scanNumber);
                linearNet.Add(feature.NetAligned - predictedLinear);
                customNet.Add(match.NetError);
                linearCustomNet.Add(feature.NetAligned - predictedLinear);

                var ppmShift = 0.0;
                if (m_useMass)
                {
                    ppmShift = GetPpmShift(feature.Mz, scanNumber);
                }
                mz.Add(feature.Mz);
                massError.Add(ppmMassError);

                massErrorCorrected.Add(ppmMassError - ppmShift);
            }
        }

        /// <summary>
        /// Updates the Alignee and Reference net lists with the data from the alignment
        /// function determined by the warper with the Net start data
        /// </summary>
        /// <param name="aligneeNet"></param>
        /// <param name="referenceNet"></param>
        public void AlignmentFunction(ref List<double> aligneeNet, ref List<double> referenceNet)
        {
            aligneeNet.Clear();
            referenceNet.Clear();
            var numPieces = m_alignmentFunc.Count;
            for (var pieceNum = 0; pieceNum < numPieces; pieceNum++)
            {              
                aligneeNet.Add(m_alignmentFunc[pieceNum].NetStart);
                referenceNet.Add(m_alignmentFunc[pieceNum].NetStart2);
            }
        }

        /// <summary>
        /// Determines the transformed NETs for the LCMSWarp function
        /// </summary>
        public void GetTransformedNets()
        {
            int featureIndex;
            var numFeatures = m_features.Count;
            var alignmentFuncLength = m_alignmentFunc.Count;
            var dicSectionToIndex = new Dictionary<int, int>();

            for (var i = 0; i < m_alignmentFunc.Count; i++)
            {
                dicSectionToIndex.Add(m_alignmentFunc[i].SectionStart, i);
            }

            for (featureIndex = 0; featureIndex < numFeatures; featureIndex++)
            {
                var feature = m_features[featureIndex];
                double netStart;
                double netEndBaseline;
                double netEnd;
                double netStartBaseline;
                if (feature.Net < m_alignmentFunc[0].NetStart)
                {
                    netStart = m_alignmentFunc[0].NetStart;
                    netStartBaseline = m_alignmentFunc[0].NetStart2;
                    netEnd = m_alignmentFunc[0].NetEnd;
                    netEndBaseline = m_alignmentFunc[0].NetEnd2;

                    var msNetTransformed = ((feature.Net - netStart) * (netEndBaseline - netStartBaseline)) / (netEnd - netStart) + netStartBaseline;
                    m_features[featureIndex].NetAligned = msNetTransformed;
                    continue;
                }
                double netTransformed;
                if (feature.Net > m_alignmentFunc[alignmentFuncLength - 1].NetEnd)
                {
                    netStart = m_alignmentFunc[alignmentFuncLength - 1].NetStart;
                    netStartBaseline = m_alignmentFunc[alignmentFuncLength - 1].NetStart2;
                    netEnd = m_alignmentFunc[alignmentFuncLength - 1].NetEnd;
                    netEndBaseline = m_alignmentFunc[alignmentFuncLength - 1].NetEnd2;

                    netTransformed = ((feature.Net - netStart) * (netEndBaseline - netStartBaseline)) / (netEnd - netStart) + netStartBaseline;
                    m_features[featureIndex].NetAligned = netTransformed;
                    continue;
                }

                var msSection1 = Convert.ToInt32(((feature.Net - MinNet) * NumSections) / (MaxNet - MinNet));
                if (msSection1 >= NumSections)
                {
                    msSection1 = NumSections - 1;
                }

                var msSectionIndex = dicSectionToIndex[msSection1];

                netStart = m_alignmentFunc[msSectionIndex].NetStart;
                netEnd = m_alignmentFunc[msSectionIndex].NetEnd;

                netStartBaseline = m_alignmentFunc[msSectionIndex].NetStart2;
                netEndBaseline = m_alignmentFunc[msSectionIndex].NetEnd2;

                netTransformed = ((feature.Net - netStart) * (netEndBaseline - netStartBaseline)) / (netEnd - netStart) + netStartBaseline;
                m_features[featureIndex].NetAligned = netTransformed;
            }
        }

        /// <summary>
        /// Initially, clears any residual feature matches to ensure no carryover of prior runs,
        /// then goes through and calculates the match score for each feature with relation to
        /// the baselines, holding onto the "best match" for each one
        /// </summary>
        public void CalculateAlignmentMatches()
        {
            m_features.Sort(ByMass);
            m_baselineFeatures.Sort(ByMass);

            var featureIndex = 0;
            var baselineFeatureIndex = 0;
            var numFeatures = m_features.Count;
            var numBaselineFeatures = m_baselineFeatures.Count;
            
            m_featureMatches.Clear();

            var minMatchScore = -0.5 * (MassTolerance * MassTolerance) / (m_massStd * m_massStd);
            minMatchScore -= 0.5 * (NetTolerance * NetTolerance) / (m_netStd * m_netStd);

            while (featureIndex < numFeatures)
            {
                var feature = m_features[featureIndex];

                var massTolerance = feature.MassMonoisotopic * MassTolerance / 1000000;

                if (baselineFeatureIndex == numBaselineFeatures)
                {
                    baselineFeatureIndex = numBaselineFeatures - 1;
                }

                while (baselineFeatureIndex >= 0 && m_baselineFeatures[baselineFeatureIndex].MassMonoisotopic > feature.MassMonoisotopic - massTolerance)
                {
                    baselineFeatureIndex--;
                }
                baselineFeatureIndex++;

                LcmsWarpFeatureMatch bestMatchFeature = null;
                var bestMatchScore = minMatchScore;
                while (baselineFeatureIndex < numBaselineFeatures && m_baselineFeatures[baselineFeatureIndex].MassMonoisotopic < feature.MassMonoisotopic + massTolerance)
                {
                    if (m_baselineFeatures[baselineFeatureIndex].MassMonoisotopic > feature.MassMonoisotopic - massTolerance)
                    {
                        //Calculate the mass and net errors
                        var netDiff         = m_baselineFeatures[baselineFeatureIndex].Net - feature.NetAligned;
                        var baselineDrift   = m_baselineFeatures[baselineFeatureIndex].DriftTime;
                        var driftDiff       = baselineDrift - feature.DriftTime;
                        var massDiff        = (m_baselineFeatures[baselineFeatureIndex].MassMonoisotopic - feature.MassMonoisotopic) * 1000000.0 / feature.MassMonoisotopic;

                        //Calculate the match score
                        var matchScore  = -0.5 * (netDiff * netDiff) / (m_netStd * m_netStd);
                        matchScore      = matchScore - 0.5 * (massDiff * massDiff) / (m_massStd * m_massStd);

                        //If the match score is greater than the best match score, update the holding item
                        if (matchScore > bestMatchScore)
                        {                            
                            bestMatchScore = matchScore;
                            bestMatchFeature = new LcmsWarpFeatureMatch
                            {
                                FeatureIndex = featureIndex,
                                FeatureIndex2 = baselineFeatureIndex,
                                Net = feature.Net,
                                NetError = netDiff,
                                PpmMassError = massDiff,
                                DriftError = driftDiff,
                                Net2 = m_baselineFeatures[baselineFeatureIndex].Net
                            };
                        }
                    }
                    baselineFeatureIndex++;
                }

                //If we found a match, add it to the list of matches
                if (bestMatchFeature != null)
                {
                    m_featureMatches.Add(bestMatchFeature);                    
                }
                featureIndex++;
            }


            CalculateNetSlopeAndIntercept();


        }

        /// <summary>
        /// Initializes the features in the LCMSWarp object to the list of UMCLights
        /// </summary>
        /// <param name="features"></param>
        public void SetFeatures(ref List<UMCLight> features)
        {
            m_features.Clear();
            foreach (var feature in features)
            {
                m_features.Add(feature);
            }
        }

        /// <summary>
        /// Updates the data passed in with the appropriate calibrated mass, aligned NET and drift times
        /// </summary>
        /// <param name="umcIndices"></param>
        /// <param name="umcCalibratedMass"></param>
        /// <param name="umcAlignedNets"></param>
        /// <param name="alignedDriftTimes"></param>
        public void GetFeatureCalibratedMassesAndAlignedNets(ref List<int> umcIndices, ref List<double> umcCalibratedMass,
                                                             ref List<double> umcAlignedNets, ref List<double> alignedDriftTimes)
        {
            foreach (var feature in m_features)
            {
                umcIndices.Add(feature.Id);
                umcCalibratedMass.Add(feature.MassMonoisotopicAligned);
                umcAlignedNets.Add(feature.NetAligned);
                alignedDriftTimes.Add(feature.DriftTime);
            }
        }

        /// <summary>
        /// Updates the data passed in with the appropriate calibrated mass, aligned NET, drift times,
        /// aligned scans based on the min and max scan numbers
        /// </summary>
        /// <param name="umcIndices"></param>
        /// <param name="umcCalibratedMasses"></param>
        /// <param name="umcAlignedNets"></param>
        /// <param name="umcAlignedScans"></param>
        /// <param name="umcDriftTimes"></param>
        /// <param name="minScan"></param>
        /// <param name="maxScan"></param>
        public void GetFeatureCalibratedMassesAndAlignedNets(ref List<int> umcIndices, ref List<double> umcCalibratedMasses,
                                                             ref List<double> umcAlignedNets, ref List<int> umcAlignedScans,
                                                             ref List<double> umcDriftTimes, int minScan, int maxScan)
        {
            var numFeatures = m_features.Count;
            var sortedFeatures = m_features.OrderBy(x => x.Id).ToList();
            for (var featureNum = 0; featureNum < numFeatures; featureNum++)
            {
                var feature = sortedFeatures[featureNum];
                umcIndices.Add(feature.Id);
                umcCalibratedMasses.Add(feature.MassMonoisotopicAligned);
                umcAlignedNets.Add(feature.NetAligned);
                umcAlignedScans.Add(minScan + (int)(feature.NetAligned * (maxScan - minScan)));
                umcDriftTimes.Add(feature.DriftTime);
            }
        }

        /// <summary>
        /// Initializes the reference features to the features entered in
        /// </summary>
        /// <param name="features"></param>
        public void SetReferenceFeatures(ref List<UMCLight> features)
        {
            m_baselineFeatures.Clear();
            foreach (var feature in features)
            {
                m_baselineFeatures.Add(feature);
            }
        }



        /// <summary>
        /// Function generates candidate matches between m_Features and m_baselineFeatures
        /// It does so by finding all pairs of MassTimeFeature that match within a provided
        /// mass tolerance window
        /// Elution time will be considered later
        /// </summary>
        public void GenerateCandidateMatches()
        {
            if (m_features.Count == 0)
            {
                return;
            }

            m_sectionUniqueFeatureIndices.Clear();
            m_numFeaturesInSections.Clear();
            m_numFeaturesInBaselineSections.Clear();
            m_alignmentFunc.Clear();
            m_featureMatches.Clear();
            m_subsectionMatchScores.Clear();
            m_tempFeatureBestDelta.Clear();
            m_tempFeatureBestIndex .Clear();


            m_features.Sort(ByMass);
            m_baselineFeatures.Sort(ByMass);

            // Go through each MassTimeFeature and see if the next baseline MassTimeFeature matches it
            var featureIndex = 0;
            var baselineFeatureIndex = 0;
            var numFeatures = m_features.Count;
            var numBaselineFeatures = m_baselineFeatures.Count;

            if (numBaselineFeatures <= 0)
            {
                // No baseline to match it to.
                return;
            }

            
            while (featureIndex < numFeatures)
            {
                var feature = m_features[featureIndex];
                var massToleranceDa = feature.MassMonoisotopic * MassTolerance/1000000;

				// Backtrack baselineFeatureIndex while the baseline feature's mass is greater than the candidate feature's mass minus massToleranceDa
                if (baselineFeatureIndex == numBaselineFeatures)
                {
                    baselineFeatureIndex = numBaselineFeatures - 1;
                }
                var baselineFeature = m_baselineFeatures[baselineFeatureIndex];
                while (baselineFeatureIndex >= 0 && (baselineFeature.MassMonoisotopic > feature.MassMonoisotopic - massToleranceDa))
                {
                    baselineFeatureIndex--;
                    if (baselineFeatureIndex >= 0)
                    {
                        baselineFeature = m_baselineFeatures[baselineFeatureIndex];
                    }
                }
                baselineFeatureIndex++;

				// Add candidate matches
                while (baselineFeatureIndex < numBaselineFeatures &&
                       (m_baselineFeatures[baselineFeatureIndex].MassMonoisotopic < (feature.MassMonoisotopic + massToleranceDa)))
                {
                    if (m_baselineFeatures[baselineFeatureIndex].MassMonoisotopic > (feature.MassMonoisotopic - massToleranceDa))
                    {
                        var matchToAdd = new LcmsWarpFeatureMatch
                        {
                            FeatureIndex = featureIndex,
                            FeatureIndex2 = baselineFeatureIndex,
                            Net = feature.Net,
                            Net2 = m_baselineFeatures[baselineFeatureIndex].Net
                        };

                        m_featureMatches.Add(matchToAdd);
                    }
                    baselineFeatureIndex++;
                }
                featureIndex++;
            }

            // Now that matches have been created, go through all the matches and find a mapping
            // of how many times a baseline feature is matched to.
            // Store the matches in a map from a mass tag id to a list of indexes of feature matches

            var massTagToMatches = new Dictionary<int, List<int>>();
            var numMatches = m_featureMatches.Count();
            for (var matchIndex = 0; matchIndex < numMatches; matchIndex++)
            {
                var featureMatch = m_featureMatches[matchIndex];
                var baselineIndex = featureMatch.FeatureIndex2;
                if (!massTagToMatches.ContainsKey(baselineIndex))
                {
                    var matchList = new List<int>();
                    massTagToMatches.Add(baselineIndex, matchList);
                }
                massTagToMatches[baselineIndex].Add(matchIndex);
            }

            // Now go through each of the baseline features matched and for each one keep at
            // most MaxPromiscuousUmcMatches (or none if KeepPromiscuousMatches is false)
            // keeping only the first MaxPromiscuousUmcMatches by scan

            var matchesToUse = new List<LcmsWarpFeatureMatch> {Capacity = m_featureMatches.Count};
            var netMatchesToIndex = new Dictionary<double, List<int>>();

            foreach (var matchIterator in massTagToMatches)
            {
                var baselineIndex = matchIterator.Key;
                var numHits = massTagToMatches[baselineIndex].Count;
                if (numHits <= MaxPromiscuousUmcMatches)
                {
                    // add all of these to the temp matches
                    for (var i = 0; i < numHits; i++)
                    {
                        var matchIndex = matchIterator.Value[i];
                        var match = m_featureMatches[matchIndex];
                        matchesToUse.Add(match);
                    }
                }
                else if (KeepPromiscuousMatches)
                {
                    // keep the matches that have the minimum scan numbers.
                    netMatchesToIndex.Clear();
                    for (var i = 0; i < numHits; i++)
                    {
                        var matchIndex = matchIterator.Value[i];
                        if (!netMatchesToIndex.ContainsKey(m_featureMatches[matchIndex].Net))
                        {
                            var matchList = new List<int>();
                            netMatchesToIndex.Add(m_featureMatches[matchIndex].Net, matchList);
                        }
                        netMatchesToIndex[m_featureMatches[matchIndex].Net].Add(matchIndex);
                    }
                    
                    // Now, keep only the first MaxPromiscuousUmcMatches in the temp list
                    //var scanMatches = netMatchesToIndex.First();
                    
                    for (var index = 0; index < MaxPromiscuousUmcMatches; index++)
                    {
                        var matchIndex = netMatchesToIndex.ElementAt(index).Value[0];
                        var match = m_featureMatches[matchIndex];
                        matchesToUse.Add(match);
                    }
                }
            }

            m_featureMatches = matchesToUse;
        }

        /// <summary>
        /// Performs Mass calibration, depending on calibration type, utilizing MZ
        /// regression, scan regression, or both (with the MZ regression preceeding
        /// the scan regression)
        /// </summary>
        public void PerformMassCalibration()
        {
            switch (CalibrationType)
            {
                case LcmsWarpCalibrationType.MzRegression:
                    PerformMzMassErrorRegression();
                    break;
                case LcmsWarpCalibrationType.ScanRegression:
                    PerformScanMassErrorRegression();
                    break;
                case LcmsWarpCalibrationType.Both:
                    PerformMzMassErrorRegression();
                    PerformScanMassErrorRegression();
                    break;
            }
        }
        
        /// <summary>
        /// Calculates the Standard deviations of the matches.
        /// Note: method requires more than 6 matches to produce meaningful
        /// results.
        /// </summary>
        public void CalculateStandardDeviations()
        {
            var numMatches = m_featureMatches.Count;

            if (numMatches <= REQUIRED_MATCHES)
                return;

            var massDeltas = new List<double>();
            var netDeltas = new List<double>();
            massDeltas.Capacity = numMatches;
            netDeltas.Capacity = numMatches;
            for (var matchNum = 0; matchNum < numMatches; matchNum++)
            {
                var match = m_featureMatches[matchNum];
                var feature = m_features[match.FeatureIndex];
                var baselineFeature = m_baselineFeatures[match.FeatureIndex2];
                var currentMassDelta = ((baselineFeature.MassMonoisotopic - feature.MassMonoisotopic) * 1000000) / feature.MassMonoisotopic;
                var currentNetDelta = baselineFeature.Net - feature.NetAligned;

                massDeltas.Add(currentMassDelta);
                netDeltas.Add(currentNetDelta);
            }
            m_normalProb = 0;
            m_u = 0;
            m_muMass = 0;
            m_muNet = 0;
            MathUtilities.TwoDem(massDeltas, netDeltas, out m_normalProb, out m_u,
                out m_muMass, out m_muNet, out m_massStd, out m_netStd);
        }
        
        /// <summary>
        /// Method to set the flag for whether to use the Mass and the Net
        /// match scores with the LCMSWarper
        /// </summary>
        /// <param name="use"></param>
        public void UseMassAndNetScore(bool use)
        {
            m_useMass = use;
        }

        /// <summary>
        /// Performs Mass error regression based on MZ of the match
        /// </summary>
        private void PerformMzMassErrorRegression()
        {
            //Copy all MZs and mass errors into a list of regression points
            var calibrations = new List<RegressionPoint>();
            
            var numMatches = m_featureMatches.Count;

            for (var matchNum = 0; matchNum < numMatches; matchNum++)
            {
                var match           = m_featureMatches[matchNum];
                var feature         = m_features[match.FeatureIndex];
                var baselineFeature = m_baselineFeatures[match.FeatureIndex2];
                var ppm             = (feature.MassMonoisotopic - baselineFeature.MassMonoisotopic) / baselineFeature.MassMonoisotopic * 1000000; //FeatureLight.ComputeMassPPMDifference(feature.MassMonoisotopic, baselineFeature.MassMonoisotopic);
                var mz              = feature.Mz;
                var netDiff         = baselineFeature.Net - feature.NetAligned;

                var calibrationMatch = new RegressionPoint(mz, 0, netDiff, ppm);

                calibrations.Add(calibrationMatch);
            }
            MzRecalibration.CalculateRegressionFunction(ref calibrations);

            var numFeatures = m_features.Count;
            for (var featureNum = 0; featureNum < numFeatures; featureNum++)
            {
                var feature     = m_features[featureNum];
                var mz          = feature.Mz;
                var mass        = feature.MassMonoisotopic;
                var ppmShift    = MzRecalibration.GetPredictedValue(mz);
                var newMass     = mass - (mass * ppmShift) / 1000000;
                feature.MassMonoisotopicAligned = newMass;
                feature.MassMonoisotopic        = newMass;
            }

        }

        /// <summary>
        /// Performs Mass error regression based on Scan of the match
        /// </summary>
        private void PerformScanMassErrorRegression()
        {
            var calibrations = new List<RegressionPoint>();            
            var numMatches   = m_featureMatches.Count;            

            for (var matchNum = 0; matchNum < numMatches; matchNum++)
            {
                var match           = m_featureMatches[matchNum];
                var feature         = m_features[match.FeatureIndex];
                var baselineFeature = m_baselineFeatures[match.FeatureIndex2];
                var ppm             = (feature.MassMonoisotopic - baselineFeature.MassMonoisotopic) / baselineFeature.MassMonoisotopic * 1000000; //FeatureLight.ComputeMassPPMDifference(feature.MassMonoisotopic, baselineFeature.MassMonoisotopic);
                var net             = feature.Net;
                var netDiff         = baselineFeature.Net - feature.NetAligned;

                var calibrationMatch = new RegressionPoint(net, 0, netDiff, ppm);                

                calibrations.Add(calibrationMatch);
            }

            NetRecalibration.CalculateRegressionFunction(ref calibrations);

            var numFeatures = m_features.Count;
            for (var featureNum = 0; featureNum < numFeatures; featureNum++)
            {
                var feature         = m_features[featureNum];
                var net             = feature.Net;
                var mass            = feature.MassMonoisotopic;
                var ppmShift        = MzRecalibration.GetPredictedValue(net);
                var newMass         = mass - (mass * ppmShift) / 1000000;
                feature.MassMonoisotopicAligned = newMass;
                feature.MassMonoisotopic = newMass;
            }
        }

        /// <summary>
        /// Goes through the matched features and determines the probability
        /// of each that the match is correct
        /// </summary>
        public void GetMatchProbabilities()
        {
            PercentComplete = 0;
            var numFeatures = m_features.Count;

            m_tempFeatureBestDelta.Clear();
            m_tempFeatureBestDelta.Capacity = numFeatures;

            m_tempFeatureBestIndex.Clear();
            m_tempFeatureBestIndex.Capacity = numFeatures;

            //Clear the old features and initialize each section to 0
            m_numFeaturesInSections.Clear();
            m_numFeaturesInSections.Capacity = NumSections;
            for (var i = 0; i < NumSections; i++)
            {
                m_numFeaturesInSections.Add(0);
            }

            // Do the same for baseline sections
            m_numFeaturesInBaselineSections.Clear();
            m_numFeaturesInBaselineSections.Capacity = numFeatures;
            for (var i = 0; i < NumBaselineSections; i++)
            {
                m_numFeaturesInBaselineSections.Add(0);
            }

            MaxNet = double.MinValue;
            MinNet = double.MaxValue;
            for (var i = 0; i < numFeatures; i++)
            {
                if (m_features[i].Net > MaxNet)
                {
                    MaxNet = m_features[i].Net;
                }
                if (m_features[i].Net < MinNet)
                {
                    MinNet = m_features[i].Net;
                }
                m_tempFeatureBestDelta.Add(double.MaxValue);
                m_tempFeatureBestIndex.Add(-1);
            }

            numFeatures = m_features.Count;
            for (var i = 0; i < numFeatures; i++)
            {
                var net = m_features[i].Net;
                var sectionNum = Convert.ToInt32(((net - MinNet) * NumSections) / (MaxNet - MinNet));
                
                if (sectionNum >= NumSections)
                {
                    sectionNum = NumSections - 1;
                }
                if (sectionNum < 0)
                {
                    sectionNum = 0;
                }
                m_numFeaturesInSections[sectionNum]++;
            }

            MinBaselineNet = double.MaxValue;
            MaxBaselineNet = double.MinValue;

            var numBaselineFeatures = m_baselineFeatures.Count;

            //MinBaselineNet = m_baselineFeatures.Min(x => x.Net);
            //MaxBaselineNet = m_baselineFeatures.Max(x => x.Net);

            for (var i = 0; i < numBaselineFeatures; i++)
            {
                if (m_baselineFeatures[i].Net < MinBaselineNet)
                {
                    MinBaselineNet = m_baselineFeatures[i].Net;
                }
                if (m_baselineFeatures[i].Net > MaxBaselineNet)
                {
                    MaxBaselineNet = m_baselineFeatures[i].Net;
                }
            }

            for (var i = 0; i < numBaselineFeatures; i++)
            {
                var net = m_baselineFeatures[i].Net;
                var msmsSectionNum = (int)(((net - MinBaselineNet) * NumBaselineSections) / (MaxBaselineNet - MinBaselineNet));
                if (msmsSectionNum == NumBaselineSections)
                {
                    msmsSectionNum--;
                }
                m_numFeaturesInBaselineSections[msmsSectionNum]++;
            }

            m_featureMatches = m_featureMatches.OrderBy(x => x.Net).ToList();
            var numMatches   = m_featureMatches.Count;

            var sectionFeatures = new List<LcmsWarpFeatureMatch>();

            var numSectionMatches = NumSections * NumMatchesPerSection;
            m_subsectionMatchScores.Clear();
            m_subsectionMatchScores.Capacity = numSectionMatches;

            for (var i = 0; i < numSectionMatches; i++)
            {
                m_subsectionMatchScores.Add(m_minScore);
            }

            if (numMatches == 0)
            {
                return;
            }

            for (var section = 0; section < NumSections; section++)
            {
                var startMatchIndex = 0;
                sectionFeatures.Clear();
                var sectionStartNet = MinNet + (section * (MaxNet - MinNet)) / NumSections;
                var sectionEndNet = MinNet + ((section + 1) * (MaxNet - MinNet)) / NumSections;

                while (startMatchIndex < numMatches && ((m_featureMatches[startMatchIndex].Net) < sectionStartNet))
                {
                    startMatchIndex++;
                }
                var endMatchIndex = startMatchIndex;
                while (endMatchIndex < numMatches && ((m_featureMatches[endMatchIndex].Net) < sectionEndNet))
                {
                    endMatchIndex++;
                }
                if (endMatchIndex != startMatchIndex)
                {
                    for (var index = startMatchIndex; index < endMatchIndex; index++)
                    {
                        sectionFeatures.Add(m_featureMatches[index]);
                    }
                }
                
                ComputeSectionMatch(section, ref sectionFeatures, sectionStartNet, sectionEndNet);

                var percentComplete = section / (double)NumSections * 100;
                OnProgress("Getting match probabilities", percentComplete);
            }
        }

        /// <summary>
        /// Calculates the alignment function for each of the sections, based
        /// on the match scores for every feature in the subsection
        /// </summary>
        public void CalculateAlignmentFunction()
        {
            var section = NumSections - 1;

            var bestPreviousAlignmentIndex = -1;
            var bestScore = double.MinValue;
            var bestAlignedBaselineSection = -1;
            var bestAlignmentIndex = -1;

            for (var baselineSection = 0; baselineSection < NumBaselineSections; baselineSection++)
            {
                // Everything past this section would have remained unmatched.
                for (var sectionWidth = 0; sectionWidth < NumMatchesPerBaseline; sectionWidth++)
                {                                       
                    var alignmentIndex = (section * NumMatchesPerSection) + (baselineSection * NumMatchesPerBaseline) + sectionWidth;
                    var alignmentScore = m_alignmentScore[alignmentIndex];

                    if (!(alignmentScore > bestScore))
                        continue;

                    bestScore = alignmentScore;
                    bestPreviousAlignmentIndex = m_bestPreviousIndex[alignmentIndex];
                    bestAlignedBaselineSection = baselineSection;
                    bestAlignmentIndex = alignmentIndex;
                }
            }

            var msmsSectionWidth = (MaxBaselineNet - MinBaselineNet) / NumBaselineSections;

            var netStart = MinNet + (section * (MaxNet - MinNet)) / NumSections;
            var netEnd = MinNet + ((section + 1) * (MaxNet - MinNet)) / NumSections;
            var baselineSectionStart = bestAlignedBaselineSection;
            var baselineSectionEnd = baselineSectionStart + bestAlignmentIndex % NumMatchesPerBaseline + 1;
            var baselineStartNet = baselineSectionStart * msmsSectionWidth + MinBaselineNet;
            var baselineEndNet = baselineSectionEnd * msmsSectionWidth + MinBaselineNet;

            var match = new LcmsWarpAlignmentMatch();
            match.Set(netStart, netEnd, section, NumSections, baselineStartNet, baselineEndNet, baselineSectionStart,
                      baselineSectionEnd, bestScore, m_subsectionMatchScores[bestAlignmentIndex]);
            m_alignmentFunc.Clear();
            m_alignmentFunc.Add(match);

            while (bestPreviousAlignmentIndex >= 0)
            {
                var sectionStart = (bestPreviousAlignmentIndex / NumMatchesPerSection); // should be current - 1
                var sectionEnd = sectionStart + 1;
                
                var nextNetStart = MinNet + (sectionStart * (MaxNet - MinNet)) / NumSections;
                var nextNetEnd = MinNet + (sectionEnd * (MaxNet - MinNet)) / NumSections;

                var nextBaselineSectionStart = (bestPreviousAlignmentIndex - (sectionStart * NumMatchesPerSection)) / NumMatchesPerBaseline;
                var nextBaselineSectionEnd = nextBaselineSectionStart + (bestPreviousAlignmentIndex % NumMatchesPerBaseline) + 1;
                var nextBaselineStartNet = (nextBaselineSectionStart * msmsSectionWidth) + MinBaselineNet;
                var nextBaselineEndNet = nextBaselineSectionEnd * msmsSectionWidth + MinBaselineNet;
                match = new LcmsWarpAlignmentMatch();
                match.Set(nextNetStart, nextNetEnd, sectionStart, sectionEnd, nextBaselineStartNet, nextBaselineEndNet,
                          nextBaselineSectionStart, nextBaselineSectionEnd, m_alignmentScore[bestPreviousAlignmentIndex],
                          m_subsectionMatchScores[bestPreviousAlignmentIndex]);

                bestPreviousAlignmentIndex = m_bestPreviousIndex[bestPreviousAlignmentIndex];
                m_alignmentFunc.Add(match);
            }
            m_alignmentFunc.Sort();

            
        }

        /// <summary>
        /// Computes the alignment matrix for all possible alignments, holding
        /// onto each and every possible alignment score and then linking the feature
        /// to the best previous index of possible alignments
        /// </summary>
        public void CalculateAlignmentMatrix()
        {
            var numPossibleAlignments = NumSections * NumBaselineSections * NumMatchesPerBaseline;

            m_alignmentScore = new double[numPossibleAlignments];
            m_bestPreviousIndex = new int[numPossibleAlignments];

            // Initialize scores to - inf, best previous index to -1
            for (var i = 0; i < numPossibleAlignments; i++)
            {
                m_alignmentScore[i] = double.MinValue;
                m_bestPreviousIndex[i] = -1;
            }
            var log2PiStdNetStdNet = Math.Log(2 * Math.PI * m_netStd * m_netStd);

            var unmatchedScore = -0.5 * log2PiStdNetStdNet;
            if (NetTolerance < 3 * m_netStd)
            {
                unmatchedScore -= (0.5 * 9.0);
            }
            else
            {
                unmatchedScore -= (0.5 * (NetTolerance * NetTolerance) / (m_netStd * m_netStd));
            }
            if (m_useMass)
            {
                //Assumes that for the unmatched, the masses were also off at mass tolerance, so use the same threshold from NET
                unmatchedScore = 2 * unmatchedScore;
            }

            
            for (var baselineSection = 0; baselineSection < NumBaselineSections; baselineSection++)
            {
                //Assume everything that was matched was past 3 standard devs in net.
                for (var sectionWidth = 0; sectionWidth < NumMatchesPerBaseline; sectionWidth++)
                {
                    //no need to mulitply with msSection because its 0
                    var alignmentIndex = baselineSection * NumMatchesPerBaseline + sectionWidth;
                    m_alignmentScore[alignmentIndex] = 0;
                }            
            }

            var numUnmatchedMsFeatures = 0;
            for (var section = 0; section < NumSections; section++)
            {
                for (var sectionWidth = 0; sectionWidth < NumMatchesPerBaseline; sectionWidth++)
                {
                    var alignmentIndex = section * NumMatchesPerSection + sectionWidth;
                    m_alignmentScore[alignmentIndex] = m_subsectionMatchScores[alignmentIndex] + unmatchedScore * numUnmatchedMsFeatures;
                }
                numUnmatchedMsFeatures += m_numFeaturesInSections[section];
            }

            for (var section = 1; section < NumSections; section++)
            {
                for (var baselineSection = 1; baselineSection < NumBaselineSections; baselineSection++)
                {
                    for (var sectionWidth = 0; sectionWidth < NumMatchesPerBaseline; sectionWidth++)
                    {
                        var alignmentIndex = section * NumMatchesPerSection + baselineSection * NumMatchesPerBaseline + sectionWidth;

                        var currentBestScore = double.MinValue;
                        var bestPreviousAlignmentIndex = 0;

                        for (var previousBaselineSection = baselineSection - 1; previousBaselineSection >= baselineSection - NumMatchesPerBaseline - MaxJump; previousBaselineSection--)
                        {
                            if (previousBaselineSection < 0)
                            {
                                break;
                            }
                            var maxWidth = baselineSection - previousBaselineSection;
                            if (maxWidth > NumMatchesPerBaseline)
                            {
                                maxWidth = NumMatchesPerBaseline;
                            }
                            var previousBaselineSectionWidth = maxWidth;
                            var previousAlignmentIndex = (section - 1) * NumMatchesPerSection + previousBaselineSection * NumMatchesPerBaseline + previousBaselineSectionWidth - 1;
                            if (!(m_alignmentScore[previousAlignmentIndex] > currentBestScore))
                                continue;

                            currentBestScore = m_alignmentScore[previousAlignmentIndex];
                            bestPreviousAlignmentIndex = previousAlignmentIndex;
                        }
                        if (Math.Abs(currentBestScore - double.MinValue) > double.Epsilon)
                        {
                            m_alignmentScore[alignmentIndex] = currentBestScore + m_subsectionMatchScores[alignmentIndex];
                            m_bestPreviousIndex[alignmentIndex] = bestPreviousAlignmentIndex;
                        }
                        else
                        {
                            m_alignmentScore[alignmentIndex] = double.MinValue;
                        }
                    }
                }
            }            
        }

        private void ComputeSectionMatch(int msSection, ref List<LcmsWarpFeatureMatch> sectionMatchingFeatures, double minNet, double maxNet)
        {
            var numMatchingFeatures = sectionMatchingFeatures.Count;
            var baselineSectionWidth = (MaxBaselineNet - MinBaselineNet) / NumBaselineSections;

            // keep track of only the unique indices of ms features because we only want the best matches for each
            m_sectionUniqueFeatureIndices.Clear();
            m_sectionUniqueFeatureIndices.Capacity = numMatchingFeatures;
            for (var i = 0; i < numMatchingFeatures; i++)
            {
                var found = false;
                var match = sectionMatchingFeatures[i];
                for (var j = 0; j < i; j++)
                {
                    if (match.FeatureIndex != sectionMatchingFeatures[j].FeatureIndex)
                        continue;

                    found = true;
                    break;
                }
                if (!found)
                {
                    m_sectionUniqueFeatureIndices.Add(match.FeatureIndex);
                }
            }

            var numUniqueFeaturesStart = m_sectionUniqueFeatureIndices.Count;
            
            for (var baselineSectionStart = 0; baselineSectionStart < NumBaselineSections; baselineSectionStart++)
            {
                var baselineStartNet = MinBaselineNet + baselineSectionStart * baselineSectionWidth;
                var endSection = baselineSectionStart + NumMatchesPerBaseline;
                if (endSection >= NumBaselineSections)
                {
                    endSection = NumBaselineSections;
                }
                for (var baselineSectionEnd = baselineSectionStart; baselineSectionEnd < endSection; baselineSectionEnd++)
                {
                    var numUniqueFeatures = numUniqueFeaturesStart;

                    var sectionIndex = msSection * NumMatchesPerSection + baselineSectionStart * NumMatchesPerBaseline
                                     + (baselineSectionEnd - baselineSectionStart);
                    var baselineEndNet = MinBaselineNet + (baselineSectionEnd + 1) * baselineSectionWidth;

                    for (var i = 0; i < numUniqueFeatures; i++)
                    {
                        var msFeatureIndex = m_sectionUniqueFeatureIndices[i];
                        m_tempFeatureBestDelta[msFeatureIndex] = double.MaxValue;
                        m_tempFeatureBestIndex[msFeatureIndex] = -1;
                    }

                    // Now that we have msmsSection and matching msSection, transform the scan numbers to nets using a
                    // transformation of the two sections, and use a temporary list to keep only the best match
                    for (var i = 0; i < numMatchingFeatures; i++)
                    {
                        var match = sectionMatchingFeatures[i];
                        var msFeatureIndex = match.FeatureIndex;
                        var featureNet = match.Net;

                        var transformNet = (featureNet - minNet) * (baselineEndNet - baselineStartNet);
                        transformNet = transformNet / (maxNet - minNet) + baselineStartNet;

                        var deltaMatch = transformNet - match.Net2;
                        if (!(Math.Abs(deltaMatch) < Math.Abs(m_tempFeatureBestDelta[msFeatureIndex])))
                            continue;

                        m_tempFeatureBestDelta[msFeatureIndex] = deltaMatch;
                        m_tempFeatureBestIndex[msFeatureIndex] = match.FeatureIndex2;
                    }

                    m_subsectionMatchScores[sectionIndex] = CurrentlyStoredSectionMatchScore(numUniqueFeatures);
                }
            }
        }

        /// <summary>
        /// Given an MZ value, return the appropriate ppm shift
        /// Note: Requires LCMSWarp to be calibrating based on MZ Regression or
        /// Hybrid regression to process.
        /// </summary>
        /// <param name="mz"></param>
        /// <returns></returns>
        public double GetPpmShiftFromMz(double mz)
        {
            if (CalibrationType == LcmsWarpCalibrationType.MzRegression || CalibrationType == LcmsWarpCalibrationType.Both)
            {
                return MzRecalibration.GetPredictedValue(mz);
            }
            return 0;
        }

        /// <summary>
        /// Given an NET value, return the appropriate ppm shift
        /// Note: Requires LCMSWarp to be calibrating based on Scan Regression or
        /// Hybrid regression to process.
        /// </summary>
        /// <param name="net"></param>
        /// <returns></returns>
        public double GetPpmShiftFromNet(double net)
        {
            if (CalibrationType == LcmsWarpCalibrationType.ScanRegression || CalibrationType == LcmsWarpCalibrationType.Both)
            {
                return NetRecalibration.GetPredictedValue(net);
            }
            return 0;
        }

        /// <summary>
        /// Calculates the match score of the subsection, saving
        /// the match scores to the first parameter and the alignee and
        /// reference values to the second and third parameters.
        /// </summary>
        /// <param name="subsectionMatchScores"></param>
        /// <param name="aligneeVals"></param>
        /// <param name="refVals"></param>
        /// <param name="standardize"></param>
        public void GetSubsectionMatchScore(ref List<double> subsectionMatchScores, ref List<double> aligneeVals,
                                            ref List<double> refVals, bool standardize)
        {
            subsectionMatchScores.Clear();
            for (var msSection = 0; msSection < NumSections; msSection++)
            {
                aligneeVals.Add(MinNet + (msSection * (MaxNet - MinNet)) / NumSections);
            }
            for (var msmsSection = 0; msmsSection < NumBaselineSections; msmsSection++)
            {
                refVals.Add(MinBaselineNet + (msmsSection * (MaxBaselineNet - MinBaselineNet)) / NumBaselineSections);
            }

            for (var msSection = 0; msSection < NumSections; msSection++)
            {
                for (var msmsSection = 0; msmsSection < NumBaselineSections; msmsSection++)
                {
                    var maxScore = double.MinValue;
                    for (var msmsSectionWidth = 0; msmsSectionWidth < NumMatchesPerBaseline; msmsSectionWidth++)
                    {
                        if (msmsSection + msmsSectionWidth >= NumBaselineSections)
                        {
                            continue;
                        }
                        var matchIndex = (msSection * NumMatchesPerSection) + (msmsSection * NumMatchesPerBaseline) + msmsSectionWidth;
                        if (m_subsectionMatchScores[matchIndex] > maxScore)
                        {
                            maxScore = m_subsectionMatchScores[matchIndex];
                        }
                    }
                    subsectionMatchScores.Add(maxScore);
                }
            }
            if (!standardize)
                return;

            var index = 0;
            for (var msSection = 0; msSection < NumSections; msSection++)
            {
                double sumX = 0, sumXx = 0;
                var startIndex = index;
                var realMinScore = double.MaxValue;
                var numPoints = 0;
                for (var msmsSection = 0; msmsSection < NumBaselineSections; msmsSection++)
                {
                    var score = subsectionMatchScores[index];
                    if (Math.Abs(score - m_minScore) > double.Epsilon)
                    {
                        if (score < realMinScore)
                        {
                            realMinScore = score;
                        }
                        sumX = sumX + score;
                        sumXx = sumXx + (score * score);
                        numPoints++;
                    }
                    index++;
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

                index = startIndex;
                for (var msmsSection = 0; msmsSection < NumBaselineSections; msmsSection++)
                {
                    var score = subsectionMatchScores[index];
                    if (Math.Abs(score - m_minScore) < double.Epsilon)
                    {
                        score = realMinScore;
                    }
                    if (numPoints > 1)
                    {
                        subsectionMatchScores[index] = ((score - avg) / stDev);
                    }
                    else
                    {
                        subsectionMatchScores[index] = 0;
                    }
                    index++;
                }
            }
        }

        public void GetErrorHistograms(double massBin, double netBin, double driftBin, ref List<double> massErrorBin,
                                       ref List<int> massErrorFrequency, ref List<double> netErrorBin,
                                       ref List<int> netErrorFrequency, ref List<double> driftErrorBin,
                                       ref List<int> driftErrorFrequency)
        {
            var numMatches = m_featureMatches.Count;
            var massErrors = new List<double> { Capacity = numMatches };
            var netErrors = new List<double> { Capacity = numMatches };
            var driftErrors = new List<double> { Capacity = numMatches };

            foreach(var match in m_featureMatches)
            {
                massErrors.Add(match.PpmMassError);
                netErrors.Add(match.NetError);
                driftErrors.Add(match.DriftError);
            }

            Histogram.CreateHistogram(massErrors, ref massErrorBin, ref massErrorFrequency, massBin);
            Histogram.CreateHistogram(netErrors, ref netErrorBin, ref netErrorFrequency, netBin);
            Histogram.CreateHistogram(driftErrors, ref driftErrorBin, ref driftErrorFrequency, driftBin);

        }
    }
}
