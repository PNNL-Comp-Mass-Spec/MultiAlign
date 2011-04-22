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

using MultiAlign.Charting;
using MultiAlign.Drawing;

namespace MultiAlignWin.Forms
{
    public partial class controlClusterInformation : UserControl
    {
        #region Members
        /// <summary>
        /// Forms displayed by preview.
        /// </summary>
        private Dictionary<string, Form> mdict_forms = new Dictionary<string, Form>();
        /// <summary>
        /// Number of charge states to visually display in cluster chart.
        /// </summary>
        private const int CONST_NUM_CHARGE_STATES = 4;
        /// <summary>
        /// Delegate when the preview is finished rendering.
        /// </summary>
        /// <param name="sender">Dataset that was rendered.</param>
        /// <param name="previews">Preview images</param>
        private delegate void DelegateUpdatePreview(Image[] previews);                
        /// <summary>
        /// Options used to cluster this dataset.
        /// </summary>
        private clsClusterOptions mobj_cluster;
        /// <summary>
        /// Object that holds all of the analysis data.
        /// </summary>
        private MultiAlignAnalysis mobj_analysis;        
        #endregion

        public controlClusterInformation(MultiAlignAnalysis analysis)
        {
            InitializeComponent();
            
            mobj_cluster        = analysis.ClusterOptions;
            mobj_analysis       = analysis;

            UpdateUserInterface();            
        }

        #region Renderers

        /// <summary>
        /// Updates the user interface with the appropiate data.
        /// </summary>
        private void UpdateUserInterface()
        {
            if (mobj_cluster == null)
                return;

            SuspendLayout();
            /// 
            /// Update the peak matching display first due to rendering z-order.
            /// 
            if (mobj_analysis.PeakMatchedToMassTagDB == false)
            {
                m_peakMatchingPanel.Visible = false;
                m_peakMatchingPanel.BringToFront();
            }
            else
            {
                m_peakMatchingPanel.Visible = true;
                m_peakMatchingPanel.SendToBack();
            }

            if (mobj_analysis.ClusterAlignmentData != null)
            {
                mlabel_stdevMass.Text = string.Format("Stdev. Mass = {0:0.000}", mobj_analysis.ClusterAlignmentData.MassStandardDeviation);
                mlabel_stdevNET.Text = string.Format("Stdev. NET = {0:0.000}", mobj_analysis.ClusterAlignmentData.NETStandardDeviation);
                m_alignmentPanel.Visible = true;
                m_alignmentPanel.SendToBack();
            }
            else
            {
                m_alignmentPanel.Visible = false;
                m_alignmentPanel.BringToFront();
            }

            if (!mobj_analysis.PeakMatchingOptions.UseSTAC)
            {
                mpictureBox_SMARTScoreHistogram.Visible = false;
                mpictureBox_SMARTScoreHistogram.Enabled = false;
                mlabel_stacScoreHistogram.Visible = false;
                mlabel_stacScoreHistogram.Enabled = false;
            }

            m_clusterPanel.SendToBack();

            ResumeLayout();
            PerformLayout();
        }
        /// <summary>
        /// Updates the previews with the preview images.
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="scanNet"></param>
        /// <param name="alignment"></param>
        public void SetImages(  Image netResidual,
                                Image alignment,
                                Image netErrorHistogram,
                                Image massErrorHistogram,
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
                      
            if (netErrorHistogram != null)
                mpictureBox_netErrorHistogram.Image = netErrorHistogram;
            if (massErrorHistogram != null)
                mpictureBox_massErrorHistogram.Image = massErrorHistogram;            
            if (massResidual != null)
                mpicture_massResiduals.Image = massResidual;
            if (mzMassResidual != null)
                mpictureBox_mzMassResidual.Image = mzMassResidual;
            if (massNetResidual != null)
                mpictureBox_massNetResiduals.Image = massNetResidual;

            if (netResidual != null)
                mpicture_preview.Image = netResidual;

        }
        // <summary>
        /// Updates the previews with the preview images.
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="scanNet"></param>
        /// <param name="alignment"></param>
        public void SetClusterImages(Image histogram, Image score, Image sizeHistogram, Image chargeState)  
        {            
            if (histogram != null)
                mpictureBox_scoreHistograms.Image    = histogram;

            if (score != null)
                mpictureBox_scoreVsClusterSize.Image = score;

            if (sizeHistogram != null)
                mpictureBox_clusterSizeHistogram.Image = sizeHistogram;

            if (chargeState != null)
                mpictureBox_chargeStateHistogram.Image = chargeState;
        }
        /// <summary>
        /// Handles rendering images when the control is finished rendering.
        /// </summary>
        /// <param name="dataset">Caller dataset, this</param>
        /// <param name="images">Images to render.</param>
        public void AlignmentPreviewHandler(Image[] images)
        {
            SetImages(  images[0],
                        images[1],
                        images[2],
                        images[3],
                        images[4],
                        images[5],
                        images[6]);
        }

