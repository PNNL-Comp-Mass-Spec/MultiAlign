using System;
using System.Collections.Generic;
using System.Text;

using MultiAlignEngine.PeakMatching;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.Features;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.Alignment;
using PNNLProteomics.SMART;
using MultiAlignCore.Algorithms.FeatureFinding;

namespace MultiAlignWin.Data
{
    /// <summary>
    /// Loads the options classes from their defaults stored in the application config file.
    /// </summary>
    public static class AnalysisDefaultOptionsFactory
    {
        /// <summary>
        /// Loads the default feature finding options from the settings file.
        /// </summary>
        public static UMCFeatureFinderOptions LoadFeatureFindingOptions()
        {
            UMCFeatureFinderOptions options = new UMCFeatureFinderOptions();
            options.MonoMassWeight       = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingMonoMass);
            options.AveMassWeight        = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingAverageMass);
            options.LogAbundanceWeight   = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingLogAbundance);
            options.ScanWeight           = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingScan);
            options.NETWeight            = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingNet);
            options.UseNET               = Properties.Settings.Default.UserPeakPickingUseNet;
            options.FitWeight            = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingFit);
            options.MaxDistance          = Properties.Settings.Default.UserPeakPickingMaxDist;
            options.MinUMCLength         = Convert.ToInt32(Properties.Settings.Default.UserPeakPickingMinDist);
            options.ConstraintMonoMass          = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingMonoMassConstraintPPM);
            options.ConstraintAveMass           = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingAverageMassConstraintPPM);
            options.UMCAbundanceReportingType   = enmAbundanceReportingType.PeakMax;
            options.IsIsotopicFitFilterInverted = Properties.Settings.Default.UserPeakPickingInvertFitFilter;
            options.IsotopicFitFilter           = Properties.Settings.Default.UserPeakPickingFitFilter;
            options.IsotopicIntensityFilter     = Properties.Settings.Default.UserPeakPickingIntensityFilter;
            options.UseIsotopicFitFilter        = Properties.Settings.Default.UserPeakPickingUseFitFilter;
            options.UseIsotopicIntensityFilter  = Properties.Settings.Default.UserPeakPickingUseIntensity;

            if (Properties.Settings.Default.UserPeakPickingUMCReportingTypePeakArea == true)
            {
                options.UMCAbundanceReportingType = MultiAlignEngine.Features.enmAbundanceReportingType.PeakArea;
            }            
            return options;
        }
        /// <summary>
        /// Loads the default cluster options from the settings file.
        /// </summary>
        public static clsClusterOptions LoadClusterOptions()
        {
            clsClusterOptions options = new MultiAlignEngine.Clustering.clsClusterOptions();
            options.MassTolerance = Properties.Settings.Default.UserClusterOptionsMassTolerance;
            options.NETTolerance = Properties.Settings.Default.UserClusterOptionsNETTolerance;
            options.DriftTimeTolerance = Properties.Settings.Default.UserClusterOptionsDriftTimeTolerance;
            options.ClusterIntensityType = MultiAlignEngine.Clustering.enmClusterIntensityType.MAX_PER_DATASET;
            options.IgnoreCharge = Properties.Settings.Default.UserClusterOptionsIgnoreChargeStates;
            options.AlignClusters = Properties.Settings.Default.UserClusterOptionsAlignToDatabase;

            if (Properties.Settings.Default.UserClusterOptionsUseMaxInDataset == false)
                options.ClusterIntensityType = MultiAlignEngine.Clustering.enmClusterIntensityType.SUM_PER_DATASET;

            options.ClusterRepresentativeType = MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEAN;
            if (Properties.Settings.Default.UserClusterOptionsUseMeanRepresentation == false)
                options.ClusterRepresentativeType = MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEDIAN;
            return options;
        }
        /// <summary>
        /// Loads the default cluster options from the settings file.
        /// </summary>
        public static clsPeakMatchingOptions LoadPeakMatchingOptions()
        {
            clsPeakMatchingOptions options  = new clsPeakMatchingOptions();
            options.MassTolerance           = Properties.Settings.Default.UserPeakMatchingMassTolerance;
            options.NETTolerance            = Properties.Settings.Default.UserPeakMatchingNETTolerance;
            options.DriftTimeTolerance      = Properties.Settings.Default.UserPeakMatchingDriftTimeTolerance;            
            return options;
        }
        /// <summary>
        /// Loads the saved alignment options from the settings file.
        /// </summary>
        public static clsAlignmentOptions LoadAlignmentOptions()
        {
            clsAlignmentOptions options = new clsAlignmentOptions();
            options.ApplyMassRecalibration = Properties.Settings.Default.UserAlignmentOptionsUseRecalibrateMasses;
            options.ContractionFactor = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsContractionFactor);
            options.MassCalibrationLSQNumKnots = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsLSQNumOptions);
            options.MassCalibrationLSQZScore = Properties.Settings.Default.UserAlignmentOptionsLSQOutlierZScore;
            options.MassCalibrationMaxJump = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxJump);
            options.MassCalibrationMaxZScore = Properties.Settings.Default.UserAlignmentOptionsOutlierZScore;
            options.MassCalibrationNumMassDeltaBins = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsNumMassDeltaBins);
            options.MassCalibrationNumXSlices = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsNumXSlices);
            options.MassCalibrationUseLSQ = Properties.Settings.Default.UserAlignmentOptionsUseLSQFit;
            options.MassCalibrationWindow = Properties.Settings.Default.UserAlignmentOptionsMassWindowPPM;
            options.MassTolerance = Properties.Settings.Default.UserAlignmentOptionsMassTolerance;
            options.MaxPromiscuity = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxPromiscuity);
            options.MaxTimeJump = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxDistortion);
            options.NETTolerance = Properties.Settings.Default.UserAlignmentOptionsNETTolerance;
            options.NumTimeSections = Properties.Settings.Default.UserAlignmentOptionsNumSections;
            options.NETBinSize = Properties.Settings.Default.UserAlignmentOptionsNETBinSize;
            options.MassBinSize = Properties.Settings.Default.UserAlignmentOptionsMassBinSize;

            if (Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeHybrid == true)
                options.RecalibrationType = MultiAlignEngine.Alignment.enmCalibrationType.HYBRID_CALIB;
            else if (Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeMzCoeff == true)
                options.RecalibrationType = MultiAlignEngine.Alignment.enmCalibrationType.MZ_CALIB;
            else
                options.RecalibrationType = MultiAlignEngine.Alignment.enmCalibrationType.SCAN_CALIB;
            options.UsePromiscuousPoints = Properties.Settings.Default.UserAlignmentOptionsIgnorePromiscuity;
            return options;
        }
        /// <summary>
        /// Loads the saved database options from the settinsg file.
        /// </summary>
        public static clsMassTagDatabaseOptions LoadDBOptions()
        {
            clsMassTagDatabaseOptions options = new clsMassTagDatabaseOptions();
            options.mdecimalMinPMTScore             = Convert.ToDecimal(Properties.Settings.Default.UserDBFormPMTQuality);
            options.mfltMinXCorr                    = Convert.ToSingle(Properties.Settings.Default.UserDBFormMinXCorr);
            options.mdblMinDiscriminant             = Properties.Settings.Default.UserDBFormMinDiscriminant;
            options.mdblPeptideProphetVal           = Properties.Settings.Default.UserDBFormPeptideProphetProbability;
            options.mstr_databaseFilePath           = Properties.Settings.Default.UserDBFormLocalDatabasePath;
            options.mintMinObservationCountFilter   = Properties.Settings.Default.UserDBFormMinObservationCountFilter;
            options.mstrExperimentFilter            = Properties.Settings.Default.UserDBFormExperimentFilter;
            options.mstrExperimentExclusionFilter   = Properties.Settings.Default.UserDBFormExperimentExclusionFilter;
            return options;
        }
        /// <summary>
        /// Loads the SMART Options from the settings file.
        /// </summary>
        public static classSMARTOptions LoadSMARTOptions()
        {
            classSMARTOptions options       = new classSMARTOptions();            
            options.IsDataPaired            = Properties.Settings.Default.STACIsDataPaired;
            options.MassTolerancePPM        = Properties.Settings.Default.STACMassTolerance;
            options.NETTolerance            = Properties.Settings.Default.STACNETTolerance;
            options.PairedMass              = Properties.Settings.Default.STACPairedMass;
            options.UsePriorProbabilities   = Properties.Settings.Default.STACUsePriorProbabilities;
            return options;
        }
    }
}
