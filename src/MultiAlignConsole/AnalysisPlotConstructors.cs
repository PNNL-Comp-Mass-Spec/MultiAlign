using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Drawing;

namespace MultiAlignConsole.Drawing
{
    public class AnalysisReportGenerator
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
        public void SaveImage(Image image, string name)
        {
            if (image != null)
            {
                string path = Path.Combine(Config.plotSavePath, name);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                Logger.PrintMessage(string.Format("Could not create {0} plot.", name));
            }
        }
        /// <summary>
        /// Creates the final analysis plots
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="clusterCache"></param>
        public void CreateFinalAnalysisPlots()
        {
            UmcDAOHibernate cache               = new UmcDAOHibernate();
            UmcClusterDAOHibernate clusterCache = new UmcClusterDAOHibernate();

            CreateFinalAnalysisPlots(cache, clusterCache);
        }
        /// <summary>
        /// Creates the final analysis plots.
        /// </summary>
        public void CreateFinalAnalysisPlots(IUmcDAO cache, IUmcClusterDAO clusterCache)
        {
            Logger.PrintMessage("Creating Final Plots");
            Config.Report.PushTextHeader("Analysis Info ");
            Config.Report.PushStartTable();
            Config.Report.PushStartTableRow();

            // Create the heatmap
            List<clsUMC> umcs = cache.FindAll();
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true,
                                                1, 100,
                                                "Charge State Histogram", "Charge State", "Count", Config.width, Config.height);
            options.DisplayLegend = false;
            Image image = RenderDatasetInfo.ChargeStateHistogram_Thumbnail(umcs, Config.width, Config.height, options);
            SaveImage(image, "ChargeStates.png");
            Config.Report.PushImageColumn(Path.Combine("Plots", "ChargeStates.png"));

            Config.Report.PushEndTableRow();
            Config.Report.PushEndTable();

            Config.Report.PushTextHeader("Cluster Data");
            Config.Report.PushStartTable();
            Config.Report.PushStartTableRow();

            List<clsCluster> clusters = clusterCache.FindAll();
            options.Title = "Cluster Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            options.DisplayLegend = false;
            image = RenderDatasetInfo.ClusterSizeHistogram_Thumbnail(clusters, Config.width, Config.height, options);
            SaveImage(image, "ClusterMemberSizes.png");
            Config.Report.PushImageColumn(Path.Combine("Plots", "ClusterMemberSizes.png"));

            options.Title = "Cluster Dataset Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            image = RenderDatasetInfo.ClusterDatasetMemberSizeHistogram_Thumbnail(clusters, Config.width, Config.height, options);
            SaveImage(image, "ClusterDatasetMemberSizes.png");
            Config.Report.PushImageColumn(Path.Combine("Plots", "ClusterDatasetMemberSizes.png"));

            // Mass vs. Cluster score 
            options.Title = "Clusters";
            options.YAxisLabel = "Cluster Monoisotopic Mass";
            options.XAxisLabel = "Cluster NET";
            image = RenderDatasetInfo.ClusterScatterPlot_Thumbnail(clusters, options);
            SaveImage(image, "ClusterScatterPlot.png");
            Config.Report.PushImageColumn(Path.Combine(PlotPath, "ClusterScatterPlot.png"));
            Config.Report.PushEndTableRow();
            Config.Report.PushStartTableRow();
            Config.Report.PushEndTable();


            Config.Report.PushStartTable(true);
            Config.Report.PushStartTableRow();
            Config.Report.PushStartTableColumn();
            Config.Report.PushData("Dataset Members");
            Config.Report.PushEndTableColumn();

            Config.Report.PushStartTableColumn();
            Config.Report.PushData("Count");
            Config.Report.PushEndTableColumn();
            Config.Report.PushEndTableRow();

