using System;
using System.Collections.Generic;
using System.Drawing;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCustomControls.Charting;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using PNNLControls;
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;

namespace MultiAlignCustomControls.Drawing
{
    /// <summary>
    /// Factory class for rendering information about a dataset info class.
    /// </summary>
    public static class RenderDatasetInfo
    {
        private const int CONST_PRE_POINT_SIZE  = 1;
        private const int CONST_POST_POINT_SIZE = 1;

        /// <summary>
        /// Creates a thumbnail from the chart provided.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Image CreateThumbnailFromChart(ctlChartBase chart, ChartDisplayOptions options)
        {
            chart.XAxisLabel    = options.XAxisLabel;
            chart.YAxisLabel    = options.YAxisLabel;
            chart.Title         = options.Title;

            chart.Margins.LeftMarginMin   = options.MarginMin;
            chart.Margins.LeftMarginMax   = options.MarginMax;
            chart.Margins.BottomMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMin = options.MarginMin;

            chart.LegendVisible  = options.DisplayLegend;
            chart.AxisVisible    = options.DisplayAxis;
            chart.TitleVisible   = options.DisplayTitle;
            chart.XAxisGridLines = options.DisplayGridLines;
            chart.YAxisGridLines = options.DisplayGridLines;

            return chart.ToBitmap(options.Width, options.Height);
        }

