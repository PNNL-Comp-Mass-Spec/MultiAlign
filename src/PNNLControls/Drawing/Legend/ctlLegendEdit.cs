using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using IDLTools;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlLegendEdit.
	/// </summary>
	public class ctlLegendEdit : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// rectangle for drag/drop
		/// </summary>
		private Rectangle dragBoxFromMouseDown;
		private ArrayList ColorPoints = new ArrayList();
		private bool mDragEnabled = true;
		private Bitmap mBitmap = null;
		private int mNLegendPoints = 256;

		private System.Windows.Forms.Panel pnlColors;
		private PNNLControls.ctlLegendColor CPUnder;
		private PNNLControls.ctlLegendColor CPOver;
		private PNNLControls.ctlLegendColor CPNaN;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel pnBitmap;
		private System.Windows.Forms.Panel pnlInterp;
		private System.Windows.Forms.PictureBox picLegend;
		private System.Windows.Forms.Panel pnlRight;
		private System.Windows.Forms.Panel pnlLeft;
		private PNNLControls.ctlLegendColor CPMin;
		private PNNLControls.ctlLegendColor CPMax;
		private System.Windows.Forms.Panel pnlSlider;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnAddIncColor;
		private System.Windows.Forms.RadioButton rdoLinear;
		private System.Windows.Forms.RadioButton rdoLog;
		private System.Windows.Forms.Label lblMin;
		private System.Windows.Forms.Label lblMax;
		private PNNLControls.ctlNumEdit neMin;
		private PNNLControls.ctlNumEdit neMax;
		private System.Windows.Forms.CheckBox chkZscore;

		private clsPlotSingleAxis plotter = new clsPlotSingleAxis();
		private System.Windows.Forms.PictureBox picAxis;

		#region constructors

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlLegendEdit()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

			//initialize min and max colors
			this.Clear();
			InterpolateLegend();
			
			UpdateAxis();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnBitmap = new System.Windows.Forms.Panel();
			this.pnlInterp = new System.Windows.Forms.Panel();
			this.picAxis = new System.Windows.Forms.PictureBox();
			this.pnlLeft = new System.Windows.Forms.Panel();
			this.pnlRight = new System.Windows.Forms.Panel();
			this.pnlSlider = new System.Windows.Forms.Panel();
			this.CPMin = new PNNLControls.ctlLegendColor();
			this.CPMax = new PNNLControls.ctlLegendColor();
			this.picLegend = new System.Windows.Forms.PictureBox();
			this.pnlColors = new System.Windows.Forms.Panel();
			this.CPOver = new PNNLControls.ctlLegendColor();
			this.label3 = new System.Windows.Forms.Label();
			this.CPNaN = new PNNLControls.ctlLegendColor();
			this.label2 = new System.Windows.Forms.Label();
			this.CPUnder = new PNNLControls.ctlLegendColor();
			this.label1 = new System.Windows.Forms.Label();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnAddIncColor = new System.Windows.Forms.Button();
			this.rdoLinear = new System.Windows.Forms.RadioButton();
			this.rdoLog = new System.Windows.Forms.RadioButton();
			this.lblMin = new System.Windows.Forms.Label();
			this.lblMax = new System.Windows.Forms.Label();
			this.neMin = new PNNLControls.ctlNumEdit();
			this.neMax = new PNNLControls.ctlNumEdit();
			this.chkZscore = new System.Windows.Forms.CheckBox();
			this.pnBitmap.SuspendLayout();
			this.pnlInterp.SuspendLayout();
			this.pnlSlider.SuspendLayout();
			this.pnlColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnBitmap
			// 
			this.pnBitmap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnBitmap.Controls.Add(this.pnlInterp);
			this.pnBitmap.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnBitmap.Location = new System.Drawing.Point(0, 0);
			this.pnBitmap.Name = "pnBitmap";
			this.pnBitmap.Size = new System.Drawing.Size(662, 70);
			this.pnBitmap.TabIndex = 7;
			// 
			// pnlInterp
			// 
			this.pnlInterp.Controls.Add(this.picAxis);
			this.pnlInterp.Controls.Add(this.pnlLeft);
			this.pnlInterp.Controls.Add(this.pnlRight);
			this.pnlInterp.Controls.Add(this.pnlSlider);
			this.pnlInterp.Controls.Add(this.picLegend);
			this.pnlInterp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlInterp.Location = new System.Drawing.Point(0, 0);
			this.pnlInterp.Name = "pnlInterp";
			this.pnlInterp.Size = new System.Drawing.Size(660, 68);
			this.pnlInterp.TabIndex = 10;
			// 
			// picAxis
			// 
			this.picAxis.BackColor = System.Drawing.SystemColors.Control;
			this.picAxis.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.picAxis.Location = new System.Drawing.Point(6, 30);
			this.picAxis.Name = "picAxis";
			this.picAxis.Size = new System.Drawing.Size(648, 16);
			this.picAxis.TabIndex = 6;
			this.picAxis.TabStop = false;
			// 
			// pnlLeft
			// 
			this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlLeft.Location = new System.Drawing.Point(0, 0);
			this.pnlLeft.Name = "pnlLeft";
			this.pnlLeft.Size = new System.Drawing.Size(6, 46);
			this.pnlLeft.TabIndex = 5;
			// 
			// pnlRight
			// 
			this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlRight.Location = new System.Drawing.Point(654, 0);
			this.pnlRight.Name = "pnlRight";
			this.pnlRight.Size = new System.Drawing.Size(6, 46);
			this.pnlRight.TabIndex = 4;
			// 
			// pnlSlider
			// 
			this.pnlSlider.BackColor = System.Drawing.SystemColors.Control;
			this.pnlSlider.Controls.Add(this.CPMin);
			this.pnlSlider.Controls.Add(this.CPMax);
			this.pnlSlider.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlSlider.Location = new System.Drawing.Point(0, 46);
			this.pnlSlider.Name = "pnlSlider";
			this.pnlSlider.Size = new System.Drawing.Size(660, 22);
			this.pnlSlider.TabIndex = 0;
			// 
			// CPMin
			// 
			this.CPMin.BackColor = System.Drawing.Color.Transparent;
			this.CPMin.Dock = System.Windows.Forms.DockStyle.Left;
			this.CPMin.LegendColor = System.Drawing.Color.Black;
			this.CPMin.Location = new System.Drawing.Point(0, 0);
			this.CPMin.Mode = PNNLControls.ctlLegendColor.DisplayMode.NorthPointer;
			this.CPMin.Name = "CPMin";
			this.CPMin.Percent = 0;
			this.CPMin.Size = new System.Drawing.Size(14, 22);
			this.CPMin.TabIndex = 0;
			// 
			// CPMax
			// 
			this.CPMax.BackColor = System.Drawing.Color.Transparent;
			this.CPMax.Dock = System.Windows.Forms.DockStyle.Right;
			this.CPMax.LegendColor = System.Drawing.Color.White;
			this.CPMax.Location = new System.Drawing.Point(646, 0);
			this.CPMax.Mode = PNNLControls.ctlLegendColor.DisplayMode.NorthPointer;
			this.CPMax.Name = "CPMax";
			this.CPMax.Percent = 0;
			this.CPMax.Size = new System.Drawing.Size(14, 22);
			this.CPMax.TabIndex = 1;
			// 
			// picLegend
			// 
			this.picLegend.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.picLegend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picLegend.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picLegend.Location = new System.Drawing.Point(0, 0);
			this.picLegend.Name = "picLegend";
			this.picLegend.Size = new System.Drawing.Size(660, 68);
			this.picLegend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picLegend.TabIndex = 3;
			this.picLegend.TabStop = false;
			// 
			// pnlColors
			// 
			this.pnlColors.Controls.Add(this.CPOver);
			this.pnlColors.Controls.Add(this.label3);
			this.pnlColors.Controls.Add(this.CPNaN);
			this.pnlColors.Controls.Add(this.label2);
			this.pnlColors.Controls.Add(this.CPUnder);
			this.pnlColors.Controls.Add(this.label1);
			this.pnlColors.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlColors.Location = new System.Drawing.Point(0, 70);
			this.pnlColors.Name = "pnlColors";
			this.pnlColors.Size = new System.Drawing.Size(662, 24);
			this.pnlColors.TabIndex = 8;
			// 
			// CPOver
			// 
			this.CPOver.BackColor = System.Drawing.SystemColors.Control;
			this.CPOver.Dock = System.Windows.Forms.DockStyle.Left;
			this.CPOver.LegendColor = System.Drawing.SystemColors.Control;
			this.CPOver.Location = new System.Drawing.Point(274, 0);
			this.CPOver.Mode = PNNLControls.ctlLegendColor.DisplayMode.EastPointer;
			this.CPOver.Name = "CPOver";
			this.CPOver.Percent = 0;
			this.CPOver.Size = new System.Drawing.Size(26, 24);
			this.CPOver.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Dock = System.Windows.Forms.DockStyle.Left;
			this.label3.Location = new System.Drawing.Point(200, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 24);
			this.label3.TabIndex = 5;
			this.label3.Text = "Over Range";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// CPNaN
			// 
			this.CPNaN.BackColor = System.Drawing.SystemColors.Control;
			this.CPNaN.Dock = System.Windows.Forms.DockStyle.Left;
			this.CPNaN.LegendColor = System.Drawing.Color.Gray;
			this.CPNaN.Location = new System.Drawing.Point(174, 0);
			this.CPNaN.Mode = PNNLControls.ctlLegendColor.DisplayMode.Block;
			this.CPNaN.Name = "CPNaN";
			this.CPNaN.Percent = 0;
			this.CPNaN.Size = new System.Drawing.Size(26, 24);
			this.CPNaN.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Left;
			this.label2.Location = new System.Drawing.Point(100, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 24);
			this.label2.TabIndex = 4;
			this.label2.Text = "Not a Number";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// CPUnder
			// 
			this.CPUnder.BackColor = System.Drawing.SystemColors.Control;
			this.CPUnder.Dock = System.Windows.Forms.DockStyle.Left;
			this.CPUnder.LegendColor = System.Drawing.SystemColors.Control;
			this.CPUnder.Location = new System.Drawing.Point(74, 0);
			this.CPUnder.Mode = PNNLControls.ctlLegendColor.DisplayMode.WestPointer;
			this.CPUnder.Name = "CPUnder";
			this.CPUnder.Percent = 0;
			this.CPUnder.Size = new System.Drawing.Size(26, 24);
			this.CPUnder.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Left;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 24);
			this.label1.TabIndex = 3;
			this.label1.Text = "Below Range";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(290, 110);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(80, 20);
			this.btnClear.TabIndex = 17;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnAddIncColor
			// 
			this.btnAddIncColor.Location = new System.Drawing.Point(290, 130);
			this.btnAddIncColor.Name = "btnAddIncColor";
			this.btnAddIncColor.Size = new System.Drawing.Size(80, 20);
			this.btnAddIncColor.TabIndex = 14;
			this.btnAddIncColor.Text = "Add Color";
			this.btnAddIncColor.Click += new System.EventHandler(this.btnAddIncColor_Click);
			// 
			// rdoLinear
			// 
			this.rdoLinear.Location = new System.Drawing.Point(4, 130);
			this.rdoLinear.Name = "rdoLinear";
			this.rdoLinear.Size = new System.Drawing.Size(54, 20);
			this.rdoLinear.TabIndex = 15;
			this.rdoLinear.Text = "Linear";
			//this.rdoLinear.CheckedChanged += new System.EventHandler(this.rdoLinear_CheckedChanged);
			// 
			// rdoLog
			// 
			this.rdoLog.Checked = true;
			this.rdoLog.Location = new System.Drawing.Point(4, 108);
			this.rdoLog.Name = "rdoLog";
			this.rdoLog.Size = new System.Drawing.Size(54, 20);
			this.rdoLog.TabIndex = 16;
			this.rdoLog.TabStop = true;
			this.rdoLog.Text = "Log";
			//this.rdoLog.CheckedChanged += new System.EventHandler(this.rdoLog_CheckedChanged);
			// 
			// lblMin
			// 
			this.lblMin.Location = new System.Drawing.Point(134, 110);
			this.lblMin.Name = "lblMin";
			this.lblMin.Size = new System.Drawing.Size(30, 18);
			this.lblMin.TabIndex = 20;
			this.lblMin.Text = "Min";
			// 
			// lblMax
			// 
			this.lblMax.Location = new System.Drawing.Point(136, 136);
			this.lblMax.Name = "lblMax";
			this.lblMax.Size = new System.Drawing.Size(30, 18);
			this.lblMax.TabIndex = 21;
			this.lblMax.Text = "Max";
			// 
			// neMin
			// 
			this.neMin.Location = new System.Drawing.Point(170, 110);
			this.neMin.MaxValue = 1.7976931348623157E+308;
			this.neMin.MinValue = -1.7976931348623157E+308;
			this.neMin.Name = "neMin";
			this.neMin.Size = new System.Drawing.Size(114, 20);
			this.neMin.TabIndex = 22;
			this.neMin.Text = "0";
			this.neMin.Value = 0;
			this.neMin.ValueChanged += new PNNLControls.ctlNumEdit.ValueChangedDelegate(this.neMin_ValueChanged);
			// 
			// neMax
			// 
			this.neMax.Location = new System.Drawing.Point(170, 134);
			this.neMax.MaxValue = 1.7976931348623157E+308;
			this.neMax.MinValue = -1.7976931348623157E+308;
			this.neMax.Name = "neMax";
			this.neMax.Size = new System.Drawing.Size(114, 20);
			this.neMax.TabIndex = 0;
			this.neMax.Text = "0";
			this.neMax.Value = 0;
			this.neMax.ValueChanged += new PNNLControls.ctlNumEdit.ValueChangedDelegate(this.neMax_ValueChanged);
			this.neMax.TextChanged += new System.EventHandler(this.neMax_TextChanged);
			// 
			// chkZscore
			// 
			this.chkZscore.Checked = true;
			this.chkZscore.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkZscore.Location = new System.Drawing.Point(64, 110);
			this.chkZscore.Name = "chkZscore";
			this.chkZscore.Size = new System.Drawing.Size(64, 16);
			this.chkZscore.TabIndex = 23;
			this.chkZscore.Text = "Z Score";
			this.chkZscore.CheckedChanged += new System.EventHandler(this.chkZscore_CheckedChanged);
			// 
			// ctlLegendEdit
			// 
			this.Controls.Add(this.chkZscore);
			this.Controls.Add(this.neMax);
			this.Controls.Add(this.neMin);
			this.Controls.Add(this.lblMax);
			this.Controls.Add(this.lblMin);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.btnAddIncColor);
			this.Controls.Add(this.rdoLinear);
			this.Controls.Add(this.rdoLog);
			this.Controls.Add(this.pnlColors);
			this.Controls.Add(this.pnBitmap);
			this.Name = "ctlLegendEdit";
			this.Size = new System.Drawing.Size(662, 178);
			this.Load += new System.EventHandler(this.ctlLegendEdit_Load);
			this.pnBitmap.ResumeLayout(false);
			this.pnlInterp.ResumeLayout(false);
			this.pnlSlider.ResumeLayout(false);
			this.pnlColors.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

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

		}

		public bool ZScore
		{
			get{return chkZscore.Checked;}
			set{chkZscore.Checked=value;}
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

		public Color UnderColor
		{
			get{return this.CPUnder.LegendColor;}
			set{this.CPUnder.LegendColor=value;}
		}

		public Color OverColor
		{
			get{return this.CPOver.LegendColor;}
			set{this.CPOver.LegendColor=value;}
		}

		public Color NaNColor
		{
			get{return this.CPNaN.LegendColor;}
			set{this.CPNaN.LegendColor=value;}
		}

		public float MinRange
		{
			get{return (float) this.neMin.Value;}
			set{this.neMin.Value=(double)value;}
		}

		public float MaxRange
		{
			get{return (float) this.neMax.Value;}
			set{this.neMax.Value=(double)value;}
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


		public ctlLegendColor AddColorPoint()
		{
			try
			{
				ctlLegendColor cp = new ctlLegendColor ();
				cp.LegendColor = CPMin.LegendColor;

				cp.Left = CPMin.Width;
				SetCPRelativePosition(cp);
		
				return AddColorPoint(cp);
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
				return null;
			}
		}

		public ctlLegendColor AddColorPoint(ctlLegendColor cp)
		{
			try
			{
				pnlSlider.Controls.Add (cp);
				cp.BackColor = Color.Transparent;
				cp.Top = 0;

				SetCPActualPosition(cp);

				cp.Width = CPMin.Width;
				cp.Height = pnlSlider.ClientSize.Height;
				cp.Mode = ctlLegendColor.DisplayMode.NorthPointer;
				//cp.BorderStyle = BorderStyle.FixedSingle;
				cp.BringToFront();

				cp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CP_MouseDown);
				cp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CP_MouseUp);
				cp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CP_MouseMove);
				
				cp.ColorChanged += new ctlLegendColor.ColorChangedDelegate(this.ColorChanged);

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
			double slideWidth = (pnlSlider.ClientSize.Width - CPMin.Width);
			cp.Percent = (double) cp.Left / slideWidth;
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
			double slideWidth = (pnlSlider.ClientSize.Width - CPMin.Width);
			cp.Left = (int) (slideWidth * cp.Percent);
		}

		public void InterpolateLegend()
		{
			ColorPoints.Sort(new SortXPosition());
			SetCPRelativePositions();

			int bitHeight = 10;
			mBitmap = new Bitmap(mNLegendPoints, 10);

			double slideWidth = (pnlSlider.ClientSize.Width - CPMin.Width);

			for (int i=0; i<ColorPoints.Count-1; i++)
			{
				ctlLegendColor cpLow = ColorPoints[i] as ctlLegendColor;
				ctlLegendColor cpHigh = ColorPoints[i+1] as ctlLegendColor;
				Color cLow = cpLow.LegendColor;
				Color cHigh = cpHigh.LegendColor;

				int iLow = (int) ((double) mNLegendPoints * 
					(double) cpLow.Left / slideWidth);
				int iHigh = (int) ((double) mNLegendPoints * 
					(double) cpHigh.Left / slideWidth);

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
					for(int k=0; k<bitHeight; k++)
						mBitmap.SetPixel(j,k,c);
				}

				picLegend.Image = mBitmap;
			}
		}

		public Bitmap LegendBitmap
		{
			get{return mBitmap;}
			set{mBitmap=value;  picLegend.Image = value;}
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
		/// mousemove event for label, continues drag/drop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CP_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
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
						cp.Top = 0;
						Point p = pnlSlider.PointToClient(new Point(e.X, e.Y));

						if (p.X <= pnlSlider.ClientSize.Width - CPMin.Width)
							cp.Left = p.X;
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
						cp.Top = 0;
						Point p = pnlSlider.PointToClient(new Point(e.X, e.Y));
						if (p.X <= pnlSlider.ClientSize.Width - CPMin.Width)
							cp.Left = p.X;
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

		private void ctlLegendEdit_Load(object sender, System.EventArgs e)
		{
		
		}

//		private void rdoLog_CheckedChanged(object sender, System.EventArgs e)
//		{
//			if (rdoLog.Checked) ApplyMode = ApplyLegendMode.log;
//		}
//
//		private void rdoLinear_CheckedChanged(object sender, System.EventArgs e)
//		{
//			if (rdoLinear.Checked) ApplyMode = ApplyLegendMode.linear;
//		}

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

		private void UpdateAxis()
		{
			try
			{
				plotter.IsVertical = false;
				plotter.Bounds = picAxis.ClientRectangle;

				if (neMin.Value < neMax.Value)
					plotter.UnitPlotter.SetRange((float)neMin.Value, (float)neMax.Value);
				Graphics g = picAxis.CreateGraphics();
				g.FillRectangle(new SolidBrush(this.BackColor), picAxis.ClientRectangle);
				plotter.Layout(g);
				plotter.Draw(g, Color.Black);
				g.Dispose();
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		private void neMin_ValueChanged(double val, double previous)
		{
			UpdateAxis();
		}

		private void neMax_ValueChanged(double val, double previous)
		{
			UpdateAxis();
		}

		private void neMax_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void chkZscore_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
