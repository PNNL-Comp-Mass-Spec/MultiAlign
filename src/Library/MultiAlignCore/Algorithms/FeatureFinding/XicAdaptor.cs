using System;
using System.Collections.Generic;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Backend.ProcessingTasks.PeakDetectors;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using DeconTools.Backend.Utilities;
using MultiAlignCore.Data;
using MultiAlignCore.Extensions;
using System.Text;
using System.Linq;
using DeconTools.Utilities;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Finds an XIC based on DeconTools profile data.
    /// </summary>
    public class XicAdaptor
    {
        Run m_run = null;

        public XicAdaptor(string rawPath, string peaksPath)
        {
            Run run     = RunUtilities.CreateAndLoadPeaks(rawPath, peaksPath);
            m_run       = run;

            NetWindow   = .02;
            MzPpmWindow = 20;
        }

        /// <summary>
        /// Gets or sets the NET tolerance window for an XIC
        /// </summary>
        public double NetWindow
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the m/z PPM tolerance window.
        /// </summary>
        public double MzPpmWindow
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the number of points to smooth.
        /// </summary>
        public int PointsToSmooth
        {
            get;
            set;
        }
        /// <summary>
        /// Finds the XIC based on the m/z and scan parameters.
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="scan"></param>
        /// <returns></returns>
        public List<XYData> FindXic(double mz, int scan, bool shouldSmooth)
        {
            LcmsFeatureTarget target    = new LcmsFeatureTarget();
            target.ID                   = 0;
            target.MZ                   = mz;
            target.ScanLCTarget         = scan;
            target.ElutionTimeUnit      = Globals.ElutionTimeUnit.ScanNum;
            m_run.CurrentMassTag        = target;
            var result                  = m_run.ResultCollection.GetTargetedResult(m_run.CurrentMassTag);

            double chromPeakGeneratorTolInPPM = MzPpmWindow;
            Globals.ChromatogramGeneratorMode chromGeneratorMode = Globals.ChromatogramGeneratorMode.MZ_BASED;

            var chromGen = new PeakChromatogramGenerator(   chromPeakGeneratorTolInPPM,
                                                            chromGeneratorMode);
            chromGen.NETWindowWidthForNonAlignedData = Convert.ToSingle(NetWindow);

            int pointsToSmooth  = 5;
            var chromSmoother   = new SavitzkyGolaySmoother(pointsToSmooth, 2);

            double chromPeakDetectorPeakBR   = 1;
            double chromPeakDetectorSigNoise = 1;
            var chromPeakDetector            = new ChromPeakDetector(   chromPeakDetectorPeakBR,
                                                                        chromPeakDetectorSigNoise);

            ChromPeakSelectorParameters chromPeakSelectorParameters = new ChromPeakSelectorParameters();
            var chromPeakSelector = new BasicChromPeakSelector(chromPeakSelectorParameters);

            //this generates an extracted ion chromatogram
            // Since we are not using the built in generator,
            chromGen.Execute(m_run.ResultCollection);

            //this smooths the data - very important step!
            if (shouldSmooth)
            {
                chromSmoother.Execute(m_run.ResultCollection);
            }

            //this detects peaks within an extracted ion chromatogram
            chromPeakDetector.Execute(m_run.ResultCollection);

            //this selects the peak
            chromPeakSelector.Parameters.PeakSelectorMode = Globals.PeakSelectorMode.ClosestToTarget;
            chromPeakSelector.Execute(m_run.ResultCollection);

            //Here's the chromatogram data...
            List<XYData> data = new List<XYData>();

            for (int i = 0; i < m_run.XYData.Xvalues.Length; i++)
            {
                XYData datum = new XYData(m_run.XYData.Xvalues[i], m_run.XYData.Yvalues[i]);
                data.Add(datum);
            }

            return data;
        }

        /// <summary>
        /// Creates an Xic for a feature based on individual charge states.
        /// </summary>
        /// <param name="rawPath">Path to the raw file.</param>
        /// <param name="peaksPath">Path to the peaks file.</param>
        /// <param name="feature">Feature to target</param>
        public Dictionary<int, Chromatogram> CreateXicForChargeStates(UMCLight feature, bool shouldSmooth)
        {

            Dictionary<int, Chromatogram> chromatograms = new Dictionary<int, Chromatogram>();
            Dictionary<int, List<XYZData>> charges      =  feature.CreateChargeSIC();

            XicFinder finder   = new XicFinder();
            feature.ChargeStateChromatograms.Clear();

            //
            foreach (int charge in charges.Keys)
            {
                // This is the current XIC - but it's not that great
                List<XYZData> xicMz             = charges[charge];

                // Finds the maximum point in the XIC, we'll use this as our target, it most likely contains
                // the exact point that we'll want to use to help identify the feature.
                var maxPoint                    = xicMz.Where( u => u.Y == xicMz.Max(x=>x.Y)).Select(u=> new { u.X, u.Y, u.Z}).FirstOrDefault();

                int     targetScan  = Convert.ToInt32(maxPoint.X);
                double  targetMz    = maxPoint.Z;

                // Find the Xic based on the target point, this may include other peaks
                List<XYData> totalXic = null;

                try
                {
                    totalXic = FindXic(targetMz, targetScan, shouldSmooth);

                    if (totalXic.Count > 1)
                    {
                        // Then find a specific Xic based on the target scan.
                        List<XYData> specificXic = finder.FindTarget(totalXic, targetScan);

                        // Add the targeted Xic to the charge state map so that we can create Xic's for each charge state.
                        Chromatogram gram = new Chromatogram(specificXic, maxPoint.Z, charge);
                        chromatograms.Add(charge, gram);
                    }
                }
                catch (PreconditionException)
                {
                    // This would mean that the charge state didnt have enough features in it.
                }
            }
            return chromatograms;
        }

        /// <summary>
        /// Creates an isotopic profile based on the m/z and scan provided.
        /// </summary>
        /// <param name="mz"></param>
        /// <param name="scan"></param>
        /// <returns></returns>
        public List<XYData> CreateIsotopicProfile(double monoisotopicPeak, double scan, int charge)
        {
            return new List<XYData>();
        }

        /// <summary>
        /// Creates an Xic for a feature based on individual charge states.
        /// </summary>
        /// <param name="rawPath">Path to the raw file.</param>
        /// <param name="peaksPath">Path to the peaks file.</param>
        /// <param name="feature">Feature to target</param>
        public Dictionary<int, List<Chromatogram>> CreateXicForIsotopes(UMCLight feature, bool shouldSmooth)
        {

            Dictionary<int, List<Chromatogram>> chromatograms = new Dictionary<int, List<Chromatogram>>();
            Dictionary<int, List<XYZData>> charges      = feature.CreateChargeSIC();
            XicFinder finder                            = new XicFinder();
            feature.IsotopeChromatograms.Clear();

            foreach (int charge in charges.Keys)
            {
                /// Creates a list of Xic's for a given chromatogram.
                List<Chromatogram> grams    = new List<Chromatogram>();
                chromatograms.Add(charge, grams);

                // This is the current XIC - but it's not that great
                List<XYZData> xicMz         = charges[charge];

                // Finds the maximum point in the XIC, we'll use this as our target, it most likely contains
                // the exact point that we'll want to use to help identify the feature.
                var maxPoint    = xicMz.Where(u => u.Y == xicMz.Max(x => x.Y)).Select(u => new { u.X, u.Y, u.Z }).FirstOrDefault();
                int targetScan  = Convert.ToInt32(maxPoint.X);
                double targetMz = maxPoint.Z;

                List<XYData>  isotopicProfile = CreateIsotopicProfile(targetMz, targetScan, charge);

                foreach (XYData isotope in isotopicProfile)
                {
                    // Find the Xic based on the target point, this may include other peaks
                    List<XYData> totalXic = FindXic(targetMz, targetScan, shouldSmooth);

                    // Then find a specific Xic based on the target scan.
                    List<XYData> targetIsotopeXic = finder.FindTarget(totalXic, targetScan);

                    // Add the targeted Xic to the charge state map so that we can create Xic's for each charge state.
                    Chromatogram gram = new Chromatogram(targetIsotopeXic, maxPoint.Z, charge);
                    grams.Add(gram);
                }

                chromatograms[charge] = grams;
            }

            feature.IsotopeChromatograms = chromatograms;
            return chromatograms;
        }
    }
}
