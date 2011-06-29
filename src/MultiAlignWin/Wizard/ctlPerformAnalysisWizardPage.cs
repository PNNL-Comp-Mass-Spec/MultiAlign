using System;
using MultiAlignCustomControls.Charting;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using PNNLControls;
using PNNLProteomics.Data;
using PNNLProteomics.MultiAlign;
using MultiAlign.Charting;
using MultiAlign.Drawing;

namespace MultiAlignWin
{
    public class ctlPerformAnalysisWizardPage : UserControl, MultiAlignWin.Forms.Wizard.IWizardControl<PNNLProteomics.Data.MultiAlignAnalysis>
    {
        /// <summary>
        /// 
        /// </summary>
        private MultiAlignAnalysisProcessor m_processor;

        #region Members

        private Label               label1;
        private ProgressBar         mprogressBar_current;
        private IContainer          components = null;        
        private ctlScatterChart     mctlScatterChartFeatures;        
        private PictureBox          mpicture_alignment;
        private MultiAlignAnalysis  m_analysis;
        private ListBox             m_statusMessages;
        private Panel               panel1;
        private Panel               m_plotPanel;
        private Splitter m_splitter;
        private string              m_logPath;
        private Splitter splitter1;
        private PictureBox m_heatmapPicture;
        private PictureBox m_netHistogramPicture;
        private PictureBox m_featuresPlot;
        private Label label4;
        private Label label3;
        private Label label2;
        private PictureBox m_massHistogramPicture;
        #endregion

        /// <summary>
        /// Default constructor for a MA perform analysis wizard page.
        /// </summary>
        public ctlPerformAnalysisWizardPage()
        {
            InitializeComponent();            
        }

