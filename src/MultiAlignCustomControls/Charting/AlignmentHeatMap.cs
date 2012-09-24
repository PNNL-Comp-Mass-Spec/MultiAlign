using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

namespace MultiAlignCustomControls.Charting 
{
	public class ctlAlignmentHeatMap : PNNLControls.ctlHeatMap
	{
		private System.ComponentModel.IContainer components = null;
		private MultiAlignEngine.Alignment.clsAlignmentFunction mobjCurrentAlignmentFnc = null ; 

		public ctlAlignmentHeatMap()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			base.BitmapPainted +=new PNNLControls.ctlHeatMapClient.BitmapPaintedDelegate(ctlAlignmentHeatMap_BitmapPainted);
		}

		public MultiAlignEngine.Alignment.clsAlignmentFunction AlignmentFunction
		{
			get
			{
				return mobjCurrentAlignmentFnc ; 
			}
			set
			{
				mobjCurrentAlignmentFnc = value ; 
				this.Refresh() ;                 
			}
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
			components = new System.ComponentModel.Container();
		}
		#endregion

		private void ctlAlignmentHeatMap_BitmapPainted(Graphics g)
		{
			try
			{
				System.Drawing.Pen fncPen = new Pen(System.Drawing.Color.Blue, 3) ; 
				PNNLControls.ctlHierarchalLabel.AxisRangeF hRange = this.HorizontalLabel.AxisRange ; 
				PNNLControls.ctlHierarchalLabel.AxisRangeF vRange = this.VerticalLabel.AxisRange ; 
				if (mobjCurrentAlignmentFnc != null)
				{
					int numPieces = mobjCurrentAlignmentFnc.marrNETFncTimeInput.Length ; 
					if (numPieces < 1)
						return ; 
					float lastX = mobjCurrentAlignmentFnc.marrNETFncTimeInput[0] ; 
					float lastY = mobjCurrentAlignmentFnc.marrNETFncTimeOutput[0] ; 
					for (int pieceNum = 1 ; pieceNum < numPieces ; pieceNum++)
					{
						float nextX = mobjCurrentAlignmentFnc.marrNETFncTimeInput[pieceNum] ; 
						float nextY = mobjCurrentAlignmentFnc.marrNETFncTimeOutput[pieceNum] ; 

						double startX = this.hMapClient.Width *(lastX-hRange.low)/(hRange.high - hRange.low) ; 
						double stopX = this.hMapClient.Width * (nextX-hRange.low)/(hRange.high - hRange.low) ; 

						double startY = this.hMapClient.Height - this.hMapClient.Height * (lastY-vRange.high)/(vRange.low - vRange.high) ; 
						double stopY = this.hMapClient.Height - this.hMapClient.Height * (nextY-vRange.high)/(vRange.low - vRange.high) ; 

						g.DrawLine(fncPen, (float)startX,(float)startY, (float)stopX, (float)stopY) ; 

						lastX = nextX ; 
						lastY = nextY ; 
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace) ; 
			}
		}        
	}
}

