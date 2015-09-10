using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Class which will use LCMSWarp to process alignment
    /// </summary>
    public sealed class LcmsWarpAlignmentProcessor:
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, LcmsWarpAlignmentData>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, LcmsWarpAlignmentData>
    {

        private enum CurrentLcmsWarpTask
        {
            Unstarted,
            GenerateCandidateMatches,
            GetMatchProbabilities,
            CalculateAlignmentMatrix,
            CalculateAlignmentFunction,
            GetTransformedNets,
            CalculateAlignmentMatches,
            Complete
        }


        // In case alignment was to MS features, this will keep track of the minimum scan in the
        // reference features. They are needed because LCMSWarp uses NET values for reference and
        // are scaled to between 0 and 1. These will scale it back to actual scan numbers
        int m_minReferenceDatasetScan;
        int m_maxReferenceDatasetScan;

        int m_minAligneeDatasetScan;
        int m_maxAligneeDatasetScan;
        double m_minAligneeDatasetMz;
        double m_maxAligneeDatasetMz;

        // LCMSWarp instance that will do the alignment when processing
        LcmsWarp m_lcmsWarp;

        /// <summary>
        /// Alignment options
        /// </summary>
        private LcmsWarpAlignmentOptions m_Options;

        private readonly Dictionary<CurrentLcmsWarpTask, double> mCurrentTaskPercentCompleteAtStart;
        private double mPercentCompleteAtStartOfTask;
        private double mPercentCompleteAtEndOfTask;

        /// <summary>
        /// Most recent progress message
        /// </summary>
        private string m_LastProgressMessage;

        #region Public properties
        /// <summary>
        /// Get property for the NET Intercept that LCMS Warp is holding
        /// Simulates a pure linear regression
        /// </summary>
        public double NetIntercept
        {
            get { return m_lcmsWarp.NetIntercept; }
        }
        /// <summary>
        /// Get property for the NET RSquared that LCMS Warp is holding
        /// Simulates a pure linear regression
        /// </summary>
        public double NetRsquared
        {
            get { return m_lcmsWarp.NetLinearRsq; }
        }
        /// <summary>
        /// Get property for the NET Slope that LCMS Warp is holding
        /// Simulates a pure linear regression
        /// </summary>
        public double NetSlope
        {
            get { return m_lcmsWarp.NetSlope; }
        }

        /// <summary>
        /// Options for the Alignment processor
        /// </summary>
        public LcmsWarpAlignmentOptions Options
        {
            get { 
                return m_Options; 
            }
            set { 
                m_Options = value;
                ApplyAlignmentOptions();
            }
        }

        /// <summary>
        /// Flag for if the Processor is aligning to a Mass Tag Database
        /// </summary>
        private bool AligningToMassTagDb { get; set; }                
        #endregion

        /// <summary>
        /// Public constructor for the LCMS Alignment Processor
        /// Initializes a new LCMSWarp object using the LCMS Alignment options
        /// which were passed into the Processor
        /// </summary>
        public LcmsWarpAlignmentProcessor()
        {
            m_lcmsWarp  = new LcmsWarp();
            Options     = new LcmsWarpAlignmentOptions();

            m_lcmsWarp.Progress += lcmsWarp_Progress;

            mCurrentTaskPercentCompleteAtStart = new Dictionary<CurrentLcmsWarpTask, double>
            {
                {CurrentLcmsWarpTask.Unstarted, 0},
                {CurrentLcmsWarpTask.GenerateCandidateMatches, 0},
                {CurrentLcmsWarpTask.GetMatchProbabilities, 10},
                {CurrentLcmsWarpTask.CalculateAlignmentMatrix, 30},
                {CurrentLcmsWarpTask.CalculateAlignmentFunction, 50},
                {CurrentLcmsWarpTask.GetTransformedNets, 70},
                {CurrentLcmsWarpTask.CalculateAlignmentMatches, 90},
                {CurrentLcmsWarpTask.Complete, 100}
            };

            mPercentCompleteAtStartOfTask = 0;
            mPercentCompleteAtEndOfTask = 100;
            
            AligningToMassTagDb = false;
        }

        // Applies the alignment options to the LCMSWarper, setting the Mass
        // and NET Tolerances, the options for NET Alignment options for 
        // Mass calibration, the Least Squares options and the calibration type
        public void ApplyAlignmentOptions()
        {
            // Applying the Mass and NET Tolerances
            m_lcmsWarp.MassTolerance = Options.MassTolerance;
            m_lcmsWarp.NetTolerance = Options.NetTolerance;

            // Applying options for NET Calibration
            m_lcmsWarp.NumSections = Options.NumTimeSections;            
            m_lcmsWarp.MaxJump = Options.MaxTimeDistortion;
            m_lcmsWarp.NumBaselineSections = Options.NumTimeSections * Options.ContractionFactor;
            m_lcmsWarp.NumMatchesPerBaseline = Options.ContractionFactor*Options.ContractionFactor;
            m_lcmsWarp.NumMatchesPerSection = m_lcmsWarp.NumBaselineSections * m_lcmsWarp.NumMatchesPerBaseline;

            m_lcmsWarp.KeepPromiscuousMatches = Options.UsePromiscuousPoints;
            m_lcmsWarp.MaxPromiscuousUmcMatches = Options.MaxPromiscuity;

            // Applying options for Mass Calibration
            m_lcmsWarp.MassCalibrationWindow = Options.MassCalibrationWindow;
            m_lcmsWarp.MassCalNumDeltaBins = Options.MassCalibNumYSlices;
            m_lcmsWarp.MassCalNumSlices = Options.MassCalibNumXSlices;
            m_lcmsWarp.MassCalNumJump = Options.MassCalibMaxJump;

            var regType = LcmsWarpRegressionType.Central;

            if (Options.MassCalibUseLsq)
            {
                regType = LcmsWarpRegressionType.Hybrid;
            }
            m_lcmsWarp.MzRecalibration.SetCentralRegressionOptions(m_lcmsWarp.MassCalNumSlices, m_lcmsWarp.MassCalNumDeltaBins,
                                                                   m_lcmsWarp.MassCalNumJump, Options.MassCalibMaxZScore,
                                                                   regType);
            m_lcmsWarp.NetRecalibration.SetCentralRegressionOptions(m_lcmsWarp.MassCalNumSlices, m_lcmsWarp.MassCalNumDeltaBins,
                                                                    m_lcmsWarp.MassCalNumJump, Options.MassCalibMaxZScore,
                                                                    regType);

            // Applying LSQ options
            m_lcmsWarp.MzRecalibration.SetLsqOptions(Options.MassCalibLsqNumKnots, Options.MassCalibLsqMaxZScore);
            m_lcmsWarp.NetRecalibration.SetLsqOptions(Options.MassCalibLsqNumKnots, Options.MassCalibLsqMaxZScore);

            // Setting the calibration type
            m_lcmsWarp.CalibrationType = Options.CalibrationType;
        }

        /// <summary>
        /// Takes a List of UMCLight data and applies the NET/Mass Function to the dataset and
        /// aligns it to the baseline. Updates the data in place for the calibrated Masses and
        /// Alignment. 
        /// </summary>
        /// <param name="data"></param>
        public void ApplyNetMassFunctionToAligneeDatasetFeatures(ref List<UMCLight> data)
        {
            if (m_lcmsWarp == null)
            {
                m_lcmsWarp = new LcmsWarp();
            }

            var umcIndices = new List<int>();
            var umcCalibratedMasses = new List<double>();
            var umcAlignedNets = new List<double>();
            var umcAlignedScans = new List<int>();
            var umcDriftTimes = new List<double>();

            if (AligningToMassTagDb)
            {
                m_lcmsWarp.GetFeatureCalibratedMassesAndAlignedNets(ref umcIndices, ref umcCalibratedMasses,
                                                                    ref umcAlignedNets, ref umcDriftTimes);

                for (var i = 0; i < umcIndices.Count; i++)
                {
                    var point = new UMCLight
                    {
                        Id = umcIndices[i],
                        MassMonoisotopicAligned = umcCalibratedMasses[i],
                        NetAligned = umcAlignedNets[i],
                        DriftTime = umcDriftTimes[i]
                    };

                    if (i < data.Count)
                    {
                        // Update the data without stomping the data that shouldn't change.
                        data[i].MassMonoisotopicAligned = point.MassMonoisotopicAligned;
                        data[i].NetAligned = point.NetAligned;
                        data[i].DriftTime = point.DriftTime;
                    }
                    else
                    {
                        data.Add(point);
                    }
                }

            }
            else
            {
                m_lcmsWarp.GetFeatureCalibratedMassesAndAlignedNets(ref umcIndices, ref umcCalibratedMasses,
                                                                    ref umcAlignedNets, ref umcAlignedScans,
                                                                    ref umcDriftTimes, m_minReferenceDatasetScan,
                                                                    m_maxReferenceDatasetScan);

                for (var i = 0; i < umcIndices.Count; i++)
                {
                    var point = new UMCLight
                    {
                        Id = umcIndices[i],
                        MassMonoisotopicAligned = umcCalibratedMasses[i],
                        NetAligned = umcAlignedNets[i],
                        ScanAligned = umcAlignedScans[i],
                        DriftTime = umcDriftTimes[i]
                    };

                    if (i < data.Count)
                    {
                        // Update the data without stomping the data that shouldn't change.
                        data[i].MassMonoisotopicAligned = point.MassMonoisotopicAligned;
                        data[i].NetAligned = point.NetAligned;
                        data[i].ScanAligned = point.ScanAligned;
                        data[i].DriftTime = point.DriftTime;
                    }
                    else
                    {
                        data.Add(point);
                    }
                }
            }
        }
        
        /// <summary>
        /// For a given List of UMCLights, warp the alignee features to the baseline.
        /// </summary>
        /// <param name="features"></param>
        public void SetAligneeDatasetFeatures(List<UMCLight> features)
        {
            var numPts = features.Count;

            var mtFeatures = new List<UMCLight> { Capacity = numPts };

            m_minAligneeDatasetScan = int.MaxValue;
            m_maxAligneeDatasetScan = int.MinValue;
            m_minAligneeDatasetMz = double.MaxValue;
            m_maxAligneeDatasetMz = double.MinValue;


            for (var index = 0; index < numPts; index++)
            {
                // Note: We are using ScanStart for the NET of the feature
                // This is to avoid odd effects from broad or tailing LC-MS features

                var mtFeature = new UMCLight
                {
                    MassMonoisotopic = features[index].MassMonoisotopic,
                    MassMonoisotopicAligned = features[index].MassMonoisotopicAligned,
                    Net = Convert.ToDouble(features[index].ScanStart),
                    Mz = features[index].Mz,
                    Abundance = features[index].Abundance,
                    Id = features[index].Id,
                    DriftTime = features[index].DriftTime
                };

                //mtFeature.MonoMassOriginal = features[index].MassMonoisotopic;

                // For if we want to split alignment at given M/Z range

                // Only allow feature to be aligned if we're splitting the alignment in MZ
                // AND if we are within the specified boundary
                //if (ValidateAlignmentBoundary(Options.AlignSplitMZs, mtFeature.Mz, boundary))
                //{
                    mtFeatures.Add(mtFeature);

                    if (features[index].Scan > m_maxAligneeDatasetScan)
                    {
                        m_maxAligneeDatasetScan = features[index].Scan;
                    }
                    if (features[index].Scan < m_minAligneeDatasetScan)
                    {
                        m_minAligneeDatasetScan = features[index].Scan;
                    }
                    if (features[index].Mz > m_maxAligneeDatasetMz)
                    {
                        m_maxAligneeDatasetMz = features[index].Mz;
                    }
                    if (features[index].Mz < m_minAligneeDatasetMz)
                    {
                        m_minAligneeDatasetMz = features[index].Mz;
                    }
                //}
            }
            m_lcmsWarp.SetFeatures(ref mtFeatures);
        }
        
        /// <summary>
        /// 
        /// Use the NET value of the UMCs in the List as the value to align to, the predictor variable
        /// </summary>
        /// <param name="umcData"></param>
        public void SetReferenceDatasetFeatures(List<UMCLight> umcData)
        {
            AligningToMassTagDb = false;

            var numPts = umcData.Count;

            var mtFeatures = new List<UMCLight> { Capacity = numPts };

            m_minAligneeDatasetScan = int.MaxValue;
            m_maxAligneeDatasetScan = int.MinValue;

            for (var index = 0; index < numPts; index++)
            {
                var feature = new UMCLight();
                var data = umcData[index];                
                feature.MassMonoisotopic = data.MassMonoisotopic;
                feature.MassMonoisotopicAligned = data.MassMonoisotopicAligned;
                feature.Net = data.Net;
                feature.Mz = data.Mz;
                feature.Abundance = data.Abundance;
                feature.DriftTime = data.DriftTime;
                feature.Id = data.Id;

                mtFeatures.Add(feature);

                if (data.Scan > m_maxReferenceDatasetScan)
                {
                    m_maxReferenceDatasetScan = data.Scan;
                }
                if (data.Scan < m_minReferenceDatasetScan)
                {
                    m_minReferenceDatasetScan = data.Scan;
                }
                if (data.Mz > m_maxAligneeDatasetMz)
                {
                    m_maxAligneeDatasetMz = data.Mz;
                }
                if (data.Mz < m_minAligneeDatasetMz)
                {
                    m_minAligneeDatasetMz = data.Mz;
                }
            }
            m_lcmsWarp.SetReferenceFeatures(ref mtFeatures);
        }

        /// <summary>
        /// Sets alignment features for a MSFeature dataset from a database
        /// </summary>
        /// <param name="features"></param>
        public void SetReferenceDatasetFeatures(List<MassTagLight> features)
        {
            AligningToMassTagDb = true;
            var numMassTags = features.Count;

            var mtFeatures = new List<UMCLight> { Capacity = numMassTags };
            mtFeatures.AddRange(features.Select(item => new UMCLight
            {
                NetAligned = item.NetAligned, 
                MassMonoisotopic = item.MassMonoisotopic, 
                MassMonoisotopicAligned = item.MassMonoisotopicAligned, 
                Mz = item.MassMonoisotopic/item.ChargeState + (1.00782*(item.ChargeState - 1)), 
                Net = item.Net, 
                DriftTime = item.DriftTime, 
                Id = item.Id,
            }));

            m_lcmsWarp.SetReferenceFeatures(ref mtFeatures);
        }

        /// <summary>
        /// Returns the Alignment Function that the processor determined
        /// </summary>
        /// <returns></returns>
        public LcmsWarpAlignmentFunction GetAlignmentFunction()
        {
            var func = new LcmsWarpAlignmentFunction(Options.CalibrationType, Options.AlignType);

            var aligneeNets = new List<double>();
            var referenceNets = new List<double>();
            m_lcmsWarp.AlignmentFunction(ref aligneeNets, ref referenceNets);

            if (AligningToMassTagDb)
            {
                func.SetNetFunction(ref aligneeNets, ref referenceNets);
            }
            else
            {
                var referenceScans = new List<double>();
                var numSections = referenceNets.Count;
                for (var sectionNum = 0; sectionNum < numSections; sectionNum++)
                {
                    referenceScans.Add(m_minReferenceDatasetScan + referenceNets[sectionNum] * (m_maxReferenceDatasetScan - m_minReferenceDatasetScan));
                }
                func.SetNetFunction(ref aligneeNets, ref referenceNets, ref referenceScans);
            }

            if (Options.AlignType == AlignmentType.NET_WARP)
            {
                return func;
            }

            var minAligneeNet = m_lcmsWarp.MinNet;
            var maxAligneeNet = m_lcmsWarp.MaxNet;

            // Get the mass calibration function with time
            var numXKnots = Options.MassCalibNumXSlices;
            var aligneeMzMassFunc = new List<double>();
            var aligneeNetMassFunc = new List<double>();
            var aligneePpmShiftMassFunc = new List<double>();

            if (Options.CalibrationType == LcmsWarpCalibrationType.ScanRegression ||
                Options.CalibrationType == LcmsWarpCalibrationType.Both)
            {
                // get the PPM for each knot
                for (var knotNum = 0; knotNum < numXKnots; knotNum++)
                {
                    var net = minAligneeNet + ((maxAligneeNet - minAligneeNet) * knotNum) / numXKnots;
                    var ppm = m_lcmsWarp.GetPpmShiftFromNet(net);
                    aligneeNetMassFunc.Add(net);
                    aligneePpmShiftMassFunc.Add(ppm);
                }
                func.SetMassCalibrationFunctionWithTime(ref aligneeNetMassFunc, ref aligneePpmShiftMassFunc);
            }

            if (Options.CalibrationType != LcmsWarpCalibrationType.MzRegression &&
                Options.CalibrationType != LcmsWarpCalibrationType.Both) return func;


            // Get the ppm for each knot
            for (var knotNum = 0; knotNum < numXKnots; knotNum++)
            {
                var net = knotNum * 1.0 / numXKnots;
                var mz = m_minAligneeDatasetMz + (int)((m_maxAligneeDatasetMz - m_minAligneeDatasetMz) * net);
                var ppm = m_lcmsWarp.GetPpmShiftFromMz(mz);
                aligneeMzMassFunc.Add(mz);
                aligneePpmShiftMassFunc.Add(ppm);
            }
            func.SetMassCalibrationFunctionWithMz(aligneeMzMassFunc, aligneePpmShiftMassFunc);
            return func;
        }

        /// <summary>
        /// Method to determine which warping method to use.
        /// Throws exception if the options were not set.
        /// </summary>
        public void PerformAlignmentToMsFeatures()
        {
            if (Options == null)
            {
                throw new NullReferenceException("Alignment Options were not set in AlignmentProcessor");
            }

            if (Options.AlignType != AlignmentType.NET_MASS_WARP)
            {
                PerformNetWarp(0, 100);
            }
            else
            {
                PerformNetMassWarp();
            }
        }
        
        /// <summary>
        /// Performs the NET Warping; Generates matches, gets the probabilities,
        /// calculates the alignment matrix and alignment function, gets the transformed NETs
        /// and then calculates the alignment matches
        /// </summary>
        private void PerformNetWarp(double percentCompleteAtStart, double percentCompleteAtEnd)
        {

            var percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.GenerateCandidateMatches);
            OnProgress("NET Warp, get candidate matches", percentCompleteOverall);
            m_lcmsWarp.GenerateCandidateMatches();

            if (m_lcmsWarp.NumCandidateMatches < 10)
            {
                throw new ApplicationException("Insufficient number of candidate matches by mass alone");
            }

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.GetMatchProbabilities);
            OnProgress("NET Warp, get match probabilities", percentCompleteOverall);
            m_lcmsWarp.GetMatchProbabilities();

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.CalculateAlignmentMatrix);
            OnProgress("NET Warp, calculate alignment matrix", percentCompleteOverall);
            m_lcmsWarp.CalculateAlignmentMatrix();

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.CalculateAlignmentFunction);
            OnProgress("NET Warp, calculate alignment function", percentCompleteOverall);
            m_lcmsWarp.CalculateAlignmentFunction();

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.GetTransformedNets);
            OnProgress("NET Warp, get transformed NETs", percentCompleteOverall);
            m_lcmsWarp.GetTransformedNets();

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.CalculateAlignmentMatches);
            OnProgress("NET Warp, calculate alignment matches", percentCompleteOverall);
            m_lcmsWarp.CalculateAlignmentMatches();

            UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.Complete);
        }

        /// <summary>
        /// Compute the effective Percent Complete value given a starting and ending percent complete, plus the percent complete of a subtask
        /// </summary>
        /// <param name="percentCompleteAtStart"></param>
        /// <param name="percentCompleteAtEnd"></param>
        /// <param name="subtaskPercentComplete"></param>
        /// <returns></returns>
        private double ComputeIncrementalProgress(double percentCompleteAtStart, double percentCompleteAtEnd, double subtaskPercentComplete)
        {
            return percentCompleteAtStart + (percentCompleteAtEnd - percentCompleteAtStart) * subtaskPercentComplete / 100;
        }

        /// <summary>
        /// Updates mCurrentTask
        /// </summary>
        /// <param name="percentCompleteAtStart"></param>
        /// <param name="percentCompleteAtEnd"></param>
        /// <param name="currentTask"></param>
        /// <returns>Effective percent complete overall</returns>
        private double UpdateCurrentTask(double percentCompleteAtStart, double percentCompleteAtEnd, CurrentLcmsWarpTask currentTask)
        {
            mPercentCompleteAtStartOfTask = ComputeIncrementalProgress(percentCompleteAtStart, percentCompleteAtEnd, mCurrentTaskPercentCompleteAtStart[currentTask]);
            
            if (!mCurrentTaskPercentCompleteAtStart.TryGetValue(currentTask + 1, out mPercentCompleteAtEndOfTask))
                mPercentCompleteAtEndOfTask = mPercentCompleteAtStartOfTask;

            return mPercentCompleteAtStartOfTask;
        }

        /// <summary>
        /// Performs the NET-Mass Warping; Sets up the mass calibration settings from the options,
        /// performs NET warping, calibrates matches based on the NETWarp results, recalibrates the mass
        /// tolerance and then performs Warping again using the mass and Net scores
        /// </summary>
        private void PerformNetMassWarp()
        {
            OnProgress("LCMSWarp phase one", 0);

            // First, perform the net calibration using a mass tolerance of the same size as the mass window
            // and then perform the net calibration again using the appropriate mass tolerance
            var massTolerance = m_lcmsWarp.MassTolerance;
            m_lcmsWarp.MassTolerance = m_lcmsWarp.MassCalibrationWindow;
            m_lcmsWarp.UseMassAndNetScore(false);
            
            PerformNetWarp(0, 50);

            OnProgress("Calibrating mass", 50);

            m_lcmsWarp.PerformMassCalibration();             
            m_lcmsWarp.CalculateStandardDeviations();

            OnProgress("LCMSWarp phase two", 60);

            m_lcmsWarp.MassTolerance = massTolerance;
            m_lcmsWarp.UseMassAndNetScore(true);

            PerformNetWarp(60, 100);            

            OnProgress("Complete", 100);
        }

        /// <summary>
        /// Method to return the heatmap of the alignment (as a 2D array of doubles) based on
        /// the output scores 
        /// </summary>
        /// <param name="outputScores"></param>
        /// <param name="xIntervals"></param>
        /// <param name="yIntervals"></param>
        public void GetAlignmentHeatMap(out double[,] outputScores, out double[] xIntervals,
                                        out double[] yIntervals)
        {
            if (m_lcmsWarp == null)
            {
                m_lcmsWarp = new LcmsWarp();
            }

            var alignmentScores = new List<double>();
            var aligneeIntervals = new List<double>();
            var baselineIntervals = new List<double>();

            m_lcmsWarp.GetSubsectionMatchScore(ref alignmentScores, ref aligneeIntervals, ref baselineIntervals, true);

            var numBaselineSections = baselineIntervals.Count;
            var numAligneeSections = aligneeIntervals.Count;

            outputScores = new double[numBaselineSections, numAligneeSections];

            var numTotalSections = alignmentScores.Count;
            if (numTotalSections != numBaselineSections * numAligneeSections)
            {
                throw new ApplicationException("Error in Alignment heatmap scores. Total section is not as expected");
            }

            var aligneeSection = 0;
            var baselineSection = 0;
            for (var i = 0; i < numTotalSections; i++)
            {
                outputScores[baselineSection, aligneeSection] = alignmentScores[i];
                baselineSection++;
                if (baselineSection != numBaselineSections)
                {
                    continue;
                }
                baselineSection = 0;    
                aligneeSection++;
                
            }

            xIntervals = new double[numAligneeSections];
            for (var i = 0; i < numAligneeSections; i++)
            {
                xIntervals[i] = aligneeIntervals[i];
            }

            yIntervals = new double[numBaselineSections];
            for (var i = 0; i < numBaselineSections; i++)
            {
                yIntervals[i] = baselineIntervals[i];
            }
        }

        /// <summary>
        /// Method that copies the baseline nets into the parameters passed in
        /// </summary>
        /// <param name="minRefNet"></param>
        /// <param name="maxRefNet"></param>
        public void GetReferenceNetRange(out double minRefNet, out double maxRefNet)
        {
            minRefNet = m_lcmsWarp.MinBaselineNet;
            maxRefNet = m_lcmsWarp.MaxBaselineNet;
        }

        #region Public Statistic Getter properties
        /// <summary>
        /// Returns the Standard Deviation of the Mass of the data
        /// </summary>
        public double MassStd
        {
            get
            {
                double massStd, netStd, netMu, massMu;
                m_lcmsWarp.GetStatistics(out massStd, out netStd, out massMu, out netMu);
                return massStd;
            }
        }
        /// <summary>
        /// Returns the Standard Deviation of the NET of the data
        /// </summary>
        public double NetStd
        {
            get
            {
                double massStd, netStd, netMu, massMu;
                m_lcmsWarp.GetStatistics(out massStd, out netStd, out massMu, out netMu);
                return netStd;
            }
        }
        /// <summary>
        /// Returns the Mean of the Mass of the data
        /// </summary>
        public double MassMu
        {
            get
            {
                double massStd, netStd, netMu, massMu;
                m_lcmsWarp.GetStatistics(out massStd, out netStd, out massMu, out netMu);
                return massMu;
            }
        }
        /// <summary>
        /// Returns the Mean of the NET of the data
        /// </summary>
        public double NetMu
        {
            get
            {
                double massStd, netStd, netMu, massMu;
                m_lcmsWarp.GetStatistics(out massStd, out netStd, out massMu, out netMu);
                return netMu;
            }
        }
        #endregion

        //TODO: Redesign this so that when we say "align(x,y)" we get this in an object separate from everything
        /// <summary>
        /// Copies the histograms from the LCMS Warping and returns them through the Histogram parameters passed in
        /// </summary>
        /// <param name="massBin"></param>
        /// <param name="netBin"></param>
        /// <param name="driftBin"></param>
        /// <param name="massHistogram"></param>
        /// <param name="netHistogram"></param>
        /// <param name="driftHistogram"></param>
        public void GetErrorHistograms(double massBin, double netBin, double driftBin,
                        out double[,] massHistogram, out double[,] netHistogram, out double[,] driftHistogram)
        {
            var massErrorBin = new List<double>();
            var netErrorBin = new List<double>();
            var driftErrorBin = new List<double>();
            var massErrorFreq = new List<int>();
            var netErrorFreq = new List<int>();
            var driftErrorFreq = new List<int>();

            m_lcmsWarp.GetErrorHistograms(massBin, netBin, driftBin, ref massErrorBin, ref massErrorFreq, ref netErrorBin,
                                          ref netErrorFreq, ref driftErrorBin, ref driftErrorFreq);

            //ErrorHistogram massErrorHistogram = new ErrorHistogram(massErrorBin, massErrorFreq);
            //ErrorHistogram netErrorHistogram = new ErrorHistogram(netErrorBin, netErrorFreq);
            //ErrorHistogram driftErrorHistogram = new ErrorHistogram(driftErrorBin, driftErrorFreq);

            massHistogram = new double[massErrorBin.Count, 2];
            netHistogram = new double[netErrorBin.Count, 2];
            driftHistogram = new double[driftErrorBin.Count, 2];

            for (var i = 0; i < massErrorBin.Count; i++)
            {
                massHistogram[i, 0] = massErrorBin[i];
                massHistogram[i, 1] = massErrorFreq[i];
            }
            for (var i = 0; i < netErrorBin.Count; i++)
            {
                netHistogram[i, 0] = netErrorBin[i];
                netHistogram[i, 1] = netErrorFreq[i];
            }
            for (var i = 0; i < driftErrorBin.Count; i++)
            {
                driftHistogram[i, 0] = driftErrorBin[i];
                driftHistogram[i, 1] = driftErrorFreq[i];
            }
        }

        /// <summary>
        /// Calculates all the residual data for the alignment and returns an object
        /// holding all of the residual data in the Residual Data object.
        /// </summary>
        /// <returns></returns>
        public ResidualData GetResidualData()
        {
            var net = new List<double>();
            var mz = new List<double>();
            var linearNet = new List<double>();
            var customNet = new List<double>();
            var linearCustomNet = new List<double>();
            var massError = new List<double>();
            var massErrorCorrected = new List<double>();

            m_lcmsWarp.GetResiduals(ref net, ref mz, ref linearNet, ref customNet, ref linearCustomNet,
                                    ref massError, ref massErrorCorrected);

            var count = net.Count;

            var scans = new double[count];
            var mzs = new double[count];
            var linearNets = new double[count];
            var customNets = new double[count];
            var linearCustomNets = new double[count];
            var massErrors = new double[count];
            var massErrorCorrecteds = new double[count];
            var mzMassErrors = new double[count];
            var mzMassErrorCorrecteds = new double[count];

            //List<ResidualData> dataList = new List<ResidualData>();
            for (var i = 0; i < count; i++)
            {

                scans[i] = net[i];
                mzs[i] = mz[i];
                linearNets[i] = linearNet[i];
                customNets[i] = customNet[i];
                linearCustomNets[i] = linearCustomNet[i];
                massErrors[i] = massError[i];
                massErrorCorrecteds[i] = massErrorCorrected[i];
                mzMassErrors[i] = massError[i];
                mzMassErrorCorrecteds[i] = massErrorCorrected[i];

            }

            var data = new ResidualData
            {
                Scan = scans,
                Mz = mzs,
                LinearNet = linearNets,
                CustomNet = customNets,
                LinearCustomNet = linearCustomNets,
                MassError = massErrors,
                MassErrorCorrected = massErrorCorrecteds,
                MzMassError = mzMassErrors,
                MzMassErrorCorrected = mzMassErrorCorrecteds
            };

            return data;
        }
       
        public ISpectraProvider BaselineSpectraProvider { get; set; }
        public ISpectraProvider AligneeSpectraProvider  { get; set; }

        #region ProgressReporting

        void lcmsWarp_Progress(object sender, ProgressNotifierArgs e)
        {
            // e.PercentComplete is a value between 0 and 100

            var percentCompleteOverall = ComputeIncrementalProgress(
                mPercentCompleteAtStartOfTask,
                mPercentCompleteAtEndOfTask,                
                e.PercentComplete);

            OnProgress(m_LastProgressMessage, percentCompleteOverall);
        }

        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Update progress
        /// </summary>
        /// <param name="message">Current task</param>
        /// <param name="percentComplete">Percent complete (value between 0 and 100)</param>
        private void OnProgress(string message, double percentComplete)
        {
            m_LastProgressMessage = message;
            
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message, percentComplete));
            }
        }
        #endregion region

        public LcmsWarpAlignmentData Align(MassTagDatabase baseline, IEnumerable<UMCLight> alignee)
        {
            throw new NotImplementedException();
        }
        public LcmsWarpAlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee)
        {
            throw new NotImplementedException();
        }
    }
}
