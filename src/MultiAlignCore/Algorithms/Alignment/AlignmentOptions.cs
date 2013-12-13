using System.Collections.Generic;
using MultiAlignCore.IO.Parameters;
using MultiAlignEngine;
using MultiAlignEngine.Alignment;
using System.Windows;
using MultiAlignCore.Data;
using System.ComponentModel;

namespace MultiAlignCore.Algorithms.Alignment
{
    public class AlignmentOptions: INotifyPropertyChanged
    {
        public AlignmentOptions()
        {
            NumTimeSections				        = 100;
            TopFeatureAbundancePercent  = 0;
			ContractionFactor			= 3; 
			MaxTimeJump			        = 10; 
			MaxPromiscuity			    = 3; 
			UsePromiscuousPoints		= false; 
			MassCalibrationUseLSQ   	= false; 
			MassCalibrationWindow		= 6.0; 
			MassCalibrationNumXSlices   = 12; 
			MassCalibrationNumMassDeltaBins    = 50; 
			MassCalibrationMaxJump  	= 20; 
			MassCalibrationMaxZScore	= 3; 
			MassCalibrationLSQZScore	= 2.5; 
			MassCalibrationLSQNumKnots  = 12; 
			MassTolerance				= 6.0; 
			NETTolerance				= 0.03; 
			AlignmentType				= enmAlignmentType.NET_MASS_WARP; 
			RecalibrationType       	= enmCalibrationType.HYBRID_CALIB;
            IsAlignmentBaselineAMasstagDB = true; 
			AlignmentBaselineName		= ""; 
			MassBinSize				    = .2;
			NETBinSize				    = .001;
			DriftTimeBinSize		    = .03;
            ShouldStoreAlignmentFunction  = false;
            SplitAlignmentInMZ            = false;
			MZBoundaries				  = new List<classAlignmentMZBoundary>();

            AlignmentAlgorithm = FeatureAlignmentType.LCMSWarp;

			/// Construct the m/z boundary object.			
			MZBoundaries.Add(new classAlignmentMZBoundary(0.0, 505.7));
			MZBoundaries.Add(new classAlignmentMZBoundary(505.7, 999999999.0));
        }

        [ParameterFileAttribute("AlignmentAlgorithmType", "Alignment")]
        [Category("Algorithm")]
        [Description("Type of algorithm to use for alignment.")]
        public FeatureAlignmentType  AlignmentAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets whether to store alignment data.
        /// </summary>
        [ParameterFileAttribute("ShouldStoreAlignmentFunction", "Alignment")]
        [Description("Stores the alignment function per dataset.  Optional only.  Set to false if processing a large amount of data.")]
        public bool ShouldStoreAlignmentFunction
        {
            get;
            set;
        }

        [ParameterFileAttribute("TopFeatureAbundancePercent", "Alignment")]
        [Description("Percentage of top Abundance features to use for alignment. ")]
        public double  TopFeatureAbundancePercent
        {
            get;
            set;
        }
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
            set {

                if (value != m_isAlignmentBaselineAMasstagDB)
                {
                    m_isAlignmentBaselineAMasstagDB = value;
                    OnNotify("IsAlignmentBaselineAMasstagDB");
                }
                
            }
        }

        /// <summary>
        /// Minimum number of observations to use for alignment.
        /// </summary>
        [ParameterFileAttribute("MassTagObservationCount", "Alignment")]
        [Category("Filtering")]
        [Description("Says how many LC-MS sample runs the mass tag must have been seen in.")]
        public int MassTagObservationCount { get; set; }
        
        [Browsable(false)]
        public string AlignmentBaselineName { get; set; }

        [ParameterFileAttribute("MassCalibrationLSQNumKnots", "Alignment")]
        [Category("Alignment Function")]
        [Description("Determines how many least square knots should be used.")]
        public int MassCalibrationLSQNumKnots { get; set; }

        [ParameterFileAttribute("HistogramDriftTimeBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Histogram size of alignment (ms)")]
        public double DriftTimeBinSize { get; set; }
        [ParameterFileAttribute("HistogramNETBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Bin size for NET error histograms.")]
        public double NETBinSize { get; set; }
        [ParameterFileAttribute("HistogramMassBinSize", "Alignment")]
        [Category("Binning")]
        [Description("Histogram size of alignment in parts per million (PPM)")]
        public double MassBinSize { get; set; }

        [ParameterFileAttribute("AlignmentType", "Alignment")]
        [Category("General Calibration")]
        [Description("Determines if NET only, or Mass and NET alignment should be performed.")]
        public enmAlignmentType AlignmentType { get; set; }
        [ParameterFileAttribute("RecalibrationType", "Alignment")]
        [Category("General Calibration")]
        [Description("Type of recalibration to perform, NET only, or NET and Mass")]
        public enmCalibrationType RecalibrationType { get; set; }
        [ParameterFileAttribute("SplitAlignmentMZ", "Alignment")]
        [Category("General Calibration")]
        [Description("Determines whether the m/z boundaries should be used for analysis.  False if recommended.")]
        public bool SplitAlignmentInMZ { get; set; }
        [Category("General Calibration")]
        [Description("m/z calibration ranges if the detector is not calibrated.  Not normal mode of operation")]
        public List<classAlignmentMZBoundary> MZBoundaries { get; set; }

