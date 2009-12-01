using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Specialized;

namespace PNNLControls
{
	/// <summary>
	/// Scatter plot class for plotting NxM datasets.
	/// </summary>
	public class ctlScatterPlot : PNNLControls.ctlDataFrame
	{
		private enum LinearParameterSave{NORMAL, LOG, ALL};
		private const int MIN_LEAF_HEIGHT = 30;

		#region Event Delegate Declarations 
		public new delegate void UpdateProgress(bool show, int percent, string message);
		public delegate void DelegateSelectedLabel(int labelIndex, bool selected, bool isRow);
		public delegate void DelegateSelectedPlot(int startRow, int endRow, int startColumn, int endColumn, bool selected);		
		/// <summary>
		/// Indicates which label was selected, tells whether the label is a row or column (row = vertical)
		/// </summary>
		//public event DelegateSelectedLabel OnSelectedLabel;
		/// <summary>
		/// Indicates which subplot was selected.
		/// </summary>
		public event DelegateSelectedPlot  OnSelectedPlot;
		#endregion

		#region Members Declarations
		/// <summary>
		/// Determines whether to redraw the control or not.
		/// </summary>
		private System.ComponentModel.Container components = null;				
	
		#region Context Menu		
		private ContextMenu m_contextMenu;	
		/*
		 *	Menus - Context menu based.
		 */
		/// <summary>
		/// Splitter
		/// </summary>
		private MenuItem menuItem3;
		/// <summary>
		/// Splitter
		/// </summary>
		private MenuItem menuItem4;
		/// <summary>
		/// Splitter
		/// </summary>
		private MenuItem menuItem5;
		/// <summary>
		/// Splitter
		/// </summary>
		private MenuItem menuItem6;
		/// <summary>
		/// Splitter
		/// </summary>
		private MenuItem menuItem2;
		/// <summary>
		/// Splitter
		/// </summary>
		private MenuItem menuItem8;
		/// <summary>
		/// Splitter
		/// </summary>
		private MenuItem menuItem7;
		private MenuItem menuItem1 = new MenuItem();
		private MenuItem mnu__saveAsEmf;
		private MenuItem mnu__saveAsGif;
		private MenuItem mnu__saveAsJpg;
		private MenuItem mnu__saveAsBmp;
		private MenuItem mnu__saveAsPng;			
		private MenuItem mnu__copyToClipboard;
		private MenuItem mnu_SaveData;
		private MenuItem mnu_SaveRSquared;
		private MenuItem mnu_SaveEquation;		
		private MenuItem mnu_SaveBothEqnR;
		private MenuItem mnu_HideBothEqnR;
		private MenuItem mnu_ShowEquation;
		private MenuItem mnu_SaveRSquaredLog;
		private MenuItem mnu_SaveLogEqn;
		private MenuItem mnu_SaveLogEqnRSq;
		private MenuItem mnu_Save;
		private MenuItem mnu_SaveAll;
		#endregion

		private ToolTip		m_toolTip = new ToolTip();
		
		private clsColorIterator miter_color = new PNNLControls.clsColorIterator() ; 						
		private ctlScatterPlotClient m_scatter;

		private bool m_firstDisplay			 = false;		
		private bool m_showThumbnail		 = true;
		private bool [] m_selectedColumn = null;
		private bool [] m_selectedRow   = null;
		private int m_prevHeight			 = 0;		
		private int m_prevWidth				 = 0;			

		private System.Windows.Forms.MenuItem mnu_FitToScreen;
		private System.Windows.Forms.MenuItem mnu_ShowScatterNormal;
		private System.Windows.Forms.MenuItem mnu_ShowScatterLogarithmic;
		private System.Windows.Forms.MenuItem mnu_ShowData;
		private System.Windows.Forms.MenuItem mnu_HideData;
		private System.Windows.Forms.MenuItem mnu_ShowBothText;
		private System.Windows.Forms.MenuItem mnu_SaveAsTiff;
		private System.Windows.Forms.MenuItem mnu_SaveAsWmf;
		private System.Windows.Forms.MenuItem mnu_Refresh;
		private System.Windows.Forms.MenuItem mnu_Properties;
		private System.Windows.Forms.MenuItem mnu_ShowRSquared;

			
		#endregion
		
		#region Constructors
		/// <summary>
		/// Constructor for scatter plot.  
		/// </summary>
		public ctlScatterPlot()
		{			
			InitControl();
		}
			
		private void InitControl()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);


			InitializeComponent();										

			this.splitterHorizontal.BringToFront();
			this.splitterHorizontal.Dock = DockStyle.Bottom;
			this.splitterVertical.BringToFront();
			this.splitterVertical.Dock = DockStyle.Left;
			

			m_scatter.Legend = base.legend;	
			
			/* Setup Event handlers */
			
			ClientSize						+= new ClientSizeDelegate(ctlScatterPlot_ClientSize);
			ClientLocation					+= new ClientLocationDelegate(ctlScatterPlot_ClientLocation);			
			LabelsUpdated					+= new PNNLControls.ctlHierarchalLabel.LabelUpdateDelegate(ctlScatterPlot_LabelsUpdated);			
			Resize							+= new EventHandler(ctlScatterPlot_Resize);
			VisibleChanged					+= new EventHandler(ctlScatterPlot_VisibleChanged);
			base.legend.LegendChanged		+= new ctlHeatMapLegend.LegendChangedDelegate(legend_LegendChanged);						
			m_scatter.OnPercentDrawn		+= new ctlScatterPlotClient.PercentageCompleteDelegate(m_scatter_OnPercentDrawn);
			m_scatter.OnDrawingComplete		+= new ctlScatterPlotClient.DrawingCompleteDelegate(m_scatter_OnDrawingComplete);
			m_scatter.OnPlotClicked			+= new ctlScatterPlotClient.ClickedPlotDelegate(m_scatter_OnPlotClicked);
			RangeHorizontal					+= new PNNLControls.ctlHierarchalLabel.RangeDelegate(ctlScatterPlot_RangeHorizontal);
			RangeVertical					+= new PNNLControls.ctlHierarchalLabel.RangeDelegate(ctlScatterPlot_RangeVertical);
			m_scatter.OnCalculationComplete += new PNNLControls.ctlScatterPlotClient.CalculationCompleteDelegate(m_scatter_OnCalculationComplete);
			m_scatter.OnPercentCalc			+= new PNNLControls.ctlScatterPlotClient.PercentageCompleteDelegate(m_scatter_OnPercentCalc);
			

			this.OverrideResize = true;

			VerticalLabel.ShowAxis		= false;
			HorizontalLabel.ShowAxis	= false;

			m_scatter.ScatterPlotMouseWheel += new MouseEventHandler(m_scatter_ScatterPlotMouseWheel);
			/* Progress Bar for painting large bitmaps */		
			ProgBar.Minimum = 0;
			ProgBar.Maximum = 100;
			ProgBar.Visible = false;
			StatBar.Visible = false;
			StatBar.Panels.Clear();			
			StatBar.Text = "";
			StatBar.ShowPanels = false;

