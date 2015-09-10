#region

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.Drawing;
using MultiAlignCore.Extensions;

#endregion

namespace MultiAlignCore.IO.Reports
{
    public sealed class AnalysisReportGenerator : IAnalysisReportGenerator
    {
        private const int HEIGHT    = 400;
        private const int WIDTH     = 400;

        /// <summary>
        ///     Default path for plots.
        /// </summary>
        private const string THUMBNAIL_PATH = "Plots";

        public AnalysisReportGenerator()
        {
            PlotPath = THUMBNAIL_PATH;
        }

        public AnalysisConfig Config { get; set; }
        public string PlotPath { get; set; }

        #region Plot Methods

        /// <summary>
        /// </summary>
        /// <param name="image"></param>
        /// <param name="name"></param>
        private void SaveImage(Image image, string name)
        {
            if (image != null)
            {
                var path = Path.Combine(Config.plotSavePath, name);
                image.Save(path, ImageFormat.Png);
            }
            else
            {
                Logger.PrintMessage(string.Format("Could not create {0} plot.", name));
            }
        }

        public void CreateClusterPlots(FeaturesClusteredEventArgs args)
        {
            // Plots: 
            //      Dataset member
            //      Total members
            //      Cluster scatter plot
            var clusters = args.Clusters;
            var report = Config.Report;

            Logger.PrintMessage("Creating Charge State Plots");

            report.PushStartTable();
            report.PushStartTableRow();
            report.PushTextHeader("Cluster Statistics");
            report.PushEndTableRow();
            report.PushStartTableRow();

            var path = Path.Combine(Config.AnalysisPath, PlotPath, "clusters_scatterplot.png");
            var clusterScatterPlot = ScatterPlotFactory.CreateClusterMassScatterPlot(clusters);
            PlotImageUtility.SaveImage(clusterScatterPlot, path);
            report.PushImageColumn(path, WIDTH, HEIGHT);

            report.PushEndTableRow();
            report.PushStartTableRow();
            path                        = Path.Combine(Config.AnalysisPath, PlotPath, "clusters_datasetMembersHistogram.png");
            var datasetMemberHistogram  =
                HistogramFactory.CreateHistogram(clusters.CreateClusterDatasetMemberSizeHistogram(), "Dataset Members", "Dataset Count");
            PlotImageUtility.SaveImage(datasetMemberHistogram, path);
            report.PushImageColumn(path, WIDTH, HEIGHT);

            path                        = Path.Combine(Config.AnalysisPath, PlotPath, "clusters_clusterSizeHistogram.png");
            var clusterSizeHistogram    = HistogramFactory.CreateHistogram(clusters.CreateClusterSizeHistogram(), "Cluster Members", "Cluster Size");
            PlotImageUtility.SaveImage(clusterSizeHistogram, path);
            report.PushImageColumn(path, WIDTH, HEIGHT);

            report.PushEndTableRow();
            report.PushEndTable();
        }

        /// <summary>
        ///     Creates the final analysis plots.
        /// </summary>
        public void CreateChargePlots(Dictionary<int, int> chargeMap)
        {
            Logger.PrintMessage("Creating Charge State Plots");

            var report = Config.Report;

            report.PushTextHeader("Cluster Charge States"); 
            report.PushStartTable();
            var path        = Path.Combine(Config.AnalysisPath, PlotPath, "clusters_chargestates.png");
            var histogram   = HistogramFactory.CreateHistogram(chargeMap, "Charge States", "Charge States");            
            PlotImageUtility.SaveImage(histogram, path);            
            report.PushImageColumn(path);
            report.PushEndTable();
        }

        /// <summary>
        ///     Creates the HTML output file.
        /// </summary>
        public void CreatePlotReport()
        {
            Logger.PrintMessage("Creating Report.");
            var htmlPath = Config.HtmlPathName;
            if (Config.AnalysisPath != null)
            {
                htmlPath = Path.Combine(Config.AnalysisPath, Config.HtmlPathName);
            }
            Config.Report.AnalysisName = Config.AnalysisName;
            Config.Report.CreateReport(htmlPath);
        }

        public void CreateMassTagPlot(MassTagsLoadedEventArgs e)
        {
            Logger.PrintMessage("Creating Mass Tag Plot.");

            if (e.Database == null)
                return;

            if (e.MassTags == null)
                return;

            var feature = ScatterPlotFactory.CreateFeatureMassScatterPlot(e.MassTags);
            var name = e.Database.Name;
            var directory = Path.Combine(Config.AnalysisPath, name);

            PlotImageUtility.SaveImage(feature, directory + "_mtdb.png");
        }

