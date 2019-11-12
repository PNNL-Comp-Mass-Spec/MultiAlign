using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureAlignment.Algorithms.Alignment.LcmsWarp;
using FeatureAlignment.Data.Alignment;
using PRISM;
#if CPP_ENABLED
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using PNNLOmics.Utilities;
#endif
using FeatureAlignmentType = FeatureAlignment.Algorithms.Alignment.FeatureAlignmentType;

namespace LCMSWarpConsole
{
    class clsLCMSWarpRunner
    {
        #region "Constants and Enums"

        private const double PROTON_MASS = 1.00727649;

        private const bool USE_THREADING = true;

        private enum AlignmentState
        {
            // ReSharper disable once IdentifierTypo
            Unstarted = 0,
            Running = 1,
            Complete = 2,
            Error = 3,
            Aborted = 4,
        }

        /// <summary>
        /// Obsolete: This is used by the C++ processor
        /// </summary>
        private enum MassMatchProcessingState
        {
            Uninitialized = 0,
            Running = 1,
            Complete = 2,
            Error = 3,
            Aborted = 4,
            InsufficientMatches = 5
        }

        private const int PMT_COLUMN_COUNT = 4;

        private enum PMTColumns
        {
            NET = 0,
            MonoisotopicMass = 1,
            ObservationCount = 2,             // Must be an integer
            PMTTagID = 3                      // Must be an integer
        }

        private const int FEATURE_COLUMN_COUNT = 7;

        private enum FeatureColumns
        {
            ClassMZ = 0,
            ClassMass = 1,
            ScanClassRep = 2,               // Must be an integer
            ClassAbundance = 3,
            PairIndex = 4,                  // Must be an integer; -1 if not a member of a pair; if part of one or more pairs, then only lists the PairIndex of the first pair the UMC belongs to
            PMTTagID = 5,                   // Must be an integer
            UMCID = 6                       // Must be an integer
        }

        #endregion


        //[DllImport(@"..\..\..\Debug\MassMatchCOM.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern double Add(double a, double b);

        #region "Properties"
        public double AlignmentProgress { get; set; }
        #endregion

        #region "Member variables"

        private AlignmentState mAlignmentState;

        private AlignmentData mAlignmentData;
        #endregion



        /// <summary>
        /// Constructor
        /// </summary>
        public clsLCMSWarpRunner()
        {
            mAlignmentState = AlignmentState.Unstarted;
        }

        public bool AlignLCMSFeaturesToPMTs(string optionsFilePath, string lcmsFeaturesFilePath, string aligneePMTsFilePath)
        {
            // Set this to true to use the C++ based LCMSWarp
            const bool USE_CPP = false;

            try
            {
                var optionsFile = new FileInfo(optionsFilePath);

                // Read the options
                // Note that LoadOptions will validate that the options file exists
                var options = LoadOptions(optionsFile.FullName);
                if (options == null)
                    return false;

                // Load the LCMS Features
                // 2D Array, ranging from 0 to mLocalFeatureCount-1 in the 1st dimension and 0 to FEATURE_COLUMN_COUNT-1 in the second dimension
                // Columns for the 2nd dimension are given by FeatureColumnConstants
                var lcmsFeatures = LoadLCMSFeatures(lcmsFeaturesFilePath);
                if (lcmsFeatures == null)
                    return false;

                // Load the Alignee PMTs
                // 2D Array, ranging from 0 to mLocalPMTCount-1 in the 1st dimension and 0 to PMT_COLUMN_COUNT-1 in the second dimension
                // Columns for the 2nd dimension are given by PMTColumnConstants
                var aligneePMTs = LoadAligneePMTs(aligneePMTsFilePath);
                if (aligneePMTs == null)
                    return false;

                var outputFolder = optionsFile.Directory;

                if (USE_CPP)
                {
                    // Use C++
                    // Delete any existing results
                    DeleteExistingResults(outputFolder, "C++");

                    AlignUseCPP(options, lcmsFeatures, aligneePMTs, outputFolder);
                }
                else
                {
                    // Use C++
                    // Delete any existing results
                    DeleteExistingResults(outputFolder, "C#");

                    AlignUseCSharp(options, lcmsFeatures, aligneePMTs, outputFolder);
                }

                return true;
            }
            catch (Exception ex)
            {
                OnError("Error in AlignLCMSFeaturesToPMTs: " + ex.Message);
                return false;
            }
        }

