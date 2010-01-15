using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MultiAlignEngine;
using MultiAlignEngine.Alignment;
using MultiAlignWin.Drawing;

using PNNLProteomics.EventModel;
using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmAlignmentPreview.
	/// </summary>
	public class frmAlignmentPreview : Form
    {
		//private MultiAlignWin.Drawing.ctlAlignmentHeatMap mctl_AlignmentHeatMap;
		private System.ComponentModel.Container components = null;
		private clsMultiAlignAnalysis mobj_analysis;
        private System.Windows.Forms.Button btnAlign;
		private System.Windows.Forms.ComboBox cboBaseline;
		private MultiAlignWin.ctlSummaryPages ctlSummaryPages;
		private static MultiAlignWin.frmStatus mfrm_status = new frmStatus();
		private int mint_aligneeIndex;

		private clsMultiAlignAnalysis.DelegateDatasetAligned mevntDatasetAligned ; 
		private static DelegateSetPercentComplete mevntPercentComplete ;
        private Button mbutton_cancel;
        private Button mbutton_ok;
        private TabControl mtabControl_views;
        private TabPage mtabPage_alignmentSelection;
        private TabPage mtabPage_summary;
        private ctlAlignmentHeatMap mctlAlignmentHeatMap;
        private Label mlabel_alignedTo;
        private RadioButton mradio_alignToFile;
        private RadioButton mradio_alignToMTDB;
        private Button mbutton_loadMassTagDatabasePeaks;
        private Label mlabel_peakMatchingDatabase;
        private GroupBox mgroupBox_baselineSelection; 
		private static DelegateSetStatusMessage mevntStatusMessage ;

        private frmDBName mform_dbForm = new frmDBName();

		static frmAlignmentPreview()
		{
			mevntPercentComplete = new DelegateSetPercentComplete(mobj_analysis_mevntPercentComplete);
			mevntStatusMessage	 = new DelegateSetStatusMessage(mobj_analysis_mevntStatusMessage);	
		}

		public frmAlignmentPreview(clsMultiAlignAnalysis analysis, int aligneeIndex)
		{
			mint_aligneeIndex = aligneeIndex;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mobj_analysis = analysis;
			ctlSummaryPages.CreateSummary("Alignment Options", mobj_analysis.DefaultAlignmentOptions);
            ctlSummaryPages.CreateSummary("Other Options", mobj_analysis);
			

            /// 
            /// Add the datasets for baseline re-alignment.
            /// 
			cboBaseline.Items.Clear();
			foreach(clsDatasetInfo data in mobj_analysis.Files)
			{
				cboBaseline.Items.Add(data.mstrDatasetName);
			}
            if (cboBaseline.Items.Count < 2)
            {
                cboBaseline.Enabled        = false;
                mradio_alignToFile.Enabled = false;
            }

            /// 
            /// Enable using a MTDB as a baseline.
            /// 
            clsAlignmentOptions options = mobj_analysis.AlignmentOptions[aligneeIndex] as clsAlignmentOptions;
            if (options != null)
            {
                if (options.IsAlignmentBaselineAMasstagDB == true)               
                    mlabel_peakMatchingDatabase.Text = options.AlignmentBaselineName;   
            }


			mevntDatasetAligned = new clsMultiAlignAnalysis.DelegateDatasetAligned(SetAlignmentHeatMap) ;

			mobj_analysis.DatasetAligned	+=  mevntDatasetAligned ;
			mobj_analysis.PercentComplete	+= mevntPercentComplete ;
			mobj_analysis.StatusMessage	+= mevntStatusMessage ;		

			base.Closed +=new EventHandler(frmAlignmentPreview_Closed);


            if (options != null)
            {
                if (options.IsAlignmentBaselineAMasstagDB == false)
                {
                    mlabel_alignedTo.Text = "Aligned To: " + System.IO.Path.GetFileNameWithoutExtension(options.AlignmentBaselineName);
                }
            }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void  Dispose(bool disposing)
        {
 	    	if (disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}		
            base.Dispose(disposing);
        }
		

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnAlign = new System.Windows.Forms.Button();
            this.cboBaseline = new System.Windows.Forms.ComboBox();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mtabControl_views = new System.Windows.Forms.TabControl();
            this.mtabPage_alignmentSelection = new System.Windows.Forms.TabPage();
            this.mgroupBox_baselineSelection = new System.Windows.Forms.GroupBox();
            this.mradio_alignToMTDB = new System.Windows.Forms.RadioButton();
            this.mbutton_loadMassTagDatabasePeaks = new System.Windows.Forms.Button();
            this.mlabel_peakMatchingDatabase = new System.Windows.Forms.Label();
            this.mradio_alignToFile = new System.Windows.Forms.RadioButton();
            this.mlabel_alignedTo = new System.Windows.Forms.Label();
            this.mtabPage_summary = new System.Windows.Forms.TabPage();
            this.mctlAlignmentHeatMap = new MultiAlignWin.Drawing.ctlAlignmentHeatMap();
            this.ctlSummaryPages = new MultiAlignWin.ctlSummaryPages();
            this.mtabControl_views.SuspendLayout();
            this.mtabPage_alignmentSelection.SuspendLayout();
            this.mgroupBox_baselineSelection.SuspendLayout();
            this.mtabPage_summary.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAlign
            // 
            this.btnAlign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlign.Enabled = false;
            this.btnAlign.Location = new System.Drawing.Point(578, 95);
            this.btnAlign.Name = "btnAlign";
            this.btnAlign.Size = new System.Drawing.Size(120, 24);
            this.btnAlign.TabIndex = 13;
            this.btnAlign.Text = "Re-Align";
            this.btnAlign.Click += new System.EventHandler(this.btnAlign_Click);
            // 
            // cboBaseline
            // 
            this.cboBaseline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBaseline.Location = new System.Drawing.Point(30, 44);
            this.cboBaseline.Name = "cboBaseline";
            this.cboBaseline.Size = new System.Drawing.Size(668, 24);
            this.cboBaseline.TabIndex = 11;
            this.cboBaseline.Text = "Select new baseline ";
            this.cboBaseline.SelectedIndexChanged += new System.EventHandler(this.cboBaseline_SelectedIndexChanged);
            // 
            // mbutton_cancel
            // 
            this.mbutton_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbutton_cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_cancel.Location = new System.Drawing.Point(645, 634);
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.Size = new System.Drawing.Size(82, 28);
            this.mbutton_cancel.TabIndex = 14;
            this.mbutton_cancel.Text = "Cancel";
            this.mbutton_cancel.UseVisualStyleBackColor = true;
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_ok.Location = new System.Drawing.Point(559, 634);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(80, 28);
            this.mbutton_ok.TabIndex = 15;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            // 
            // mtabControl_views
            // 
            this.mtabControl_views.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtabControl_views.Controls.Add(this.mtabPage_alignmentSelection);
            this.mtabControl_views.Controls.Add(this.mtabPage_summary);
            this.mtabControl_views.Location = new System.Drawing.Point(2, 3);
            this.mtabControl_views.Name = "mtabControl_views";
            this.mtabControl_views.SelectedIndex = 0;
            this.mtabControl_views.Size = new System.Drawing.Size(725, 625);
            this.mtabControl_views.TabIndex = 18;
            // 
            // mtabPage_alignmentSelection
            // 
            this.mtabPage_alignmentSelection.Controls.Add(this.mgroupBox_baselineSelection);
            this.mtabPage_alignmentSelection.Controls.Add(this.mlabel_alignedTo);
            this.mtabPage_alignmentSelection.Controls.Add(this.mctlAlignmentHeatMap);
            this.mtabPage_alignmentSelection.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_alignmentSelection.Name = "mtabPage_alignmentSelection";
            this.mtabPage_alignmentSelection.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_alignmentSelection.Size = new System.Drawing.Size(717, 599);
            this.mtabPage_alignmentSelection.TabIndex = 0;
            this.mtabPage_alignmentSelection.Text = "Alignment";
            this.mtabPage_alignmentSelection.UseVisualStyleBackColor = true;
            // 
            // mgroupBox_baselineSelection
            // 
            this.mgroupBox_baselineSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_baselineSelection.Controls.Add(this.mradio_alignToMTDB);
            this.mgroupBox_baselineSelection.Controls.Add(this.mbutton_loadMassTagDatabasePeaks);
            this.mgroupBox_baselineSelection.Controls.Add(this.cboBaseline);
            this.mgroupBox_baselineSelection.Controls.Add(this.btnAlign);
            this.mgroupBox_baselineSelection.Controls.Add(this.mlabel_peakMatchingDatabase);
            this.mgroupBox_baselineSelection.Controls.Add(this.mradio_alignToFile);
            this.mgroupBox_baselineSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_baselineSelection.Location = new System.Drawing.Point(9, 40);
            this.mgroupBox_baselineSelection.Name = "mgroupBox_baselineSelection";
            this.mgroupBox_baselineSelection.Size = new System.Drawing.Size(704, 125);
            this.mgroupBox_baselineSelection.TabIndex = 26;
            this.mgroupBox_baselineSelection.TabStop = false;
            this.mgroupBox_baselineSelection.Text = "Re-Align with:";
            // 
            // mradio_alignToMTDB
            // 
            this.mradio_alignToMTDB.AutoSize = true;
            this.mradio_alignToMTDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradio_alignToMTDB.Location = new System.Drawing.Point(6, 74);
            this.mradio_alignToMTDB.Name = "mradio_alignToMTDB";
            this.mradio_alignToMTDB.Size = new System.Drawing.Size(199, 17);
            this.mradio_alignToMTDB.TabIndex = 15;
            this.mradio_alignToMTDB.TabStop = true;
            this.mradio_alignToMTDB.Text = "Align to Mass Tag Database (MTDB)";
            this.mradio_alignToMTDB.UseVisualStyleBackColor = true;
            this.mradio_alignToMTDB.CheckedChanged += new System.EventHandler(this.mradio_alignToMTDB_CheckedChanged);
            // 
            // mbutton_loadMassTagDatabasePeaks
            // 
            this.mbutton_loadMassTagDatabasePeaks.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_loadMassTagDatabasePeaks.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_loadMassTagDatabasePeaks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_loadMassTagDatabasePeaks.Location = new System.Drawing.Point(211, 71);
            this.mbutton_loadMassTagDatabasePeaks.Name = "mbutton_loadMassTagDatabasePeaks";
            this.mbutton_loadMassTagDatabasePeaks.Size = new System.Drawing.Size(27, 23);
            this.mbutton_loadMassTagDatabasePeaks.TabIndex = 24;
            this.mbutton_loadMassTagDatabasePeaks.Text = "...";
            this.mbutton_loadMassTagDatabasePeaks.UseVisualStyleBackColor = false;
            this.mbutton_loadMassTagDatabasePeaks.Click += new System.EventHandler(this.mbutton_loadMassTagDatabasePeaks_Click);
            // 
            // mlabel_peakMatchingDatabase
            // 
            this.mlabel_peakMatchingDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_peakMatchingDatabase.AutoEllipsis = true;
            this.mlabel_peakMatchingDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_peakMatchingDatabase.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.mlabel_peakMatchingDatabase.Location = new System.Drawing.Point(244, 74);
            this.mlabel_peakMatchingDatabase.Name = "mlabel_peakMatchingDatabase";
            this.mlabel_peakMatchingDatabase.Size = new System.Drawing.Size(454, 20);
            this.mlabel_peakMatchingDatabase.TabIndex = 25;
            this.mlabel_peakMatchingDatabase.Text = "No Database Selected";
            this.mlabel_peakMatchingDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mradio_alignToFile
            // 
            this.mradio_alignToFile.AutoSize = true;
            this.mradio_alignToFile.Checked = true;
            this.mradio_alignToFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradio_alignToFile.Location = new System.Drawing.Point(6, 21);
            this.mradio_alignToFile.Name = "mradio_alignToFile";
            this.mradio_alignToFile.Size = new System.Drawing.Size(107, 17);
            this.mradio_alignToFile.TabIndex = 14;
            this.mradio_alignToFile.TabStop = true;
            this.mradio_alignToFile.Text = "Align to a dataset";
            this.mradio_alignToFile.UseVisualStyleBackColor = true;
            this.mradio_alignToFile.CheckedChanged += new System.EventHandler(this.mradio_alignToFile_CheckedChanged);
            // 
            // mlabel_alignedTo
            // 
            this.mlabel_alignedTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_alignedTo.AutoEllipsis = true;
            this.mlabel_alignedTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_alignedTo.ForeColor = System.Drawing.Color.Blue;
            this.mlabel_alignedTo.Location = new System.Drawing.Point(16, 13);
            this.mlabel_alignedTo.Name = "mlabel_alignedTo";
            this.mlabel_alignedTo.Size = new System.Drawing.Size(696, 24);
            this.mlabel_alignedTo.TabIndex = 15;
            this.mlabel_alignedTo.Text = "Aligned to:";
            // 
            // mtabPage_summary
            // 
            this.mtabPage_summary.Controls.Add(this.ctlSummaryPages);
            this.mtabPage_summary.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_summary.Name = "mtabPage_summary";
            this.mtabPage_summary.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_summary.Size = new System.Drawing.Size(717, 599);
            this.mtabPage_summary.TabIndex = 1;
            this.mtabPage_summary.Text = "Summary";
            this.mtabPage_summary.UseVisualStyleBackColor = true;
            // 
            // mctlAlignmentHeatMap
            // 
            this.mctlAlignmentHeatMap.AlignmentFunction = null;
            this.mctlAlignmentHeatMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mctlAlignmentHeatMap.Data = null;
            this.mctlAlignmentHeatMap.DrawDemaractionLines = false;
            this.mctlAlignmentHeatMap.Location = new System.Drawing.Point(6, 171);
            this.mctlAlignmentHeatMap.Name = "mctlAlignmentHeatMap";
            this.mctlAlignmentHeatMap.OverrideResize = false;
            this.mctlAlignmentHeatMap.ProgBarPercent = 0;
            this.mctlAlignmentHeatMap.ShowProgBar = false;
            this.mctlAlignmentHeatMap.ShowStatBar = true;
            this.mctlAlignmentHeatMap.Size = new System.Drawing.Size(705, 422);
            this.mctlAlignmentHeatMap.TabIndex = 14;
            this.mctlAlignmentHeatMap.UpdateComplete = true;
            // 
            // ctlSummaryPages
            // 
            this.ctlSummaryPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlSummaryPages.Location = new System.Drawing.Point(3, 3);
            this.ctlSummaryPages.Name = "ctlSummaryPages";
            this.ctlSummaryPages.Size = new System.Drawing.Size(711, 593);
            this.ctlSummaryPages.TabIndex = 11;
            // 
            // frmAlignmentPreview
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(730, 674);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.mtabControl_views);
            this.Controls.Add(this.mbutton_ok);
            this.Name = "frmAlignmentPreview";
            this.Text = "Alignment Preview";
            this.mtabControl_views.ResumeLayout(false);
            this.mtabPage_alignmentSelection.ResumeLayout(false);
            this.mgroupBox_baselineSelection.ResumeLayout(false);
            this.mgroupBox_baselineSelection.PerformLayout();
            this.mtabPage_summary.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        /// <summary>
        /// Displays the alignment heat map.
        /// </summary>
        /// <param name="alignmentFnc"></param>
        /// <param name="fileName"></param>
        /// <param name="mScores"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <param name="isPart"></param>
        public void SetAlignmentHeatMap(clsAlignmentFunction alignmentFnc, 
                                        string title,
                                        ref float[,] mScores, 
                                        float minAligneeScan,
                                        float maxAligneeScan, 
                                        float minBaselineScan,
                                        float maxBaselineScan,
                                        int part)
		{
			try
			{
				// first zscore the data on the level of the x axis.
				int numRows    = mScores.GetUpperBound(0) - mScores.GetLowerBound(0) ; 
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
                
				mctlAlignmentHeatMap.SetData(mScores,
                                             new PNNLControls.ctlHierarchalLabel.AxisRangeF(minAligneeScan, maxAligneeScan), 
					                         new PNNLControls.ctlHierarchalLabel.AxisRangeF(minBaselineScan, maxBaselineScan)); 
				mctlAlignmentHeatMap.AlignmentFunction = alignmentFnc; 
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace) ; 
			}
		}
		private static void mobj_analysis_mevntPercentComplete(int percentDone)
		{
			if (mfrm_status.mevntPercentComplete != null)
				mfrm_status.mevntPercentComplete(percentDone);
		}
		private static void mobj_analysis_mevntStatusMessage(int statusLevel, string status)
		{
			if (mfrm_status.mevntStatusMessage != null)
				mfrm_status.mevntStatusMessage(statusLevel, status);
		}
        /// <summary>
        /// Hides the status form.
        /// </summary>
        private void HideStatusForm()
        {
            mfrm_status.Hide();
        }
        /// <summary>
        /// Performs the re-alignment for a given dataset index.
        /// </summary>
		private void PerformAnalysis()
		{
			mobj_analysis.AlignDataset(mint_aligneeIndex);
            if (InvokeRequired == true)
            {
                Invoke(new MethodInvoker(HideStatusForm));
            }
            else
            {
                HideStatusForm();
            }
		}
        /// <summary>
        /// Starts the re-alignment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnAlign_Click(object sender, System.EventArgs e)
		{

            if (mradio_alignToMTDB.Checked == true)
                mobj_analysis.LoadMassTagDB();

			// need a new thread for this operation rather than performing it on the UI threads.
			System.Threading.ThreadStart tStart = new System.Threading.ThreadStart(PerformAnalysis); 
			System.Threading.Thread thread      = new System.Threading.Thread(tStart);
			thread.Start();

			System.Threading.ThreadStart tStartMonitor = new System.Threading.ThreadStart(mobj_analysis.MonitorAlignments);
            System.Threading.Thread threadMonitor      = new System.Threading.Thread(tStartMonitor);
			threadMonitor.Start();
			mfrm_status.ShowDialog();	
			threadMonitor.Abort() ;             
		}
        /// <summary>
        /// Handles when the form is closing to kill any lingering threads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void frmAlignmentPreview_Closed(object sender, EventArgs e)
		{
			if (mobj_analysis != null && mevntDatasetAligned != null)
			{
				mobj_analysis.DatasetAligned -= mevntDatasetAligned ;
			}
			if (mobj_analysis != null && mevntPercentComplete != null)
			{
				mobj_analysis.PercentComplete -= mevntPercentComplete ;
			}
			if (mobj_analysis != null && mevntStatusMessage != null)
			{
				mobj_analysis.StatusMessage -= mevntStatusMessage ; 
			}
		}
        /// <summary>
        /// Handles when the user wants to align to the MTDB.  Disables the button if MTDB parameters are incorect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mradio_alignToMTDB_CheckedChanged(object sender, EventArgs e)
        {
            if (mlabel_peakMatchingDatabase.Text == "No Database Selected" || string.IsNullOrEmpty(mlabel_peakMatchingDatabase.Text))
            {
                if (mradio_alignToMTDB.Checked == true)
                {
                    btnAlign.Enabled = false;
                }
            }
            else
            {
                btnAlign.Enabled = true;
            }
        }
        /// <summary>
        /// Handles when the user wants to align the dataset to the baseline dataset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mradio_alignToFile_CheckedChanged(object sender, EventArgs e)
        {
            if (mradio_alignToFile.Checked == true)
            {
                cboBaseline.Enabled = true;

                clsAlignmentOptions options = mobj_analysis.AlignmentOptions[mint_aligneeIndex] as clsAlignmentOptions;
                if (options != null && mint_aligneeIndex != cboBaseline.SelectedIndex && cboBaseline.SelectedIndex >= 0)
                {
                    options.AlignmentBaselineName = mobj_analysis.FileNames[cboBaseline.SelectedIndex];
                }
                options.IsAlignmentBaselineAMasstagDB = false;

                if (cboBaseline.SelectedIndex >= 0)
                    btnAlign.Enabled = true;
                else
                    btnAlign.Enabled = false;
            }
            else
            {
                cboBaseline.Enabled = false;
            }
        }
        /// <summary>
        /// Determine if we are to load the data from the mass tag database for alignment or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_loadMassTagDatabasePeaks_Click(object sender, EventArgs e)
        {                        
            if (mform_dbForm.ShowDialog() == DialogResult.OK)
            {
                clsAlignmentOptions options                 = mobj_analysis.AlignmentOptions[mint_aligneeIndex] as clsAlignmentOptions;
                if (options != null)
                {
                    mobj_analysis.MassTagDBOptions          = mform_dbForm.MassTagDatabaseOptions;
                    options.AlignmentBaselineName           = mform_dbForm.MassTagDatabaseOptions.mstrDatabase;
                    options.IsAlignmentBaselineAMasstagDB   = true;
                    btnAlign.Enabled                        = true;
                    mlabel_peakMatchingDatabase.Text        = mobj_analysis.MassTagDBOptions.mstrDatabase;
                    mradio_alignToMTDB.Checked              = true;
                }
            }            
        }
        /// <summary>
        /// Enables the button to re-align if the right baseline is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBaseline_SelectedIndexChanged(object sender, EventArgs e)
        {           
                clsAlignmentOptions options = mobj_analysis.AlignmentOptions[mint_aligneeIndex] as clsAlignmentOptions;
                if (options != null && mint_aligneeIndex != cboBaseline.SelectedIndex && cboBaseline.SelectedIndex >= 0)
                {
                    options.AlignmentBaselineName = mobj_analysis.FileNames[cboBaseline.SelectedIndex];
                }
                options.IsAlignmentBaselineAMasstagDB = false;

                if (cboBaseline.SelectedIndex >= 0)
                    btnAlign.Enabled = true;
                else
                    btnAlign.Enabled = false;           
        }
	}
}
