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

        private readonly List<double> _tempFeatureBestDelta = new List<double>();
        private readonly List<int> _tempFeatureBestIndex = new List<int>();

        private readonly List<int> _sectionUniqueFeatureIndices = new List<int>();
        private readonly List<int> _numFeaturesInSections = new List<int>();
        private readonly List<int> _numFeaturesInBaselineSections = new List<int>();

        //Interpolation m_interpolation;
        private double[] _alignmentScore = null;
        private int[] _bestPreviousIndex = null;

        private const double MIN_MASS_NET_LIKELIHOOD = 1e-4;
        private const int REQUIRED_MATCHES = 6;

        private bool _useMass = false;

        // Used to control the granularity of the MSMS section size when comparing against MS Sections.
        // The number of sections in the MSMS will be # of sections in MS * m_maxSectionDistortion.
        // Thus each section of the MS can be compared to MSMS section which are 1/m_maxSectionDistortion to
        // m_maxSectionDistortion times the ms section size of the chromatographic run.

        private const double MinScore = -100000;

        // Mass window around which the mass tolerance is applied
        private double _massStd = 20;

        private double _netStd = 0.007;

        private double _normalProb = 0.3;
        private double _u = 0;
        private double _muMass = 0;
        private double _muNet = 0;
        private readonly List<LcmsWarpAlignmentMatch> _alignmentFunc = new List<LcmsWarpAlignmentMatch>();
        private readonly List<UMCLight> _features = new List<UMCLight>();
        private readonly List<UMCLight> _baselineFeatures = new List<UMCLight>();

        public List<LcmsWarpFeatureMatch> FeatureMatches = new List<LcmsWarpFeatureMatch>();

        private readonly List<double> _subsectionMatchScores = new List<double>();
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
            get { return FeatureMatches.Count; }
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
            if (left != null)
            {
                if (right != null)
                {
                    return left.MassMonoisotopic.CompareTo(right.MassMonoisotopic);
                }
                return 1;
            }
            if (right == null)
            {
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// Public Constructor, doesn't take arguments, initializes memory space and sets it
        /// to default values
        /// </summary>
        public LcmsWarp()
        {
            MzRecalibration = new LcmsWarpCombinedRegression();
            NetRecalibration = new LcmsWarpCombinedRegression();

            MassCalibrationWindow = 50;
            MassTolerance = 20; // ppm
            NetTolerance = 0.02;

            MaxJump = 10;

            KeepPromiscuousMatches = false;
            MaxPromiscuousUmcMatches = 3;

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

            const double outlierZScore = 2.5;
            const int numKnots = 12;
            MzRecalibration.SetLsqOptions(numKnots, outlierZScore);
            NetRecalibration.SetLsqOptions(numKnots, outlierZScore);
        }

        /// <summary>
        /// Method which calculates the Net slope, intercept and r squared as if
        /// a linear regression was performed
        /// </summary>
        private void CalculateNetSlopeAndIntercept()
        {
            var startNets = FeatureMatches.Select(match => match.Net).ToList();
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
                        var alignmentIndex = (section * NumMatchesPerSection) +
                                             (baselineSection * NumMatchesPerBaseline) + sectionWidth;

                        if (!(_subsectionMatchScores[alignmentIndex] > maxScore))
                            continue;

                        maxScore = _subsectionMatchScores[alignmentIndex];
                        y = baselineSection;
                    }
                }

                var net = ((section * (MaxNet - MinNet)) / NumSections) + MinNet;
                var alignedNet = ((y * (MaxBaselineNet - MinBaselineNet)) / NumBaselineSections) + MinBaselineNet;

                sumX += net;
                sumY += alignedNet;
                sumXy += net * alignedNet;
                sumXx += net * net;
                sumYy += alignedNet * alignedNet;
                numSumPoints++;
            }

            NetSlope = ((numSumPoints * sumXy) - (sumX * sumY)) / ((numSumPoints * sumXx) - (sumX * sumX));
            NetIntercept = (sumY - NetSlope * sumX) / numSumPoints;

            var temp = ((numSumPoints * sumXy) - (sumX * sumY)) /
                       Math.Sqrt(((numSumPoints * sumXx) - (sumX * sumX)) * ((numSumPoints * sumYy) - (sumY * sumY)));
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

            var lg2PiStdNetSqrd = Math.Log(2 * Math.PI * _netStd * _netStd);
            for (var i = 0; i < numUniqueFeatures; i++)
            {
                var msFeatureIndex = _sectionUniqueFeatureIndices[i];
                var featureMonoMass = _features[msFeatureIndex].MassMonoisotopic;
                var baselineFeatureMonoMass = _baselineFeatures[_tempFeatureBestIndex[msFeatureIndex]].MassMonoisotopic;

                var deltaNet = _tempFeatureBestDelta[msFeatureIndex];

                if (_useMass)
                {
                    var massDelta = (featureMonoMass - baselineFeatureMonoMass) * 1000000 /
                                    baselineFeatureMonoMass;
                    var likelihood = GetMatchLikelihood(massDelta, deltaNet);
                    matchScore += Math.Log(likelihood);
                }
                else
                {
                    var calcVal = deltaNet;
                    if (Math.Abs(deltaNet) > NetTolerance)
                    {
                        calcVal = NetTolerance;
                    }
                    matchScore -= 0.5 * (calcVal / _netStd) * (calcVal / _netStd);
                    matchScore -= 0.5 * lg2PiStdNetSqrd;
                }
            }
            return matchScore;
        }

        private double GetMatchLikelihood(double massDelta, double netDelta)
        {
            var massZ = massDelta / _massStd;
            var netZ = netDelta / _netStd;
            var normProb = Math.Exp(-0.5 * ((massZ * massZ) + (netZ * netZ))) / (2 * Math.PI * _netStd * _massStd);
            var likelihood = (normProb * _normalProb + ((1 - _normalProb) * _u));
            if (likelihood < MIN_MASS_NET_LIKELIHOOD)
            {
                likelihood = MIN_MASS_NET_LIKELIHOOD;
            }
            return likelihood;
        }

        /// <summary>
        /// Statistical Mass standard deviation, or standard error, or standard something... (from LCMS Warper)
        /// </summary>
        public double StatisticMassStd
        {
            get { return _massStd; }
        }

        /// <summary>
        /// Statistical Net standard deviation, or standard error, or standard something... (from LCMS Warper)
        /// </summary>
        public double StatisticNetStd
        {
            get { return _netStd; }
        }

        /// <summary>
        /// Statistical Mass Mu (population mean) (from LCMS Warper)
        /// </summary>
        public double StatisticMassMu
        {
            get { return _muMass; }
        }

        /// <summary>
        /// Statistical Net Mu (population mean) (from LCMS Warper)
        /// </summary>
        public double StatisticNetMu
        {
            get { return _muNet; }
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
        /// Gets the residual data from the Warper
        /// </summary>
        public ResidualData GetResiduals()
        {
            var count = FeatureMatches.Count;
            var rd = new ResidualData();
            rd.Net.Capacity = count;
            rd.Mz.Capacity = count;
            rd.LinearNet.Capacity = count;
            rd.CustomNet.Capacity = count;
            rd.LinearCustomNet.Capacity = count;
            rd.MassError.Capacity = count;
            rd.MassErrorCorrected.Capacity = count;
            rd.MzMassError.Capacity = count;
            rd.MzMassErrorCorrected.Capacity = count;

            foreach (var match in FeatureMatches)
            {
                var feature = _features[match.FeatureIndex];
                var predictedLinear = (NetSlope * match.Net2) + NetIntercept;
                var ppmMassError = match.PpmMassError;
                var scanNumber = match.Net;

                // NET
                rd.Net.Add(scanNumber);
                rd.LinearNet.Add(feature.NetAligned - predictedLinear);
                rd.CustomNet.Add(match.NetError);
                rd.LinearCustomNet.Add(feature.NetAligned - predictedLinear);

                var ppmShift = 0.0;
                if (_useMass)
                {
                    ppmShift = GetPpmShift(feature.Mz, scanNumber);
                }
                rd.Mz.Add(feature.Mz);
                rd.MassError.Add(ppmMassError);
                rd.MzMassError.Add(ppmMassError);

                rd.MassErrorCorrected.Add(ppmMassError - ppmShift);
                rd.MzMassErrorCorrected.Add(ppmMassError - ppmShift);
            }

            return rd;
        }

        /// <summary>
        /// Updates the Alignee and Reference net lists with the data from the alignment
        /// function determined by the warper with the Net start data
        /// </summary>
        /// <param name="aligneeNet"></param>
        /// <param name="referenceNet"></param>
        public void AlignmentFunction(out List<double> aligneeNet, out List<double> referenceNet)
        {
            aligneeNet = new List<double>();
            referenceNet = new List<double>();
            var numPieces = _alignmentFunc.Count;
            for (var pieceNum = 0; pieceNum < numPieces; pieceNum++)
            {
                aligneeNet.Add(_alignmentFunc[pieceNum].NetStart);
                referenceNet.Add(_alignmentFunc[pieceNum].NetStart2);
            }
        }

        /// <summary>
        /// Determines the transformed NETs for the LCMSWarp function
        /// </summary>
        public void GetTransformedNets()
        {
            var dicSectionToIndex = new Dictionary<int, int>();
            for (var i = 0; i < _alignmentFunc.Count; i++)
            {
                dicSectionToIndex.Add(_alignmentFunc[i].SectionStart, i);
            }

            for (int featureIndex = 0; featureIndex < _features.Count; featureIndex++)
            {
                var feature = _features[featureIndex];
                _features[featureIndex].NetAligned = GetTransformedNet(feature.Net, dicSectionToIndex);
                _features[featureIndex].NetStart = GetTransformedNet(feature.NetStart, dicSectionToIndex);
                _features[featureIndex].NetEnd = GetTransformedNet(feature.NetEnd, dicSectionToIndex);
            }
        }

        /// <summary>
        /// Transform a single NET value using alignment function.
        /// </summary>
        /// <param name="aligneeNet">NET to transform.</param>
        /// <param name="dicSectionToIndex">Mapping of section to net index.</param>
        /// <returns>Transformed NET.</returns>
        private double GetTransformedNet(double aligneeNet, Dictionary<int, int> dicSectionToIndex)
        {
            var alignmentFuncLength = _alignmentFunc.Count;
            double netStart;
            double netEndBaseline;
            double netEnd;
            double netStartBaseline;
            if (aligneeNet < _alignmentFunc[0].NetStart)
            {
                netStart = _alignmentFunc[0].NetStart;
                netStartBaseline = _alignmentFunc[0].NetStart2;
                netEnd = _alignmentFunc[0].NetEnd;
                netEndBaseline = _alignmentFunc[0].NetEnd2;

                return ((aligneeNet - netStart) * (netEndBaseline - netStartBaseline)) /
                                       (netEnd - netStart) + netStartBaseline;
            }
            if (aligneeNet > _alignmentFunc[alignmentFuncLength - 1].NetEnd)
            {
                netStart = _alignmentFunc[alignmentFuncLength - 1].NetStart;
                netStartBaseline = _alignmentFunc[alignmentFuncLength - 1].NetStart2;
                netEnd = _alignmentFunc[alignmentFuncLength - 1].NetEnd;
                netEndBaseline = _alignmentFunc[alignmentFuncLength - 1].NetEnd2;

                return ((aligneeNet - netStart) * (netEndBaseline - netStartBaseline)) / (netEnd - netStart) +
                                 netStartBaseline;
            }

            var msSection1 = Convert.ToInt32(((aligneeNet - MinNet) * NumSections) / (MaxNet - MinNet));
            if (msSection1 >= NumSections)
            {
                msSection1 = NumSections - 1;
            }

            var msSectionIndex = dicSectionToIndex[msSection1];

            netStart = _alignmentFunc[msSectionIndex].NetStart;
            netEnd = _alignmentFunc[msSectionIndex].NetEnd;

            netStartBaseline = _alignmentFunc[msSectionIndex].NetStart2;
            netEndBaseline = _alignmentFunc[msSectionIndex].NetEnd2;

            return ((aligneeNet - netStart) * (netEndBaseline - netStartBaseline)) / (netEnd - netStart) +
                             netStartBaseline;
        }

        /// <summary>
        /// Initially, clears any residual feature matches to ensure no carryover of prior runs,
        /// then goes through and calculates the match score for each feature with relation to
        /// the baselines, holding onto the "best match" for each one
        /// </summary>
        public void CalculateAlignmentMatches()
        {
            _features.Sort(ByMass);
            _baselineFeatures.Sort(ByMass);

            var baselineFeatureIndex = 0;
            var numBaselineFeatures = _baselineFeatures.Count;

            FeatureMatches.Clear();

            var minMatchScore = -0.5 * (MassTolerance * MassTolerance) / (_massStd * _massStd);
            minMatchScore -= 0.5 * (NetTolerance * NetTolerance) / (_netStd * _netStd);

            for (var featureIndex = 0; featureIndex < _features.Count; featureIndex++)
            {
                var feature = _features[featureIndex];
                var massTolerance = feature.MassMonoisotopic * MassTolerance / 1000000;

                while (baselineFeatureIndex == numBaselineFeatures || baselineFeatureIndex >= 0 &&
                       _baselineFeatures[baselineFeatureIndex].MassMonoisotopic >
                       feature.MassMonoisotopic - massTolerance)
                {
                    baselineFeatureIndex--;
                }
                baselineFeatureIndex++;

                LcmsWarpFeatureMatch bestMatchFeature = null;
                var bestMatchScore = minMatchScore;
                while (baselineFeatureIndex < numBaselineFeatures &&
                       _baselineFeatures[baselineFeatureIndex].MassMonoisotopic <
                       feature.MassMonoisotopic + massTolerance)
                {
                    var baselineFeature = _baselineFeatures[baselineFeatureIndex];
                    if (baselineFeature.MassMonoisotopic >
                        feature.MassMonoisotopic - massTolerance)
                    {
                        //Calculate the mass and net errors
                        var netDiff = baselineFeature.Net - feature.NetAligned;
                        var driftDiff = baselineFeature.DriftTime - feature.DriftTime;
                        var massDiff = (baselineFeature.MassMonoisotopic -
                                        feature.MassMonoisotopic) * 1000000.0 / feature.MassMonoisotopic;

                        //Calculate the match score
                        var matchScore = -0.5 * (netDiff * netDiff) / (_netStd * _netStd);
                        matchScore -= 0.5 * (massDiff * massDiff) / (_massStd * _massStd);

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
                                Net2 = _baselineFeatures[baselineFeatureIndex].Net
                            };
                        }
                    }
                    baselineFeatureIndex++;
                }

                //If we found a match, add it to the list of matches
                if (bestMatchFeature != null)
                {
                    FeatureMatches.Add(bestMatchFeature);
                }
            }

            CalculateNetSlopeAndIntercept();
        }

        /// <summary>
        /// Initializes the features in the LCMSWarp object to the list of UMCLights
        /// </summary>
        /// <param name="features"></param>
        public void SetFeatures(List<UMCLight> features)
        {
            _features.Clear();
            _features.AddRange(features);
        }

        /// <summary>
        /// Returns UMCLight data with calibrated masses and aligned nets
        /// </summary>
        public IEnumerable<UMCLight> GetFeatureCalibratedMassesAndAlignedNets()
        {
            return _features.Select(x => new UMCLight
            {
                Id = x.Id,
                MassMonoisotopicAligned = x.MassMonoisotopicAligned,
                NetAligned = x.NetAligned,
                NetStart = x.NetStart,
                NetEnd = x.NetEnd,
                DriftTime = x.DriftTime
            });
        }

        /// <summary>
        /// Returns UMCLight data with calibrated masses, aligned NETs, drift times,
        /// aligned scans based on the min and max scan numbers
        /// </summary>
        /// <param name="minScan"></param>
        /// <param name="maxScan"></param>
        public IEnumerable<UMCLight> GetFeatureCalibratedMassesAndAlignedNets(int minScan, int maxScan)
        {
            return _features.OrderBy(x => x.Id).Select(x => new UMCLight
            {
                Id = x.Id,
                MassMonoisotopicAligned = x.MassMonoisotopicAligned,
                NetAligned = x.NetAligned,
                NetStart = x.NetStart,
                NetEnd = x.NetEnd,
                ScanAligned = minScan + (int) (x.NetAligned * (maxScan - minScan)),
                DriftTime = x.DriftTime
            });
        }

        /// <summary>
        /// Initializes the reference features to the features entered in
        /// </summary>
        /// <param name="features"></param>
        public void SetReferenceFeatures(List<UMCLight> features)
        {
            _baselineFeatures.Clear();
            _baselineFeatures.AddRange(features);
        }

        /// <summary>
        /// Function generates candidate matches between m_Features and m_baselineFeatures
        /// It does so by finding all pairs of MassTimeFeature that match within a provided
        /// mass tolerance window
        /// Elution time will be considered later
        /// </summary>
        public void GenerateCandidateMatches()
        {
            if (_features.Count == 0)
            {
                return;
            }

            _sectionUniqueFeatureIndices.Clear();
            _numFeaturesInSections.Clear();
            _numFeaturesInBaselineSections.Clear();
            _alignmentFunc.Clear();
            FeatureMatches.Clear();
            _subsectionMatchScores.Clear();
            _tempFeatureBestDelta.Clear();
            _tempFeatureBestIndex.Clear();

            _features.Sort(ByMass);
            _baselineFeatures.Sort(ByMass);

            // Go through each MassTimeFeature and see if the next baseline MassTimeFeature matches it
            var baselineFeatureIndex = 0;
            var numBaselineFeatures = _baselineFeatures.Count;

            if (numBaselineFeatures <= 0)
            {
                // No baseline to match it to.
                return;
            }

            for (var featureIndex = 0; featureIndex < _features.Count; featureIndex++)
            {
                var feature = _features[featureIndex];
                var massToleranceDa = feature.MassMonoisotopic * MassTolerance / 1000000;

                // Backtrack baselineFeatureIndex while the baseline feature's mass is greater than the candidate feature's mass minus massToleranceDa
                while (baselineFeatureIndex == numBaselineFeatures || baselineFeatureIndex >= 0 &&
                       (_baselineFeatures[baselineFeatureIndex].MassMonoisotopic > feature.MassMonoisotopic - massToleranceDa))
                {
                    baselineFeatureIndex--;
                }
                baselineFeatureIndex++;

                // Add candidate matches
                while (baselineFeatureIndex < numBaselineFeatures &&
                       (_baselineFeatures[baselineFeatureIndex].MassMonoisotopic <
                        (feature.MassMonoisotopic + massToleranceDa)))
                {
                    var baselineFeature = _baselineFeatures[baselineFeatureIndex];
                    if (baselineFeature.MassMonoisotopic >
                        (feature.MassMonoisotopic - massToleranceDa))
                    {
                        var matchToAdd = new LcmsWarpFeatureMatch
                        {
                            FeatureIndex = featureIndex,
                            FeatureIndex2 = baselineFeatureIndex,
                            Net = feature.Net,
                            Net2 = baselineFeature.Net
                        };

                        FeatureMatches.Add(matchToAdd);
                    }
                    baselineFeatureIndex++;
                }
            }

            // Now that matches have been created, go through all the matches and find a mapping
            // of how many times a baseline feature is matched to.
            // Store the matches in a map from a mass tag id to a list of indexes of feature matches

            var massTagToMatches = new Dictionary<int, List<int>>();
            for (var matchIndex = 0; matchIndex < FeatureMatches.Count; matchIndex++)
            {
                var featureMatch = FeatureMatches[matchIndex];
                var baselineIndex = featureMatch.FeatureIndex2;
                if (!massTagToMatches.ContainsKey(baselineIndex))
                {
                    massTagToMatches.Add(baselineIndex, new List<int>());
                }
                massTagToMatches[baselineIndex].Add(matchIndex);
            }

            // Now go through each of the baseline features matched and for each one keep at
            // most MaxPromiscuousUmcMatches (or none if KeepPromiscuousMatches is false)
            // keeping only the first MaxPromiscuousUmcMatches by scan

            var matchesToUse = new List<LcmsWarpFeatureMatch> {Capacity = FeatureMatches.Count};
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
                        matchesToUse.Add(FeatureMatches[matchIndex]);
                    }
                }
                else if (KeepPromiscuousMatches)
                {
                    // keep the matches that have the minimum scan numbers.
                    netMatchesToIndex.Clear();
                    for (var i = 0; i < numHits; i++)
                    {
                        var matchIndex = matchIterator.Value[i];
                        if (!netMatchesToIndex.ContainsKey(FeatureMatches[matchIndex].Net))
                        {
                            netMatchesToIndex.Add(FeatureMatches[matchIndex].Net, new List<int>());
                        }
                        netMatchesToIndex[FeatureMatches[matchIndex].Net].Add(matchIndex);
                    }

                    // Now, keep only the first MaxPromiscuousUmcMatches in the temp list
                    //var scanMatches = netMatchesToIndex.First();

                    for (var index = 0; index < MaxPromiscuousUmcMatches && index < netMatchesToIndex.Count; index++)
                    {
                        var matchIndex = netMatchesToIndex.ElementAt(index).Value[0];
                        matchesToUse.Add(FeatureMatches[matchIndex]);
                    }
                }
            }

            FeatureMatches = matchesToUse;
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
            var numMatches = FeatureMatches.Count;

            if (numMatches <= REQUIRED_MATCHES)
                return;

            var massDeltas = new List<double>(numMatches);
            var netDeltas = new List<double>(numMatches);
            for (var matchNum = 0; matchNum < numMatches; matchNum++)
            {
                var match = FeatureMatches[matchNum];
                var feature = _features[match.FeatureIndex];
                var baselineFeature = _baselineFeatures[match.FeatureIndex2];
                massDeltas.Add(((baselineFeature.MassMonoisotopic - feature.MassMonoisotopic) * 1000000) /
                                       feature.MassMonoisotopic);
                netDeltas.Add(baselineFeature.Net - feature.NetAligned);
            }
            MathUtilities.TwoDem(massDeltas, netDeltas, out _normalProb, out _u,
                out _muMass, out _muNet, out _massStd, out _netStd);
        }

        /// <summary>
        /// Method to set the flag for whether to use the Mass and the Net
        /// match scores with the LCMSWarper
        /// </summary>
        /// <param name="use"></param>
        public void UseMassAndNetScore(bool use)
        {
            _useMass = use;
        }

        /// <summary>
        /// Performs Mass error regression based on MZ of the match
        /// </summary>
        private void PerformMzMassErrorRegression()
        {
            //Copy all MZs and mass errors into a list of regression points
            var calibrations = new List<RegressionPoint>();

            for (var matchNum = 0; matchNum < FeatureMatches.Count; matchNum++)
            {
                var match = FeatureMatches[matchNum];
                var feature = _features[match.FeatureIndex];
                var baselineFeature = _baselineFeatures[match.FeatureIndex2];
                var ppm = (feature.MassMonoisotopic - baselineFeature.MassMonoisotopic) /
                          baselineFeature.MassMonoisotopic * 1000000;
                    //FeatureLight.ComputeMassPPMDifference(feature.MassMonoisotopic, baselineFeature.MassMonoisotopic);
                var netDiff = baselineFeature.Net - feature.NetAligned;

                calibrations.Add(new RegressionPoint(feature.Mz, 0, netDiff, ppm));
            }
            MzRecalibration.CalculateRegressionFunction(calibrations);

            for (var featureNum = 0; featureNum < _features.Count; featureNum++)
            {
                var feature = _features[featureNum];
                var mass = feature.MassMonoisotopic;
                var ppmShift = MzRecalibration.GetPredictedValue(feature.Mz);
                var newMass = mass - (mass * ppmShift) / 1000000;
                feature.MassMonoisotopicAligned = newMass;
                feature.MassMonoisotopic = newMass;
            }
        }

        /// <summary>
        /// Performs Mass error regression based on Scan of the match
        /// </summary>
        private void PerformScanMassErrorRegression()
        {
            var calibrations = new List<RegressionPoint>();

            for (var matchNum = 0; matchNum < FeatureMatches.Count; matchNum++)
            {
                var match = FeatureMatches[matchNum];
                var feature = _features[match.FeatureIndex];
                var baselineFeature = _baselineFeatures[match.FeatureIndex2];
                var ppm = (feature.MassMonoisotopic - baselineFeature.MassMonoisotopic) /
                          baselineFeature.MassMonoisotopic * 1000000;
                    //FeatureLight.ComputeMassPPMDifference(feature.MassMonoisotopic, baselineFeature.MassMonoisotopic);
                var netDiff = baselineFeature.Net - feature.NetAligned;

                calibrations.Add(new RegressionPoint(feature.Net, 0, netDiff, ppm));
            }

            NetRecalibration.CalculateRegressionFunction(calibrations);

            for (var featureNum = 0; featureNum < _features.Count; featureNum++)
            {
                var feature = _features[featureNum];
                var mass = feature.MassMonoisotopic;
                var ppmShift = MzRecalibration.GetPredictedValue(feature.Net);
                var newMass = mass - (mass * ppmShift) / 1000000;
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
            var numFeatures = _features.Count;

            _tempFeatureBestDelta.Clear();
            _tempFeatureBestDelta.Capacity = numFeatures;

            _tempFeatureBestIndex.Clear();
            _tempFeatureBestIndex.Capacity = numFeatures;

            //Clear the old features and initialize each section to 0
            _numFeaturesInSections.Clear();
            _numFeaturesInSections.Capacity = NumSections;
            for (var i = 0; i < NumSections; i++)
            {
                _numFeaturesInSections.Add(0);
            }

            // Do the same for baseline sections
            _numFeaturesInBaselineSections.Clear();
            _numFeaturesInBaselineSections.Capacity = numFeatures;
            for (var i = 0; i < NumBaselineSections; i++)
            {
                _numFeaturesInBaselineSections.Add(0);
            }

            MaxNet = double.MinValue;
            MinNet = double.MaxValue;
            for (var i = 0; i < numFeatures; i++)
            {
                MaxNet = Math.Max(MaxNet, _features[i].Net);
                MinNet = Math.Min(MinNet, _features[i].Net);
                _tempFeatureBestDelta.Add(double.MaxValue);
                _tempFeatureBestIndex.Add(-1);
            }

            for (var i = 0; i < _features.Count; i++)
            {
                var net = _features[i].Net;
                var sectionNum = Convert.ToInt32(((net - MinNet) * NumSections) / (MaxNet - MinNet));

                if (sectionNum >= NumSections)
                {
                    sectionNum = NumSections - 1;
                }
                if (sectionNum < 0)
                {
                    sectionNum = 0;
                }
                _numFeaturesInSections[sectionNum]++;
            }

            MinBaselineNet = double.MaxValue;
            MaxBaselineNet = double.MinValue;

            var numBaselineFeatures = _baselineFeatures.Count;

            //MinBaselineNet = m_baselineFeatures.Min(x => x.Net);
            //MaxBaselineNet = m_baselineFeatures.Max(x => x.Net);

            for (var i = 0; i < numBaselineFeatures; i++)
            {
                MinBaselineNet = Math.Min(MinBaselineNet, _baselineFeatures[i].Net);
                MaxBaselineNet = Math.Max(MaxBaselineNet, _baselineFeatures[i].Net);
            }

            for (var i = 0; i < numBaselineFeatures; i++)
            {
                var net = _baselineFeatures[i].Net;
                var msmsSectionNum =
                    (int) (((net - MinBaselineNet) * NumBaselineSections) / (MaxBaselineNet - MinBaselineNet));
                if (msmsSectionNum == NumBaselineSections)
                {
                    msmsSectionNum--;
                }
                _numFeaturesInBaselineSections[msmsSectionNum]++;
            }

            FeatureMatches = FeatureMatches.OrderBy(x => x.Net).ToList();
            var numMatches = FeatureMatches.Count;

            var sectionFeatures = new List<LcmsWarpFeatureMatch>();

            var numSectionMatches = NumSections * NumMatchesPerSection;
            _subsectionMatchScores.Clear();
            _subsectionMatchScores.Capacity = numSectionMatches;

            for (var i = 0; i < numSectionMatches; i++)
            {
                _subsectionMatchScores.Add(MinScore);
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

                while (startMatchIndex < numMatches && ((FeatureMatches[startMatchIndex].Net) < sectionStartNet))
                {
                    startMatchIndex++;
                }
                var endMatchIndex = startMatchIndex;
                while (endMatchIndex < numMatches && ((FeatureMatches[endMatchIndex].Net) < sectionEndNet))
                {
                    endMatchIndex++;
                }
                if (endMatchIndex != startMatchIndex)
                {
                    for (var index = startMatchIndex; index < endMatchIndex; index++)
                    {
                        sectionFeatures.Add(FeatureMatches[index]);
                    }
                }

                ComputeSectionMatch(section, sectionFeatures, sectionStartNet, sectionEndNet);

                var percentComplete = section / (double) NumSections * 100;
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
                    var alignmentIndex = (section * NumMatchesPerSection) + (baselineSection * NumMatchesPerBaseline) +
                                         sectionWidth;
                    var alignmentScore = _alignmentScore[alignmentIndex];

                    if (!(alignmentScore > bestScore))
                        continue;

                    bestScore = alignmentScore;
                    bestPreviousAlignmentIndex = _bestPreviousIndex[alignmentIndex];
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
                baselineSectionEnd, bestScore, _subsectionMatchScores[bestAlignmentIndex]);
            _alignmentFunc.Clear();
            _alignmentFunc.Add(match);

            while (bestPreviousAlignmentIndex >= 0)
            {
                var sectionStart = (bestPreviousAlignmentIndex / NumMatchesPerSection); // should be current - 1
                var sectionEnd = sectionStart + 1;

                var nextNetStart = MinNet + (sectionStart * (MaxNet - MinNet)) / NumSections;
                var nextNetEnd = MinNet + (sectionEnd * (MaxNet - MinNet)) / NumSections;

                var nextBaselineSectionStart = (bestPreviousAlignmentIndex - (sectionStart * NumMatchesPerSection)) /
                                               NumMatchesPerBaseline;
                var nextBaselineSectionEnd = nextBaselineSectionStart +
                                             (bestPreviousAlignmentIndex % NumMatchesPerBaseline) + 1;
                var nextBaselineStartNet = (nextBaselineSectionStart * msmsSectionWidth) + MinBaselineNet;
                var nextBaselineEndNet = nextBaselineSectionEnd * msmsSectionWidth + MinBaselineNet;
                match = new LcmsWarpAlignmentMatch();
                match.Set(nextNetStart, nextNetEnd, sectionStart, sectionEnd, nextBaselineStartNet, nextBaselineEndNet,
                    nextBaselineSectionStart, nextBaselineSectionEnd, _alignmentScore[bestPreviousAlignmentIndex],
                    _subsectionMatchScores[bestPreviousAlignmentIndex]);

                bestPreviousAlignmentIndex = _bestPreviousIndex[bestPreviousAlignmentIndex];
                _alignmentFunc.Add(match);
            }
            _alignmentFunc.Sort();
        }

        /// <summary>
        /// Computes the alignment matrix for all possible alignments, holding
        /// onto each and every possible alignment score and then linking the feature
        /// to the best previous index of possible alignments
        /// </summary>
        public void CalculateAlignmentMatrix()
        {
            var numPossibleAlignments = NumSections * NumBaselineSections * NumMatchesPerBaseline;

            _alignmentScore = new double[numPossibleAlignments];
            _bestPreviousIndex = new int[numPossibleAlignments];

            // Initialize scores to - inf, best previous index to -1
            for (var i = 0; i < numPossibleAlignments; i++)
            {
                _alignmentScore[i] = double.MinValue;
                _bestPreviousIndex[i] = -1;
            }
            var log2PiStdNetStdNet = Math.Log(2 * Math.PI * _netStd * _netStd);

            var unmatchedScore = -0.5 * log2PiStdNetStdNet;
            if (NetTolerance < 3 * _netStd)
            {
                unmatchedScore -= (0.5 * 9.0);
            }
            else
            {
                unmatchedScore -= (0.5 * (NetTolerance * NetTolerance) / (_netStd * _netStd));
            }
            if (_useMass)
            {
                //Assumes that for the unmatched, the masses were also off at mass tolerance, so use the same threshold from NET
                unmatchedScore *= 2;
            }

            for (var baselineSection = 0; baselineSection < NumBaselineSections; baselineSection++)
            {
                //Assume everything that was matched was past 3 standard devs in net.
                for (var sectionWidth = 0; sectionWidth < NumMatchesPerBaseline; sectionWidth++)
                {
                    //no need to mulitply with msSection because its 0
                    _alignmentScore[baselineSection * NumMatchesPerBaseline + sectionWidth] = 0;
                }
            }

            var numUnmatchedMsFeatures = 0;
            for (var section = 0; section < NumSections; section++)
            {
                for (var sectionWidth = 0; sectionWidth < NumMatchesPerBaseline; sectionWidth++)
                {
                    var alignmentIndex = section * NumMatchesPerSection + sectionWidth;
                    _alignmentScore[alignmentIndex] = _subsectionMatchScores[alignmentIndex] +
                                                      unmatchedScore * numUnmatchedMsFeatures;
                }
                numUnmatchedMsFeatures += _numFeaturesInSections[section];
            }

            for (var section = 1; section < NumSections; section++)
            {
                for (var baselineSection = 1; baselineSection < NumBaselineSections; baselineSection++)
                {
                    for (var sectionWidth = 0; sectionWidth < NumMatchesPerBaseline; sectionWidth++)
                    {
                        var alignmentIndex = section * NumMatchesPerSection + baselineSection * NumMatchesPerBaseline +
                                             sectionWidth;

                        var currentBestScore = double.MinValue;
                        var bestPreviousAlignmentIndex = 0;

                        for (var previousBaselineSection = baselineSection - 1;
                            previousBaselineSection >= baselineSection - NumMatchesPerBaseline - MaxJump;
                            previousBaselineSection--)
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
                            var previousAlignmentIndex = (section - 1) * NumMatchesPerSection +
                                                         previousBaselineSection * NumMatchesPerBaseline +
                                                         previousBaselineSectionWidth - 1;
                            if (!(_alignmentScore[previousAlignmentIndex] > currentBestScore))
                                continue;

                            currentBestScore = _alignmentScore[previousAlignmentIndex];
                            bestPreviousAlignmentIndex = previousAlignmentIndex;
                        }
                        if (Math.Abs(currentBestScore - double.MinValue) > double.Epsilon)
                        {
                            _alignmentScore[alignmentIndex] = currentBestScore + _subsectionMatchScores[alignmentIndex];
                            _bestPreviousIndex[alignmentIndex] = bestPreviousAlignmentIndex;
                        }
                        else
                        {
                            _alignmentScore[alignmentIndex] = double.MinValue;
                        }
                    }
                }
            }
        }

        private void ComputeSectionMatch(int msSection, List<LcmsWarpFeatureMatch> sectionMatchingFeatures,
            double minNet, double maxNet)
        {
            var numMatchingFeatures = sectionMatchingFeatures.Count;
            var baselineSectionWidth = (MaxBaselineNet - MinBaselineNet) / NumBaselineSections;

            // keep track of only the unique indices of ms features because we only want the best matches for each
            _sectionUniqueFeatureIndices.Clear();
            _sectionUniqueFeatureIndices.Capacity = numMatchingFeatures;
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
                    _sectionUniqueFeatureIndices.Add(match.FeatureIndex);
                }
            }

            var numUniqueFeaturesStart = _sectionUniqueFeatureIndices.Count;

            for (var baselineSectionStart = 0; baselineSectionStart < NumBaselineSections; baselineSectionStart++)
            {
                var baselineStartNet = MinBaselineNet + baselineSectionStart * baselineSectionWidth;
                var endSection = baselineSectionStart + NumMatchesPerBaseline;
                if (endSection >= NumBaselineSections)
                {
                    endSection = NumBaselineSections;
                }
                for (var baselineSectionEnd = baselineSectionStart;
                    baselineSectionEnd < endSection;
                    baselineSectionEnd++)
                {
                    var numUniqueFeatures = numUniqueFeaturesStart;

                    var sectionIndex = msSection * NumMatchesPerSection + baselineSectionStart * NumMatchesPerBaseline
                                       + (baselineSectionEnd - baselineSectionStart);
                    var baselineEndNet = MinBaselineNet + (baselineSectionEnd + 1) * baselineSectionWidth;

                    for (var i = 0; i < numUniqueFeatures; i++)
                    {
                        var msFeatureIndex = _sectionUniqueFeatureIndices[i];
                        _tempFeatureBestDelta[msFeatureIndex] = double.MaxValue;
                        _tempFeatureBestIndex[msFeatureIndex] = -1;
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
                        if (!(Math.Abs(deltaMatch) < Math.Abs(_tempFeatureBestDelta[msFeatureIndex])))
                            continue;

                        _tempFeatureBestDelta[msFeatureIndex] = deltaMatch;
                        _tempFeatureBestIndex[msFeatureIndex] = match.FeatureIndex2;
                    }

                    _subsectionMatchScores[sectionIndex] = CurrentlyStoredSectionMatchScore(numUniqueFeatures);
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
            if (CalibrationType == LcmsWarpCalibrationType.MzRegression ||
                CalibrationType == LcmsWarpCalibrationType.Both)
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
            if (CalibrationType == LcmsWarpCalibrationType.ScanRegression ||
                CalibrationType == LcmsWarpCalibrationType.Both)
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
        /// <param name="baselineVals"></param>
        /// <param name="standardize"></param>
        public void GetSubsectionMatchScore(out double[,] subsectionMatchScores, out double[] aligneeVals,
            out double[] baselineVals, bool standardize)
        {
            subsectionMatchScores = new double[NumSections, NumBaselineSections];
            aligneeVals = new double[NumSections];
            baselineVals = new double[NumBaselineSections];
            for (var aligneeSection = 0; aligneeSection < NumSections; aligneeSection++)
            {
                aligneeVals[aligneeSection] = MinNet + (aligneeSection * (MaxNet - MinNet)) / NumSections;
            }
            for (var baselineSection = 0; baselineSection < NumBaselineSections; baselineSection++)
            {
                baselineVals[baselineSection] = MinBaselineNet + (baselineSection * (MaxBaselineNet - MinBaselineNet)) / NumBaselineSections;
            }

            for (var aligneeSection = 0; aligneeSection < NumSections; aligneeSection++)
            {
                for (var baselineSection = 0; baselineSection < NumBaselineSections; baselineSection++)
                {
                    var maxScore = double.MinValue;
                    for (var baselineSectionWidth = 0; baselineSectionWidth < NumMatchesPerBaseline; baselineSectionWidth++)
                    {
                        if (baselineSection + baselineSectionWidth >= NumBaselineSections)
                        {
                            continue;
                        }
                        var matchIndex = (aligneeSection * NumMatchesPerSection) + (baselineSection * NumMatchesPerBaseline) +
                                         baselineSectionWidth;
                        if (_subsectionMatchScores[matchIndex] > maxScore)
                        {
                            maxScore = _subsectionMatchScores[matchIndex];
                        }
                    }
                    subsectionMatchScores[aligneeSection, baselineSection] = maxScore;
                }
            }

            if (standardize)
            {
                StandardizeSubsectionMatchScore(subsectionMatchScores);
            }
        }

        /// <summary>
        /// Standardizes the match scores of the subsection
        /// </summary>
        /// <param name="subsectionMatchScores"></param>
        private void StandardizeSubsectionMatchScore(double[,] subsectionMatchScores)
        {
            for (var aligneeSection = 0; aligneeSection < NumSections; aligneeSection++)
            {
                double sumX = 0, sumXx = 0;
                var realMinScore = double.MaxValue;
                var numPoints = 0;
                for (var baselineSection = 0; baselineSection < NumBaselineSections; baselineSection++)
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

                for (var baselineSection = 0; baselineSection < NumBaselineSections; baselineSection++)
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
                        subsectionMatchScores[aligneeSection, baselineSection] = ((score - avg) / stDev);
                    }
                }
            }
        }

        public void GetErrorHistograms(double massBin, double netBin, double driftBin,
            out Dictionary<double, int> massErrorHist,
            out Dictionary<double, int> netErrorHist, out Dictionary<double, int> driftErrorHist)
        {
            var numMatches = FeatureMatches.Count;
            var massErrors = new List<double> {Capacity = numMatches};
            var netErrors = new List<double> {Capacity = numMatches};
            var driftErrors = new List<double> {Capacity = numMatches};

            foreach (var match in FeatureMatches)
            {
                massErrors.Add(match.PpmMassError);
                netErrors.Add(match.NetError);
                driftErrors.Add(match.DriftError);
            }

            massErrorHist = Histogram.CreateHistogram(massErrors, massBin);
            netErrorHist = Histogram.CreateHistogram(netErrors, netBin);
            driftErrorHist = Histogram.CreateHistogram(driftErrors, driftBin);
        }
    }
}