        private bool AlignUseCPP(
            clsAlignmentOptions options,
            IReadOnlyList<FeatureAlignment.Data.Features.UMCLight> datasetFeatures,
            IReadOnlyList<FeatureAlignment.Data.MassTags.MassTagLight> aligneePMTs,
            DirectoryInfo outputFolder)
        {

#if !CPP_ENABLED
            Console.WriteLine("C++ alignment is disabled");
            return false;
#else
            Console.WriteLine("Aligning using C++");
            var massMatchObject = new CMassMatchWrapper();

            // Set the MassMatch Options
            massMatchObject.ResetStatus();

            massMatchObject.SetNetOptions(options.NumberOfSections,
                                           options.ContractionFactor,
                                           options.MaxDistortion,
                                           options.NETTol,
                                           options.NET_MIN,
                                           options.NET_MAX,
                                           options.MatchPromiscuity);

            // Old: Convert MassUseLSQ to -1 if true, otherwise 0
            // var useLSQ = options.MassUseLSQ ? -1 : 0;

            massMatchObject.SetMassOptions(options.MWTol,
                                             options.MassNumMassDeltaBins,
                                             options.MassWindowPPM,
                                             options.MassMaxJump,
                                             options.MassNumXSlices,
                                             options.MassZScoreTolerance,
                                             options.MassUseLSQ);

            // 2D Array, ranging from 0 to mLocalFeatureCount-1 in the 1st dimension and 0 to FEATURE_COLUMN_COUNT-1 in the second dimension
            var lcmsFeatures2D = new double[datasetFeatures.Count, FEATURE_COLUMN_COUNT];

            // 2D Array, ranging from 0 to mLocalPMTCount-1 in the 1st dimension and 0 to PMT_COLUMN_COUNT-1 in the second dimension
            var aligneePMTs2D = new double[aligneePMTs.Count, PMT_COLUMN_COUNT];

            for (var i = 0; i < datasetFeatures.Count; i++)
            {
                lcmsFeatures2D[i, (int)FeatureColumns.ClassMZ] = datasetFeatures[i].Mz;
                lcmsFeatures2D[i, (int)FeatureColumns.ClassMass] = datasetFeatures[i].MassMonoisotopic;
                lcmsFeatures2D[i, (int)FeatureColumns.ScanClassRep] = datasetFeatures[i].Scan;
                lcmsFeatures2D[i, (int)FeatureColumns.ClassAbundance] = datasetFeatures[i].AbundanceSum;
                lcmsFeatures2D[i, (int)FeatureColumns.PairIndex] = -1;
                lcmsFeatures2D[i, (int)FeatureColumns.PMTTagID] = -1;
                lcmsFeatures2D[i, (int)FeatureColumns.UMCID] = datasetFeatures[i].Id;
            }

            for (var i = 0; i < aligneePMTs.Count; i++)
            {
                aligneePMTs2D[i, (int)PMTColumns.NET] = aligneePMTs[i].Net;
                aligneePMTs2D[i, (int)PMTColumns.MonoisotopicMass] = aligneePMTs[i].MassMonoisotopic;
                aligneePMTs2D[i, (int)PMTColumns.ObservationCount] = aligneePMTs[i].ObservationCount;
                aligneePMTs2D[i, (int)PMTColumns.PMTTagID] = aligneePMTs[i].Id;
            }

            massMatchObject.SetMassLSQOptions(options.MassLSQNumKnots, options.OutlierZScore);

            massMatchObject.SetRegressionOrder(options.MassSplineOrder);
            massMatchObject.SetRecalibrationType((short)options.MassCalibrationType);

            massMatchObject.SetAlignmentType((short)options.AlignmentType);

            var dtStartTime = DateTime.UtcNow;
            var stateBeforeStart = (MassMatchProcessingState)massMatchObject.GetState;

            if (USE_THREADING)
            {
                massMatchObject.MS2MSMSDBAlignPeptidesThreaded(lcmsFeatures2D, aligneePMTs2D);
            }
            else
            {
                // Run on this thread
                massMatchObject.MS2MSMSDBAlignPeptides(lcmsFeatures2D, aligneePMTs2D);
            }

            var keepProcessing = true;

            while (keepProcessing)
            {
                var mMassMatchState = (MassMatchProcessingState)massMatchObject.GetState;

                System.Threading.Thread.Sleep(500);
                var message = massMatchObject.ProgressMessage;
                var secondsElapsed = DateTime.UtcNow.Subtract(dtStartTime).TotalSeconds;
                switch (mMassMatchState)
                {
                    case MassMatchProcessingState.Running:
                        var intPercentComplete = massMatchObject.ProgressPercentComplete;
                        OnStatus(string.Format("{0}: {1}% complete ({2:F0} seconds elapsed)", message, intPercentComplete, secondsElapsed));
                        break;

                    case MassMatchProcessingState.Complete:
                        FinalizeAlignment(massMatchObject, options, secondsElapsed, outputFolder);

                        keepProcessing = false;
                        break;
                    case MassMatchProcessingState.Error:
                    case MassMatchProcessingState.Aborted:
                    case MassMatchProcessingState.InsufficientMatches:
                        keepProcessing = false;
                        message = "Error during MS Warp NET Alignment; state = " + mMassMatchState;

                        if (mMassMatchState == MassMatchProcessingState.InsufficientMatches)
                            OnError("Error during MS Warp NET Alignment; Insufficient matches");
                        else
                            OnStatus(message);

                        break;
                }

            }

            return true;
#endif
        }

        private bool AlignUseCSharp(
            clsAlignmentOptions options,
            IList<FeatureAlignment.Data.Features.UMCLight> datasetFeatures,
            IEnumerable<FeatureAlignment.Data.MassTags.MassTagLight> aligneePMTs,
            DirectoryInfo outputFolder)
        {
            try
            {
                // For now, this program only supports LC data; not drift time
                var separationTypes = new List<FeatureAlignment.Data.Features.SeparationTypes> {
                    FeatureAlignment.Data.Features.SeparationTypes.LC
                };

                var alignmentOptions = new FeatureAlignment.Algorithms.Alignment.LcmsWarp.LcmsWarpAlignmentOptions
                {
                    NumTimeSections = options.NumberOfSections,                 // 100 in VIPER

                    // This should get re-computed when we instantiate LcmsWarp
                    // NumBaselineSections = this.NumTimeSections * this.ContractionFactor,

                    // Note: VIPER takes care of filtering the features prior to saving to disk
                    // Optionally filter more here
                    TopFeatureAbundancePercent = 0,

                    ContractionFactor = options.ContractionFactor,              //   3 in VIPER

                    // This should get re-computed when we instantiate LcmsWarp
                    // MaxExpansionWidth = this.ContractionFactor * this.ContractionFactor,

                    MaxTimeDistortion = options.MaxDistortion,                  //  10 in VIPER
                    MaxPromiscuity = options.MatchPromiscuity,                  //   2 in VIPER
                    UsePromiscuousPoints = true,
                    MassCalibUseLsq = options.MassUseLSQ,                       // false for Dataset to Dataset alignment, true for Dataset to AMT tag alignment
                    MassCalibrationWindow = options.MassWindowPPM,              //  50 ppm in VIPER
                    MassCalibNumXSlices = options.MassNumXSlices,               //  20 in VIPER
                    MassCalibNumYSlices = options.MassNumMassDeltaBins,         // 100 in VIPER
                    MassCalibMaxJump = options.MassMaxJump,                     //  50 in VIPER
                    MassCalibMaxZScore = options.MassZScoreTolerance,           //   3 in VIPER
                    MassCalibLsqMaxZScore = options.OutlierZScore,              //   3 in VIPER
                    MassCalibLsqNumKnots = options.MassLSQNumKnots,             //  12 in VIPER
                    MassTolerance = options.MWTol,                              //  10 in VIPER
                    NetTolerance = options.NETTol,                              //   0.02 in VIPER

                    AlignType = options.AlignmentType,
                    CalibrationType = options.MassCalibrationType,

                    AlignToMassTagDatabase = true,

                    // AMTTagFilterNETMin = 0,
                    // AMTTagFilterNETMax = 0,
                    // AMTTagFilterMassMin = 0,
                    // AMTTagFilterMassMax = 0,

                    MinimumAMTTagObsCount = options.MinimumAMTTagObsCount,      // 5 in VIPER

                    MassBinSize = options.MassHistogramMassBinSizePPM,          // 0.2 in VIPER
                    NetBinSize = options.MassHistogramGANETBinSize,             // 0.001 in VIPER
                    DriftTimeBinSize = options.MassHistogramDriftTimeBinSize,   // 0.05 in VIPER
                    StoreAlignmentFunction = false,
                    AlignmentAlgorithmType = FeatureAlignmentType.LCMS_WARP,
                    SeparationTypes = separationTypes,
                    StandardizeHeatScores = false

                };

                var aligner = new LcmsWarpFeatureAligner(alignmentOptions);

                OnStatus("Alignment starting");
                mAlignmentState = AlignmentState.Unstarted;

                if (!USE_THREADING)
                {
                    // Run the alignment on this thread
                    StartAlignment(aligner, datasetFeatures, aligneePMTs);
                }
                else
                {
                    // Start the alignment in a separate thread
                    var t = Task.Run(() => StartAlignment(aligner, datasetFeatures, aligneePMTs));

                    var keepWaiting = true;
                    while (keepWaiting)
                    {
                        t.Wait(500);

                        OnStatus(string.Format("Aligning, {0:F1}% complete", AlignmentProgress));

                        switch (mAlignmentState)
                        {
                            case AlignmentState.Unstarted:
                            case AlignmentState.Running:
                                break;
                            case AlignmentState.Complete:
                            case AlignmentState.Error:
                                keepWaiting = false;
                                break;
                        }
                    }
                }
                OnStatus("Alignment complete");

                // Save the the alignment results to disk

                ExportResultsCS(outputFolder, alignmentOptions, datasetFeatures, aligner, mAlignmentData);

            }
            catch (Exception ex)
            {
                OnError("Error in AlignUseCSharp: " + ex.Message);
                mAlignmentState = AlignmentState.Error;
            }

            return true;
        }


