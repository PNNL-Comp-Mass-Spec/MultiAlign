using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using PNNLControls;

using MultiAlignEngine;

using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.PeakMatching;

using PNNLProteomics.SMART;
using PNNLProteomics.Data.Analysis;
using PNNLProteomics.Data.Alignment;

using MultiAlign.Charting;

namespace MultiAlign.Drawing
{
    /// <summary>
    /// Factory class for rendering information about a dataset info class.
    /// </summary>
    public static class RenderDatasetInfo
    {
        private const int CONST_PRE_POINT_SIZE  = 1;
        private const int CONST_POST_POINT_SIZE = 1;


        #region NET Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image NETResiduals_Thumbnail( MultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.NETResiduals_Chart(analysis, datasetNum);

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
        public static ctlScatterChart NETResiduals_Chart(MultiAlignAnalysis analysis, int datasetNum)
        {
            ctlScatterChart chart = null;

            if (analysis.AlignmentData.Count < datasetNum)
                return null;
            if (analysis.AlignmentData[datasetNum] == null)
                return null;

            /// 
            /// Set the data for the chart.          
            /// 
            classAlignmentResidualData residual = analysis.AlignmentData[datasetNum].ResidualData;
            chart                           = new ctlScatterChart();
            chart.XAxisLabel                = "Scan #";
            chart.YAxisLabel                = "NET Residual";
            chart.Title                     = "NET Residuals " + analysis.UMCData.DatasetName[datasetNum];
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
        public static Image MassNETResiduals_Thumbnail(MultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height,
                                                    bool displayLegend,
                                                    bool displayAxis,
                                                    bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.MassNETResiduals_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin   = 1;
                    chart.Margins.LeftMarginMax   = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = displayLegend;
                    chart.AxisVisible   = displayAxis;
                    chart.TitleVisible = displayTitle;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;

                    image               = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch
            {
            }
            return image;
        }
        public static ctlScatterChart MassNETResiduals_Chart(MultiAlignAnalysis analysis, int datasetNum)
        {
            ctlScatterChart chart = null;

            if (analysis.AlignmentData.Count < datasetNum)
                return null;
            if (analysis.AlignmentData[datasetNum] == null) 
                return null;

            /// 
            /// Set the data for the chart.          
            /// 
            classAlignmentResidualData residual = analysis.AlignmentData[datasetNum].ResidualData;
            chart               = new ctlScatterChart();
            chart.YAxisLabel    = "Mass Residuals (PPM)";
            chart.XAxisLabel    = "NET Residuals (%)";
            chart.Title = "Mass and NET Residuals Plot" + analysis.UMCData.DatasetName[datasetNum];
            chart.PadViewPortX = .1F;
            chart.PadViewPortY = .1F;

            clsShape alignedShape           = new BubbleShape(CONST_PRE_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name          = "Feature Match";
            chart.AutoViewPortOnAddition    = true;
            clsSeries seriesAligned         = new clsSeries(ref residual.customNet, ref residual.massError, plt_paramsAligned);
            chart.AddSeries(seriesAligned);
            chart.ViewPortHistory.Clear();

            return chart;
        }
        #endregion

        #region Mass Vs Scan Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart MassVsScanResiduals_Chart(MultiAlignAnalysis analysis, int datasetNum)
        {
            ctlScatterChart chart = null;

            if (analysis.AlignmentData.Count < datasetNum)
                return null;

            if (analysis.AlignmentData[datasetNum] == null)
                return null;

            string name = analysis.Datasets[datasetNum].DatasetName;
            if (name == null)
                name = "";

            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            classAlignmentResidualData residual = analysis.AlignmentData[datasetNum].ResidualData;
            chart               = new ctlScatterChart();
            chart.XAxisLabel    = "Scan #";
            chart.YAxisLabel    = "Mass Residual (PPM)";
            chart.Title         = name + "Mass (PPM) vs Scan Residual";
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
        public static Image MassVsScanResiduals_Thumbnail(MultiAlignAnalysis analysis,
                                                        int datasetNum,
                                                        int width,
                                                        int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.MassVsScanResiduals_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin     = 1;
                    chart.Margins.LeftMarginMax     = 1;
                    chart.Margins.BottomMarginMax   = 1;
                    chart.Margins.BottomMarginMin   = 1;

                    chart.LegendVisible = displayLegend;
                    chart.AxisVisible   = displayAxis;
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
        #endregion

        #region Mass Vs Mz Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart MassVsMZResidual_Chart(MultiAlignAnalysis analysis,
                                                    int datasetNum)
        {
            ctlScatterChart chart = null;

            if (analysis.AlignmentData.Count < datasetNum)
                return null;

            if (analysis.AlignmentData[datasetNum] == null)
                return null;

            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            classAlignmentResidualData residual = analysis.AlignmentData[datasetNum].ResidualData;
            chart = new ctlScatterChart();
            chart.XAxisLabel = "m/z";
            chart.YAxisLabel = "Mass Residual (PPM)";
            chart.Title = analysis.UMCData.DatasetName[datasetNum] + " Mass Residual (PPM) vs. m/z";
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
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name = "Pre-Alignment";
            chart.AutoViewPortOnAddition = true;

            float[] data = new float[residual.mzMassErrorCorrected.Length];
            for (int kk = 0; kk < data.Length; kk++)
                data[kk] = residual.mzMassErrorCorrected[kk] * -1;

            clsSeries seriesCorrected = new clsSeries(ref residual.mz, ref data, plt_paramsAligned); //ref residual.mzMassErrorCorrected, plt_paramsAligned);

            
            chart.AddSeries(seriesCorrected);
            chart.AddSeries(series);
            chart.ViewPortHistory.Clear();

            return chart;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image MassVsMZResidual_Thumbnail(MultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.MassVsMZResidual_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;
                    chart.LegendVisible = displayLegend;
                    chart.AxisVisible = displayAxis;
                    chart.TitleVisible = displayTitle;
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

        #region Error Histograms
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image MassErrorHistogram_Thumbnail(MultiAlignAnalysis analysis,
                                                            int datasetNum,
                                                            int width,
                                                            int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                controlHistogram chart = RenderDatasetInfo.MassErrorHistogram_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin   = 1;
                    chart.Margins.LeftMarginMax   = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = displayLegend;
                    chart.AxisVisible   = displayAxis;
                    chart.TitleVisible = displayTitle;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;

                    image               = chart.ToBitmap(width, height);
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
        public static controlHistogram MassErrorHistogram_Chart(MultiAlignAnalysis analysis,
                                                                int datasetNum)
        {
            controlHistogram chart = null;

            try
            {
                if (analysis.AlignmentData[datasetNum] == null)
                    return null;
                
                double [,] massError    = analysis.AlignmentData[datasetNum].massErrorHistogram;
                string name             = analysis.Datasets[datasetNum].DatasetName;
                /// 
                /// Element by element copies take a lot of time but these arrays are not that big
                /// So we take the perf hit here and just copy this that way.  Improvement could
                /// be made to speed this up with a better block copy or changing the histogram
                /// code in MAEngine to return float arrays.  However, double is better precision
                /// and we'll take that benefit over this one.
                /// 
                float[] bins  = new float[massError.GetLength(0)];
                float[] freqs = new float[massError.GetLength(0)];
                for (int i = 0; i < massError.GetLength(0); i++)
                {
                    bins[i]  = Convert.ToSingle(massError[i, 0]);
                    freqs[i] = Convert.ToSingle(massError[i, 1]);
                }

                float diff = bins[1] - bins[0];                
                chart            = new controlHistogram(bins, freqs, "Mass Error Histogram (PPM) " + name);
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
        public static Image NETErrorHistogram_Thumbnail(MultiAlignAnalysis analysis,
                                                            int datasetNum,
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
                if (analysis.AlignmentData[datasetNum] == null)
                    return null;
                controlHistogram chart = RenderDatasetInfo.NETErrorHistogram_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = false;
                    chart.AxisVisible   = false;
                    chart.TitleVisible  = false;

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
        public static controlHistogram NETErrorHistogram_Chart(MultiAlignAnalysis analysis,
                                                                int datasetNum)
        {
            controlHistogram chart = null;

            try
            {
                if (analysis.AlignmentData[datasetNum] == null)
                    return null;

                double[,] NETError = analysis.AlignmentData[datasetNum].netErrorHistogram;
                string name = analysis.Datasets[datasetNum].DatasetName;

                /// 
                /// Element by element copies take a lot of time but these arrays are not that big
                /// So we take the perf hit here and just copy this that way.  Improvement could
                /// be made to speed this up with a better block copy or changing the histogram
                /// code in MAEngine to return float arrays.  However, double is better precision
                /// and we'll take that benefit over this one.
                /// 
                float[] bins  = new float[NETError.GetLength(0)];
                float[] freqs = new float[NETError.GetLength(0)];
                for (int i = 0; i < NETError.GetLength(0); i++)
                {
                    bins[i] = Convert.ToSingle(NETError[i, 0]);
                    freqs[i] = Convert.ToSingle(NETError[i, 1]);
                }
                float diff = bins[1] - bins[0];
                chart            = new controlHistogram(bins, freqs, "NET Error Histogram " + name);
                chart.BinSize    = diff;
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
        public static Image ScanVsClusterNet_Thumbnail(MultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.ScanVsClusterNet_Chart(analysis, datasetNum);

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
        public static ctlScatterChart MassErrorVsScanResidual_Chart(MultiAlignAnalysis analysis,
                                                    int datasetNum)
        {
            ctlScatterChart chart = null;

            return chart;
        }
   #endregion

        #region Scan vs Net
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart ScanVsClusterNet_Chart(MultiAlignAnalysis analysis,
                                                    int datasetNum)
        {
            ctlScatterChart chart = null;
    
            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            int numClusters = analysis.UMCData.mobjClusterData.NumClusters;
            int numDatasets = analysis.UMCData.NumDatasets;
            
            
            ArrayList clusterIndices = new ArrayList();
            for (int clusterNum = 0; clusterNum < numClusters; clusterNum++)
            {
                int umcIndex = analysis.UMCData.mobjClusterData.marrClusterMainMemberIndex[clusterNum * numDatasets + datasetNum];
                if (umcIndex != -1)
                {
                    clusterIndices.Add(clusterNum);
                }
            }

            if (clusterIndices.Count != 0)
            {
                int numPoints = clusterIndices.Count;
                float[] scanNums = new float[numPoints];
                float[] nets = new float[numPoints];
                for (int index = 0; index < numPoints; index++)
                {
                    int clusterIndex = (int)clusterIndices[index];
                    int umcIndex = analysis.UMCData.mobjClusterData.marrClusterMainMemberIndex[clusterIndex * numDatasets + datasetNum];
                    scanNums[index] = analysis.UMCData.marr_umcs[umcIndex].mint_scan;
                    nets[index] = (float)analysis.UMCData.mobjClusterData.GetCluster(clusterIndex).mdouble_net;
                }
                /// 
                /// Add data points to the chart.
                /// 
                chart = new ctlScatterChart();
                chart.XAxisLabel = "Scan #";
                chart.YAxisLabel = "NET";

                int ptSize = 1;
                Color clr = Color.Red;
                clsShape shape = new BubbleShape(ptSize, false); ;
                clsPlotParams plt_params = new clsPlotParams(shape, clr);
                plt_params.Name = analysis.UMCData.DatasetName[datasetNum];
                chart.AutoViewPortOnAddition = true;

                clsSeries series = new clsSeries(ref scanNums, ref nets, plt_params);
                chart.AddSeries(series);
                chart.ViewPortHistory.Clear();               
            }
            return chart;
        }
        #endregion

        #region Heatmap
        /// <summary>
        /// Renders the alignment heatmap
        /// </summary>
        public static Image AlignmentHeatmap_Thumbnail(MultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height)
        {
            classAlignmentData data = analysis.AlignmentData[datasetNum];
            return AlignmentHeatmap_Thumbnail(data, width, height);
        }
        /// <summary>
        /// 
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

        #region Cluster Chart
        public static Image ClusterChart_Thumbnail(MultiAlignAnalysis analysis,
                                                   int datasetNum,
                                                   int width,
                                                   int height,
                                                   bool aligned,
                                                   int chargeStates,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {
            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterChart_Chart(analysis,
                                                                                    datasetNum,
                                                                                    aligned,
                                                                                    chargeStates);
                /// 
                /// If a chart exists then modify it so we can make a smooth bitmap
                /// 
                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;
                    chart.LegendVisible  = displayLegend;
                    chart.AxisVisible    = displayAxis;
                    chart.TitleVisible   = displayTitle;
                    chart.XAxisGridLines = false;
                    chart.YAxisGridLines = false;
                    image = chart.ToBitmap(width, height);
                    chart.Dispose();
                }
            }
            catch 
            {
                return null;
            }
            return image;
        }
        public static ctlScatterChart ClusterChart_Chart(   MultiAlignAnalysis analysis,
                                                            int datasetNum,
                                                            bool aligned,
                                                            int chargeStates)
        {
            ctlClusterChart chart = new ctlClusterChart(analysis, datasetNum);            
            return chart;
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
        public static Image ClusterMassVsScanResiduals_Thumbnail(MultiAlignAnalysis analysis,                                                        
                                                        int width,
                                                        int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterMassVsScanResiduals_Chart(analysis);

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
        public static ctlScatterChart ClusterMassVsScanResiduals_Chart(MultiAlignAnalysis analysis)
        {
            ctlScatterChart chart = null;

            if (analysis.ClusterAlignmentData == null)
                return null;

            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            classAlignmentResidualData residual = analysis.ClusterAlignmentData.ResidualData;
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
        public static ctlScatterChart ClusterMassVsMZResidual_Chart(MultiAlignAnalysis analysis)
        {
            ctlScatterChart chart = null;


            if (analysis.ClusterAlignmentData == null)
                return null;

            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            classAlignmentResidualData residual = analysis.ClusterAlignmentData.ResidualData;
            chart = new ctlScatterChart();
            chart.XAxisLabel = "m/z";
            chart.YAxisLabel = "Mass Residual (PPM)";
            chart.Title = " Mass Residual (PPM) vs. m/z";
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
        public static Image ClusterMassVsMZResidual_Thumbnail(MultiAlignAnalysis analysis,                                                    
                                                    int width,
                                                    int height,
                                                            bool displayLegend,
                                                            bool displayAxis,
                                                            bool displayTitle)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterMassVsMZResidual_Chart(analysis);

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
        public static Image ClusterScoreHistogram_Thumbnail(    MultiAlignAnalysis analysis,                                                            
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
                controlHistogram chart = RenderDatasetInfo.ClusterScoreHistogram_Chart(analysis.UMCData.mobjClusterData);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible  = false;
                    chart.AxisVisible    = false;
                    chart.TitleVisible   = false;
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
        public static controlHistogram ClusterScoreHistogram_Chart(clsClusterData clusters)
        {
            controlHistogram chart = null;

            try
            {
                if (clusters == null)
                    return null;

                int N               = clusters.NumClusters;
                float binSize       = 1; 
                int totalBins       = 20;
                float[] bins        = new float[totalBins];
                float[] freqsMedian = new float[totalBins];
                float[] freqsMean   = new float[totalBins];

                double min = double.MaxValue;
                double max = double.MinValue;

                for (int i = 0; i < N; i++)
                {
                    clsCluster cluster = clusters.GetCluster(i);
                    min = Math.Min(min, Math.Max(0, cluster.MedianScore));
                    max = Math.Max(max, cluster.MedianScore);

                    min = Math.Min(min, Math.Max(0, cluster.MeanScore));
                    max = Math.Max(max, cluster.MeanScore);
                }

                // Here we make sure we have a min and max. Otherwise the scores are all the same.
                if (min != max)
                {
                     binSize = Convert.ToSingle((max - min) / Convert.ToDouble(totalBins));
                }

                for(int i = 0; i < totalBins; i++)
                {
                    freqsMean[i] = 0;
                    freqsMedian[i] = 0;
                    bins[i] = Convert.ToSingle(i) * binSize;                    
                }
                
                for (int i = 0; i < N; i++)
                {
                    clsCluster cluster = clusters.GetCluster(i);
                    int j = 0;
                    try
                    {
                        j = Convert.ToInt32(Math.Max(-1.0F, Math.Min(cluster.MedianScore / binSize, totalBins - 1)));
                    }
                    catch 
                    {

                    }
                    
                    if (j >= 0)
                    {
                        freqsMedian[j]++;
                    }

                    try
                    {
                        j = Convert.ToInt32(Math.Max(-1.0F, Math.Min(cluster.MeanScore / binSize, totalBins - 1)));
                    }
                    catch
                    {

                    }

                    if (j >= 0)
                    {
                        freqsMean[j]++;
                    }
                }

                float diff       = binSize;
                chart = new controlHistogram();
                chart.AutoViewPortOnSeriesChange = true;

                chart.Title = "Cluster Score Histogram";
                chart.AddData(bins, freqsMedian, "Median Score", Color.Green);
                chart.AddData(bins, freqsMean, "Mean Score", Color.FromArgb(200, Color.Red));
                chart.BinSize    = diff;
                chart.XAxisLabel = "Score";
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
        public static Image ClusterScoreVsClusterSize_Thumbnail(MultiAlignAnalysis analysis,                                                            
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
                ctlScatterChart chart = RenderDatasetInfo.ClusterScoreVsClusterSize_Chart(analysis.UMCData.mobjClusterData);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = false;
                    chart.AxisVisible   = false;
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
        public static ctlScatterChart ClusterScoreVsClusterSize_Chart(clsClusterData clusters)
        {
            ctlScatterChart chart = null;

            try
            {
                if (clusters == null)
                    return null;

                int N = clusters.NumClusters;
                
                List<float> sizeList        = new List<float>();
                List<float> meanScoreList   = new List<float>();
                List<float> medianScoreList = new List<float>();

                int minn, maxx;
                minn = 100;
                maxx = 0;
                int[] x = new int[10];
                for (int i = 0; i < N; i++)
                {
                    clsCluster cluster = clusters.GetCluster(i);

                    minn = Math.Min(cluster.mshort_num_dataset_members, minn);
                    maxx = Math.Max(cluster.mshort_num_dataset_members, maxx);

                    if (cluster.MeanScore >= 0 && cluster.MedianScore >=0 )
                    {
                        x[cluster.mshort_num_dataset_members]++;
                        meanScoreList.Add(Convert.ToSingle(cluster.MeanScore));
                        medianScoreList.Add(Convert.ToSingle(cluster.MedianScore));
                        sizeList.Add(Convert.ToSingle(cluster.mshort_num_dataset_members));
                    }
                }
                
                float[] meanScores      = new float[meanScoreList.Count];
                float[] medianScores    = new float[meanScoreList.Count];
                float[] sizes           = new float[meanScoreList.Count];

                meanScoreList.CopyTo(meanScores, 0);
                medianScoreList.CopyTo(medianScores, 0);
                sizeList.CopyTo(sizes, 0);

                chart = new ctlScatterChart();

                clsPlotParams medianParameters  = new clsPlotParams(new BubbleShape(2, false), Color.Green);
                medianParameters.Name           = "Median Score";
                clsSeries medianSeries          = new clsSeries(ref sizes, ref medianScores, medianParameters);

                clsPlotParams meanParameters = new clsPlotParams(new BubbleShape(2, false), Color.FromArgb(200, Color.Red));
                meanParameters.Name          = "Mean Score";
                clsSeries meanSeries         = new clsSeries(ref sizes, ref meanScores, meanParameters);


                chart.PadViewPortX = .01F;
                chart.PadViewPortY = .01F;
                chart.AddSeries(medianSeries);
                chart.AddSeries(meanSeries);
                chart.AutoViewPort();
                
                chart.Title         = "Cluster Scores Vs. Cluster Sizes";
                chart.XAxisLabel    = "Cluster Size (Number of Datasets)";
                chart.YAxisLabel    = "Scores";                
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
        public static Image ClusterSizeHistogram_Thumbnail(MultiAlignAnalysis analysis,
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
                controlHistogram chart = RenderDatasetInfo.ClusterSizeHistogram_Chart(analysis);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible  = false;
                    chart.AxisVisible    = false;
                    chart.TitleVisible   = false;
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
        public static controlHistogram  ClusterSizeHistogram_Chart(MultiAlignAnalysis analysis)
        {
            int maxClusters = analysis.Datasets.Count + 1;
            if (maxClusters > 1)
            {
                float[] bins  = new float[maxClusters];
                float[] freqs = new float[maxClusters];

                int i = 0;
                for (i = 0; i < maxClusters; i++)
                {
                    bins[i] = Convert.ToSingle(i + 1);
                    freqs[i] = 0;
                }

                /// 
                /// Make the cluster histogram
                /// 
                foreach (clsCluster cluster in analysis.UMCData.mobjClusterData.marrClusters)
                {
                    freqs[cluster.mshort_num_dataset_members - 1] = freqs[cluster.mshort_num_dataset_members - 1] + 1;
                }


                controlHistogram histogram  = new controlHistogram();
                histogram.BinSize           = 1.0F;
                histogram.AddData(bins, freqs, "Cluster Sizes");
                histogram.XAxisLabel        = "Cluster Size";
                histogram.YAxisLabel        = "Count";
                histogram.Title             = "Cluster Size histogram";

                return histogram;
            }
            
            return null;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image ChargeStateHistogram_Thumbnail(MultiAlignAnalysis analysis,
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
                controlHistogram chart = RenderDatasetInfo.ChargeStateHistogram_Chart(analysis);

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
        public static controlHistogram ChargeStateHistogram_Chart(MultiAlignAnalysis analysis)
        {
            /// 
            /// Find all of the charge states 
            /// 
            float[] charges = new float[30];
            for (int i = 0; i < 30; i++)
            {
                charges[i] = 0;
            }
            int maxCharge = 0;
            for (int i = 0; i < analysis.Datasets.Count; i++)
            {
                clsUMC[] umcs = analysis.UMCData.GetUMCS(i);
                foreach (clsUMC umc in umcs)
                {
                    int j = umc.ChargeRepresentative;
                    if (j > 30)
                        j = 30;
                    maxCharge = Math.Max(maxCharge, j);
                    if (j < 1)
                        j = 1;
                    charges[j - 1]++;
                }
            }

            /// 
            /// Construct histogram
            /// 
            float[] bins  = new float[maxCharge];
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
                ctlScatterChart chart = RenderDatasetInfo.PeakMatchMassNET_Chart(analysis);

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
        public static ctlScatterChart PeakMatchMassNET_Chart(MultiAlignAnalysis analysis)
        {
            clsPeakMatchingResults results = analysis.PeakMatchingResults;                                              
            if (results == null)
                return null;

            clsPeakMatchingResults.clsPeakMatchingTriplet[] arrPeakMatchingTriplets = null;
            clsProtein[] arrPeakMatchingProteins = null;
            clsMassTag[] arrPeakMatchingMassTags = null;

            arrPeakMatchingTriplets = analysis.PeakMatchingResults.marrPeakMatchingTriplet;
            arrPeakMatchingProteins = analysis.PeakMatchingResults.marrProteins;
            arrPeakMatchingMassTags = analysis.PeakMatchingResults.marrMasstags;
            
            int lastClusterNumber = -1;

            /// ////////////////////////////////////////////////////////////////////////////// 
            /// Now we add the data 
            ///     We dont make this clustering writing into a separate method because 
            ///     we have to account for clusters matching to more than one tag.
            /// ////////////////////////////////////////////////////////////////////////////// 
            /// 
            List<float> massResidual   = new List<float>();
            List<float> netResidual    = new List<float>();
            int clusterNum              = 0;
            int num_clusters            = analysis.UMCData.mobjClusterData.NumClusters;
            int currentPeakMatchNum     = 0;

            while (clusterNum < num_clusters)
            {
                bool clusterDidNotPeakMatch = false;
                clsCluster cluster          = analysis.UMCData.mobjClusterData.GetCluster(clusterNum);
                clsUMC[] arrUMCs            = analysis.UMCData.marr_umcs;

                if (arrPeakMatchingTriplets != null &&
                    currentPeakMatchNum < arrPeakMatchingTriplets.Length &&
                    arrPeakMatchingTriplets[currentPeakMatchNum].mintFeatureIndex == clusterNum)
                {
                    ///
                    /// So this peakmatchtriplet corresponds to the current cluster.                     
                    ///                     
                    clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[currentPeakMatchNum];
                    clsMassTag massTag = arrPeakMatchingMassTags[triplet.mintMassTagIndex];

                    massResidual.Add(Convert.ToSingle(cluster.MassCalibrated - massTag.mdblMonoMass));
                    netResidual.Add(Convert.ToSingle(cluster.NetAligned      - massTag.mdblAvgGANET)); 

                    lastClusterNumber   = clusterNum;
                    currentPeakMatchNum++; 
                }
                else
                {                                     
                    clusterNum++;                 
                }
            }

            float[] massResidualPoints = new float[massResidual.Count];
            float[] netResidualPoints = new float[netResidual.Count];

            massResidual.CopyTo(massResidualPoints);
            netResidual.CopyTo(netResidualPoints);

            ctlScatterChart chart            = new ctlScatterChart();

            clsPlotParams parameters         = new clsPlotParams(new BubbleShape(1, false), Color.Red);
            parameters.Name                  = "Feature Cluster Match";
            clsSeries series                 = new clsSeries(ref netResidualPoints, ref massResidualPoints, parameters);
            chart.AutoViewPortOnSeriesChange = true;
            chart.AutoViewPortOnAddition     = true;
            chart.AddSeries(series);
            chart.PadViewPortX = .5F;
            chart.PadViewPortY = .5F;
            chart.Title = "Peak Matching Residuals";
            chart.XAxisLabel = "NET Residuals (%)";
            chart.YAxisLabel = "Mass Residuals (PPM)";

            return chart;
        }

        
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image SMARTScoreHistogram_Thumbnail(MultiAlignAnalysis analysis,int width, int height, bool displayLegend, bool displayAxis, bool displayTitle)
        {

            Image image = null;
            try
            {
                controlHistogram chart = RenderDatasetInfo.SMARTScoreHistogram_Chart(analysis);

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
        public static controlHistogram SMARTScoreHistogram_Chart(MultiAlignAnalysis analysis)
        {
            
            clsPeakMatchingResults results = analysis.PeakMatchingResults;                                              
            if (results == null)
                return null;

            if (analysis.UseSMART == false)
                return null;

            clsPeakMatchingResults.clsPeakMatchingTriplet[] arrPeakMatchingTriplets = null;
            clsProtein[] arrPeakMatchingProteins = null;
            clsMassTag[] arrPeakMatchingMassTags = null;

            if (!analysis.UseSMART)
                return null;

            arrPeakMatchingTriplets = analysis.PeakMatchingResults.marrPeakMatchingTriplet;
            arrPeakMatchingProteins = analysis.PeakMatchingResults.marrProteins;
            arrPeakMatchingMassTags = analysis.PeakMatchingResults.marrMasstags;
            
            int lastClusterNumber = -1;

            /// ////////////////////////////////////////////////////////////////////////////// 
            /// Now we add the data 
            ///     We dont make this clustering writing into a separate method because 
            ///     we have to account for clusters matching to more than one tag.
            /// ////////////////////////////////////////////////////////////////////////////// 
            /// 
            int clusterNum              = 0;
            int num_clusters            = analysis.UMCData.mobjClusterData.NumClusters;
            int currentPeakMatchNum     = 0;

            List<float> scores = new List<float>();

            while (clusterNum < num_clusters)
            {                
                clsCluster cluster          = analysis.UMCData.mobjClusterData.GetCluster(clusterNum);
                clsUMC[] arrUMCs            = analysis.UMCData.marr_umcs;

                if (arrPeakMatchingTriplets != null &&
                    currentPeakMatchNum < arrPeakMatchingTriplets.Length &&
                    arrPeakMatchingTriplets[currentPeakMatchNum].mintFeatureIndex == clusterNum)
                {
                    ///
                    /// So this peakmatchtriplet corresponds to the current cluster.                     
                    ///                     
                    clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[currentPeakMatchNum];
                    clsMassTag massTag = arrPeakMatchingMassTags[triplet.mintMassTagIndex];

                    /// 
                    /// See if a SMART score exists
                    /// 
                    List<classSMARTProbabilityResult> smartScores = null;
                    smartScores = analysis.SMARTResults.GetResultFromUMCIndex(triplet.mintFeatureIndex);
                    if (smartScores != null)
                    {
                        /// 
                        /// Then pull out the SMART score that matches for this triplet Mass Tag
                        /// 
                        PNNLProteomics.SMART.classSMARTProbabilityResult finalResult = null;
                        foreach (PNNLProteomics.SMART.classSMARTProbabilityResult score in smartScores)
                        {
                            if (score.MassTagID == massTag.Id)
                            {
                                finalResult = score;
                                break;
                            }
                        }

                        if (finalResult != null && finalResult.Score > 0.0)
                        {
                            scores.Add(Convert.ToSingle(finalResult.Score));
                        }
                    }
                    lastClusterNumber   = clusterNum;
                    currentPeakMatchNum++; 
                }
                else
                {                                     
                    clusterNum++;                 
                }
            }

            if (scores.Count == 0)
                return null;
            scores.Sort();

            float min  = scores[0];
            float max  = scores[scores.Count - 1];
            float diff = .01F;

            if (diff == 0)
                return null;

            int N = 101;

            float[] bins  = new float[N];
            float[] freqs = new float[N];

            for (int i = 0; i < N; i++)
            {
                bins[i]  = i * diff;
                freqs[i] = 0.0F;
            }

            foreach (float score in scores)
            {
                int bin =  Convert.ToInt32(score / diff);
                bin     = Math.Min(freqs.Length - 1, bin);
                bin     = Math.Max(0, bin);
                freqs[bin]++;
            }

            controlHistogram chart           = new controlHistogram();

            clsPlotParams parameters         = new clsPlotParams(new BubbleShape(1, false), Color.Red);
            parameters.Name                  = "STAC Score";
            clsSeries series                 = new clsSeries(ref bins, ref freqs, parameters);
            chart.BinSize                    = diff;
            chart.AutoViewPortOnAddition     = true;
            chart.AddSeries(series);
            chart.Title                      = "STAC Score Histogram";
            chart.XAxisLabel                 = "STAC Score";
            chart.YAxisLabel                 = "Count";

            return chart;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image UnidentifiedFeatures_Thumbnail( MultiAlignAnalysis analysis, int width, int height, 
                                                            bool displayLegend, bool displayAxis, bool displayTitle,
                                                            double stacThreshold)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.UnidentifiedFeatures_Chart(analysis, stacThreshold);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible  = false;
                    chart.AxisVisible    = false;
                    chart.TitleVisible   = false;
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
        public static ctlScatterChart UnidentifiedFeatures_Chart(MultiAlignAnalysis analysis, double stacThreshold)
        {
            clsPeakMatchingResults results = analysis.PeakMatchingResults;                                              
            if (results == null)
                return null;

            clsPeakMatchingResults.clsPeakMatchingTriplet[] arrPeakMatchingTriplets = null;
            clsProtein[] arrPeakMatchingProteins = null;
            clsMassTag[] arrPeakMatchingMassTags = null;


            arrPeakMatchingTriplets = analysis.PeakMatchingResults.marrPeakMatchingTriplet;
            arrPeakMatchingProteins = analysis.PeakMatchingResults.marrProteins;
            arrPeakMatchingMassTags = analysis.PeakMatchingResults.marrMasstags;
            
            int lastClusterNumber = -1;

            /// ////////////////////////////////////////////////////////////////////////////// 
            /// Now we add the data 
            ///     We dont make this clustering writing into a separate method because 
            ///     we have to account for clusters matching to more than one tag.
            /// ////////////////////////////////////////////////////////////////////////////// 
            /// 
            List<float> massLowConfidence   = new List<float>();
            List<float> netLowConfidence    = new List<float>();
            int clusterNum                  = 0;
            int num_clusters                = analysis.UMCData.mobjClusterData.NumClusters;
            int currentPeakMatchNum         = 0;

            clsUMC[] arrUMCs                = analysis.UMCData.marr_umcs;

            bool hasPeakMatched             = false;
            bool isHighConfident            = true;
            List<clsCluster> clusters = new List<clsCluster>();

            while (clusterNum < num_clusters)
            {                
                clsCluster cluster          = analysis.UMCData.mobjClusterData.GetCluster(clusterNum);
                

                if (arrPeakMatchingTriplets != null &&
                    currentPeakMatchNum < arrPeakMatchingTriplets.Length &&
                    arrPeakMatchingTriplets[currentPeakMatchNum].mintFeatureIndex == clusterNum)
                {
                    ///
                    /// So this peakmatchtriplet corresponds to the current cluster.                     
                    ///                     
                    clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[currentPeakMatchNum];
                    clsMassTag massTag = arrPeakMatchingMassTags[triplet.mintMassTagIndex];

                    if (analysis.UseSMART)
                    {
                        /// 
                        /// See if a SMART score exists
                        /// 
                        List<classSMARTProbabilityResult> smartScores = null;
                        smartScores = analysis.SMARTResults.GetResultFromUMCIndex(triplet.mintFeatureIndex);
                        if (smartScores != null)
                        {
                            /// 
                            /// Then pull out the SMART score that matches for this triplet Mass Tag
                            /// 
                            PNNLProteomics.SMART.classSMARTProbabilityResult finalResult = null;
                            foreach (PNNLProteomics.SMART.classSMARTProbabilityResult score in smartScores)
                            {
                                if (score.MassTagID == massTag.Id)
                                {
                                    finalResult = score;
                                    break;
                                }
                            }
                            /// 
                            /// If we have a final result, then we have a smart score for this MTID for the matched UMC.
                            /// 

                            if (finalResult != null && stacThreshold < finalResult.Score)
                            {
                                // Add feature to plot...
                                //mass.Add(cluster.MassCalibrated);
                                //net.Add(cluster.NetAligned);
                                isHighConfident = true;
                            }
                            else
                            {
                                isHighConfident = false;
                            }
                        }
                    }
                    hasPeakMatched = true;


                    lastClusterNumber   = clusterNum;
                    currentPeakMatchNum++; 
                }
                else
                {
                    /// We only add if this guy has peak matched or not...
                    if (!hasPeakMatched)
                    {
                        clusters.Add(cluster);
                    }
                    else if (! isHighConfident)
                    {
                        massLowConfidence.Add(Convert.ToSingle(cluster.MassCalibrated));
                        netLowConfidence.Add(Convert.ToSingle(cluster.NetAligned));

                    }
                    hasPeakMatched  = false;
                    isHighConfident = true;
                    clusterNum++;                 
                }
            }

            

            ctlScatterChart chart               = new ctlScatterChart();
            chart.AutoViewPortOnSeriesChange    = true;
            chart.AutoViewPortOnAddition        = true;
            chart.Title         = "Unidentified Feature Clusters";
            chart.XAxisLabel    = "NET";
            chart.YAxisLabel    = "Monoisotopic Mass";

            clsColorIterator iterator = new clsColorIterator();
            iterator.CreateColorGradient(Color.Red, analysis.Datasets.Count);

            for (int i = analysis.Datasets.Count; i > 0; i--)
            {

                clsSeries sizeSeries = GetSeriesOfClusterSizes(i, clusters, "Unidentified clusters size = " + i.ToString(), iterator.GetColor(i));
                if (sizeSeries != null)
                {
                    chart.AddSeries(sizeSeries);
                }
            }                                
            
            if (analysis.UseSMART == true)
            {
                float[] massLowScores = new float[massLowConfidence.Count];
                float[] netLowScores = new float[netLowConfidence.Count];
                massLowConfidence.CopyTo(massLowScores);
                netLowConfidence.CopyTo(netLowScores);

                clsPlotParams parametersLowScores = new clsPlotParams(new DiamondShape(2, false), Color.Green);
                parametersLowScores.Name = string.Format("Low STAC Score Clusters: {0:0.00}", stacThreshold);
                clsSeries seriesLowScores = new clsSeries(ref netLowScores, ref massLowScores, parametersLowScores);
                chart.AddSeries(seriesLowScores);
            }
            return chart;
             
        }

        /// <summary>
        /// Returns a series for painting whose point size is displayed as a function of cluster size.
        /// </summary>
        /// <param name="clusterSize"></param>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static clsSeries GetSeriesOfClusterSizes(int clusterSize, List<clsCluster> clusters, string name, Color color)
        {
            
            int r = Math.Max(clusterSize, 10);

            List<float> mass = new List<float>();
            List<float> net  = new List<float>();

            foreach (clsCluster cluster in clusters)
            {
                if (cluster.mshort_num_dataset_members == clusterSize)
                {
                    mass.Add(Convert.ToSingle(cluster.MassCalibrated));
                    net.Add(Convert.ToSingle(cluster.NetAligned));
                }
            }

            if (mass.Count == 0)
                return null;

            float[] masses = new float[mass.Count];
            float[] nets   = new float[net.Count];

            mass.CopyTo(masses, 0);
            net.CopyTo(nets, 0);

            clsPlotParams param = new clsPlotParams(new BubbleShape(clusterSize, false), color);
            param.Name          = name;
            clsSeries series    = new clsSeries(ref nets, ref masses, param);
            return series;
        }

        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image IdentifiedFeatures_Thumbnail(MultiAlignAnalysis analysis, int width, int height,
                                                            bool displayLegend, bool displayAxis, bool displayTitle,
                                                            double stacThreshold)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = RenderDatasetInfo.IdentifiedFeatures_Chart(analysis, stacThreshold);

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
        public static ctlScatterChart IdentifiedFeatures_Chart(MultiAlignAnalysis analysis, double stacThreshold)
        {
            clsPeakMatchingResults results = analysis.PeakMatchingResults;
            if (results == null)
                return null;

            clsPeakMatchingResults.clsPeakMatchingTriplet[] arrPeakMatchingTriplets = null;
            clsProtein[] arrPeakMatchingProteins = null;
            clsMassTag[] arrPeakMatchingMassTags = null;


            arrPeakMatchingTriplets = analysis.PeakMatchingResults.marrPeakMatchingTriplet;
            arrPeakMatchingProteins = analysis.PeakMatchingResults.marrProteins;
            arrPeakMatchingMassTags = analysis.PeakMatchingResults.marrMasstags;

            int lastClusterNumber = -1;

            /// ////////////////////////////////////////////////////////////////////////////// 
            /// Now we add the data 
            ///     We dont make this clustering writing into a separate method because 
            ///     we have to account for clusters matching to more than one tag.
            /// //////////////////////////////////////////////////////////////////////////////             
            List<float> massHighConfidence = new List<float>();
            List<float> netHighConfidence = new List<float>();
            
            int clusterNum = 0;
            int num_clusters = analysis.UMCData.mobjClusterData.NumClusters;
            int currentPeakMatchNum = 0;

            clsUMC[] arrUMCs = analysis.UMCData.marr_umcs;

            bool hasPeakMatched = false;
            bool isHighConfident = true;

            List<clsCluster> clusters = new List<clsCluster>();

            while (clusterNum < num_clusters)
            {
                clsCluster cluster = analysis.UMCData.mobjClusterData.GetCluster(clusterNum);


                if (arrPeakMatchingTriplets != null &&
                    currentPeakMatchNum < arrPeakMatchingTriplets.Length &&
                    arrPeakMatchingTriplets[currentPeakMatchNum].mintFeatureIndex == clusterNum)
                {
                    ///
                    /// So this peakmatchtriplet corresponds to the current cluster.                     
                    ///                     
                    clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[currentPeakMatchNum];
                    clsMassTag massTag = arrPeakMatchingMassTags[triplet.mintMassTagIndex];

                    if (analysis.UseSMART)
                    {
                        /// 
                        /// See if a SMART score exists
                        /// 
                        List<classSMARTProbabilityResult> smartScores = null;
                        smartScores = analysis.SMARTResults.GetResultFromUMCIndex(triplet.mintFeatureIndex);
                        if (smartScores != null)
                        {
                            /// 
                            /// Then pull out the SMART score that matches for this triplet Mass Tag
                            /// 
                            PNNLProteomics.SMART.classSMARTProbabilityResult finalResult = null;
                            foreach (PNNLProteomics.SMART.classSMARTProbabilityResult score in smartScores)
                            {
                                if (score.MassTagID == massTag.Id)
                                {
                                    finalResult = score;
                                    break;
                                }
                            }
                            /// 
                            /// If we have a final result, then we have a smart score for this MTID for the matched UMC.
                            /// 

                            if (finalResult != null && stacThreshold < finalResult.Score)
                            {
                                isHighConfident = true;
                            }
                            else
                            {
                                isHighConfident = false;
                            }
                        }
                    }
                    hasPeakMatched = true;


                    lastClusterNumber = clusterNum;
                    currentPeakMatchNum++;
                }
                else
                {
                    /// We only add if this guy has peak matched or not...
                    if (hasPeakMatched)
                    {
                        clusters.Add(cluster);
                    }
                    else if (isHighConfident)
                    {
                        massHighConfidence.Add(Convert.ToSingle(cluster.MassCalibrated));
                        netHighConfidence.Add(Convert.ToSingle(cluster.NetAligned));
                    }
                    hasPeakMatched = false;
                    isHighConfident = true;
                    clusterNum++;
                }
            }

            ctlScatterChart chart = new ctlScatterChart();
            chart.AutoViewPortOnSeriesChange = true;
            chart.AutoViewPortOnAddition = true;
            chart.Title = "Identified Feature Clusters";
            chart.XAxisLabel = "NET";
            chart.YAxisLabel = "Monoisotopic Mass";
            clsColorIterator iterator = new clsColorIterator();
            iterator.CreateColorGradient(Color.Red, analysis.Datasets.Count);

            for (int i = analysis.Datasets.Count; i > 0; i--)
            {

                clsSeries sizeSeries = GetSeriesOfClusterSizes(i, clusters, "Identified clusters size = " + i.ToString(), iterator.GetColor(i));
                if (sizeSeries != null)
                {
                    chart.AddSeries(sizeSeries);
                }
            }   
                        
            if (analysis.UseSMART == true)
            {
                float[] massHighScores = new float[massHighConfidence.Count];
                float[] netHighScores = new float[netHighConfidence.Count];
                massHighConfidence.CopyTo(massHighScores);
                netHighConfidence.CopyTo(netHighScores);

                clsPlotParams parametersHighScores = new clsPlotParams(new BubbleShape(1, false), Color.Green);
                parametersHighScores.Name = string.Format("High STAC Score Clusters: {0:0.00}", stacThreshold);

                clsSeries seriesHighScores = new clsSeries(ref netHighScores, ref massHighScores, parametersHighScores);
                chart.AddSeries(seriesHighScores);
            }
            return chart;

        }
        #endregion
    }  
}
