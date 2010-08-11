using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlDataFrame.
	/// </summary>
	public class ctlDataFrame : System.Windows.Forms.UserControl
	{
		protected System.Windows.Forms.Panel panel1;
		protected PNNLControls.ctlHierarchalLabel hLabelHorz;
		protected PNNLControls.ctlHierarchalLabel hLabelVert;
		protected PNNLControls.ctlHeatMapLegend legend;
		protected System.Windows.Forms.Panel pnlSE;
		protected System.Windows.Forms.Panel pnlHeader;
		protected System.Windows.Forms.PictureBox picRight;
		protected System.Windows.Forms.PictureBox picLeft;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public event ctlHierarchalLabel.AlignDelegate AlignHorizontal = null;
		public event ctlHierarchalLabel.AlignDelegate AlignVertical = null;

		public event ctlHierarchalLabel.LeavesMovedDelegate LeavesMovedHorizontal = null;
		public event ctlHierarchalLabel.LeavesMovedDelegate LeavesMovedVertical = null;
		public event ctlHierarchalLabel.SelectedDelegate  SelectedHorizontal = null;
		public event ctlHierarchalLabel.SelectedDelegate SelectedVertical = null;
		public event ctlHierarchalLabel.RangeDelegate  RangeHorizontal = null;
		public event ctlHierarchalLabel.RangeDelegate  RangeVertical = null;
		public event ctlHierarchalLabel.LabelUpdateDelegate LabelsUpdated = null;

		private bool mbln_overrideResize = false;

		/// <summary>
		/// 
		/// </summary>
		public delegate void ClientSizeDelegate (Size size);
		public new event ClientSizeDelegate ClientSize = null;

		/// <summary>
		/// 
		/// </summary>
		public delegate void ClientLocationDelegate (Point location);
		public event ClientLocationDelegate ClientLocation = null;

		private Size mClientSize = new Size(int.MinValue, int.MinValue);
		private System.Windows.Forms.Panel pnlStatus;
		private System.Windows.Forms.StatusBar statBar;
		private System.Windows.Forms.StatusBarPanel Vertical;
		private System.Windows.Forms.StatusBarPanel Horizontal;
		private System.Windows.Forms.ProgressBar progBar;
		protected System.Windows.Forms.Splitter splitterHorizontal;
		protected System.Windows.Forms.Splitter splitterVertical;
		private Point mClientLocation = new Point(0, 0);

		public ctlDataFrame()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			legend.CreateHeatLegend() ; 

			this.splitterHorizontal.Move += new EventHandler(splitter_Move);
			this.splitterVertical.Move	 += new EventHandler(splitter_Move);

			this.splitterHorizontal.MouseDown += new MouseEventHandler(splitter_MouseDown);
			this.splitterHorizontal.MouseUp   += new MouseEventHandler(splitter_MouseUp);
			this.splitterVertical.MouseDown   += new MouseEventHandler(splitter_MouseDown);
			this.splitterVertical.MouseUp	  += new MouseEventHandler(splitter_MouseUp);
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

		public bool OverrideResize
		{
			get
			{
				return mbln_overrideResize;
			}
			set
			{
				mbln_overrideResize = value;
				VerticalLabel.OverrideResize = value;
				HorizontalLabel.OverrideResize = value;
			}
		}

		#region Properties

		private bool mUpdateComplete=true;
		public bool UpdateComplete
		{
			get{return this.mUpdateComplete;}
			set{this.mUpdateComplete = value;}
		}

		public ctlHeatMapLegend Legend 
		{
			get{return this.legend;}
			set{this.legend = value;}
		}

		[System.ComponentModel.Browsable(true)]
		public ctlHierarchalLabel VerticalLabel
		{
			get{return this.hLabelVert;}
			set{this.hLabelVert = value;}
		}

		[System.ComponentModel.Browsable(true)]
		public ctlHierarchalLabel HorizontalLabel
		{
			get{return this.hLabelHorz;}
			set{this.hLabelHorz = value;}
		}


		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            PNNLControls.clsLabelAttributes clsLabelAttributes2 = new PNNLControls.clsLabelAttributes();
            PNNLControls.clsLabelAttributes clsLabelAttributes3 = new PNNLControls.clsLabelAttributes();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlDataFrame));
            this.hLabelVert = new PNNLControls.ctlHierarchalLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.hLabelHorz = new PNNLControls.ctlHierarchalLabel();
            this.picLeft = new System.Windows.Forms.PictureBox();
            this.pnlSE = new System.Windows.Forms.Panel();
            this.picRight = new System.Windows.Forms.PictureBox();
            this.legend = new PNNLControls.ctlHeatMapLegend();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.statBar = new System.Windows.Forms.StatusBar();
            this.Vertical = new System.Windows.Forms.StatusBarPanel();
            this.Horizontal = new System.Windows.Forms.StatusBarPanel();
            this.progBar = new System.Windows.Forms.ProgressBar();
            this.splitterVertical = new System.Windows.Forms.Splitter();
            this.splitterHorizontal = new System.Windows.Forms.Splitter();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLeft)).BeginInit();
            this.pnlSE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRight)).BeginInit();
            this.pnlHeader.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Vertical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Horizontal)).BeginInit();
            this.SuspendLayout();
            // 
            // hLabelVert
            // 
            this.hLabelVert.BackColor = System.Drawing.SystemColors.Control;
            this.hLabelVert.Dock = System.Windows.Forms.DockStyle.Left;
            this.hLabelVert.DragEnabled = true;
            this.hLabelVert.DragOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.hLabelVert.LevelWidth = 24;
            this.hLabelVert.Location = new System.Drawing.Point(0, 56);
            this.hLabelVert.Mode = PNNLControls.ctlHierarchalLabel.AxisMode.axisModeLabel;
            this.hLabelVert.Name = "hLabelVert";
            this.hLabelVert.OverrideResize = false;
            this.hLabelVert.Root = clsLabelAttributes2;
            this.hLabelVert.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.hLabelVert.ShowAxis = false;
            this.hLabelVert.ShowScroll = false;
            this.hLabelVert.Size = new System.Drawing.Size(130, 424);
            this.hLabelVert.SortFunction = null;
            this.hLabelVert.TabIndex = 0;
            this.hLabelVert.UpdateComplete = true;
            this.hLabelVert.Vertical = true;
            this.hLabelVert.Load += new System.EventHandler(this.hLabelVert_Load);
            this.hLabelVert.LeavesMoved += new PNNLControls.ctlHierarchalLabel.LeavesMovedDelegate(this.hLabelVert_LeavesMoved);
            this.hLabelVert.ClientHeight += new PNNLControls.ctlHierarchalLabel.ClientHeightDelegate(this.hLabelVert_ClientHeight);
            this.hLabelVert.Resize += new System.EventHandler(this.hLabelVert_Resize);
            this.hLabelVert.ScrollPosition += new PNNLControls.ctlHierarchalLabel.ScrollPositionDelegate(this.hLabelVert_ScrollPosition);
            this.hLabelVert.LabelUpdated += new PNNLControls.ctlHierarchalLabel.LabelUpdateDelegate(this.hLabelVert_LabelUpdated);
            this.hLabelVert.Range += new PNNLControls.ctlHierarchalLabel.RangeDelegate(this.hLabelVert_Range);
            this.hLabelVert.Align += new PNNLControls.ctlHierarchalLabel.AlignDelegate(this.hLabelVert_Align);
            this.hLabelVert.Selected += new PNNLControls.ctlHierarchalLabel.SelectedDelegate(this.hLabelVert_Selected);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.hLabelHorz);
            this.panel1.Controls.Add(this.picLeft);
            this.panel1.Controls.Add(this.pnlSE);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 480);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(632, 120);
            this.panel1.TabIndex = 1;
            // 
            // hLabelHorz
            // 
            this.hLabelHorz.BackColor = System.Drawing.SystemColors.Control;
            this.hLabelHorz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hLabelHorz.DragEnabled = true;
            this.hLabelHorz.DragOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.hLabelHorz.LevelWidth = 24;
            this.hLabelHorz.Location = new System.Drawing.Point(136, 0);
            this.hLabelHorz.Mode = PNNLControls.ctlHierarchalLabel.AxisMode.axisModeLabel;
            this.hLabelHorz.Name = "hLabelHorz";
            this.hLabelHorz.OverrideResize = false;
            this.hLabelHorz.Root = clsLabelAttributes3;
            this.hLabelHorz.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.hLabelHorz.ShowAxis = false;
            this.hLabelHorz.ShowScroll = false;
            this.hLabelHorz.Size = new System.Drawing.Size(456, 120);
            this.hLabelHorz.SortFunction = null;
            this.hLabelHorz.TabIndex = 1;
            this.hLabelHorz.UpdateComplete = true;
            this.hLabelHorz.Vertical = false;
            this.hLabelHorz.LeavesMoved += new PNNLControls.ctlHierarchalLabel.LeavesMovedDelegate(this.hLabelHorz_LeavesMoved);
            this.hLabelHorz.ScrollPosition += new PNNLControls.ctlHierarchalLabel.ScrollPositionDelegate(this.hLabelHorz_ScrollPosition);
            this.hLabelHorz.LabelUpdated += new PNNLControls.ctlHierarchalLabel.LabelUpdateDelegate(this.hLabelHorz_LabelUpdated);
            this.hLabelHorz.ClientWidth += new PNNLControls.ctlHierarchalLabel.ClientWidthDelegate(this.hLabelHorz_ClientWidth);
            this.hLabelHorz.Range += new PNNLControls.ctlHierarchalLabel.RangeDelegate(this.hLabelHorz_Range);
            this.hLabelHorz.Align += new PNNLControls.ctlHierarchalLabel.AlignDelegate(this.hLabelHorz_Align);
            this.hLabelHorz.Selected += new PNNLControls.ctlHierarchalLabel.SelectedDelegate(this.hLabelHorz_Selected);
            // 
            // picLeft
            // 
            this.picLeft.BackColor = System.Drawing.SystemColors.Control;
            this.picLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.picLeft.Location = new System.Drawing.Point(0, 0);
            this.picLeft.Name = "picLeft";
            this.picLeft.Size = new System.Drawing.Size(136, 120);
            this.picLeft.TabIndex = 3;
            this.picLeft.TabStop = false;
            // 
            // pnlSE
            // 
            this.pnlSE.Controls.Add(this.picRight);
            this.pnlSE.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSE.Location = new System.Drawing.Point(592, 0);
            this.pnlSE.Name = "pnlSE";
            this.pnlSE.Size = new System.Drawing.Size(40, 120);
            this.pnlSE.TabIndex = 2;
            // 
            // picRight
            // 
            this.picRight.BackColor = System.Drawing.SystemColors.Control;
            this.picRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.picRight.Location = new System.Drawing.Point(0, 0);
            this.picRight.Name = "picRight";
            this.picRight.Size = new System.Drawing.Size(40, 120);
            this.picRight.TabIndex = 1;
            this.picRight.TabStop = false;
            // 
            // legend
            // 
            this.legend.ApplyMode = PNNLControls.ctlHeatMapLegend.ApplyLegendMode.linear;
            this.legend.BackColor = System.Drawing.SystemColors.Control;
            this.legend.Dock = System.Windows.Forms.DockStyle.Right;
            this.legend.LegendBitmap = ((System.Drawing.Bitmap)(resources.GetObject("legend.LegendBitmap")));
            this.legend.LegendDir = "";
            this.legend.LegendFile = "";
            this.legend.Location = new System.Drawing.Point(592, 56);
            this.legend.LowerRange = 0F;
            this.legend.MaxColor = System.Drawing.Color.White;
            this.legend.MinColor = System.Drawing.Color.Black;
            this.legend.Name = "legend";
            this.legend.NaNColor = System.Drawing.Color.Gray;
            this.legend.OverColor = System.Drawing.Color.White;
            this.legend.Size = new System.Drawing.Size(40, 419);
            this.legend.TabIndex = 5;
            this.legend.UnderColor = System.Drawing.Color.Black;
            this.legend.UpperRange = 0F;
            this.legend.UseZScore = false;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeader.Controls.Add(this.pnlStatus);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(632, 56);
            this.pnlHeader.TabIndex = 6;
            // 
            // pnlStatus
            // 
            this.pnlStatus.Controls.Add(this.statBar);
            this.pnlStatus.Controls.Add(this.progBar);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStatus.Location = new System.Drawing.Point(0, 0);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(630, 54);
            this.pnlStatus.TabIndex = 3;
            // 
            // statBar
            // 
            this.statBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statBar.Location = new System.Drawing.Point(0, 0);
            this.statBar.Name = "statBar";
            this.statBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.Vertical,
            this.Horizontal});
            this.statBar.ShowPanels = true;
            this.statBar.Size = new System.Drawing.Size(630, 31);
            this.statBar.TabIndex = 2;
            // 
            // Vertical
            // 
            this.Vertical.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.Vertical.Name = "Vertical";
            this.Vertical.Width = 306;
            // 
            // Horizontal
            // 
            this.Horizontal.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.Horizontal.Name = "Horizontal";
            this.Horizontal.Width = 306;
            // 
            // progBar
            // 
            this.progBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progBar.Location = new System.Drawing.Point(0, 31);
            this.progBar.Name = "progBar";
            this.progBar.Size = new System.Drawing.Size(630, 23);
            this.progBar.TabIndex = 3;
            this.progBar.Visible = false;
            // 
            // splitterVertical
            // 
            this.splitterVertical.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitterVertical.Location = new System.Drawing.Point(130, 56);
            this.splitterVertical.Name = "splitterVertical";
            this.splitterVertical.Size = new System.Drawing.Size(5, 419);
            this.splitterVertical.TabIndex = 7;
            this.splitterVertical.TabStop = false;
            // 
            // splitterHorizontal
            // 
            this.splitterHorizontal.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitterHorizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterHorizontal.Location = new System.Drawing.Point(130, 475);
            this.splitterHorizontal.Name = "splitterHorizontal";
            this.splitterHorizontal.Size = new System.Drawing.Size(502, 5);
            this.splitterHorizontal.TabIndex = 8;
            this.splitterHorizontal.TabStop = false;
            // 
            // ctlDataFrame
            // 
            this.Controls.Add(this.splitterVertical);
            this.Controls.Add(this.legend);
            this.Controls.Add(this.splitterHorizontal);
            this.Controls.Add(this.hLabelVert);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlHeader);
            this.Name = "ctlDataFrame";
            this.Size = new System.Drawing.Size(632, 600);
            this.Load += new System.EventHandler(this.ctlDataFrame_Load);
            this.Resize += new System.EventHandler(this.DataFrame_Resize);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLeft)).EndInit();
            this.pnlSE.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picRight)).EndInit();
            this.pnlHeader.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Vertical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Horizontal)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Status

		private int mProgValue = 0;
		private bool mProgRunning = false;

		public bool ShowProgBar
		{
			get{return progBar.Visible;}
			set{progBar.Visible = value;}
		}

		public int ProgBarPercent
		{
			get{return progBar.Value;}
			set{progBar.Value = value;}
		}

		public ProgressBar ProgBar
		{
			get{return progBar;}
			set{progBar = value;}
		}

		public delegate void UpdateProgress( int percent);

		private void OnUpdateProgress( int percent)
		{
			progBar.Value = percent;						
			progBar.Refresh();					
		}

		public void  ShowProgressAsync(int percent)
		{
			Invoke(new UpdateProgress(OnUpdateProgress), new object [] {mProgValue});
		}

		private void ProgressThread()
		{
			try
			{
				while (true)
				{
					mProgValue+=1;					
					mProgValue %= 100;

					Invoke(new UpdateProgress(OnUpdateProgress), new object [] { mProgValue});
					
					Thread.Sleep(50);
				}
			}
			catch(ThreadInterruptedException){}
		}

		private Thread mProgressThread = null;

		public void StartProgress()
		{
			if (mProgressThread != null || mProgRunning==true) return;

			progBar.Value = 0;
			mProgValue=0;
			
			mProgRunning = true;
			mProgressThread = new Thread(new ThreadStart(this.ProgressThread));
			mProgressThread.Start();
		}

		public void StopProgress()
		{
			mProgressThread.Interrupt();
			mProgressThread.Join();
			mProgressThread= null;
			mProgRunning=false;
			progBar.Value = 0;
			progBar.Refresh();
		}

		public bool ShowStatBar
		{
			get{return statBar.Visible;}
			set{statBar.Visible = value;}
		}

		public StatusBar  StatBar
		{
			get{return statBar;}
			set{statBar = value;}
		}
		#endregion

		private void DataFrame_Load(object sender, System.EventArgs e)
		{
		}

		private void DataFrame_Resize(object sender, System.EventArgs e)
		{
			/// Overriding allows the derived class to resize the base.
			if (mbln_overrideResize == false)
			{
				UpdateControlSize();
			}
		}
		
		protected void UpdateControlSize()
		{
			//MessageBox.Show ("DataResize");
			//hLabelVert.Width = picLeft.Width - 2;
			legend.HideEdit();
			legend.Width = pnlSE.Width;
			Console.WriteLine("DataFrame Resize Hlabel Size =" + this.hLabelHorz.Size + "VLabel Size = " +this.hLabelVert.Size) ; 
			
			VerticalLabel.RedrawLabel();
			HorizontalLabel.RedrawLabel();
			if (LabelsUpdated != null)
				LabelsUpdated();
		}

		private void hLabelVert_Align(int[] positions)
		{
			if (AlignVertical!=null)
				AlignVertical(positions);
		}

		private void hLabelHorz_Align(int[] positions)
		{
			if (AlignHorizontal!=null)
				AlignHorizontal(positions);
		}

		private void hLabelVert_Range(int lowRange, int highRange)
		{
			if (RangeVertical!=null)
				RangeVertical(lowRange, highRange);		
		}

		private void hLabelHorz_Range(int lowRange, int highRange)
		{
			if (RangeHorizontal!=null)
				RangeHorizontal(lowRange, highRange);
		}

		private void hLabelVert_Load(object sender, System.EventArgs e)
		{
		
		}

		private void hLabelVert_Resize(object sender, System.EventArgs e)
		{
			picLeft.Width = hLabelVert.Width + 1;
			legend.UpdateAxis();
		}

		private void hLabelVert_Selected(int lowPixel, int highPixel)
		{
			if (SelectedVertical!=null)
				SelectedVertical(lowPixel, highPixel);
		}

		private void hLabelHorz_Selected(int lowPixel, int highPixel)
		{
			if (SelectedHorizontal!=null)
				SelectedHorizontal(lowPixel, highPixel);
		}

		private void hLabelVert_ClientHeight(int height)
		{
			mClientSize.Height = height;
			mClientSize.Width = Math.Max(hLabelHorz.Width, mClientSize.Width);
			if (ClientSize!=null)
				ClientSize(mClientSize);
		}

		private void hLabelHorz_ClientWidth(int width)
		{
			mClientSize.Height = Math.Max(hLabelVert.Height, mClientSize.Height);
			mClientSize.Width = width;
			if (ClientSize!=null)
				ClientSize(mClientSize);
			//if (ClientLocation != null)
			//	ClientLocation(mClientLocation);
		}

		private void hLabelHorz_ScrollPosition(int pos)
		{
			mClientLocation.X = pos;
			//if (hLabelVert.Visible)
			{
				mClientLocation.X += hLabelVert.Width;
			}

			if (hLabelVert.ShowScroll)
			{
				
				mClientLocation.Y = hLabelVert.CurrentScrollPosition;
			//	if (pnlHeader.Visible == true)
				{
					mClientLocation.Y += pnlHeader.Height;
				}
			}
			//else if (pnlHeader.Visible)
				mClientLocation.Y = pnlHeader.Height;
			//else
			//	mClientLocation.Y = 0;

			if (ClientLocation!=null)
				ClientLocation(mClientLocation);
		
		}

		private void hLabelVert_ScrollPosition(int pos)
		{
			mClientLocation.Y = pos;
			//if (pnlHeader.Visible)
			{
				mClientLocation.Y+=pnlHeader.Height;
			}

			if (hLabelHorz.ShowScroll)
				mClientLocation.X = hLabelHorz.CurrentScrollPosition + hLabelVert.Width;
			else
				mClientLocation.X = hLabelVert.Width;

			if (ClientLocation!=null)
				ClientLocation(mClientLocation);
		}

		private void hLabelVert_LabelUpdated()
		{
			if (this.Height==0) return; //catches minimized case

			if (hLabelVert.UpdateComplete && hLabelHorz.UpdateComplete)
				if (LabelsUpdated!=null)
					LabelsUpdated();
		}

		private void hLabelHorz_LabelUpdated()
		{
			if (this.Height==0) return; //catches minimized case

			if (hLabelVert.UpdateComplete && hLabelHorz.UpdateComplete)
				if (LabelsUpdated!=null)
					LabelsUpdated();
		}

		private void hLabelVert_LeavesMoved(int[] tags)
		{
			if(LeavesMovedVertical!=null)
				LeavesMovedVertical(tags);
		}

		private void hLabelHorz_LeavesMoved(int[] tags)
		{
			if(LeavesMovedHorizontal!=null)
				LeavesMovedHorizontal(tags);
		}
		private void progBar_Click(object sender, System.EventArgs e)
		{
			if (mProgRunning)
				StopProgress();
			else
				StartProgress();
		}

		private void ShowLabelInfo(int pnl, PNNLControls.clsLabelAttributes lbl)
		{
			try
			{
				this.StatBar.Visible = true;
				this.StatBar.Panels[pnl].Text = lbl.text;
			}
			catch{}
		}

		private void hLabelVert_LabelMouseUp(PNNLControls.clsLabelAttributes lbl)
		{
			ShowLabelInfo(0, lbl);
		}

		private void hLabelHorz_LabelMouseUp(PNNLControls.clsLabelAttributes lbl)
		{
			ShowLabelInfo(1, lbl);
		}

		private void ctlDataFrame_Load(object sender, System.EventArgs e)
		{
		
		}

		private bool m_mouseDownSplitter = false;


		private void splitter_Move(object sender, EventArgs e)
		{
			/// need to handle override situation here where
			/// the user can move hte splitter by a mouse down but then
			///  we eneed to check to see if this is being triggered by a resize event
			///  or the user event- resize would come from thecontrol.
			if (this.m_mouseDownSplitter == true)
			{
				m_mouseDownSplitter = false;
				this.HorizontalLabel.RedrawLabel();
				this.VerticalLabel.RedrawLabel();
				if (this.LabelsUpdated != null)
					LabelsUpdated();
			}
		}

		private void splitter_MouseDown(object sender, MouseEventArgs e)
		{
			m_mouseDownSplitter = true;
		}

		private void splitter_MouseUp(object sender, MouseEventArgs e)
		{
			
		}
	}
}