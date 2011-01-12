using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

using PNNLControls;
using PNNLProteomics.EventModel;
using MultiAlign.Charting;

namespace MultiAlignWin
{
	public class ctlPerformAnalysisWizardPage : Wizard.UI.InternalWizardPage
    {
        private delegate void MethodInvokerString(string message);

        #region Members        
        private Label ProgressBarLabel;
        private Label label1;
        private ProgressBar mprogressBar_current;
        private TabControl mtabs_messages;
        private TabPage tabPgMessages;
        private TreeView mtreeview_statusMessages;
        private IContainer components = null;
        #endregion        
        private ExternalControls.controlStepOverview mcontrol_steps;
        private ctlScatterChart mctlScatterChartFeatures;
        private ctlAlignmentHeatMap mcontrol_heatMap;
        private PictureBox mpicture_alignment;

        public delegate void DelegateControlLoaded();
        public event DelegateControlLoaded ReadyForAnalysis;
        private bool mbool_hackLoaded = false;

                
        /// <summary>
        /// Default constructor for a MA perform analysis wizard page.
        /// </summary>
        public ctlPerformAnalysisWizardPage( )
        {
            InitializeComponent();
            
            SetActive                   += new System.ComponentModel.CancelEventHandler(ctlPerformAnalysisWizardPage_SetActive);
            TabPage current               = mtabs_messages.SelectedTab;
            mtabs_messages.SelectedTab    = current;                                 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);


