/*////////////////////////////////////////////////////////////////////////
 *  File  : frmAnalysisWizard.cs
 *  Author: Ashoka Polpitya
 *          Navdeep Jaitly
 *          Brian LaMarche
 *  Date  : 9/11/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      Analysis wizard form for starting and running an analysis.
 * 
 *  Revisions:
 *      9-11-2008:
 *          - Removed the select data source option so data can be loaded
 *              from multiple sources.
 *          - Added this comment header block.
 *      9-12-2008:
 *          - Added a reference to the server information to 
 *              mass tag databases used.
 *      9-16-2008:
 *          - Moved the analysis methods into the clsMultiAlignAnalysisObject
 * 
 *////////////////////////////////////////////////////////////////////////	

using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features;
using MultiAlignWin.Forms.Wizard;


namespace MultiAlignWin
{
    /// <summary>
    /// Form that handles the user interface for setting up analysis through the use of a wizard.
    /// </summary>
	public class frmAnalysisWizard: Form
    {
        public event EventHandler<AnalysisCompleteEventArgs>  AnalysisComplete;
        public event EventHandler<AnalysisErrorEventArgs>     AnalysisError;
        ctlLoadDatasetWizardPage            m_loadDatasetPage;
        DefineFactorsWizardPage             m_defineFactorsPage;
        ctlSelectParametersWizardPage       m_selectParametersPage;
        ctlSelectOutputNameWizardPage       m_selectOutputNamePage;
        ctlPerformAnalysisWizardPage        m_performAnalysisPage;
        IWizardControl<MultiAlignAnalysis>  m_currentPage;


        /// <summary>
        /// Controls acces to the wizard pages.
        /// </summary>
        WizardController<IWizardControl<MultiAlignAnalysis>> m_wizardPageController;

        #region Members
        private clsDMSServerInformation     m_serverInformation;        
        private MultiAlignAnalysis          m_analysis;
        private MultiAlignAnalysisProcessor m_processor;
        private Panel panel1;
        private PictureBox m_sidebar;
        private Button m_backButton;
        private Button m_nextButton;
        private Label m_stepsLabel;
        private Panel m_controlPanel;
        private StatusStrip m_statusBar;
        private ToolStripStatusLabel m_statusLabel;
        private IContainer components           = null;          
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor that starts a new type of analysis.
        /// </summary>
		public frmAnalysisWizard()
		{			
			InitializeComponent();

            m_wizardPageController = new WizardController<IWizardControl<MultiAlignAnalysis>>();

            m_analysis                          = new MultiAlignAnalysis();
            m_analysis.DefaultAlignmentOptions  = MultiAlignWin.Data.AnalysisDefaultOptionsFactory.LoadAlignmentOptions();            
            m_analysis.ClusterOptions           = MultiAlignWin.Data.AnalysisDefaultOptionsFactory.LoadClusterOptions();
            m_analysis.PeakMatchingOptions      = MultiAlignWin.Data.AnalysisDefaultOptionsFactory.LoadPeakMatchingOptions();
            m_analysis.UMCFindingOptions        = MultiAlignWin.Data.AnalysisDefaultOptionsFactory.LoadFeatureFindingOptions();
            m_analysis.MassTagDBOptions         = MultiAlignWin.Data.AnalysisDefaultOptionsFactory.LoadDBOptions();
            m_analysis.STACOptions             = MultiAlignWin.Data.AnalysisDefaultOptionsFactory.LoadSMARTOptions();

            // Create the control pages
            InitializeWizardPages();   
            FormClosing += new FormClosingEventHandler(frmAnalysisWizard_FormClosing);
        }

        private void SetButtonVisibility(bool backButton, bool nextButton)
        {
            m_backButton.Visible = backButton;
            m_nextButton.Visible = nextButton;
        }
        /// <summary>
        /// Initializes the Wizard control pages.
        /// </summary>
        private void InitializeWizardPages()
        {                        
            clsDMSServerInformation info = new clsDMSServerInformation();
            info.ServerName         = Properties.Settings.Default.DMSServerName;
            info.DatabaseName       = Properties.Settings.Default.DMSDatabaseName;
            info.Username           = "dmswebuser";
            info.Password           = "icr4fun";
            info.ConnectionTimeout  = Properties.Settings.Default.DMSConnectionTimeout;
            m_serverInformation     = info;

            m_loadDatasetPage      = new ctlLoadDatasetWizardPage(info);
            m_defineFactorsPage    = new DefineFactorsWizardPage();
            m_selectParametersPage = new ctlSelectParametersWizardPage();
            m_selectOutputNamePage = new ctlSelectOutputNameWizardPage();
            m_performAnalysisPage  = new ctlPerformAnalysisWizardPage();            
                   
            m_selectParametersPage.ServerInformation = m_serverInformation;

            m_loadDatasetPage.Dock       = DockStyle.Fill;
            m_defineFactorsPage.Dock     = DockStyle.Fill;
            m_selectParametersPage.Dock  = DockStyle.Fill;
            m_selectOutputNamePage.Dock  = DockStyle.Fill;
            m_performAnalysisPage.Dock   = DockStyle.Fill;
            
            m_wizardPageController.AddWizardPage(m_loadDatasetPage);
            m_wizardPageController.AddWizardPage(m_defineFactorsPage);
            m_wizardPageController.AddWizardPage(m_selectParametersPage);
            m_wizardPageController.AddWizardPage(m_selectOutputNamePage);
            m_wizardPageController.AddWizardPage(m_performAnalysisPage);

            
            m_loadDatasetPage.Data = m_analysis;
            SetPage(m_loadDatasetPage);
            SetButtonVisibility(false, true);
        }
        #endregion

