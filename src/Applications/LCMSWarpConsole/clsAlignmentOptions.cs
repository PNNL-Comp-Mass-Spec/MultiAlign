using FeatureAlignment.Algorithms.Alignment.LcmsWarp;
using FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration;

namespace LCMSWarpConsole
{
    class clsAlignmentOptions
    {

        public int NumberOfSections { get; set; }
        public short ContractionFactor { get; set; }
        public short MaxDistortion { get; set; }
        public double NETTol { get; set; }
        public double NET_MIN { get; set; }
        public double NET_MAX { get; set; }
        public int MatchPromiscuity { get; set; }
        public double MWTol { get; set; }
        public int MassNumMassDeltaBins { get; set; }
        public int MassWindowPPM { get; set; }
        public int MassMaxJump { get; set; }
        public int MassNumXSlices { get; set; }
        public double MassZScoreTolerance { get; set; }
        public double OutlierZScore { get; set; }
        public bool MassUseLSQ { get; set; }
        public short MassLSQNumKnots { get; set; }

        public int MinimumAMTTagObsCount { get; set; }
        public double MassHistogramMassBinSizePPM { get; set; }
        public double MassHistogramGANETBinSize { get; set; }
        public double MassHistogramDriftTimeBinSize { get; set; }

        /// <summary>
        /// Use with .SetRecalibrationType
        /// </summary>
        public LcmsWarpCalibrationType MassCalibrationType { get; set; }

        /// <summary>
        /// Use with .SetRegressionOrder
        /// </summary>
        public short MassSplineOrder { get; set; }

        public LcmsWarpAlignmentType AlignmentType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public clsAlignmentOptions()
        {
            NumberOfSections  = 100;
            ContractionFactor  = 3;
            MaxDistortion  = 10;
            NETTol  = 0.01f;
            NET_MIN  = 0;
            NET_MAX  = 1;
            MatchPromiscuity  = 2;
            MWTol  = 10;
            MassNumMassDeltaBins  = 100;
            MassWindowPPM  = 50;
            MassMaxJump  = 50;
            MassNumXSlices  = 20;
            MassZScoreTolerance  = 3;
            OutlierZScore  = 3;
            MassUseLSQ  = true;
            MassLSQNumKnots  = 12;
            MinimumAMTTagObsCount  = 5;
            MassHistogramMassBinSizePPM  = 0.2;
            MassHistogramGANETBinSize  = 0.001;
            MassHistogramDriftTimeBinSize  = 0.05;

            MassCalibrationType = LcmsWarpCalibrationType.Both;
            MassSplineOrder = 2;
            AlignmentType = LcmsWarpAlignmentType.NET_WARP;
        }

    }
}