        /// <summary>
        /// Handles rendering images when the control is finished rendering.
        /// </summary>
        /// <param name="dataset">Caller dataset, this</param>
        /// <param name="images">Images to render.</param>
        public void ClusterPreviewHandler(Image[] images)
        {
            SetClusterImages(images[0],
                             images[1],
                             images[2],
                             images[3]);
        }// <summary>
        /// Updates the previews with the preview images.
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="scanNet"></param>
        /// <param name="alignment"></param>
        public void SetPeakMatchImages(Image massNetResidualPlot, Image unidentified, Image smartHistogram, Image identified)
        {
            if (massNetResidualPlot != null)
                mpictureBox_peakMatchingResiduals.Image = massNetResidualPlot;
            if (unidentified != null)
                mpictureBox_unidentifiedScatterPlot.Image = unidentified;
            if (smartHistogram != null)
                mpictureBox_SMARTScoreHistogram.Image = smartHistogram;
            if (identified != null)
                mpictureBox_identifiedFeatures.Image = identified;
        }
        /// <summary>
        /// Handles rendering images when the control is finished rendering.
        /// </summary>
        /// <param name="dataset">Caller dataset, this</param>
        /// <param name="images">Images to render.</param>
        public void PeakMatchPreviewHandler(Image[] images)
        {
            SetPeakMatchImages(images[0], images[1], images[2], images[3]);

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

            if (mobj_analysis.ClusterAlignmentData != null)
            {
                /// ------------------------------------------------------------------------------------
                /// Alignment
                /// ------------------------------------------------------------------------------------            

                Image alignmentPreview = RenderDatasetInfo.ClusterAlignmentHeatmap_Thumbnail(mobj_analysis,
                                                                                           width,
                                                                                           height);

                /// ------------------------------------------------------------------------------------
                /// Mass error histogram
                /// ------------------------------------------------------------------------------------   

                Image massErrorHistogram = RenderDatasetInfo.ClusterMassErrorHistogram_Thumbnail(mobj_analysis,
                                                                                                width,
                                                                                                height,
                                                                                                false, false, false);



                /// ------------------------------------------------------------------------------------
                /// NET error histogram
                /// ------------------------------------------------------------------------------------   

                Image netErrorHistogram = RenderDatasetInfo.ClusterNETErrorHistogram_Thumbnail(mobj_analysis,
                                                                                                    width,
                                                                                                    height,
                                                                                                    false, false, false);


                /// ------------------------------------------------------------------------------------
                /// Residual Net error histogram
                /// ------------------------------------------------------------------------------------   
                Image netResidual = RenderDatasetInfo.ClusterNETResiduals_Thumbnail(mobj_analysis,
                                                                                                mpicture_preview.Width,
                                                                                                mpicture_preview.Height,
                                                                                                    false, false, false);


                /// ------------------------------------------------------------------------------------
                /// Mass Residual histogram
                /// ------------------------------------------------------------------------------------   
                Image massResidual = RenderDatasetInfo.ClusterMassVsScanResiduals_Thumbnail(mobj_analysis,
                                                                                                 width, height,
                                                                                                 false, false, false);

                /// ------------------------------------------------------------------------------------
                /// M/Z Residual
                /// ------------------------------------------------------------------------------------                            
                Image mzMassResidual = RenderDatasetInfo.ClusterMassVsMZResidual_Thumbnail(mobj_analysis,
                                                                                                mpictureBox_mzMassResidual.Width,
                                                                                                mpictureBox_mzMassResidual.Height,
                                                                                                    false, false, false);


                Image massNetResidual = RenderDatasetInfo.ClusterMassNETResiduals_Thumbnail(mobj_analysis,
                                                                                            mpictureBox_massNetResiduals.Width,
                                                                                            mpictureBox_massNetResiduals.Height,
                                                                                            false, false, false);

                /// 
                /// Display low-res images 
                /// 
                Image[] images = new Image[]{   
                                                netResidual,
                                                alignmentPreview,
                                                netErrorHistogram,
                                                massErrorHistogram,
                                                massResidual,
                                                mzMassResidual,
                                                massNetResidual          
                                            };



                if (InvokeRequired == true)
                {
                    BeginInvoke(new DelegateUpdatePreview(AlignmentPreviewHandler), new object[] { images });
                }
                else
                {
                    AlignmentPreviewHandler(images);
                }
            }
            else
            {
                //Disable the cluster alignment displays.
            }



            //if (mobj_analysis.UMCData.mobjClusterData != null)
            //{
            //    Image scoreHistogram = RenderDatasetInfo.ClusterScoreHistogram_Thumbnail(mobj_analysis,
            //                                                                                  mpictureBox_scoreHistograms.Width,
            //                                                                                  mpictureBox_scoreHistograms.Height,
            //                                                                                  false,
            //                                                                                  false,
            //                                                                                  false);

            //    Image chargeStateHistogram = RenderDatasetInfo.ChargeStateHistogram_Thumbnail(mobj_analysis.GetAllUMCS(),
            //                                                                                  mpictureBox_scoreHistograms.Width,
            //                                                                                  mpictureBox_scoreHistograms.Height,
            //                                                                                  new ChartDisplayOptions(false, false, false, false));

            //    Image clusterSizeHistogram = RenderDatasetInfo.ClusterSizeHistogram_Thumbnail(mobj_analysis.GetClusters(),
            //                                                                                  mpictureBox_scoreHistograms.Width,
            //                                                                                  mpictureBox_scoreHistograms.Height,
            //                                                                                  new ChartDisplayOptions(false, false, false, false));
            //    Image scoreSize = RenderDatasetInfo.ClusterScoreVsClusterSize_Thumbnail(mobj_analysis,
            //                                                                                  mpictureBox_clusterSizeHistogram.Width,
            //                                                                                  mpictureBox_clusterSizeHistogram.Height,
            //                                                                                  false,
            //                                                                                  false,
            //                                                                                  false);


            //    Image[] images = new Image[4] {scoreHistogram, scoreSize, clusterSizeHistogram, chargeStateHistogram};
            //    if (InvokeRequired == true)
            //    {
            //        BeginInvoke(new DelegateUpdatePreview(ClusterPreviewHandler), new object[] { images });
            //    }
            //    else
            //    {
            //        ClusterPreviewHandler(images);
            //    }
            //}

            if (mobj_analysis.PeakMatchedToMassTagDB)
            {
                //Image peakMatchPlot = RenderDatasetInfo.PeakMatchMassNET_Thumbnail(mobj_analysis,
                //                                                                              mpictureBox_scoreHistograms.Width,
                //                                                                              mpictureBox_scoreHistograms.Height,
                //                                                                              false,
                //                                                                              false,
                //                                                                              false);

                //Image unidentified = RenderDatasetInfo.UnidentifiedFeatures_Thumbnail(mobj_analysis,
                //                                                                                mpictureBox_unidentifiedScatterPlot.Width,
                //                                                                                mpictureBox_unidentifiedScatterPlot.Height,
                //                                                                                false,
                //                                                                                false,
                //                                                                                false,
                //                                                                                .7);

                //Image identified = RenderDatasetInfo.IdentifiedFeatures_Thumbnail(mobj_analysis,
                //                                                                                mpictureBox_identifiedFeatures.Width,
                //                                                                                mpictureBox_identifiedFeatures.Height,
                //                                                                                false,
                //                                                                                false,
                //                                                                                false,
                //                                                                                .7);


                //Image smartHistogram = RenderDatasetInfo.SMARTScoreHistogram_Thumbnail(mobj_analysis,
                //                                                                                mpictureBox_unidentifiedScatterPlot.Width,
                //                                                                                mpictureBox_unidentifiedScatterPlot.Height,
                //                                                                                false,
                //                                                                                false,
                //                                                                                false);


                //Image[] images = new Image[4] { peakMatchPlot, unidentified, smartHistogram, identified};
                //if (InvokeRequired == true)
                //{
                //    BeginInvoke(new DelegateUpdatePreview(PeakMatchPreviewHandler), new object[] { images });
                //}
                //else
                //{
                //    PeakMatchPreviewHandler(images);
                //}
            }
        }
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
        #endregion

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
                controlHistogram chart = RenderDatasetInfo.ClusterMassErrorHistogram_Chart(mobj_analysis);
                if (chart == null)
                    return;