            if (mbool_hackLoaded == false)
            {
                mbool_hackLoaded = true;
                if (ReadyForAnalysis != null)
                    ReadyForAnalysis();
            }
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
            PNNLControls.PenProvider penProvider1 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider2 = new PNNLControls.PenProvider();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlPerformAnalysisWizardPage));
            this.ProgressBarLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mprogressBar_current = new System.Windows.Forms.ProgressBar();
            this.mtabs_messages = new System.Windows.Forms.TabControl();
            this.tabPgMessages = new System.Windows.Forms.TabPage();
            this.mpicture_alignment = new System.Windows.Forms.PictureBox();
            this.mctlScatterChartFeatures = new PNNLControls.ctlScatterChart();
            this.mtreeview_statusMessages = new System.Windows.Forms.TreeView();
            this.mcontrol_steps = new ExternalControls.controlStepOverview();
            this.mcontrol_heatMap = new ctlAlignmentHeatMap();
            this.mtabs_messages.SuspendLayout();
            this.tabPgMessages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_alignment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mctlScatterChartFeatures)).BeginInit();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(879, 64);
            this.Banner.Subtitle = "Create UMCs, Cluster, and Save";
            this.Banner.Title = "Step5. Performing MultiAlign Analysis";
            // 
            // ProgressBarLabel
            // 
            this.ProgressBarLabel.Location = new System.Drawing.Point(140, 67);
            this.ProgressBarLabel.Name = "ProgressBarLabel";
            this.ProgressBarLabel.Size = new System.Drawing.Size(87, 19);
            this.ProgressBarLabel.TabIndex = 5;
            this.ProgressBarLabel.Text = "Overall Progress:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(140, 139);
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
            this.mprogressBar_current.Location = new System.Drawing.Point(232, 139);
            this.mprogressBar_current.Name = "mprogressBar_current";
            this.mprogressBar_current.Size = new System.Drawing.Size(761, 16);
            this.mprogressBar_current.TabIndex = 4;
            this.mprogressBar_current.Click += new System.EventHandler(this.mprogressBar_current_Click);
            // 
            // mtabs_messages
            // 
            this.mtabs_messages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtabs_messages.Controls.Add(this.tabPgMessages);
            this.mtabs_messages.Location = new System.Drawing.Point(143, 170);
            this.mtabs_messages.Name = "mtabs_messages";
            this.mtabs_messages.SelectedIndex = 0;
            this.mtabs_messages.Size = new System.Drawing.Size(870, 606);
            this.mtabs_messages.TabIndex = 7;
            // 
            // tabPgMessages
            // 
            this.tabPgMessages.Controls.Add(this.mpicture_alignment);
            this.tabPgMessages.Controls.Add(this.mctlScatterChartFeatures);
            this.tabPgMessages.Controls.Add(this.mtreeview_statusMessages);
            this.tabPgMessages.Controls.Add(this.mcontrol_heatMap);
            this.tabPgMessages.Location = new System.Drawing.Point(4, 22);
            this.tabPgMessages.Name = "tabPgMessages";
            this.tabPgMessages.Padding = new System.Windows.Forms.Padding(10);
            this.tabPgMessages.Size = new System.Drawing.Size(862, 580);
            this.tabPgMessages.TabIndex = 0;
            this.tabPgMessages.Text = "Messages";
            this.tabPgMessages.UseVisualStyleBackColor = true;
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
            // mctlScatterChartFeatures
            // 
            this.mctlScatterChartFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mctlScatterChartFeatures.AutoViewPortXBase = 0F;
            this.mctlScatterChartFeatures.AutoViewPortYBase = 0F;
            this.mctlScatterChartFeatures.AxisAndLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.mctlScatterChartFeatures.AxisAndLabelMaxFontSize = 15;
            this.mctlScatterChartFeatures.AxisAndLabelMinFontSize = 8;
            this.mctlScatterChartFeatures.AxisVisible = true;
            this.mctlScatterChartFeatures.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mctlScatterChartFeatures.ChartBackgroundColor = System.Drawing.Color.White;
            this.mctlScatterChartFeatures.ChartLayout.LegendFraction = 0.2F;
            this.mctlScatterChartFeatures.ChartLayout.LegendLocation = PNNLControls.ChartLegendLocation.Right;
            this.mctlScatterChartFeatures.ChartLayout.MaxLegendHeight = 150;
            this.mctlScatterChartFeatures.ChartLayout.MaxLegendWidth = 250;
            this.mctlScatterChartFeatures.ChartLayout.MaxTitleHeight = 50;
            this.mctlScatterChartFeatures.ChartLayout.MinLegendHeight = 50;
            this.mctlScatterChartFeatures.ChartLayout.MinLegendWidth = 75;
            this.mctlScatterChartFeatures.ChartLayout.MinTitleHeight = 15;
            this.mctlScatterChartFeatures.ChartLayout.TitleFraction = 0.1F;
            this.mctlScatterChartFeatures.DefaultZoomHandler.Active = true;
            this.mctlScatterChartFeatures.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.mctlScatterChartFeatures.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            this.mctlScatterChartFeatures.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            penProvider1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            penProvider1.Width = 1F;
            this.mctlScatterChartFeatures.GridLinePen = penProvider1;
            this.mctlScatterChartFeatures.HasLegend = false;
            this.mctlScatterChartFeatures.HilightColor = System.Drawing.Color.Magenta;
            this.mctlScatterChartFeatures.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider2.Color = System.Drawing.Color.Black;
            penProvider2.Width = 1F;
            this.mctlScatterChartFeatures.Legend.BorderPen = penProvider2;
            this.mctlScatterChartFeatures.Legend.Bounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.mctlScatterChartFeatures.Legend.ColumnWidth = 125;
            this.mctlScatterChartFeatures.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.mctlScatterChartFeatures.Legend.MaxFontSize = 12F;
            this.mctlScatterChartFeatures.Legend.MinFontSize = 6F;
            this.mctlScatterChartFeatures.LegendVisible = false;
            this.mctlScatterChartFeatures.Location = new System.Drawing.Point(549, 10);
            this.mctlScatterChartFeatures.Margins.BottomMarginFraction = 0.1F;
            this.mctlScatterChartFeatures.Margins.BottomMarginMax = 72;
            this.mctlScatterChartFeatures.Margins.BottomMarginMin = 30;
            this.mctlScatterChartFeatures.Margins.DefaultMarginFraction = 0.05F;
            this.mctlScatterChartFeatures.Margins.DefaultMarginMax = 15;
            this.mctlScatterChartFeatures.Margins.DefaultMarginMin = 5;
            this.mctlScatterChartFeatures.Margins.LeftMarginFraction = 0.2F;
            this.mctlScatterChartFeatures.Margins.LeftMarginMax = 150;
            this.mctlScatterChartFeatures.Margins.LeftMarginMin = 72;
            this.mctlScatterChartFeatures.Name = "mctlScatterChartFeatures";
            this.mctlScatterChartFeatures.PadViewPortX = 0F;
            this.mctlScatterChartFeatures.PadViewPortY = 0F;
            this.mctlScatterChartFeatures.Size = new System.Drawing.Size(295, 250);
            this.mctlScatterChartFeatures.TabIndex = 8;
            this.mctlScatterChartFeatures.Title = "Input Data";
            this.mctlScatterChartFeatures.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.mctlScatterChartFeatures.TitleMaxFontSize = 8F;
            this.mctlScatterChartFeatures.TitleMinFontSize = 8F;
            this.mctlScatterChartFeatures.TitleVisible = true;
            this.mctlScatterChartFeatures.VerticalExpansion = 1F;
            this.mctlScatterChartFeatures.ViewPort = ((System.Drawing.RectangleF)(resources.GetObject("mctlScatterChartFeatures.ViewPort")));
            this.mctlScatterChartFeatures.XAxisLabel = "scan #";
            this.mctlScatterChartFeatures.YAxisLabel = "monoisotopic mass";
            // 
            // mtreeview_statusMessages
            // 
            this.mtreeview_statusMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtreeview_statusMessages.Location = new System.Drawing.Point(10, 10);
            this.mtreeview_statusMessages.Name = "mtreeview_statusMessages";
            this.mtreeview_statusMessages.Size = new System.Drawing.Size(514, 567);
            this.mtreeview_statusMessages.TabIndex = 7;
            // 
            // mcontrol_steps
            // 
            this.mcontrol_steps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcontrol_steps.Location = new System.Drawing.Point(144, 83);
            this.mcontrol_steps.Name = "mcontrol_steps";
            this.mcontrol_steps.Size = new System.Drawing.Size(862, 41);
            this.mcontrol_steps.TabIndex = 8;
            // 
            // mcontrol_heatMap
            // 
            this.mcontrol_heatMap.AlignmentFunction = null;
            this.mcontrol_heatMap.Data = null;
            this.mcontrol_heatMap.DrawDemaractionLines = false;
            this.mcontrol_heatMap.Location = new System.Drawing.Point(65, 113);
            this.mcontrol_heatMap.Name = "mcontrol_heatMap";
            this.mcontrol_heatMap.OverrideResize = false;
            this.mcontrol_heatMap.ProgBarPercent = 0;
            this.mcontrol_heatMap.ShowProgBar = false;
            this.mcontrol_heatMap.ShowStatBar = true;
            this.mcontrol_heatMap.Size = new System.Drawing.Size(478, 437);
            this.mcontrol_heatMap.TabIndex = 9;
            this.mcontrol_heatMap.UpdateComplete = true;
            this.mcontrol_heatMap.Visible = false;
            // 
            // ctlPerformAnalysisWizardPage
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.mcontrol_steps);
            this.Controls.Add(this.mtabs_messages);
            this.Controls.Add(this.mprogressBar_current);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProgressBarLabel);
            this.Name = "ctlPerformAnalysisWizardPage";
            this.Size = new System.Drawing.Size(1013, 779);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.ProgressBarLabel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.mprogressBar_current, 0);
            this.Controls.SetChildIndex(this.mtabs_messages, 0);
            this.Controls.SetChildIndex(this.mcontrol_steps, 0);
            this.mtabs_messages.ResumeLayout(false);
            this.tabPgMessages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_alignment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mctlScatterChartFeatures)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
        #endregion
        
        /// <summary>
        /// Handles when the page is loaded and active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ctlPerformAnalysisWizardPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
		{
			mtabs_messages.SelectedTab = mtabs_messages.TabPages[0] ;

            mprogressBar_current.Value = 0;
            mtreeview_statusMessages.Nodes.Clear();
            
			SetWizardButtons(Wizard.UI.WizardButtons.Back | Wizard.UI.WizardButtons.Next);
            NextButtonText = "Stop";                                    
		}

        public void ResetAnalysisReady()
        {
            mbool_hackLoaded = false;
        }

        private void InvokeAddStatusMessage(int statusLevel, string message)
        {
            TreeNode insertNode = new TreeNode();
            insertNode.Text     = message;
            int lastNode        = mtreeview_statusMessages.Nodes.Count - 1;

            if (statusLevel > 0)
            {
                mtreeview_statusMessages.Nodes[lastNode].Nodes.Add(insertNode);
            }
            else
            {                
                mtreeview_statusMessages.Nodes.Add(insertNode);                                
            }
            mtreeview_statusMessages.SelectedNode = insertNode;
            mtreeview_statusMessages.ExpandAll();            
        }        
		public void AddStatusMessage(int statusLevel, string message)
		{
            if (InvokeRequired == true)
                BeginInvoke(new DelegateSetStatusMessage(InvokeAddStatusMessage), statusLevel, message);
            else
                InvokeAddStatusMessage(statusLevel, message);
		}
		public void ClearListBox()
		{
            mtreeview_statusMessages.Nodes.Clear();
		}
        
        /// <summary>
        /// Displays the list of steps that will be performed.
        /// </summary>
        /// <param name="steps"></param>
        public void DisplayListOfSteps(List<string> steps)
        {
            mcontrol_steps.DisplayListOfSteps(steps);
        }
        /// <summary>
        /// Displays the current step.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="stepName"></param>
        public void SetStep(int index, string stepName)
        {
            mcontrol_steps.SetStep(index, stepName);
        }

		public void IncreaseProgressBar(int increaseBy)
		{
			if (increaseBy < 100 && increaseBy > 0)
			{
				this.mprogressBar_current.Step = increaseBy;
				this.mprogressBar_current.PerformStep();
				this.mprogressBar_current.Refresh();
			}
		}
		public void SetProgressBar(int percentComplete)
		{
            percentComplete = Math.Max(Convert.ToInt32(mprogressBar_current.Minimum), Math.Min(Convert.ToInt32(mprogressBar_current.Maximum), percentComplete));

			this.mprogressBar_current.Value = percentComplete;
			this.mprogressBar_current.Refresh();
		}		
		private void AddPeaksToChart(ref float []scans, ref float []masses, int ptSize, Color clr, string name)
		{
			PNNLControls.clsShape shape = new PNNLControls.DiamondShape(ptSize, false) ;  ; 
			PNNLControls.clsPlotParams plt_params = new PNNLControls.clsPlotParams(shape, clr) ; 
			plt_params.Name = name ; 
			mctlScatterChartFeatures.ViewPortHistory.Clear(); 
			mctlScatterChartFeatures.AutoViewPortOnAddition = true ; 
			PNNLControls.clsSeries series = new PNNLControls.clsSeries(ref scans, ref masses, plt_params) ; 
			mctlScatterChartFeatures.AddSeries(series) ;
        }
        public void DisplayUMCS(string fileName, MultiAlignEngine.Features.clsUMC[] umcs)
        {
            mctlScatterChartFeatures.BeginUpdate();
            mctlScatterChartFeatures.SeriesCollection.Clear();
            int numIsotopePeaks = umcs.Length;
            mctlScatterChartFeatures.Title = fileName;

            // Get data for charge 1. 
            // how many charge 1 points are there ? 
            int numCharge1 = 0;
            for (int isoNum = 0; isoNum < numIsotopePeaks; isoNum++)
            {
                if (umcs[isoNum].ChargeRepresentative == 1)
                    numCharge1++;
            }
            float[] massCharge1 = new float[numCharge1];
            float[] scanCharge1 = new float[numCharge1];
            int numCharge1SoFar = 0;
            for (int isoNum = 0; isoNum < numIsotopePeaks; isoNum++)
            {
                if (umcs[isoNum].ChargeRepresentative == 1)
                {
                    massCharge1[numCharge1SoFar] = Convert.ToSingle(umcs[isoNum].mdouble_mono_mass);
                    scanCharge1[numCharge1SoFar] = Convert.ToSingle(umcs[isoNum].mint_scan);
                    numCharge1SoFar++;
                }
            }
            AddPeaksToChart(ref scanCharge1, ref massCharge1, 1, System.Drawing.Color.Blue, "1");

            // Get data for charge 2. 
            // how many charge 2 points are there ? 
            int numCharge2 = 0;
            for (int isoNum = 0; isoNum < numIsotopePeaks; isoNum++)
            {
                if (umcs[isoNum].ChargeRepresentative == 2)
                    numCharge2++;
            }
            float[] massCharge2 = new float[numCharge2];
            float[] scanCharge2 = new float[numCharge2];
            int numCharge2SoFar = 0;
            for (int isoNum = 0; isoNum < numIsotopePeaks; isoNum++)
            {
                if (umcs[isoNum].ChargeRepresentative == 2)
                {
                    massCharge2[numCharge2SoFar] = Convert.ToSingle(umcs[isoNum].mdouble_mono_mass);
                    scanCharge2[numCharge2SoFar] = Convert.ToSingle(umcs[isoNum].mint_scan);
                    numCharge2SoFar++;
                }
            }
            AddPeaksToChart(ref scanCharge2, ref massCharge2, 1, System.Drawing.Color.Red, "2");

            // Get data for charge 3. 
            // how many charge 3 points are there ? 
            int numCharge3 = 0;
            for (int isoNum = 0; isoNum < numIsotopePeaks; isoNum++)
            {
                if (umcs[isoNum].ChargeRepresentative == 3)
                    numCharge3++;
            }
            float[] massCharge3 = new float[numCharge3];
            float[] scanCharge3 = new float[numCharge3];
            int numCharge3SoFar = 0;
            for (int isoNum = 0; isoNum < numIsotopePeaks; isoNum++)
            {
                if (umcs[isoNum].ChargeRepresentative == 3)
                {
                    massCharge3[numCharge3SoFar] = Convert.ToSingle(umcs[isoNum].mdouble_mono_mass);
                    scanCharge3[numCharge3SoFar] = Convert.ToSingle(umcs[isoNum].mint_scan);
                    numCharge3SoFar++;
                }
            }
            AddPeaksToChart(ref scanCharge3, ref massCharge3, 1, System.Drawing.Color.Green, "3");

            // Get data for charge 4. 
            // how many charge 4 points are there ? 
            int numCharge4 = 0;
            for (int isoNum = 0; isoNum < numIsotopePeaks; isoNum++)
            {
                if (umcs[isoNum].ChargeRepresentative >= 4)
                    numCharge4++;
            }
            float[] massCharge4 = new float[numCharge4];
            float[] scanCharge4 = new float[numCharge4];
            int numCharge4SoFar = 0;
            for (int isoNum = 0; isoNum < numIsotopePeaks; isoNum++)
            {
                if (umcs[isoNum].ChargeRepresentative >= 4)
                {
                    massCharge4[numCharge4SoFar] = Convert.ToSingle(umcs[isoNum].mdouble_mono_mass);
                    scanCharge4[numCharge4SoFar] = Convert.ToSingle(umcs[isoNum].mint_scan);
                    numCharge4SoFar++;
                }
            }
            AddPeaksToChart(ref scanCharge4, ref massCharge4, 1, System.Drawing.Color.Yellow, ">=4");
            mctlScatterChartFeatures.EndUpdate();
        }
	    public void DisplayIsotopePeaks(string fileName, MultiAlignEngine.Features.clsIsotopePeak []isotopePeaks)
		{
			mctlScatterChartFeatures.BeginUpdate() ; 
			mctlScatterChartFeatures.SeriesCollection.Clear() ; 
			int numIsotopePeaks = isotopePeaks.Length ; 
			mctlScatterChartFeatures.Title = fileName ;

			// Get data for charge 1. 
			// how many charge 1 points are there ? 
			int numCharge1 = 0 ; 
			for (int isoNum = 0 ; isoNum < numIsotopePeaks ; isoNum++)
			{
				if (isotopePeaks[isoNum].mshort_charge == 1)
					numCharge1++ ; 
			}
			float [] massCharge1 = new float [numCharge1] ; 
			float [] scanCharge1 = new float [numCharge1] ; 
			int numCharge1SoFar = 0 ; 
			for (int isoNum = 0 ; isoNum < numIsotopePeaks ; isoNum++)
			{
				if (isotopePeaks[isoNum].mshort_charge == 1)
				{
					massCharge1[numCharge1SoFar] = Convert.ToSingle(isotopePeaks[isoNum].mdouble_mono_mass) ; 
					scanCharge1[numCharge1SoFar] = Convert.ToSingle(isotopePeaks[isoNum].mint_scan) ; 
					numCharge1SoFar++ ; 
				}
			}
			AddPeaksToChart(ref scanCharge1, ref massCharge1, 1, System.Drawing.Color.Blue, "1") ; 

			// Get data for charge 2. 
			// how many charge 2 points are there ? 
			int numCharge2 = 0 ; 
			for (int isoNum = 0 ; isoNum < numIsotopePeaks ; isoNum++)
			{
				if (isotopePeaks[isoNum].mshort_charge == 2)
					numCharge2++ ; 
			}
			float [] massCharge2 = new float [numCharge2] ; 
			float [] scanCharge2 = new float [numCharge2] ; 
			int numCharge2SoFar = 0 ; 
			for (int isoNum = 0 ; isoNum < numIsotopePeaks ; isoNum++)
			{
				if (isotopePeaks[isoNum].mshort_charge == 2)
				{
					massCharge2[numCharge2SoFar] = Convert.ToSingle(isotopePeaks[isoNum].mdouble_mono_mass) ; 
					scanCharge2[numCharge2SoFar] = Convert.ToSingle(isotopePeaks[isoNum].mint_scan) ; 
					numCharge2SoFar++ ; 
				}
			}
			AddPeaksToChart(ref scanCharge2, ref massCharge2, 1, System.Drawing.Color.Red, "2") ; 

			// Get data for charge 3. 
			// how many charge 3 points are there ? 
			int numCharge3 = 0 ; 
			for (int isoNum = 0 ; isoNum < numIsotopePeaks ; isoNum++)
			{
				if (isotopePeaks[isoNum].mshort_charge == 3)
					numCharge3++ ; 
			}
			float [] massCharge3 = new float [numCharge3] ; 
			float [] scanCharge3 = new float [numCharge3] ; 
			int numCharge3SoFar = 0 ; 
			for (int isoNum = 0 ; isoNum < numIsotopePeaks ; isoNum++)
			{
				if (isotopePeaks[isoNum].mshort_charge == 3)
				{
					massCharge3[numCharge3SoFar] = Convert.ToSingle(isotopePeaks[isoNum].mdouble_mono_mass) ; 
					scanCharge3[numCharge3SoFar] = Convert.ToSingle(isotopePeaks[isoNum].mint_scan) ; 
					numCharge3SoFar++ ; 
				}
			}
			AddPeaksToChart(ref scanCharge3, ref massCharge3, 1, System.Drawing.Color.Green, "3") ; 

			// Get data for charge 4. 
			// how many charge 4 points are there ? 
			int numCharge4 = 0 ; 
			for (int isoNum = 0 ; isoNum < numIsotopePeaks ; isoNum++)
			{
				if (isotopePeaks[isoNum].mshort_charge >= 4)
					numCharge4++ ; 
			}
			float [] massCharge4 = new float [numCharge4] ; 
			float [] scanCharge4 = new float [numCharge4] ; 
			int numCharge4SoFar = 0 ; 
			for (int isoNum = 0 ; isoNum < numIsotopePeaks ; isoNum++)
			{
				if (isotopePeaks[isoNum].mshort_charge >= 4)
				{
					massCharge4[numCharge4SoFar] = Convert.ToSingle(isotopePeaks[isoNum].mdouble_mono_mass) ; 
					scanCharge4[numCharge4SoFar] = Convert.ToSingle(isotopePeaks[isoNum].mint_scan) ; 
					numCharge4SoFar++ ; 
				}
			}
			AddPeaksToChart(ref scanCharge4, ref massCharge4, 1, System.Drawing.Color.Orange, ">=4") ; 
			mctlScatterChartFeatures.EndUpdate() ; 
		}
        /// <summary>
        /// Displays the alignment heat map for this dataset.
        /// </summary>
        /// <param name="alignmentFnc"></param>
        /// <param name="title"></param>
        /// <param name="mScores"></param>
        /// <param name="minAligneeScan"></param>
        /// <param name="maxAligneeScan"></param>
        /// <param name="minBaselineScan"></param>
        /// <param name="maxBaselineScan"></param>
		public void SetAlignmentHeatMap(MultiAlignEngine.Alignment.clsAlignmentFunction alignmentFnc, string title, 
			                            float [,] mScores, 
                                        float minAligneeScan, 
                                        float maxAligneeScan, 
                                        float minBaselineScan, 
			                            float maxBaselineScan,
                                        int   part)
		{
			try
			{
				// first zscore the data on the level of the x axis.
				int numRows = mScores.GetUpperBound(0) - mScores.GetLowerBound(0) ; 
				int numColumns = mScores.GetUpperBound(1) - mScores.GetLowerBound(1) ; 
				for (int colNum = 0 ; colNum < numColumns ; colNum++)
				{
					for (int rowNum = 0 ; rowNum < numRows/2 ; rowNum++)
					{
						float tmp                           = mScores[rowNum, colNum] ; 
						mScores[rowNum, colNum]             = mScores[numRows-rowNum-1, colNum] ;
						mScores[numRows-rowNum-1, colNum]   = tmp ;  
					}
				}

                //mcontrol_heatMap.AlignmentFunction = alignmentFnc;
                //mcontrol_heatMap.Legend.UseZScore = true;
		    	mcontrol_heatMap.SetData(mScores,
                                            new PNNLControls.ctlHierarchalLabel.AxisRangeF(minAligneeScan, maxAligneeScan), 
				                            new PNNLControls.ctlHierarchalLabel.AxisRangeF(minBaselineScan, maxBaselineScan)) ;
                mpicture_alignment.Image            = mcontrol_heatMap.GetThumbnail(mpicture_alignment.Size);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace) ; 
			}
		}

        private void mprogressBar_current_Click(object sender, EventArgs e)
        {

        }

	}
}

