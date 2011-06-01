using System;
using MultiAlignCustomControls.Charting;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MultiAlign.Charting;
using MultiAlign.Drawing;
using MultiAlignEngine.Alignment;
using PNNLControls;
using PNNLProteomics.Data;
using PNNLProteomics.Data.Alignment;

using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;

using MultiAlignEngine.Features;

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
        private const int NUM_CHARGE_STATES = 10;
        /// <summary>
        /// Delegate when the preview is finished rendering.
        /// </summary>
        /// <param name="sender">Dataset that was rendered.</param>
        /// <param name="previews">Preview images</param>
        private delegate void DelegateUpdatePreview(controlDatasetInformation sender, Image[] previews);        
        /// <summary>
        /// Dataset to probe.
        /// </summary>
        private DatasetInformation m_dataset;
        /// <summary>
        /// Options defining the alignment for this dataset.
        /// </summary>
        private clsAlignmentOptions m_alignmentOptions;        
        /// <summary>
        /// Number of charge states to display on cluster chart.
        /// </summary>
        private int m_chargeStates;
        /// <summary>
        /// Alignment data.
        /// </summary>
        private PNNLProteomics.Data.Alignment.classAlignmentData m_alignmentData;
        /// <summary>
        /// Determines how to load features from the feature cache.
        /// </summary>
        private IUmcDAO m_featureCache;
        /// <summary>
        /// Used to determine what features we can load from the feature cache.
        /// </summary>
        private int m_datasetIndex;
        #endregion

        /// <summary>
        /// Default constructor for a dataset class.
        /// </summary>
        public controlDatasetInformation(DatasetInformation     info,
                                         classAlignmentData     alignmentData,
                                         clsAlignmentOptions    alignmentOptions,
                                         IUmcDAO                featureCache)            
        {
            InitializeComponent();

            AllowTopLevelForms  = false;
            m_dataset           = info;
            m_alignmentOptions  = alignmentOptions;
            m_alignmentData     = alignmentData;            
            m_chargeStates      = NUM_CHARGE_STATES;
            m_featureCache      = featureCache;
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
            DatasetSummary summary = new DatasetSummary(m_dataset, m_alignmentOptions);
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
            if (m_alignmentOptions == null || m_dataset == null)
                return;

            /// 
            /// Dataset information pane
            /// 
            mlabel_datasetID.Text    = string.Format("{0}", m_dataset.mstrDatasetId);
            mbutton_datasetName.Text = string.Format("{0}", m_dataset.DatasetName);
            mbutton_baseline.Text    = Path.GetFileName(m_alignmentOptions.AlignmentBaselineName);
           
            if (m_alignmentData != null)
            {
                mlabel_stdevMass.Text = string.Format("Stdev. Mass = {0:0.000}", m_alignmentData.MassStandardDeviation);
                mlabel_stdevNET.Text  = string.Format("Stdev. NET = {0:0.000}", m_alignmentData.NETStandardDeviation);
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
            if (m_alignmentOptions == null)
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

                mpicture_scanvsNet.Image = image;
                mpicture_alignmentHeatmap.Image = image;
                mpictureBox_massErrorHistogram.Image = image;
                mpictureBox_netErrorHistogram.Image = image;
                mpicture_massResiduals.Image = image;
                mpictureBox_massNetResiduals.Image = image;
                mpictureBox_mzMassResidual.Image = image;
                mpicture_netResiduals.Image = image;
                mpicture_alignmentHeatmap.Visible = true;
                mlabel_alignment.Visible = true;
            }
            else
            {

                if (scanNet != null)
                    mpicture_scanvsNet.Image = scanNet;
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

            if (rawData != null)
                mpicture_rawData.Image = rawData;
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
            options.MarginMax        = 2;
            options.MarginMin        = 1;

            List<clsUMC> features = m_featureCache.FindByDatasetId(m_datasetIndex);

            /// 
            /// Previews
            /// 
            Image previewScanClusterNet = null;
                //RenderDatasetInfo.ScanVsClusterNet_Thumbnail(    mobj_analysis,
                //                                                                                m_datasetIndex,
                //                                                                                mpicture_preview.Width,
                //                                                                                mpicture_preview.Height,
                //                                                                                false, false, false);                                                                                                
                        
            Image previewClusterChart = RenderDatasetInfo.FeatureChart_Thumbnail(   m_dataset,
                                                                                    features,
                                                                                    width,
                                                                                    height,
                                                                                    true,
                                                                                    NUM_CHARGE_STATES, false, false, false);

            /// ------------------------------------------------------------------------------------
            /// Alignment
            /// ------------------------------------------------------------------------------------            
            Image alignmentPreview = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(m_alignmentData, 
                                                                                  width,
                                                                                  height);                        
            Image massErrorHistogram = null;
            Image netErrorHistogram  = null;            
            Image alignedNetResidual = null;            
            Image massResidual       = null;
            Image mzMassResidual     = null;
            Image massNetResidual    = null;

            if (m_alignmentData != null)
            {

                massErrorHistogram = RenderDatasetInfo.ErrorHistogram_Thumbnail(m_alignmentData.massErrorHistogram, options);
                netErrorHistogram = RenderDatasetInfo.ErrorHistogram_Thumbnail(m_alignmentData.netErrorHistogram, options);

                if (m_alignmentData.ResidualData != null)
                {
                    classAlignmentResidualData data = m_alignmentData.ResidualData;         
                    alignedNetResidual  = GenericPlotFactory.Residual_Thumbnail(data.scans, data.customNet, null, false, options);
                    massResidual        = GenericPlotFactory.Residual_Thumbnail(data.scans, data.massError, data.massErrorCorrected, false, options);
                    mzMassResidual      = GenericPlotFactory.Residual_Thumbnail(data.mz, data.mzMassError, data.mzMassErrorCorrected, true, options);
                    massNetResidual     = GenericPlotFactory.Residual_Thumbnail(data.customNet, data.massError, null, false, options);                    
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
        }
        #endregion

        #region Properties
        /// <summary>
        /// Determines if forms can be created as top level objects or if the images are just static.
        /// </summary>
        public bool AllowTopLevelForms
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the dataset information.
        /// </summary>
        public DatasetInformation Dataset
        {
            get
            {
                return m_dataset;
            }
            set
            {
                m_dataset = value;
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
                return m_alignmentOptions;
            }
            set
            {
                m_alignmentOptions = value;
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
            if (!AllowTopLevelForms)
            {
                return;
            }

            //string name = "Scan Vs. Cluster NET";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.ScanVsClusterNet_Chart(_analysis, mint_datasetIndex);
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //}
        }
        /// <summary>
        /// Displays the raw data chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_rawData_Click(object sender, EventArgs e)
        {
            if (!AllowTopLevelForms)
            {
                return;
            }
      
            string name = "Cluster Chart";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {
                List<clsUMC> features = m_featureCache.FindByDatasetId(m_datasetIndex);
                ctlScatterChart chart = RenderDatasetInfo.FeatureChart_Chart(m_dataset, features, false, NUM_CHARGE_STATES);
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
            if (!AllowTopLevelForms)
            {
                return;
            }
                  
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
                                
                controlHistogram chart = RenderDatasetInfo.ErrorHistogram_Chart(m_alignmentData.massErrorHistogram, options);
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
            if (!AllowTopLevelForms)
            {
                return;
            }

            string name = "NET Error Histogram";
            Form displayForm = RetrieveForm(name);

            if (displayForm == null)
            {                
                ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true);
                options.MarginMin   = 100;
                options.MarginMin   = 0;
                options.Title       = "";
                options.XAxisLabel  = "NET Error (%)";
                options.YAxisLabel  = "Count";

                controlHistogram chart = RenderDatasetInfo.ErrorHistogram_Chart(m_alignmentData.netErrorHistogram, options);
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
            if (!AllowTopLevelForms)
            {
                return;
            }

            //string name = "Mass vs Scan Residuals";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.MassVsScanResiduals_Chart(mobj_analysis, mint_datasetIndex);
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //}
        }
        /// <summary>
        /// Displays the NET residual chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpicture_netResiduals_Click(object sender, EventArgs e)
        {
            if (!AllowTopLevelForms)
            {
                return;
            }

            //string name = "NET Residuals";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.NETResiduals_Chart(mobj_analysis, mint_datasetIndex);
            //    displayForm = RegisterChart(name, chart);
            //}
            //if (displayForm != null)
            //{
            //    displayForm.Show();
            //    displayForm.BringToFront();
            //}
        }
        /// <summary>
        /// Displays the Mass vs M/Z Residual plots.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpictureBox_mzMassResidual_Click(object sender, EventArgs e)
        {
            if (!AllowTopLevelForms)
            {
                return;
            }

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
            if (!AllowTopLevelForms)
            {
                return;
            }

            ctlAlignmentHeatMap chart = RenderDatasetInfo.AlignmentHeatMap_Chart(m_alignmentData);            
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
            if (!AllowTopLevelForms)
            {
                return;
            }

            //string name = "Mass and NET Residuals";
            //Form displayForm = RetrieveForm(name);

            //if (displayForm == null)
            //{
            //    ctlScatterChart chart = RenderDatasetInfo.MassNETResiduals_Chart(mobj_analysis, mint_datasetIndex);
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

