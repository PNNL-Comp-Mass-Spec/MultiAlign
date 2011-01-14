using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;

using System.Windows.Forms;

using MultiAlignEngine;
using MultiAlignEngine.Features;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;

using PNNLControls;
using MultiAlignWin.Drawing;

using PNNLProteomics.Data;
using PNNLProteomics.Data.Analysis;

using MultiAlign.Drawing;
using MultiAlign.Charting;

namespace MultiAlignWin.Forms
{
    /// <summary>
    /// Class that handles rendering the dataset information.
    /// </summary>
    public partial class controlDatasetInformation : UserControl
    {
        #region Members
        /// <summary>
        /// Forms displayed by preview.
        /// </summary>
        private Dictionary<string, Form> mdict_forms = new Dictionary<string, Form>();
        /// <summary>
        /// Number of charge states to visually display in cluster chart.
        /// </summary>
        private const int CONST_NUM_CHARGE_STATES = 5;
        /// <summary>
        /// Delegate when the preview is finished rendering.
        /// </summary>
        /// <param name="sender">Dataset that was rendered.</param>
        /// <param name="previews">Preview images</param>
        private delegate void DelegateUpdatePreview(controlDatasetInformation sender, Image[] previews);        
        /// <summary>
        /// Dataset to probe.
        /// </summary>
        private DatasetInformation mobj_dataset;
        /// <summary>
        /// Options defining the alignment for this dataset.
        /// </summary>
        private clsAlignmentOptions mobj_alignment;
        /// <summary>
        /// Options used to cluster this dataset.
        /// </summary>
        private clsClusterOptions mobj_cluster;
        /// <summary>
        /// Object that holds all of the analysis data.
        /// </summary>
        private MultiAlignAnalysis mobj_analysis;
        /// <summary>
        /// Index of this dataset in the analysis.
        /// </summary>
        private int mint_datasetIndex;
        /// <summary>
        /// Number of charge states to display on cluster chart.
        /// </summary>
        private int mint_chargeStates;
        /// <summary>
        /// Alignment data.
        /// </summary>
        private PNNLProteomics.Data.Alignment.classAlignmentData mobj_alignmentData;
        #endregion

        /// <summary>
        /// Default constructor for a dataset class.
        /// </summary>
        public controlDatasetInformation(MultiAlignAnalysis analysis, int datasetIndex)
        {
            InitializeComponent();

            mobj_dataset        = analysis.Datasets[datasetIndex] as DatasetInformation;
            mobj_alignment      = analysis.AlignmentOptions[datasetIndex] as clsAlignmentOptions;
            mobj_alignmentData  = analysis.AlignmentData[datasetIndex]; 
            mobj_cluster        = analysis.ClusterOptions;
            mobj_analysis       = analysis;
            mint_datasetIndex   = datasetIndex;
            mint_chargeStates   = CONST_NUM_CHARGE_STATES;

            UpdateUserInterface();            
        }


        #region Button Link Event Handlers
        
