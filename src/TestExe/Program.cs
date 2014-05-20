using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Drawing;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using PNNLOmicsViz.Drawing;

namespace TestWrapper
{
    class Program
    {
        private const string mBasePath = @"E:\MultiAlignTest";
        private const string mResultsPath = @"E:\MultiAlignTest\Results";
        private const string TextDelimiter = ",";

        private static void Main(string[] args)
        {
            try
            {
                TestNow(@"Lamarche-Data\QC_Shew_13_04_1b_6Oct13_Cougar_13-06-14.features",
                        @"Lamarche-Data\QC_Shew_13_04_1b_18Sep13_Cougar_13-06-14.features",
                        @"Alignment\QC-Shew-Annotated3\",
                        @"qc_shew_13_04_1b");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: " + ex.Message);
                System.Threading.Thread.Sleep(4000);
            }
        }

        private static string GetPath(string path1)
        {
            return Path.Combine(mBasePath, path1);

        }

        private static string GetOutputPath(string path1)
        {
            return Path.Combine(mResultsPath, path1);

        }

        private static void TestNow(string relativeBaselinePath, string relativeAligneePath, string relativeOutput, string name)
        {

            string baselinePath = GetPath(relativeBaselinePath);

            var aligneePath = GetPath(relativeAligneePath);
            var aligner = new LcmsWarpFeatureAligner();
            var rawBaselineData = File.ReadAllLines(baselinePath);
            var rawFeaturesData = File.ReadAllLines(aligneePath);
            var outputPath = GetOutputPath(relativeOutput);
            var delimiter = new[] { TextDelimiter };

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var baseline = (from line in rawBaselineData
                            where line != ""
                            select line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                                into parsed
                                select new UMCLight
                                {
                                    Net = Convert.ToDouble(parsed[0]),
                                    ChargeState = Convert.ToInt32(parsed[1]),
                                    Mz = Convert.ToDouble(parsed[2]),
                                    Scan = Convert.ToInt32(parsed[3]),
                                    MassMonoisotopic = Convert.ToDouble(parsed[4]),
                                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                                    Id = Convert.ToInt32(parsed[6]),
                                    ScanStart = Convert.ToInt32(parsed[7]),
                                    ScanEnd = Convert.ToInt32(parsed[8]),
                                    ScanAligned = Convert.ToInt32(parsed[9])
                                }).ToList();

            var features = (from line in rawFeaturesData
                            where line != ""
                            select line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                                into parsed
                                select new UMCLight
                                {
                                    Net = Convert.ToDouble(parsed[0]),
                                    ChargeState = Convert.ToInt32(parsed[1]),
                                    Mz = Convert.ToDouble(parsed[2]),
                                    Scan = Convert.ToInt32(parsed[3]),
                                    MassMonoisotopic = Convert.ToDouble(parsed[4]),
                                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                                    Id = Convert.ToInt32(parsed[6]),
                                    ScanStart = Convert.ToInt32(parsed[7]),
                                    ScanEnd = Convert.ToInt32(parsed[8]),
                                    ScanAligned = Convert.ToInt32(parsed[9])
                                }).ToList();

            var maxd = features.Max(x => x.Net);
            var mind = features.Min(x => x.Net);
            if (maxd - mind < double.Epsilon)
                throw new Exception("There is something wrong with the features NET values");
            aligner.Options.AlignmentType = enmAlignmentType.NET_MASS_WARP;


            var outputData = aligner.Align(baseline, features);
            var residuals = outputData.ResidualData;

            using (var writer = new StreamWriter(@"E:\MultiAlignTest\Results\heat-scores-old.txt"))
            {
                foreach (var oovarscore in outputData.heatScores)
                {
                    writer.WriteLine(oovarscore);
                }
            }

            var heatmap = HeatmapFactory.CreateAlignedHeatmap(outputData.heatScores);
            var netHistogram = HistogramFactory.CreateHistogram(outputData.netErrorHistogram, "NET Error Histogram", "NET Error");
            var massHistogram = HistogramFactory.CreateHistogram(outputData.massErrorHistogram, "Mass Error Histogram", "Mass Error (ppm)");

            var netResidual = ScatterPlotFactory.CreateResidualPlot(residuals.scans, residuals.linearCustomNet,
               residuals.linearNet, "NET Residuals", "Scans", "NET");
            var massMzResidual = ScatterPlotFactory.CreateResidualPlot(residuals.mz, residuals.mzMassError,
                residuals.mzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");
            var massScanResidual = ScatterPlotFactory.CreateResidualPlot(residuals.scans, residuals.mzMassError,
                residuals.mzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");

            var directory = Path.Combine(outputPath, name);

            var encoder = new SvgEncoder();
            PlotImageUtility.SaveImage(heatmap, directory + "_heatmap.svg", encoder);
            PlotImageUtility.SaveImage(netResidual, directory + "_netResidual.svg", encoder);
            PlotImageUtility.SaveImage(massMzResidual, directory + "_massMzResidual.svg", encoder);
            PlotImageUtility.SaveImage(massScanResidual, directory + "_massScanResidual.svg", encoder);
            //PlotImageUtility.SaveImage(netHistogram,       directory + "_netHistogram.svg",       encoder);
            PlotImageUtility.SaveImage(massHistogram, directory + "_massHistogram.svg", encoder);
        }
       
    }
}