        #region Properties
        public MultiAlignAnalysis MultiAlignAnalysis
        {
            get
            {
                return m_analysis;
            }
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            m_loadDatasetPage.Dispose();
            if (m_processor != null)
            {
                m_processor.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Generated Code
		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_backButton = new System.Windows.Forms.Button();
            this.m_nextButton = new System.Windows.Forms.Button();
            this.m_sidebar = new System.Windows.Forms.PictureBox();
            this.m_stepsLabel = new System.Windows.Forms.Label();
            this.m_controlPanel = new System.Windows.Forms.Panel();
            this.m_statusBar = new System.Windows.Forms.StatusStrip();
            this.m_statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_sidebar)).BeginInit();
            this.m_statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_backButton);
            this.panel1.Controls.Add(this.m_nextButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(168, 695);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(932, 38);
            this.panel1.TabIndex = 0;
            // 
            // m_backButton
            // 
            this.m_backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_backButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_backButton.Location = new System.Drawing.Point(777, 4);
            this.m_backButton.Name = "m_backButton";
            this.m_backButton.Size = new System.Drawing.Size(72, 27);
            this.m_backButton.TabIndex = 1;
            this.m_backButton.Text = "<<< Back";
            this.m_backButton.UseVisualStyleBackColor = true;
            this.m_backButton.Click += new System.EventHandler(this.m_backButton_Click);
            // 
            // m_nextButton
            // 
            this.m_nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_nextButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_nextButton.Location = new System.Drawing.Point(855, 4);
            this.m_nextButton.Name = "m_nextButton";
            this.m_nextButton.Size = new System.Drawing.Size(72, 27);
            this.m_nextButton.TabIndex = 0;
            this.m_nextButton.Text = "Next >>>";
            this.m_nextButton.UseVisualStyleBackColor = true;
            this.m_nextButton.Click += new System.EventHandler(this.m_nextButton_Click);
            // 
            // m_sidebar
            // 
            this.m_sidebar.BackgroundImage = global::MultiAlignWin.Properties.Resources.ExampleSidebar;
            this.m_sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.m_sidebar.Location = new System.Drawing.Point(3, 3);
            this.m_sidebar.Name = "m_sidebar";
            this.m_sidebar.Size = new System.Drawing.Size(165, 730);
            this.m_sidebar.TabIndex = 1;
            this.m_sidebar.TabStop = false;
            // 
            // m_stepsLabel
            // 
            this.m_stepsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_stepsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_stepsLabel.Location = new System.Drawing.Point(168, 3);
            this.m_stepsLabel.Name = "m_stepsLabel";
            this.m_stepsLabel.Size = new System.Drawing.Size(932, 42);
            this.m_stepsLabel.TabIndex = 0;
            this.m_stepsLabel.Text = "Steps:";
            this.m_stepsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_controlPanel
            // 
            this.m_controlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_controlPanel.Location = new System.Drawing.Point(168, 45);
            this.m_controlPanel.Name = "m_controlPanel";
            this.m_controlPanel.Size = new System.Drawing.Size(932, 650);
            this.m_controlPanel.TabIndex = 2;
            // 
            // m_statusBar
            // 
            this.m_statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_statusLabel});
            this.m_statusBar.Location = new System.Drawing.Point(3, 733);
            this.m_statusBar.Name = "m_statusBar";
            this.m_statusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.m_statusBar.Size = new System.Drawing.Size(1097, 22);
            this.m_statusBar.TabIndex = 3;
            this.m_statusBar.Text = "statusStrip1";
            // 
            // m_statusLabel
            // 
            this.m_statusLabel.Name = "m_statusLabel";
            this.m_statusLabel.Size = new System.Drawing.Size(42, 17);
            this.m_statusLabel.Text = "Ready.";
            // 
            // frmAnalysisWizard
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1103, 758);
            this.Controls.Add(this.m_controlPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_stepsLabel);
            this.Controls.Add(this.m_sidebar);
            this.Controls.Add(this.m_statusBar);
            this.Name = "frmAnalysisWizard";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MultiAlign Analysis Wizard";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_sidebar)).EndInit();
            this.m_statusBar.ResumeLayout(false);
            this.m_statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
        #endregion

        #region Form Event Handlers
        /// <summary>
        /// Handles when the user tries to close the form while performing a dataset analysis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void frmAnalysisWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            /// 
            /// Only allow the user to cancel closing if the close reason is by clicking on the close button.
            /// 
            if (e.CloseReason == CloseReason.UserClosing)
            {                
                // Make sure the user wants to stop the analysis
                if (m_processor != null)
                {
                    bool stop = StopAnalysis();
                    if (stop == false)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        e.Cancel = false;
                        return;
                    }
                }
                else
                {
                    // Make sure the user does in fact want to close and stop the process at hand.                    
                    CancelEventArgs cancelArgs = new CancelEventArgs(false);                    
                    if (cancelArgs.Cancel == true)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        e.Cancel = false;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Stops the analysis from running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool StopAnalysis()
        {
            try
            {
                if (m_processor != null)
                {
                    m_processor.StopAnalysis();
                }
                return true;
            }
            catch (ThreadAbortException abortException)
            {
                return true;
                // This is expected.
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Stops the analysis from running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartAnalysis()
        {
            if (!Directory.Exists(m_analysis.MetaData.AnalysisPath))
            {
                Directory.CreateDirectory(m_analysis.MetaData.AnalysisPath);
            }     

            AlgorithmBuilder builder            = new AlgorithmBuilder();                     
            m_processor                         = new MultiAlignAnalysisProcessor();
            m_processor.AnalysisComplete        += new EventHandler<AnalysisCompleteEventArgs>(m_processor_AnalysisComplete);
            m_processor.AnalysisError           += new EventHandler<AnalysisErrorEventArgs>(m_processor_AnalysisError);
            m_analysis.DataProviders            = DataAccessFactory.CreateDataAccessProviders(m_analysis);
            m_processor.AlgorithmProvders       = builder.GetAlgorithmProvider();
            m_performAnalysisPage.Processor     = m_processor;
        }
        
        #region Control Button Event Handlers
        private void m_backButton_Click(object sender, EventArgs e)
        {
            MoveLastPage();
        }
        private void m_nextButton_Click(object sender, EventArgs e)
        {
            MoveNextPage();
        }
        private void MoveNextPage()
        {
            bool isLast = m_wizardPageController.IsLast();
            if (isLast)
            {
                StopAnalysis();
                m_nextButton.Text = "Next";
                SetButtonVisibility(true, true);
                MoveLastPage();
                return;
            }

            bool isComplete = m_currentPage.IsComplete();
            if (!isComplete)
            {
                m_statusLabel.Text = "Please fill out all data to proceed.";
                return;
            }
            m_statusLabel.Text = "";

            SetButtonVisibility(true, true);

            IWizardControl<MultiAlignAnalysis> currentPage = m_wizardPageController.GetNext();

            isLast = m_wizardPageController.IsLast();
            if (isLast)
            {
                StartAnalysis();
                m_nextButton.Text = "Stop";
                SetButtonVisibility(false, true);
            }

            SetPage(currentPage);
            Refresh();
        }
        private void MoveLastPage()
        {
            m_statusLabel.Text = "";

            // We are already at the first page?
            bool isFirst = m_wizardPageController.IsFirst();
            if (isFirst)
            {
                SetButtonVisibility(false, true);
                return;
            }

            IWizardControl<MultiAlignAnalysis>  currentPage = m_wizardPageController.GetPrevious();
            if (currentPage != null)
            {
                SetPage(currentPage);
            }

            // Did we just move to the first page?
            isFirst = m_wizardPageController.IsFirst();
            if (isFirst)
            {
                SetButtonVisibility(false, true);
            }
            else
            {
                SetButtonVisibility(true, true);
            }
        }
        private void SetPage(IWizardControl<MultiAlignAnalysis> currentPage)
        {
            if (currentPage != null)
            {
                // Remove old.
                if (m_currentPage != null)
                {
                    m_controlPanel.Controls.Remove(m_currentPage as Control);
                    currentPage.Data = m_currentPage.Data;
                }

                m_stepsLabel.Text = currentPage.Title;

                // Add New.
                Control currentControl  = currentPage as Control;
                currentControl.Dock     = DockStyle.Fill;
                currentControl.BringToFront();

                m_controlPanel.Controls.Add(currentControl);
                m_currentPage = currentPage;
                Refresh();
                m_currentPage.SetAsActivePage();
            }
        }

        #endregion

        private void m_processor_AnalysisComplete(object sender, AnalysisCompleteEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<AnalysisCompleteEventArgs>(m_processor_AnalysisComplete), new object[] { sender, e });
            }
            else
            {
                if (AnalysisComplete != null)
                {
                    AnalysisComplete(this, e);
                }
                Close();
            }
        }
        private void m_processor_AnalysisError(object sender, AnalysisErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<AnalysisErrorEventArgs>(m_processor_AnalysisError), new object[] { sender, e });
            }
            else
            {
                if (AnalysisError != null)
                {
                    AnalysisError(this, e);
                }
                Close();
            }
        }
    }
}

