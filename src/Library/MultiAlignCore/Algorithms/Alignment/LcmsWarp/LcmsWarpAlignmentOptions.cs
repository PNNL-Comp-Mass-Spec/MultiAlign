using System.ComponentModel;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Object to hold the options for LcmsWarp Alignment. 
    /// </summary>
    public class LcmsWarpAlignmentOptions
    {
        #region Auto Properties
        /// <summary>
        /// Number of Time Sections
        /// </summary>
        [Description("Percentage of top Abundance features to use for alignment. ")]        
        public int NumTimeSections { get; set; }

        /// <summary>
        /// Contraction factor for the alignment
        /// </summary>
        public int ContractionFactor { get; set; }

        /// <summary>
        /// Max time distortion at which to filter afterwards
        /// </summary>
        public int MaxTimeDistortion { get; set; }

        /// <summary>
        /// Max number of promiscuous points to use for alignment
        /// </summary>
        public int MaxPromiscuity { get; set; }

        /// <summary>
        /// Flag for whether to even use promiscuous points or not
        /// </summary>
        /// <remarks>This should be true when aligning to AMT tag databases
        /// It should be false when aligning MS data to MS data</remarks>
        public bool UsePromiscuousPoints { get; set; }

        /// <summary>
        /// Flag for whether to use LSQ during mass calibration
        /// </summary>
        public bool MassCalibUseLsq { get; set; }

        /// <summary>
        /// Window for the Mass calibration alignment (in ppm)
        /// </summary>
        public double MassCalibrationWindow { get; set; }

        /// <summary>
        /// Number of Mass slices for the mass calibration
        /// </summary>
        public int MassCalibNumXSlices { get; set; }

        /// <summary>
        /// Number of NET slices for the mass calibration
        /// </summary>
        public int MassCalibNumYSlices { get; set; }

        /// <summary>
        /// Number of jumps for the alignment function
        /// </summary>
        public int MassCalibMaxJump { get; set; }

        /// <summary>
        /// Z Tolerance for the calibration that Central Regression would use
        /// </summary>
        public double MassCalibMaxZScore { get; set; }

        /// <summary>
        /// Z Tolerance for the calibration that LSQ Regression would use
        /// </summary>
        public double MassCalibLsqMaxZScore { get; set; }

        /// <summary>
        /// Number of Knots that LSQ Regression would use
        /// </summary>
        public int MassCalibLsqNumKnots { get; set; }

        /// <summary>
        /// Mass tolerance (in ppm) for the Alignment and warping
        /// </summary>
        public double MassTolerance { get; set; }

        /// <summary>
        /// NET tolerance for the Alignment and warping
        /// </summary>
        public double NetTolerance { get; set; }

        /// <summary>
        /// The type of alignment which will be performed; either Net warping or Net-Mass warping
        /// </summary>
        public AlignmentType AlignType { get; set; }

        /// <summary>
        /// The type of calibration which will be performed; Either MZ, Scan or hybrid 
        /// </summary>
        public LcmsWarpCalibrationType CalibrationType { get; set; }

        /// <summary>
        /// Flag for if the warper is aligning to a database of mass tags
        /// </summary>
        public bool AlignToMassTagDatabase { get; set; }

        /// <summary>
        /// When using AMT tags from a database, minimum NET filter
        /// </summary>
        /// <remarks>AMTTagFilterNETMin is ignored if AMTTagFilterNETMax is less than or equal to AMTTagFilterNETMin</remarks>
        public double AMTTagFilterNETMin { get; set; }

        /// <summary>
        /// When using AMT tags from a database, maximum NET filter
        /// </summary>
        /// <remarks>AMTTagFilterNETMin is ignored if AMTTagFilterNETMax is less than or equal to AMTTagFilterNETMin</remarks>
        public double AMTTagFilterNETMax { get; set; }

        /// <summary>
        /// When using AMT tags from a database, minimum monoisotopic mass filter
        /// </summary>
        /// <remarks>AMTTagFilterMassMin is ignored if AMTTagFilterMassMax is less than or equal to AMTTagFilterMassMin</remarks>
        public double AMTTagFilterMassMin { get; set; }

        /// <summary>
        /// When using AMT tags from a database, maximum monoisotopic mass filter
        /// </summary>
        /// <remarks>AMTTagFilterMassMin is ignored if AMTTagFilterMassMax is less than or equal to AMTTagFilterMassMin</remarks>
        public double AMTTagFilterMassMax { get; set; }

        /// <summary>
        /// When using AMT tags from a database, only use those AMT tags with an observation count of this value or larger
        /// </summary>
        /// <remarks>
        /// If all of the AMT tags have an observation count of 0, then they will all be used 
        /// (i.e. this filter will be effectively ignored)</remarks>
        public int MinimumAMTTagObsCount { get; set; }

        /// <summary>
        /// How wide the Mass histogram bins are (in ppm)
        /// </summary>
        public double MassBinSize { get; set; }

        /// <summary>
        /// How wide the NET histogram bins are
        /// </summary>
        public double NetBinSize { get; set; }

        /// <summary>
        /// How wide the Drift time histogram bins are
        /// </summary>
        public double DriftTimeBinSize { get; set; }

        /// <summary>
        /// Abundance percentage under which to filter alignment.
        /// Set to 0 means all features are matched, set to 100 means no features are matched,
        /// set to 33 the top 67% of features sorted by abundance are matched
        /// </summary>
        public int TopFeatureAbundancePercent { get; set; }

        /// <summary>
        /// Flag for whether to store the alignment function from one alignment to another
        /// </summary>
        public bool StoreAlignmentFunction { get; set; }

        /// <summary>
        /// The type of aligner the processor uses.
        /// </summary>
        public FeatureAlignmentType AlignmentAlgorithmType { get; set; }
        
        #endregion

        /// <summary>
        /// Default constructor, initializes every value to commonly used values and flags
        /// </summary>
        public LcmsWarpAlignmentOptions()
        {
            NumTimeSections = 100;              // 100 in VIPER
            TopFeatureAbundancePercent = 0;
            ContractionFactor = 3;              //   3 in VIPER
            MaxTimeDistortion = 10;             //  10 in VIPER
            MaxPromiscuity = 3;                 //   2 in VIPER
            UsePromiscuousPoints = false;       // false for Dataset to Dataset alignment; true for Dataset to AMT tag alignment
            MassCalibUseLsq = false;
            MassCalibrationWindow = 40.0;       //  50 ppm in VIPER
            MassCalibNumXSlices = 12;           //  20 in VIPER
            MassCalibNumYSlices = 50;           // 100 in VIPER
            MassCalibMaxJump = 20;              //  50 in VIPER
            MassCalibMaxZScore = 3;             //   3 in VIPER
            MassCalibLsqMaxZScore = 2.5;        //   3 in VIPER
            MassCalibLsqNumKnots = 12;          //  12 in VIPER
            MassTolerance = 10;                 //  10 in VIPER
            NetTolerance = 0.02;                //   0.02 in VIPER

            AlignType = AlignmentType.NET_MASS_WARP;
            CalibrationType = LcmsWarpCalibrationType.Both;

            AlignToMassTagDatabase = false;
            
            AMTTagFilterNETMin = 0;
            AMTTagFilterNETMax = 0;

            AMTTagFilterMassMin = 0;
            AMTTagFilterMassMax = 0;

            MinimumAMTTagObsCount = 5;          // 5 in VIPER

            MassBinSize = 0.2;                  // 0.2 in VIPER
            NetBinSize = 0.001;                 // 0.001 in VIPER
            DriftTimeBinSize = 0.03;
            StoreAlignmentFunction = false;
            AlignmentAlgorithmType = FeatureAlignmentType.LCMS_WARP;
        }


    }

    /// <summary>
    /// Enumerations of possible Feature aligner types
    /// </summary>
    public enum FeatureAlignmentType
    {
        /// <summary>
        /// Uses LCMSWarp
        /// </summary>
        LCMS_WARP,

        /// <summary>
        /// Not Implemented.
        /// </summary>
        DIRECT_IMS_INFUSION,

        SPECTRAL_ALIGNMENT
    }
    /// <summary>
    /// Enumerations of possible Alignment Types
    /// </summary>
    public enum AlignmentType
    {
        /// <summary>
        /// Alignment type that uses a single NET warp
        /// </summary>
        NET_WARP = 0,
        /// <summary>
        /// Alignment type that performs a NET warp, recalibrates with regards to Mass
        /// and then performs warping again
        /// </summary>
        NET_MASS_WARP
    }
}
