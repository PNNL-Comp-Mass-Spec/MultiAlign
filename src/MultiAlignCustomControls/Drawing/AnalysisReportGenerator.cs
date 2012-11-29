using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCustomControls.Charting;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCustomControls.Drawing
{
    public class AnalysisReportGenerator : IAnalysisReportGenerator
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
        public void SaveImage(Image image, string name, string description)
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
        public void CreateClusterPlots(List<UMCClusterLight> clusters)
        {
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true,
                                                1, 100,
                                                "Charge State Histogram", "Charge State", "Count", Config.width, Config.height);
            options.DisplayLegend = false;

            Config.Report.PushTextHeader("Cluster Data");
            Config.Report.PushStartTable();
            Config.Report.PushStartTableRow();
            
            options.Title = "Cluster Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            options.DisplayLegend = false;
            Image image = RenderDatasetInfo.ClusterSizeHistogram_Thumbnail(clusters, Config.width, Config.height, options);
            SaveImage(image, "ClusterMemberSizes.png", "Cluster Member Sizes");
            Config.Report.PushImageColumn(Path.Combine("Plots", "ClusterMemberSizes.png"));

            options.Title = "Cluster Dataset Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            image = RenderDatasetInfo.ClusterDatasetMemberSizeHistogram_Thumbnail(clusters, Config.width, Config.height, options);
            SaveImage(image, "ClusterDatasetMemberSizes.png", "Cluster Dataset Member Sizes");
            Config.Report.PushImageColumn(Path.Combine("Plots", "ClusterDatasetMemberSizes.png"));

            // Mass vs. Cluster score 
            options.Title = "Clusters";
            options.YAxisLabel = "Cluster Monoisotopic Mass";
            options.XAxisLabel = "Cluster NET";
            image = RenderDatasetInfo.ClusterScatterPlot_Thumbnail(clusters, options);
            SaveImage(image, "ClusterScatterPlot.png", "Clusters");
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

            float[] histogram = MultiAlignCore.Data.Features.ClusterStats.GetClusterMemberSizes(clusters);
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
        /// Creates the final analysis plots.
        /// </summary>
        public void CreateChargePlots(Dictionary<int, int> chargeMap)
        {
            Logger.PrintMessage("Creating Charge State Plots");
            Config.Report.PushTextHeader("Analysis Info ");
            Config.Report.PushStartTable();
            Config.Report.PushStartTableRow();

            // Create the heatmap            
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true,
                                                1, 100,
                                                "Charge State Histogram", "Charge State", "Count", Config.width, Config.height);
            options.DisplayLegend   = false;            
            Image image = RenderDatasetInfo.CreateHistogram_Thumbnail(chargeMap, Config.width, Config.height, options);
            SaveImage(image, "ChargeStates.png", "Charge State Histogram");
            Config.Report.PushImageColumn(Path.Combine("Plots", "ChargeStates.png"));

            Config.Report.PushEndTableRow();
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
        public void CreateMassTagPlot(MassTagsLoadedEventArgs e)
        {            
            Logger.PrintMessage("Creating Mass Tag Plot.");
            
            Config.Report.PushTextHeader("Mass Tag Plot ");
            Config.Report.PushStartTable();
            Config.Report.PushStartTableRow();
            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);

            options.MarginMin = 1;
            options.MarginMax = 100;
            options.Title = "Mass Tags ";
            options.XAxisLabel = "GA NET";
            options.YAxisLabel = "Monoisotopic Mass";
            options.Width           = Config.width;
            options.Height          = Config.height;
            options.DisplayLegend   = true;

            List<PNNLOmics.Data.MassTags.MassTagLight> tags = e.MassTags;
            List<PointF> points = new List<PointF>();
            foreach (PNNLOmics.Data.MassTags.MassTagLight tagLight in tags)
            {
                PointF point = new PointF(Convert.ToSingle(tagLight.NETAverage), Convert.ToSingle(tagLight.MassMonoisotopic));
                points.Add(point);
            }

            SeriesOptions series = new SeriesOptions();
            series.Points        = points;
            series.Label         = "Mass Tags";
            series.Color         = Color.Red;
            series.Shape         = new PNNLControls.BubbleShape(1, false);

            Image image      = RenderDatasetInfo.GenericScatterPlot_Thumbnail(new List<SeriesOptions>() {series}, options);
            string labelName = "massTags_plot.png";            
            SaveImage(image, labelName, "Mass Tag Plots");
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            Config.Report.PushEndTableRow();
            Config.Report.PushEndTable();

            Config.Report.PushLargeText("Mass Tag Database Stats");
            Config.Report.PushStartTable();
            Config.Report.PushStartTableRow();
            string databaseTags = string.Format("Number Of Mass Tags Loaded {0}", e.MassTags.Count);
            Config.Report.PushData(databaseTags);
            Config.Report.PushEndTableRow();
            Config.Report.PushEndTable();
        }
        public void CreatePeakMatchedPlots(FeaturesPeakMatchedEventArgs e)
        {
            Config.Report.PushTextHeader("Clusters to Mass Tag Matches");
            Config.Report.PushStartTable();
            Config.Report.PushStartTableRow();
            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);

            options.MarginMin       = 1;
            options.MarginMax       = 100;
            options.Title           = "Cluster matches with Mass Tags ";
            options.XAxisLabel      = "NET";
            options.YAxisLabel      = "Monoisotopic Mass";
            options.Width           = Config.width;
            options.Height          = Config.height;
            options.DisplayLegend   = true;

            /// Get all of the matches first.
            List<FeatureMatchLight<UMCClusterLight, MassTagLight>> matches = e.Matches;
            List<PointF> matchPoints   = new List<PointF>();
            List<PointF> clusterPoints = new List<PointF>();

            Dictionary<int, UMCClusterLight> clusterMap = new Dictionary<int, UMCClusterLight>();
            foreach (UMCClusterLight cluster in e.Clusters)
            {
                clusterMap.Add(cluster.ID, cluster);
            }

            double confidence   = 0;
            double step         = .1;
            int columnIndex     =  0;

            Image  image     = null; 
            string labelName = ""; 
            //string path      = "";  
            SeriesOptions clusterSeries = null;

            while (confidence <= .9)
            {
                matchPoints.Clear();
                clusterPoints.Clear();

                options.Title = "Cluster Matches at STAC = " + string.Format("{0:.00}", confidence);
 
                // Matches Series ----------------------------------------------------
                foreach (FeatureMatchLight<UMCClusterLight, MassTagLight> match in matches)
                {
                    if (clusterMap.ContainsKey(match.Observed.ID))
                    {
                        clusterMap.Remove(match.Observed.ID);
                    }

                    if (match.Confidence < confidence)
                        continue;

                    PointF matchPoint = new PointF(Convert.ToSingle(match.Target.NET), Convert.ToSingle(match.Target.MassMonoisotopic));
                    matchPoints.Add(matchPoint);



                    PointF clusterPoint = new PointF(Convert.ToSingle(match.Observed.NET), Convert.ToSingle(match.Observed.MassMonoisotopic));
                    clusterPoints.Add(clusterPoint);
                }

                SeriesOptions tagsSeries =
                    new SeriesOptions()
                    {
                        Shape = new PNNLControls.SquareShape(2, true),
                        Label = "Matched Tags",
                        Color = Color.Red,
                        Points = matchPoints
                    };

                clusterSeries = new SeriesOptions()
                   {
                       Shape = new PNNLControls.BubbleShape(2, false),
                       Label = "Matched Clusters",
                       Color = Color.Black,
                       Points = clusterPoints
                   };

                image       = RenderDatasetInfo.GenericScatterPlot_Thumbnail(new List<SeriesOptions>() { clusterSeries, tagsSeries }, options);
                labelName   = string.Format("matches-STAC-{0}", confidence - step);                
                SaveImage(image, labelName, "STAC Matches");
                Config.Report.PushData(labelName);
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName + ".png"));

                confidence += step;
                columnIndex++;

                if (columnIndex > 3)
                {
                    columnIndex = 0;
                    Config.Report.PushEndTableRow();
                    Config.Report.PushStartTableRow();
                }
            }

            // Unattributed Series ----------------------------------------------------
            List<PointF> unattributedPoints = new List<PointF>();
            foreach (int key in clusterMap.Keys)
            {
                UMCClusterLight cluster = clusterMap[key];
                PointF clusterPoint = new PointF(Convert.ToSingle(cluster.NET), Convert.ToSingle(cluster.MassMonoisotopic));
                unattributedPoints.Add(clusterPoint);                    
            }

            SeriesOptions unattributedSeries = new SeriesOptions()
            {
                Shape  = new PNNLControls.PlusShape(3, false),
                Label  = "Unattributed Clusters",
                Points = unattributedPoints,
                Color  = Color.Green                
            };

            options.Title   = "Unattributed clusters";
            image           = RenderDatasetInfo.GenericScatterPlot_Thumbnail(new List<SeriesOptions>() {unattributedSeries}, options);
            labelName       = "unattributedClusters.png";            
            SaveImage(image, labelName, "Unattributed Clusters");
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            // All Series ----------------------------------------------------
            options.Title       = string.Format("Clusters (STAC = {0:.00}) and Unattributed Clusters", confidence);
            clusterSeries.Label = "Matched Clusters";
            image               = RenderDatasetInfo.GenericScatterPlot_Thumbnail(new List<SeriesOptions>() {unattributedSeries, clusterSeries}, options);
            labelName           = "clustersAndUnattributedClusters.png";            
            SaveImage(image, labelName, "Clusters and Unattributed Clusters");
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            Config.Report.PushEndTableRow();
            Config.Report.PushEndTable();

        }
        public void CreateBaselinePlots(BaselineFeaturesLoadedEventArgs e)
        {
            DatasetInformation baselineInfo = e.DatasetInformation;
            if (baselineInfo != null)
            {
                Config.Report.PushTextHeader("Baseline Dataset: " + baselineInfo.DatasetName);
                Config.Report.PushStartTable();
                Config.Report.PushStartTableRow();

                // chart setup
                ChartDisplayOptions baselineOptions = new ChartDisplayOptions(false, true, true, true);
                baselineOptions.MarginMin           = 1;
                baselineOptions.MarginMax           = 100;
                baselineOptions.Title               = "Feature Plot " + baselineInfo.DatasetName;
                baselineOptions.XAxisLabel          = "Scan";
                baselineOptions.YAxisLabel          = "Monoisotopic Mass";
                baselineOptions.Width               = Config.width;
                baselineOptions.Height              = Config.height;
                baselineOptions.DisplayLegend       = true;
                
                // Image creation and saving
                List<UMCLight> baselineFeatures     = e.Features;                
                Image baselineImage                 = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(baselineFeatures, baselineOptions);
                string baselineLabelName            = Path.GetFileNameWithoutExtension(baselineInfo.DatasetName) + "_featurePlot.png";
                string baselinePath                 = Path.Combine(Config.plotSavePath, baselineLabelName);                
                SaveImage(baselineImage, baselineLabelName, "Baseline feature plot ");

                // Report setup.
                Config.Report.PushImageColumn(Path.Combine("Plots", baselineLabelName));
                Config.Report.PushEndTableRow();
                Config.Report.PushEndTable();   
            }           
            else
            {
                //TODO: Load information about the mass tag database.  
            }
        }
        /// <summary>
        /// Creates alignment plots.
        /// </summary>
        public void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            string name = e.AligneeDatasetInformation.DatasetName;
            Logger.PrintMessage("Features Aligned - " + name);
            
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


            List<UMCLight> features = e.AlignedFeatures;            
            Image image      = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(features, options);
            string labelName = Path.GetFileNameWithoutExtension(name) + "_featurePlot.png";            
            SaveImage(image, labelName, "Features " + name);
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
            SaveImage(image, labelName, "Alignment heat map " + name);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayLegend = false;
            options.Title = "NET Error Histogram " + name;
            options.XAxisLabel = "NET Error (%)";
            options.YAxisLabel = "Count";
            image = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.netErrorHistogram, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_netErrorHistogram.png";
            SaveImage(image, labelName, "NET error histogram " + name);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayGridLines = true;
            options.DisplayLegend = false;
            options.Title = "Net vs. Scan Residuals" + name;
            image = RenderDatasetInfo.NETResiduals_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_netResiduals.png";
            SaveImage(image, labelName, "NET vs. scan residuals " + name);
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.Title = "Mass Error Histogram " + name;
            options.XAxisLabel = "Mass Error (PPM)";
            image = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.massErrorHistogram, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_massErrorHistogram.png";
            SaveImage(image, labelName, "Mass error histogram (ppm) " + name );
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));



            if (e.AlignmentData.driftErrorHistogram != null)
            {
                options.Title = "Drift Time Error Histogram " + name;
                options.XAxisLabel = "Drift Time Error (ms)";
                image = RenderDatasetInfo.ErrorHistogram_Thumbnail(e.AlignmentData.driftErrorHistogram, options);
                if (image != null)
                {
                    labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimeErrorHistogram.png";
                    SaveImage(image, labelName, "Drift time error histogram " + name);
                    Config.Report.PushImageColumn(Path.Combine("Plots", labelName));
                }
            }

            options.DisplayLegend = true;
            options.Title   = "Mass vs. Scan Residuals" + name;
            image           = RenderDatasetInfo.MassVsScanResiduals_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName       = Path.GetFileNameWithoutExtension(name) + "_massScanResiduals.png";           
            SaveImage(image, labelName, "Mass vs Scan Residuals");
            Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

            options.DisplayLegend = true;
            options.Title = "Mass vs. m/z Residuals" + name;
            image = RenderDatasetInfo.ClusterMassVsMZResidual_Thumbnail(e.AlignmentData.ResidualData, options);
            labelName = Path.GetFileNameWithoutExtension(name) + "_massMZResiduals.png";            
            SaveImage(image, labelName, "Mass vs m/z Residuals");
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
                image = GenericPlotFactory.ScatterPlot_Thumbnail(x, y, options);
                SaveImage(image, labelName, "Drift Times " + name);
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

                options.Title = "Aligned Drift Time Plot";
                labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimesAligned.png";                
                image = GenericPlotFactory.ScatterPlot_Thumbnail(x, yC, options);
                SaveImage(image, labelName, "Aligned Drift Times " + name);
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

                options.Title = "Drift Time Error Distributions";
                labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimesErrorHistogram.png";
                image = GenericPlotFactory.ResidualHistogram_Thumbnail(x, y, options);
                SaveImage(image, labelName, "Drift Time Error Distributions " + name);
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName));

                options.Title = "Aligned Drift Time Error Distributions";
                labelName = Path.GetFileNameWithoutExtension(name) + "_driftTimesErrorHistogramAligned.png";                
                image = GenericPlotFactory.ResidualHistogram_Thumbnail(x, yC, options);
                SaveImage(image, labelName, "Aligned Drift Time Error Distributions");
                Config.Report.PushImageColumn(Path.Combine("Plots", labelName));
                Config.Report.PushEndTableRow();
            }

            Config.Report.PushEndTable();
        }
        #endregion
    }
}