			/* Setup the labels */
			HorizontalLabel.MinLeafHeight = MIN_LEAF_HEIGHT;
			HorizontalLabel.DragEnabled	  = false;
			VerticalLabel.MinLeafHeight   = MIN_LEAF_HEIGHT;						
			VerticalLabel.DragEnabled	  = false;
			
			/* Previous width, height to track when the control has changed sizes for minimizes and maximizes -- ensures valid refresh */
			m_prevWidth  = Width;
			m_prevHeight = Height;
			
			/* Enable the right context menu options for displaying data. */ 
			mnu_ShowData.Enabled				 = false;						
			mnu_ShowRSquared.Enabled			 = false;
			mnu_ShowScatterNormal.Enabled		 = false;
			mnu_ShowScatterLogarithmic.Enabled = true;	
		
			UpdateContextMenuUI(false,false);
			mnu_ShowData.Enabled = true;
			mnu_HideData.Enabled = false;
			m_scatter.ShowScatterData = false;
			
			m_toolTip.InitialDelay = 2;				
		}

		#endregion
			
		#region Component Designer generated code
		
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{					
					m_scatter.Dispose();
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_contextMenu = new System.Windows.Forms.ContextMenu();
			this.mnu_ShowScatterNormal = new System.Windows.Forms.MenuItem();
			this.mnu_ShowScatterLogarithmic = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnu_ShowData = new System.Windows.Forms.MenuItem();
			this.mnu_HideData = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.mnu_ShowRSquared = new System.Windows.Forms.MenuItem();
			this.mnu_ShowEquation = new System.Windows.Forms.MenuItem();
			this.mnu_ShowBothText = new System.Windows.Forms.MenuItem();
			this.mnu_HideBothEqnR = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mnu_Save = new System.Windows.Forms.MenuItem();
			this.mnu_SaveAsTiff = new System.Windows.Forms.MenuItem();
			this.mnu_SaveAsWmf = new System.Windows.Forms.MenuItem();
			this.mnu__saveAsEmf = new System.Windows.Forms.MenuItem();
			this.mnu__saveAsGif = new System.Windows.Forms.MenuItem();
			this.mnu__saveAsJpg = new System.Windows.Forms.MenuItem();
			this.mnu__saveAsBmp = new System.Windows.Forms.MenuItem();
			this.mnu__saveAsPng = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.mnu__copyToClipboard = new System.Windows.Forms.MenuItem();
			this.mnu_SaveData = new System.Windows.Forms.MenuItem();
			this.mnu_SaveRSquared = new System.Windows.Forms.MenuItem();
			this.mnu_SaveEquation = new System.Windows.Forms.MenuItem();
			this.mnu_SaveBothEqnR = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.mnu_SaveRSquaredLog = new System.Windows.Forms.MenuItem();
			this.mnu_SaveLogEqn = new System.Windows.Forms.MenuItem();
			this.mnu_SaveLogEqnRSq = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.mnu_SaveAll = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mnu_FitToScreen = new System.Windows.Forms.MenuItem();
			this.mnu_Refresh = new System.Windows.Forms.MenuItem();
			this.mnu_Properties = new System.Windows.Forms.MenuItem();
			this.m_scatter = new PNNLControls.ctlScatterPlotClient();
			this.panel1.SuspendLayout();
			this.pnlSE.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(5, 475);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(747, 120);
			// 
			// hLabelHorz
			// 
			this.hLabelHorz.Location = new System.Drawing.Point(137, 0);
			this.hLabelHorz.Name = "hLabelHorz";
			this.hLabelHorz.Size = new System.Drawing.Size(570, 120);
			// 
			// hLabelVert
			// 
			this.hLabelVert.Location = new System.Drawing.Point(5, 56);
			this.hLabelVert.Name = "hLabelVert";
			this.hLabelVert.Size = new System.Drawing.Size(136, 419);
			// 
			// legend
			// 
			this.legend.Location = new System.Drawing.Point(712, 56);
			this.legend.Name = "legend";
			// 
			// pnlSE
			// 
			this.pnlSE.Location = new System.Drawing.Point(707, 0);
			this.pnlSE.Name = "pnlSE";
			// 
			// pnlHeader
			// 
			this.pnlHeader.Location = new System.Drawing.Point(5, 0);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(747, 56);
			// 
			// picRight
			// 
			this.picRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picRight.Name = "picRight";
			// 
			// picLeft
			// 
			this.picLeft.Name = "picLeft";
			this.picLeft.Size = new System.Drawing.Size(137, 120);
			// 
			// splitterHorizontal
			// 
			this.splitterHorizontal.Location = new System.Drawing.Point(0, 595);
			this.splitterHorizontal.Name = "splitterHorizontal";
			this.splitterHorizontal.Size = new System.Drawing.Size(752, 5);
			// 
			// splitterVertical
			// 
			this.splitterVertical.Location = new System.Drawing.Point(0, 0);
			this.splitterVertical.Name = "splitterVertical";
			this.splitterVertical.Size = new System.Drawing.Size(5, 595);
			// 
			// m_contextMenu
			// 
			this.m_contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.mnu_ShowScatterNormal,
																						  this.mnu_ShowScatterLogarithmic,
																						  this.menuItem3,
																						  this.mnu_ShowData,
																						  this.mnu_HideData,
																						  this.menuItem7,
																						  this.mnu_ShowRSquared,
																						  this.mnu_ShowEquation,
																						  this.mnu_ShowBothText,
																						  this.mnu_HideBothEqnR,
																						  this.menuItem5,
																						  this.mnu_Save,
																						  this.mnu_SaveData,
																						  this.menuItem4,
																						  this.mnu_FitToScreen,
																						  this.mnu_Refresh,
																						  this.mnu_Properties});
			// 
			// mnu_ShowScatterNormal
			// 
			this.mnu_ShowScatterNormal.Index = 0;
			this.mnu_ShowScatterNormal.Text = "View as Normal Data";
			this.mnu_ShowScatterNormal.Click += new System.EventHandler(this.mnu__show_scatter_normal_Click);
			// 
			// mnu_ShowScatterLogarithmic
			// 
			this.mnu_ShowScatterLogarithmic.Index = 1;
			this.mnu_ShowScatterLogarithmic.Text = "View as Logarithmic Data";
			this.mnu_ShowScatterLogarithmic.Click += new System.EventHandler(this.mnu__show_scatter_logarithmic_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "-";
			// 
			// mnu_ShowData
			// 
			this.mnu_ShowData.Index = 3;
			this.mnu_ShowData.Text = "Show Plot Points ";
			this.mnu_ShowData.Click += new System.EventHandler(this.mnu__showData_Click);
			// 
			// mnu_HideData
			// 
			this.mnu_HideData.Index = 4;
			this.mnu_HideData.Text = "Hide Plot Points";
			this.mnu_HideData.Click += new System.EventHandler(this.mnu__hideData_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 5;
			this.menuItem7.Text = "-";
			// 
			// mnu_ShowRSquared
			// 
			this.mnu_ShowRSquared.Index = 6;
			this.mnu_ShowRSquared.Text = "Show Only R-Squared Value Text";
			this.mnu_ShowRSquared.Click += new System.EventHandler(this.mnu__showRSquared_Click);
			// 
			// mnu_ShowEquation
			// 
			this.mnu_ShowEquation.Index = 7;
			this.mnu_ShowEquation.Text = "Show Only Equation Text";
			this.mnu_ShowEquation.Click += new System.EventHandler(this.mnu_ShowEquation_Click);
			// 
			// mnu_ShowBothText
			// 
			this.mnu_ShowBothText.Index = 8;
			this.mnu_ShowBothText.Text = "Show Both R-Squared/Equation Text";
			this.mnu_ShowBothText.Click += new System.EventHandler(this.menuShowBothText_Click);
			// 
			// mnu_HideBothEqnR
			// 
			this.mnu_HideBothEqnR.Index = 9;
			this.mnu_HideBothEqnR.Text = "Hide All Text";
			this.mnu_HideBothEqnR.Click += new System.EventHandler(this.mnu_HideBothEqnR_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 10;
			this.menuItem5.Text = "-";
			// 
			// mnu_Save
			// 
			this.mnu_Save.Index = 11;
			this.mnu_Save.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnu_SaveAsTiff,
																					 this.mnu_SaveAsWmf,
																					 this.mnu__saveAsEmf,
																					 this.mnu__saveAsGif,
																					 this.mnu__saveAsJpg,
																					 this.mnu__saveAsBmp,
																					 this.mnu__saveAsPng,
																					 this.menuItem6,
																					 this.mnu__copyToClipboard});
			this.mnu_Save.Text = "Save Image";
			// 
			// mnu_SaveAsTiff
			// 
			this.mnu_SaveAsTiff.Index = 0;
			this.mnu_SaveAsTiff.Text = "as .tiff";
			this.mnu_SaveAsTiff.Click += new System.EventHandler(this.mnu__saveAsTiff_Click);
			// 
			// mnu_SaveAsWmf
			// 
			this.mnu_SaveAsWmf.Index = 1;
			this.mnu_SaveAsWmf.Text = "as .wmf";
			this.mnu_SaveAsWmf.Click += new System.EventHandler(this.mnu__saveAsWmf_Click);
			// 
			// mnu__saveAsEmf
			// 
			this.mnu__saveAsEmf.Index = 2;
			this.mnu__saveAsEmf.Text = "as .emf";
			this.mnu__saveAsEmf.Click += new System.EventHandler(this.mnu__saveAsEmf_Click);
			// 
			// mnu__saveAsGif
			// 
			this.mnu__saveAsGif.Index = 3;
			this.mnu__saveAsGif.Text = "as .gif";
			this.mnu__saveAsGif.Click += new System.EventHandler(this.mnu__saveAsGif_Click);
			// 
			// mnu__saveAsJpg
			// 
			this.mnu__saveAsJpg.Index = 4;
			this.mnu__saveAsJpg.Text = "as .jpg";
			this.mnu__saveAsJpg.Click += new System.EventHandler(this.mnu__saveAsJpg_Click);
			// 
			// mnu__saveAsBmp
			// 
			this.mnu__saveAsBmp.Index = 5;
			this.mnu__saveAsBmp.Text = "as .bmp";
			this.mnu__saveAsBmp.Click += new System.EventHandler(this.mnu__saveAsBmp_Click);
			// 
			// mnu__saveAsPng
			// 
			this.mnu__saveAsPng.Index = 6;
			this.mnu__saveAsPng.Text = "as .png";
			this.mnu__saveAsPng.Click += new System.EventHandler(this.mnu__saveAsPng_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 7;
			this.menuItem6.Text = "-";
			// 
			// mnu__copyToClipboard
			// 
			this.mnu__copyToClipboard.Index = 8;
			this.mnu__copyToClipboard.Text = "Copy To Clipboard";
			this.mnu__copyToClipboard.Click += new System.EventHandler(this.mnu__copyToClipboard_Click);
			// 
			// mnu_SaveData
			// 
			this.mnu_SaveData.Index = 12;
			this.mnu_SaveData.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.mnu_SaveRSquared,
																						 this.mnu_SaveEquation,
																						 this.mnu_SaveBothEqnR,
																						 this.menuItem2,
																						 this.mnu_SaveRSquaredLog,
																						 this.mnu_SaveLogEqn,
																						 this.mnu_SaveLogEqnRSq,
																						 this.menuItem8,
																						 this.mnu_SaveAll});
			this.mnu_SaveData.Text = "Save Data";
			// 
			// mnu_SaveRSquared
			// 
			this.mnu_SaveRSquared.Index = 0;
			this.mnu_SaveRSquared.Text = "Save Normal R-Squared Matrix";
			this.mnu_SaveRSquared.Click += new System.EventHandler(this.mnu_SaveRSquared_Click);
			// 
			// mnu_SaveEquation
			// 
			this.mnu_SaveEquation.Index = 1;
			this.mnu_SaveEquation.Text = "Save Normal Equation Matrix";
			this.mnu_SaveEquation.Click += new System.EventHandler(this.mnu_SaveEquation_Click);
			// 
			// mnu_SaveBothEqnR
			// 
			this.mnu_SaveBothEqnR.Index = 2;
			this.mnu_SaveBothEqnR.Text = "Save Normal Equation and R-Squared";
			this.mnu_SaveBothEqnR.Click += new System.EventHandler(this.mnu_SaveBothEqnR_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 3;
			this.menuItem2.Text = "-";
			// 
			// mnu_SaveRSquaredLog
			// 
			this.mnu_SaveRSquaredLog.Index = 4;
			this.mnu_SaveRSquaredLog.Text = "Save Log R-Squared Matrix";
			this.mnu_SaveRSquaredLog.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// mnu_SaveLogEqn
			// 
			this.mnu_SaveLogEqn.Index = 5;
			this.mnu_SaveLogEqn.Text = "Save Log Equation Matrix";
			this.mnu_SaveLogEqn.Click += new System.EventHandler(this.mnu_SaveLogEqn_Click);
			// 
			// mnu_SaveLogEqnRSq
			// 
			this.mnu_SaveLogEqnRSq.Index = 6;
			this.mnu_SaveLogEqnRSq.Text = "Save Log Equation and R-Squared";
			this.mnu_SaveLogEqnRSq.Click += new System.EventHandler(this.mnu_SaveLogEqnRSq_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 7;
			this.menuItem8.Text = "-";
			// 
			// mnu_SaveAll
			// 
			this.mnu_SaveAll.Index = 8;
			this.mnu_SaveAll.Text = "Save All";
			this.mnu_SaveAll.Click += new System.EventHandler(this.mnu_SaveAll_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 13;
			this.menuItem4.Text = "-";
			// 
			// mnu_FitToScreen
			// 
			this.mnu_FitToScreen.Index = 14;
			this.mnu_FitToScreen.Text = "Fit To Screen";
			this.mnu_FitToScreen.Click += new System.EventHandler(this.mnu_FitToScreen_Click);
			// 
			// mnu_Refresh
			// 
			this.mnu_Refresh.Index = 15;
			this.mnu_Refresh.Text = "Refresh";
			this.mnu_Refresh.Click += new System.EventHandler(this.mnu__refresh_Click);
			// 
			// mnu_Properties
			// 
			this.mnu_Properties.Index = 16;
			this.mnu_Properties.Text = "Properties";
			this.mnu_Properties.Click += new System.EventHandler(this.mnu__properties_Click);
			// 
			// m_scatter
			// 
			this.m_scatter.AlignHorizontal = null;
			this.m_scatter.AlignVertical = null;
			this.m_scatter.AutoComputeDataPointColor = true;
			this.m_scatter.AutoScaleColor = true;
			this.m_scatter.AxisPadding = 3;
			this.m_scatter.CommonSets = null;
			this.m_scatter.ContextMenu = this.m_contextMenu;
			this.m_scatter.DataColor = System.Drawing.Color.Black;
			this.m_scatter.DataColorAlternative = System.Drawing.Color.White;
			this.m_scatter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_scatter.EndColumn = 0;
			this.m_scatter.EndRow = 0;
			this.m_scatter.Legend = null;
			this.m_scatter.LinearFitParameters = null;
			this.m_scatter.Location = new System.Drawing.Point(141, 56);
			this.m_scatter.Name = "m_scatter";
			this.m_scatter.RSquaredFont = new System.Drawing.Font("Times New Roman", 8F);
			this.m_scatter.ScatterPlotData = null;
			this.m_scatter.ShowEquation = false;
			this.m_scatter.ShowRSquaredValue = true;
			this.m_scatter.ShowScatterData = true;
			this.m_scatter.Size = new System.Drawing.Size(571, 419);
			this.m_scatter.StartColumn = 0;
			this.m_scatter.StartRow = 0;
			this.m_scatter.TabIndex = 7;
			this.m_scatter.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			// 
			// ctlScatterPlot
			// 
			this.Controls.Add(this.m_scatter);
			this.Name = "ctlScatterPlot";
			this.ShowProgBar = true;
			this.Size = new System.Drawing.Size(752, 600);
			this.AlignVertical += new PNNLControls.ctlHierarchalLabel.AlignDelegate(this.ctlScatter_AlignVertical);
			this.AlignHorizontal += new PNNLControls.ctlHierarchalLabel.AlignDelegate(this.ctlScatter_AlignHorizontal);
			this.Controls.SetChildIndex(this.splitterHorizontal, 0);
			this.Controls.SetChildIndex(this.splitterVertical, 0);
			this.Controls.SetChildIndex(this.pnlHeader, 0);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.hLabelVert, 0);
			this.Controls.SetChildIndex(this.legend, 0);
			this.Controls.SetChildIndex(this.m_scatter, 0);
			this.panel1.ResumeLayout(false);
			this.pnlSE.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		#region Helper Methods
		
		private void FitToScreen()
		{
			if (m_scatter == null)
				return;

			int numDatasets = Convert.ToInt32(m_scatter.NumberOfDatasets);
			if (m_scatter.NumberOfDatasets <= 0)
				return;

			mnu_FitToScreen.Checked = true;

			int width  = this.Width - this.VerticalLabel.Width - this.legend.Width;
			int height = this.Height - this.pnlHeader.Height - this.HorizontalLabel.Height;
 
			int lblWidth = width / numDatasets;
			int lblHeight = height / numDatasets;
            
			VerticalLabel.MinLeafHeight   = lblHeight;
			HorizontalLabel.MinLeafWidth  = lblWidth;
			VerticalLabel.RedrawLabel();
			HorizontalLabel.RedrawLabel();
			OnRefresh();
		}

		private void CloneAttr(clsLabelAttributes destRoot, clsLabelAttributes srcRoot)
		{
			if (srcRoot == null || srcRoot.branches == null)
			{
				return;
			}

			for(int i = 0; i < srcRoot.branches.Count; i++)
			{
				clsLabelAttributes srcChild  = srcRoot.branches[i] as clsLabelAttributes;
				clsLabelAttributes destChild = srcChild.Clone() as clsLabelAttributes;
				destRoot.AddBranch(destChild);
				CloneAttr(destChild, ((clsLabelAttributes) srcRoot.branches[i]));
			}
		}

		public void InitScatterPlot(float[,] data, clsLabelAttributes lbl)
		{						
			
			VerticalLabel.ContextMenu   = new ContextMenu();
			HorizontalLabel.ContextMenu = new ContextMenu();
			VerticalLabel.Root = lbl;

			ArrayList leaves = this.VerticalLabel.GetLeaves();
			VerticalLabel.Init();					
			
			clsLabelAttributes hlblRoot;
			hlblRoot = lbl.Clone() as clsLabelAttributes;
			CloneAttr(hlblRoot, lbl);
			
			HorizontalLabel.Root = hlblRoot;	
			HorizontalLabel.Init();
					
			int leaveCount = leaves.Count;
			
			StartRow    = 0;
			EndRow      = leaveCount - 1; 
			StartColumn = 0;
			EndColumn   = leaveCount - 1; 
			
			HorizontalLabel.LoadLabels(0, leaveCount - 1);
			VerticalLabel.LoadLabels(0,leaveCount - 1);
									
			/*
			 *  Finally, set the scatterplot data			
			 */ 				
			
			this.pnlHeader.Visible = true;
			AutoScaleColor  = true;
			ScatterPlotData = data;			
			
			StartRow    = 0;
			EndRow      = leaveCount - 1;
			StartColumn = 0;
			EndColumn   = leaveCount - 1;

			FitToScreen();
		}
									
		/// <summary>
		/// Displays a thumbnail image.
		/// </summary>
		private void DisplayThumbnail()
		{						
			if (m_showThumbnail != true)
				return;

			try
			{
				Image i = m_scatter.GetThumbnail(picLeft.Width, picLeft.Height);	
				if (i != null)
				{				
					if (picLeft.Image != null)
						picLeft.Image.Dispose();
					picLeft.Image = i;			
					picLeft.Refresh();					
				}
			}
			catch(Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
		}
			
		/// <summary>
		/// Calculates the data range (x,y) for the supplied scatter plot data.
		/// </summary>
		public void AutoRangeData()
		{
			m_scatter.AutoRangeData();
		}		
				
		/// <summary>
		/// Creates a bitmap with the scatter plot data labels.
		/// </summary>		
		/// <param name="bmp">Bitmap of scatter plot to render labels to.</param>
		/// <returns>Rendered bitmap with labels attached.</returns>
		private Bitmap RenderLabeledBitmap(Bitmap bmp)
		{
			/* 
			 * Figure out how many lables need to be fitted onto bitmap.
			 * use the graphics context from the bitmap argument 
			 */					
			Graphics gbmp = Graphics.FromImage(bmp);					
			clsLabelPlotter fontPlotter = new clsLabelPlotter();					
			fontPlotter.IsVertical		= false;
													
			/* 
			 * Make sure that we have labels to plot, otherwise bail.
			 */
			float maxFontSize = 0;				

			// Create an arbitrary box size to test font bounds against.
			Rectangle sizeBox = new Rectangle(0,
				0,
				Math.Abs(m_scatter.AlignHorizontal[0] - m_scatter.AlignHorizontal[1]),
				Math.Abs(m_scatter.AlignVertical[0] - m_scatter.AlignVertical[1])
				);	
																											
			/*
			 *	 Find the max height and the max Width 
			 */	
			ArrayList leaves = HorizontalLabel.GetLeaves();			
			for(int i = 0; i < leaves.Count; i++)
			{										
				fontPlotter.Bounds		= sizeBox;
				PNNLControls.clsLabelAttributes label = leaves[i] as clsLabelAttributes;
				if (label != null)
				{	
					fontPlotter.Label	= label.text;					 		
					maxFontSize			= Math.Max(maxFontSize, fontPlotter.GetBestFontSize(gbmp));			
				}				
			}
			
			/*
			 * Create the bitmap with labels. 
			 */ 
			int width  = bmp.Width  + sizeBox.Width; 
			int height = bmp.Height + sizeBox.Height;
								
			Bitmap newBmp = new Bitmap(width, height, bmp.PixelFormat);								
			Graphics graphics			= Graphics.FromImage(newBmp);
			graphics.InterpolationMode	= System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;										
			graphics.DrawImageUnscaled(bmp, sizeBox.Width,0); 
			graphics.TextRenderingHint	= System.Drawing.Text.TextRenderingHint.AntiAlias; 
						
			for(int i = 0; i < leaves.Count; i++)
			{																	
				Font font	   		   = new Font(FontFamily.GenericSerif, maxFontSize);
				StringFormat strFormat = new StringFormat();					
									
				// Create the bounds for rendering the string to.	
				Rectangle plotBoundsV = new Rectangle(0,
					m_scatter.AlignVertical[i],
					sizeBox.Width,
					Math.Abs(m_scatter.AlignVertical[i] - m_scatter.AlignVertical[i + 1]));			
				Rectangle plotBoundsH = new Rectangle(m_scatter.AlignHorizontal[i] + sizeBox.Width,
					bmp.Height,
					Math.Abs(m_scatter.AlignHorizontal[i] - m_scatter.AlignHorizontal[i+1]),
					sizeBox.Height);

				/*
				 *	Draw labels onto bitmap.
				 */
				PNNLControls.clsLabelAttributes label = leaves[i] as clsLabelAttributes;
				if (label != null)
				{
					graphics.FillRectangle( new System.Drawing.SolidBrush(Color.White), plotBoundsV);
					graphics.FillRectangle( new System.Drawing.SolidBrush(Color.White), plotBoundsH);

					graphics.DrawString(label.text, font, Brushes.Black, plotBoundsV, strFormat);							
					graphics.DrawString(label.text, font, Brushes.Black, plotBoundsH, strFormat);																		
				}
				font.Dispose();
				strFormat.Dispose();				
			}			
			return newBmp;
		}

		
		/// <summary>
		/// Calls for the bitmap to be re-rendered and updates the thumbnail image.
		/// </summary>
		private void OnRefresh()
		{			
			mnu_Save.Enabled = false;
			try
			{
				//System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
				//Console.WriteLine(System.Threading.Thread.CurrentThread.Name + "'s stack frame:  " + trace.ToString());
				m_scatter.OnRefresh();				
			}
			catch(Exception ex)
			{
				System.Console.WriteLine("On referesh exception: " + ex.Message);
			}
		}

		#endregion
						
		#region ScatterPlot Events
		private void OnUpdateProgress(bool show, int percent, string message)
		{			
			this.StatBar.Text = message;
			this.ProgBar.Visible = show;
			this.ProgBar.Value = percent;
			this.ProgBar.Refresh();
		}

		/// <summary>
		/// Updates the thumbnail image, the paint area, and the progressbar.
		/// </summary>
		/// <param name="percent"></param>
		private void m_scatter_OnPercentDrawn(int percent)
		{
			try
			{
				Invoke(new UpdateProgress(OnUpdateProgress), new object [] {true, percent, "Rendering Scatterplot."});				
				m_scatter.Refresh();			
				DisplayThumbnail();
			}
			catch(System.InvalidOperationException ex)
			{
				System.Console.WriteLine(ex.Message);
			}		
		}

        private void OnDrawingComplete()
        {
            this.ProgBar.Value = 0;
            this.ProgBar.Visible = false;
            this.ProgBar.Refresh();
            StatBar.Visible = false;

            m_scatter.Refresh();
            DisplayThumbnail();
            mnu_Save.Enabled = true;
        }

		/// <summary>
		/// Caught when all rendering threads are finished drawing.
		/// </summary>
		private void m_scatter_OnDrawingComplete()
		{
            if (InvokeRequired == true)
            {
                Invoke(new DelegateCalculationComplete(OnDrawingComplete));
            }
            else
            {
                OnDrawingComplete();
            }
		}
		private void m_scatter_OnPlotClicked(int startRow, int endRow, int startColumn, int endColumn, Point p, bool selected)
		{
			if (this.OnSelectedPlot != null)
			{
				OnSelectedPlot(startRow, endRow, startColumn, endColumn, selected);
			}
		}
		#endregion
		
		#region Dataframe Events 
		/// <summary>
		/// Event captured to re-render the bitmap so the labels match the data displayed.
		/// </summary>
		private void ctlScatterPlot_LabelsUpdated()
		{			
			System.Diagnostics.Trace.WriteLine("\tScatterPlot: LabelsUpdated");
			if(Visible) 
			{					
				m_prevWidth  = Width;
				m_prevHeight = Height;
				//m_redraw = true;	
				System.Diagnostics.Trace.WriteLine("\t\tScatterPlot: LabelsUpdatd - calling refresh");
				OnRefresh();
			}
		}

		/// <summary>
		/// Arrays for handling the horizontal alignment.
		/// </summary>
		/// <param name="positions"></param>
		private void ctlScatter_AlignHorizontal(int[] positions)
		{
			
			System.Diagnostics.Trace.Write("\tScatterPlot: HAlignment { ");
			for( int i = 0; i < positions.Length; i++)
			{
				System.Diagnostics.Trace.Write(positions[i].ToString() + ",");
			}
			System.Diagnostics.Trace.WriteLine("}");
			m_scatter.AlignHorizontal = positions;						
		}

		/// <summary>
		/// Arrays for handling the vertical alignment.
		/// </summary>
		/// <param name="positions"></param>
		private void ctlScatter_AlignVertical(int[] positions)
		{
			System.Diagnostics.Trace.Write("\tScatterPlot: VAlignment{");
			for( int i = 0; i < positions.Length; i++)
			{
				System.Diagnostics.Trace.Write(positions[i].ToString() + ",");
			}
			System.Diagnostics.Trace.WriteLine("}");
			m_scatter.AlignVertical = positions;						
		}
					
		/// <summary>
		/// Handles update for setting client control location.  Enables us to render the whole bitmap at once.
		/// </summary>
		/// <param name="location"></param>
		private void ctlScatterPlot_ClientLocation(Point location)
		{
			System.Diagnostics.Trace.WriteLine("\tScatterPlot: Location " + location.ToString());
			m_scatter.Dock = DockStyle.None;
			m_scatter.SendToBack();
			m_scatter.Location = location;	
		}

		/// <summary>
		/// Changes the size of the client bitmap to be rendered.
		/// </summary>
		/// <param name="size"></param>		
		private void ctlScatterPlot_ClientSize(Size size)
		{			
			m_scatter.Size = size;
		}

		/// <summary>
		/// Event handler for when the legend values change.  Forces a refresh of the scatterplot.
		/// </summary>
		private void legend_LegendChanged()
		{
			OnRefresh();
		}		
		#endregion
		
		#region Context Menu Events / Methods 
		private void mnu__properties_Click(object sender, EventArgs e)
		{						
			frmPropertyGrid fpg = new frmPropertyGrid();
			fpg.SelectedObject  = this;			
			fpg.Owner			= this.FindForm(); 
			DialogResult propertyResults = fpg.ShowDialog();
			if (propertyResults == DialogResult.OK)
			{
				OnRefresh();
			}
			fpg.Dispose();
		}
		
		private void UpdateContextMenuUI(bool showEquation, bool showRSquared)
		{
			mnu_ShowRSquared.Enabled = showRSquared == false;						
			mnu_ShowEquation.Enabled  = showEquation == false;			
			mnu_ShowBothText.Enabled  = showEquation == false;
			m_scatter.ShowRSquaredValue   = showRSquared;
			m_scatter.ShowEquation        = showEquation;			
			mnu_HideBothEqnR.Enabled  = (showEquation == showRSquared) == false;
		}

		private void mnu__show_scatter_normal_Click(object sender, EventArgs e)
		{
			mnu_ShowScatterLogarithmic.Enabled   = true;
			mnu_ShowScatterNormal.Enabled		= false;
			m_scatter.ShowNormal();						
		}

		private void mnu__show_scatter_logarithmic_Click(object sender, EventArgs e)
		{			
			mnu_ShowScatterLogarithmic.Enabled   = false;
			mnu_ShowScatterNormal.Enabled		= true;

			m_scatter.ShowLogarithmic();				
		}
		/// <summary>
		/// Shows data on chart.  
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__showData_Click(object sender, System.EventArgs e)
		{
			mnu_ShowData.Enabled = false;
			mnu_HideData.Enabled = true;
			m_scatter.ShowScatterData = true;			
			OnRefresh();	
		}

		/// <summary>
		/// Hides data on chart.  
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__hideData_Click(object sender, System.EventArgs e)
		{
			mnu_ShowData.Enabled = true;
			mnu_HideData.Enabled = false;
			m_scatter.ShowScatterData = false;
			OnRefresh();
		}

		/// <summary>
		/// Hides both equation and r-squared value texts on the scatter subplots
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu_HideBothEqnR_Click(object sender, System.EventArgs e)
		{
			UpdateContextMenuUI(false,false);
			OnRefresh();	
		}

		private void mnu_ShowEquation_Click(object sender, System.EventArgs e)
		{
			UpdateContextMenuUI(true,false);
			OnRefresh();	
		}

		/// <summary>
		/// Sets to show the r-Squared value on the chart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__showRSquared_Click(object sender, System.EventArgs e)
		{
			UpdateContextMenuUI(false,true);
			OnRefresh();		
		}		

		private void mnu__refresh_Click(object sender, System.EventArgs e)
		{
			OnRefresh();
		}

		private void mnu_SaveRSquared_Click(object sender, System.EventArgs e)
		{
			SaveCorelationData(true,false, LinearParameterSave.NORMAL);	
		}

		private void mnu_SaveEquation_Click(object sender, System.EventArgs e)
		{
			SaveCorelationData(false, true, LinearParameterSave.NORMAL);	
		}

		private void mnu_SaveBothEqnR_Click(object sender, System.EventArgs e)
		{
			SaveCorelationData(true,true, LinearParameterSave.NORMAL);	
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			SaveCorelationData(true,false, LinearParameterSave.LOG);	
		}

		private void mnu_SaveLogEqn_Click(object sender, System.EventArgs e)
		{
			SaveCorelationData(false,true, LinearParameterSave.LOG);	
		}

		private void mnu_SaveLogEqnRSq_Click(object sender, System.EventArgs e)
		{
			SaveCorelationData(true,true, LinearParameterSave.LOG);	
		}

		private void mnu_SaveAll_Click(object sender, System.EventArgs e)
		{
			SaveCorelationData(true,true, LinearParameterSave.ALL);	
		}
		
				
		/// <summary>
		/// Saves the scatterplot as a bitmap.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__saveAsBmp_Click(object sender, System.EventArgs e)
		{
			Save(System.Drawing.Imaging.ImageFormat.Bmp, "Bitmap (*.bmp)|*.bmp", "bmp");																
		}

		/// <summary>
		/// Saves the scatterplot as a jpeg.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__saveAsJpg_Click(object sender, System.EventArgs e)
		{
			Save(System.Drawing.Imaging.ImageFormat.Jpeg, "Jpeg (*.jpg)|*.jpg", "jpg");		
		}


		/// <summary>
		/// Saves the scatterplot as a gif.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__saveAsGif_Click(object sender, System.EventArgs e)
		{			
			Save(System.Drawing.Imaging.ImageFormat.Gif, "Gif (*.gif)|*.gif", "gif");
		}

		
		/// <summary>
		/// Saves the scatterplot as a gif.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__saveAsWmf_Click(object sender, System.EventArgs e)
		{			
			Save(System.Drawing.Imaging.ImageFormat.Wmf, "Wmf (*.wmf)|*.wmf", "wmf");			
		}

		
		/// <summary>
		/// Saves the scatterplot as a tiff.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__saveAsTiff_Click(object sender, System.EventArgs e)
		{			
			Save(System.Drawing.Imaging.ImageFormat.Tiff, "Tiff (*.tiff)|*.tiff", "tiff");
		}

		
		/// <summary>
		/// Saves the scatterplot as an emf.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__saveAsEmf_Click(object sender, System.EventArgs e)
		{
			Save(System.Drawing.Imaging.ImageFormat.Emf, "Emf (*.emf)|*.emf", "emf");			
		}

		/// <summary>
		/// Saves the scatterplot as a png.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__saveAsPng_Click(object sender, System.EventArgs e)
		{
			Save(System.Drawing.Imaging.ImageFormat.Png, "Png (*.png)|*.png", "png");
		}


		/// <summary>
		/// Copies the scatterplot to the clipboard.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnu__copyToClipboard_Click(object sender, System.EventArgs e)
		{
			Bitmap btmp = m_scatter.ToBitmap();			
			DataObject d = new DataObject();
			d.SetData(System.Windows.Forms.DataFormats.Bitmap, btmp);
			Clipboard.SetDataObject(d);
		}
			
		#endregion
		
		#region Data Saving 				
		private void SaveCorelationData(bool saveRSquared, bool saveEquations, LinearParameterSave type)
		{
			SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();				
			fileDialog.AddExtension = true;
			fileDialog.CheckPathExists = true;
			fileDialog.DefaultExt = ".csv";
			fileDialog.DereferenceLinks = true;
			fileDialog.ValidateNames = true;
			fileDialog.Filter = "Comma Separated Value (*.csv)|*.csv\", \"csv\")";
			fileDialog.OverwritePrompt = true;
			fileDialog.FilterIndex = 1;

			if (fileDialog.ShowDialog() != DialogResult.OK) 
			{
				return;
			}
			
			System.IO.TextWriter writer;
			try
			{
				writer = new System.IO.StreamWriter(fileDialog.FileName);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
				return;
			}

			if (type == LinearParameterSave.ALL)
			{
				SaveCorrelationDataR(writer,true, true,  LinearParameterSave.NORMAL);
				SaveCorrelationDataR(writer,true, true,  LinearParameterSave.LOG);
			}
			else
			{
				SaveCorrelationDataR(writer,saveRSquared,saveEquations,type);
			}						
			writer.Close();
		}



		private void SaveCorrelationDataR(System.IO.TextWriter writer, bool saveRSquared, bool saveEquations, LinearParameterSave type)
		{
			string currentLine;
			
			if (saveRSquared)
			{								
				float [,] data;				
				if (type == LinearParameterSave.NORMAL)
				{					
					writer.WriteLine("Normal R-Squared");
					data = m_scatter.LinearFitNormal;
				}
				else
				{
					writer.WriteLine("Logarithmic R-Squared");
					data = m_scatter.LinearFitLog;		
				}
				
				for(int i = 0 ; i < m_scatter.NumberOfDatasets; i++)
				{
					currentLine = "";
					for (int j = 0; j < m_scatter.NumberOfDatasets; j++)
					{
						currentLine += "," + String.Format("{0:0.00}",data[j*m_scatter.NumberOfDatasets + i,(int) LinearParameters.RSQUARED]);
					}	
					writer.WriteLine(currentLine);					
				}
			}

			if (saveEquations)
			{
				float [,] data;				
				if (type == LinearParameterSave.NORMAL)
				{					
					writer.WriteLine("Normal Equation");
					data = m_scatter.LinearFitNormal;
				}
				else
				{
					writer.WriteLine("Logarithmic Equation");
					data = m_scatter.LinearFitLog;		
				}
				
				for(int i = 0 ; i < m_scatter.NumberOfDatasets; i++)
				{
					currentLine = "";
					for (int j = 0; j < m_scatter.NumberOfDatasets; j++)
					{
						currentLine += "," + String.Format("y={0:0.00}x + {1:0.00}",
							data[j*m_scatter.NumberOfDatasets + i,(int) LinearParameters.SLOPE],
							data[j*m_scatter.NumberOfDatasets + i,(int) LinearParameters.INTERCEPT]);
					}	
					writer.WriteLine(currentLine);					
				}
			}						
		}		
		/// <summary>
		/// Saves the rendered bitmap from the client to a file.
		/// </summary>
		/// <param name="format">Image Format</param>
		/// <param name="selector"></param>
		/// <param name="defaultExtension">Image File Extension</param>
		private void Save(System.Drawing.Imaging.ImageFormat format, String selector, String defaultExtension) 
		{
			try 
			{
				// Save a file - get the filename
				SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();				
				fileDialog.AddExtension = true;
				fileDialog.CheckPathExists = true;
				fileDialog.DefaultExt = defaultExtension;
				fileDialog.DereferenceLinks = true;
				fileDialog.ValidateNames = true;
				fileDialog.Filter = selector;
				fileDialog.OverwritePrompt = true;
				fileDialog.FilterIndex = 1;

				if (fileDialog.ShowDialog() != DialogResult.OK) 
				{
					return;
				}

				/* 
				 * Get the bitmap from the scatter plot client and create another bitmap. 
				 */
				Bitmap bmp		   = m_scatter.ToBitmap();	
				Bitmap labeledBmp  = RenderLabeledBitmap(bmp); 
			
				frmSaveDPI frmSave = new frmSaveDPI();				
				frmSave.NonLabeledBitmap = bmp;												
				frmSave.Owner = ParentForm;
				frmSave.LabeledBitmap    = labeledBmp;  
				frmSave.DPI	   = 96.0f;				
				frmSave.Text   += " for " + fileDialog.FileName;

				System.Windows.Forms.DialogResult diagResult = frmSave.ShowDialog();								
				if (diagResult == System.Windows.Forms.DialogResult.OK)
				{							
					Bitmap bmpSave = frmSave.Bitmap;					
					bmpSave.Save(fileDialog.FileName, format);						
				}

				frmSave.Dispose();
				
			}
			catch (Exception e) 
			{
				MessageBox.Show("Save failed: " + e.Message);
			}
		}



		#endregion				

		#region This Events 
		/// <summary>
		/// Handles resize event from system.
		/// </summary>
		/// <param name="sender">control initiating call</param>
		/// <param name="e">event arguments for resize.</param>
		private void ctlScatterPlot_Resize(object sender, EventArgs e)
		{
			/*
			 * Redraw if the labelsupdated have been called.  Redraw here to avoid redrawing twice.
			 * Wait for the control to be resized before handling XY resizes from base controls.
			 */
			{				
				// Cause the base to repaint itself.
				if (mnu_FitToScreen.Checked == true)
				{
					FitToScreen();
				}
				else
				{
					UpdateControlSize();
				}
				//m_redraw = false;
			}
		}
		
		private void ctlScatterPlot_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible && m_firstDisplay == false)
			{
				OnRefresh();			
				//m_redraw = false;
				m_firstDisplay = true;
			}						
		}
				
		#endregion
		
		#region This Properties		

		/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Color of plot points to use.")]
		public Color DataPointColor
		{
			get
			{
				return m_scatter.DataColor;
			}
			set
			{
				m_scatter.DataColor = value;
			}
		}
		/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets / Sets the start displayable row.")]
		public int StartRow
		{
			get
			{
				return m_scatter.StartRow;
			}
			set
			{
				m_scatter.StartRow = value;
			}
		}

		/// <summary>
		/// Gets/Sets the Font to use for the text on each subplot.
		/// </summary>		
		[Category("Plot Appearance")]
		[Description("Gets / Sets the font for the subplot text.")]
		public Font TextFont
		{
			get
			{
				return m_scatter.TextFont;
			}
			set
			{
				m_scatter.TextFont = value;
			}
		}
		
		
		/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets / Sets the end displayable row.")]
		public int EndRow
		{
			get
			{
				return m_scatter.EndRow;
			}
			set
			{
				m_scatter.EndRow = value;
			}
		}

		
		/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets / Sets the start displayable Column.")]
		public int StartColumn
		{
			get
			{
				return m_scatter.StartColumn;
			}
			set
			{
				m_scatter.StartColumn= value;
			}
		}/// <summary>
		/// Get/Set the color of the data point.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets / Sets the end displayable Column.")]
		public int EndColumn
		{
			get
			{
				return m_scatter.EndColumn;
			}
			set
			{
				m_scatter.EndColumn= value;
			}
		}



		/// <summary>
		/// Gets/Sets whether to auto compute the data color or not.
		/// </summary>
		[Category("Plot Appearance")]
		[Description("Gets/Sets whether to auto compute the data point color or not.  False will make every plot color equal to DataPointColor")]
		public bool AutoComputeDataPointColor
		{
			get
			{
				return m_scatter.AutoComputeDataPointColor;
			}
			set
			{
				m_scatter.AutoComputeDataPointColor = value;
			}
		}

		
		[Category("Plot Appearance")]
		[Description("Determines whether to autoscale the color according to the data used.")]
		public bool AutoScaleColor
		{
			get
			{
				return m_scatter.AutoScaleColor;
			}
			set 
			{
				m_scatter.AutoScaleColor = value;
			}
		}

		
		[Category("Scatter Data")]
		[Description("Scatter plot data in 2-D form.")]
		public float[,] ScatterPlotData
		{
			get
			{
				return m_scatter.ScatterPlotData;
			}
			set
			{
				m_scatter.ScatterPlotData = value;				
				mnu_ShowScatterLogarithmic.Enabled = true;
				mnu_ShowScatterNormal.Enabled		 = false;

				if (Visible)
				{
					if (value != null)
					{
						m_selectedColumn = new bool[value.GetUpperBound(1) + 1];
						m_selectedRow    = new bool[value.GetUpperBound(1) + 1];
					}
					OnRefresh();
				}
			}
		}
		[Category("Plot Appearance")]
		[Description("Returns the number of visible rows.")]
		public int NumberOfRows
		{
			get
			{
				return m_scatter.NumberOfRows;
			}			
		}
		
		[Category("Plot Appearance")]
		[Description("Returns the number of visible columns.")]
		public int NumberOfColumns
		{
			get
			{
				return m_scatter.NumberOfColumns;
			}
		}

		/// <summary>
		/// Get/Set whether to show the header/progress bar or not.
		/// </summary>
		public bool ShowThumbnail
		{
			get
			{
				return m_showThumbnail; 
			}
			set
			{
				m_showThumbnail = value;
			}
		}
				
		/// <summary>
		/// Gets/Sets the axis padding of how far to draw the axis lines from the edge of the scatter plot box.
		/// </summary>
		/// 
		[Category("Plot Appearance")]
		[Description("Padding to use between each plot axis and its respective neighbors plot axes.")]
		public int AxisPadding
		{
			get
			{
				return m_scatter.AxisPadding;
			}
			set
			{
				m_scatter.AxisPadding = value;
			}
		}

		[Category("Plot Appearance")]
		[Description("Get/Set how big the label columns should be.")]
		public int LabelColumnsHeight
		{
			get
			{
				return Height; // - bottomSplitter.Top;
			}
			set
			{
				if (value > 0)
				{
					//bottomSplitter.Top = Height - value;
					//OnRefresh();
				}
			}
		}
		[Category("Plot Appearance")]
		[Description("Get/Set how tall the vertical labels should be.")]
		public int LabelColumnsWidth
		{
			get
			{
				return HorizontalLabel.MinLeafHeight;
			}
			set
			{
				HorizontalLabel.MinLeafHeight = value;
			}
		}

		[Category("Plot Appearance")]
		[Description("Get/Set how tall the label columns should be.")]
		public int LabelRowsHeight
		{
			get
			{
				return VerticalLabel.MinLeafHeight;
			}
			set
			{
				VerticalLabel.MinLeafHeight = value;
			}
		}

		[Category("Plot Appearance")]
		[Description("Get/Set how wide the row labels should be.")]
		public int LabelRowsWidth
		{
			get
			{
				return this.VerticalLabel.Width;
			}
			set
			{
				if (value > 0)
				{
					this.VerticalLabel.Width = value;
					OnRefresh();
				}
			}
		}
	
		
		[Category("Plot Appearance")]
		[Description("Get/Set whether to show the R-Squared value on the plot.")]
		public bool ShowRSquared
		{
			get
			{
				return m_scatter.ShowRSquaredValue;
			}
			set
			{
				m_scatter.ShowRSquaredValue = value;
			}
		}

		[Category("Plot Appearance")]
		[Description("Get/Set whether to show the linear regression equation on the plot.")]		
		public bool ShowEquation
		{
			get
			{
				return m_scatter.ShowEquation;
			}
			set
			{
				m_scatter.ShowEquation = value;
			}
		}

		[Category("Plot Appearance")]
		[Description("Get/Set whether to show the color legend.")]		
		public bool ShowLegend
		{
			get
			{
				return Legend.Visible;
			}
			set
			{
				Legend.Visible = value;				
			}
		}


		
		[Category("Plot Appearance")]
		[Description("Get/Set whether to show the bottom right picture box.")]		
		public bool ShowRightPanel
		{
			get
			{
				return picRight.Visible;
			}
			set
			{
				pnlSE.Visible = value;					
			}
		}
		#endregion
		
		/// <summary>
		/// Handles when the menu item to show both r-squared and equation is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuShowBothText_Click(object sender, System.EventArgs e)
		{
			m_scatter.ShowRSquaredValue = true;
			m_scatter.ShowEquation		= true;
			mnu_ShowBothText.Enabled = false;
			mnu_ShowEquation.Enabled = true;
			mnu_ShowRSquared.Enabled	= true;
			// Bad name for the menu
			mnu_HideBothEqnR.Enabled = true;
			OnRefresh();
		}

		private void ctlScatterPlot_RangeHorizontal(int lowRange, int highRange)
		{			
			StartColumn = lowRange;
			EndColumn   = highRange;
		}

		private void ctlScatterPlot_RangeVertical(int lowRange, int highRange)
		{						
			StartRow = lowRange;
			EndRow   = highRange;
		}
        private delegate void DelegateCalculationComplete();

        private void OnCalculationComplete()
        {
            System.Diagnostics.Trace.WriteLine("ScatterPlot calcuations complete.");
            ProgBar.Visible = false;
            ProgBar.Refresh();
            StatBar.Visible = false;
            ProgBar.Visible = false;
            OnRefresh();
        }

		private void m_scatter_OnCalculationComplete()
		{
            if (InvokeRequired == true)
            {
                try
                {

                    Invoke(new DelegateCalculationComplete(OnCalculationComplete));
                }
                catch (System.InvalidOperationException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
            else
            {
                OnCalculationComplete();
            }
		}

		private void m_scatter_OnPercentCalc(int percent)
		{
			try
			{
				Invoke(new UpdateProgress(OnUpdateProgress), new object [] {true, percent, "Calculating Correlation Values"});				
			}
			catch(System.InvalidOperationException ex)
			{
				System.Console.WriteLine(ex.Message);
			}		
		}

		private void m_scatter_ScatterPlotMouseWheel(object sender, MouseEventArgs e)
		{
			
		}

		private void mnu_FitToScreen_Click(object sender, System.EventArgs e)
		{
			mnu_FitToScreen.Checked = mnu_FitToScreen.Checked == false;
			if (mnu_FitToScreen.Checked == true)
			{
				FitToScreen();
			}
		}
	}
}