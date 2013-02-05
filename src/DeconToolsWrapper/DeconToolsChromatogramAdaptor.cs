using System.Collections.Generic;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Backend.ProcessingTasks.PeakDetectors;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using DeconTools.Backend.Utilities;
using PNNLOmics.Algorithms;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Data;
using MultiAlignCore.Extensions;
using System;

namespace DeconToolsTesting
{
    public class DeconToolsChromatogramAdaptor
    {
        public Dictionary<int, Chromatogram> CreateXic(DatasetInformation information,
                                             FeatureTolerances tolerances,
                                             int pointsToSmooth,
                                             UMCLight feature)
        {
            string rawFile = information.Raw.Path;
            string peaksFile = information.Peaks.Path;

            Dictionary<int, Chromatogram> chromatograms = new Dictionary<int, Chromatogram>();
            Dictionary<int, List<XYZData>> charges = feature.CreateChargeSIC();

            Run run = RunUtilities.CreateAndLoadPeaks(rawFile, peaksFile);

            foreach (int charge in charges.Keys)
            {
                // This is the current XIC - but it's not that great
                List<XYZData> sic = charges[charge];

                double targetMz = sic[0].Z;
                // This is the target feature setup
                LcmsFeatureTarget target = new LcmsFeatureTarget();
                target.ID = feature.ID;
                target.MZ = targetMz;
                target.ScanLCTarget = feature.Scan;
                run.CurrentMassTag = target;

                double chromPeakGeneratorTolInPPM = tolerances.Mass;
                Globals.ChromatogramGeneratorMode chromGeneratorMode = Globals.ChromatogramGeneratorMode.MZ_BASED;

                var chromGen = new PeakChromatogramGenerator(chromPeakGeneratorTolInPPM, chromGeneratorMode);
                chromGen.NETWindowWidthForNonAlignedData = Convert.ToSingle(tolerances.RetentionTime);

                var chromSmoother = new SavitzkyGolaySmoother(pointsToSmooth, 2);

                //BLL We also tried to set the BR and SIG NOISE to 0.  This did not work 
                double chromPeakDetectorPeakBR = 1;
                double chromPeakDetectorSigNoise = 1;
                var chromPeakDetector = new ChromPeakDetector(chromPeakDetectorPeakBR,
                                                                            chromPeakDetectorSigNoise);

                ChromPeakSelectorParameters chromPeakSelectorParameters = new ChromPeakSelectorParameters();
                var chromPeakSelector = new BasicChromPeakSelector(chromPeakSelectorParameters);

                //this generates an extracted ion chromatogram
                // Since we are not using the built in generator, 
                chromGen.Execute(run.ResultCollection);
                chromSmoother.Execute(run.ResultCollection);
                chromPeakDetector.Execute(run.ResultCollection);

                //this selects the peak
                chromPeakSelector.Parameters.PeakSelectorMode = Globals.PeakSelectorMode.ClosestToTarget;
                chromPeakSelector.Execute(run.ResultCollection);

                Chromatogram gram = new Chromatogram();
                gram.ChargeState = charge;

                DeconTools.Backend.XYData data = run.XYData;
                for (int i = 0; i < run.XYData.Xvalues.Length; i++)
                {
                    double x = data.Xvalues[i];
                    double y = data.Xvalues[i];

                    XYZData datum = new PNNLOmics.Data.XYZData(x, y, targetMz);
                    gram.Points.Add(datum);
                }
                chromatograms.Add(charge, gram);
            }

            return chromatograms;
        }
    }
}
