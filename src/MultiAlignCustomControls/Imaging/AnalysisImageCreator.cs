
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Drawing;
using PNNLOmics.Data.Features;
using System.Collections.Generic;
using System;
using System.Drawing;
using MultiAlignCore.Data.MassTags;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.Data.Imaging
{
    public static class AnalysisImageCreator
    {
        public static AlignmentImageData CreateAlignmentPlots(FeaturesAlignedEventArgs data,
                                                            int width,
                                                            int height)
        {

            return CreateAlignmentPlots(data, width, height, true);
        }
            public static AlignmentImageData CreateAlignmentPlots(FeaturesAlignedEventArgs data,
                                                            int width, 
                                                            int height,
                        bool shouldDisplayText)
        {

            AlignmentImageData imageData = new AlignmentImageData();
            string name = data.AligneeDatasetInformation.DatasetName;

            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);
            SetupDisplayOptions(shouldDisplayText, options);

            options.MarginMin = 1;
            options.MarginMax = 100;
            options.Title = "Feature Plot " + name;
            options.XAxisLabel = "Scan";
            options.YAxisLabel = "Monoisotopic Mass";
            
            options.Width  = width;
            options.Height = height;

            List<UMCLight> features     = data.AlignedFeatures;
            imageData.FeaturePlotImage  = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(features, options);
            
            options.MarginMin = 1;
            options.MarginMax = 100;
            options.Title = "Alignment Heatmap ";
            options.XAxisLabel = "Baseline";
            options.YAxisLabel = "Alignee";
            options.Width  = width;
            options.Height = height;
            imageData.HeatmapImage = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(data.AlignmentData, width, height);
            imageData.HeatmapImage.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

            options.Title = "NET Error Histogram " + name;
            options.XAxisLabel = "NET Error (%)";
            options.YAxisLabel = "Count";
            imageData.NetHistogramImage = RenderDatasetInfo.ErrorHistogram_Thumbnail(data.AlignmentData.netErrorHistogram, options);

            options.Title = "Net vs. Scan Residuals" + name;
            imageData.NetResidualsHistogramImage = RenderDatasetInfo.NETResiduals_Thumbnail(data.AlignmentData.ResidualData, options);

            options.Title = "Mass Error Histogram " + name;
            options.XAxisLabel = "Mass Error (PPM)";
            imageData.MassHistogramImage = RenderDatasetInfo.ErrorHistogram_Thumbnail(data.AlignmentData.massErrorHistogram, options);

            options.Title = "Mass vs. Scan Residuals" + name;
            imageData.MassScanImage = RenderDatasetInfo.MassVsScanResiduals_Thumbnail(data.AlignmentData.ResidualData, options);

            options.Title = "Mass vs. m/z Residuals" + name;
            imageData.MassMzImage = RenderDatasetInfo.ClusterMassVsMZResidual_Thumbnail(data.AlignmentData.ResidualData, options);


            if (data.AlignmentData.driftErrorHistogram != null)
            {
                options.Title = "Drift Time Error Histogram " + name;
                options.XAxisLabel = "Drift Time Error (ms)";
                imageData.DriftTimeHistogramImage = RenderDatasetInfo.ErrorHistogram_Thumbnail(data.AlignmentData.driftErrorHistogram, options);
                                
                options.Title = "Drift Time Plot";
                options.XAxisLabel = "Baseline Drift Times (ms)";
                options.YAxisLabel = "Alignee Drift Times (ms)";

                if (data.DriftTimeAlignmentData != null)
                {
                    List<FeatureMatch<UMC, UMC>> matches = data.DriftTimeAlignmentData.Matches;
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
                    imageData.DriftTimeScatterImage = GenericPlotFactory.ScatterPlot_Thumbnail(x, y, options);

                    options.Title = "Aligned Drift Time Plot";
                    imageData.DriftTimeAlignedErrorImage = GenericPlotFactory.ScatterPlot_Thumbnail(x, yC, options);

                    options.Title = "Drift Time Error Distributions";
                    imageData.DriftTimeErrorImage = GenericPlotFactory.ResidualHistogram_Thumbnail(x, y, options);

                    options.Title = "Aligned Drift Time Error Distributions";
                    imageData.DriftTimeHistogramImage = GenericPlotFactory.ResidualHistogram_Thumbnail(x, yC, options);
                }
            }
            return imageData;
        }

            private static void SetupDisplayOptions(bool shouldDisplayText, ChartDisplayOptions options)
            {
                options.DisplayTitle        = shouldDisplayText;
                options.DisplayLegend       = shouldDisplayText;                
                options.DisplayGridLines    = shouldDisplayText;
            }

        public static FeatureImageData CreateBaselinePlots(BaselineFeaturesLoadedEventArgs data,
                                                            int width,
                                                            int height)
        {
            return CreateBaselinePlots(data, width, height, true);
        }
        public static FeatureImageData CreateBaselinePlots(BaselineFeaturesLoadedEventArgs data,
                                                            int width,
                                                            int height,
                                                            bool shouldDisplayText)
        {
            FeatureImageData imageData      = new FeatureImageData();
            DatasetInformation baselineInfo = data.DatasetInformation;

            if (baselineInfo != null)
            {
                // chart setup
                ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);
                options.MarginMin = 1;
                options.MarginMax = 100;
                options.Title = "Feature Plot " + baselineInfo.DatasetName;
                options.XAxisLabel = "Scan";
                options.YAxisLabel = "Monoisotopic Mass";
                options.Width = width;
                options.Height = height;

                SetupDisplayOptions(shouldDisplayText, options);

                // Image creation and saving
                List<UMCLight> baselineFeatures = data.Features;
                imageData.FeatureImage = RenderDatasetInfo.FeaturesScatterPlot_Thumbnail(baselineFeatures, options);                
            }
            else
            {
                imageData.FeatureImage = CreateMassTagPlotImage(data.Database, width, height, shouldDisplayText);
                //TODO: Load information about the mass tag database.  
            }

            return imageData;
        }
        public static FeatureImageData CreateMassTagPlots(MassTagDatabase database,
                                                           int width,
                                                           int height,
                                                           bool shouldDisplayText)
        {
            FeatureImageData data   = new FeatureImageData();
            data.FeatureImage       = CreateMassTagPlotImage(database, width, height, shouldDisplayText);
            return data;
        }
        private static Image CreateMassTagPlotImage( MassTagDatabase database,
                                                    int width,
                                                    int height,
                                                    bool shouldDisplayText)
        {                               
            ChartDisplayOptions options = new ChartDisplayOptions(false, true, true, true);
            options.MarginMin           = 1;
            options.MarginMax           = 100;
            options.Title               = "Mass Tags ";
            options.XAxisLabel          = "NET";
            options.YAxisLabel          = "Monoisotopic Mass";
            options.Width               = width;
            options.Height              = height;

            SetupDisplayOptions(shouldDisplayText, options);
            
            List<MassTagLight> tags     = database.MassTags;
            List<PointF> points         = new List<PointF>();
            foreach (MassTagLight tagLight in tags)
            {
                PointF point = new PointF(Convert.ToSingle(tagLight.NETAverage), Convert.ToSingle(tagLight.MassMonoisotopic));
                points.Add(point);
            }

            SeriesOptions series    = new SeriesOptions();
            series.Points           = points;
            series.Label            = "Mass Tags";
            series.Color            = Color.Red;
            series.Shape            = new PNNLControls.BubbleShape(1, false);
            Image image             = RenderDatasetInfo.GenericScatterPlot_Thumbnail(new List<SeriesOptions>() {series}, options);

            return image;                      
        }


        public static Dictionary<string, Image> CreateChargePlots(Dictionary<int, int> chargeMap,
                                                                int width,
                                                                int height)
        {
            return CreateChargePlots(chargeMap, width, height, true);
        }
        public static Dictionary<string, Image> CreateChargePlots(Dictionary<int, int> chargeMap,
                                                                int width,
                                                                int height,
                                                                bool shouldDisplayText)
        {
            Dictionary<string, Image> images = new Dictionary<string, Image>();

            string name = "Charge State Histogram";
            // Create the heatmap            
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true,
                                                1, 100,
                                                name,
                                                "Charge State", 
                                                "Count", 
                                                width, 
                                                height);

            SetupDisplayOptions(shouldDisplayText, options);
            Image image                 = RenderDatasetInfo.CreateHistogram_Thumbnail(chargeMap, width, height, options);

            images.Add(name, image);
            return images;
        }
        public static ClustersImageData CreateClusterPlots(List<UMCClusterLight> clusters,
                                                               int width,
                                                               int height)
        {
            return CreateClusterPlots(clusters, width, height);
        }
        public static ClustersImageData CreateClusterPlots(List<UMCClusterLight> clusters,
                                                               int width,
                                                               int height,
                                                                bool shouldDisplayText)
        {
            ClustersImageData imageData = new ClustersImageData();

            ChartDisplayOptions options      = new ChartDisplayOptions(true, true, true, true,
                                                1, 100,
                                                "Charge State Histogram", "Charge State", "Count", width, height);

            SetupDisplayOptions(shouldDisplayText, options);

            options.Title           = "Cluster Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";            
            imageData.ClustersSizeHistogramImage = RenderDatasetInfo.ClusterSizeHistogram_Thumbnail(clusters, width, height, options);
            

            options.Title       = "Cluster Dataset Member Size Histogram ( Total Clusters = " + clusters.Count.ToString() + ")";
            imageData.ClustersDatasetSizeHistogramImage = RenderDatasetInfo.ClusterDatasetMemberSizeHistogram_Thumbnail(clusters, width, height, options);
            

            // Mass vs. Cluster score 
            options.Title       = "Clusters";
            options.YAxisLabel  = "Cluster Monoisotopic Mass";
            options.XAxisLabel  = "Cluster NET";
            imageData.ClustersImage = RenderDatasetInfo.ClusterScatterPlot_Thumbnail(clusters, options);



            return imageData;
        }
    }
}
