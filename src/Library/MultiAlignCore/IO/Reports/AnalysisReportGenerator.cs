using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.Drawing;
using PNNLOmicsViz.Drawing;

namespace MultiAlignCore.IO.Reports
{
    public sealed class AnalysisReportGenerator : IAnalysisReportGenerator
    {
        /// <summary>
        /// Default path for plots.
        /// </summary>
        private const string THUMBNAIL_PATH = "Plots";

        public AnalysisReportGenerator()
        {
            PlotPath = THUMBNAIL_PATH;
        }

        public AnalysisConfig Config
        {
            get;
            set;
        }
        public string PlotPath
        {
            get;
            set;
        }
      
        #region Plot Methods

        /// <summary>
        /// 
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
            //var clusters = args.Clusters;
            
            //TODO: Create a histogram of the cluster sizes
            // Plots: 
            //      Dataset member
            //      Total members
            //      Cluster scatter plot
            //Config.Report.PushTextHeader("Cluster Data" + chargeStateAmmendum);
            //Config.Report.PushStartTable();
            //Config.Report.PushStartTableRow();
            //Config.Report.PushImageColumn(Path.Combine("Plots", imageNameMemberSizes + ".png"));
                                    
        }
        /// <summary>
        /// Creates the final analysis plots.
        /// </summary>
        public void CreateChargePlots(Dictionary<int, int> chargeMap)
        {
            Logger.PrintMessage("Creating Charge State Plots");

            var path = Path.Combine(Config.AnalysisPath, "chargestates.png");
            var histogram = HistogramFactory.CreateHistogram(chargeMap, "Charge States");
            PlotImageUtility.SaveImage(histogram, path);
        }
        /// <summary>
        /// Creates the HTML output file.
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
            
            if (e.Database == null) return;
            if (e.MassTags == null) return;

            var feature     = ScatterPlotFactory.CreateFeatureMassScatterPlot(e.MassTags);
            var name        = e.Database.Name;
            var directory   = Path.Combine(Config.AnalysisPath, name);

            PlotImageUtility.SaveImage(feature, directory + "_mtdb.png");                       
        }
        /// <summary>
        /// Creates images for the HTML output for baseline data
        /// </summary>
        /// <param name="e"></param>
        public void CreateBaselinePlots(BaselineFeaturesLoadedEventArgs e)
        {
            var baselineInfo    = e.DatasetInformation;
            if (baselineInfo == null) return;

            var feature         = ScatterPlotFactory.CreateFeatureMassScatterPlot(e.Features);
            var name            = e.DatasetInformation.DatasetName;
            var directory       = Path.Combine(Config.AnalysisPath, name);

            PlotImageUtility.SaveImage(feature, directory + "_features.png");
        }
        /// <summary>
        /// Creates alignment plots for the HTML output.
        /// </summary>
        public void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            var name          = e.AligneeDatasetInformation.DatasetName;            
            var alignmentData = e.AlignmentData;
            if (alignmentData == null) return;

            var directory           = Path.Combine(Config.AnalysisPath, PlotPath, name);                    
            var heatmap             = HeatmapFactory.CreateAlignedHeatmap(alignmentData.heatScores);
            var feature             = ScatterPlotFactory.CreateFeatureMassScatterPlot(e.AligneeFeatures);
            var netHistomgram       = HistogramFactory.CreateHistogram(alignmentData.netErrorHistogram, "NET Error");
            var massHistomgram      = HistogramFactory.CreateHistogram(alignmentData.netErrorHistogram, "Mass Error");


            var residuals = alignmentData.ResidualData;

            var netResidual         = ScatterPlotFactory.CreateResidualPlot(residuals.scans, residuals.linearCustomNet, residuals.linearNet, "NET Residuals", "Scans", "NET");
            var massMzResidual      = ScatterPlotFactory.CreateResidualPlot(residuals.mz, residuals.mzMassError, residuals.mzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");
            var massScanResidual    = ScatterPlotFactory.CreateResidualPlot(residuals.scans, residuals.mzMassError, residuals.mzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");

            Config.Report.PushImageColumn(directory + "_netHistomgram.png");

            PlotImageUtility.SaveImage(heatmap                   , directory +  "_heatmap.png");
            PlotImageUtility.SaveImage(feature                   , directory +  "_features.png");
            PlotImageUtility.SaveImage(netHistomgram             , directory +  "_netHistomgram.png");
            PlotImageUtility.SaveImage(massHistomgram            , directory +  "_massHistomgram.png");
            PlotImageUtility.SaveImage(netResidual               , directory +  "_netResidual.png");
            PlotImageUtility.SaveImage(massMzResidual            , directory +  "_massMzResidual.png");
            PlotImageUtility.SaveImage(massScanResidual          , directory +  "_massScanResidual.png");            
        }
        #endregion


        public void CreatePeakMatchedPlots(FeaturesPeakMatchedEventArgs e)
        {
            
        }
    }
}
