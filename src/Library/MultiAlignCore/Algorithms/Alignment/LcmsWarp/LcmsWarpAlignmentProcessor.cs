using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Class which will use LCMSWarp to process alignment
    /// </summary>
    public sealed class LcmsWarpAlignmentProcessor :
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
        private int _minReferenceDatasetScan;
        private int _maxReferenceDatasetScan;

        private int _minAligneeDatasetScan;
        private int _maxAligneeDatasetScan;
        private double _minAligneeDatasetMz;
        private double _maxAligneeDatasetMz;

        // LCMSWarp instance that will do the alignment when processing
        private LcmsWarp _lcmsWarp;

        /// <summary>
        /// Alignment options
        /// </summary>
        private LcmsWarpAlignmentOptions _options;

        private readonly Dictionary<CurrentLcmsWarpTask, double> _currentTaskPercentCompleteAtStart;
        private double _percentCompleteAtStartOfTask;
        private double _percentCompleteAtEndOfTask;

        /// <summary>
        /// Most recent progress message
        /// </summary>
        private string _lastProgressMessage;

        /// <summary>
        /// Flag for if the Processor is aligning to a Mass Tag Database
        /// </summary>
        private bool _aligningToMassTagDb;

        #region Public properties

        /// <summary>
        /// Get property for the NET Intercept that LCMS Warp is holding
        /// Simulates a pure linear regression
        /// </summary>
        public double NetIntercept
        {
            get { return _lcmsWarp.NetIntercept; }
        }

        /// <summary>
        /// Get property for the NET RSquared that LCMS Warp is holding
        /// Simulates a pure linear regression
        /// </summary>
        public double NetRsquared
        {
            get { return _lcmsWarp.NetLinearRsq; }
        }

        /// <summary>
        /// Get property for the NET Slope that LCMS Warp is holding
        /// Simulates a pure linear regression
        /// </summary>
        public double NetSlope
        {
            get { return _lcmsWarp.NetSlope; }
        }

        /// <summary>
        /// Options for the Alignment processor
        /// </summary>
        public LcmsWarpAlignmentOptions Options
        {
            get { return _options; }
            set
            {
                _options = value;
                ApplyAlignmentOptions();
            }
        }

        #endregion

        /// <summary>
        /// Public constructor for the LCMS Alignment Processor
        /// Initializes a new LCMSWarp object using the LCMS Alignment options
        /// which were passed into the Processor
        /// </summary>
        public LcmsWarpAlignmentProcessor()
        {
            _lcmsWarp = new LcmsWarp();
            Options = new LcmsWarpAlignmentOptions();

            _lcmsWarp.Progress += lcmsWarp_Progress;

            _currentTaskPercentCompleteAtStart = new Dictionary<CurrentLcmsWarpTask, double>
            {
                {CurrentLcmsWarpTask.Unstarted, 0},
                {CurrentLcmsWarpTask.GenerateCandidateMatches, 0},
                {CurrentLcmsWarpTask.GetMatchProbabilities, 10},
                {CurrentLcmsWarpTask.CalculateAlignmentMatrix, 90},
                {CurrentLcmsWarpTask.CalculateAlignmentFunction, 93},
                {CurrentLcmsWarpTask.GetTransformedNets, 96},
                {CurrentLcmsWarpTask.CalculateAlignmentMatches, 98},
                {CurrentLcmsWarpTask.Complete, 100}
            };

            _percentCompleteAtStartOfTask = 0;
            _percentCompleteAtEndOfTask = 100;

            _aligningToMassTagDb = false;
        }

        // Applies the alignment options to the LCMSWarper, setting the Mass
        // and NET Tolerances, the options for NET Alignment options for 
        // Mass calibration, the Least Squares options and the calibration type
        public void ApplyAlignmentOptions()
        {
            _lcmsWarp.ApplyAlignmentOptions(Options);
        }

        /// <summary>
        /// Takes a List of UMCLight data and applies the NET/Mass Function to the dataset and
        /// aligns it to the baseline. Updates the data in place for the calibrated Masses and
        /// Alignment. 
        /// </summary>
        /// <param name="data"></param>
        public void ApplyNetMassFunctionToAligneeDatasetFeatures(List<UMCLight> data)
        {
            if (_lcmsWarp == null)
            {
                _lcmsWarp = new LcmsWarp();
            }

            IEnumerable<UMCLight> featureData;

            if (_aligningToMassTagDb)
            {
                featureData = _lcmsWarp.GetFeatureCalibratedMassesAndAlignedNets();
            }
            else
            {
                featureData = _lcmsWarp.GetFeatureCalibratedMassesAndAlignedNets(_minReferenceDatasetScan,
                    _maxReferenceDatasetScan);
            }

            var i = 0;
            foreach (var point in featureData)
            {
                if (i < data.Count)
                {
                    // Update the data without stomping the data that shouldn't change.
                    // TODO: Are we updating the right data???
                    data[i].MassMonoisotopicAligned = point.MassMonoisotopicAligned;
                    data[i].NetAligned = point.NetAligned;
                    data[i].NetStart = point.NetStart;
                    data[i].NetEnd = point.NetEnd;
                    data[i].DriftTime = point.DriftTime;
                    if (_aligningToMassTagDb)
                    {
                        data[i].ScanAligned = point.ScanAligned;
                    }
                }
                else
                {
                    data.Add(point);
                }
                i++;
            }
        }

        /// <summary>
        /// For a given List of UMCLights, warp the alignee features to the baseline.
        /// </summary>
        /// <param name="features"></param>
        public void SetAligneeDatasetFeatures(List<UMCLight> features)
        {
            var mtFeatures = new List<UMCLight> { Capacity = features.Count };

            _minAligneeDatasetScan = int.MaxValue;
            _maxAligneeDatasetScan = int.MinValue;
            _minAligneeDatasetMz = double.MaxValue;
            _maxAligneeDatasetMz = double.MinValue;

            for (var index = 0; index < features.Count; index++)
            {
                var feature = features[index];

                // Note: We are using ScanStart for the NET of the feature
                // This is to avoid odd effects from broad or tailing LC-MS features
                var mtFeature = new UMCLight
                {
                    MassMonoisotopic = feature.MassMonoisotopic,
                    MassMonoisotopicAligned = feature.MassMonoisotopicAligned,
                    Net = Convert.ToDouble(feature.ScanStart),
                    Mz = feature.Mz,
                    Abundance = feature.Abundance,
                    Id = feature.Id,
                    DriftTime = feature.DriftTime
                };

                //mtFeature.MonoMassOriginal = features[index].MassMonoisotopic;

                // For if we want to split alignment at given M/Z range

                // Only allow feature to be aligned if we're splitting the alignment in MZ
                // AND if we are within the specified boundary
                //if (ValidateAlignmentBoundary(Options.AlignSplitMZs, mtFeature.Mz, boundary))
                //{
                mtFeatures.Add(mtFeature);

                _maxAligneeDatasetScan = Math.Max(_maxAligneeDatasetScan, feature.Scan);
                _minAligneeDatasetScan = Math.Min(_minAligneeDatasetScan, feature.Scan);
                _maxAligneeDatasetMz = Math.Max(_maxAligneeDatasetMz, feature.Mz);
                _minAligneeDatasetMz = Math.Min(_minAligneeDatasetMz, feature.Mz);
                //}
            }
            _lcmsWarp.SetFeatures(mtFeatures);
        }

        /// <summary>
        /// 
        /// Use the NET value of the UMCs in the List as the value to align to, the predictor variable
        /// </summary>
        /// <param name="umcData"></param>
        public void SetReferenceDatasetFeatures(List<UMCLight> umcData)
        {
            _aligningToMassTagDb = false;
            var mtFeatures = new List<UMCLight> { Capacity = umcData.Count };

            _minAligneeDatasetScan = int.MaxValue;
            _maxAligneeDatasetScan = int.MinValue;
            _minReferenceDatasetScan = int.MaxValue;
            _maxReferenceDatasetScan = int.MinValue;
            _minAligneeDatasetMz = int.MaxValue;
            _maxAligneeDatasetMz = int.MinValue;

            for (var index = 0; index < umcData.Count; index++)
            {
                var data = umcData[index];

                mtFeatures.Add(new UMCLight
                {
                    MassMonoisotopic = data.MassMonoisotopic,
                    MassMonoisotopicAligned = data.MassMonoisotopicAligned,
                    Net = data.Net,
                    Mz = data.Mz,
                    Abundance = data.Abundance,
                    DriftTime = data.DriftTime,
                    Id = data.Id,
                });

                _maxReferenceDatasetScan = Math.Max(_maxReferenceDatasetScan, data.Scan);
                _minReferenceDatasetScan = Math.Min(_minReferenceDatasetScan, data.Scan);
                _maxAligneeDatasetMz = Math.Max(_maxAligneeDatasetMz, data.Mz);
                _minAligneeDatasetMz = Math.Min(_minAligneeDatasetMz, data.Mz);
            }
            _lcmsWarp.SetReferenceFeatures(mtFeatures);
        }

        /// <summary>
        /// Sets alignment features for a MSFeature dataset from a database
        /// </summary>
        /// <param name="features"></param>
        public void SetReferenceDatasetFeatures(List<MassTagLight> features)
        {
            _aligningToMassTagDb = true;

            var mtFeatures = new List<UMCLight> { Capacity = features.Count };
            var minimumObservationCount = _options.MinimumAMTTagObsCount;

            while (true)
            {
                mtFeatures.AddRange(features
                    .Where(item => AMTTagPassesFilter(item, minimumObservationCount))
                    .Select(item => new UMCLight
                    {
                        NetAligned = item.NetAligned,
                        MassMonoisotopic = item.MassMonoisotopic,
                        MassMonoisotopicAligned = item.MassMonoisotopicAligned,
                        //Mz = item.Mz,
                        Mz = item.MassMonoisotopic / item.ChargeState + (1.00782 * (item.ChargeState - 1)),
                        Net = item.Net,
                        DriftTime = item.DriftTime,
                        Id = item.Id
                    }));

                if (mtFeatures.Count > 0)
                    break;

                if (minimumObservationCount == 0)
                {
                    break;
                }

                // Lower the minimum observation and try again
                if (minimumObservationCount >= 4)
                    minimumObservationCount = (int) Math.Floor(minimumObservationCount / 2.0);
                else
                    minimumObservationCount = 0;
            }

            _lcmsWarp.SetReferenceFeatures(mtFeatures);
        }

        private bool AMTTagPassesFilter(MassTagLight item, int minimumObservationCount)
        {
            if (item.ObservationCount < minimumObservationCount)
                return false;

            if (_options.AMTTagFilterNETMax > _options.AMTTagFilterNETMin)
            {
                if (item.Net < _options.AMTTagFilterNETMin || item.Net > _options.AMTTagFilterNETMax)
                    return false;
            }

            if (_options.AMTTagFilterMassMax > _options.AMTTagFilterMassMin)
            {
                if (item.MassMonoisotopic < _options.AMTTagFilterMassMin ||
                    item.MassMonoisotopic > _options.AMTTagFilterMassMax)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the Alignment Function that the processor determined
        /// </summary>
        /// <returns></returns>
        public LcmsWarpAlignmentFunction GetAlignmentFunction()
        {
            var func = new LcmsWarpAlignmentFunction(Options.CalibrationType, Options.AlignType);

            List<double> aligneeNets;
            List<double> referenceNets;
            _lcmsWarp.AlignmentFunction(out aligneeNets, out referenceNets);

            if (_aligningToMassTagDb)
            {
                func.SetNetFunction(aligneeNets, referenceNets);
            }
            else
            {
                var referenceScans = referenceNets.Select(rNet => _minReferenceDatasetScan +
                                             rNet * (_maxReferenceDatasetScan - _minReferenceDatasetScan)).ToList();
                func.SetNetFunction(aligneeNets, referenceNets, referenceScans);
            }

            if (Options.AlignType == LcmsWarpAlignmentType.NET_WARP)
            {
                return func;
            }

            var minAligneeNet = _lcmsWarp.MinNet;
            var maxAligneeNet = _lcmsWarp.MaxNet;

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
                    aligneeNetMassFunc.Add(net);
                    aligneePpmShiftMassFunc.Add(_lcmsWarp.GetPpmShiftFromNet(net));
                }
                func.SetMassCalibrationFunctionWithTime(aligneeNetMassFunc, aligneePpmShiftMassFunc);
            }

            if (Options.CalibrationType == LcmsWarpCalibrationType.MzRegression ||
                Options.CalibrationType == LcmsWarpCalibrationType.Both)
            {
                // Get the ppm for each knot
                for (var knotNum = 0; knotNum < numXKnots; knotNum++)
                {
                    var net = knotNum * 1.0 / numXKnots;
                    var mz = _minAligneeDatasetMz + (int) ((_maxAligneeDatasetMz - _minAligneeDatasetMz) * net);
                    aligneeMzMassFunc.Add(mz);
                    aligneePpmShiftMassFunc.Add(_lcmsWarp.GetPpmShiftFromMz(mz));
                }
                func.SetMassCalibrationFunctionWithMz(aligneeMzMassFunc, aligneePpmShiftMassFunc);
            }
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

            if (Options.AlignType == LcmsWarpAlignmentType.NET_WARP)
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
            var percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.GenerateCandidateMatches);
            OnProgress("NET Warp, get candidate matches", percentCompleteOverall);

            _lcmsWarp.GenerateCandidateMatches();

            if (_lcmsWarp.NumCandidateMatches < 10)
            {
                throw new ApplicationException("Insufficient number of candidate matches by mass alone");
            }

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.GetMatchProbabilities);
            OnProgress("NET Warp, get match probabilities", percentCompleteOverall);

            _lcmsWarp.GetMatchProbabilities();

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.CalculateAlignmentMatrix);
            OnProgress("NET Warp, calculate alignment matrix", percentCompleteOverall);

            _lcmsWarp.CalculateAlignmentMatrix();

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.CalculateAlignmentFunction);
            OnProgress("NET Warp, calculate alignment function", percentCompleteOverall);

            _lcmsWarp.CalculateAlignmentFunction();

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.GetTransformedNets);
            OnProgress("NET Warp, get transformed NETs", percentCompleteOverall);

            _lcmsWarp.GetTransformedNets();

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.CalculateAlignmentMatches);
            OnProgress("NET Warp, calculate alignment matches", percentCompleteOverall);

            _lcmsWarp.CalculateAlignmentMatches();

            UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.Complete);
        }

        /// <summary>
        /// Compute the effective Percent Complete value given a starting and ending percent complete, plus the percent complete of a subtask
        /// </summary>
        /// <param name="percentCompleteAtStart"></param>
        /// <param name="percentCompleteAtEnd"></param>
        /// <param name="subtaskPercentComplete"></param>
        /// <returns></returns>
        private double ComputeIncrementalProgress(double percentCompleteAtStart, double percentCompleteAtEnd,
            double subtaskPercentComplete)
        {
            return percentCompleteAtStart +
                   (percentCompleteAtEnd - percentCompleteAtStart) * subtaskPercentComplete / 100;
        }

        /// <summary>
        /// Updates mCurrentTask
        /// </summary>
        /// <param name="percentCompleteAtStart"></param>
        /// <param name="percentCompleteAtEnd"></param>
        /// <param name="currentTask"></param>
        /// <returns>Effective percent complete overall</returns>
        private double UpdateCurrentTask(double percentCompleteAtStart, double percentCompleteAtEnd,
            CurrentLcmsWarpTask currentTask)
        {
            _percentCompleteAtStartOfTask = ComputeIncrementalProgress(percentCompleteAtStart, percentCompleteAtEnd,
                _currentTaskPercentCompleteAtStart[currentTask]);

            if (!_currentTaskPercentCompleteAtStart.TryGetValue(currentTask + 1, out _percentCompleteAtEndOfTask))
                _percentCompleteAtEndOfTask = _percentCompleteAtStartOfTask;

            return _percentCompleteAtStartOfTask;
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
            var massTolerance = _lcmsWarp.MassTolerance;
            _lcmsWarp.MassTolerance = _lcmsWarp.MassCalibrationWindow;
            _lcmsWarp.UseMassAndNetScore(false);

            PerformNetWarp(0, 50);

            OnProgress("Calibrating mass", 50);

            _lcmsWarp.PerformMassCalibration();
            _lcmsWarp.CalculateStandardDeviations();

            OnProgress("LCMSWarp phase two", 60);

            _lcmsWarp.MassTolerance = massTolerance;
            _lcmsWarp.UseMassAndNetScore(true);

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
            if (_lcmsWarp == null)
            {
                _lcmsWarp = new LcmsWarp();
            }

            // Alignment Scores, alignee intervals, baseline intervals
            _lcmsWarp.GetSubsectionMatchScore(out outputScores, out xIntervals, out yIntervals, true);
        }

        /// <summary>
        /// Minimum baseline/reference NET
        /// </summary>
        public double MinReferenceNet
        {
            get { return _lcmsWarp.MinBaselineNet; }
        }

        /// <summary>
        /// Maximum baseline/reference NET
        /// </summary>
        public double MaxReferenceNet
        {
            get { return _lcmsWarp.MaxBaselineNet; }
        }

        #region Public Statistic Getter properties

        /// <summary>
        /// Returns the Standard Deviation of the Mass of the data
        /// </summary>
        public double MassStd
        {
            get { return _lcmsWarp.StatisticMassStd; }
        }

        /// <summary>
        /// Returns the Standard Deviation of the NET of the data
        /// </summary>
        public double NetStd
        {
            get { return _lcmsWarp.StatisticNetStd; }
        }

        /// <summary>
        /// Returns the Mean of the Mass of the data
        /// </summary>
        public double MassMu
        {
            get { return _lcmsWarp.StatisticMassMu; }
        }

        /// <summary>
        /// Returns the Mean of the NET of the data
        /// </summary>
        public double NetMu
        {
            get { return _lcmsWarp.StatisticNetMu; }
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
            out Dictionary<double, int> massHistogram, out Dictionary<double, int> netHistogram,
            out Dictionary<double, int> driftHistogram)
        {
            _lcmsWarp.GetErrorHistograms(massBin, netBin, driftBin, out massHistogram, out netHistogram,
                out driftHistogram);
        }

        /// <summary>
        /// Calculates all the residual data for the alignment and returns an object
        /// holding all of the residual data in the Residual Data object.
        /// </summary>
        /// <returns></returns>
        public ResidualData GetResidualData()
        {
            return _lcmsWarp.GetResiduals();
        }

        public ISpectraProvider BaselineSpectraProvider { get; set; }
        public ISpectraProvider AligneeSpectraProvider { get; set; }

        #region ProgressReporting

        private void lcmsWarp_Progress(object sender, ProgressNotifierArgs e)
        {
            // e.PercentComplete is a value between 0 and 100

            var percentCompleteOverall = ComputeIncrementalProgress(
                _percentCompleteAtStartOfTask,
                _percentCompleteAtEndOfTask,
                e.PercentComplete);

            OnProgress(_lastProgressMessage, percentCompleteOverall);
        }

        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Update progress
        /// </summary>
        /// <param name="message">Current task</param>
        /// <param name="percentComplete">Percent complete (value between 0 and 100)</param>
        private void OnProgress(string message, double percentComplete)
        {
            _lastProgressMessage = message;

            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message, percentComplete));
            }
        }

        #endregion region

        public LcmsWarpAlignmentData Align(MassTagDatabase baseline, IEnumerable<UMCLight> alignee,
            IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public LcmsWarpAlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee,
            IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}