            float[] histogram = MultiAlignCore.Data.Cluster.ClusterStats.GetClusterMemberSizes(clusters);
            if (histogram != null)
            {
                for (int i = 0; i < histogram.Length; i++)
                {
                    Config.Report.PushStartTableRow();
                    Config.Report.PushStartTableColumn();
                    Config.Report.PushData(i.ToString());
                    Config.Report.PushEndTableColumn();

                    Config.Report.PushStartTableColumn();
                    Config.Report.PushData(histogram[i].ToString());
                    Config.Report.PushEndTableColumn();
                    Config.Report.PushEndTableRow();
                }
            }
            Config.Report.PushEndTable();
        }
        /// <summary>
        /// Creates the HTML output file.
        /// </summary>
        public void CreatePlotReport()
        {
            Logger.PrintMessage("Creating Report.");
            string htmlPath = Config.HtmlPathName;
            if (Config.AnalysisPath != null)
            {
                htmlPath = Path.Combine(Config.AnalysisPath, Config.HtmlPathName);
            }
            Config.Report.AnalysisName = Config.AnalysisName;
            Config.Report.CreateReport(htmlPath);
        }
        /// <summary>
        /// Creates alignment plots.
        /// </summary>
        public void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            string name = e.AligneeDatasetInformation.DatasetName;
            Logger.PrintMessage("Features Aligned - " + name);

            // Hack so that the baseline plot is made first.
            if (!Config.createdBaselinePlots)
            {
                Config.createdBaselinePlots = true;
                if (Config.Analysis.MetaData.BaselineDataset != null)
                {
                    Config.Report.PushTextHeader("Baseline Dataset for " + Config.Analysis.MetaData.BaselineDataset.DatasetName);
                    Config.Report.PushStartTable();
                    Config.Report.PushStartTableRow();

                    DatasetInformation baselineInfo = Config.Analysis.MetaData.BaselineDataset;
                    ChartDisplayOptions baselineOptions = new ChartDisplayOptions(false, true, true, true);
                    baselineOptions.MarginMin = 1;
                    baselineOptions.MarginMax = 100;
                    baselineOptions.Title = "Feature Plot " + baselineInfo.DatasetName;
                    baselineOptions.XAxisLabel = "Scan";
                    baselineOptions.YAxisLabel = "Monoisotopic Mass";
                    baselineOptions.Width = Config.width;
                    baselineOptions.Height = Config.height;
                    baselineOptions.DisplayLegend = true;
                    List<clsUMC> baselineUmcs = Config.dataProviders.FeatureCache.FindByDatasetId(Convert.ToInt32(baselineInfo.DatasetId));
                    Image baselineImage = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(baselineUmcs, baselineOptions);
                    string baselineLabelName = Path.GetFileNameWithoutExtension(baselineInfo.DatasetName) + "_featurePlot.png";
                    string baselinePath = Path.Combine(Config.plotSavePath, baselineLabelName);
                    baselineImage.Save(baselinePath, System.Drawing.Imaging.ImageFormat.Png);
                    Config.Report.PushImageColumn(Path.Combine("Plots", baselineLabelName));
                    Config.Report.PushEndTableRow();
                    Config.Report.PushEndTable();
                }
            }

            Config.Report.PushTextHeader("Alignment Plots for " + e.AligneeDatasetInformation.DatasetName);
            Config.Report.PushStartTable();
            Config.Report.PushStartTableRow();
            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);

            options.MarginMin = 1;
            options.MarginMax = 100;
            options.Title = "Feature Plot " + name;
            options.XAxisLabel = "Scan";
            options.YAxisLabel = "Monoisotopic Mass";
            options.Width = Config.width;
            options.Height = Config.height;
            options.DisplayLegend = true;

            List<clsUMC> umcs = Config.dataProviders.FeatureCache.FindByDatasetId(Convert.ToInt32(e.AligneeDatasetInformation.DatasetId));
            Image image = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(umcs, options);
            string labelName = Path.GetFileNameWithoutExtension(name) + "_featurePlot.png";
            string path = Path.Combine(Config.plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.MarginMin = 1;
            options.MarginMax = 100;
            options.Title = "Alignment Heatmap " + name;
            options.XAxisLabel = "Baseline";
            options.YAxisLabel = "Alignee";
            options.Width = Config.width;
            options.Height = Config.height;

            image = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(e.AlignmentData, Config.width, Config.height);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            labelName = Path.GetFileNameWithoutExtension(name) + "_heatmap.png";
            path = Path.Combine(Config.plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayLegend = false;
            options.Title = "NET Error Histogram " + name;
            options.XAxisLabel = "NET Error (%)";
            options.YAxisLabel = "Count";
            image = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.netErrorHistogram, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_netErrorHistogram.png";
            path = Path.Combine(Config.plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayGridLines = true;
            options.DisplayLegend = false;
            options.Title = "Net vs. Scan Residuals" + name;
            image = RenderDatasetInfo.NETResiduals_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_netResiduals.png";
            path = Path.Combine(Config.plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.Title = "Mass Error Histogram " + name;
            options.XAxisLabel = "Mass Error (PPM)";
            image = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.massErrorHistogram, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_massErrorHistogram.png";
            path = Path.Combine(Config.plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));



            if (e.AlignmentData.driftErrorHistogram != null)
            {
                options.Title = "Drift Time Error Histogram " + name;
                options.XAxisLabel = "Drift Time Error (ms)";
                image = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.driftErrorHistogram, options);
                if (image != null)
                {
                    labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimeErrorHistogram.png";
                    path = Path.Combine(Config.plotSavePath, labelName);
                    image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    Config.Report.PushImageColumn(Path.Combine("Plots", labelName));
                }
            }

            options.DisplayLegend = true;
            options.Title = "Mass vs. Scan Residuals" + name;
            image = RenderDatasetInfo.MassVsScanResiduals_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_massScanResiduals.png";
            path = Path.Combine(Config.plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayLegend = true;
            options.Title = "Mass vs. m/z Residuals" + name;
            image = RenderDatasetInfo.ClusterMassVsMZResidual_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_massMZResiduals.png";
            path = Path.Combine(Config.plotSavePath, labelName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));
            Config.Report.PushEndTableRow();

            if (e.DriftTimeAlignmentData != null)
            {

                options.DisplayLegend = false;
                options.Title = "Drift Time Plot";
                options.XAxisLabel = "Baseline Drift Times (ms)";
                options.YAxisLabel = "Alignee Drift Times (ms)";

                List<FeatureMatch<UMC, UMC>> matches = e.DriftTimeAlignmentData.Matches;
                int totalMatches = matches.Count;
                float[] x = new float[totalMatches];
                float[] yC = new float[totalMatches];
                float[] y = new float[totalMatches];

                int i = 0;
                foreach (FeatureMatch<UMC, UMC> match in matches)
                {
                    y[i] = Convert.ToSingle(match.ObservedFeature.DriftTime);
                    yC[i] = Convert.ToSingle(match.ObservedFeature.DriftTimeAligned);
                    x[i] = Convert.ToSingle(match.TargetFeature.DriftTime);
                    i++;
                }
                labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimes.png";
                path = Path.Combine(Config.plotSavePath, labelName);
                image = GenericPlotFactory.ScatterPlot_Thumbnail(x, y, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

                options.Title = "Aligned Drift Time Plot";
                labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimesAligned.png";
                path = Path.Combine(Config.plotSavePath, labelName);
                image = GenericPlotFactory.ScatterPlot_Thumbnail(x, yC, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

                options.Title = "Drift Time Error Distributions";
                labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimesErrorHistogram.png";
                path = Path.Combine(Config.plotSavePath, labelName);
                image = GenericPlotFactory.ResidualHistogram_Thumbnail(x, y, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

                options.Title = "Aligned Drift Time Error Distributions";
                labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimesErrorHistogramAligned.png";
                path = Path.Combine(Config.plotSavePath, labelName);
                image = GenericPlotFactory.ResidualHistogram_Thumbnail(x, yC, options);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName));
                Config.Report.PushEndTableRow();
            }

            Config.Report.PushEndTable();
        }
        #endregion
    }
}
