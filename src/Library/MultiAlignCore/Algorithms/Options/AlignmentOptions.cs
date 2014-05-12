#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.IO.Parameters;
using MultiAlignEngine.Alignment;

#endregion

namespace MultiAlignCore.Algorithms.Options
{
    public class AlignmentOptions
    {
        public AlignmentOptions()
        {
            NumTimeSections = 100;
            TopFeatureAbundancePercent = 0;
            ContractionFactor = 3;
            MaxTimeJump = 10;
            MaxPromiscuity = 3;
            UsePromiscuousPoints = false;
            MassCalibrationUseLSQ = false;
            MassCalibrationWindow = 6.0;
            MassCalibrationNumXSlices = 12;
            MassCalibrationNumMassDeltaBins = 50;
            MassCalibrationMaxJump = 20;
            MassCalibrationMaxZScore = 3;
            MassCalibrationLSQZScore = 2.5;
            MassCalibrationLSQNumKnots = 12;
            MassTolerance = 6.0;
            NETTolerance = 0.03;
            AlignmentType = enmAlignmentType.NET_MASS_WARP;
            RecalibrationType = enmCalibrationType.HYBRID_CALIB;
            IsAlignmentBaselineAMasstagDB = true;
            AlignmentBaselineName = "";
            MassBinSize = .2;
            NETBinSize = .001;
            DriftTimeBinSize = .03;
            ShouldStoreAlignmentFunction = false;
            SplitAlignmentInMZ = false;
            MZBoundaries = new List<classAlignmentMZBoundary>();

            AlignmentAlgorithm = FeatureAlignmentType.LcmsWarp;

            /// Construct the m/z boundary object.			
            MZBoundaries.Add(new classAlignmentMZBoundary(0.0, 505.7));
            MZBoundaries.Add(new classAlignmentMZBoundary(505.7, 999999999.0));
        }

        [ParameterFile("AlignmentAlgorithmType", "Alignment")]
        [Category("Algorithm")]
        [Description("Type of algorithm to use for alignment.")]
        public FeatureAlignmentType AlignmentAlgorithm { get; set; }

        /// <summary>
        ///     Gets or sets whether to store alignment data.
        /// </summary>
        [ParameterFile("ShouldStoreAlignmentFunction", "Alignment")]
        [Description(
            "Stores the alignment function per dataset.  Optional only.  Set to false if processing a large amount of data."
            )]
        public bool ShouldStoreAlignmentFunction { get; set; }

        [ParameterFile("TopFeatureAbundancePercent", "Alignment")]
        [Description("Percentage of top Abundance features to use for alignment. ")]
        public double TopFeatureAbundancePercent { get; set; }