        /// <summary>
        ///     Creates images for the HTML output for baseline data
        /// </summary>
        /// <param name="e"></param>
        public void CreateBaselinePlots(BaselineFeaturesLoadedEventArgs e)
        {
            var baselineInfo = e.DatasetInformation;
            if (baselineInfo == null)
                return;

            var feature = ScatterPlotFactory.CreateFeatureMassScatterPlot(e.Features);
            var name = e.DatasetInformation.DatasetName;
            var directory = Path.Combine(Config.AnalysisPath, PlotPath, name);

            var report = Config.Report;
            report.PushTextHeader("Baseline Features");
            report.PushStartTableRow();
            Config.Report.PushImageColumn(directory + "_features.png", WIDTH, HEIGHT);
            PlotImageUtility.SaveImage(feature, directory + "_features.png");
            report.PushEndTableRow();
        }

        /// <summary>
        ///     Creates alignment plots for the HTML output.
        /// </summary>
        public void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            var name = e.AligneeDatasetInformation.DatasetName;
            var alignmentData = e.AlignmentData;
            if (alignmentData == null)
                return;

            var directory       = Path.Combine(Config.AnalysisPath, PlotPath, name);
            var heatmap         = HeatmapFactory.CreateAlignedHeatmap(alignmentData.heatScores, alignmentData.baselineIsAmtDB);
            var feature         = ScatterPlotFactory.CreateFeatureMassScatterPlot(e.AligneeFeatures);
            var netHistogram    = HistogramFactory.CreateHistogram(alignmentData.netErrorHistogram, "NET Error", "NET Error");
            var massHistogram   = HistogramFactory.CreateHistogram(alignmentData.massErrorHistogram, "Mass Error", "Mass Error (ppm)");
            var residuals       = alignmentData.ResidualData;

            var netResidual         = ScatterPlotFactory.CreateResidualPlot(residuals.Scan, residuals.LinearCustomNet,
                residuals.LinearNet, "NET Residuals", "Scans", "NET");
            var massMzResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Mz, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");

            var massScanResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Scan, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");
            
            var report = Config.Report;
            report.PushLargeText(string.Format("Alignee Statistics - {0}", name));            
            report.PushStartTable();

            // Features
            report.PushStartTableRow();
            report.PushTextHeader("Features");
            report.PushEndTableRow();
            report.PushStartTableRow();
            
            Config.Report.PushImageColumn(directory + "_features.png", WIDTH, HEIGHT);
            Config.Report.PushImageColumn(directory + "_heatmap.png", WIDTH, HEIGHT);
            PlotImageUtility.SaveImage(feature, directory + "_features.png");
            PlotImageUtility.SaveImage(heatmap, directory + "_heatmap.png");
            report.PushEndTableRow();

            // Histograms 
            report.PushStartTableRow();
            report.PushTextHeader("Histograms");
            report.PushEndTableRow();
            report.PushStartTableRow();

            Config.Report.PushImageColumn(directory + "_netHistogram.png");
            Config.Report.PushImageColumn(directory + "_massHistogram.png");
            PlotImageUtility.SaveImage(netHistogram, directory + "_netHistogram.png");
            PlotImageUtility.SaveImage(massHistogram, directory + "_massHistogram.png");
            report.PushEndTableRow();

            // Residuals
            report.PushStartTableRow();
            report.PushTextHeader("Histograms");
            report.PushEndTableRow();
            report.PushStartTableRow();

            Config.Report.PushImageColumn(directory + "_netResidual.png");
            Config.Report.PushImageColumn(directory + "_massMzResidual.png");
            Config.Report.PushImageColumn(directory + "_massScanResidual.png");

            PlotImageUtility.SaveImage(netResidual, directory + "_netResidual.png");
            PlotImageUtility.SaveImage(massMzResidual, directory + "_massMzResidual.png");
            PlotImageUtility.SaveImage(massScanResidual, directory + "_massScanResidual.png");
            report.PushEndTableRow();

            report.PushEndTable();
        }

        #endregion

        public void CreatePeakMatchedPlots(FeaturesPeakMatchedEventArgs e)
        {
        }
    }
}