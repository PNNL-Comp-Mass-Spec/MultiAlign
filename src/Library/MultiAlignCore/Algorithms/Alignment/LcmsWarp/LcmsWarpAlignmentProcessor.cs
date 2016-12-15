using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using PNNLOmics.Data.Constants.Libraries;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration;

    /// <summary>
    /// Class which will use LCMSWarp to process alignment
    /// </summary>
    public sealed class LcmsWarpAlignmentProcessor
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
        private double _minReferenceDatasetTime;
        private double _maxReferenceDatasetTime;

        private int _minAligneeDatasetScan;
        private int _maxAligneeDatasetScan;
        private double _minAligneeDatasetTime;
        private double _maxAligneeDatasetTime;
        private double _minAligneeDatasetMz;
        private double _maxAligneeDatasetMz;

        // LCMSWarp instance that will do the alignment when processing
        private readonly LcmsWarp _lcmsWarp;

        /// <summary>
        /// Alignment options
        /// </summary>
        private readonly LcmsWarpAlignmentOptions _options;

        private readonly Dictionary<CurrentLcmsWarpTask, double> _currentTaskPercentCompleteAtStart;
        private double _percentCompleteAtStartOfTask;
        private double _percentCompleteAtEndOfTask;

        /// <summary>
        /// Most recent progress message
        /// </summary>
        private string _lastProgressMessage;

        /// <summary>
        /// Flag for if the Processor is aligning to a Mass Tag Database (list of AMT tags)
        /// </summary>
        private bool _aligningToMassTagDb;

        #region Public properties

        public List<LcmsWarpFeatureMatch> FeatureMatches
        {
            get { return _lcmsWarp.FeatureMatches; }
        }

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

        #endregion

        /// <summary>
        /// Public constructor for the LCMS Alignment Processor
        /// Initializes a new LCMSWarp object using the LCMS Alignment options
        /// that are passed into the Processor
        /// </summary>
        /// <remarks>
        /// Store data using SetReferenceDatasetFeatures and SetAligneeDatasets,
        /// then call PerformAlignmentToMsFeatures, which calls either PerformNetWarp or PerformNetMassWarp</remarks>
        public LcmsWarpAlignmentProcessor(LcmsWarpAlignmentOptions options)
        {
            _options = options;
            _lcmsWarp = new LcmsWarp(_options);

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

        /// <summary>
        /// Takes a List of UMCLight data and applies the NET/Mass Function to the dataset and
        /// aligns it to the baseline. Updates the data in place for the calibrated Masses and
        /// Alignment.
        /// </summary>
        /// <param name="data"></param>
        public List<UMCLight> ApplyNetMassFunctionToAligneeDatasetFeatures(List<UMCLight> data)
        {
            var sortedData = data.OrderBy(x => x.Id).ToList();

            IEnumerable<UMCLight> featureData;

            if (_aligningToMassTagDb)
            {
                featureData = _lcmsWarp.GetFeatureCalibratedMassesAndAlignedNets();
            }
            else
            {
                featureData = _lcmsWarp.GetFeatureCalibratedMassesAndAlignedNets(_minReferenceDatasetScan,
                    _maxReferenceDatasetScan, _minReferenceDatasetTime, _maxReferenceDatasetTime);
            }

            // Update sortedData using featureData

            var i = 0;
            foreach (var point in featureData.OrderBy(x => x.Id))
            {
                while (i < data.Count - 1 && sortedData[i].Id < point.Id)
                {
                    i++;
                }

                while (i > 0 && sortedData[i].Id > point.Id)
                {
                    i--;
                }

                if (sortedData[i].Id != point.Id)
                {
                    Console.WriteLine("Possible code bug; id {0} not found in sortedData: ", point.Id);
                    sortedData.Add(point);
                    continue;
                }

                if (this._options.AlignType == LcmsWarpAlignmentType.NET_MASS_WARP)
                    sortedData[i].MassMonoisotopicAligned = point.MassMonoisotopicAligned;
                else
                    sortedData[i].MassMonoisotopicAligned = sortedData[i].MassMonoisotopic;

                sortedData[i].MassMonoisotopicOriginal = point.MassMonoisotopicOriginal;

                sortedData[i].NetAligned = point.NetAligned;
                sortedData[i].NetStart = point.NetStart;
                sortedData[i].NetEnd = point.NetEnd;
                sortedData[i].DriftTime = point.DriftTime;
                if (!_aligningToMassTagDb)
                {
                    sortedData[i].ScanAligned = point.ScanAligned;
                    sortedData[i].DriftTimeAligned = point.DriftTimeAligned;
                }

                i++;
            }

            return sortedData;
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
            _minAligneeDatasetTime = double.MaxValue;
            _maxAligneeDatasetTime = double.MinValue;
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
                    MassMonoisotopicOriginal = feature.MassMonoisotopicOriginal,
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
                _maxAligneeDatasetTime = Math.Max(_maxAligneeDatasetTime, feature.DriftTime);
                _minAligneeDatasetTime = Math.Min(_minAligneeDatasetTime, feature.DriftTime);
                _maxAligneeDatasetMz = Math.Max(_maxAligneeDatasetMz, feature.Mz);
                _minAligneeDatasetMz = Math.Min(_minAligneeDatasetMz, feature.Mz);
                //}
            }
            _lcmsWarp.SetFeatures(mtFeatures);
        }

        /// <summary>
        /// Use the NET value of the UMCs in the List as the value to align to, the predictor variable
        /// </summary>
        /// <param name="umcData"></param>
        public void SetReferenceDatasetFeatures(List<UMCLight> umcData)
        {
            _aligningToMassTagDb = false;
            var mtFeatures = new List<UMCLight> { Capacity = umcData.Count };

            _minAligneeDatasetScan = int.MaxValue;
            _maxAligneeDatasetScan = int.MinValue;
            _minAligneeDatasetTime = double.MaxValue;
            _maxAligneeDatasetTime = double.MinValue;
            _minReferenceDatasetScan = int.MaxValue;
            _maxReferenceDatasetScan = int.MinValue;
            _minReferenceDatasetTime = double.MaxValue;
            _maxReferenceDatasetTime = double.MinValue;
            _minAligneeDatasetMz = int.MaxValue;
            _maxAligneeDatasetMz = int.MinValue;

            for (var index = 0; index < umcData.Count; index++)
            {
                var data = umcData[index];

                mtFeatures.Add(new UMCLight
                {
                    MassMonoisotopic = data.MassMonoisotopic,
                    MassMonoisotopicAligned = data.MassMonoisotopicAligned,
                    MassMonoisotopicOriginal = data.MassMonoisotopicOriginal,
                    Net = data.Net,
                    Mz = data.Mz,
                    Abundance = data.Abundance,
                    DriftTime = data.DriftTime,
                    Id = data.Id,
                });

                _maxReferenceDatasetTime = Math.Max(_maxReferenceDatasetTime, data.DriftTime);
                _minReferenceDatasetTime = Math.Min(_minReferenceDatasetTime, data.DriftTime);
                _maxReferenceDatasetScan = Math.Max(_maxReferenceDatasetScan, data.Scan);
                _minReferenceDatasetScan = Math.Min(_minReferenceDatasetScan, data.Scan);
                _maxAligneeDatasetMz = Math.Max(_maxAligneeDatasetMz, data.Mz);
                _minAligneeDatasetMz = Math.Min(_minAligneeDatasetMz, data.Mz);
            }
            _lcmsWarp.SetReferenceFeatures(mtFeatures);
        }

        /// <summary>
        /// Sets alignment features for a MSFeature dataset from a database
        /// These are either the features in a baseline dataset or AMT tags to align to
        /// </summary>
        /// <param name="features"></param>
        /// <remarks>
        /// Only uses features that pass all of the filters:
        ///  minimum observation count (at least _options.MinimumAMTTagObsCount)
        ///  NET range (between _options.AMTTagFilterNETMin and _options.AMTTagFilterNETMax)
        ///  Mass range (between _options.AMTTagFilterMassMin and _options.AMTTagFilterMassMax)
        /// </remarks>
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
                        MassMonoisotopicOriginal = item.MassMonoisotopicOriginal,
                        // Mz = item.Mz,
                        Mz = item.ChargeState != 0 ? item.MassMonoisotopic / item.ChargeState + (SubAtomicParticleLibrary.MASS_PROTON * (item.ChargeState - 1)) : 0,
                        Net = item.Net,
                        DriftTime = item.DriftTime,
                        Id = item.Id,
                        // Note: SpectralCount is used by LCMSWarp's scorer
                        SpectralCount = item.ObservationCount
                    }));

                if (mtFeatures.Count > 0)
                    break;

                if (minimumObservationCount == 0)
                {
                    break;
                }

                // Lower the minimum observation and try again
                if (minimumObservationCount >= 4)
                    minimumObservationCount = (int)Math.Floor(minimumObservationCount / 2.0);
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
            var func = new LcmsWarpAlignmentFunction(_options.CalibrationType, _options.AlignType);

            List<double> aligneeNets;
            List<double> referenceNets;
            _lcmsWarp.AlignmentFunction(out aligneeNets, out referenceNets);

            if (_aligningToMassTagDb)
            {
                func.SetNetFunction(aligneeNets, referenceNets);
            }
            else
            {
                var referenceScans = referenceNets.Select(
                    rNet => _minReferenceDatasetScan + rNet * (_maxReferenceDatasetScan - _minReferenceDatasetScan)).ToList();

                var referenceTimes = referenceNets.Select(
                    rNet => _minReferenceDatasetTime + rNet * (_maxReferenceDatasetTime - _minReferenceDatasetTime)).ToList();

                func.SetNetFunction(aligneeNets, referenceNets, referenceScans, referenceTimes);
            }

            if (_options.AlignType == LcmsWarpAlignmentType.NET_WARP)
            {
                return func;
            }

            var minAligneeNet = _lcmsWarp.MinNet;
            var maxAligneeNet = _lcmsWarp.MaxNet;

            // Get the mass calibration function with time
            var numXSlices = _options.MassCalibNumXSlices;

            if (_options.CalibrationType == LcmsWarpCalibrationType.NetRegression ||
                _options.CalibrationType == LcmsWarpCalibrationType.Both)
            {
                // get the PPM for each X slice

                var aligneeNetMassFunc = new List<double>();
                var aligneePpmShiftMassFunc = new List<double>();

                for (var sliceIndex = 0; sliceIndex < numXSlices; sliceIndex++)
                {
                    var net = minAligneeNet + ((maxAligneeNet - minAligneeNet) * sliceIndex) / numXSlices;
                    aligneeNetMassFunc.Add(net);
                    aligneePpmShiftMassFunc.Add(_lcmsWarp.GetPpmShiftFromNet(net));
                }
                func.SetMassCalibrationFunctionWithNet(aligneeNetMassFunc, aligneePpmShiftMassFunc);
            }

            if (_options.CalibrationType == LcmsWarpCalibrationType.MzRegression ||
                _options.CalibrationType == LcmsWarpCalibrationType.Both)
            {
                // Get the ppm for each X slice

                var aligneeMzMassFunc = new List<double>();
                var aligneePpmShiftMassFunc = new List<double>();

                for (var sliceIndex = 0; sliceIndex < numXSlices; sliceIndex++)
                {
                    var net = sliceIndex * 1.0 / numXSlices;
                    var mz = _minAligneeDatasetMz + (int)((_maxAligneeDatasetMz - _minAligneeDatasetMz) * net);
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
        /// <param name="progress"></param>
        public void PerformAlignmentToMsFeatures(IProgress<ProgressData> progress = null)
        {
            if (_options == null)
            {
                throw new NullReferenceException("Alignment Options were not set in AlignmentProcessor");
            }

            if (_options.AlignType == LcmsWarpAlignmentType.NET_WARP)
            {
                PerformNetWarp(0, 100, progress);
            }
            else
            {
                PerformNetMassWarp(progress);
            }
        }

        /// <summary>
        /// Performs the NET Warping; Generates matches, gets the probabilities,
        /// calculates the alignment matrix and alignment function, gets the transformed NETs
        /// and then calculates the alignment matches
        /// </summary>
        private void PerformNetWarp(double percentCompleteAtStart, double percentCompleteAtEnd, IProgress<ProgressData> progress = null)
        {
            var progData = new ProgressData(progress);
            var prog = new Progress<ProgressData>(p => progData.Report(p.Percent, p.Status));
            var percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.GenerateCandidateMatches);
            OnProgress("NET Warp, get candidate matches", percentCompleteOverall);
            progData.StepRange(10, "NET Warp, get candidate matches");
            progData.Report(0.0);

            _lcmsWarp.GenerateCandidateMatches(prog);

            if (_lcmsWarp.NumCandidateMatches < 10)
            {
                throw new ApplicationException("Insufficient number of candidate matches by mass alone (NumMatches < 10)");
            }

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.GetMatchProbabilities);
            OnProgress("NET Warp, get match probabilities", percentCompleteOverall);
            progData.StepRange(90, "NET Warp, get match probabilities");

            _lcmsWarp.GetMatchProbabilities(prog);

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.CalculateAlignmentMatrix);
            OnProgress("NET Warp, calculate alignment matrix", percentCompleteOverall);
            progData.StepRange(93, "NET Warp, calculate alignment matrix");

            _lcmsWarp.CalculateAlignmentMatrix(prog);

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.CalculateAlignmentFunction);
            OnProgress("NET Warp, calculate alignment function", percentCompleteOverall);
            progData.StepRange(96, "NET Warp, calculate alignment function");

            _lcmsWarp.CalculateAlignmentFunction(prog);

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.GetTransformedNets);
            OnProgress("NET Warp, get transformed NETs", percentCompleteOverall);
            progData.StepRange(98, "NET Warp, get transformed NETs");

            _lcmsWarp.GetTransformedNets(prog);

            percentCompleteOverall = UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd,
                CurrentLcmsWarpTask.CalculateAlignmentMatches);
            OnProgress("NET Warp, calculate alignment matches", percentCompleteOverall);
            progData.StepRange(100, "NET Warp, calculate alignment matches");

            _lcmsWarp.CalculateAlignmentMatches(prog);

            UpdateCurrentTask(percentCompleteAtStart, percentCompleteAtEnd, CurrentLcmsWarpTask.Complete);
            progData.Report(100.0, "NET Warp, complete");
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
        /// <param name="progress"></param>
        private void PerformNetMassWarp(IProgress<ProgressData> progress = null)
        {
            var progData = new ProgressData(progress);
            var prog = new Progress<ProgressData>(p => progData.Report(p.Percent, p.Status));
            OnProgress("LCMSWarp phase one", 0);
            progData.StepRange(50, "LCMSWarp phase one");

            // First, perform the net calibration using a mass tolerance of the same size as the mass window
            // and then perform the net calibration again using the appropriate mass tolerance
            var massTolerance = _lcmsWarp.MassTolerance;
            _lcmsWarp.MassTolerance = _lcmsWarp.MassCalibrationWindow;
            _lcmsWarp.UseMassAndNetScore(false);

            PerformNetWarp(0, 50, prog);

            OnProgress("Calibrating mass", 50);
            progData.StepRange(60, "Calibrating mass");

            _lcmsWarp.PerformMassCalibration();
            _lcmsWarp.CalculateStandardDeviations();

            OnProgress("LCMSWarp phase two", 60);
            progData.StepRange(100, "LCMSWarp phase two");

            _lcmsWarp.MassTolerance = massTolerance;
            _lcmsWarp.UseMassAndNetScore(true);

            PerformNetWarp(60, 100, prog);

            OnProgress("Complete", 100);
            progData.Report(100, "Complete");
        }

        /// <summary>
        /// The alignee values of the SubsectionMatchScore, which correspond to the second (y in [x,y]) axis of the heat map
        /// </summary>
        /// <returns></returns>
        public double[] GetAlignmentHeatMapYAxisVals()
        {
            return _lcmsWarp.GetSubsectionAligneeVals();
        }

        /// <summary>
        /// The baseline values of the SubsectionMatchScore, which correspond to the first (x in [x,y]) axis of the heat map
        /// </summary>
        /// <returns></returns>
        public double[] GetAlignmentHeatMapXAxisVals()
        {
            return _lcmsWarp.GetSubsectionBaselineVals();
        }

        /// <summary>
        /// Method to return the heatmap of the alignment (as a 2D array of doubles) based on
        /// the output scores
        /// </summary>
        /// <returns></returns>
        public double[,] GetAlignmentHeatMap(bool standardize = true)
        {
            return _lcmsWarp.GetSubsectionMatchScore(standardize);
        }

        /// <summary>
        /// Determines the transformed NETs for a range of scans
        /// </summary>
        public Dictionary<int, double> GetScanToNETMapping()
        {
            if (_lcmsWarp == null)
                return new Dictionary<int, double>();

            return _lcmsWarp.GetScanToNETMapping(_minAligneeDatasetScan, _maxAligneeDatasetScan);
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

        /// <summary>
        /// Get the mass error histogram from the LCMS Warping
        /// </summary>
        /// <param name="massBinSize"></param>
        /// <returns></returns>
        public Dictionary<double, int> GetMassErrorHistogram(double massBinSize)
        {
            return _lcmsWarp.GetMassErrorHistogram(massBinSize);
        }

        /// <summary>
        /// Get the net error histogram from the LCMS Warping
        /// </summary>
        /// <param name="netBinSize"></param>
        /// <returns></returns>
        public Dictionary<double, int> GetNetErrorHistogram(double netBinSize)
        {
            return _lcmsWarp.GetNetErrorHistogram(netBinSize);
        }

        /// <summary>
        /// Get the drift error histogram from the LCMS Warping
        /// </summary>
        /// <param name="driftBinSize"></param>
        /// <returns></returns>
        public Dictionary<double, int> GetDriftErrorHistogram(double driftBinSize)
        {
            return _lcmsWarp.GetDriftErrorHistogram(driftBinSize);
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

        public IScanSummaryProvider BaselineSpectraProvider { get; set; }
        public IScanSummaryProvider AligneeSpectraProvider { get; set; }

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
    }
}