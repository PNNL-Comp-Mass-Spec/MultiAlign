using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;


namespace PNNLControls.Drawing.Charting
{
    public class ctlHistogram: PNNLControls.ctlChartBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private float mfloat_binSize = 1.0F;

        public ctlHistogram()
		{			
			InitializeComponent();
			
            Title = "Histogram";
		}

		protected override void PaintSeries(Graphics g, Bitmap bitmap, clsSeries data)
		{
            ChartDataPoint [] points = data.PlotData;
            float baseY = GetScreenPixelY(0);
            foreach (ChartDataPoint point in points)
            {
                float x = GetScreenPixelX(point.x);
                float y = GetScreenPixelY(point.y);
                float width = GetScreenPixelX(point.x + mfloat_binSize);
                
                g.FillRectangle(new SolidBrush(Color.Red), x, y, width - x, baseY -  y); 
                g.DrawRectangle(new Pen(new SolidBrush(Color.Black)), x, y, width - x, baseY - y); 
            }
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
			components = new System.ComponentModel.Container();
		}
		#endregion


        /// <summary>
        /// Gets or sets the size of the bins 
        /// </summary>
        public float BinSize
        {
            get
            {
                return mfloat_binSize;
            }
            set
            {
                mfloat_binSize = value;
            }
        }

        internal override Size GetPreferredLegendSymbolSize(clsSeries series)
        {
            Pen pen = series.PlotParams.LinePen.Pen;
            int penWidth = (int)Math.Ceiling(pen.Width);
            pen.Dispose();
            int height = penWidth * 2;
            int width = Math.Max(10, penWidth * 7);
            
            height += series.PlotParams.Shape.Size * 2;
            width += series.PlotParams.Shape.Size * 2;
            
            return new Size(width, height);
        }

        internal override void PaintLegendSymbol(Graphics g, clsSeries series, Rectangle bounds)
        {
            System.Drawing.Drawing2D.GraphicsContainer container = g.BeginContainer();
            // center the image on the bounds
            int centerX = (bounds.Left + bounds.Right) / 2;
            int centerY = (bounds.Top + bounds.Bottom) / 2;
            int y = centerY - series.PlotParams.Shape.Size;
            int x = centerX - series.PlotParams.Shape.Size;
            g.IntersectClip(bounds);
            g.DrawLine(series.PlotParams.LinePen.Pen, bounds.Left, centerY, bounds.Right, centerY);
            g.TranslateTransform(centerX, centerY);
            series.PlotParams.DrawShape(g);
            g.EndContainer(container);
        }
    }
}