        #region Generic
        private static clsSeries CreateSeries(List<PointF> points, Color color, clsShape shape, string title)
        {
            clsPlotParams parameters = new clsPlotParams(shape, color);
            parameters.Name = title;

            float[] x = new float[points.Count];
            float[] y = new float[points.Count];
            int i = 0;

            foreach (PointF feature in points)
            {
                x[i] = feature.X;
                y[i] = feature.Y;
                i++;
            }
            clsSeries series = new clsSeries(ref x, ref y, parameters);
            return series;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dotFeatures"></param>
        /// <param name="xFeatuers"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Image GenericScatterPlot_Thumbnail(List<SeriesOptions> series, ChartDisplayOptions options)
        {
            // This is gross...but this whole class should be divided out.
            Image image = null;

            using (ctlChartBase chart = GenericScatterPlot_Chart(series, options))
            {
                if (chart == null)
                    return null;
                image = CreateThumbnailFromChart(chart, options);
            }
            return image;
        }
        public static ctlScatterChart GenericScatterPlot_Chart(List<SeriesOptions> series, ChartDisplayOptions options)
        {            
            ctlScatterChart chart   = new ctlScatterChart();
            chart.XAxisLabel        = options.XAxisLabel;
            chart.YAxisLabel        = options.YAxisLabel;
            chart.Title             = options.Title;

            chart.Margins.LeftMarginMin     = options.MarginMin;
            chart.Margins.LeftMarginMax     = options.MarginMax;
            chart.Margins.BottomMarginMax   = options.MarginMax;
            chart.Margins.BottomMarginMin   = options.MarginMin;

            chart.LegendVisible     = options.DisplayLegend;
            chart.AxisVisible       = options.DisplayAxis;
            chart.TitleVisible      = options.DisplayTitle;
            chart.XAxisGridLines    = options.DisplayGridLines;
            chart.YAxisGridLines    = options.DisplayGridLines;

            foreach (SeriesOptions ser in series)
            {
                clsSeries data = CreateSeries(ser.Points, ser.Color, ser.Shape, ser.Label);
                chart.AddSeries(data);   
            }
            chart.AutoViewPort();
            return chart;
        }
        #endregion

        #region NET Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image NETResiduals_Thumbnail(classAlignmentResidualData residual, ChartDisplayOptions options)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.NETResiduals_Chart(residual);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = options.MarginMin;
                    chart.Margins.LeftMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMin = options.MarginMin;


                    chart.Title = options.Title;
                    chart.LegendVisible = options.DisplayLegend;
                    chart.AxisVisible = options.DisplayAxis;
                    chart.TitleVisible = options.DisplayTitle;
                    chart.XAxisGridLines = options.DisplayGridLines;
                    chart.YAxisGridLines = options.DisplayGridLines;
                    image = chart.ToBitmap(options.Width, options.Height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        public static Image ClusterScatterPlot_Thumbnail(List<UMCClusterLight> clusters, ChartDisplayOptions options)
        {
            Image image = null;

            using (ctlChartBase chart = ClusterScatterPlot_Chart(clusters, options))
            {
                if (chart == null)
                    return null;
                image = CreateThumbnailFromChart(chart, options);
            }
            return image;
        }
        /// <summary>
        /// Creates a scan width chart for a set of clusters.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static ctlScatterChart ClusterScatterPlot_Chart(List<UMCClusterLight> clusters, ChartDisplayOptions options)
        {
            if (clusters == null)
                return null;

            if (clusters.Count < 1)
                return null;

            ctlScatterChart chart = new ctlScatterChart();
            chart.XAxisLabel = options.XAxisLabel;
            chart.YAxisLabel = options.YAxisLabel;
            chart.Title = options.Title;

            chart.Margins.LeftMarginMin = options.MarginMin;
            chart.Margins.LeftMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMin = options.MarginMin;

            chart.LegendVisible = options.DisplayLegend;
            chart.AxisVisible = options.DisplayAxis;
            chart.TitleVisible = options.DisplayTitle;
            chart.XAxisGridLines = options.DisplayGridLines;
            chart.YAxisGridLines = options.DisplayGridLines;

            float[] x = new float[clusters.Count];
            float[] y = new float[clusters.Count];

            int i = 0;
            foreach (UMCClusterLight cluster in clusters)
            {
                y[i] = Convert.ToSingle(cluster.MassMonoisotopic);
                x[i] = Convert.ToSingle(cluster.RetentionTime);
                i++;
            }
            clsPlotParams parameters = new clsPlotParams(new BubbleShape(1, false), Color.Black);
            clsSeries series = new clsSeries(ref x, ref y, parameters);

            chart.AddSeries(series);
            chart.AutoViewPort();

            return chart;
        }
        public static Image FeaturesScatterPlot_Thumbnail(List<UMCLight> features, ChartDisplayOptions options)
        {
            Image image = null;

            using (ctlChartBase chart = FeaturesScatterPlot_Chart(features, options))
            {
                if (chart == null)
                    return null;
                image = CreateThumbnailFromChart(chart, options);
            }
            return image;
        }
        /// <summary>
        /// Creates a scan width chart for a set of clusters.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static ctlScatterChart FeaturesScatterPlot_Chart(List<UMCLight> features, ChartDisplayOptions options)
        {
            if (features == null)
                return null;

            if (features.Count < 1)
                return null;

            ctlScatterChart chart = new ctlScatterChart();
            chart.XAxisLabel = options.XAxisLabel;
            chart.YAxisLabel = options.YAxisLabel;
            chart.Title = options.Title;

            chart.Margins.LeftMarginMin = options.MarginMin;
            chart.Margins.LeftMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMin = options.MarginMin;

            chart.LegendVisible = options.DisplayLegend;
            chart.AxisVisible = options.DisplayAxis;
            chart.TitleVisible = options.DisplayTitle;
            chart.XAxisGridLines = options.DisplayGridLines;
            chart.YAxisGridLines = options.DisplayGridLines;


            int i = 0;
            Dictionary<int, List<UMCLight>> chargeMaps = new Dictionary<int, List<UMCLight>>();
            for (i = 0; i < 8; i++)
            {
                chargeMaps.Add(i, new List<UMCLight>());
            }
            chargeMaps.Add(8, new List<UMCLight>());

            
            foreach (UMCLight feature in features)
            {
                int charge = feature.ChargeState;
                if (charge >= 8)
                {
                    chargeMaps[8].Add(feature);
                }
                else
                {
                    chargeMaps[charge].Add(feature);
                }
            }

            clsColorIterator colors = new clsColorIterator();

            foreach(int charge in chargeMaps.Keys)
            {
                List<UMCLight> charges = chargeMaps[charge];

                float[] x = new float[charges.Count];
                float[] y = new float[charges.Count];
                for(i = 0; i < charges.Count; i++)
                {
                    UMCLight feature = charges[i];
                    y[i] = Convert.ToSingle(feature.MassMonoisotopic);
                    x[i] = Convert.ToSingle(feature.Scan);                    
                }
                clsPlotParams parameters = new clsPlotParams(new BubbleShape(1, false), colors.GetColor(charge));                
                parameters.Name          = "Charge " + charge.ToString();
                if (charge == 8)
                {
                    parameters.Name += "+";
                }
                clsSeries series         = new clsSeries(ref x, ref y, parameters);
                chart.AddSeries(series);
            }
            
            chart.AutoViewPort();
            return chart;
        }        
      

        public static Image ClusterSizeNETRange_Thumbnail(int size, List<double> widths, ChartDisplayOptions options)
        {
            Image image = null;

            using (ctlChartBase chart = ClusterSizeNETRange_Chart(size, widths, options))
            {
                if (chart == null)
                    return null;
                image = CreateThumbnailFromChart(chart, options);
            }
            return image;
        }
        /// <summary>
        /// Creates a scan width chart for a set of clusters.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static controlHistogram ClusterSizeNETRange_Chart(int size, List<double> widths, ChartDisplayOptions options)            
        {
            
            if (widths == null)
                return null;

            if (widths.Count < 1)
                return null;

            controlHistogram chart = new controlHistogram();
            chart.XAxisLabel = options.XAxisLabel;
            chart.YAxisLabel = options.YAxisLabel;
            chart.Title = options.Title;

            chart.Margins.LeftMarginMin = options.MarginMin;
            chart.Margins.LeftMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMin = options.MarginMin;

            chart.LegendVisible = options.DisplayLegend;
            chart.AxisVisible = options.DisplayAxis;
            chart.TitleVisible = options.DisplayTitle;
            chart.XAxisGridLines = options.DisplayGridLines;
            chart.YAxisGridLines = options.DisplayGridLines;

            widths.Sort();
            double max    = widths[widths.Count - 1];
            double min    = widths[0];
            double dWidth = (max - min) / 50; 
            
            float[] x = new float[51];
            float[] y = new float[51];

            // Edge case, if we have one value for the histogram then
            // this wont work.
            if (widths.Count == 1)
            {
                dWidth = widths[0] * .1;
                min = widths[0] - (dWidth * 25);
                for (int i = 0; i < 50; i++)
                {
                    x[i] = Convert.ToSingle((i * dWidth) + min);
                }
                 int index = Convert.ToInt32((widths[0] - min) / dWidth);
                 index = Math.Min(49, Math.Max(0, index));
                 y[index]++;                
            }
            else
            {
                for (int i = 0; i < 51; i++)
                {
                    x[i] = Convert.ToSingle((i * dWidth) + min);
                }
                for (int i = 0; i < widths.Count; i++)
                {
                    int index = 0;
                    if (dWidth != 0)
                    {
                        index = Convert.ToInt32((widths[i] - min)/ dWidth);
                    }
                    index = Math.Min(49, Math.Max(0, index));
                    y[index]++;
                }
            }
            

            clsPlotParams parameters = new clsPlotParams(new BubbleShape(1, false), Color.Red);
            clsSeries series = new clsSeries(ref x, ref y, parameters);
            chart.BinSize = Convert.ToSingle(dWidth);
            chart.AddSeries(series);
            chart.AutoViewPort();
            
            return chart;
        }                
        /// <summary>
        /// Creates a scan width chart for a set of clusters.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static ctlScatterChart ClusterNetRangeVsScore_Chart(List<double> ranges, List<double> scores, ChartDisplayOptions options)
        {

            ctlScatterChart chart = new ctlScatterChart();
            chart.XAxisLabel = options.XAxisLabel;
            chart.YAxisLabel = options.YAxisLabel;
            chart.Title = options.Title;

            chart.Margins.LeftMarginMin = options.MarginMin;
            chart.Margins.LeftMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMax = options.MarginMax;
            chart.Margins.BottomMarginMin = options.MarginMin;

            chart.LegendVisible     = options.DisplayLegend;
            chart.AxisVisible       = options.DisplayAxis;
            chart.TitleVisible      = options.DisplayTitle;
            chart.XAxisGridLines    = options.DisplayGridLines;
            chart.YAxisGridLines    = options.DisplayGridLines;

            float[] x = new float[scores.Count];
            float[] y = new float[scores.Count];

            for(int i = 0; i < scores.Count; i++)
            {
                x[i] = Convert.ToSingle(ranges[i]);
                y[i] = Convert.ToSingle(scores[i]);
                i++;
            }
            clsPlotParams parameters = new clsPlotParams(new BubbleShape(1, false), Color.Black);
            clsSeries series = new clsSeries(ref x, ref y, parameters);

            chart.AddSeries(series);
            chart.AutoViewPort();

            return chart;
        }        
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart NETResiduals_Chart(classAlignmentResidualData residual)
        {
            ctlScatterChart chart = null;

            // Set the data for the chart.          
            chart                           = new ctlScatterChart();
            chart.XAxisLabel                = "Scan #";
            chart.YAxisLabel                = "NET Residual";
            chart.PadViewPortX = .1F;
            chart.PadViewPortY = .1F;

            clsShape alignedShape           = new BubbleShape(CONST_POST_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name          = "Net Error";
            chart.AutoViewPortOnAddition    = true;
            clsSeries seriesAligned         = new clsSeries(ref residual.scans, ref residual.customNet, plt_paramsAligned);
            chart.AddSeries(seriesAligned);            
            chart.ViewPortHistory.Clear();

            return chart;
        }
        #endregion

        #region Mass and NET 2D residual plot 
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image MassNETResiduals_Thumbnail(classAlignmentResidualData residual, ChartDisplayOptions options)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.MassNETResiduals_Chart(residual);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = options.MarginMin;
                    chart.Margins.LeftMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMin = options.MarginMin;


                    chart.Title = options.Title;
                    chart.LegendVisible     = options.DisplayLegend;
                    chart.AxisVisible       = options.DisplayAxis;
                    chart.TitleVisible      = options.DisplayTitle;
                    chart.XAxisGridLines    = options.DisplayGridLines;
                    chart.YAxisGridLines    = options.DisplayGridLines;
                    image               = chart.ToBitmap(options.Width, options.Height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        public static ctlScatterChart MassNETResiduals_Chart(classAlignmentResidualData residual)
        {
            ctlScatterChart chart = null;
            chart               = new ctlScatterChart();
            chart.YAxisLabel    = "Mass Residuals (PPM)";
            chart.XAxisLabel    = "NET Residuals (%)";
            chart.PadViewPortX  = .1F;
            chart.PadViewPortY  = .1F;

            clsShape alignedShape               = new BubbleShape(CONST_PRE_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned     = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name              = "Feature Match";
            chart.AutoViewPortOnAddition        = true;
            clsSeries seriesAligned             = new clsSeries(ref residual.customNet, ref residual.massError, plt_paramsAligned);
            chart.AddSeries(seriesAligned);
            chart.ViewPortHistory.Clear();

            return chart;
        }
        #endregion

        #region Mass Vs Scan Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart MassVsScanResiduals_Chart(classAlignmentResidualData residual)
        {
            ctlScatterChart chart = null;
            
            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            chart               = new ctlScatterChart();
            chart.XAxisLabel    = "Scan #";
            chart.YAxisLabel    = "Mass Residual (PPM)";
            chart.PadViewPortX  = .1F;
            chart.PadViewPortY  = .1F;
            int ptSize          = CONST_PRE_POINT_SIZE;
            Color clr           = Color.Blue;
            clsShape shape      = new BubbleShape(ptSize, false);

            /// 
            /// Residual Plots of mass error pre-corrected
            /// 
            clsPlotParams plt_params = new clsPlotParams(shape, Color.Blue);
            plt_params.Name = "Post-Alignment";
            chart.AutoViewPortOnAddition = true;

            clsSeries series = new clsSeries(ref residual.scans, ref residual.massError, plt_params);
            

            /// 
            /// Residual Plots of mass error post-corrected?
            /// 
            clsShape alignedShape           = new BubbleShape(CONST_POST_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name          = "Pre-Alignment";
            chart.AutoViewPortOnAddition    = true;

            clsSeries seriesCorrected = new clsSeries(ref residual.scans, ref residual.massErrorCorrected, plt_paramsAligned);
            
            chart.ViewPortHistory.Clear();            
            chart.AddSeries(seriesCorrected);
            chart.AddSeries(series);


            return chart;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image MassVsScanResiduals_Thumbnail(classAlignmentResidualData residual, ChartDisplayOptions options)                                                        
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.MassVsScanResiduals_Chart(residual);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = options.MarginMin;
                    chart.Margins.LeftMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMin = options.MarginMin;

                    chart.Title = options.Title;
                    chart.LegendVisible     = options.DisplayLegend;
                    chart.AxisVisible       = options.DisplayAxis;
                    chart.TitleVisible      = options.DisplayTitle;
                    chart.XAxisGridLines    = options.DisplayGridLines;
                    chart.YAxisGridLines    = options.DisplayGridLines;

                    image = chart.ToBitmap(options.Width, options.Height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        #endregion

        #region Error Histograms
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ErrorHistogram_Thumbnail(double [,] error, ChartDisplayOptions options)
        {
            if (error == null)
                return null;

            Image image = null;
            try
            {
                                
                controlHistogram chart = RenderDatasetInfo.ErrorHistogram_Chart(error,
                                                                                options);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin     = options.MarginMin;
                    chart.Margins.LeftMarginMax     = options.MarginMax;
                    chart.Margins.BottomMarginMax   = options.MarginMax;
                    chart.Margins.BottomMarginMin   = options.MarginMin;

                    chart.LegendVisible     = options.DisplayLegend;
                    chart.AxisVisible       = options.DisplayAxis;
                    chart.TitleVisible      = options.DisplayTitle;
                    chart.XAxisGridLines    = options.DisplayGridLines;
                    chart.YAxisGridLines    = options.DisplayGridLines;                    
                    image = chart.ToBitmap(options.Width, options.Height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="title"></param>
        /// <param name="xaxisLabel"></param>
        /// <param name="yaxisLabel"></param>
        /// <returns></returns>
        public static controlHistogram ErrorHistogram_Chart(double [,] data,
                                                            ChartDisplayOptions options)
        {

            if (data == null)
                return null;

            controlHistogram chart = null;

            try
            {
                
                float[] bins  = new float[data.GetLength(0)];
                float[] freqs = new float[data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    bins[i] = Convert.ToSingle(data[i, 0]);
                    freqs[i] = Convert.ToSingle(data[i, 1]);
                }
                float diff = bins[1] - bins[0];
                chart = new controlHistogram(bins, freqs, options.Title);
                chart.BinSize = diff;
                chart.XAxisLabel = options.XAxisLabel;
                chart.YAxisLabel = options.YAxisLabel;
                chart.Title      = options.Title;    
            }
            catch
            {
                chart = null;
            }
            return chart;
        }
        #endregion

        #region Mass NET error Histogram
        ///// <summary>
        ///// Renders the scan versus the cluster net to the provided bitmap.
        ///// </summary>
        //public static Image ScanVsClusterNet_Thumbnail(MultiAlignAnalysis analysis,
        //                                            int datasetNum,
        //                                            int width,
        //                                            int height,
        //                                                    bool displayLegend,
        //                                                    bool displayAxis,
        //                                                    bool displayTitle)
        //{

        //    Image image = null;
        //    try
        //    {
        //        ctlScatterChart chart = RenderDatasetInfo.ScanVsClusterNet_Chart(analysis, datasetNum);

        //        if (chart != null)
        //        {
        //            chart.Margins.LeftMarginMin = 1;
        //            chart.Margins.LeftMarginMax = 1;
        //            chart.Margins.BottomMarginMax = 1;
        //            chart.Margins.BottomMarginMin = 1;
        //            chart.LegendVisible = displayLegend;
        //            chart.AxisVisible = displayAxis;
        //            chart.TitleVisible = displayTitle;
        //            chart.XAxisGridLines = false;
        //            chart.YAxisGridLines = false;
        //            image = chart.ToBitmap(width, height);
        //            chart.Dispose();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return image;
        //}
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart MassErrorVsScanResidual_Chart(MultiAlignAnalysis analysis,
                                                    int datasetNum)
        {
            ctlScatterChart chart = null;

            return chart;
        }
   #endregion

        #region Scan vs Net
        ///// <summary>
        ///// Renders the scan versus the cluster net to the provided bitmap.
        ///// </summary>
        //public static ctlScatterChart ScanVsClusterNet_Chart(MultiAlignAnalysis analysis,
        //                                            int datasetNum)
        //{
        //    ctlScatterChart chart = null;
    
        //    //TODO: Add back in.
        //    ///// 
        //    ///// Set the data for the chart. 
        //    ///// Go through each cluster that this dataset was seen in
        //    ///// and plot scan vs net of cluster. 
        //    ///// 
        //    //int numClusters = analysis.UMCData.mobjClusterData.NumClusters;
        //    //int numDatasets = analysis.UMCData.NumDatasets;
            
            
        //    //ArrayList clusterIndices = new ArrayList();
        //    //for (int clusterNum = 0; clusterNum < numClusters; clusterNum++)
        //    //{
        //    //    int umcIndex = analysis.UMCData.mobjClusterData.marrClusterMainMemberIndex[clusterNum * numDatasets + datasetNum];
        //    //    if (umcIndex != -1)
        //    //    {
        //    //        clusterIndices.Add(clusterNum);
        //    //    }
        //    //}

        //    //if (clusterIndices.Count != 0)
        //    //{
        //    //    int numPoints = clusterIndices.Count;
        //    //    float[] scanNums = new float[numPoints];
        //    //    float[] nets = new float[numPoints];
        //    //    for (int index = 0; index < numPoints; index++)
        //    //    {
        //    //        int clusterIndex = (int)clusterIndices[index];
        //    //        int umcIndex = analysis.UMCData.mobjClusterData.marrClusterMainMemberIndex[clusterIndex * numDatasets + datasetNum];
        //    //        scanNums[index] = analysis.UMCData.marr_umcs[umcIndex].mint_scan;
        //    //        nets[index] = (float)analysis.UMCData.mobjClusterData.GetCluster(clusterIndex).mdouble_net;
        //    //    }
        //    //    /// 
        //    //    /// Add data points to the chart.
        //    //    /// 
        //    //    chart = new ctlScatterChart();
        //    //    chart.XAxisLabel = "Scan #";
        //    //    chart.YAxisLabel = "NET";

        //    //    int ptSize = 1;
        //    //    Color clr = Color.Red;
        //    //    clsShape shape = new BubbleShape(ptSize, false); ;
        //    //    clsPlotParams plt_params = new clsPlotParams(shape, clr);
        //    //    plt_params.Name = analysis.UMCData.DatasetName[datasetNum];
        //    //    chart.AutoViewPortOnAddition = true;

        //    //    clsSeries series = new clsSeries(ref scanNums, ref nets, plt_params);
        //    //    chart.AddSeries(series);
        //    //    chart.ViewPortHistory.Clear();               
        //    //}
        //    return chart;
        //}
        #endregion

        #region Heatmap
        /// <summary>
        /// Renders the alignment heatmap
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image AlignmentHeatmap_Thumbnail(classAlignmentData data,                                                    
                                                            int width,
                                                            int height)
        {
            Image image = null;            
            try
            {
                ctlAlignmentHeatMap chart = RenderDatasetInfo.AlignmentHeatMap_Chart(data);
                if (chart != null)
                {
                    chart.Legend.UseZScore  = false;
                    image                   = chart.GetThumbnail(new Size(width, height));
                    chart.Dispose();
                }
            }
            catch 
            {
                return null;
            }
            return image;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="datasetNum"></param>
        /// <returns></returns>
        public static ctlAlignmentHeatMap AlignmentHeatMap_Chart(MultiAlignAnalysis analysis,
                                                    int datasetNum)
        {
            
            classAlignmentData data = analysis.AlignmentData[datasetNum];            
            return AlignmentHeatMap_Chart(data);
        }   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ctlAlignmentHeatMap AlignmentHeatMap_Chart(classAlignmentData data)
        {

            if (data == null)
                return null;

            ctlAlignmentHeatMap     heatMap = new ctlAlignmentHeatMap();
            clsAlignmentFunction    alignmentFnc;
            int     minAligneeScan,     maxAligneeScan;
            float   minBaselineScan,    maxBaselineScan;
            float[,] mScores;

            try
            {                
                alignmentFnc    = data.alignmentFunction;
                mScores         = data.heatScores;
                minBaselineScan = data.minMTDBNET;
                maxBaselineScan = data.maxMTDBNET;
                minAligneeScan  = data.minScanBaseline;
                maxAligneeScan  = data.maxScanBaseline;

                heatMap.SetData(mScores,
                                            new PNNLControls.ctlHierarchalLabel.AxisRangeF(minAligneeScan, maxAligneeScan),
                                            new PNNLControls.ctlHierarchalLabel.AxisRangeF(minBaselineScan, maxBaselineScan));
                heatMap.AlignmentFunction = alignmentFnc;
            }
            catch
            {
                return null;
            }
            return heatMap;
        }
        #endregion


        #region Clusters 

        #region NET Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ClusterNETResiduals_Thumbnail(MultiAlignAnalysis analysis,
                                                    int width,
                                                    int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterNETResiduals_Chart(analysis);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = displayLegend;
                    chart.AxisVisible = displayAxis;
                    chart.TitleVisible = displayTitle;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart ClusterNETResiduals_Chart(MultiAlignAnalysis analysis)
        {
            ctlScatterChart chart = null;

            if (analysis.ClusterAlignmentData == null)
                return null;

            /// 
            /// Set the data for the chart.          
            /// 
            classAlignmentResidualData residual = analysis.ClusterAlignmentData.ResidualData;
            chart = new ctlScatterChart();
            chart.XAxisLabel = "Scan #";
            chart.YAxisLabel = "NET Residual";
            chart.Title = "Cluster NET Residuals ";
            chart.PadViewPortX = .1F;
            chart.PadViewPortY = .1F;

            clsShape alignedShape = new BubbleShape(CONST_POST_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name = "Net Error";
            chart.AutoViewPortOnAddition = true;
            clsSeries seriesAligned = new clsSeries(ref residual.scans, ref residual.customNet, plt_paramsAligned);
            chart.AddSeries(seriesAligned);
            chart.ViewPortHistory.Clear();

            return chart;
        }
        #endregion

        #region Mass and NET 2D residual plot
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ClusterMassNETResiduals_Thumbnail(MultiAlignAnalysis analysis,                                                    
                                                    int width,
                                                    int height,
                                                    bool displayLegend,
                                                    bool displayAxis,
                                                    bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterMassNETResiduals_Chart(analysis);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = displayLegend;
                    chart.AxisVisible = displayAxis;
                    chart.TitleVisible = displayTitle;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        /// <summary>
        /// Creates a cluster mass vs net residual chart.
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static ctlScatterChart ClusterMassNETResiduals_Chart(MultiAlignAnalysis analysis)
        {
            ctlScatterChart chart = null;

            if (analysis.ClusterAlignmentData == null)
                return null;

            /// 
            /// Set the data for the chart.          
            /// 
            classAlignmentResidualData residual = analysis.ClusterAlignmentData.ResidualData;
            chart = new ctlScatterChart();
            chart.YAxisLabel = "Mass Residuals (PPM)";
            chart.XAxisLabel = "NET Residuals (%)";
            chart.Title = "Cluster Mass and NET Residuals Plot";
            chart.PadViewPortX = .1F;
            chart.PadViewPortY = .1F;

            clsShape alignedShape = new BubbleShape(CONST_PRE_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name = "Feature Match";
            chart.AutoViewPortOnAddition = true;
            clsSeries seriesAligned = new clsSeries(ref residual.customNet, ref  residual.massError, plt_paramsAligned);
            chart.AddSeries(seriesAligned);
            chart.ViewPortHistory.Clear();

            return chart;
        }
        #endregion

        #region Mass Vs Scan Residual Plots
 
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ClusterMassVsScanResiduals_Thumbnail(classAlignmentResidualData residual, ChartDisplayOptions options)
        {
            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterMassVsScanResiduals_Chart(residual);

                if (chart != null)
                {
                    //chart.Margins.LeftMarginMin = options.MarginMin;
                    //chart.Margins.LeftMarginMax = options.MarginMax;
                    //chart.Margins.BottomMarginMax = options.MarginMax;
                    //chart.Margins.BottomMarginMin = options.MarginMin;

                    chart.LegendVisible     = options.DisplayLegend;
                    chart.AxisVisible       = options.DisplayAxis;
                    chart.TitleVisible      = options.DisplayTitle;
                    chart.XAxisGridLines    = false;
                    chart.YAxisGridLines    = false;

                    image = chart.ToBitmap(options.Width, options.Height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart ClusterMassVsScanResiduals_Chart(classAlignmentResidualData residual)
        {
            ctlScatterChart chart = null;

            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            chart = new ctlScatterChart();
            chart.XAxisLabel = "Scan #";
            chart.YAxisLabel = "Mass Residual (PPM)";
            chart.Title = "Mass (PPM) vs Scan Residual";
            chart.PadViewPortX = .1F;
            chart.PadViewPortY = .1F;
            int ptSize = CONST_PRE_POINT_SIZE;
            Color clr = Color.Blue;
            clsShape shape = new BubbleShape(ptSize, false);

            /// 
            /// Residual Plots of mass error pre-corrected
            /// 
            clsPlotParams plt_params = new clsPlotParams(shape, Color.Blue);
            plt_params.Name = "Post-Alignment";
            chart.AutoViewPortOnAddition = true;
            clsSeries series = new clsSeries(ref residual.scans, ref residual.massError, plt_params);


            /// 
            /// Residual Plots of mass error post-corrected?
            /// 
            clsShape alignedShape = new BubbleShape(CONST_POST_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);            
            plt_paramsAligned.Name = "Pre-Alignment";
            chart.AutoViewPortOnAddition = true;
            clsSeries seriesCorrected = new clsSeries(ref residual.scans, ref residual.massErrorCorrected, plt_paramsAligned);
            chart.ViewPortHistory.Clear();

            chart.AddSeries(seriesCorrected);
            chart.AddSeries(series);


            return chart;
        }

        #endregion

        #region Mass Vs Mz Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart ClusterMassVsMZResidual_Chart(classAlignmentResidualData residual)
        {
            ctlScatterChart chart = null;
            
            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            chart = new ctlScatterChart();
            chart.XAxisLabel = "m/z";
            chart.YAxisLabel = "Mass Residual (PPM)";
            chart.PadViewPortX = .1F;
            chart.PadViewPortY = .1F;
            int ptSize = CONST_PRE_POINT_SIZE;
            Color clr = Color.FromArgb(255, Color.Blue);
            clsShape shape = new BubbleShape(ptSize, false);

            /// 
            /// Residual Plots of mass vs mz error pre-corrected
            /// 
            clsPlotParams plt_params = new clsPlotParams(shape, Color.Blue);
            plt_params.Name = "Post-Alignment";
            chart.AutoViewPortOnAddition = true;
            clsSeries series = new clsSeries(ref residual.mz, ref residual.mzMassError, plt_params);


            /// 
            /// Residual Plots of mass vs mz error post-correcteds
            /// 
            clsShape alignedShape = new BubbleShape(CONST_POST_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.FromArgb(128, Color.Red));
            plt_paramsAligned.Name = "Pre-Alignment";
            chart.AutoViewPortOnAddition = true;
            clsSeries seriesCorrected = new clsSeries(ref residual.mz, ref residual.mzMassErrorCorrected, plt_paramsAligned);


            chart.AddSeries(seriesCorrected);
            chart.AddSeries(series);
            chart.ViewPortHistory.Clear();

            return chart;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ClusterMassVsMZResidual_Thumbnail(classAlignmentResidualData residuals, ChartDisplayOptions options)            
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterMassVsMZResidual_Chart(residuals);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = options.MarginMin;
                    chart.Margins.LeftMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMin = options.MarginMin;


                    chart.Title = options.Title;
                    chart.LegendVisible     = options.DisplayLegend;
                    chart.AxisVisible       = options.DisplayAxis;
                    chart.TitleVisible      = options.DisplayTitle;
                    chart.XAxisGridLines    = false;
                    chart.YAxisGridLines    = false;

                    image = chart.ToBitmap(options.Width, options.Height);

                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        #endregion

        #region Error Histograms
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ClusterMassErrorHistogram_Thumbnail(MultiAlignAnalysis analysis,                                                            
                                                            int width,
                                                            int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                controlHistogram chart = RenderDatasetInfo.ClusterMassErrorHistogram_Chart(analysis);
                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = displayLegend;
                    chart.AxisVisible = displayAxis;
                    chart.TitleVisible = displayTitle;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static controlHistogram ClusterMassErrorHistogram_Chart(MultiAlignAnalysis analysis)
        {
            controlHistogram chart = null;

            try
            {
                if (analysis.ClusterAlignmentData == null)
                    return null;

                double[,] massError = analysis.ClusterAlignmentData.massErrorHistogram;
                
                /// 
                /// Element by element copies take a lot of time but these arrays are not that big
                /// So we take the perf hit here and just copy this that way.  Improvement could
                /// be made to speed this up with a better block copy or changing the histogram
                /// code in MAEngine to return float arrays.  However, double is better precision
                /// and we'll take that benefit over this one.
                /// 
                float[] bins = new float[massError.GetLength(0)];
                float[] freqs = new float[massError.GetLength(0)];
                for (int i = 0; i < massError.GetLength(0); i++)
                {
                    bins[i] = Convert.ToSingle(massError[i, 0]);
                    freqs[i] = Convert.ToSingle(massError[i, 1]);
                }

                float diff = bins[1] - bins[0];
                chart = new controlHistogram(bins, freqs, "Mass Error Histogram (PPM) ");
                chart.BinSize = diff;
                chart.XAxisLabel = "Mass Error (PPM)";
                chart.YAxisLabel = "Count";
            }
            catch
            {
                chart = null;
            }
            return chart;
        }

        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ClusterNETErrorHistogram_Thumbnail(MultiAlignAnalysis analysis,
                                                            int width,
                                                            int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle
                                                            )
        {

            Image image = null;
            try
            {                
                controlHistogram chart = RenderDatasetInfo.ClusterNETErrorHistogram_Chart(analysis);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = false;
                    chart.AxisVisible = false;
                    chart.TitleVisible = false;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static controlHistogram ClusterNETErrorHistogram_Chart(MultiAlignAnalysis analysis)
        {
            controlHistogram chart = null;

            try
            {

                double[,] NETError = analysis.ClusterAlignmentData.netErrorHistogram;
                

                /// 
                /// Element by element copies take a lot of time but these arrays are not that big
                /// So we take the perf hit here and just copy this that way.  Improvement could
                /// be made to speed this up with a better block copy or changing the histogram
                /// code in MAEngine to return float arrays.  However, double is better precision
                /// and we'll take that benefit over this one.
                /// 
                float[] bins = new float[NETError.GetLength(0)];
                float[] freqs = new float[NETError.GetLength(0)];
                for (int i = 0; i < NETError.GetLength(0); i++)
                {
                    bins[i] = Convert.ToSingle(NETError[i, 0]);
                    freqs[i] = Convert.ToSingle(NETError[i, 1]);
                }
                float diff = bins[1] - bins[0];
                chart = new controlHistogram(bins, freqs, "NET Error Histogram ");
                chart.BinSize = diff;
                chart.XAxisLabel = "NET Error (%)";
                chart.YAxisLabel = "Count";
            }
            catch
            {
                chart = null;
            }
            return chart;
        }
        #endregion

        #region Mass NET error Histogram
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart ClusterMassErrorVsScanResidual_Chart(MultiAlignAnalysis analysis)                                                    
        {
            ctlScatterChart chart = null;

            return chart;
        }
        #endregion
     
        #region Heatmap
        /// <summary>
        /// Renders the alignment heatmap
        /// </summary>
        public static Image ClusterAlignmentHeatmap_Thumbnail(MultiAlignAnalysis analysis,                                                    
                                                    int width,
                                                    int height)
        {
            Image image = null;
            try
            {
                ctlAlignmentHeatMap chart = RenderDatasetInfo.ClusterAlignmentHeatMap_Chart(analysis);
                if (chart != null)
                {
                    image = chart.GetThumbnail(new Size(width, height));
                    chart.Dispose();
                }
            }
            catch
            {
                return null;
            }
            return image;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlAlignmentHeatMap ClusterAlignmentHeatMap_Chart(MultiAlignAnalysis analysis)
        {
            ctlAlignmentHeatMap heatMap = new ctlAlignmentHeatMap();
            clsAlignmentFunction alignmentFnc;
            string datasetName;
            int minAligneeScan, maxAligneeScan;
            float minBaselineScan, maxBaselineScan;
            float[,] mScores;

            try
            {
                classAlignmentData data = analysis.ClusterAlignmentData;
                if (data == null)
                    return null;

                alignmentFnc = data.alignmentFunction;
                datasetName = data.aligneeDataset;
                mScores = data.heatScores;
                minBaselineScan = data.minMTDBNET;
                maxBaselineScan = data.maxMTDBNET;
                minAligneeScan = data.minScanBaseline;
                maxAligneeScan = data.maxScanBaseline;


                heatMap.SetData(mScores,
                                            new PNNLControls.ctlHierarchalLabel.AxisRangeF(minAligneeScan, maxAligneeScan),
                                            new PNNLControls.ctlHierarchalLabel.AxisRangeF(minBaselineScan, maxBaselineScan));
                heatMap.AlignmentFunction = alignmentFnc;
            }
            catch
            {
                return null;
            }
            return heatMap;
        }
        #endregion
        #endregion

        #region Cluster Score Plots
        
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ClusterSizeHistogram_Thumbnail(List<UMCClusterLight> clusters,
                                                            int width,
                                                            int height,
                                                            ChartDisplayOptions options
                                                            )
        {

            Image image = null;
            try
            {
                GenericHistogram chart = RenderDatasetInfo.ClusterSizeHistogram_Chart(clusters, options);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin     = options.MarginMin;
                    chart.Margins.LeftMarginMax     = options.MarginMax;
                    chart.Margins.BottomMarginMax   = options.MarginMax;
                    chart.Margins.BottomMarginMin   = options.MarginMin;

                    chart.LegendVisible     = options.DisplayLegend;
                    chart.AxisVisible       = options.DisplayAxis;
                    chart.TitleVisible      = options.DisplayTitle;
                    chart.XAxisGridLines    = options.DisplayGridLines;
                    chart.YAxisGridLines    = options.DisplayGridLines;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }

        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ClusterDatasetMemberSizeHistogram_Thumbnail(List<UMCClusterLight> clusters,
                                                            int width,
                                                            int height,
                                                            ChartDisplayOptions options
                                                            )
        {

            Image image = null;
            try
            {
                GenericHistogram chart = RenderDatasetInfo.ClusterDatasetMemberSizeHistogram_Chart(clusters, options);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = options.MarginMin;
                    chart.Margins.LeftMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMin = options.MarginMin;

                    chart.LegendVisible = options.DisplayLegend;
                    chart.AxisVisible = options.DisplayAxis;
                    chart.TitleVisible = options.DisplayTitle;
                    chart.XAxisGridLines = options.DisplayGridLines;
                    chart.YAxisGridLines = options.DisplayGridLines;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static GenericHistogram ClusterSizeHistogram_Chart(List<UMCClusterLight> clusters, ChartDisplayOptions options)
        {
            Dictionary<int, int> map    = clusters.CreateClusterSizeHistogram();            
            GenericHistogram histogram  = new GenericHistogram();            
            histogram.BinSize           = 1.0F;
            histogram.XAxisLabel        = options.YAxisLabel;
            histogram.YAxisLabel        = options.XAxisLabel;
            histogram.Title             = options.Title;
            histogram.ConstructHistogram(map);
            histogram.AutoViewPort();
            histogram.Refresh();
            return histogram;                          
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public static GenericHistogram ClusterDatasetMemberSizeHistogram_Chart(List<UMCClusterLight> clusters, ChartDisplayOptions options)
        {
            Dictionary<int, int> map = clusters.CreateClusterDatasetMemeberSizeHistogram();            
            GenericHistogram histogram  = new GenericHistogram();            
            histogram.BinSize           = 1.0F;
            histogram.XAxisLabel        = options.YAxisLabel;
            histogram.YAxisLabel        = options.XAxisLabel;
            histogram.Title             = options.Title;
            histogram.ConstructHistogram(map);
            histogram.AutoViewPort();
            histogram.Refresh();
            return histogram;
        }
        
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image CreateHistogram_Thumbnail(Dictionary<int, int> map ,
                                                            int width,
                                                            int height,
                                                            ChartDisplayOptions options
                                                            )
        {
            Image image = null;
            try
            {
                GenericHistogram chart = RenderDatasetInfo.CreateHistogram_Chart(map, options);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = options.MarginMin;
                    chart.Margins.LeftMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMax = options.MarginMax;
                    chart.Margins.BottomMarginMin = options.MarginMin;

                    chart.LegendVisible = options.DisplayLegend;
                    chart.AxisVisible = options.DisplayAxis;
                    chart.TitleVisible = options.DisplayTitle;
                    chart.XAxisGridLines = options.DisplayGridLines;
                    chart.YAxisGridLines = options.DisplayGridLines;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;

        }
        public static GenericHistogram CreateHistogram_Chart(Dictionary<int, int> map,
                                                                                                    ChartDisplayOptions options)
        {                      
            GenericHistogram histogram  = new GenericHistogram();            
            histogram.BinSize           = 1.0F;
            histogram.XAxisLabel        = options.YAxisLabel;
            histogram.YAxisLabel        = options.XAxisLabel;
            histogram.Title             = options.Title;
            histogram.ConstructHistogram(map);
            histogram.AutoViewPort();
            histogram.Refresh();
            return histogram;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ChargeStateHistogram_Thumbnail(List<UMCLight> umcs,
                                                            int width,
                                                            int height,
                                                            ChartDisplayOptions options
                                                            )
        {

            Image image = null;
            try
            {
                controlHistogram chart = RenderDatasetInfo.ChargeStateHistogram_Chart(umcs);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin     = options.MarginMin;
                    chart.Margins.LeftMarginMax     = options.MarginMax;
                    chart.Margins.BottomMarginMax   = options.MarginMax;
                    chart.Margins.BottomMarginMin   = options.MarginMin;
                    chart.LegendVisible             = options.DisplayLegend;
                    chart.AxisVisible               = options.DisplayAxis;
                    chart.TitleVisible              = options.DisplayTitle;
                    chart.XAxisGridLines            = options.DisplayGridLines;
                    chart.YAxisGridLines            = options.DisplayGridLines;
                    chart.Title                     = options.Title;
                    image                           = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        /// <summary>
        /// Creates a chart based on the cluster data.
        /// </summary>
        /// <param name="clusterCache"></param>
        /// <returns></returns>
        public static controlHistogram ChargeStateHistogram_Chart(List<UMCLight> umcs)
        {
            /// 
            /// Find all of the charge states 
            /// 
            int N = 31;
            float[] charges = new float[N];
            for (int i = 0; i < N; i++)
            {
                charges[i] = 0;
            }
            int maxCharge = 0;

            foreach (UMCLight umc in umcs)
            {                
                int j = umc.ChargeState;
                if (j > N - 1)
                    j = N - 1;
                maxCharge = Math.Max(maxCharge, j);
                if (j < 1)
                    j = 1;
                charges[j - 1]++;
            }            

            /// 
            /// Construct histogram
            /// 
            float[] bins = new float[maxCharge];
            float[] freqs = new float[maxCharge];
            for (int i = 0; i < maxCharge; i++)
            {
                bins[i]  = Convert.ToSingle(i + 1);
                freqs[i] = charges[i];
            }

            /// 
            /// Display histogram 
            /// 
            controlHistogram chart = new controlHistogram();
            
            chart.BinSize = 1.0F;
            chart.AddData(bins, freqs, "Charge States");
            chart.XAxisLabel = "Charge States";
            chart.YAxisLabel = "Count";
            //chart.ViewPort   = new RectangleF(0.5F, 0.0F, Convert.ToSingle(maxCharge), chart.ViewPort.Height);
            return chart;
        }        
        #endregion
        
        #region Peak Matching Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image PeakMatchMassNET_Thumbnail(MultiAlignAnalysis analysis,int width, int height, bool displayLegend, bool displayAxis, bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = null;
                //TODO: Add back in
                //RenderDatasetInfo.PeakMatchMassNET_Chart(analysis);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = false;
                    chart.AxisVisible = false;
                    chart.TitleVisible = false;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }                
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image SMARTScoreHistogram_Thumbnail(MultiAlignAnalysis analysis,int width, int height, bool displayLegend, bool displayAxis, bool displayTitle)
        {

            Image image = null;
            try
            {
                controlHistogram chart = null;//
                //TODO: Add back in 
                //RenderDatasetInfo.SMARTScoreHistogram_Chart(analysis);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = false;
                    chart.AxisVisible = false;
                    chart.TitleVisible = false;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;

                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        #endregion
    }


    /// <summary>
    /// Class that has generic dataset plot tools.
    /// </summary>
    public class GenericPlotFactory
    {
        private const int CONST_PRE_POINT_SIZE  = 1;
        private const int CONST_POST_POINT_SIZE = 1;

        #region Scatter Plots
        public static controlHistogram ResidualHistogram_Chart(float[] x,
                                                              float[] y,
                                                              ChartDisplayOptions options)
        {
            controlHistogram chart = new controlHistogram();

            float maxDiff       = float.MinValue;
            float minDiff       = float.MaxValue;
            List<float> diffs   = new List<float>();
            for (int i = 0; i < x.Length; i++)
            {
                float diff  = x[i] - y[i];
                minDiff     = Math.Min(minDiff, diff);
                maxDiff     = Math.Max(maxDiff, diff);
                diffs.Add(diff);
            }
            
            int N           = 100;
            float[] bins    = new float[N];
            float[] freqs   = new float[N];
            float dx        = (maxDiff - minDiff) / Convert.ToSingle(N);            

            for(int i = 0; i < N; i++)
            {
                bins[i]  = minDiff + (i * dx);
                freqs[i] = 0;
            }

            foreach (float diff in diffs)
            {
                int index       = Convert.ToInt32((diff - minDiff) / dx);
                index           = Math.Max(0, Math.Min(N - 1, index));
                freqs[index]++;
            }

            int ptSize                   = CONST_PRE_POINT_SIZE;
            clsShape shape               = new BubbleShape(ptSize, false);
            clsPlotParams plt_params     = new clsPlotParams(shape, Color.Red);
            chart.AutoViewPortOnAddition = true;
            clsSeries series             = new clsSeries(ref bins, ref freqs, plt_params);
            chart.BinSize                = dx;
            chart.AddSeries(series);
            chart.ViewPortHistory.Clear();
            return chart;
        }
        public static Image ResidualHistogram_Thumbnail(float[] x,
                                                        float[] y,
                                                        ChartDisplayOptions options)
        {

            Image image = null;
            try
            {
                controlHistogram chart = GenericPlotFactory.ResidualHistogram_Chart(x, y, options);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin     = options.MarginMin;
                    chart.Margins.LeftMarginMax     = options.MarginMax;
                    chart.Margins.BottomMarginMax   = options.MarginMax;
                    chart.Margins.BottomMarginMin   = options.MarginMin;
                    chart.LegendVisible             = options.DisplayLegend;
                    chart.AxisVisible               = options.DisplayAxis;
                    chart.TitleVisible              = options.DisplayTitle;
                    chart.XAxisLabel                = options.XAxisLabel;
                    chart.YAxisLabel                = options.YAxisLabel;
                    chart.Title                     = options.Title;
                    image                           = chart.ToBitmap(options.Width, options.Height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        #endregion

        #region Scatter Plots
        public static ctlScatterChart ScatterPlot_Chart(float[] x,
                                                        float[] y,
                                                        ChartDisplayOptions options)
        {
            ctlScatterChart chart   = null;
            chart                   = new ctlScatterChart();
            
                
            int ptSize                      = CONST_PRE_POINT_SIZE;
            Color clr                       = Color.FromArgb(255, Color.Black);
            clsShape shape                  = new BubbleShape(ptSize, false);

            clsPlotParams plt_params        = new clsPlotParams(shape, Color.Blue);
            chart.AutoViewPortOnAddition    = true;
            clsSeries series                = new clsSeries(ref x, ref y, plt_params);       
    
            chart.AddSeries(series);
            chart.ViewPortHistory.Clear();
            return chart;
        }
        public static Image ScatterPlot_Thumbnail(float[] x,
                                                  float[] y,
                                                  ChartDisplayOptions options)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = GenericPlotFactory.ScatterPlot_Chart(x, y, options);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin     = options.MarginMin;
                    chart.Margins.LeftMarginMax     = options.MarginMax;
                    chart.Margins.BottomMarginMax   = options.MarginMax;
                    chart.Margins.BottomMarginMin   = options.MarginMin;
                    chart.LegendVisible             = options.DisplayLegend;
                    chart.AxisVisible               = options.DisplayAxis;
                    chart.TitleVisible              = options.DisplayTitle;
                    chart.XAxisLabel                = options.XAxisLabel;
                    chart.YAxisLabel                = options.YAxisLabel;
                    chart.Title                     = options.Title;
                    image = chart.ToBitmap(options.Width, options.Height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        #endregion

        #region Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart Residual_Chart(   float [] x, 
                                                        float [] error,
                                                        float [] corrected,
                                                        bool invert,
                                                        ChartDisplayOptions options)
        {
            ctlScatterChart chart = null;
            
            // Set the data for the chart. Go through each cluster that this dataset was seen in
            // and plot scan vs net of cluster. 
            chart               = new ctlScatterChart();
            chart.XAxisLabel    = options.XAxisLabel;
            chart.YAxisLabel    = options.YAxisLabel;
            chart.Title         = options.Title;
            chart.PadViewPortX  = .1F;
            chart.PadViewPortY  = .1F;

            int ptSize          = CONST_PRE_POINT_SIZE;
            Color clr           = Color.FromArgb(255, Color.Blue);
            clsShape shape      = new BubbleShape(ptSize, false);

            // Residual Plots of mass vs mz error pre-corrected
            clsPlotParams plt_params     = new clsPlotParams(shape, Color.Blue);
            plt_params.Name              = "Post-Alignment";
            chart.AutoViewPortOnAddition = true;            
            clsSeries series             = new clsSeries(ref x, ref error, plt_params);

            if (corrected != null)
            {
                // Residual Plots of mass vs mz error post-correcteds             
                clsShape alignedShape = new BubbleShape(CONST_POST_POINT_SIZE, false);
                clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
                plt_paramsAligned.Name = "Pre-Alignment";
                chart.AutoViewPortOnAddition = true;

                float[] data = corrected;
                if (invert)
                {
                    data = new float[corrected.Length];
                    for (int kk = 0; kk < data.Length; kk++)
                        data[kk] = corrected[kk] * -1;
                }

                clsSeries seriesCorrected = new clsSeries(ref x, ref corrected, plt_paramsAligned);
                chart.AddSeries(seriesCorrected);
            }
            chart.AddSeries(series);
            chart.ViewPortHistory.Clear();
            return chart;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image Residual_Thumbnail(float[] x,
                                                       float[] error,
                                                       float[] corrected,
                                                       bool invert,
                                                       ChartDisplayOptions options)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = GenericPlotFactory.Residual_Chart(x, error, corrected, invert, options);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin     = 1;
                    chart.Margins.LeftMarginMax     = 1;
                    chart.Margins.BottomMarginMax   = 1;
                    chart.Margins.BottomMarginMin   = 1;
                    chart.LegendVisible = options.DisplayLegend;
                    chart.AxisVisible   = options.DisplayAxis;
                    chart.TitleVisible  = options.DisplayTitle;
                    image               = chart.ToBitmap(options.Width, options.Height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        #endregion
    }
}
