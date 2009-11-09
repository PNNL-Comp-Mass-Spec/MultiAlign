using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;

using Derek;
using IDLTools;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlHeatMapLegend.
	/// </summary>
	public unsafe class ctlHeatMapLegend: System.Windows.Forms.UserControl
	{

		#region system
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.pnlEdit = new System.Windows.Forms.Panel();
			this.pnlProperties = new System.Windows.Forms.Panel();
			this.btnApply = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnlData = new System.Windows.Forms.Panel();
			this.chkZscore = new System.Windows.Forms.CheckBox();
			this.rdoLinear = new System.Windows.Forms.RadioButton();
			this.rdoLog = new System.Windows.Forms.RadioButton();
			this.pnlMinMax = new System.Windows.Forms.Panel();
			this.btnAutoScale = new System.Windows.Forms.Button();
			this.neMax = new PNNLControls.ctlNumEdit();
			this.lblMax = new System.Windows.Forms.Label();
			this.neMin = new PNNLControls.ctlNumEdit();
			this.lblMin = new System.Windows.Forms.Label();
			this.pnlColors = new System.Windows.Forms.Panel();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnAddIncColor = new System.Windows.Forms.Button();
			this.pnlOverUnder = new System.Windows.Forms.Panel();
			this.CPNaN = new PNNLControls.ctlLegendColor();
			this.CPOver = new PNNLControls.ctlLegendColor();
			this.CPUnder = new PNNLControls.ctlLegendColor();
			this.pnlSlider = new System.Windows.Forms.Panel();
			this.CPMin = new PNNLControls.ctlLegendColor();
			this.CPMax = new PNNLControls.ctlLegendColor();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlView = new System.Windows.Forms.Panel();
			this.axis = new PNNLControls.ctlSingleAxis();
			this.pic = new System.Windows.Forms.PictureBox();
			this.tip = new System.Windows.Forms.ToolTip(this.components);
			this.openFile = new System.Windows.Forms.OpenFileDialog();
			this.saveFile = new System.Windows.Forms.SaveFileDialog();
			this.pnlEdit.SuspendLayout();
			this.pnlProperties.SuspendLayout();
			this.pnlData.SuspendLayout();
			this.pnlMinMax.SuspendLayout();
			this.pnlColors.SuspendLayout();
			this.pnlOverUnder.SuspendLayout();
			this.pnlSlider.SuspendLayout();
			this.pnlView.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlEdit
			// 
			this.pnlEdit.BackColor = System.Drawing.SystemColors.ControlDark;
			this.pnlEdit.Controls.Add(this.pnlProperties);
			this.pnlEdit.Controls.Add(this.pnlColors);
			this.pnlEdit.Controls.Add(this.pnlSlider);
			this.pnlEdit.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlEdit.Location = new System.Drawing.Point(0, 0);
			this.pnlEdit.Name = "pnlEdit";
			this.pnlEdit.Size = new System.Drawing.Size(176, 328);
			this.pnlEdit.TabIndex = 2;
			this.pnlEdit.Visible = false;
			// 
			// pnlProperties
			// 
			this.pnlProperties.Controls.Add(this.btnApply);
			this.pnlProperties.Controls.Add(this.panel1);
			this.pnlProperties.Controls.Add(this.pnlData);
			this.pnlProperties.Controls.Add(this.pnlMinMax);
			this.pnlProperties.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlProperties.DockPadding.All = 4;
			this.pnlProperties.Location = new System.Drawing.Point(0, 0);
			this.pnlProperties.Name = "pnlProperties";
			this.pnlProperties.Size = new System.Drawing.Size(96, 328);
			this.pnlProperties.TabIndex = 49;
			// 
			// btnApply
			// 
			this.btnApply.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnApply.Location = new System.Drawing.Point(4, 192);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(88, 20);
			this.btnApply.TabIndex = 50;
			this.btnApply.Text = "Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(4, 168);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(88, 24);
			this.panel1.TabIndex = 51;
			// 
			// pnlData
			// 
			this.pnlData.Controls.Add(this.chkZscore);
			this.pnlData.Controls.Add(this.rdoLinear);
			this.pnlData.Controls.Add(this.rdoLog);
			this.pnlData.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlData.DockPadding.All = 4;
			this.pnlData.Location = new System.Drawing.Point(4, 104);
			this.pnlData.Name = "pnlData";
			this.pnlData.Size = new System.Drawing.Size(88, 64);
			this.pnlData.TabIndex = 49;
			// 
			// chkZscore
			// 
			this.chkZscore.Checked = true;
			this.chkZscore.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkZscore.Dock = System.Windows.Forms.DockStyle.Top;
			this.chkZscore.Location = new System.Drawing.Point(4, 44);
			this.chkZscore.Name = "chkZscore";
			this.chkZscore.Size = new System.Drawing.Size(80, 16);
			this.chkZscore.TabIndex = 36;
			this.chkZscore.Text = "Z Score";
			// 
			// rdoLinear
			// 
			this.rdoLinear.Dock = System.Windows.Forms.DockStyle.Top;
			this.rdoLinear.Location = new System.Drawing.Point(4, 24);
			this.rdoLinear.Name = "rdoLinear";
			this.rdoLinear.Size = new System.Drawing.Size(80, 20);
			this.rdoLinear.TabIndex = 34;
			this.rdoLinear.Text = "Linear";
			// 
			// rdoLog
			// 
			this.rdoLog.Checked = true;
			this.rdoLog.Dock = System.Windows.Forms.DockStyle.Top;
			this.rdoLog.Location = new System.Drawing.Point(4, 4);
			this.rdoLog.Name = "rdoLog";
			this.rdoLog.Size = new System.Drawing.Size(80, 20);
			this.rdoLog.TabIndex = 35;
			this.rdoLog.TabStop = true;
			this.rdoLog.Text = "Log";
			// 
			// pnlMinMax
			// 
			this.pnlMinMax.Controls.Add(this.btnAutoScale);
			this.pnlMinMax.Controls.Add(this.neMax);
			this.pnlMinMax.Controls.Add(this.lblMax);
			this.pnlMinMax.Controls.Add(this.neMin);
			this.pnlMinMax.Controls.Add(this.lblMin);
			this.pnlMinMax.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlMinMax.DockPadding.All = 4;
			this.pnlMinMax.Location = new System.Drawing.Point(4, 4);
			this.pnlMinMax.Name = "pnlMinMax";
			this.pnlMinMax.Size = new System.Drawing.Size(88, 100);
			this.pnlMinMax.TabIndex = 46;
			// 
			// btnAutoScale
			// 
			this.btnAutoScale.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnAutoScale.Location = new System.Drawing.Point(4, 78);
			this.btnAutoScale.Name = "btnAutoScale";
			this.btnAutoScale.Size = new System.Drawing.Size(80, 20);
			this.btnAutoScale.TabIndex = 48;
			this.btnAutoScale.Text = "Scale to Data";
			this.btnAutoScale.Click += new System.EventHandler(this.btnAutoScale_Click);
			// 
			// neMax
			// 
			this.neMax.Dock = System.Windows.Forms.DockStyle.Top;
			this.neMax.Location = new System.Drawing.Point(4, 58);
			this.neMax.MaxValue = 1.7976931348623157E+308;
			this.neMax.MinValue = -1.7976931348623157E+308;
			this.neMax.Name = "neMax";
			this.neMax.Size = new System.Drawing.Size(80, 20);
			this.neMax.TabIndex = 33;
			this.neMax.Text = "0";
			this.neMax.Value = 0;
			this.neMax.ValueChanged += new PNNLControls.ctlNumEdit.ValueChangedDelegate(this.neMax_ValueChanged);
			// 
			// lblMax
			// 
			this.lblMax.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblMax.Location = new System.Drawing.Point(4, 40);
			this.lblMax.Name = "lblMax";
			this.lblMax.Size = new System.Drawing.Size(80, 18);
			this.lblMax.TabIndex = 35;
			this.lblMax.Text = "Max";
			// 
			// neMin
			// 
			this.neMin.Dock = System.Windows.Forms.DockStyle.Top;
			this.neMin.Location = new System.Drawing.Point(4, 20);
			this.neMin.MaxValue = 1.7976931348623157E+308;
			this.neMin.MinValue = -1.7976931348623157E+308;
			this.neMin.Name = "neMin";
			this.neMin.Size = new System.Drawing.Size(80, 20);
			this.neMin.TabIndex = 36;
			this.neMin.Text = "0";
			this.neMin.Value = 0;
			this.neMin.ValueChanged += new PNNLControls.ctlNumEdit.ValueChangedDelegate(this.neMin_ValueChanged);
			// 
			// lblMin
			// 
			this.lblMin.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblMin.Location = new System.Drawing.Point(4, 4);
			this.lblMin.Name = "lblMin";
			this.lblMin.Size = new System.Drawing.Size(80, 16);
			this.lblMin.TabIndex = 34;
			this.lblMin.Text = "Min";
			// 
			// pnlColors
			// 
			this.pnlColors.Controls.Add(this.btnClear);
			this.pnlColors.Controls.Add(this.btnDelete);
			this.pnlColors.Controls.Add(this.btnAddIncColor);
			this.pnlColors.Controls.Add(this.pnlOverUnder);
			this.pnlColors.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlColors.DockPadding.All = 4;
			this.pnlColors.Location = new System.Drawing.Point(96, 0);
			this.pnlColors.Name = "pnlColors";
			this.pnlColors.Size = new System.Drawing.Size(56, 328);
			this.pnlColors.TabIndex = 43;
			// 
			// btnClear
			// 
			this.btnClear.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnClear.Location = new System.Drawing.Point(4, 192);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(48, 20);
			this.btnClear.TabIndex = 46;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnDelete.Location = new System.Drawing.Point(4, 172);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(48, 20);
			this.btnDelete.TabIndex = 48;
			this.btnDelete.Text = "Delete";
			// 
			// btnAddIncColor
			// 
			this.btnAddIncColor.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnAddIncColor.Location = new System.Drawing.Point(4, 152);
			this.btnAddIncColor.Name = "btnAddIncColor";
			this.btnAddIncColor.Size = new System.Drawing.Size(48, 20);
			this.btnAddIncColor.TabIndex = 39;
			this.btnAddIncColor.Text = "Add";
			this.btnAddIncColor.Click += new System.EventHandler(this.btnAddIncColor_Click);
			// 
			// pnlOverUnder
			// 
			this.pnlOverUnder.Controls.Add(this.CPNaN);
			this.pnlOverUnder.Controls.Add(this.CPOver);
			this.pnlOverUnder.Controls.Add(this.CPUnder);
			this.pnlOverUnder.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlOverUnder.Location = new System.Drawing.Point(4, 4);
			this.pnlOverUnder.Name = "pnlOverUnder";
			this.pnlOverUnder.Size = new System.Drawing.Size(48, 148);
			this.pnlOverUnder.TabIndex = 38;
			// 
			// CPNaN
			// 
			this.CPNaN.BackColor = System.Drawing.SystemColors.Control;
			this.CPNaN.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CPNaN.LegendColor = System.Drawing.Color.Gray;
			this.CPNaN.Location = new System.Drawing.Point(0, 32);
			this.CPNaN.Mode = PNNLControls.ctlLegendColor.DisplayMode.Block;
			this.CPNaN.Name = "CPNaN";
			this.CPNaN.Percent = 0;
			this.CPNaN.Size = new System.Drawing.Size(48, 84);
			this.CPNaN.TabIndex = 2;
			// 
			// CPOver
			// 
			this.CPOver.BackColor = System.Drawing.SystemColors.Control;
			this.CPOver.Dock = System.Windows.Forms.DockStyle.Top;
			this.CPOver.LegendColor = System.Drawing.SystemColors.Control;
			this.CPOver.Location = new System.Drawing.Point(0, 0);
			this.CPOver.Mode = PNNLControls.ctlLegendColor.DisplayMode.NorthPointer;
			this.CPOver.Name = "CPOver";
			this.CPOver.Percent = 0;
			this.CPOver.Size = new System.Drawing.Size(48, 32);
			this.CPOver.TabIndex = 1;
			// 
			// CPUnder
			// 
			this.CPUnder.BackColor = System.Drawing.SystemColors.Control;
			this.CPUnder.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.CPUnder.LegendColor = System.Drawing.SystemColors.Control;
			this.CPUnder.Location = new System.Drawing.Point(0, 116);
			this.CPUnder.Mode = PNNLControls.ctlLegendColor.DisplayMode.SouthPointer;
			this.CPUnder.Name = "CPUnder";
			this.CPUnder.Percent = 0;
			this.CPUnder.Size = new System.Drawing.Size(48, 32);
			this.CPUnder.TabIndex = 0;
			// 
			// pnlSlider
			// 
			this.pnlSlider.BackColor = System.Drawing.SystemColors.Control;
			this.pnlSlider.Controls.Add(this.CPMin);
			this.pnlSlider.Controls.Add(this.CPMax);
			this.pnlSlider.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlSlider.Location = new System.Drawing.Point(152, 0);
			this.pnlSlider.Name = "pnlSlider";
			this.pnlSlider.Size = new System.Drawing.Size(24, 328);
			this.pnlSlider.TabIndex = 44;
			// 
			// CPMin
			// 
			this.CPMin.BackColor = System.Drawing.Color.Transparent;
			this.CPMin.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.CPMin.LegendColor = System.Drawing.Color.Black;
			this.CPMin.Location = new System.Drawing.Point(0, 304);
			this.CPMin.Mode = PNNLControls.ctlLegendColor.DisplayMode.SouthEastPointer;
			this.CPMin.Name = "CPMin";
			this.CPMin.Percent = 0;
			this.CPMin.Size = new System.Drawing.Size(24, 24);
			this.CPMin.TabIndex = 0;
			// 
			// CPMax
			// 
			this.CPMax.BackColor = System.Drawing.Color.Transparent;
			this.CPMax.Dock = System.Windows.Forms.DockStyle.Top;
			this.CPMax.LegendColor = System.Drawing.Color.White;
			this.CPMax.Location = new System.Drawing.Point(0, 0);
			this.CPMax.Mode = PNNLControls.ctlLegendColor.DisplayMode.NorthEastPointer;
			this.CPMax.Name = "CPMax";
			this.CPMax.Percent = 1;
			this.CPMax.Size = new System.Drawing.Size(24, 22);
			this.CPMax.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Left;
			this.label2.Location = new System.Drawing.Point(0, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(12, 168);
			this.label2.TabIndex = 4;
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label3.Location = new System.Drawing.Point(0, 232);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 32);
			this.label3.TabIndex = 5;
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			// 
			// pnlView
			// 
			this.pnlView.Controls.Add(this.axis);
			this.pnlView.Controls.Add(this.pic);
			this.pnlView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlView.Location = new System.Drawing.Point(176, 0);
			this.pnlView.Name = "pnlView";
			this.pnlView.Size = new System.Drawing.Size(56, 328);
			this.pnlView.TabIndex = 4;
			// 
			// axis
			// 
			this.axis.Alignment = null;
			this.axis.BackColor = System.Drawing.SystemColors.Control;
			this.axis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.axis.Dock = System.Windows.Forms.DockStyle.Fill;
			this.axis.Location = new System.Drawing.Point(0, 0);
			this.axis.Name = "axis";
			this.axis.SelectedColor = System.Drawing.Color.Pink;
			this.axis.Size = new System.Drawing.Size(40, 328);
			this.axis.TabIndex = 4;
			// 
			// pic
			// 
			this.pic.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pic.Dock = System.Windows.Forms.DockStyle.Right;
			this.pic.Location = new System.Drawing.Point(40, 0);
			this.pic.Name = "pic";
			this.pic.Size = new System.Drawing.Size(16, 328);
			this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pic.TabIndex = 1;
			this.pic.TabStop = false;
			this.pic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Color_MouseMove);
			this.pic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic_MouseDown);
			// 
			// tip
			// 
			this.tip.AutomaticDelay = 100;
			this.tip.AutoPopDelay = 100;
			this.tip.InitialDelay = 100;
			this.tip.ReshowDelay = 20;
			// 
			// openFile
			// 
			this.openFile.AddExtension = false;
			this.openFile.DefaultExt = "xml";
			this.openFile.Filter = "\"XML files (*xml)|*.xml|All files (*.*)|*.*\"";
			this.openFile.RestoreDirectory = true;
			this.openFile.Title = "Load Legend";
			// 
			// ctlHeatMapLegend
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.pnlView);
			this.Controls.Add(this.pnlEdit);
			this.Name = "ctlHeatMapLegend";
			this.Size = new System.Drawing.Size(232, 328);
			this.pnlEdit.ResumeLayout(false);
			this.pnlProperties.ResumeLayout(false);
			this.pnlData.ResumeLayout(false);
			this.pnlMinMax.ResumeLayout(false);
			this.pnlColors.ResumeLayout(false);
			this.pnlOverUnder.ResumeLayout(false);
			this.pnlSlider.ResumeLayout(false);
			this.pnlView.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		//keep the extra colors at the end of the color map
		private System.Windows.Forms.Panel pnlEdit;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlColors;
		private System.Windows.Forms.Panel pnlSlider;
		private System.Windows.Forms.Panel pnlOverUnder;
		private PNNLControls.ctlLegendColor CPOver;
		private PNNLControls.ctlLegendColor CPUnder;
		private PNNLControls.ctlLegendColor CPNaN;
		private PNNLControls.ctlLegendColor CPMin;
		private PNNLControls.ctlLegendColor CPMax;
		private System.Windows.Forms.Button btnAddIncColor;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Panel pnlProperties;
		private System.Windows.Forms.Panel pnlMinMax;
		private PNNLControls.ctlNumEdit neMax;
		private System.Windows.Forms.Label lblMax;
		private PNNLControls.ctlNumEdit neMin;
		private System.Windows.Forms.Label lblMin;
		private System.Windows.Forms.Panel pnlData;
		private System.Windows.Forms.CheckBox chkZscore;
		private System.Windows.Forms.RadioButton rdoLinear;
		private System.Windows.Forms.RadioButton rdoLog;
		private System.Windows.Forms.Panel pnlView;
		private System.Windows.Forms.PictureBox pic;
		private PNNLControls.ctlSingleAxis axis;

		#endregion

		public enum ApplyLegendMode
		{
			linear,
			log, 
            wingdings
		}

		private int mNExtraColors = 3;
		private int mNaNIndex;
		private int mOverRangeIndex;
		private int mUnderRangeIndex;

		//raised to signal a change in the legend that could indicate that the legend be 
		//reapplied to the data
		public delegate void LegendChangedDelegate ();
		public event LegendChangedDelegate LegendChanged = null;

		public delegate void AutoScaleRequestDelegate ();
		public event AutoScaleRequestDelegate AutoScaleRequest = null;

		//used for dragging colorPoints on the slider bar
		private Rectangle dragBoxFromMouseDown;
		private bool mDragEnabled = true;

		//current list of points
		private ArrayList ColorPoints = new ArrayList();

		//bitmap holding the legend colormap
		private Bitmap mColorMapBMP = new Bitmap(1,1);

		//bitmap holding viewable representation of the legend
		private Bitmap mViewBMP = new Bitmap(1,1);

		private int mPaletteSize = 256;  //8bit colormap
		private int mNLegendPoints = 256; //nice round number

		//legend can be applied to data linearly or as a function of the log of the data
		private ApplyLegendMode mApplyMode = ApplyLegendMode.linear;
		private System.Windows.Forms.ToolTip tip;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button btnAutoScale;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.OpenFileDialog openFile;
		private System.Windows.Forms.SaveFileDialog saveFile;

		//tracks whether changes have been applied to the legend
		private bool mEditApplied = false;

		/// <summary>
		/// constructor
		/// </summary>
		public ctlHeatMapLegend()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			//
			// TODO: Add constructor logic here
			//

			//initialize min and max colors
			CPAssignDelagates(CPMin);
			CPAssignDelagates(CPMax);

			this.Clear();
			InterpolateLegend();			
			axis.Zoom += new ctlSingleAxis.ZoomDelegate(this.AxisZoomIn);
			axis.UnZoom += new ctlSingleAxis.UnZoomDelegate(this.AxisZoomOut);

			UpdateAxis();
		}

		private void AxisZoomOut()
		{
			if (AutoScaleRequest!=null)
			{
				AutoScaleRequest();
				CreateLegend(mViewBMP);
			}
		}

		/// <summary>
		/// applies the zoom rectangle to zoom in on a range of labels
		/// </summary>
		private void AxisZoomIn(Point start, Point stop)
		{
			try
			{
				//calc then apply as the neMin and neMax vals are used for the calc
				double high = ValueAtPosition(Math.Min(start.Y, stop.Y));
				double low = ValueAtPosition(Math.Max(start.Y, stop.Y));
				this.neMin.Value = low;
				this.neMax.Value = high;

				CreateLegend(mViewBMP);
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// color representing values that fall under the minimum range
		/// </summary>
		public Color UnderColor
		{
			get {return CPUnder.LegendColor;}
			set	
			{
				CPUnder.LegendColor = value;
				mColorMapBMP.SetPixel(0, mUnderRangeIndex, value);
			}
		}

		/// <summary>
		/// color representing values falling over the maximum range
		/// </summary>
		public Color OverColor
		{
			get {return CPOver.LegendColor;}
			set	
			{
				CPOver.LegendColor = value;
				mColorMapBMP.SetPixel(0, mOverRangeIndex, value);
			}
		}

		/// <summary>
		/// color representing NAN
		/// </summary>
		public Color NaNColor
		{
			get{return CPNaN.LegendColor;}
			set{CPNaN.LegendColor=value;
				mColorMapBMP.SetPixel(0, mNaNIndex, value);
			}
		}

		public Bitmap BMap 
		{
			get {return mViewBMP;}
		} 

		public Color GetColor(int index)
		{
			try{return (mColorMapBMP.GetPixel(0, index));}
			catch{return (System.Drawing.Color.Black);}
		}

		public Color GetColor(float val)
		{
			try{return (mColorMapBMP.GetPixel(0, PixelIndex(val, LowerRange, UpperRange)));}
			catch{return (System.Drawing.Color.Black);}
		}

		public int PixelIndex(float val, float minRange, float maxRange)
		{
			try
			{
				if (float.IsNaN(val)) 
					return(mNaNIndex);

				float factor = 1.0f;
				float usedRange = maxRange - minRange;

				factor = (val-minRange) / (usedRange); 

				if (mApplyMode == ApplyLegendMode.log)
				{
					if (val-minRange<=0)
						factor = 0.0f;
					else
						factor = (float) (Math.Log(1.0 + (double) factor) / Math.Log(2.0));
						//factor = (float) (Math.Log((double)(val-minRange)) / Math.Log((double)(usedRange)));
				}

				if (factor > 1.0f)
					return(mOverRangeIndex);
				if (factor < 0.0f)
					return(mUnderRangeIndex);

				int i = (int) ((float)(mPaletteSize-4) - (float) (mPaletteSize-4) * factor);

				return(i);
			}
			catch
			{
				return (0);
			}
		}

		public float[,] ZScore(float[,] data, out float min, out float max) 
		{ 
			int rows = data.GetLength(0);
			int cols = data.GetLength(1);

			min = float.MaxValue;
			max = float.MinValue;

			float[,] zscore = new float[rows, cols];

			fixed (float* d = data)
			{
				float*[] srcYOffsets = new float*[rows];
				for(int i=0; i<rows; i++)
				{
					srcYOffsets[i] = d + i*cols;
				}
				fixed (float* z = zscore)
				{
					float*[] destYOffsets = new float*[rows];
					for(int i=0; i<rows; i++)
					{
						destYOffsets[i] = z + i*cols;
					}
					for (int i=0; i<rows; i++)
					{
						//get mean and standard deviation for the row
						float Sum = 0.0F, SumOfSqrs = 0.0F, count = 0.0f; 
						for (int j=0; j<cols; j++) 
						{ 
							float val = *(srcYOffsets[i]+j);
							if (!float.IsNaN(val))
							{
								Sum += val; 
								SumOfSqrs += val * val; 
								count++;
							}
						} 
						float stdDev = 1.0f;
						if (count>1.0f)
						{
							float topSum = count * SumOfSqrs - Sum*Sum;
							stdDev = (float)Math.Sqrt((topSum / (count * (count-1)))); 
						}
						//deep, added this to clear errors but don't know if it is appropriate
						if (float.IsNaN(stdDev)) stdDev = 1.0f;
						float mean = 0.0f;
						if (count>0.0f)
						{
							mean = Sum / count;
						}
						for (int j=0; j<cols; j++) 
						{ 
							float val = *(srcYOffsets[i]+j);
							if (!float.IsNaN(val))
							{
								//deep, added this to clear errors but don't know if it is appropriate
								if (stdDev==0.0f) stdDev=1.0f;

								float zVal = (val-mean) / stdDev;
								if (zVal < min) min = zVal;
								if (zVal > max) max = zVal;
								*(destYOffsets[i]+j) = zVal;
							}
							else
								*(destYOffsets[i]+j) = float.NaN;
						} 
					}
				}
			}
			
			return zscore;
		} 

//		public float[,] ZScore(float[,] data) 
//		{ 
//			int rows = data.GetLength(0);
//			int cols = data.GetLength(1);
//
//			min = float.MaxValue;
//			max = float.MinValue;
//
//			float[,] zscore = new float[rows, cols];
//
//			fixed (float* d = data)
//			{
//				float*[] srcYOffsets = new float*[rows];
//				for(int i=0; i<rows; i++)
//				{
//					srcYOffsets[i] = d + i*cols;
//				}
//				fixed (float* z = zscore)
//				{
//					float*[] destYOffsets = new float*[rows];
//					for(int i=0; i<rows; i++)
//					{
//						destYOffsets[i] = z + i*cols;
//					}
//					for (int i=0; i<rows; i++)
//					{
//						//get mean and standard deviation for the row
//						float Sum = 0.0F, SumOfSqrs = 0.0F, count = 0.0f; 
//						for (int j=0; j<cols; j++) 
//						{ 
//							float val = *(srcYOffsets[i]+j);
//							if (!float.IsNaN(val))
//							{
//								Sum += val; 
//								SumOfSqrs += val * val; 
//								count++;
//							}
//						} 
//						float stdDev = 1.0f;
//						if (count>1.0f)
//						{
//							float topSum = count * SumOfSqrs - Sum*Sum;
//							stdDev = (float)Math.Sqrt((topSum / (count * (count-1)))); 
//						}
//						//deep, added this to clear errors but don't know if it is appropriate
//						if (float.IsNaN(stdDev)) stdDev = 1.0f;
//						float mean = 0.0f;
//						if (count>0.0f)
//						{
//							mean = Sum / count;
//						}
//						for (int j=0; j<cols; j++) 
//						{ 
//							float val = *(srcYOffsets[i]+j);
//							if (!float.IsNaN(val))
//							{
//								//deep, added this to clear errors but don't know if it is appropriate
//								if (stdDev==0.0f) stdDev=1.0f;
//
//								float zVal = (val-mean) / stdDev;
//								if (zVal < min) min = zVal;
//								if (zVal > max) max = zVal;
//								*(destYOffsets[i]+j) = zVal;
//							}
//							else
//								*(destYOffsets[i]+j) = float.NaN;
//						} 
//					}
//				}
//			}
//			
//			return zscore;
//		} 

//		private float[] mZMeans = null;
//		private float[] mZStdDevs = null;
//		private float mZMin = float.MaxValue;
//		private float mZMax = float.MinValue;
//
//		public void InitZScore(float[,] totalData) 
//		{ 
//			int rows = totalData.GetLength(0);
//			int cols = totalData.GetLength(1);
//
//			mZMeans = new float[rows];
//			mZStdDevs = new float[rows];
//
//			mZMin = float.MaxValue;
//			mZMax = float.MinValue;
//
//			fixed (float* d = totalData)
//			{
//				float*[] srcYOffsets = new float*[rows];
//				for(int i=0; i<rows; i++)
//				{
//					srcYOffsets[i] = d + i*cols;
//				}
//
//				for (int i=0; i<rows; i++)
//				{
//					//get mean and standard deviation for the row
//					float Sum = 0.0F, SumOfSqrs = 0.0F, count = 0.0f; 
//					for (int j=0; j<cols; j++) 
//					{ 
//						float val = *(srcYOffsets[i]+j);
//						if (!float.IsNaN(val))
//						{
//							Sum += val; 
//							SumOfSqrs += val * val; 
//							count++;
//						}
//					} 
//					
//					if (count>1.0f)
//					{
//						float topSum = count * SumOfSqrs - Sum*Sum;
//						mZStdDevs[i] = (float)Math.Sqrt((topSum / (count * (count-1)))); 
//					}
//					else
//					{
//						mZStdDevs[i]  = 1.0f;
//					}
//
//					//deep, added this to clear errors but don't know if it is appropriate
//					if (float.IsNaN(mZStdDevs[i])) 
//					{
//						mZStdDevs[i] = 1.0f;
//					}
//
//					//deep, added this to clear errors but don't know if it is appropriate
//					if (mZStdDevs[i]==0.0f) 
//					{
//						mZStdDevs[i]=1.0f ;
//					}
//
//					if (count>0.0f)
//					{
//						mZMeans[i] = Sum / count;
//					}
//					else
//					{
//						mZMeans[i] = 0.0f;
//					}
//
//					for (int j=0; j<cols; j++) 
//					{ 
//						float val = *(srcYOffsets[i]+j);
//						if (!float.IsNaN(val))
//						{
//							float zVal = (val-mZMeans[i]) / mZStdDevs[i];
//							if (zVal < mZMin) mZMin = zVal;
//							if (zVal > mZMax) mZMax = zVal;
//						}
//					} 
//				}
//			}
//		} 

//		public float[,] oldZScore(float[,] data, out float min, out float max) 
//		{ 
//			int rows = data.GetLength(0);
//			int cols = data.GetLength(1);
//
//			min = float.MaxValue;
//			max = float.MinValue;
//
//			float[,] zscore = new float[rows, cols];
//
//			for (int i=0; i<rows; i++)
//			{
//				//get mean and standard deviation for the row
//				float Sum = 0.0F, SumOfSqrs = 0.0F, count = 0.0f; 
//				for (int j=0; j<cols; j++) 
//				{ 
//					if (!float.IsNaN(data[i,j]))
//					{
//						Sum += data[i,j]; 
//						SumOfSqrs += data[i,j] * data[i,j]; 
//						count++;
//					}
//				} 
//				float stdDev = 1.0f;
//				if (count>1.0f)
//				{
//					float topSum = count * SumOfSqrs - Sum*Sum;
//					stdDev = (float)Math.Sqrt((topSum / (count * (count-1)))); 
//				}
//				if (float.IsNaN(stdDev)) stdDev = 1.0f;
//				float mean = 0.0f;
//				if (count>0.0f)
//				{
//					mean = Sum / count;
//				}
//				for (int j=0; j<cols; j++) 
//				{ 
//					if (!float.IsNaN(data[i,j]))
//					{
//						if (stdDev==0.0f) stdDev=1.0f;
//						zscore[i,j]=(data[i,j]-mean) / stdDev;
//						if (!float.IsNaN(zscore[i,j]))
//						{
//							if (zscore[i,j] < min) min = zscore[i,j];
//							if (zscore[i,j] > max) max = zscore[i,j];
//						}
//					}
//					else
//						zscore[i,j]=float.NaN;
//				} 
//			}
//			
//			return zscore;
//		} 

		public bool UseZScore
		{
			get{return chkZscore.Checked;}
			set{chkZscore.Checked=value;}
		}

		public float UpperRange
		{
			get{return (float) this.neMax.Value;}
			set{this.neMax.Value=(double)value;}
		}

		public float LowerRange
		{
			get{return (float) this.neMin.Value;}
			set{this.neMin.Value=(double)value;}
		}

		public Bitmap ApplyLegend(float [,] data, Range displayRange)
		{
			int width = displayRange.EndColumn - displayRange.StartColumn + 1;
			int height = displayRange.EndRow - displayRange.StartRow + 1;

			Console.WriteLine("ctlHeatMapLegend: ApplyLegend. Size = " + this.Size + "Width = " + width.ToString() 
				+ " Height = " + height.ToString()) ; 

			if (width*height<=0) return null;

			Color c = mColorMapBMP.GetPixel(0, 256);

			Bitmap b = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			
			BitmapTools bmt = new BitmapTools();
			BitmapTools.BitmapInfo fromInfo = bmt.LockBitmap(mColorMapBMP);
			BitmapTools.BitmapInfo toInfo = bmt.LockBitmap(b);


			try
			{
				int mapOffset = (int)fromInfo.pBase;
				int bmpOffset = (int)toInfo.pBase;
				int bmpWidth = (int)toInfo.width;

				float [,] target = data;

				float min = LowerRange;
				float max = UpperRange;

				//zscore undefined for < 2 columns
				if (UseZScore && displayRange.NumColumns>1) 
				{
					//use the min and max values established on AutoScale so that the range is consistent when zooming
					float dummyMin, dummyMax;
					target = ZScore(data, out dummyMin, out dummyMax);
					min = mMinZScore;
					max = mMaxZScore;
				}

				int pixIndex = mNaNIndex;
				bool log = (mApplyMode == ApplyLegendMode.log);
				float pRange = (float)(mPaletteSize-4);
				float valRange = max-min;
				int pixSize = sizeof(BitmapTools.PixelData);
				int bHeight = b.Height;
				int bWidth = b.Width;
				int[] xOffsets = new int[bWidth];
				for(int i=0; i<bWidth; i++)
				{
					xOffsets[i] = i*pixSize;
				}
				int mapHeight = mColorMapBMP.Height;
				int[] yOffsets = new int[mapHeight];
				for(int i=0; i<mapHeight; i++)
				{
					yOffsets[i] = i*pixSize;
				}

				fixed (float* d = target)
				{
					for (int j=0; j<bHeight; j++)
					{
						int yOffset = bmpOffset + j*bmpWidth;
						for(int i=0; i<bWidth; i++)
						{
							float val = *(d+width*(displayRange.StartRow+j) + i);

							if (float.IsNaN(val)) 
							{
								pixIndex = mNaNIndex;
							}
							else if (val>max)
							{
								pixIndex = mOverRangeIndex;
							}
							else if (val<min)
							{
								pixIndex = mUnderRangeIndex;
							}
							else 
							{
								if (log)
								{
									if (val-min<=0)
									{
										pixIndex = 0 ; 
									}
									else
									{
										// DJ May 06 2007.
										//										factor = (float) (Math.Log(1.0 + (double) factor) / Math.Log(2.0));
										pixIndex = (int)(pRange * Math.Log(val-min)/Math.Log(max-min)) ; 
										if (val == max)
											pixIndex = mapHeight-1 ; 
									}
								}
								else
								{
									//case where there is only one value, so position that one value in the center of the range
									float factor = .5f; 
								//	if(valRange>0)
										factor =  (val-min) / valRange; 

									pixIndex = (int) (pRange - pRange*factor) ;
									if (factor == 0)
										pixIndex = (int)(pRange-1) ;
								}
								if (pixIndex<0 || pixIndex>=mapHeight) pixIndex = mNaNIndex;
							}

							try
							{
						
								BitmapTools.PixelData* srcPixel = (BitmapTools.PixelData*)(mapOffset + yOffsets[pixIndex]);
								BitmapTools.PixelData* destPixel = (BitmapTools.PixelData*)(yOffset + + xOffsets[i]);
								*(destPixel) = *(srcPixel);
							}catch(Exception e) 
							{System.Windows.Forms.MessageBox.Show(e.Message);}
						}
					}
				}
			}
			catch (Exception e) {System.Windows.Forms.MessageBox.Show(e.Message);}
			finally 
			{
				bmt.UnlockBitmap(mColorMapBMP, fromInfo);
				bmt.UnlockBitmap(b, toInfo);
			}
			return(b);
		}

		private float mMinZScore = float.MaxValue;
		private float mMaxZScore = float.MinValue;

		public void AutoScaleZScore(float[,]srcData)
		{
			try
			{
				float [,] dummy = ZScore(srcData, out mMinZScore, out mMaxZScore);
			}
			catch{}
		}

		public void AutoScale(float[,]srcData, Range range)
		{
			float max = float.MinValue;
			float min = float.MaxValue;
			int srcWidth = srcData.GetUpperBound(1)+1;
			int srcHeight = srcData.GetUpperBound(0)+1;

			AutoScaleZScore(srcData);

			int minRangeRow = -1 ; 
			int minRangeCol = -1 ; 
			fixed (float* s = srcData)
			{
				for (int j=range.StartRow; j<=range.EndRow; j++)
				{
					int offset = j*srcWidth;
					for (int i = range.StartColumn; i <= range.EndColumn; i++)
					{
						float val = *(s+offset+i);
						if (val > max) 
						{
							max = val;
							minRangeRow = j ; 
						}
						if (val < min) 
						{
							min = val;
							minRangeCol = i ; 
						}
					}
				}
			}
			
			this.UpperRange = max;
			this.LowerRange = min;

			UpdateAxis();
		}


		private void getRGB(float wl, BitmapTools.PixelData *retVal)
		{
			//			'  this code translated from fortran code by:
			//			'      RGB VALUES FOR VISIBLE WAVELENGTHS   by Dan Bruton (astro@tamu.edu)
			//			'      This program can be found at
			//			'      http://www.isc.tamu.edu/~astro/color.html
			//			'      and was last updated on February 20, 1996.
			//
			//			'      This program will create a ppm (portable pixmap) image of a spectrum.
			//			'      The spectrum is generated using approximate RGB values for visible
			//			'      wavelengths between 380 nm and 780 nm.
			//			'      NetPBM's ppmtogif can be used to convert the ppm image
			//																	'      to a gif.  The red, green and blue values (RGB) are
			//			'      assumed to vary linearly with wavelength (for GAMMA=1).
			//			'      NetPBM Software: ftp://ftp.cs.ubc.ca/ftp/archive/netpbm/
			//			'
			//			'      IMAGE INFO - WIDTH, HEIGHT, DEPTH, GAMMA
			//			'
			int Max = 255;
			float GAMMA = 0.8f;
			float R=0.0f, G=0.0f, B=0.0f, SSS=0.0f;

			if ((wl > 380.0f) && (wl <= 440.0f)) 
			{
				R = -1.0f * (wl - 440.0f) / (440.0f - 380.0f);
				G = 0.0f;
				B = 1.0f;
			}
			else if ((wl > 440.0f) && (wl <= 490.0f)) 
			{
				R = 0.0f;
				G = (wl - 440.0f) / (490.0f - 440.0f);
				B = 1.0f;
			}
			else if ((wl > 490.0f) && (wl <= 510.0f)) 
			{
				R = 0.0f;
				G = 1.0f;
				B = -1.0f * (wl - 510.0f) / (510.0f - 490.0f);
			}
			else if ((wl > 510.0f) && (wl <= 580.0f)) 
			{
				R = (wl - 510.0f) / (580.0f - 510.0f);
				G = 1.0f;
				B = 0.0f;
			}
			else if ((wl > 580.0f) && (wl <= 645.0f)) 
			{
				R = 1.0f;
				G = -1.0f * (wl - 645.0f) / (645.0f - 580.0f);
				B = 0.0f;
			}
			else if ((wl > 645.0f) && (wl <= 780.0f)) 
			{
				R = 1.0f;
				G = 0.0f;
				B = 0.0f;
			}
			else 
			{
				R = 0.0f;
				G = 0.0f;
				B = 0.0f;
			}

			//      LET THE INTENSITY SSS FALL OFF NEAR THE VISION LIMITS
 
			if (wl > 700.0f) 
				SSS = 0.3f + 0.7f * (780.0f - wl) / (780.0f - 700.0f);
			else if (wl < 420.0f) 
				SSS = 0.3f + 0.7f * (wl - 380.0f) / (420.0f - 380.0f);
			else
				SSS = 1.0f;
				
 
			//      GAMMA ADJUST
			R = (float)  Math.Pow((SSS * R),  GAMMA);
			G = (float)  Math.Pow((SSS * G),  GAMMA);
			B = (float) Math.Pow((SSS * B),  GAMMA);
        
			retVal->red = (byte)(Max * R);
			retVal->green = (byte)(Max * G);
			retVal->blue = (byte)(Max * B);

			return;
		}

		public void CreateLegend(Bitmap b)
		{
			mOverRangeIndex = mPaletteSize;
			mUnderRangeIndex = mPaletteSize + 1;
			mNaNIndex = mPaletteSize + 2;

			mColorMapBMP = new Bitmap(1, mPaletteSize+mNExtraColors);

			//create color map by interpolating colors from source bitmap
			double srcLen = (double) b.Height;
			for(int i=0; i<mPaletteSize; i++)
			{	
				double frac = (double) i / (double)mPaletteSize;
				int indx = (int) (frac*srcLen);
				mColorMapBMP.SetPixel(0, i, b.GetPixel(0, indx));
			}

			mColorMapBMP.SetPixel(0, mUnderRangeIndex, UnderColor);
			mColorMapBMP.SetPixel(0, mOverRangeIndex, OverColor);
			mColorMapBMP.SetPixel(0, mNaNIndex, NaNColor);

			if (LegendChanged!=null)
				LegendChanged();

			mEditApplied=true;
		}

//		public Bitmap ReverseBitmap(Bitmap b)
//		{
//			Bitmap dest = new Bitmap(b.Height, b.Width);
//
//			for (int i=0; i<dest.Width; i++)
//				for (int j=0; j<dest.Height; j++)
//					dest.SetPixel(i,j,b.GetPixel(dest.Height-j-1,i));
//
//			return(dest);
//		}

		private Color [] GetHotColorArray()
		{
			int num_color = 8 ; 
			Color [] colors;
			colors = new Color [num_color] ;
			for(int j = 0 ; j < num_color ; j++)
			{
				int i = (j*64)/num_color ; 
				int r , g , b ;
				if (i < 24)
				{
					r =  (int) (255 * i * 1.0 /24) ;
					g = 0 ;
					b = 0 ;
				}
				else if (i < 48)
				{
					r = 255 ;
					g =  (int) (255 * (i-24) * 1.0 /24) ;
					b = 0 ;
				}
				else
				{
					r = 255 ;
					g = 255 ;
					b = (int) (255 * (i - 48) * 1.0 / 16) ;
				}
				colors[j] = Color.FromArgb(255  , r,g,b) ;
			}
			return colors ; 
		}

		public void CreateHeatLegend()
		{
			Clear();

			//BitmapTools.PixelData p = new BitmapTools.PixelData();

			Color [] hotClrArray = GetHotColorArray() ; 
			int divs = hotClrArray.Length -1 ;
			double percentStep = 1.0 / (double)divs;

			ArrayList list = new ArrayList();
			for (int i=0; i<=divs; i++)
			{ 
				ctlLegendColor cp = new ctlLegendColor();
				cp.LegendColor = hotClrArray[i] ;
				cp.Percent = (double)i*percentStep;
				list.Add(cp);
			}

			//create the colormap bitmap
			LoadLegend(list);
		}

		public void CreateVisibleLightLegend()
		{
			Clear();

			BitmapTools.PixelData p = new BitmapTools.PixelData();

			int divs = 5;
			int step = (700-400)/divs;
			double percentStep = 1.0 / (double)divs;

			ArrayList list = new ArrayList();
			for (int i=0; i<=divs; i++)
			{ 
				ctlLegendColor cp = new ctlLegendColor();
				getRGB((float)(400+i*step), &p);
				cp.LegendColor = Color.FromArgb(p.red, p.green, p.blue);
				cp.Percent = (double)i*percentStep;
				list.Add(cp);
			}

			//create the colormap bitmap
			LoadLegend(list);
		}

		private bool mShowEdit = false;
		private int mViewWidth = 1;
		private void ShowEdit()
		{
			mViewWidth = this.Width;
			pnlEdit.Visible = true;
			this.Width = pnlEdit.Width + mViewWidth;
			SetCPActualPositions();
			mShowEdit=true;
			mEditApplied=false;
		}

		public void HideEdit()
		{
			pnlEdit.Visible = false;
			this.Width = mViewWidth;
			mShowEdit=false;

			if (!mEditApplied)
				CreateLegend(mViewBMP);
		}

		private void pic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button==MouseButtons.Right)
			{
				//EditLegend();
				if (!mShowEdit)
				{
					ShowEdit();
				}
				else
				{
					HideEdit();
				}
			}
		}
		public void UpdateAxis()
		{
			axis.UpdateAxis(this.LowerRange, this.UpperRange);
		}

		//**********************************************************************************
		// edit code

		public void Clear()
		{
			ColorPoints.Clear();
			pnlSlider.Controls.Clear();
		}

		public void SaveLegend(string fName)
		{
			try
			{
				Persistance persist = new Persistance();

				Type t = Type.GetType("PNNLControls.ctlLegendColor");

				MetaData mdata = new MetaData ();
				MetaNode child = mdata.OpenChild ("ColorPoints");
				persist.PersistList(ref ColorPoints, t, child, IDLTools.Persistance.Direction.SetToXML);
					
				mdata.WriteFile(fName);
			}
			catch{}
		}

		public void SaveLegend()
		{
			if (mLegendFile!=null)
			{
				SaveLegend(mLegendFile);
			}
			else
			{
				SaveLegendAs();
			}
		}

		public void LoadLegend(ArrayList points)
		{
			if (points==null) return;

			this.Clear();
			
			if (points.Count>0)
			{

				CPMin.LegendColor  = (points[0] as ctlLegendColor).LegendColor;
				CPMin.ColorChanged += new ctlLegendColor.ColorChangedDelegate(this.ColorChanged);
				pnlSlider.Controls.Add(CPMin);
				ColorPoints.Add(CPMin);

				CPMax.LegendColor  = (points[points.Count-1] as ctlLegendColor).LegendColor;
				CPMax.ColorChanged += new ctlLegendColor.ColorChangedDelegate(this.ColorChanged);
				pnlSlider.Controls.Add(CPMax);
				ColorPoints.Add(CPMax);

				for (int i=1; i<points.Count-1; i++)
				{
					AddColorPoint(points[i] as ctlLegendColor);
				}
			}
			InterpolateLegend();

			CreateLegend(mViewBMP);
		}

		public void LoadLegend(string fName)
		{
			Persistance persist = new Persistance();
			MetaNode child = null;
			ArrayList points = null;

			try
			{
				MetaData mdata = new MetaData ();
				mdata.ReadFile(fName);
				child = mdata.OpenChild ("ColorPoints");
			}
			catch
			{
				return;
			}

			try
			{
				Type t = Type.GetType("PNNLControls.ctlLegendColor");
				persist.PersistList(ref points, t, child, IDLTools.Persistance.Direction.GetFromXML);
			}
			catch
			{
				return;
			}

			LoadLegend(points);

			mLegendFile=fName;
		}

		public ApplyLegendMode ApplyMode
		{
			get{return mApplyMode;}
			set{mApplyMode=value;}
		}

		public Color MinColor
		{
			get{return this.CPMin.LegendColor;}
			set{this.CPMin.LegendColor=value;}
		}

		public Color MaxColor
		{
			get{return this.CPMax.LegendColor;}
			set{this.CPMax.LegendColor=value;}
		}

		private class SortXPosition : IComparer  
		{

			// Calls CaseInsensitiveComparer.Compare with the parameters reversed.
			int IComparer.Compare( Object x, Object y )  
			{
				try
				{
					Control cX = x as Control;
					Control cY = y as Control;
					return (cX.Left.CompareTo(cY.Left));
				}
				catch
				{
					throw (new ArgumentException("Arguments must be Controls"));
				}
			}
		}

		private class SortYPosition : IComparer  
		{

			// Calls CaseInsensitiveComparer.Compare with the parameters reversed.
			int IComparer.Compare( Object x, Object y )  
			{
				try
				{
					Control cX = x as Control;
					Control cY = y as Control;
					return (cX.Top.CompareTo(cY.Top));
				}
				catch
				{
					throw (new ArgumentException("Arguments must be Controls"));
				}
			}
		}

		private class SortPercent : IComparer  
		{

			// Calls CaseInsensitiveComparer.Compare with the parameters reversed.
			int IComparer.Compare( Object x, Object y )  
			{
				try
				{
					ctlLegendColor cX = x as ctlLegendColor;
					ctlLegendColor cY = y as ctlLegendColor;
					return (cX.Percent.CompareTo(cY.Percent));
				}
				catch
				{
					throw (new ArgumentException("Arguments must be ctlLegendColor"));
				}
			}
		}


		public ctlLegendColor AddColorPoint()
		{
			try
			{
				ctlLegendColor cp = new ctlLegendColor ();
				cp.LegendColor = CPMin.LegendColor;
		
				AddColorPoint(cp);

				cp.Top = CPMin.Top - cp.PointerPosition;
				SetCPRelativePosition(cp);

				return cp;
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
                
				return null;
			}
		}

		public void CPAssignDelagates(ctlLegendColor cp)
		{
			try
			{
				cp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CP_MouseDown);
				cp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CP_MouseUp);
				cp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CP_MouseMove);
				
				cp.ColorChanged += new ctlLegendColor.ColorChangedDelegate(this.ColorChanged);
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}

		public ctlLegendColor AddColorPoint(ctlLegendColor cp)
		{
			try
			{
				pnlSlider.Controls.Add (cp);
				cp.BackColor = Color.Transparent;
				cp.Mode = ctlLegendColor.DisplayMode.EastPointer;
				
				cp.Left = 0;
				cp.Height = CPMin.Height;
				cp.Width = pnlSlider.ClientSize.Width;

				//sets top of control
				SetCPActualPosition(cp);

				cp.BringToFront();

				CPAssignDelagates(cp);

				ColorPoints.Add(cp);
				return cp;
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
				return null;
			}
		}

		private void ColorChanged()
		{
			InterpolateLegend();
		}

		public void SetCPRelativePosition(ctlLegendColor cp)
		{
			cp.Percent = (double)(pnlSlider.ClientRectangle.Height - (cp.Top + cp.PointerPosition)) / (pnlSlider.ClientRectangle.Height);
		}

		public void SetCPRelativePositions()
		{
			for (int i=0; i<ColorPoints.Count; i++)
			{
				SetCPRelativePosition(ColorPoints[i] as ctlLegendColor);
			}
		}

		public void SetCPActualPosition(ctlLegendColor cp)
		{
			int ptrPos = (int) (pic.Height * cp.Percent);
			cp.Top = pic.Height - ptrPos  -  cp.PointerPosition;
		}

		public void SetCPActualPositions()
		{
			for (int i=0; i<ColorPoints.Count; i++)
			{
				SetCPActualPosition(ColorPoints[i] as ctlLegendColor);
			}
		}

		public void InterpolateLegend()
		{
			ColorPoints.Sort(new SortPercent());
			int bmpWidth = 10;
			mViewBMP = new Bitmap(bmpWidth, mNLegendPoints);

			for(int i=0; i<mViewBMP.Width; i++)
				for (int j=0; j<mViewBMP.Height; j++)
					mViewBMP.SetPixel(i,j,Color.Pink);

			double slideHeight = pnlSlider.ClientSize.Height;
			slideHeight = pic.Height;

			for (int i=0; i<ColorPoints.Count-1; i++)
			{
				ctlLegendColor cpLow = ColorPoints[i] as ctlLegendColor;
				ctlLegendColor cpHigh = ColorPoints[i+1] as ctlLegendColor;
				Color cLow = cpLow.LegendColor;
				Color cHigh = cpHigh.LegendColor;

				int ptrLow = cpLow.PointerPosition;

				int iLow = (int) ((double) mNLegendPoints * cpLow.Percent);
				int iHigh = (int) ((double) mNLegendPoints * cpHigh.Percent);

				double divisor = (double)(iHigh-iLow);
				for (int j=iLow; j<iHigh; j++)
				{
					double frac = (double) (j-iLow)/divisor;
					int R = (int)((double) cLow.R + (frac * ((double)cHigh.R - 
						(double)cLow.R + 1.0)));
					int B = (int)((double) cLow.B + (frac * ((double)cHigh.B - 
						(double)cLow.B + 1.0)));
					int G = (int)((double) cLow.G + (frac * ((double)cHigh.G - 
						(double)cLow.G + 1.0)));
					Color c = Color.FromArgb(R,G,B);

					for(int k=0; k<bmpWidth; k++)
						mViewBMP.SetPixel(k,mNLegendPoints-j-1,c);
				}
			}

			pic.Image = mViewBMP;
		}

		public Bitmap LegendBitmap
		{
			get{return mViewBMP;}
			set{mViewBMP=value;  pic.Image = value;}
		}

		/// <summary>
		/// mousedown event for label, selects and starts drag/drop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CP_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) 
		{      
			if (e.Button==MouseButtons.Right) return;

			try
			{
				ctlLegendColor cp = sender as ctlLegendColor;

				if (mDragEnabled)
				{
					// Remember the point where the mouse down occurred. The DragSize indicates
					// the size that the mouse can move before a drag event should be started.                
					Size dragSize = SystemInformation.DragSize;

					// Create a rectangle using the DragSize, with the mouse position being
					// at the center of the rectangle.
					dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width /2),
						e.Y - (dragSize.Height /2)), dragSize);
				}
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// mouseup event for label, ends drag/drop, raises selected event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CP_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			try
			{
				if (mDragEnabled)
				{
					// Reset the drag rectangle when the mouse button is raised.
					dragBoxFromMouseDown = Rectangle.Empty;
				}
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// mousemove event, continues drag/drop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CP_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			try
			{
				ctlLegendColor cp = sender as ctlLegendColor;
				tip.SetToolTip (cp as Control, StringValueAtPosition(cp.Top + cp.PointerPosition));
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}

			if (mDragEnabled)
			{
				try
				{
					if ((e.Button & MouseButtons.Left) == MouseButtons.Left) 
					{
						ctlLegendColor cp = sender as ctlLegendColor;

						// If the mouse moves outside the rectangle, start the drag.
						if (dragBoxFromMouseDown != Rectangle.Empty && 
							!dragBoxFromMouseDown.Contains(e.X, e.Y)) 
						{
							// Proceed with the drag and drop, passing in the dragged label.                    
							DragDropEffects dropEffect = cp.DoDragDrop(sender, DragDropEffects.Move);
						}
					}
				}
				catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
			}
		}

		/// <summary>
		/// dragover event for label, shows dragover label
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CP_DragOver(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			if (mDragEnabled)
			{
				try
				{
					if (e.Data.GetDataPresent(typeof(ctlLegendColor))) 
					{
						e.Effect = DragDropEffects.Move;

						ctlLegendColor cp =e.Data.GetData(typeof(ctlLegendColor))as ctlLegendColor;
						cp.Left = 0;
						Point p = pnlSlider.PointToClient(new Point(e.X, e.Y));

						if (p.Y <= pnlSlider.ClientSize.Height)
							cp.Top = p.Y-cp.PointerPosition;
					}

				}
				catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
			}
		}

		/// <summary>
		/// dragleave event for label, unselects dragover label
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CP_DragLeave(object sender, System.EventArgs e)
		{
			if (mDragEnabled)
			{
				ctlLegendColor cp = sender as ctlLegendColor;
			}
		
		}

		/// <summary>
		/// dragdrop event for label, raises Moved event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CP_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (mDragEnabled)
			{
				try
				{
					if (e.Data.GetDataPresent(typeof(ctlLegendColor))) 
					{
						ctlLegendColor cp =e.Data.GetData(typeof(ctlLegendColor))as ctlLegendColor;
						cp.Left = 0;
						Point p = pnlSlider.PointToClient(new Point(e.X, e.Y));
						if (p.Y <= pnlSlider.ClientSize.Height)
							cp.Top = p.Y-cp.PointerPosition;

						SetCPRelativePosition(cp);
						InterpolateLegend();
					}
				}
				catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
			}
		}

		private void btnAddIncColor_Click(object sender, System.EventArgs e)
		{
			AddColorPoint();
		}

		private void rdoLog_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rdoLog.Checked) mApplyMode = ApplyLegendMode.log;
		}

		private void rdoLinear_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rdoLinear.Checked) mApplyMode = ApplyLegendMode.linear;
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			ArrayList list = new ArrayList();

			ctlLegendColor c = new ctlLegendColor();
			c.LegendColor = Color.Black;
			list.Add(c);

			c = new ctlLegendColor();
			c.LegendColor = Color.White;
			list.Add(c);

			LoadLegend(list);
		}

		private void neMin_ValueChanged(double val, double previous)
		{
			UpdateAxis();
		}

		private void neMax_ValueChanged(double val, double previous)
		{
			UpdateAxis();
		}

		private void btnApply_Click(object sender, System.EventArgs e)
		{
			mEditApplied=false;
			HideEdit();
			ShowEdit();
			mEditApplied=true;
		}

		private double ValueAtPosition (int pos)
		{
			int inverted = pic.Height - pos;
			double factor = (UpperRange-LowerRange) / pic.Height;

			double dPos = (factor * inverted) + LowerRange;
			return dPos;
		}

		private string StringValueAtPosition (int pos)
		{
			double dPos = ValueAtPosition(pos);
			return dPos.ToString("0.000");
		}

		private void Color_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			tip.SetToolTip (sender as Control, StringValueAtPosition(e.Y));
		}

		private void btnAutoScale_Click(object sender, System.EventArgs e)
		{
			AxisZoomOut();
		}

		private string mLegendFile = "";
		public string LegendFile
		{
			get{return mLegendFile;}
			set{mLegendFile = value;}
		}

		private string mLegendDir = "";
		public string LegendDir
		{
			get{return mLegendDir;}
			set{mLegendDir = value;}
		}

		public void SaveLegendAs()
		{
			if (mLegendDir!="")
			{
				saveFile.InitialDirectory = mLegendDir;
			}

			if (saveFile.ShowDialog()==DialogResult.OK)
			{
				this.SaveLegend(saveFile.FileName);
			}
		}

		public void LoadLegend()
		{
			if (mLegendDir!="")
			{
				openFile.InitialDirectory = mLegendDir;
			}

			if (openFile.ShowDialog()==DialogResult.OK)
			{
				this.LoadLegend(openFile.FileName);
			}
		}
	}
}