        private void CreatePlotsCPP()
        {
            /*
            if (AlignmentData == null || AlignmentData.AlignmentData == null)
                return;

            var alignmentData = AlignmentData.AlignmentData;

            var heatmap = HeatmapFactory.CreateAlignedHeatmap(alignmentData.HeatScores, alignmentData.BaselineIsAmtDB);
            var feature = ScatterPlotFactory.CreateFeatureMassScatterPlot(AlignmentData.AligneeFeatures);
            var netHistogram = HistogramFactory.CreateHistogram(alignmentData.NetErrorHistogram, "NET Error", "NET Error");
            var massHistogram = HistogramFactory.CreateHistogram(alignmentData.NetErrorHistogram, "Mass Error", "Mass Error (ppm)");

            var residuals = alignmentData.ResidualData;

            var netResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Net, residuals.LinearCustomNet,
                residuals.LinearNet, "NET Residuals", "Scans", "NET");

            var massMzResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Mz, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");

            var massScanResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Net, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");

            NetScanImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(netResidual));
            MassHistogram = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massHistogram));
            NetHistogram = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(netHistogram));
            HeatmapImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(heatmap));
            MassMzImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massMzResidual));
            MassScanImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massScanResidual));
            FeaturePlotImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(feature));
            */

            throw new NotImplementedException();
        }