                chart.Name = name;
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
                controlHistogram chart = RenderDatasetInfo.ClusterNETErrorHistogram_Chart(mobj_analysis);
                if (chart == null)
                    return;

                chart.Name = name;
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
                ctlScatterChart chart = RenderDatasetInfo.ClusterMassVsScanResiduals_Chart(mobj_analysis);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }
        private void mpicture_alignmentHeatmap_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Displays the Mass vs M/Z Residual plots.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpictureBox_mzMassResidual_Click(object sender, EventArgs e)
        {
            string name = "Mass Vs. M/Z Residuals";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterMassVsMZResidual_Chart(mobj_analysis);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }

        private void mpicture_preview_Click(object sender, EventArgs e)
        {
            string name = "NET Residuals";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterNETResiduals_Chart(mobj_analysis);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }

        private void mpictureBox_massNetResiduals_Click(object sender, EventArgs e)
        {
            string name = "Mass and NET Residuals";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                ctlScatterChart chart = RenderDatasetInfo.ClusterMassNETResiduals_Chart(mobj_analysis);
                displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }

        private void mpictureBox_scoreHistograms_Click(object sender, EventArgs e)
        {
            string name = "Cluster Score Historgram";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                //controlHistogram chart = RenderDatasetInfo.ClusterScoreHistogram_Chart(mobj_analysis.UMCData.mobjClusterData);
                //displayForm = RegisterChart(name, chart);
            }
            if (displayForm != null)
            {
                displayForm.Show();
                displayForm.BringToFront();
            }
        }

        private void mpictureBox_scoreVsClusterSize_Click(object sender, EventArgs e)
        {

            //string name = "Cluster Score Vs. Cluster Size";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.ClusterScoreVsClusterSize_Chart(mobj_analysis.UMCData.mobjClusterData);
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //}
        }

        private void mpictureBox_clusterSizeHistogram_Click(object sender, EventArgs e)
        {

            //string name = "Cluster Size Histogram";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ChartDisplayOptions options = new ChartDisplayOptions();
            //    options.Title = "Cluster Size Histogram";
            //    options.YAxisLabel = "Count";
            //    options.XAxisLabel = "Cluster Size";
            //    controlHistogram chart = RenderDatasetInfo.ClusterSizeHistogram_Chart(mobj_analysis.GetClusters(), options);
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //} 
        }

        private void mpictureBox_peakMatchingResiduals_Click(object sender, EventArgs e)
        {

            //string name = "Peak Matching Residuals";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.PeakMatchMassNET_Chart(mobj_analysis); 
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //}
        }

        private void mpictureBox_SMARTScoreHistogram_Click(object sender, EventArgs e)
        {
            
            //string name = "STAC Score Histogram";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    controlHistogram chart = RenderDatasetInfo.SMARTScoreHistogram_Chart(mobj_analysis);
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //}
             
        }

        private void mpictureBox_SMARTScoreSpatialPlot_Click(object sender, EventArgs e)
        {

            //string name = "STAC Score Sptial Plot";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.UnidentifiedFeatures_Chart(mobj_analysis, .5);
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //}              
        }

        private void mpictureBox_chargeStateHistogram_Click(object sender, EventArgs e)
        {
        //{            
        //    string name = "Charge State Histogram"; 
        //    Form displayForm = RetrieveForm(name);

        //    if (displayForm == null)
        //    {
        //        List<clsUMC> umcs = mobj_analysis.GetAllUMCS();
        //        controlHistogram chart = RenderDatasetInfo.ChargeStateHistogram_Chart(umcs);
        //        displayForm = RegisterChart(name, chart);
        //    }

        //    if (displayForm != null)
        //    {
        //        displayForm.Show();
        //        displayForm.BringToFront();
        //    }

        }
        private void mpictureBox_identifiedFeatures_Click(object sender, EventArgs e)
        {

            //string name = "Identified Clusters";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.IdentifiedFeatures_Chart(mobj_analysis, .7);
            //    displayForm = RegisterChart(name, chart);
            //}

            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //}
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

            Form displayform = new Form();
            displayform.FormClosing += new FormClosingEventHandler(displayform_FormClosing);
            displayform.Size = ParentForm.Size;
            chart.Dock = DockStyle.Fill;
            chart.BackColor = Color.White;
            displayform.Icon = ParentForm.Icon;
            displayform.Name = name;
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

    }
}