        [ParameterFileAttribute("MassCalibrationLSQZScore", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Determines the z-score cutoff for mass calibration")]
        public double MassCalibrationLSQZScore { get; set; }
        [ParameterFileAttribute("MassCalibrationMaxJump", "Alignment")]
        [Category("Mass Calibration")]
        [Description("")]
        public int MassCalibrationMaxJump { get; set; }
        [ParameterFileAttribute("MassCalibrationMaxZScore", "Alignment")]
        [Category("Mass Calibration")]
        [Description("")]
        public double MassCalibrationMaxZScore { get; set; }
        [ParameterFileAttribute("MassCalibrationNumMassDeltaBins", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Number of divisions to make in the mass dimension.")]
        public int MassCalibrationNumMassDeltaBins { get; set; }
        [ParameterFileAttribute("MassCalibrationNumXSlices", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Number of divisions to make in NET.")]
        public int MassCalibrationNumXSlices { get; set; }
        [ParameterFileAttribute("MassCalibrationUseLSQ", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Determines whether least squares is used for the fit.")]
        public bool MassCalibrationUseLSQ { get; set; }
        [ParameterFileAttribute("MassCalibrationWindow", "Alignment")]
        [Category("Mass Calibration")]
        [Description("Mass Calibration window to use in parts per million (PPM)")]
        public double MassCalibrationWindow { get; set; }

        [ParameterFileAttribute("MaxPromiscuity", "Alignment")]
        [Category("Matching")]
        [Description("Total number of candidate matches that can be allowed for a single LC-MS Features before being discarded.")]
        public int MaxPromiscuity { get; set; }
        [ParameterFileAttribute("UsePromiscuousPoints", "Alignment")]
        [Category("Matching")]
        [Description("Determines whether features can match to multiple features or whether they are considered ambiguous.")]
        public bool UsePromiscuousPoints { get; set; }       

        [ParameterFileAttribute("MassTolerance", "Alignment")]
        [Category("Tolerances")]
        [Description("Mass tolerance in parts per million (PPM)")]
        public double MassTolerance { get; set; }
        [ParameterFileAttribute("MaxTimeJump", "Alignment")]
        [Category("Tolerances")]
        [Description("Largest scan range allowed for a feature match.")]
        public int MaxTimeJump { get; set; }
        [ParameterFileAttribute("NETTolerance", "Alignment")]
        [Category("Tolerances")]
        [Description("NET tolerance for allowable feature matches.")]
        public double NETTolerance { get; set; }
        [ParameterFileAttribute("NumTimeSections", "Alignment")]
        [Category("Tolerances")]
        [Description("Number of divisions to make in NET.")]
        public int NumTimeSections { get; set; }


        [ParameterFileAttribute("ContractionFactor", "Alignment")]
        [Category("Weights")]
        [Description("Determines how far away a baseline scan can be compared to alignee scans.")]
        public int ContractionFactor { get; set; }

        /// <summary>
        /// Converts the current options into the older engine based ones.
        /// </summary>
        /// <param name="newOptions"></param>
        /// <returns></returns>
        public static clsAlignmentOptions ConvertToEngine(AlignmentOptions newOptions)
        {
            clsAlignmentOptions options             = new clsAlignmentOptions();
            options.AlignmentBaselineName           = newOptions.AlignmentBaselineName;
            options.AlignmentType                   = newOptions.AlignmentType;
           
            options.ContractionFactor               = System.Convert.ToInt16(newOptions.ContractionFactor);
            options.DriftTimeBinSize                = newOptions.DriftTimeBinSize;
            options.IsAlignmentBaselineAMasstagDB   = newOptions.IsAlignmentBaselineAMasstagDB;
            options.MassBinSize                     = newOptions.MassBinSize;
            options.MassCalibrationLSQNumKnots      = System.Convert.ToInt16(newOptions.MassCalibrationLSQNumKnots);
            options.MassCalibrationLSQZScore        = newOptions.MassCalibrationLSQZScore;
            options.MassCalibrationMaxJump          = System.Convert.ToInt16(newOptions.MassCalibrationMaxJump);
            options.MassCalibrationMaxZScore        = newOptions.MassCalibrationMaxZScore;
            options.MassCalibrationNumMassDeltaBins = System.Convert.ToInt16(newOptions.MassCalibrationNumMassDeltaBins);
            options.MassCalibrationNumXSlices       = System.Convert.ToInt16(newOptions.MassCalibrationNumXSlices);
            options.MassCalibrationUseLSQ           = newOptions.MassCalibrationUseLSQ;
            options.MassCalibrationWindow           = newOptions.MassCalibrationWindow;
            options.MassTolerance                   = newOptions.MassTolerance;
            options.MaxPromiscuity                  = System.Convert.ToInt16(newOptions.MaxPromiscuity); 
            options.MaxTimeJump                     = System.Convert.ToInt16(newOptions.MaxTimeJump);
            options.MZBoundaries                    = newOptions.MZBoundaries;
            options.NETBinSize                      = newOptions.NETBinSize;
            options.NETTolerance                    = newOptions.NETTolerance;
            options.NumTimeSections                 = newOptions.NumTimeSections; 
            options.RecalibrationType               = newOptions.RecalibrationType; 
            options.SplitAlignmentInMZ              = newOptions.SplitAlignmentInMZ; 
            options.UsePromiscuousPoints            = newOptions.UsePromiscuousPoints;

            return options;
        }
    
#region INotifyPropertyChanged Members

public event PropertyChangedEventHandler  PropertyChanged;

#endregion
}
}
