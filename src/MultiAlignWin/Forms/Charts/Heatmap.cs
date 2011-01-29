using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using PNNLControls;

namespace MultiAlignWin.Drawing
{
    public partial class Heatmap : ctlHeatMapClient
    {
        private MultiAlignEngine.Alignment.clsAlignmentFunction mobjCurrentAlignmentFnc = null;
        private AxisRangeF m_verticalRange;
        private AxisRangeF m_horizontalRange;

        public Heatmap()
        {
            InitializeComponent();

            base.BitmapPainted += new BitmapPaintedDelegate(Heatmap_BitmapPainted);
            Legend              = new ctlHeatMapLegend();
            Legend.UseZScore    = false;
            ResizeRedraw        = true;
            Resize               += new EventHandler(Heatmap_Resize);
        }

        void Heatmap_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        public MultiAlignEngine.Alignment.clsAlignmentFunction AlignmentFunction
        {
            get
            {
                return mobjCurrentAlignmentFnc;
            }
            set
            {
                mobjCurrentAlignmentFnc = value;
                this.Refresh();
            }
        }

        public Image GetThumbnail(Size size)
        {
            return base.Thumbnail(size);
        }

        void Heatmap_BitmapPainted(System.Drawing.Graphics g)
        {
            try
            {
                System.Drawing.Pen fncPen = new Pen(System.Drawing.Color.Blue, 3);
                AxisRangeF hRange = m_horizontalRange;
                AxisRangeF vRange = m_verticalRange;

                if (mobjCurrentAlignmentFnc != null)
                {
                    int numPieces = mobjCurrentAlignmentFnc.marrNETFncTimeInput.Length;
                    if (numPieces < 1)
                        return;
                    float lastX = mobjCurrentAlignmentFnc.marrNETFncTimeInput[0];
                    float lastY = mobjCurrentAlignmentFnc.marrNETFncTimeOutput[0];
                    for (int pieceNum = 1; pieceNum < numPieces; pieceNum++)
                    {
                        float nextX = mobjCurrentAlignmentFnc.marrNETFncTimeInput[pieceNum];
                        float nextY = mobjCurrentAlignmentFnc.marrNETFncTimeOutput[pieceNum];

                        double startX = Width * (lastX - hRange.low) / (hRange.high - hRange.low);
                        double stopX  = Width * (nextX - hRange.low) / (hRange.high - hRange.low);

                        double startY = Height - Height * (lastY - vRange.high) / (vRange.low - vRange.high);
                        double stopY  = Height - Height * (nextY - vRange.high) / (vRange.low - vRange.high);

                        g.DrawLine(fncPen, (float)startX, (float)startY, (float)stopX, (float)stopY);

                        lastX = nextX;
                        lastY = nextY;
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message + ex.StackTrace);
            }   
        }

        private delegate void dlgSetData(   float[,]   data,
                                            AxisRangeF horizRange,
                                            AxisRangeF vertRange); 

        public void SetData(float [,]  data, 
                            AxisRangeF horizRange, 
			                AxisRangeF vertRange)
		{
            if (this.InvokeRequired)
            {
                this.Invoke(new dlgSetData(SetData), new object[] { data, horizRange, vertRange });
            }
            else
            {
                int numRows       = data.GetUpperBound(0) - data.GetLowerBound(0);
                int numColumns    = data.GetUpperBound(1) - data.GetLowerBound(1);

                base.Data         = data;
                m_verticalRange = new AxisRangeF(0, numRows - 1); //vertRange;
                m_horizontalRange = new AxisRangeF(0, numColumns - 1); // horizRange;
                OnRefresh();              
            }
		}
    }

    
	public class AxisRangeF
	{
		public  double low = double.MinValue;
		public  double high =double.MinValue;

		public AxisRangeF (double lower, double upper)
		{
			low = lower;
			high = upper;
		}
	}
}
