using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;


namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlSingleAxis.
	/// </summary>
	public class ctlSingleAxis : System.Windows.Forms.Panel
	{

		/// <summary>
		/// 
		/// </summary>
		public delegate void ZoomDelegate (Point start, Point stop);
		public event ZoomDelegate Zoom = null;

		/// <summary>
		/// 
		/// </summary>
		public delegate void UnZoomDelegate ();
		public event UnZoomDelegate UnZoom = null;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private clsPlotSingleAxis mPlotter = new clsPlotSingleAxis();
		private Rectangle mZoomRectangle = new Rectangle(0,0,1,1);
		private Range mSelected = new Range(int.MinValue,int.MinValue,int.MinValue,int.MinValue);
        private int mMinSelectionRange = 10; //10 pixels
        private int[] mAlignment = null;

		private Color mSelectedColor = Color.Pink;

		/// <summary>
		/// flag which indicates whether we are in the process of selecting a zoom area
		/// </summary>
		private bool mZooming = false;


        public ctlSingleAxis()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            Color temp     = this.BackColor;
            this.BackColor = SystemColors.Control;
            this.BackColor = temp;
            mPlotter.UnitPlotter.SetRange(0f, 100f);
        }


        public Color SelectedColor
        {
            get { return this.mSelectedColor; }
            set { this.mSelectedColor = value; }
        }
		public clsPlotSingleAxis Plotter
		{
			get{return mPlotter;}
			set{mPlotter = value;}
		}	
		public int[] Alignment 
		{
			get{return this.mAlignment;}
			set{this.mAlignment = value;}
		}

		public void DrawDemarcations(Graphics g)
		{
			if (mAlignment==null) return;

			int length = mAlignment.Length;
			Pen greyPen = new Pen(Color.DarkGray, 1);
			Pen whitePen = new Pen(Color.White, 1);
			Point p1, p2, p3, p4;

			if (mAlignment!=null) 
			{
				for (int j=1; j<length-1; j++)
				{
					if (Plotter.IsVertical)
					{
						p1 = new Point(0, mAlignment[j]-1);
						p2 = new Point(this.Width, mAlignment[j]-1);
						p3 = new Point(0, mAlignment[j]);
						p4 = new Point(this.Width, mAlignment[j]);
					}
					else
					{
						p1 = new Point(mAlignment[j]-1, 0);
						p2 = new Point(mAlignment[j]-1, this.Height);
						p3 = new Point(mAlignment[j], 0);
						p4 = new Point(mAlignment[j], this.Height);
					}
					g.DrawLine(whitePen, p1, p2);
					g.DrawLine(greyPen, p3, p4);
				}
			}

			g.Dispose();

			return;
		}

		private void ShowSelection(Graphics g)
		{
			if (mSelected.StartRow==int.MinValue && mSelected.StartColumn==int.MinValue) return;

			try
			{
				SolidBrush b = new SolidBrush(mSelectedColor);

				Rectangle r = new Rectangle();
				r.X = Math.Max(mSelected.StartColumn, 0);
				r.Y = Math.Max(mSelected.StartRow, 0);
				if (mSelected.EndColumn==int.MinValue)
				{
					r.Width  = this.Width; 
				}
				else
				{
					r.Width  = mSelected.NumColumns; 
				}

				if (mSelected.EndRow==int.MinValue)
				{
					r.Height  = this.Height; 
				}
				else
				{
					r.Height  = mSelected.NumRows; 
				}
				
				g.FillRectangle (b, r);

			}
			catch{}
		}

		public void SelectedVertical(int lowPixel, int highPixel)
		{
			mSelected.StartRow = lowPixel;
			mSelected.EndRow = highPixel;
		}

		public void SelectedHorizontal(int lowPixel, int highPixel)
		{
			mSelected.StartColumn = lowPixel;
			mSelected.EndColumn = highPixel;
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
			// 
			// ctlSingleAxis
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Axis_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ctlSingleAxis_Paint);
			this.MouseHover += new System.EventHandler(this.ctlSingleAxis_MouseHover);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Axis_MouseMove);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Axis_MouseWheel);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Axis_MouseDown);

		}
		#endregion

		public void UpdateAxis()
		{
			try
			{
				mPlotter.Bounds = this.ClientRectangle;

				Graphics g = this.CreateGraphics();

				//clear old axis
				g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
				ShowSelection(g);

				mPlotter.Layout(g);
				mPlotter.Draw(g, Color.Black);

				
				DrawDemarcations(g);
				
				g.Dispose();
			}
			catch (Exception ex)
            {
            }
            
		}

		private void ctlSingleAxis_MouseHover(object sender, System.EventArgs e)
		{
		    this.Focus();
		}

		/// <summary>
		/// mousedown event for numeric axis, starts zoom
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Axis_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if (e.Button == MouseButtons.Right)
				{
					if (UnZoom!=null)
						UnZoom();
				}
				else
				{
					mZooming = true;
					mZoomRectangle = new Rectangle(((Control)sender).PointToScreen(new Point(e.X, e.Y)), new Size(1,1));
					ControlPaint.DrawReversibleFrame(mZoomRectangle, this.BackColor, FrameStyle.Dashed);
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		private void Axis_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Delta < 0)
			{
				if (Zoom!=null)
				{
					int delta = mMinSelectionRange/2;

					Point start =new Point (e.X-delta, e.Y-delta);
					Point stop = new Point(e.X+delta, 	e.Y+delta);
					Zoom(start, stop);
				}
			}
			else
			{
				if (UnZoom!=null)
					UnZoom();
			}
		}

		/// <summary>
		/// mousemove event for numeric axis, shows zoom rectangle
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Axis_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				ShowZoom((Control)sender, e.X, e.Y);
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// mouseup event for numeric axis, stops zoom, applies zoom
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Axis_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if (!mZooming)  return;
				if (e.Button == MouseButtons.Right) return;

				mZooming = false;
				ShowZoom((Control)sender, e.X, e.Y);
				ControlPaint.DrawReversibleFrame(mZoomRectangle, ((Control)sender).BackColor, FrameStyle.Dashed);
				if (Zoom!=null)
				{
					Point start = this.PointToClient(new Point (mZoomRectangle.X, mZoomRectangle.Y));
					Point stop = this.PointToClient(new Point(mZoomRectangle.X + mZoomRectangle.Width, 
													mZoomRectangle.Y + mZoomRectangle.Height));

					if (Math.Abs(start.X-stop.X) < mMinSelectionRange) return;
					if (Math.Abs(start.Y-stop.Y) < mMinSelectionRange) return;

					Zoom(start, stop);
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		private void ctlSingleAxis_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (this.Height>this.Width)
				mPlotter.IsVertical = true;
			else
				mPlotter.IsVertical = false;

			UpdateAxis();
		}

		/// <summary>
		/// shows zoom rectangle
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void ShowZoom (Control sender, int x, int y)
		{
			try
			{
				if (mZooming)
				{
					//hide old rectangle
					ControlPaint.DrawReversibleFrame(mZoomRectangle, sender.BackColor, FrameStyle.Dashed);
					Point newPoint = sender.PointToScreen(new Point(x, y));
					mZoomRectangle.Width = newPoint.X - mZoomRectangle.X;
					mZoomRectangle.Height = newPoint.Y - mZoomRectangle.Y;
					ControlPaint.DrawReversibleFrame(mZoomRectangle, sender.BackColor, FrameStyle.Dashed);
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		public void UpdateAxis(float min, float max)
		{

			try
			{
				if (min==max)
					return;

				if (min < max)
					mPlotter.UnitPlotter.SetRange(min, max);
				else
					return;

				UpdateAxis();

			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}
	}
}
