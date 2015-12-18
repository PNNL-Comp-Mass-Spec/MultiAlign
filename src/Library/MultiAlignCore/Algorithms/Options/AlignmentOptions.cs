#region

using System.ComponentModel;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.Parameters;

#endregion

namespace MultiAlignCore.Algorithms.Options
{
    using MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration;

    public class AlignmentOptions
    {
        public AlignmentOptions()
        {
            LCMSWarpOptions = new LcmsWarpAlignmentOptions();
            AlignmentBaselineName = "";
            MassTagObservationCount = 5;
            AlignToAMT = false;
        }

        #region Fields

        private bool m_isAlignmentBaselineAMasstagDB;

        public LcmsWarpAlignmentOptions LCMSWarpOptions { get; set; }

        #endregion

        #region Properties
        [ParameterFile("AlignmentAlgorithmType", "Alignment")]
        [Category("Algorithm")]
        [Description("Type of algorithm to use for alignment.")]
        public FeatureAlignmentType AlignmentAlgorithm { get { return LCMSWarpOptions.AlignmentAlgorithmType; } set { LCMSWarpOptions.AlignmentAlgorithmType = value; } }

        /// <summary>
        /// Gets or sets whether to store alignment data.
        /// </summary>
        [ParameterFile("ShouldStoreAlignmentFunction", "Alignment")]
        [Description(
            "Stores the alignment function per dataset.  Optional only.  Set to false if processing a large amount of data."
            )]
        public bool ShouldStoreAlignmentFunction { get { return LCMSWarpOptions.StoreAlignmentFunction; } set { LCMSWarpOptions.StoreAlignmentFunction = value; } }

        [ParameterFile("TopFeatureAbundancePercent", "Alignment")]
        [Description("Percentage of top Abundance features to use for alignment (value between 0 and 100). Set to 0 means all features are matched, set to 100 means no features are matched, set to 33 the top 67% of features sorted by abundance are matched.")]
        public int TopFeatureAbundancePercent { get { return LCMSWarpOptions.TopFeatureAbundancePercent; } set { LCMSWarpOptions.TopFeatureAbundancePercent = value; } }

        [Browsable(false)]
        public bool IsAlignmentBaselineAMasstagDB
        {
            get { return m_isAlignmentBaselineAMasstagDB; }
            set
            {
                if (value != m_isAlignmentBaselineAMasstagDB)
                {
                    m_isAlignmentBaselineAMasstagDB = value;
                    OnNotify("IsAlignmentBaselineAMasstagDB");
                }
            }
        }

        /// <summary>
        ///     Minimum number of observations to use for alignment.
        /// </summary>
        [ParameterFile("MassTagObservationCount", "Alignment")]
        [Category("Filtering")]
        [Description("Minimum number of LC-MS/MS sample runs that the mass tag must have been seen in.")]
        public int MassTagObservationCount { get; set; }

        [Browsable(false)]
        public string AlignmentBaselineName { get; set; }

        [ParameterFile("MassCalibrationLSQNumKnots", "Alignment")]
        [Category("Alignment Function")]
        [Description("Determines how many least square knots should be used.")]
        public int MassCalibrationLSQNumKnots { get { return LCMSWarpOptions.MassCalibLsqNumKnots; } set { LCMSWarpOptions.MassCalibLsqNumKnots = value; } }

        [ParameterFile("HistogramDriftTimeBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Histogram size of alignment (ms)")]
        public double DriftTimeBinSize { get { return LCMSWarpOptions.DriftTimeBinSize; } set { LCMSWarpOptions.DriftTimeBinSize = value; } }

        [ParameterFile("HistogramNETBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Bin size for NET error histograms.")]
        public double NETBinSize { get { return LCMSWarpOptions.NetBinSize; } set { LCMSWarpOptions.NetBinSize = value; } }

        [ParameterFile("HistogramMassBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Histogram size of alignment in parts per million (PPM)")]
        public double MassBinSize { get { return LCMSWarpOptions.MassBinSize; } set { LCMSWarpOptions.MassBinSize = value; } }

        [ParameterFile("AlignmentType", "Alignment")]
        [Category("General Calibration")]
        [Description("Determines if NET only, or Mass and NET alignment should be performed.")]
        public LcmsWarpAlignmentType AlignmentType { get { return LCMSWarpOptions.AlignType; } set { LCMSWarpOptions.AlignType = value; } }

        [ParameterFile("RecalibrationType", "Alignment")]
        [Category("General Calibration")]
        [Description("Type of recalibration to perform, NET only, or NET and Mass")]
        public LcmsWarpCalibrationType RecalibrationType { get { return LCMSWarpOptions.CalibrationType; } set { LCMSWarpOptions.CalibrationType = value; } }

        // Legacy
        // [ParameterFile("SplitAlignmentMZ", "Alignment")]
        // [Category("General Calibration")]
        // [Description("Determines whether the m/z boundaries should be used for analysis.  False if recommended.")]
        // public bool SplitAlignmentInMZ { get; set; }

