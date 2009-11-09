using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using PNNLControls;

using MultiAlignEngine;
using MultiAlignWin;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;

using PNNLProteomics.Data.Analysis;
using PNNLProteomics.Data.Alignment;

namespace MultiAlignWin.Drawing
{
    /// <summary>
    /// Factory class for rendering information about a dataset info class.
    /// </summary>
    public class classRenderDatasetInfo
    {
        private const int CONST_PRE_POINT_SIZE  = 3;
        private const int CONST_POST_POINT_SIZE = 2;


        #region NET Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image NETResiduals_Thumbnail( clsMultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = classRenderDatasetInfo.NETResiduals_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = false;
                    chart.AxisVisible = false;
                    chart.TitleVisible = false;

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
        public static ctlScatterChart NETResiduals_Chart(clsMultiAlignAnalysis analysis, int datasetNum)
        {
            ctlScatterChart chart = null;

            if (analysis.AlignmentData.Count < datasetNum)
                return null; 

            /// 
            /// Set the data for the chart.          
            /// 
            classAlignmentResidualData residual = analysis.AlignmentData[datasetNum].ResidualData;
            chart                           = new ctlScatterChart();
            chart.XAxisLabel                = "Scan #";
            chart.YAxisLabel                = "NET Residual";
            chart.Title                     = analysis.UMCData.DatasetName[datasetNum] + " NET Residuals";
            
            clsShape shape                  = new BubbleShape(CONST_PRE_POINT_SIZE, false);
            clsPlotParams plt_params        = new clsPlotParams(shape, Color.Blue);
            plt_params.Name                 = "Predicted Linear";
            chart.AutoViewPortOnAddition    = true;
            clsSeries series                = new clsSeries(ref residual.scans, ref residual.linearNet, plt_params);
            chart.AddSeries(series);

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

        #region Mass Vs Scan Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart MassVsScanResiduals_Chart(clsMultiAlignAnalysis analysis, int datasetNum)
        {
            ctlScatterChart chart = null;

            if (analysis.AlignmentData.Count < datasetNum)
                return null; 

            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            classAlignmentResidualData residual = analysis.AlignmentData[datasetNum].ResidualData;
            chart = new ctlScatterChart();
            chart.XAxisLabel = "Scan #";
            chart.YAxisLabel = "Mass Residual";
            chart.Title = analysis.UMCData.DatasetName[datasetNum] + " Mass vs Scan Residual";
            int ptSize     = CONST_PRE_POINT_SIZE;
            Color clr      = Color.Blue;
            clsShape shape = new BubbleShape(ptSize, false);

            /// 
            /// Residual Plots of mass error pre-corrected
            /// 
            clsPlotParams plt_params = new clsPlotParams(shape, Color.Blue);
            plt_params.Name = "Pre-Alignment";
            chart.AutoViewPortOnAddition = true;
            clsSeries series = new clsSeries(ref residual.scans, ref residual.massError, plt_params);
            chart.AddSeries(series);


            /// 
            /// Residual Plots of mass error post-corrected?
            /// 
            clsShape alignedShape = new BubbleShape(CONST_POST_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name = "Aligned";
            chart.AutoViewPortOnAddition = true;
            clsSeries seriesCorrected = new clsSeries(ref residual.scans, ref residual.massErrorCorrected, plt_paramsAligned);
            chart.AddSeries(seriesCorrected);
            chart.ViewPortHistory.Clear();

            return chart;
        }

        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image MassVsScanResiduals_Thumbnail(clsMultiAlignAnalysis analysis,
                                                        int datasetNum,
                                                        int width,
                                                        int height)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = classRenderDatasetInfo.MassVsScanResiduals_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin     = 1;
                    chart.Margins.LeftMarginMax     = 1;
                    chart.Margins.BottomMarginMax   = 1;
                    chart.Margins.BottomMarginMin   = 1;

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
        #endregion

        #region Mass Vs Mz Residual Plots
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static ctlScatterChart MassVsMZResidual_Chart(clsMultiAlignAnalysis analysis,
                                                    int datasetNum)
        {
            ctlScatterChart chart = null;

            if (analysis.AlignmentData.Count < datasetNum)
                return null; 


            /// 
            /// Set the data for the chart. 
            /// Go through each cluster that this dataset was seen in
            /// and plot scan vs net of cluster. 
            /// 
            classAlignmentResidualData residual = analysis.AlignmentData[datasetNum].ResidualData;
            chart = new ctlScatterChart();
            chart.XAxisLabel = "m/z";
            chart.YAxisLabel = "Mass Residual";
            chart.Title = analysis.UMCData.DatasetName[datasetNum] + " Mass vs M/Z Residual";
            int ptSize = CONST_PRE_POINT_SIZE;
            Color clr = Color.Blue;
            clsShape shape = new BubbleShape(ptSize, false);

            /// 
            /// Residual Plots of mass vs mz error pre-corrected
            /// 
            clsPlotParams plt_params = new clsPlotParams(shape, Color.Blue);
            plt_params.Name = "Pre-Alignment";
            chart.AutoViewPortOnAddition = true;
            clsSeries series = new clsSeries(ref residual.mz, ref residual.mzMassError, plt_params);
            chart.AddSeries(series);


            /// 
            /// Residual Plots of mass vs mz error post-correcteds
            /// 
            clsShape alignedShape = new BubbleShape(CONST_POST_POINT_SIZE, false);
            clsPlotParams plt_paramsAligned = new clsPlotParams(alignedShape, Color.Red);
            plt_paramsAligned.Name = "Post-Alignment";
            chart.AutoViewPortOnAddition = true;
            clsSeries seriesCorrected = new clsSeries(ref residual.mz, ref residual.mzMassErrorCorrected, plt_paramsAligned);
            chart.AddSeries(seriesCorrected);
            chart.ViewPortHistory.Clear();

            return chart;
        }
        /// <summary>
        /// Renders the scan versus the cluster net to the provided bitmap.
        /// </summary>
        public static Image MassVsMZResidual_Thumbnail(clsMultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = classRenderDatasetInfo.MassVsMZResidual_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;
                    chart.LegendVisible = false;
                    chart.AxisVisible = false;
                    chart.TitleVisible = false;
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
        public static Image MassErrorHistogram_Thumbnail(clsMultiAlignAnalysis analysis,
                                                            int datasetNum,
                                                            int width,
                                                            int height)
        {

            Image image = null;
            try
            {
                controlHistogram chart = classRenderDatasetInfo.MassErrorHistogram_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin   = 1;
                    chart.Margins.LeftMarginMax   = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;

                    chart.LegendVisible = false;
                    chart.AxisVisible   = false;
                    chart.TitleVisible  = false;

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
        public static controlHistogram MassErrorHistogram_Chart(clsMultiAlignAnalysis analysis,
                                                                int datasetNum)
        {
            controlHistogram chart = null;

            try
            {
                
                double [,] massError = analysis.AlignmentData[datasetNum].massErrorHistogram;
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
                chart            = new controlHistogram(bins, freqs, "Mass Error Histogram");
                chart.XAxisLabel = "Mass Error";
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
        public static Image NETErrorHistogram_Thumbnail(clsMultiAlignAnalysis analysis,
                                                            int datasetNum,
                                                            int width,
                                                            int height)
        {

            Image image = null;
            try
            {
                controlHistogram chart = classRenderDatasetInfo.NETErrorHistogram_Chart(analysis, datasetNum);

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
        public static controlHistogram NETErrorHistogram_Chart(clsMultiAlignAnalysis analysis,
                                                                int datasetNum)
        {
            controlHistogram chart = null;

            try
            {

                double[,] NETError = analysis.AlignmentData[datasetNum].netErrorHistogram;
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
                chart            = new controlHistogram(bins, freqs, "NET Error Histogram");
                chart.XAxisLabel = "NET Error";
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
        public static Image ScanVsClusterNet_Thumbnail(clsMultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height)
        {

            Image image = null;
            try
            {
                ctlScatterChart chart = classRenderDatasetInfo.ScanVsClusterNet_Chart(analysis, datasetNum);

                if (chart != null)
                {
                    chart.Margins.LeftMarginMin = 1;
                    chart.Margins.LeftMarginMax = 1;
                    chart.Margins.BottomMarginMax = 1;
                    chart.Margins.BottomMarginMin = 1;
                    chart.LegendVisible = false;
                    chart.AxisVisible = false;
                    chart.TitleVisible = false;
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
        public static ctlScatterChart MassErrorVsScanResidual_Chart(clsMultiAlignAnalysis analysis,
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
        public static ctlScatterChart ScanVsClusterNet_Chart(clsMultiAlignAnalysis analysis,
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
              //  try
                {
                    chart = new ctlScatterChart();
                    chart.XAxisLabel = "Scan #";
                    chart.YAxisLabel = "Cluster NET";

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
               //catch(Exception ex)
                {

                    //return null;
                }
            }
            return chart;
        }
        #endregion

        #region Heatmap
        /// <summary>
        /// Renders the alignment heatmap
        /// </summary>
        public static Image AlignmentHeatmap_Thumbnail(clsMultiAlignAnalysis analysis,
                                                    int datasetNum,
                                                    int width,
                                                    int height)
        {
            Image image = null;            
            try
            {
                ctlAlignmentHeatMap chart = classRenderDatasetInfo.AlignmentHeatMap_Chart(analysis, datasetNum);
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
        public static ctlAlignmentHeatMap AlignmentHeatMap_Chart(clsMultiAlignAnalysis analysis,
                                                    int datasetNum)
        {            
            ctlAlignmentHeatMap heatMap = new ctlAlignmentHeatMap();
            clsAlignmentFunction alignmentFnc;
            string   datasetName;
            int      minAligneeScan, maxAligneeScan;
            float    minBaselineScan, maxBaselineScan;
            float[,] mScores;

            try
            {
                datasetName = analysis.FileNames[datasetNum];                
                classAlignmentData data = analysis.AlignmentData[datasetNum];                                
                if (data == null)
                    return null;

                alignmentFnc    = data.alignmentFunction;
                datasetName     = data.aligneeDataset;
                mScores         = data.heatScores;
                minBaselineScan = data.minMTDBNET;
                maxBaselineScan = data.maxMTDBNET;
                minAligneeScan  = data.minScanBaseline;
                maxAligneeScan  = data.maxScanBaseline;
                
                /*minBaselineScan = data.minMTDBNET;
                maxBaselineScan = data.maxMTDBNET;
                minAligneeScan = data.minScanBaseline;
                maxAligneeScan = data.maxScanBaseline;
                */

                /*GetAlignmentData(  datasetNum,
                                        ref alignmentFnc, 
                                        ref datasetName, 
                                        ref mScores,
                                        ref minAligneeScan,
                                        ref maxAligneeScan,
                                        ref minBaselineScan,
                                        ref maxBaselineScan);
                */

                // first zscore the data on the level of the x axis.
                /*
                int numRows = mScores.GetUpperBound(0) - mScores.GetLowerBound(0);
                int numColumns = mScores.GetUpperBound(1) - mScores.GetLowerBound(1);
                for (int colNum = 0; colNum < numColumns; colNum++)
                {
                    for (int rowNum = 0; rowNum < numRows / 2; rowNum++)
                    {
                        float tmp = mScores[rowNum, colNum];
                        mScores[rowNum, colNum] = mScores[numRows - rowNum - 1, colNum];
                        mScores[numRows - rowNum - 1, colNum] = tmp;
                    }
                }
                 */
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
        public static Image ClusterChart_Thumbnail(clsMultiAlignAnalysis analysis,
                                                   int datasetNum,
                                                   int width,
                                                   int height,
                                                   bool aligned,
                                                   int chargeStates)
        {
            Image image = null;
            try
            {
                ctlScatterChart chart = classRenderDatasetInfo.ClusterChart_Chart(analysis,
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
                    chart.LegendVisible = false;
                    chart.AxisVisible = false;
                    chart.TitleVisible = false;
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
        public static ctlScatterChart ClusterChart_Chart(   clsMultiAlignAnalysis analysis,
                                                            int datasetNum,
                                                            bool aligned,
                                                            int chargeStates)
        {
            ctlClusterChart chart = new ctlClusterChart();
            chart.AddDatasetToOverlapChart(analysis,
                                            datasetNum,
                                            true, 
                                            chargeStates);                           
            return chart;
        }
        #endregion
    }
}