        #region Windows Designer and Dispose Interface Implementation
        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.mprogressBar_current = new System.Windows.Forms.ProgressBar();
            this.mpicture_alignment = new System.Windows.Forms.PictureBox();
            this.m_statusMessages = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_plotPanel = new System.Windows.Forms.Panel();
            this.m_massHistogramPicture = new System.Windows.Forms.PictureBox();
            this.m_netHistogramPicture = new System.Windows.Forms.PictureBox();
            this.m_heatmapPicture = new System.Windows.Forms.PictureBox();
            this.m_splitter = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.m_featuresPlot = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_alignment)).BeginInit();
            this.panel1.SuspendLayout();
            this.m_plotPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_massHistogramPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_netHistogramPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_heatmapPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_featuresPlot)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Progress:";
            // 
            // mprogressBar_current
            // 
            this.mprogressBar_current.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mprogressBar_current.ForeColor = System.Drawing.Color.Lime;
            this.mprogressBar_current.Location = new System.Drawing.Point(71, 7);
            this.mprogressBar_current.Name = "mprogressBar_current";
            this.mprogressBar_current.Size = new System.Drawing.Size(922, 18);
            this.mprogressBar_current.TabIndex = 4;
            // 
            // mpicture_alignment
            // 
            this.mpicture_alignment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mpicture_alignment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_alignment.Location = new System.Drawing.Point(541, 292);
            this.mpicture_alignment.Name = "mpicture_alignment";
            this.mpicture_alignment.Size = new System.Drawing.Size(303, 275);
            this.mpicture_alignment.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.mpicture_alignment.TabIndex = 10;
            this.mpicture_alignment.TabStop = false;
            // 
            // m_statusMessages
            // 
            this.m_statusMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_statusMessages.FormattingEnabled = true;
            this.m_statusMessages.Location = new System.Drawing.Point(5, 35);
            this.m_statusMessages.Name = "m_statusMessages";
            this.m_statusMessages.Size = new System.Drawing.Size(1005, 531);
            this.m_statusMessages.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.mprogressBar_current);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1007, 32);
            this.panel1.TabIndex = 7;
            // 
            // m_plotPanel
            // 
            this.m_plotPanel.BackColor = System.Drawing.Color.White;
            this.m_plotPanel.Controls.Add(this.label4);
            this.m_plotPanel.Controls.Add(this.label3);
            this.m_plotPanel.Controls.Add(this.label2);
            this.m_plotPanel.Controls.Add(this.m_featuresPlot);
            this.m_plotPanel.Controls.Add(this.m_massHistogramPicture);
            this.m_plotPanel.Controls.Add(this.m_netHistogramPicture);
            this.m_plotPanel.Controls.Add(this.m_heatmapPicture);
            this.m_plotPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_plotPanel.Location = new System.Drawing.Point(5, 569);
            this.m_plotPanel.Name = "m_plotPanel";
            this.m_plotPanel.Padding = new System.Windows.Forms.Padding(5);
            this.m_plotPanel.Size = new System.Drawing.Size(1005, 207);
            this.m_plotPanel.TabIndex = 8;
            // 
            // m_massHistogramPicture
            // 
            this.m_massHistogramPicture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.m_massHistogramPicture.Location = new System.Drawing.Point(427, 23);
            this.m_massHistogramPicture.Name = "m_massHistogramPicture";
            this.m_massHistogramPicture.Size = new System.Drawing.Size(189, 180);
            this.m_massHistogramPicture.TabIndex = 2;
            this.m_massHistogramPicture.TabStop = false;
            // 
            // m_netHistogramPicture
            // 
            this.m_netHistogramPicture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.m_netHistogramPicture.Location = new System.Drawing.Point(218, 23);
            this.m_netHistogramPicture.Name = "m_netHistogramPicture";
            this.m_netHistogramPicture.Size = new System.Drawing.Size(203, 179);
            this.m_netHistogramPicture.TabIndex = 1;
            this.m_netHistogramPicture.TabStop = false;
            // 
            // m_heatmapPicture
            // 
            this.m_heatmapPicture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.m_heatmapPicture.Location = new System.Drawing.Point(5, 23);
            this.m_heatmapPicture.Name = "m_heatmapPicture";
            this.m_heatmapPicture.Size = new System.Drawing.Size(207, 179);
            this.m_heatmapPicture.TabIndex = 0;
            this.m_heatmapPicture.TabStop = false;
            // 
            // m_splitter
            // 
            this.m_splitter.BackColor = System.Drawing.Color.Silver;
            this.m_splitter.Location = new System.Drawing.Point(3, 35);
            this.m_splitter.Name = "m_splitter";
            this.m_splitter.Size = new System.Drawing.Size(2, 741);
            this.m_splitter.TabIndex = 9;
            this.m_splitter.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.Gray;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(5, 566);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1005, 3);
            this.splitter1.TabIndex = 10;
            this.splitter1.TabStop = false;
            // 
            // m_featuresPlot
            // 
            this.m_featuresPlot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.m_featuresPlot.Location = new System.Drawing.Point(622, 23);
            this.m_featuresPlot.Name = "m_featuresPlot";
            this.m_featuresPlot.Size = new System.Drawing.Size(177, 180);
            this.m_featuresPlot.TabIndex = 3;
            this.m_featuresPlot.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Alignment Heat Map";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(268, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "NET Error Histogram";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(465, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Mass Error Histogram";
            // 
            // ctlPerformAnalysisWizardPage
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.m_statusMessages);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.m_plotPanel);
            this.Controls.Add(this.m_splitter);
            this.Controls.Add(this.panel1);
            this.Name = "ctlPerformAnalysisWizardPage";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(1013, 779);
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_alignment)).EndInit();
            this.panel1.ResumeLayout(false);
            this.m_plotPanel.ResumeLayout(false);
            this.m_plotPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_massHistogramPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_netHistogramPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_heatmapPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_featuresPlot)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return "Performing Analysis"; }
        }
        /// <summary>
        /// 
        /// </summary>
        public PNNLProteomics.Data.MultiAlignAnalysis Data
        {
            get
            {
                return m_analysis;
            }
            set
            {
                m_analysis = value;
            }
        }
        /// <summary>
        /// Gets or sets the analysis object.
        /// </summary>
        public MultiAlignAnalysisProcessor Processor
        {
            get
            {
                return m_processor;
            }
            set
            {
                m_processor = value;    
            
                if (m_processor != null)
                {
                    RegisterAnalysis();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterAnalysis()
        {            
            m_processor.FeaturesAligned     += new EventHandler<FeaturesAlignedEventArgs>(m_processor_FeaturesAligned);
            m_processor.FeaturesClustered   += new EventHandler<FeaturesClusteredEventArgs>(m_processor_FeaturesClustered);
            m_processor.FeaturesLoaded      += new EventHandler<FeaturesLoadedEventArgs>(m_processor_FeaturesLoaded);
            m_processor.FeaturesPeakMatched += new EventHandler<FeaturesPeakMatchedEventArgs>(m_processor_FeaturesPeakMatched);
            m_processor.Status              += new EventHandler<AnalysisStatusEventArgs>(m_processor_Status);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void StatusUpdate(string message)
        {
            string logMessage = BuildLogString(message);
            m_statusMessages.Items.Add(" " + logMessage);
            Log(logMessage);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusUpdate(object sender, AnalysisStatusEventArgs e)
        {
            StatusUpdate(e.StatusMessage);            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusUpdate(object sender, FeaturesAlignedEventArgs e)
        {
            string logMessage = string.Format("Aligned features {0} to {1}",
                                                            e.AligneeDatasetInformation.DatasetName,
                                                            e.BaselineDatasetInformation.DatasetName);
            StatusUpdate(logMessage);
            
            PNNLProteomics.Data.Alignment.classAlignmentData data = e.AlignmentData;
            System.Drawing.Image heatImage =  RenderDatasetInfo.AlignmentHeatmap_Thumbnail(data, 
                                                                                    m_heatmapPicture.Width,
                                                                                    m_heatmapPicture.Height);            
            heatImage.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            m_heatmapPicture.Image       = heatImage;
            
            ChartDisplayOptions options  = new ChartDisplayOptions();
            options.DisplayAxis          = true;
            options.DisplayGridLines     = false;
            options.DisplayTitle         = false;
            options.Height               = m_massHistogramPicture.Height;
            options.Width                = m_massHistogramPicture.Width;
            m_netHistogramPicture.Image  = RenderDatasetInfo.ErrorHistogram_Thumbnail(data.netErrorHistogram, options);
            m_massHistogramPicture.Image = RenderDatasetInfo.ErrorHistogram_Thumbnail(data.massErrorHistogram, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusUpdate(object sender, FeaturesClusteredEventArgs e)
        {
            string logMessage = "Features clustered.";                                                            
            StatusUpdate(logMessage);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusUpdate(object sender, FeaturesPeakMatchedEventArgs e)
        {
            string logMessage = "Features Peak Matched.";
            StatusUpdate(logMessage);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusUpdate(object sender, FeaturesLoadedEventArgs e)
        {
            string logMessage = "Loaded " + e.Features.Count + " LC-MS features.";
            StatusUpdate(logMessage);            
        } 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_processor_Status(object sender, AnalysisStatusEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<AnalysisStatusEventArgs>(StatusUpdate), new object[] { sender, e });
            }
            else
            {
                StatusUpdate(sender, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_processor_FeaturesPeakMatched(object sender, FeaturesPeakMatchedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<FeaturesPeakMatchedEventArgs>(StatusUpdate), new object[] { sender, e });
            }
            else
            {
                StatusUpdate(sender, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_processor_FeaturesLoaded(object sender, FeaturesLoadedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<FeaturesLoadedEventArgs>(StatusUpdate), new object[] { sender, e });
            }
            else
            {
                StatusUpdate(sender, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_processor_FeaturesClustered(object sender, FeaturesClusteredEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<FeaturesClusteredEventArgs>(StatusUpdate), new object[] { sender, e });
            }
            else
            {
                StatusUpdate(sender, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_processor_FeaturesAligned(object sender, FeaturesAlignedEventArgs e)
        {

            if (InvokeRequired)
            {
                Invoke(new EventHandler<FeaturesAlignedEventArgs>(StatusUpdate), new object[] { sender, e });
            }
            else
            {
                StatusUpdate(sender, e);
            }
        }
        private void LogVersion()
        {

            Log("[VersionInfo]");
            // get the version object for this assembly
            Assembly assembly   = Assembly.GetExecutingAssembly();
            AssemblyName name   = assembly.GetName();
            Version version     = name.Version;
            Log(string.Format("{0} - version {1}", name, version));

            AppDomain MyDomain  = AppDomain.CurrentDomain;
            Assembly[] AssembliesLoaded = MyDomain.GetAssemblies();

            Log("Loaded Assemblies");
            foreach (Assembly subAssembly in AssembliesLoaded)
            {
                AssemblyName subName = subAssembly.GetName();
                if (!subName.Equals(name))
                {
                    Log(string.Format("\t{0} - version {1}",
                                                                    subName,
                                                                    subName.Version));
                }
            }

            Log("");
            Log("[LogStart]");
        }
        /// <summary>
        /// Starts a MultiAlign Analysis.
        /// </summary>
        private void StartAnalysis()
        {
            m_statusMessages.Items.Clear();

            string analysisName     = m_analysis.MetaData.AnalysisName;
            string parameterPath = AnalysisPathUtils.BuildParameterPath(m_analysis.MetaData.AnalysisPath, 
                                                                        m_analysis.MetaData.AnalysisName, 
                                                                        ".xml");
            m_logPath               = AnalysisPathUtils.BuildLogPath(   m_analysis.MetaData.AnalysisPath, 
                                                                        m_analysis.MetaData.AnalysisName);

            PNNLProteomics.IO.XMLParameterFileWriter writer = new PNNLProteomics.IO.XMLParameterFileWriter();
            writer.WriteParameterFile(parameterPath, m_analysis);

            LogVersion();
            m_processor.StartAnalysis(m_analysis);
        }
        public void SetAsActivePage()
        {
            StartAnalysis();
        }
        public bool IsComplete()
        {
            return true;
        }

        /// <summary>
        /// Calculates the current usage of current processes memory.
        /// </summary>
        /// <returns>Memory usage of current process.</returns>
        static long GetMemory()
        {
            Process process = Process.GetCurrentProcess();
            long memory = process.WorkingSet64;
            memory /= 1024;
            memory /= 1024;
            process.Dispose();

            return memory;
        }
        string BuildLogString(string message)
        {
            return DateTime.Now.ToString() + " - " + GetMemory().ToString() + " MB - " + message;
        }
        /// <summary>
        /// Logs the data to the stored text file path.
        /// </summary>
        /// <param name="message"></param>
        void Log(string message)
        {
            try
            {
                File.AppendAllText(m_logPath, message + Environment.NewLine);
            }
            catch
            {
                //TODO: Handle this exception?
            }
        }
    }
}