        private void DeleteExistingResults(DirectoryInfo outputFolder, string resultSource)
        {

            try
            {
                OnStatus($"Looking for {resultSource} results");

                var outputFileBasePath = GetBaseResultsFilePath(outputFolder);
                var filesToExamine = outputFolder.GetFiles(Path.GetFileName(outputFileBasePath) + "*.txt");

                if (filesToExamine.Length > 0)
                    OnStatus("Deleting files in folder " + outputFolder.FullName);

                foreach (var file in filesToExamine)
                {
                    var deleteFile = false;

                    if (file.Length == 0)
                    {
                        deleteFile = true;
                    }
                    else
                    {
                        // Check the first line for a matching result source
                        using (var reader = new StreamReader(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            if (!reader.EndOfStream)
                            {
                                var firstLine = reader.ReadLine();
                                if (string.Equals(firstLine, "Source=" + resultSource))
                                {
                                    deleteFile = true;
                                }
                            }
                        }
                    }

                    if (deleteFile)
                    {
                        OnStatus(" ... deleting " + file.Name);
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                OnError("Error in DeleteExistingResults: " + ex.Message);
                return;
            }


        }

        private void ExportResultsCPP(
            DirectoryInfo outputFolder,
            double[,] matchScores,
            double[,] alignmentFunc,
            double[,] matches,
            double[,] pepTransformRt,
            double[,] transformRt,
            double[,] massErrorHistogram,
            double[,] netErrorHistogram,
            double computedMinNet,
            double computedMaxNet,
            int computedMinScan,
            int computedMaxScan)
        {
            try
            {
                if (!outputFolder.Exists)
                    outputFolder.Create();

                var resultSource = "Source=C++";
                var outputFileBasePath = GetBaseResultsFilePath(outputFolder);

                var matchScoresFilePath = outputFileBasePath + "_MatchScores.txt";
                var alignmentFunctionFilePath = outputFileBasePath + "_AlignmentFunction.txt";
                var matchesFilePath = outputFileBasePath + "_Matches.txt";
                var umcNETsFilePath = outputFileBasePath + "_UMCNets.txt";
                var scanNETsFilePath = outputFileBasePath + "_ScanNets.txt";
                var massErrorHistogramFilePath = outputFileBasePath + "_MassErrorHistogram.txt";
                var netErrorHistogramFilePath = outputFileBasePath + "_NETErrorHistogram.txt";

                var dataLine = new StringBuilder();

                // mMatchScores
                OnStatus("Exporting match scores for heatmap to: " + matchScoresFilePath);

                WriteMatchScores(matchScoresFilePath, matchScores, computedMinNet, computedMaxNet, computedMinScan, computedMaxScan, resultSource);


                // mAlignmentFunc
                OnStatus("Exporting alignment function to: " + alignmentFunctionFilePath);

                using (var writer = new StreamWriter(new FileStream(alignmentFunctionFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    var headers = new List<string>()
                    {
                        "Index",
                        "Y1",
                        "Y2",
                        "ScanStart",
                        "ScanEnd",
                        "NETStart",
                        "NETEnd",
                        "ScoreA",
                        "ScoreB"
                    };

                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine(string.Join("\t", headers));

                    WriteMatrix(writer, dataLine, alignmentFunc, false);
                }


                // mMatches
                OnStatus("Exporting matches to: " + matchesFilePath);

                using (var writer = new StreamWriter(new FileStream(matchesFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine("UMCIndex" + "\t" + "PMTTag_ID" + "\t" + "NETError" + "\t" + "MassErrorPPM");

                    WriteMatrix(writer, dataLine, matches, false);
                }



                // mPepTransformRT
                OnStatus("Exporting the NET values for all UMCs to: " + umcNETsFilePath);

                using (var writer = new StreamWriter(new FileStream(umcNETsFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    // ReSharper disable once StringLiteralTypo
                    writer.WriteLine("UMCIndex" + "\t" + "UMCNET");

                    var sortedData = SortMatrix(pepTransformRt);
                    WriteMatrix(writer, dataLine, sortedData, false);
                }

                // mTransformRT
                OnStatus("Exporting the NET values for all Scans to: " + scanNETsFilePath);

                using (var writer = new StreamWriter(new FileStream(scanNETsFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine("Scan" + "\t" + "NET");

                    WriteMatrix(writer, dataLine, transformRt, false);
                }


                // mMassErrorHistogram
                OnStatus("Exporting the Mass Error Histogram to: " + massErrorHistogramFilePath);

                using (var writer = new StreamWriter(new FileStream(massErrorHistogramFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine("Mass" + "\t" + "BinCount");

                    WriteMatrix(writer, dataLine, massErrorHistogram, false);
                }

                // mNetErrorHistogram
                OnStatus("Exporting the NET Error Histogram to: " + netErrorHistogramFilePath);

                using (var writer = new StreamWriter(new FileStream(netErrorHistogramFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine("NET" + "\t" + "BinCount");

                    WriteMatrix(writer, dataLine, netErrorHistogram, false);
                }

            }
            catch (Exception ex)
            {
                OnError("Error in ExportResultsCPP: " + ex.Message);
            }

        }

        private double[,] SortMatrix(double[,] matrix)
        {
            var rowCount = matrix.GetUpperBound(0) + 1;
            var colCount = matrix.GetUpperBound(1) + 1;

            var sortedMatrix = new double[rowCount, colCount];

            var sortKeys = new SortedList<double, List<int>>();

            for (var i = 0; i < rowCount; i++)
            {
                if (sortKeys.TryGetValue(matrix[i, 0], out var rowIndices))
                {
                    rowIndices.Add(i);
                } else
                {
                    sortKeys.Add(matrix[i, 0], new List<int> {i});
                }
            }

            var targetRowIndex = 0;
            foreach (var item in sortKeys)
            {
                foreach (var rowIndex in item.Value)
                {
                    for (var j = 0; j < colCount; j++)
                    {
                        sortedMatrix[targetRowIndex, j] = matrix[rowIndex, j];
                    }
                    targetRowIndex++;
                }

            }

            return sortedMatrix;
        }

        private void ExportResultsCS(
            DirectoryInfo outputFolder,
            FeatureAlignment.Algorithms.Alignment.LcmsWarp.LcmsWarpAlignmentOptions alignmentOptions,
            IEnumerable<FeatureAlignment.Data.Features.UMCLight> datasetFeatures,
            LcmsWarpFeatureAligner aligner,
            AlignmentData alignmentResult)
        {

            try
            {

                if (!outputFolder.Exists)
                    outputFolder.Create();

                var resultSource = "Source=C#";

                var outputFileBasePath = GetBaseResultsFilePath(outputFolder);

                var matchScoresFilePath = outputFileBasePath + "_MatchScores.txt";
                var alignmentFunctionFilePath = outputFileBasePath + "_AlignmentFunction.txt";
                var matchesFilePath = outputFileBasePath + "_Matches.txt";
                var umcNETsFilePath = outputFileBasePath + "_UMCNets.txt";
                var scanNETsFilePath = outputFileBasePath + "_ScanNETs.txt";
                var massErrorHistogramFilePath = outputFileBasePath + "_MassErrorHistogram.txt";
                var netErrorHistogramFilePath = outputFileBasePath + "_NETErrorHistogram.txt";

                // mMatchScores
                OnStatus("Exporting match scores for heatmap to: " + matchScoresFilePath);

                WriteMatchScores(
                    matchScoresFilePath, alignmentResult.HeatScores,
                    alignmentResult.MinMTDBNET, alignmentResult.MaxMTDBNET,
                    alignmentResult.MinScanBaseline, alignmentResult.MaxScanBaseline,
                    resultSource);

                // mAlignmentFunc
                OnStatus("Exporting alignment function to: " + alignmentFunctionFilePath);

                using (var writer = new StreamWriter(new FileStream(alignmentFunctionFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {

                    var alignmentFunction = alignmentResult.AlignmentFunction;

                    if (alignmentOptions.AlignType == LcmsWarpAlignmentType.NET_WARP)
                    {
                        // AlignmentProcessor called: func.SetNetFunction(aligneeNets, referenceNets);
                    }
                    else
                    {
                        // Used LcmsWarpAlignmentType.NET_MASS_WARP

                        if (alignmentOptions.CalibrationType == FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.NetRegression ||
                            alignmentOptions.CalibrationType == FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.Both)
                        {
                            // AlignmentProcessor called: func.SetMassCalibrationFunctionWithNet(aligneeNetMassFunc, aligneePpmShiftMassFunc);
                        }

                        if (alignmentOptions.CalibrationType == FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.MzRegression ||
                            alignmentOptions.CalibrationType == FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.Both)
                        {
                            // AlignmentProcessor called: func.SetMassCalibrationFunctionWithMz(aligneeMzMassFunc, aligneePpmShiftMassFunc);
                        }
                    }

                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine("Index" + "\t" + "Scan" + "\t" + "NET");

                    var dataIndex = 0;
                    foreach (var dataPoint in alignmentFunction.NetAlignmentFunction)
                    {
                        writer.WriteLine(
                            dataIndex + "\t" +
                            StringUtilities.ValueToString(dataPoint.Key, 2) + "\t" +
                            StringUtilities.ValueToString(dataPoint.Value, 5));
                        dataIndex++;
                    }

                }

                // mMatches
                OnStatus("Exporting matches to: " + matchesFilePath);

                using (var writer = new StreamWriter(new FileStream(matchesFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine(
                        "UMCIndex" + "\t" + "PMTTag_ID" + "\t" + "NETError" + "\t" +
                        "MassErrorPPM Original" + "\t" + "MassErrorPPM AfterAlignment" + "\t" +
                        "Feature_Index" + "\t" + "PMTTag_Index");

                    foreach (var match in alignmentResult.FeatureMatches)
                    {

                        writer.WriteLine(
                            match.AligneeFeature.Id + "\t" +
                            match.BaselineFeature.Id.ToString("0") + "\t" +
                            StringUtilities.ValueToString(match.NetError, 5) + "\t" +
                            StringUtilities.ValueToString(match.PpmMassErrorOriginal, 5) + "\t" +
                            StringUtilities.ValueToString(match.PpmMassError, 5) + "\t" +
                            match.FeatureIndex + "\t" +
                            match.BaselineFeatureIndex);
                    }
                }

                var alignerScanToNET = aligner.ScanToNETMap;

                // mPepTransformRT
                OnStatus("Exporting the NET values for all UMCs to: " + umcNETsFilePath);

                using (var writer = new StreamWriter(new FileStream(umcNETsFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    // ReSharper disable once StringLiteralTypo
                    writer.WriteLine("UMCIndex" + "\t" + "UMCNET" + "\t" + "AlignedMass");

                    foreach (var feature in (from item in datasetFeatures orderby item.Id select item))
                    {
                        writer.WriteLine(
                            feature.Id + "\t" +
                            StringUtilities.ValueToString(feature.NetAligned, 5) + "\t" +
                            StringUtilities.ValueToString(feature.MassMonoisotopicAligned, 5));

                        if (!alignerScanToNET.TryGetValue(feature.Scan, out var computedNET))
                        {
                            // Scan not found; this is unexpected
                            computedNET = double.MinValue;
                        }

                        var netDiff = Math.Abs(computedNET - feature.NetAligned);

                        if (netDiff > 0.01)
                        {
                            Console.WriteLine(
                                "Warning: Difference between .ScanToNETMap and stored NET is larger than 0.01: " +
                                StringUtilities.ValueToString(computedNET, 4) + " vs. " +
                                StringUtilities.ValueToString(feature.NetAligned, 4));
                        }
                    }
                }

                // mTransformRT
                OnStatus("Exporting the NET values for all Scans to: " + scanNETsFilePath);

                using (var writer = new StreamWriter(new FileStream(scanNETsFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine("Scan" + "\t" + "Interpolated_NET" + "\t" + "Aligned_NET");

                    var scanStart = 1;
                    var scanEnd = alignerScanToNET.Keys.Max();

                    for (var scan = scanStart; scan <= scanEnd; scan++)
                    {
                        string computedNET;
                        if (alignerScanToNET.ContainsKey(scan))
                        {
                            computedNET = StringUtilities.ValueToString(alignerScanToNET[scan], 5);
                        }
                        else
                        {
                            computedNET = "";
                        }

                        var computedNET2 = alignmentResult.AlignmentFunction.GetInterpolatedNet(scan);

                        writer.WriteLine(
                            scan + "\t" +
                            StringUtilities.ValueToString(computedNET2, 5) + "\t" +
                            computedNET);

                    }
                }


                // mMassErrorHistogram
                OnStatus("Exporting the Mass Error Histogram to: " + massErrorHistogramFilePath);

                using (var writer = new StreamWriter(new FileStream(massErrorHistogramFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine("Mass" + "\t" + "BinCount");

                    foreach (var dataPoint in alignmentResult.MassErrorHistogram)
                    {
                        writer.WriteLine(StringUtilities.ValueToString(dataPoint.Key, 4) + "\t" + dataPoint.Value);
                    }
                }

                // mNetErrorHistogram
                OnStatus("Exporting the NET Error Histogram to: " + netErrorHistogramFilePath);

                using (var writer = new StreamWriter(new FileStream(netErrorHistogramFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
                {
                    // Result source
                    writer.WriteLine(resultSource);

                    // Header line
                    writer.WriteLine("NET" + "\t" + "BinCount");

                    foreach (var dataPoint in alignmentResult.NetErrorHistogram)
                    {
                        writer.WriteLine(StringUtilities.ValueToString(dataPoint.Key, 5) + "\t" + dataPoint.Value);
                    }
                }

            }
            catch (Exception ex)
            {
                OnError("Error in ExportResultsCS: " + ex.Message);
            }


        }

        private static void WriteMatchScores(
            string matchScoresFilePath,
            double[,] matchScores,
            double computedMinNet,
            double computedMaxNet,
            int computedMinScan,
            int computedMaxScan,
            string resultSource)
        {

            var dataLine = new StringBuilder();
            using (var writer = new StreamWriter(new FileStream(matchScoresFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)))
            {
                // Result source
                writer.WriteLine(resultSource);

                // Score bounds
                writer.WriteLine("MinScan=" + computedMinScan);
                writer.WriteLine("MaxScan=" + computedMaxScan);
                writer.WriteLine("MinNet=" + StringUtilities.ValueToString(computedMinNet, 5));
                writer.WriteLine("MaxNet=" + StringUtilities.ValueToString(computedMaxNet, 5));

                // Header line for the data section
                writer.WriteLine("Scores");

                WriteMatrix(writer, dataLine, matchScores);
            }
        }

        private static void WriteMatrix(TextWriter writer, StringBuilder dataLine, double[,] matrix, bool columnMajor=true)
        {
            if (columnMajor)
            {
                var rowCount = matrix.GetUpperBound(1) + 1;
                var colCount = matrix.GetUpperBound(0) + 1;

                for (var i = 0; i < rowCount; i++)
                {
                    dataLine.Clear();
                    for (var j = 0; j < colCount; j++)
                    {
                        if (j > 0)
                            dataLine.Append("\t");

                        if (matrix[j, i] > 999999 && Math.Abs(matrix[j, i] - (int)matrix[j, i]) < 0.0001)
                        {
                            // Large integer value
                            dataLine.Append(((int)matrix[j, i]).ToString("0"));
                        }
                        else
                        {
                            // Any other number, including doubles
                            dataLine.Append(StringUtilities.ValueToString(matrix[j, i], 5));
                        }
                    }
                    writer.WriteLine(dataLine);
                }
            }
            else
            {
                var rowCount = matrix.GetUpperBound(0) + 1;
                var colCount = matrix.GetUpperBound(1) + 1;

                for (var i = 0; i < rowCount; i++)
                {
                    dataLine.Clear();
                    for (var j = 0; j < colCount; j++)
                    {
                        if (j > 0)
                            dataLine.Append("\t");

                        if (matrix[i, j] > 999999 && Math.Abs(matrix[i, j] - (int)matrix[i, j]) < 0.0001)
                        {
                            // Large integer value
                            dataLine.Append(((int)matrix[i, j]).ToString("0"));
                        }
                        else
                        {
                            // Any other number, including doubles
                            dataLine.Append(StringUtilities.ValueToString(matrix[i, j], 5));
                        }

                    }
                    writer.WriteLine(dataLine);
                }
            }
        }

        private string GetBaseResultsFilePath(FileSystemInfo outputFolder)
        {
            var outputFileBasePath = Path.Combine(outputFolder.FullName, "LCMSWarpResults");
            return outputFileBasePath;
        }
#if CPP_ENABLED
        private void FinalizeAlignment(CMassMatchWrapper massMatchObject, clsAlignmentOptions options, double secondsElapsed, DirectoryInfo outputFolder)
        {

            bool blnUpdateMassValues;

            // 2D Array of variants, ranging from 0 to UBound(mMatches, 1) in the first dimension and containing 0 to 3 in the 2nd dimension
            double[,] matches;

            // 2D Array of variants, ranging from 0 to UBound(mMatchScores, 1) in the first dimension and 0 to UBound(mMatchScores, 2) in the second dimension
            // The number of rows corresponds to the number of time slices; the number of columns corresponds to the number of regions the NET scale was divided into
            double[,] matchScores;

            // 2D Array with the alignment function values, ranging from UBound(mAlignmentFunc, 1) in the first dimension and from 0 to 8 in the 2nd dimension
            // Columns for the 2nd dimension are given by afAlignmentFuncColumnConstants
            double[,] alignmentFunc;

            // 2D array ranging from 0 to UBound(mAlignmentFunc, 1) in the first dimension and 0 to 1 in the 2nd dimension
            double[,] transformRT;

            // UMC to Transformed NET mapping
            // 2D array ranging from 0 to mLocalFeatureCount-1 in the first dimension and 0 to 1 in the 2nd dimension
            double[,] pepTransformRT;

            // Mass Error Histogram
            // 2D array of doubles with X values in the first dimension and Y values in the second dimension
            double[,] massErrorHistogram;

            // NET Error Histogram
            // 2D array of doubles with X values in the first dimension and Y values in the second dimension
            double[,] netErrorHistogram;

            double computedMinNet;
            double computedMaxNet;
            int computedMinScan;
            int computedMaxScan;

            try
            {

                // Obtain the alignment results
                massMatchObject.GetResults(out matchScores, out alignmentFunc, out matches, out pepTransformRT,
                                           out transformRT,
                                           out massErrorHistogram, out netErrorHistogram,
                                           options.MassHistogramMassBinSizePPM,
                                           options.MassHistogramGANETBinSize);


                if (options.AlignmentType == LcmsWarpAlignmentType.NET_MASS_WARP)
                    blnUpdateMassValues = true;
                else
                    blnUpdateMassValues = false;

                // Find range of features and MT tags
                massMatchObject.GetBounds(out computedMinNet, out computedMaxNet, out computedMinScan,
                                          out computedMaxScan);

                var numMatched = matches.GetLength(0) + 1;


                OnStatus(string.Format("Processing: 100% complete ({0} seconds elapsed)", secondsElapsed));

                // Save the results to disk
                ExportResultsCPP(outputFolder, matchScores, alignmentFunc, matches,
                    pepTransformRT, transformRT,
                    massErrorHistogram, netErrorHistogram,
                    computedMinNet, computedMaxNet, computedMinScan, computedMaxScan);

            }
            catch (Exception ex)
            {
                OnError("Error in FinalizeAlignment: " + ex.Message);
                return;
            }

        }
#endif

        private short ImputeCharge(double mz, double monoisotopicMass)
        {
            short bestCharge = 0;
            var bestChargeDelMz = double.MaxValue;

            for (short charge = 1; charge < 30; charge++)
            {
                var candidateMz = (monoisotopicMass + PROTON_MASS * charge) / charge;
                var delMz = Math.Abs(candidateMz - mz);
                if (delMz < bestChargeDelMz)
                {
                    bestCharge = charge;
                    bestChargeDelMz = delMz;

                    if (Math.Abs(delMz) < 0.05)
                        break;
                }
            }

            return bestCharge;
        }

        private clsAlignmentOptions LoadOptions(string optionsFilePath)
        {

            var alignmentOptions = new clsAlignmentOptions();

            try
            {
                if (string.IsNullOrWhiteSpace(optionsFilePath))
                    throw new ArgumentException("optionsFilePath");

                var optionsFile = new FileInfo(optionsFilePath);
                if (!optionsFile.Exists)
                {
                    OnError("File not found: " + optionsFile.FullName);
                    return null;
                }

                using (var reader = new StreamReader(new FileStream(optionsFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    while (!reader.EndOfStream)
                    {
                        var dataLine = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(dataLine))
                            continue;

                        var optionParts = dataLine.Split('=');
                        if (optionParts.Length < 2)
                            continue;

                        switch (optionParts[0].Trim())
                        {
                            case "NumberOfSections":
                                alignmentOptions.NumberOfSections = ParseInt(optionParts[1], alignmentOptions.NumberOfSections);
                                break;
                            case "MSWarpOptions_ContractionFactor":
                                alignmentOptions.ContractionFactor = (short)ParseInt(optionParts[1], alignmentOptions.ContractionFactor);
                                break;
                            case "MSWarpOptions_MaxDistortion":
                                alignmentOptions.MaxDistortion = (short)ParseInt(optionParts[1], alignmentOptions.MaxDistortion);
                                break;
                            case "MSWarpOptions_NETTol":
                                alignmentOptions.NETTol = ParseDbl(optionParts[1], alignmentOptions.NETTol);
                                break;
                            case "NET_MIN":
                                alignmentOptions.NET_MIN = ParseDbl(optionParts[1], alignmentOptions.NET_MIN);
                                break;
                            case "NET_MAX":
                                alignmentOptions.NET_MAX = ParseDbl(optionParts[1], alignmentOptions.NET_MAX);
                                break;
                            case "MSWarpOptions_MatchPromiscuity":
                                alignmentOptions.MatchPromiscuity = ParseInt(optionParts[1], alignmentOptions.MatchPromiscuity);
                                break;
                            case "MWTol":
                                alignmentOptions.MWTol = ParseDbl(optionParts[1], alignmentOptions.MWTol);
                                break;
                            case "MSWarpOptions_MassNumMassDeltaBins":
                                alignmentOptions.MassNumMassDeltaBins = ParseInt(optionParts[1], alignmentOptions.MassNumMassDeltaBins);
                                break;
                            case "MSWarpOptions_MassWindowPPM":
                                alignmentOptions.MassWindowPPM = ParseInt(optionParts[1], alignmentOptions.MassWindowPPM);
                                break;
                            case "MSWarpOptions_MassMaxJump":
                                alignmentOptions.MassMaxJump = ParseInt(optionParts[1], alignmentOptions.MassMaxJump);
                                break;
                            case "MSWarpOptions_MassNumXSlices":
                                alignmentOptions.MassNumXSlices = ParseInt(optionParts[1], alignmentOptions.MassNumXSlices);
                                break;
                            case "MSWarpOptions_MassZScoreTolerance":
                                alignmentOptions.MassZScoreTolerance = ParseDbl(optionParts[1], alignmentOptions.MassZScoreTolerance);
                                break;
                            case "MSWarpOptions_MassUseLSQ":
                                if (bool.TryParse(optionParts[1], out var useLSQ))
                                {
                                    alignmentOptions.MassUseLSQ = useLSQ;
                                }
                                break;
                            case "MSWarpOptions_MassLSQNumKnots":
                                alignmentOptions.MassLSQNumKnots = (short)ParseInt(optionParts[1], alignmentOptions.MassLSQNumKnots);
                                break;
                            case "MassCalibrationType":
                                var massCalibrationType = ParseInt(optionParts[1], (int)alignmentOptions.MassCalibrationType);

                                switch (massCalibrationType)
                                {
                                    case (int)FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.MzRegression:
                                        alignmentOptions.MassCalibrationType = FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.MzRegression;
                                        break;
                                    case (int)FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.NetRegression:
                                        alignmentOptions.MassCalibrationType = FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.NetRegression;
                                        break;
                                    case (int)FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.Both:
                                        alignmentOptions.MassCalibrationType = FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.Both;
                                        break;
                                    default:
                                        alignmentOptions.MassCalibrationType = FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration.LcmsWarpCalibrationType.Both;
                                        break;
                                }

                                break;
                            case "MassSplineOrder":
                                // This option appears to be unused by LCMSWarp
                                alignmentOptions.MassSplineOrder = (short)ParseInt(optionParts[1], alignmentOptions.MassSplineOrder);
                                break;
                            case "AlignmentType":
                                var alignmentType = ParseInt(optionParts[1], (int)alignmentOptions.AlignmentType);


                                switch (alignmentType)
                                {
                                    case (int)LcmsWarpAlignmentType.NET_WARP:
                                        alignmentOptions.AlignmentType = LcmsWarpAlignmentType.NET_WARP;
                                        break;
                                    case (int)LcmsWarpAlignmentType.NET_MASS_WARP:
                                        alignmentOptions.AlignmentType = LcmsWarpAlignmentType.NET_MASS_WARP;
                                        break;
                                    default:
                                        alignmentOptions.AlignmentType = LcmsWarpAlignmentType.NET_MASS_WARP;
                                        break;
                                }

                                break;
                            case "MinimumAMTTagObsCount":
                                alignmentOptions.MinimumAMTTagObsCount = ParseInt(optionParts[1], alignmentOptions.MinimumAMTTagObsCount);
                                break;
                            case "MassBinSize":
                                alignmentOptions.MassHistogramMassBinSizePPM = ParseDbl(optionParts[1], alignmentOptions.MassHistogramMassBinSizePPM);
                                break;
                            case "NetBinSize":
                                alignmentOptions.MassHistogramGANETBinSize = ParseDbl(optionParts[1], alignmentOptions.MassHistogramGANETBinSize);
                                break;
                            case "DriftTimeBinSize":
                                alignmentOptions.MassHistogramDriftTimeBinSize = ParseDbl(optionParts[1], alignmentOptions.MassHistogramDriftTimeBinSize);
                                break;
                            default:
                                OnStatus("Warning: unrecognized option: " + dataLine);
                                break;
                        }
                    }
                }

                return alignmentOptions;
            }
            catch (Exception ex)
            {
                OnError("Error loading options: " + ex.Message);
                return null;
            }

        }

        private List<FeatureAlignment.Data.Features.UMCLight> LoadLCMSFeatures(string lcmsFeaturesFilePath)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(lcmsFeaturesFilePath))
                    throw new ArgumentException("lcmsFeaturesFilePath");

                var lcmsFeaturesFile = new FileInfo(lcmsFeaturesFilePath);
                if (!lcmsFeaturesFile.Exists)
                {
                    OnError("File not found: " + lcmsFeaturesFile.FullName);
                    return null;
                }

                var lcmsFeatures = new List<FeatureAlignment.Data.Features.UMCLight>();
                var lineNumber = 0;

                using (var reader = new StreamReader(new FileStream(lcmsFeaturesFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    var expectedHeaders = new List<string>()
                    {
                        "ClassMZ",
                        "ClassMass",
                        "ScanClassRep",
                        "ClassAbundance",
                        "PairIndex",
                        "PMTTagID",
                        "UMCID"
                    };

                    var headerLine = reader.ReadLine();
                    if (!ValidateHeaderLine(lcmsFeaturesFile.Name, headerLine, expectedHeaders))
                    {
                        return null;
                    }

                    while (!reader.EndOfStream)
                    {
                        var dataLine = reader.ReadLine();
                        lineNumber++;

                        if (string.IsNullOrWhiteSpace(dataLine))
                            continue;

                        var dataCols = dataLine.Split('\t');
                        if (dataCols.Length < FEATURE_COLUMN_COUNT)
                        {
                            OnStatus(string.Format("LCMSFeatures data line {0} has fewer than {1} columns; ignoring", lineNumber, FEATURE_COLUMN_COUNT));
                            continue;
                        }

                        var umcLight = new FeatureAlignment.Data.Features.UMCLight
                        {
                            Mz = ParseColumnDbl(dataCols, (int)FeatureColumns.ClassMZ, 0),
                            MassMonoisotopic = ParseColumnDbl(dataCols, (int)FeatureColumns.ClassMass, 0),
                            Scan = ParseColumnInt(dataCols, (int)FeatureColumns.ScanClassRep, 0),
                            AbundanceSum = ParseColumnDbl(dataCols, (int)FeatureColumns.ClassAbundance, 0),
                            // Skip: ParseColumnInt(dataCols, (int)FeatureColumns.PairIndex, 0);
                            // Skip: ParseColumnInt(dataCols, (int)FeatureColumns.PMTTagID, 0);
                            Id = ParseColumnInt(dataCols, (int)FeatureColumns.UMCID, 0)
                        };

                        umcLight.Abundance = umcLight.AbundanceSum;
                        umcLight.ScanStart = umcLight.Scan;
                        umcLight.ScanEnd = umcLight.Scan;
                        umcLight.ChargeState = ImputeCharge(umcLight.Mz, umcLight.MassMonoisotopic);

                        lcmsFeatures.Add(umcLight);

                    }
                }

                return lcmsFeatures;
            }
            catch (Exception ex)
            {
                OnError("Error loading LCMSFeatures: " + ex.Message);
                return null;
            }


        }

        private List<FeatureAlignment.Data.MassTags.MassTagLight> LoadAligneePMTs(string aligneePMTsFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(aligneePMTsFilePath))
                    throw new ArgumentException("aligneePMTsFilePath");

                // ReSharper disable once IdentifierTypo
                var pmtsFile = new FileInfo(aligneePMTsFilePath);
                if (!pmtsFile.Exists)
                {
                    OnError("File not found: " + pmtsFile.FullName);
                    return null;
                }

                var aligneePMTs = new List<FeatureAlignment.Data.MassTags.MassTagLight>();
                var lineNumber = 0;

                using (var reader = new StreamReader(new FileStream(pmtsFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    // Read and validate the header line
                    if (reader.EndOfStream)
                    {
                        OnError("Error loading alignee PMTs: empty file");
                        return null;
                    }

                    var expectedHeaders = new List<string>()
                    {
                        "NET",
                        "MonoisotopicMass",
                        "ObservationCount",
                        "PMTTagID"
                    };

                    var headerLine = reader.ReadLine();
                    if (!ValidateHeaderLine(pmtsFile.Name, headerLine, expectedHeaders))
                    {
                        return null;
                    }

                    while (!reader.EndOfStream)
                    {
                        var dataLine = reader.ReadLine();
                        lineNumber++;

                        if (string.IsNullOrWhiteSpace(dataLine))
                            continue;

                        var dataCols = dataLine.Split('\t');
                        if (dataCols.Length < PMT_COLUMN_COUNT)
                        {
                            OnStatus(string.Format("PMT Alignee data line {0} has fewer than {1} columns; ignoring", lineNumber, FEATURE_COLUMN_COUNT));
                            continue;
                        }

                        var amtTag = new FeatureAlignment.Data.MassTags.MassTagLight
                        {
                            Net = ParseColumnDbl(dataCols, (int)PMTColumns.NET, 0),
                            MassMonoisotopic = ParseColumnDbl(dataCols, (int)PMTColumns.MonoisotopicMass, 0),
                            ObservationCount = ParseColumnInt(dataCols, (int)PMTColumns.ObservationCount, 0),
                            Id = ParseColumnInt(dataCols, (int)PMTColumns.PMTTagID, 0)
                        };

                        aligneePMTs.Add(amtTag);

                    }
                }

                return aligneePMTs;
            }
            catch (Exception ex)
            {
                OnError("Error loading alignee PMTs: " + ex.Message);
                return null;
            }
        }

        private double ParseColumnDbl(IReadOnlyList<string> dataCols, int columnIndex, double defaultValue)
        {
            var result = ParseDbl(dataCols[columnIndex], defaultValue);
            return result;
        }

        private int ParseColumnInt(IReadOnlyList<string> dataCols, int columnIndex, int defaultValue)
        {
            var result = ParseInt(dataCols[columnIndex], defaultValue);
            return result;
        }

        private int ParseInt(string valueText, int defaultValue)
        {
            if (int.TryParse(valueText, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        private double ParseDbl(string valueText, double defaultValue)
        {
            if (double.TryParse(valueText, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Start the LCMSWarp alignment
        /// </summary>
        /// <param name="aligner">Aligner class</param>
        /// <param name="datasetFeatures">LC-MS Features to align to the baseline</param>
        /// <param name="aligneePMTs">Base dataset or AMT tags to align to</param>
        private void StartAlignment(
            LcmsWarpFeatureAligner aligner,
            IEnumerable<FeatureAlignment.Data.Features.UMCLight> datasetFeatures,
            IEnumerable<FeatureAlignment.Data.MassTags.MassTagLight> aligneePMTs)
        {

            var datasetProgress =
                   new Progress<PRISM.ProgressData>(
                       pd =>
                       {
                           AlignmentProgress = pd.Percent;
                           OnProgress(pd.Percent);
                       });

            try
            {
                mAlignmentState = AlignmentState.Running;

                aligner.Progress += aligner_Progress;

                var alignmentData = aligner.Align(aligneePMTs, datasetFeatures, datasetProgress);

                mAlignmentState = AlignmentState.Complete;

                mAlignmentData = alignmentData;
            }
            catch (Exception ex)
            {
                OnError("Error in alignment: " + ex.Message);
                OnError(ex.StackTrace);
                mAlignmentState = AlignmentState.Error;
            }


        }

        void aligner_Progress(object sender, FeatureAlignment.Algorithms.ProgressNotifierArgs e)
        {
            OnStatus(e.Message + ": " + e.PercentComplete.ToString("0.00") + "%");
        }

        private bool ValidateHeaderLine(string fileName, string headerLine, IReadOnlyList<string> expectedHeaders)
        {
            var validHeaders = true;
            var unknownColumnName = string.Empty;

            if (string.IsNullOrWhiteSpace(headerLine))
            {
                validHeaders = false;
            }
            else
            {
                var dataCols = headerLine.Split('\t');
                if (dataCols.Length < expectedHeaders.Count)
                {
                    validHeaders = false;
                }
                else
                {
                    for (var i = 0; i < expectedHeaders.Count; i++)
                    {
                        if (!string.Equals(dataCols[i], expectedHeaders[i]))
                        {
                            unknownColumnName = dataCols[i];
                            break;
                        }
                    }
                }
            }

            if (validHeaders)
                return true;

            OnError("Header line in file " + fileName + " should be: " + string.Join("\t", expectedHeaders));
            if (!string.IsNullOrWhiteSpace(unknownColumnName))
                OnError("Unrecognized column name: " + unknownColumnName);

            return false;
        }

        private void OnProgress(double percent)
        {
            // Redundant progress; ignore it
            // OnStatus("Progress update: " + percent.ToString("0.0"));
        }

        private void OnError(string message)
        {
            // Future: raise an event

            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine(message);
            Console.WriteLine("-----------------------------------------------");
        }

        private void OnStatus(string message)
        {
            // Future: raise an event

            Console.WriteLine(message);
        }

    }
}