        // Legacy
        // [Category("General Calibration")]
        // [Description("m/z calibration ranges if the detector is not calibrated.  Not normal mode of operation")]
        // public List<classAlignmentMZBoundary> MZBoundaries { get; set; }

        [ParameterFile("MassCalibrationLSQZScore", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Determines the z-score cutoff for mass calibration")]
        public double MassCalibrationLSQZScore { get { return LCMSWarpOptions.MassCalibLsqMaxZScore; } set { LCMSWarpOptions.MassCalibLsqMaxZScore = value; } }

        [ParameterFile("MassCalibrationMaxJump", "Alignment")]
        [Category("Mass Calibration")]
        [Description("")]
        public int MassCalibrationMaxJump { get { return LCMSWarpOptions.MassCalibMaxJump; } set { LCMSWarpOptions.MassCalibMaxJump = value; } }

        [ParameterFile("MassCalibrationMaxZScore", "Alignment")]
        [Category("Mass Calibration")]
        [Description("")]
        public double MassCalibrationMaxZScore { get { return LCMSWarpOptions.MassCalibMaxZScore; } set { LCMSWarpOptions.MassCalibMaxZScore = value; } }

        [ParameterFile("MassCalibrationNumMassDeltaBins", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Number of divisions to make in the mass dimension.")]
        public int MassCalibrationNumMassDeltaBins { get { return LCMSWarpOptions.MassCalibNumYSlices; } set { LCMSWarpOptions.MassCalibNumYSlices = value; } }

        [ParameterFile("MassCalibrationNumXSlices", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Number of divisions to make in NET.")]
        public int MassCalibrationNumXSlices { get { return LCMSWarpOptions.MassCalibNumXSlices; } set { LCMSWarpOptions.MassCalibNumXSlices = value; } }

        [ParameterFile("MassCalibrationUseLSQ", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Determines whether least squares is used for the fit.")]
        public bool MassCalibrationUseLSQ { get { return LCMSWarpOptions.MassCalibUseLsq; } set { LCMSWarpOptions.MassCalibUseLsq = value; } }

        [ParameterFile("MassCalibrationWindow", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Mass Calibration window to use in parts per million (PPM)")]
        public double MassCalibrationWindow { get { return LCMSWarpOptions.MassCalibrationWindow; } set { LCMSWarpOptions.MassCalibrationWindow = value; } }

        [ParameterFile("MaxPromiscuity", "Alignment")]
        [Category("Matching")]
        [Description(
            "Total number of candidate matches that can be allowed for a single LC-MS Features before being discarded.")
        ]
        public int MaxPromiscuity { get { return LCMSWarpOptions.MaxPromiscuity; } set { LCMSWarpOptions.MaxPromiscuity = value; } }

        [ParameterFile("UsePromiscuousPoints", "Alignment")]
        [Category("Matching")]
        [Description(
            "Determines whether features can match to multiple features or whether they are considered ambiguous.")]
        public bool UsePromiscuousPoints { get { return LCMSWarpOptions.UsePromiscuousPoints; } set { LCMSWarpOptions.UsePromiscuousPoints = value; } }

        [ParameterFile("MassTolerance", "Alignment")]
        [Category("Tolerances")]
        [Description("Mass tolerance in parts per million (PPM)")]
        public double MassTolerance { get { return LCMSWarpOptions.MassTolerance; } set { LCMSWarpOptions.MassTolerance = value; } }

        [ParameterFile("MaxTimeJump", "Alignment")]
        [Category("Tolerances")]
        [Description("Largest scan range allowed for a feature match.")]
        public int MaxTimeJump { get { return LCMSWarpOptions.MaxTimeDistortion; } set { LCMSWarpOptions.MaxTimeDistortion = value; } }

        [ParameterFile("NETTolerance", "Alignment")]
        [Category("Tolerances")]
        [Description("NET tolerance for allowable feature matches.")]
        public double NETTolerance { get { return LCMSWarpOptions.NetTolerance; } set { LCMSWarpOptions.NetTolerance = value; } }

        [ParameterFile("NumTimeSections", "Alignment")]
        [Category("Tolerances")]
        [Description("Number of divisions to make in NET.")]
        public int NumTimeSections { get { return LCMSWarpOptions.NumTimeSections; } set { LCMSWarpOptions.NumTimeSections = value; } }

        [ParameterFile("ContractionFactor", "Alignment")]
        [Category("Weights")]
        [Description("Determines how far away a baseline scan can be compared to alignee scans.")]
        public int ContractionFactor { get { return LCMSWarpOptions.ContractionFactor; } set { LCMSWarpOptions.ContractionFactor = value; } }

        /// <summary>
        /// Gets or sets a value that indicates whether the alignment should be
        /// to an AMT database.
        /// </summary>
        public bool AlignToAMT { get; set; }

        /// <summary>
        /// Gets or sets the mass tag database to align to.
        /// </summary>
        public InputDatabase InputDatabase { get; set; }

        #endregion

        #region Methods

        private void OnNotify(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}