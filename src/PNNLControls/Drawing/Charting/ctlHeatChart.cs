using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlHeatChart.
	/// </summary>
	public class ctlHeatChart : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Panel panelTitle;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelLeft;
		private System.Windows.Forms.Panel panelPad;
		private System.Windows.Forms.Panel panelRight;
		private PNNLControls.ctlHeatMapClient ctlHeatMapScores;
		private System.Windows.Forms.Panel panel1;
		private PNNLControls.ctlSingleAxis ctlSingleAxisX;
		private PNNLControls.ctlSingleAxis ctlSingleAxisY;
		private PNNLControls.ctlHeatMapLegend ctlHeatMapLegend1;
		private System.Windows.Forms.Label labelTitle;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlHeatChart()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			ctlHeatMapLegend1.CreateHeatLegend();
			ctlHeatMapScores.Legend = ctlHeatMapLegend1 ; 
			this.Resize +=new EventHandler(ctlHeatChart_Resize);
		}

		public void SetData(string title, float[,] data, float minX, float maxX, 
			float minY, float maxY)
		{
			// the data that comes in, is usually 0th index in the data is the first point.
			// but for our purposes, the 0th index is the highest one. lets transform the 
			// data first.
			int numRows = data.GetUpperBound(0) - data.GetLowerBound(0) ; 
			int numColumns = data.GetUpperBound(1) - data.GetLowerBound(1) ; 
			float [,] dataTransformed = new float[numRows, numColumns] ; 
			for (int rowNum = 0 ; rowNum < numRows ; rowNum++)
			{
				int transformedRowNum = numRows-rowNum-1 ; 
				for (int colNum = 0 ; colNum < numColumns ; colNum++)
				{
					dataTransformed[transformedRowNum, colNum] = data[rowNum, colNum] ; 
				}
			}
			ctlHeatMapScores.Data = dataTransformed ; 
			ctlHeatMapScores.DisplayedData = new Range(data.GetLowerBound(0), data.GetLowerBound(1), 
				data.GetUpperBound(0)-1,data.GetUpperBound(1)-1);
			ctlSingleAxisX.UpdateAxis(minX, maxX) ; 
			ctlSingleAxisY.UpdateAxis(minY, maxY) ; 
			ctlHeatMapScores.OnRefresh();
			labelTitle.Text = title ; 
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ctlHeatChart));
			this.panelTitle = new System.Windows.Forms.Panel();
			this.labelTitle = new System.Windows.Forms.Label();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.ctlSingleAxisX = new PNNLControls.ctlSingleAxis();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panelPad = new System.Windows.Forms.Panel();
			this.panelLeft = new System.Windows.Forms.Panel();
			this.ctlSingleAxisY = new PNNLControls.ctlSingleAxis();
			this.panelRight = new System.Windows.Forms.Panel();
			this.ctlHeatMapLegend1 = new PNNLControls.ctlHeatMapLegend();
			this.ctlHeatMapScores = new PNNLControls.ctlHeatMapClient();
			this.panelTitle.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.panelLeft.SuspendLayout();
			this.panelRight.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTitle
			// 
			this.panelTitle.Controls.Add(this.labelTitle);
			this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTitle.Location = new System.Drawing.Point(0, 0);
			this.panelTitle.Name = "panelTitle";
			this.panelTitle.Size = new System.Drawing.Size(560, 32);
			this.panelTitle.TabIndex = 0;
			// 
			// labelTitle
			// 
			this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelTitle.Location = new System.Drawing.Point(0, 0);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(560, 32);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "Title";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.ctlSingleAxisX);
			this.panelBottom.Controls.Add(this.panel1);
			this.panelBottom.Controls.Add(this.panelPad);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 416);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(560, 56);
			this.panelBottom.TabIndex = 1;
			// 
			// ctlSingleAxisX
			// 
			this.ctlSingleAxisX.Alignment = null;
			this.ctlSingleAxisX.BackColor = System.Drawing.SystemColors.Control;
			this.ctlSingleAxisX.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlSingleAxisX.Location = new System.Drawing.Point(56, 0);
			this.ctlSingleAxisX.Name = "ctlSingleAxisX";
			this.ctlSingleAxisX.Size = new System.Drawing.Size(456, 56);
			this.ctlSingleAxisX.TabIndex = 2;
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(512, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(48, 56);
			this.panel1.TabIndex = 1;
			// 
			// panelPad
			// 
			this.panelPad.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelPad.Location = new System.Drawing.Point(0, 0);
			this.panelPad.Name = "panelPad";
			this.panelPad.Size = new System.Drawing.Size(56, 56);
			this.panelPad.TabIndex = 0;
			// 
			// panelLeft
			// 
			this.panelLeft.Controls.Add(this.ctlSingleAxisY);
			this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelLeft.Location = new System.Drawing.Point(0, 32);
			this.panelLeft.Name = "panelLeft";
			this.panelLeft.Size = new System.Drawing.Size(56, 384);
			this.panelLeft.TabIndex = 2;
			// 
			// ctlSingleAxisY
			// 
			this.ctlSingleAxisY.Alignment = null;
			this.ctlSingleAxisY.BackColor = System.Drawing.SystemColors.Control;
			this.ctlSingleAxisY.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlSingleAxisY.Location = new System.Drawing.Point(0, 0);
			this.ctlSingleAxisY.Name = "ctlSingleAxisY";
			this.ctlSingleAxisY.Size = new System.Drawing.Size(56, 384);
			this.ctlSingleAxisY.TabIndex = 0;
			// 
			// panelRight
			// 
			this.panelRight.Controls.Add(this.ctlHeatMapLegend1);
			this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelRight.Location = new System.Drawing.Point(512, 32);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(48, 384);
			this.panelRight.TabIndex = 3;
			// 
			// ctlHeatMapLegend1
			// 
			this.ctlHeatMapLegend1.ApplyMode = PNNLControls.ctlHeatMapLegend.ApplyLegendMode.log;
			this.ctlHeatMapLegend1.BackColor = System.Drawing.SystemColors.Control;
			this.ctlHeatMapLegend1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlHeatMapLegend1.LegendBitmap = ((System.Drawing.Bitmap)(resources.GetObject("ctlHeatMapLegend1.LegendBitmap")));
			this.ctlHeatMapLegend1.Location = new System.Drawing.Point(0, 0);
			this.ctlHeatMapLegend1.LowerRange = 0F;
			this.ctlHeatMapLegend1.MaxColor = System.Drawing.Color.White;
			this.ctlHeatMapLegend1.MinColor = System.Drawing.Color.Black;
			this.ctlHeatMapLegend1.Name = "ctlHeatMapLegend1";
			this.ctlHeatMapLegend1.NaNColor = System.Drawing.Color.Gray;
			this.ctlHeatMapLegend1.OverColor = System.Drawing.SystemColors.Control;
			this.ctlHeatMapLegend1.Size = new System.Drawing.Size(48, 384);
			this.ctlHeatMapLegend1.TabIndex = 0;
			this.ctlHeatMapLegend1.UnderColor = System.Drawing.SystemColors.Control;
			this.ctlHeatMapLegend1.UpperRange = 0F;
			// 
			// ctlHeatMapScores
			// 
			this.ctlHeatMapScores.AlignHorizontal = null;
			this.ctlHeatMapScores.AlignVertical = null;
			this.ctlHeatMapScores.ColumnMap = null;
			this.ctlHeatMapScores.Data = null;
			this.ctlHeatMapScores.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlHeatMapScores.DrawDemarcationLines = true;
			this.ctlHeatMapScores.Legend = null;
			this.ctlHeatMapScores.Location = new System.Drawing.Point(56, 32);
			this.ctlHeatMapScores.Name = "ctlHeatMapScores";
			this.ctlHeatMapScores.RowMap = null;
			this.ctlHeatMapScores.Size = new System.Drawing.Size(456, 384);
			this.ctlHeatMapScores.TabIndex = 4;
			// 
			// ctlHeatChart
			// 
			this.Controls.Add(this.ctlHeatMapScores);
			this.Controls.Add(this.panelRight);
			this.Controls.Add(this.panelLeft);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTitle);
			this.Name = "ctlHeatChart";
			this.Size = new System.Drawing.Size(560, 472);
			this.panelTitle.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.panelLeft.ResumeLayout(false);
			this.panelRight.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ctlHeatChart_Resize(object sender, EventArgs e)
		{
			this.ctlHeatMapScores.OnRefresh() ; 
			this.ctlSingleAxisX.Refresh() ; 
			this.ctlSingleAxisY.Refresh() ; 
		}
	}
}
