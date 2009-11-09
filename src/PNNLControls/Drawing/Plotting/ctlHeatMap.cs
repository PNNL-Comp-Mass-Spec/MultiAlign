using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PNNLControls
{
	public class ctlHeatMap : PNNLControls.ctlDataFrame
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Splitter splitterVertical;
		private System.Windows.Forms.Splitter splitterHorizontal;
		protected PNNLControls.ctlHeatMapClient hMapClient;

		
		public event ctlHeatMapClient.BitmapPaintedDelegate BitmapPainted = null;

		public bool DrawDemaractionLines
		{
			get
			{
				return hMapClient.DrawDemarcationLines ; 
			}
			set
			{
				hMapClient.DrawDemarcationLines = value ; 
			}
		}

		public float[,] Data 
		{
			get{return hMapClient.Data;}
			set{
				hMapClient.Data = value;				
			}
		}

		//someone has been nice enough to synch the labels to the data, now we just 
		//have to organize the heatmap control.
		public void Init(float[,] data, clsLabelAttributes vert, clsLabelAttributes horz)
		{
			this.UpdateComplete = false;
			HorizontalLabel.UpdateComplete = false;
			VerticalLabel.UpdateComplete = false;

			Data = data;

			//setting root initializes label
			HorizontalLabel.Root = horz;
			//HorizontalLabel.Init();
			hMapClient.ColumnMap = HorizontalLabel.DataTags;
			HorizontalLabel.LoadLabels(0, data.GetUpperBound(1));

			VerticalLabel.Root = vert;
			//VerticalLabel.Init();
			hMapClient.RowMap = VerticalLabel.DataTags;
			VerticalLabel.LoadLabels(0, data.GetUpperBound(0));

			this.UpdateComplete = true;
		}

		public void Thumb()
		{
			picLeft.Image = hMapClient.Thumbnail(picLeft.Size);
		}
        public Image GetThumbnail(Size size)
        {
            return hMapClient.Thumbnail(size);
        }

		public ctlHeatMap()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			this.splitterHorizontal.BringToFront();
			this.splitterVertical.BringToFront();

			// TODO: Add any initialization after the InitializeComponent call

		    hMapClient.Legend = base.Legend;
			base.Legend.LegendChanged += new ctlHeatMapLegend.LegendChangedDelegate(this.LegendChanged);
			this.ResizeRedraw = true ; 
			this.Resize +=new EventHandler(ctlHeatMap_Resize);
	}

	private void LegendChanged()
	{
		this.Thumb();
	}

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
			this.splitterVertical = new System.Windows.Forms.Splitter();
			this.splitterHorizontal = new System.Windows.Forms.Splitter();
			this.hMapClient = new PNNLControls.ctlHeatMapClient();
			this.panel1.SuspendLayout();
			this.pnlSE.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Name = "panel1";
			// 
			// hLabelHorz
			// 
			this.hLabelHorz.Location = new System.Drawing.Point(137, 0);
			this.hLabelHorz.Name = "hLabelHorz";
			this.hLabelHorz.Size = new System.Drawing.Size(455, 120);
			this.hLabelHorz.UpdateComplete = true;
			// 
			// hLabelVert
			// 
			this.hLabelVert.Location = new System.Drawing.Point(0, 24);
			this.hLabelVert.Name = "hLabelVert";
			this.hLabelVert.Size = new System.Drawing.Size(136, 456);
			// 
			// legend
			// 
			this.legend.Location = new System.Drawing.Point(592, 24);
			this.legend.Name = "legend";
			this.legend.Size = new System.Drawing.Size(40, 456);
			// 
			// pnlSE
			// 
			this.pnlSE.Name = "pnlSE";
			// 
			// pnlHeader
			// 
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(632, 24);
			// 
			// picRight
			// 
			this.picRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.picRight.Name = "picRight";
			// 
			// picLeft
			// 
			this.picLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.picLeft.Name = "picLeft";
			this.picLeft.Size = new System.Drawing.Size(137, 120);
			// 
			// splitterVertical
			// 
			this.splitterVertical.BackColor = System.Drawing.SystemColors.Control;
			this.splitterVertical.Location = new System.Drawing.Point(136, 24);
			this.splitterVertical.Name = "splitterVertical";
			this.splitterVertical.Size = new System.Drawing.Size(1, 456);
			this.splitterVertical.TabIndex = 8;
			this.splitterVertical.TabStop = false;
			// 
			// splitterHorizontal
			// 
			this.splitterHorizontal.BackColor = System.Drawing.SystemColors.Control;
			this.splitterHorizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitterHorizontal.Location = new System.Drawing.Point(137, 479);
			this.splitterHorizontal.Name = "splitterHorizontal";
			this.splitterHorizontal.Size = new System.Drawing.Size(455, 1);
			this.splitterHorizontal.TabIndex = 9;
			this.splitterHorizontal.TabStop = false;
			// 
			// hMapClient
			// 
			this.hMapClient.AlignHorizontal = null;
			this.hMapClient.AlignVertical = null;
			this.hMapClient.Data = null;
			this.hMapClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.hMapClient.DrawDemarcationLines = false;
			this.hMapClient.Legend = null;
			this.hMapClient.Location = new System.Drawing.Point(137, 24);
			this.hMapClient.Name = "hMapClient";
			this.hMapClient.Size = new System.Drawing.Size(455, 455);
			this.hMapClient.TabIndex = 10;
			this.hMapClient.BitmapPainted += new PNNLControls.ctlHeatMapClient.BitmapPaintedDelegate(this.hMapClient_BitmapPainted);
			this.hMapClient.UnZoom += new PNNLControls.ctlHeatMapClient.UnZoomDelegate(this.hMapClient_UnZoom);
			this.hMapClient.Zoom += new PNNLControls.ctlHeatMapClient.ZoomDelegate(this.hMapClient_Zoom);
			// 
			// ctlHeatMap
			// 
			this.Controls.Add(this.hMapClient);
			this.Controls.Add(this.splitterHorizontal);
			this.Controls.Add(this.splitterVertical);
			this.Name = "ctlHeatMap";
			this.LabelsUpdated += new PNNLControls.ctlHierarchalLabel.LabelUpdateDelegate(this.ctlHeatMap_LabelsUpdated);
			this.AlignVertical += new PNNLControls.ctlHierarchalLabel.AlignDelegate(this.HeatMap_AlignVertical);
			this.SelectedHorizontal += new PNNLControls.ctlHierarchalLabel.SelectedDelegate(this.ctlHeatMap_SelectedHorizontal);
			this.ClientSize += new PNNLControls.ctlDataFrame.ClientSizeDelegate(this.ctlHeatMap_ClientSize);
			this.LeavesMovedHorizontal += new PNNLControls.ctlHierarchalLabel.LeavesMovedDelegate(this.ctlHeatMap_LeavesMovedHorizontal);
			this.AlignHorizontal += new PNNLControls.ctlHierarchalLabel.AlignDelegate(this.HeatMap_AlignHorizontal);
			this.ClientLocation += new PNNLControls.ctlDataFrame.ClientLocationDelegate(this.ctlHeatMap_ClientLocation);
			this.RangeVertical += new PNNLControls.ctlHierarchalLabel.RangeDelegate(this.HeatMap_RangeVertical);
			this.LeavesMovedVertical += new PNNLControls.ctlHierarchalLabel.LeavesMovedDelegate(this.ctlHeatMap_LeavesMovedVertical);
			this.RangeHorizontal += new PNNLControls.ctlHierarchalLabel.RangeDelegate(this.HeatMap_RangeHorizontal);
			this.SelectedVertical += new PNNLControls.ctlHierarchalLabel.SelectedDelegate(this.ctlHeatMap_SelectedVertical);
			this.Controls.SetChildIndex(this.pnlHeader, 0);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.legend, 0);
			this.Controls.SetChildIndex(this.hLabelVert, 0);
			this.Controls.SetChildIndex(this.splitterVertical, 0);
			this.Controls.SetChildIndex(this.splitterHorizontal, 0);
			this.Controls.SetChildIndex(this.hMapClient, 0);
			this.panel1.ResumeLayout(false);
			this.pnlSE.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void HeatMap_AlignHorizontal(int[] positions)
		{
			this.hMapClient.AlignHorizontal=positions;
		}

		private void HeatMap_AlignVertical(int[] positions)
		{
			this.hMapClient.AlignVertical=positions;
		}

		private void HeatMap_RangeHorizontal(int lowRange, int highRange)
		{
			//if the range hasn't changed, bail.
			if (hMapClient.DisplayedData.StartColumn==lowRange && 
				hMapClient.DisplayedData.EndColumn==highRange) return;

			hMapClient.DisplayedData = new Range(hMapClient.DisplayedData.StartRow, lowRange,
												 hMapClient.DisplayedData.EndRow, highRange);
		}

		private void HeatMap_RangeVertical(int lowRange, int highRange)
		{

			//if the range hasn't changed, bail.
			if (hMapClient.DisplayedData.StartRow==lowRange && 
				hMapClient.DisplayedData.EndRow==highRange) return;

			hMapClient.DisplayedData = new Range(lowRange, hMapClient.DisplayedData.StartColumn,
				highRange, hMapClient.DisplayedData.EndColumn);
		}

		private void HeatMap_MovedRangeHorizontal(int lowRange, int highRange, int dest)
		{
			//hMapClient.MovedRangeHorizontal(lowRange, highRange, dest);
		}

		private void HeatMap_MovedRangeVertical(int lowRange, int highRange, int dest)
		{
			//hMapClient.MovedRangeVertical(lowRange, highRange, dest);
		}

		private void ctlHeatMap_SelectedHorizontal(int lowPixel, int highPixel)
		{
			hMapClient.SelectedHorizontal(lowPixel, highPixel);
			//picLeft.Image = hMapClient.SelectedThumbnail(picLeft.Size);
		}

		private void ctlHeatMap_SelectedVertical(int lowPixel, int highPixel)
		{
			hMapClient.SelectedVertical(lowPixel, highPixel);
			//picLeft.Image = hMapClient.SelectedThumbnail(picLeft.Size);
		}

		private void ctlHeatMap_ClientLocation(System.Drawing.Point location)
		{
			hMapClient.Dock = DockStyle.None;
			hMapClient.SendToBack();
			hMapClient.Location = location;
		}

		private void ctlHeatMap_ClientSize(System.Drawing.Size size)
		{
			hMapClient.Size = size;
		}

		//int updateCount = 0;
		private void ctlHeatMap_LabelsUpdated()
		{
			//updateCount++;
			hMapClient.OnRefresh();
			picLeft.Image = hMapClient.SelectedThumbnail(picLeft.Size);
			//this.Thumb();
		}

		private void ctlHeatMap_LeavesMovedHorizontal(int[] tags)
		{
			hMapClient.ColumnMap = tags;
		}

		private void ctlHeatMap_LeavesMovedVertical(int[] tags)
		{
			hMapClient.RowMap = tags;
		}

		private void hMapClient_BitmapPainted(System.Drawing.Graphics g)
		{
			if (BitmapPainted!=null)
				BitmapPainted(g);		
		}

		private void hMapClient_UnZoom()
		{
			this.VerticalLabel.AxisZoomOut();
		}

		private void hMapClient_Zoom(System.Drawing.Point start, System.Drawing.Point stop)
		{
			this.VerticalLabel.AxisZoomIn(start, stop);
		}

		private delegate void dlgSetData(float [,] data, ctlHierarchalLabel.AxisRangeF horizRange, 
			ctlHierarchalLabel.AxisRangeF vertRange) ; 

		public void SetData(float [,] data, ctlHierarchalLabel.AxisRangeF horizRange, 
			ctlHierarchalLabel.AxisRangeF vertRange)
		{
			if (this.InvokeRequired)
				this.Invoke(new dlgSetData(this.SetData), new object[]{data, horizRange, vertRange}) ; 
			else
			{
				int numRows = data.GetUpperBound(0) - data.GetLowerBound(0) ; 
				int numColumns = data.GetUpperBound(1) - data.GetLowerBound(1) ; 

				this.Data = data ; 
				this.VerticalLabel.AxisRange = vertRange ;
				this.VerticalLabel.LoadLabels(0,numRows-1, (double)vertRange.high, (double)vertRange.low);
		
				this.HorizontalLabel.AxisRange =  horizRange ;
				this.HorizontalLabel.LoadLabels(0,numColumns-1, (double) horizRange.low, (double) horizRange.high);
				this.Thumb();
			}
		}

		private void ctlHeatMap_Resize(object sender, EventArgs e)
		{
			hMapClient.Invalidate() ;
		}
	}
}

