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
using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin.Forms
{
    /// <summary>
    /// Class that handles rendering the dataset information.
    /// </summary>
    public partial class controlDatasetInformation : UserControl
    {
        /// <summary>
        /// Number of charge states to visually display in cluster chart.
        /// </summary>
        private const int CONST_NUM_CHARGE_STATES = 4;
        /// <summary>
        /// Delegate when the preview is finished rendering.
        /// </summary>
        /// <param name="sender">Dataset that was rendered.</param>
        /// <param name="previews">Preview images</param>
        private delegate void DelegateUpdatePreview(controlDatasetInformation sender, Image[] previews);        
        /// <summary>
        /// Dataset to probe.
        /// </summary>
        private clsDatasetInfo mobj_dataset;
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
        private clsMultiAlignAnalysis mobj_analysis;
        /// <summary>
        /// Index of this dataset in the analysis.
        /// </summary>
        private int mint_datasetIndex;
        /// <summary>
        /// Number of charge states to display on cluster chart.
        /// </summary>
        private int mint_chargeStates;

        /// <summary>
        /// Default constructor for a dataset class.
        /// </summary>
        public controlDatasetInformation(clsMultiAlignAnalysis analysis, int datasetIndex)
        {
            InitializeComponent();

            mobj_dataset        = analysis.Files[datasetIndex] as clsDatasetInfo;
            mobj_alignment      = analysis.AlignmentOptions[datasetIndex] as clsAlignmentOptions;
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
            frmDatasetSummary summary = new frmDatasetSummary(mobj_dataset, mobj_alignment, mobj_cluster);
            summary.ShowDialog();
        }
        /// <summary>
        /// Handles when the user clicks to see the baseline options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_baseline_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {            
            /*frmAlignmentPreview preview = new frmAlignmentPreview(mobj_analysis, mint_datasetIndex);
            preview.ShowDialog();
             **/
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
            mbutton_datasetName.Text = string.Format("{0}", mobj_dataset.mstrDatasetName);
            mbutton_baseline.Text    = Path.GetFileName(mobj_alignment.AlignmentBaselineName);

            clsUMC [] umcs = mobj_analysis.UMCData.GetUMCS(mint_datasetIndex);
            mlabel_umcs.Text = string.Format("{0}", umcs.Length);

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
                                Image mzMassResidual)
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
            if (mobj_dataset.mstrDatasetName == Path.GetFileName(mobj_alignment.AlignmentBaselineName))
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

                mpicture_alignmentHeatmap.Image = image;
                mpictureBox_massErrorHistogram.Image = image;
                mpictureBox_netErrorHistogram.Image = image;
                mpicture_massResiduals.Image = image;
                mpictureBox_mzMassResidual.Image = image;
                mpicture_netResiduals.Image = image;
                mpicture_alignmentHeatmap.Visible = true;
                mlabel_alignment.Visible = true;
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
                        images[7]);
        }
        /// <summary>
        /// Creates the preview images for display.
        /// </summary>
        public void RenderPreviews()
        {
            /// 
            /// Previews
            /// 
            Image previewScanClusterNet = classRenderDatasetInfo.ScanVsClusterNet_Thumbnail(    mobj_analysis,
                                                                                                mint_datasetIndex,
                                                                                                mpicture_preview.Width,
                                                                                                mpicture_preview.Height);
            

            /// 
            /// Previews
            /// 
            Image previewClusterChart = classRenderDatasetInfo.ClusterChart_Thumbnail(          mobj_analysis,
                                                                                                mint_datasetIndex,
                                                                                                mpicture_preview.Width,
                                                                                                mpicture_preview.Height,
                                                                                                true,
                                                                                                mint_chargeStates);

            Image alignmentPreview = classRenderDatasetInfo.AlignmentHeatmap_Thumbnail(         mobj_analysis,
                                                                                                mint_datasetIndex,
                                                                                                mpicture_preview.Width,
                                                                                                mpicture_preview.Height);

            Image massErrorHistogram = classRenderDatasetInfo.MassErrorHistogram_Thumbnail(     mobj_analysis,
                                                                                                mint_datasetIndex,
                                                                                                mpicture_preview.Width,
                                                                                                mpicture_preview.Height);

            Image netErrorHistogram = classRenderDatasetInfo.NETErrorHistogram_Thumbnail(       mobj_analysis,
                                                                                                mint_datasetIndex,
                                                                                                mpicture_preview.Width,
                                                                                                mpicture_preview.Height);

            Image alignedNetResidual = classRenderDatasetInfo.NETResiduals_Thumbnail(mobj_analysis,
                                                                                            mint_datasetIndex,
                                                                                            mpicture_netResiduals.Width,
                                                                                            mpicture_netResiduals.Height);

            Image massResidual    = classRenderDatasetInfo.MassVsScanResiduals_Thumbnail(mobj_analysis,
                                                                                            mint_datasetIndex,
                                                                                            mpicture_massResiduals.Width,
                                                                                            mpicture_massResiduals.Height);
            Image mzMassResidual = classRenderDatasetInfo.MassVsMZResidual_Thumbnail(mobj_analysis,
                                                                                            mint_datasetIndex,
                                                                                            mpictureBox_mzMassResidual.Width,
                                                                                            mpictureBox_mzMassResidual.Height);
    


            Image [] images = new Image[]{  
                                            previewClusterChart,
                                            previewScanClusterNet, 
                                            alignmentPreview,
                                            netErrorHistogram,
                                            massErrorHistogram,
                                            alignedNetResidual ,
                                            massResidual,
                                            mzMassResidual                           
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
        public clsDatasetInfo Dataset
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
            ctlScatterChart chart = classRenderDatasetInfo.ScanVsClusterNet_Chart(mobj_analysis, mint_datasetIndex);

            if (chart != null)
            {
                Form displayform = new Form();
                displayform.Size = ParentForm.Size;
                chart.Title = mobj_dataset.mstrDatasetName;
                chart.Dock = DockStyle.Fill;
                chart.BackColor = Color.White;
                displayform.Icon = ParentForm.Icon;
                displayform.Controls.Add(chart);
                displayform.ShowDialog();
            }
        }
        /// <summary>
        /// Displays the raw data chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_rawData_Click(object sender, EventArgs e)
        {
            ctlScatterChart chart = classRenderDatasetInfo.ClusterChart_Chart(mobj_analysis, mint_datasetIndex, false, mint_chargeStates);

            if (chart != null)
            {
                Form displayform    = new Form();
                displayform.Size    = ParentForm.Size;
                chart.Title         = mobj_dataset.mstrDatasetName;
                chart.Dock          = DockStyle.Fill;
                chart.BackColor     = Color.White;
                displayform.Icon    = ParentForm.Icon;
                displayform.Controls.Add(chart);
                displayform.ShowDialog();
            }
        }
        /// <summary>
        /// Displays the mass error histogram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpictureBox_massErrorHistogram_Click(object sender, EventArgs e)
        {
            controlHistogram chart = classRenderDatasetInfo.MassErrorHistogram_Chart(mobj_analysis, mint_datasetIndex);
            if (chart != null)
            {
                Form displayform = new Form();
                displayform.Text = "Mass Error Histogram " + mobj_dataset.mstrDatasetName;
                displayform.Size = ParentForm.Size;
                chart.Title      = "Mass Error Histogram ";
                chart.Dock       = DockStyle.Fill;
                chart.BackColor  = Color.White;

                chart.LegendVisible = false;
                displayform.Icon    = ParentForm.Icon;

                displayform.Controls.Add(chart);
                displayform.ShowDialog();
            }
        }
        /// <summary>
        /// Displays the NET error histogram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpictureBox_netErrorHistogram_Click(object sender, EventArgs e)
        {
            controlHistogram chart = classRenderDatasetInfo.NETErrorHistogram_Chart(mobj_analysis, mint_datasetIndex);
            if (chart != null)
            {
                Form displayform = new Form();
                displayform.Text = "NET Error Histogram " + mobj_dataset.mstrDatasetName;
                displayform.Size = ParentForm.Size;
                chart.Title      = "NET Error Histogram ";
                chart.Dock       = DockStyle.Fill;
                chart.BackColor  = Color.White;                
                chart.LegendVisible = false;
                displayform.Icon    = ParentForm.Icon;

                displayform.Controls.Add(chart);
                displayform.ShowDialog();
            }
        }
        /// <summary>
        /// Displays the calibrated mass residual plots from alignment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_massResiduals_Click(object sender, EventArgs e)
        {
            ctlScatterChart chart = classRenderDatasetInfo.MassVsScanResiduals_Chart(mobj_analysis, mint_datasetIndex);

            if (chart != null)
            {
                Form displayform    = new Form();
                displayform.Size    = ParentForm.Size;                
                chart.Dock          = DockStyle.Fill;
                chart.BackColor     = Color.White;
                displayform.Icon    = ParentForm.Icon;
                displayform.Controls.Add(chart);
                displayform.ShowDialog();
            }
        }
        /// <summary>
        /// Displays the NET residual chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_netResiduals_Click(object sender, EventArgs e)
        {
            ctlScatterChart chart = classRenderDatasetInfo.NETResiduals_Chart(mobj_analysis, mint_datasetIndex);
            if (chart != null)
            {
                Form displayform    = new Form();
                displayform.Size    = ParentForm.Size;
                chart.Dock          = DockStyle.Fill;
                chart.BackColor     = Color.White;
                displayform.Icon    = ParentForm.Icon;
                displayform.Controls.Add(chart);
                displayform.ShowDialog();
            }
        }
        /// <summary>
        /// Displays the Mass vs M/Z Residual plots.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpictureBox_mzMassResidual_Click(object sender, EventArgs e)
        {

            ctlScatterChart chart = classRenderDatasetInfo.MassVsMZResidual_Chart(mobj_analysis, mint_datasetIndex);

            if (chart != null)
            {
                Form displayform = new Form();
                displayform.Size = ParentForm.Size;
                chart.Dock = DockStyle.Fill;
                chart.BackColor = Color.White;
                displayform.Icon = ParentForm.Icon;
                displayform.Controls.Add(chart);
                displayform.ShowDialog();
            }
        }
        #endregion

    }
}