        /// <summary>
        /// Handles when the user clicks to see the dataset information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_datasetName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DatasetSummary summary = new DatasetSummary(mobj_dataset, mobj_alignment, mobj_cluster);
            summary.ShowDialog();
        }
        /// <summary>
        /// Handles when the user clicks to see the baseline options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_baseline_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {         
        }
        #endregion 

        #region Renderers

        /// <summary>
        /// Updates the user interface with the appropiate data.
        /// </summary>
        private void UpdateUserInterface()
        {
            if (mobj_alignment == null || mobj_dataset == null || mobj_cluster == null)
                return;

            /// 
            /// Dataset information pane
            /// 
            mlabel_datasetID.Text    = string.Format("{0}", mobj_dataset.mstrDatasetId);
            mbutton_datasetName.Text = string.Format("{0}", mobj_dataset.DatasetName);
            mbutton_baseline.Text    = Path.GetFileName(mobj_alignment.AlignmentBaselineName);

            clsUMC [] umcs = mobj_analysis.UMCData.GetUMCS(mint_datasetIndex);
            mlabel_umcs.Text = string.Format("{0}", umcs.Length);

            if (mobj_alignmentData != null)
            {
                mlabel_stdevMass.Text = string.Format("Stdev. Mass = {0:0.000}", mobj_alignmentData.MassStandardDeviation);
                mlabel_stdevNET.Text  = string.Format("Stdev. NET = {0:0.000}", mobj_alignmentData.NETStandardDeviation);
            }
            PerformLayout();
        }
        /// <summary>
        /// Updates the previews with the preview images.
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="scanNet"></param>
        /// <param name="alignment"></param>
        public void SetImages(Image rawData,
                                Image scanNet,
                                Image alignment,
                                Image net,
                                Image massError,
                                Image alignedNetResidual,
                                Image massResidual,
                                Image mzMassResidual,
                                Image massNetResidual)
        {
            if (alignment != null)
            {
                mpicture_alignmentHeatmap.Image = alignment;
                mpicture_alignmentHeatmap.Visible = true;
                mlabel_alignment.Visible = true;
            }

            /// 
            /// If the dataset is the baseline display that.
            /// 
            if (mobj_dataset.DatasetName == Path.GetFileName(mobj_alignment.AlignmentBaselineName))
            {
                if (mpicture_alignmentHeatmap.Image != null)
                    mpicture_alignmentHeatmap.Image.Dispose();

                Image image = new Bitmap(mpicture_alignmentHeatmap.Width, mpicture_alignmentHeatmap.Height);
                Graphics g = Graphics.FromImage(image);

                StringFormat format = new StringFormat(StringFormatFlags.FitBlackBox);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;


                g.DrawString("Baseline",
                            this.Font,
                            new SolidBrush(Color.Black),
                            new RectangleF(0.0F, 0.0F, image.Width, image.Height),
                            format);
                g.RotateTransform(45.0F);

                mpicture_preview.Image               = image;
                mpicture_alignmentHeatmap.Image      = image;
                mpictureBox_massErrorHistogram.Image = image;
                mpictureBox_netErrorHistogram.Image  = image;
                mpicture_massResiduals.Image         = image;
                mpictureBox_massNetResiduals.Image   = image;
                mpictureBox_mzMassResidual.Image     = image;
                mpicture_netResiduals.Image          = image;
                mpicture_alignmentHeatmap.Visible    = true;
                mlabel_alignment.Visible             = true;
            }

            if (scanNet != null)
                mpicture_preview.Image = scanNet;
            if (rawData != null)
                mpicture_rawData.Image = rawData;
            if (net != null)
                mpictureBox_netErrorHistogram.Image = net;
            if (massError != null)
                mpictureBox_massErrorHistogram.Image = massError;
            if (alignedNetResidual != null)
                mpicture_netResiduals.Image = alignedNetResidual;
            if (massResidual != null)
                mpicture_massResiduals.Image = massResidual;
            if (mzMassResidual != null)
                mpictureBox_mzMassResidual.Image = mzMassResidual;
            if (massNetResidual != null)
                mpictureBox_massNetResiduals.Image = massNetResidual;

        }
        /// <summary>
        /// Handles rendering images when the control is finished rendering.
        /// </summary>
        /// <param name="dataset">Caller dataset, this</param>
        /// <param name="images">Images to render.</param>
        public void PreviewHandler(controlDatasetInformation dataset, Image[] images)
        {
            SetImages(images[0],
                        images[1],
                        images[2],
                        images[3],
                        images[4],
                        images[5],
                        images[6],
                        images[7],
                        images[8]);
        }
        private bool AbortImagePreview()
        {
            return false;
        }       
        /// <summary>
        /// Creates the preview images for display.
        /// </summary>
        public void RenderPreviews()
        {
            int width, height;

            width  = mpicture_alignmentHeatmap.Width;
            height = mpicture_alignmentHeatmap.Height;

            ChartDisplayOptions options = new ChartDisplayOptions();
            options.DisplayTitle     = false;
            options.DisplayLegend    = false;
            options.DisplayGridLines = false;
            options.DisplayAxis      = false;
            options.Height           = height;
            options.Width            = width;
            options.XAxisLabel       = "";
            options.YAxisLabel       = "";
            options.Title            = "";
            options.MarginMax        = 1;
            options.MarginMin        = 0;

            /// 
            /// Previews
            /// 
            Image previewScanClusterNet = RenderDatasetInfo.ScanVsClusterNet_Thumbnail(    mobj_analysis,
                                                                                                mint_datasetIndex,
                                                                                                mpicture_preview.Width,
                                                                                                mpicture_preview.Height,
                                                                                                false, false, false);                                                                                                
                        
            Image previewClusterChart = RenderDatasetInfo.ClusterChart_Thumbnail(mobj_analysis,
                                                                                                mint_datasetIndex,
                                                                                                width,
                                                                                                height,
                                                                                                true,
                                                                                                mint_chargeStates, false, false, false);

            /// ------------------------------------------------------------------------------------
            /// Alignment
            /// ------------------------------------------------------------------------------------            
            Image alignmentPreview = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(         mobj_analysis,
                                                                                                mint_datasetIndex, width, height);
            
            
            Image massErrorHistogram = null;
            Image netErrorHistogram  = null;            
            Image alignedNetResidual = null;            
            Image massResidual       = null;
            Image mzMassResidual     = null;
            Image massNetResidual    = null;

            if (mobj_alignmentData != null)
            {

                massErrorHistogram = RenderDatasetInfo.ErrorHistogram_Thumbnail(mobj_alignmentData.massErrorHistogram, options);
                netErrorHistogram = RenderDatasetInfo.ErrorHistogram_Thumbnail(mobj_alignmentData.netErrorHistogram, options);

                if (mobj_alignmentData.ResidualData != null)
                {
                    //alignedNetResidual = RenderDatasetInfo.Residuals_Thumbnail(mobj_alignmentData.ResidualData.
                    //                                                                            mint_datasetIndex,
                    //                                                                            mpicture_netResiduals.Width,
                    //                                                                            mpicture_netResiduals.Height,
                    //                                                                                false, false, false);


                    //massResidual = RenderDatasetInfo.MassVsScanResiduals_Thumbnail(mobj.options);

                    //mzMassResidual = RenderDatasetInfo.MassVsMZResidual_Thumbnail(mobj_analysis,
                    //                                                                            mint_datasetIndex,
                    //                                                                            mpictureBox_mzMassResidual.Width,
                    //                                                                            mpictureBox_mzMassResidual.Height,
                    //                                                                                false, false, false);


                    //massNetResidual = RenderDatasetInfo.MassNETResiduals_Thumbnail(mobj_analysis,
                    //                                                                        mint_datasetIndex,
                    //                                                                        mpictureBox_massNetResiduals.Width,
                    //                                                                        mpictureBox_massNetResiduals.Height,
                    //                                                                    false, false, false);
                }
            }
           
            Image [] images = new Image[]{  
                                            previewClusterChart,
                                            previewScanClusterNet, 
                                            alignmentPreview,
                                            netErrorHistogram,
                                            massErrorHistogram,
                                            alignedNetResidual ,
                                            massResidual,
                                            mzMassResidual,
                                            massNetResidual          
                                        };

            

            if (InvokeRequired == true)
            {
                BeginInvoke(new DelegateUpdatePreview(PreviewHandler), new object[] { this, images });
            }
            else
            {
                PreviewHandler(this, images);
            }

            GC.Collect();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the dataset information.
        /// </summary>
        public DatasetInformation Dataset
        {
            get
            {
                return mobj_dataset;
            }
            set
            {
                mobj_dataset = value;
                if (value != null)
                {
                    UpdateUserInterface();
                }
            }
        }
        /// <summary>
        /// Gets or sets the alignment options.
        /// </summary>
        public clsAlignmentOptions AlignmentOptions
        {
            get
            {
                return mobj_alignment;
            }
            set
            {
                mobj_alignment = value;
                if (value != null)
                {
                    UpdateUserInterface();
                }
            }
        }
        #endregion

        #region Chart Top Level Displays
        /// <summary>
        /// Dispalys the scan vs net data chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_preview_Click(object sender, EventArgs e)
        {
            string name = "Scan Vs. Cluster NET";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ctlScatterChart chart = RenderDatasetInfo.ScanVsClusterNet_Chart(mobj_analysis, mint_datasetIndex);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }
        /// <summary>
        /// Displays the raw data chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_rawData_Click(object sender, EventArgs e)
        {            
            string name = "Cluster Chart";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterChart_Chart(mobj_analysis, mint_datasetIndex, false, mint_chargeStates);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }
        /// <summary>
        /// Displays the mass error histogram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpictureBox_massErrorHistogram_Click(object sender, EventArgs e)
        {                        
            string name = "Mass Error Histogram";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true);
                options.MarginMin = 100;
                options.MarginMin = 0;
                options.Title = "";
                options.XAxisLabel = "Mass Error (PPM)";
                options.YAxisLabel = "Count";
                                
                controlHistogram chart = RenderDatasetInfo.ErrorHistogram_Chart(mobj_alignmentData.massErrorHistogram, options);
                if (chart == null)
                    return; 

                chart.Name  = name;
                chart.Title = name;
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }
        /// <summary>
        /// Displays the NET error histogram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpictureBox_netErrorHistogram_Click(object sender, EventArgs e)
        {
            string name = "NET Error Histogram";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                double[,] data = mobj_analysis.AlignmentData[mint_datasetIndex].netErrorHistogram;

                
                ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true);
                options.MarginMin = 100;
                options.MarginMin = 0;
                options.Title = "";
                options.XAxisLabel = "NET Error (%)";
                options.YAxisLabel = "Count";

                controlHistogram chart = RenderDatasetInfo.ErrorHistogram_Chart(mobj_alignmentData.netErrorHistogram, options);
                if (chart == null)
                    return;

                chart.Name  = name;
                chart.Title = name;
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }
        /// <summary>
        /// Displays the calibrated mass residual plots from alignment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_massResiduals_Click(object sender, EventArgs e)
        {
            string name = "Mass vs Scan Residuals";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ctlScatterChart chart = RenderDatasetInfo.MassVsScanResiduals_Chart(mobj_analysis, mint_datasetIndex);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }
        /// <summary>
        /// Displays the NET residual chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_netResiduals_Click(object sender, EventArgs e)
        {
            string name = "NET Residuals";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ctlScatterChart chart = RenderDatasetInfo.NETResiduals_Chart(mobj_analysis, mint_datasetIndex);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }
        /// <summary>
        /// Displays the Mass vs M/Z Residual plots.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpictureBox_mzMassResidual_Click(object sender, EventArgs e)
        {
            //string name = "Mass Vs. M/Z Residuals";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.MassVsMZResidual_Chart(mobj_analysis, mint_datasetIndex);
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //} 
        }
        private void mpicture_alignmentHeatmap_Click(object sender, EventArgs e)
        {
            ctlAlignmentHeatMap chart = RenderDatasetInfo.AlignmentHeatMap_Chart(mobj_analysis, mint_datasetIndex);            
            if (chart != null)
            {
                Form displayform = new Form();
                displayform.Size = ParentForm.Size;
                chart.Dock = DockStyle.Fill;
                chart.BackColor = Color.White;

                displayform.Icon = ParentForm.Icon;
                displayform.Controls.Add(chart);
                displayform.Show();
            }
        }

        private void mpictureBox_massNetResiduals_Click(object sender, EventArgs e)
        {
            string name = "Mass and NET Residuals";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ctlScatterChart chart = RenderDatasetInfo.MassNETResiduals_Chart(mobj_analysis, mint_datasetIndex);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }                            
        }
        /// <summary>
        /// Retrieves the form if a form exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Form RetrieveForm(string name)
        {
            if (mdict_forms.ContainsKey(name))
                return mdict_forms[name];
            else return null;
        }
        private Form RegisterChart(string name, Control chart)
        {
            if (chart == null)
                return null;

            if (mdict_forms.ContainsKey(name))
                return mdict_forms[name];

            Form displayform    = new Form();
            displayform.FormClosing += new FormClosingEventHandler(displayform_FormClosing);
            displayform.Size    = ParentForm.Size;
            chart.Dock          = DockStyle.Fill;
            chart.BackColor     = Color.White;
            displayform.Icon    = ParentForm.Icon;
            displayform.Name    = name; 
            displayform.Controls.Add(chart);
            displayform.MdiParent = ParentForm.MdiParent;

            mdict_forms.Add(name, displayform);

            return displayform;
        }

        void displayform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Form form = sender as Form;
                form.Hide();
            }
        }
        #endregion

        /// <summary>
        /// Closes all of the forms that are currenty displayed.
        /// </summary>
        private void CloseAllOpenCharts()
        {
            foreach (string key in mdict_forms.Keys)
            {
                Form f = mdict_forms[key];
                if (f != null)
                    f.Close();
            }

            mdict_forms.Clear();
        }
    }
}