        private void OnNotify(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool m_isAlignmentBaselineAMasstagDB;

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
        [Description("Says how many LC-MS sample runs the mass tag must have been seen in.")]
        public int MassTagObservationCount { get; set; }

        [Browsable(false)]
        public string AlignmentBaselineName { get; set; }

        [ParameterFile("MassCalibrationLSQNumKnots", "Alignment")]
        [Category("Alignment Function")]
        [Description("Determines how many least square knots should be used.")]
        public int MassCalibrationLSQNumKnots { get; set; }

        [ParameterFile("HistogramDriftTimeBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Histogram size of alignment (ms)")]
        public double DriftTimeBinSize { get; set; }

        [ParameterFile("HistogramNETBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Bin size for NET error histograms.")]
        public double NETBinSize { get; set; }

        [ParameterFile("HistogramMassBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Histogram size of alignment in parts per million (PPM)")]
        public double MassBinSize { get; set; }

        [ParameterFile("AlignmentType", "Alignment")]
        [Category("General Calibration")]
        [Description("Determines if NET only, or Mass and NET alignment should be performed.")]
        public enmAlignmentType AlignmentType { get; set; }

        [ParameterFile("RecalibrationType", "Alignment")]
        [Category("General Calibration")]
        [Description("Type of recalibration to perform, NET only, or NET and Mass")]
        public enmCalibrationType RecalibrationType { get; set; }

        [ParameterFile("SplitAlignmentMZ", "Alignment")]
        [Category("General Calibration")]
        [Description("Determines whether the m/z boundaries should be used for analysis.  False if recommended.")]
        public bool SplitAlignmentInMZ { get; set; }

        [Category("General Calibration")]
        [Description("m/z calibration ranges if the detector is not calibrated.  Not normal mode of operation")]
        public List<classAlignmentMZBoundary> MZBoundaries { get; set; }

        [ParameterFile("MassCalibrationLSQZScore", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Determines the z-score cutoff for mass calibration")]
        public double MassCalibrationLSQZScore { get; set; }

        [ParameterFile("MassCalibrationMaxJump", "Alignment")]
        [Category("Mass Calibration")]
        [Description("")]
        public int MassCalibrationMaxJump { get; set; }

        [ParameterFile("MassCalibrationMaxZScore", "Alignment")]
        [Category("Mass Calibration")]
        [Description("")]
        public double MassCalibrationMaxZScore { get; set; }

        [ParameterFile("MassCalibrationNumMassDeltaBins", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Number of divisions to make in the mass dimension.")]
        public int MassCalibrationNumMassDeltaBins { get; set; }

        [ParameterFile("MassCalibrationNumXSlices", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Number of divisions to make in NET.")]
        public int MassCalibrationNumXSlices { get; set; }

        [ParameterFile("MassCalibrationUseLSQ", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Determines whether least squares is used for the fit.")]
        public bool MassCalibrationUseLSQ { get; set; }

        [ParameterFile("MassCalibrationWindow", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Mass Calibration window to use in parts per million (PPM)")]
        public double MassCalibrationWindow { get; set; }

        [ParameterFile("MaxPromiscuity", "Alignment")]
        [Category("Matching")]
        [Description(
            "Total number of candidate matches that can be allowed for a single LC-MS Features before being discarded.")
        ]
        public int MaxPromiscuity { get; set; }

        [ParameterFile("UsePromiscuousPoints", "Alignment")]
        [Category("Matching")]
        [Description(
            "Determines whether features can match to multiple features or whether they are considered ambiguous.")]
        public bool UsePromiscuousPoints { get; set; }

        [ParameterFile("MassTolerance", "Alignment")]
        [Category("Tolerances")]
        [Description("Mass tolerance in parts per million (PPM)")]
        public double MassTolerance { get; set; }

        [ParameterFile("MaxTimeJump", "Alignment")]
        [Category("Tolerances")]
        [Description("Largest scan range allowed for a feature match.")]
        public int MaxTimeJump { get; set; }

        [ParameterFile("NETTolerance", "Alignment")]
        [Category("Tolerances")]
        [Description("NET tolerance for allowable feature matches.")]
        public double NETTolerance { get; set; }

        [ParameterFile("NumTimeSections", "Alignment")]
        [Category("Tolerances")]
        [Description("Number of divisions to make in NET.")]
        public int NumTimeSections { get; set; }

        [ParameterFile("ContractionFactor", "Alignment")]
        [Category("Weights")]
        [Description("Determines how far away a baseline scan can be compared to alignee scans.")]
        public int ContractionFactor { get; set; }

        /// <summary>
        ///     Converts the current options into the older engine based ones.
        /// </summary>
        /// <param name="newOptions"></param>
        /// <returns></returns>
        public static clsAlignmentOptions ConvertToEngine(AlignmentOptions newOptions)
        {
            var options = new clsAlignmentOptions
            {
                AlignmentBaselineName = newOptions.AlignmentBaselineName,
                AlignmentType = newOptions.AlignmentType,
                ContractionFactor = Convert.ToInt16(newOptions.ContractionFactor),
                DriftTimeBinSize = newOptions.DriftTimeBinSize,
                IsAlignmentBaselineAMasstagDB = newOptions.IsAlignmentBaselineAMasstagDB,
                MassBinSize = newOptions.MassBinSize,
                MassCalibrationLSQNumKnots = Convert.ToInt16(newOptions.MassCalibrationLSQNumKnots),
                MassCalibrationLSQZScore = newOptions.MassCalibrationLSQZScore,
                MassCalibrationMaxJump = Convert.ToInt16(newOptions.MassCalibrationMaxJump),
                MassCalibrationMaxZScore = newOptions.MassCalibrationMaxZScore,
                MassCalibrationNumMassDeltaBins = Convert.ToInt16(newOptions.MassCalibrationNumMassDeltaBins),
                MassCalibrationNumXSlices = Convert.ToInt16(newOptions.MassCalibrationNumXSlices),
                MassCalibrationUseLSQ = newOptions.MassCalibrationUseLSQ,
                MassCalibrationWindow = newOptions.MassCalibrationWindow,
                MassTolerance = newOptions.MassTolerance,
                MaxPromiscuity = Convert.ToInt16(newOptions.MaxPromiscuity),
                MaxTimeJump = Convert.ToInt16(newOptions.MaxTimeJump),
                MZBoundaries = newOptions.MZBoundaries,
                NETBinSize = newOptions.NETBinSize,
                NETTolerance = newOptions.NETTolerance,
                NumTimeSections = newOptions.NumTimeSections,
                RecalibrationType = newOptions.RecalibrationType,
                SplitAlignmentInMZ = newOptions.SplitAlignmentInMZ,
                UsePromiscuousPoints = newOptions.UsePromiscuousPoints
            };

            return options;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}