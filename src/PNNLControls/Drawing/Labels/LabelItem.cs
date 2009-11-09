using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for LabelItem.
	/// </summary>
	public class LabelItem : System.Windows.Forms.Label 
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LabelItem()
		{
			// Activates double buffering
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
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
			// LabelItem
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Size = new System.Drawing.Size(224, 48);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.labelItem_Paint);

		}
		#endregion

		#region Fields

		/// <summary>
		/// describes how the label is drawn
		/// </summary>
		private StringFormat mFormat = new StringFormat();

		/// <summary>
		/// pointer to forebrush that lives as long as the label item
		/// </summary>
		SolidBrush mForeBrush = new SolidBrush(Color.Black);

		/// <summary>
		/// pointer to backbrush that lives as long as the label item
		/// </summary>
		SolidBrush mBackBrush = new SolidBrush(Color.White);

		/// <summary>
		/// if flag is true, border is drawn
		/// </summary>
		bool mDrawBorder = false;

		/// <summary>
		/// if flag is true, label is drawn horizontally
		/// </summary>
		bool mVertical = false;

		/// <summary>
		/// minimum and maximum font sizes for label
		/// </summary>
		float mMinFontSize = 4f;
		float mMaxFontSize = 14f;

		/// <summary>
		/// links this label to a label hierarchy item
		/// </summary>
		LabelAttributes mLabelAttributes = null; 

		#endregion

		#region Properties

		/// <summary>
		/// layout is vertical if true, horizontal if false
		/// </summary>
		public bool Vertical 
		{
			get {return this.mVertical;}
			set	{this.mVertical = value;}
		}

		/// <summary>
		/// border is drawn if true
		/// </summary>
		public bool DrawBorder 
		{
			get {return this.mDrawBorder;}
			set	{this.mDrawBorder = value;}
		}		

		/// <summary>
		/// a pointer to a labelAttributes class is maintained.  This allows us to maintain the
		/// relationship of a label to a larger, undrawn set of labels
		/// </summary>
		public LabelAttributes LabelAttributes 
		{
			get {return this.mLabelAttributes;}
			set	{this.mLabelAttributes = value;}
		}		

		/// <summary>
		/// format of the label text
		/// </summary>
		public StringFormat Format 
		{
			get {return this.mFormat;}
			set	{this.mFormat = value;}
		}		

		/// <summary>
		/// minimum font size in Points supported.  Smaller fonts not drawn
		/// </summary>
		public float MinFontSize 
		{
			get {return this.mMinFontSize;}
			set	{this.mMinFontSize = value;}
		}		

		/// <summary>
		/// maximum font size in Points supported.  Larger fonts not drawn
		/// </summary>
		public float MaxFontSize 
		{
			get {return this.mMaxFontSize;}
			set	{this.mMaxFontSize = value;}
		}		

		#endregion

		#region Rendering Methods

		/// <summary>
		/// returns the text that will fit in the label given the contstraints of label and font size
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		private string GetDisplayText(Graphics g)
		{
			float dpiY = (float) g.DpiY;
			float height = 1f;
			float width = 1f;
			string displayText = this.Text;

			if (mVertical)
			{
				height = this.Width;
				width = this.Height;
			}
			else
			{
				height = this.Height;
				width = this.Width;
			}

			if (mDrawBorder)
			{
				height-=2;
				width-=2;
			}

			// 72 points per inch
			// calculate the height of the label in points
			float points = height * 72.0f / dpiY;
			
			if (points > mMaxFontSize)
			{
				points = mMaxFontSize;
			}

			//can't display label, height too small
			if (points < mMinFontSize)
			{
				return("");
			}

			this.Font = new Font(this.Font.Name, (float) points);

			SizeF sizeF = g.MeasureString(displayText, this.Font);

			if (sizeF.Width <= width)
			{
				return (displayText);
			}

			//trim the string 
			while (sizeF.Width > width && displayText.Length > 0)
			{
				displayText = displayText.Substring(0, displayText.Length-1);
				sizeF = g.MeasureString(displayText + ".", this.Font);
			}

			if (displayText.Length > 0)
				return (displayText + "..");
		    else
				return ("");
		}

		/// <summary>
		/// autosizes the font for the label text
		/// </summary>
		/// <param name="g"></param>
		private void AutoSize (Graphics g)
		{
			double dpiY = (double) g.DpiY;
			double height = 1.0;

			if (mVertical)
				height = this.ClientSize.Width;
			else
				height = this.ClientSize.Height;

			double points = height * 72.0 / dpiY;

			if (points < MinFontSize)
			{
				points = MinFontSize;
			}


			this.Font = new Font(this.Font.Name, (float) points);
		}

		/// <summary>
		/// draws the label, either horizontally or vertically
		/// </summary>
		/// <param name="g"></param>
		public void Draw(Graphics g)
		{
			string displayText = GetDisplayText(g);

			SizeF sizeF = g.MeasureString(displayText, this.Font);
			
			Matrix matrix = g.Transform;

			g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, this.Width, this.Height); //overwrite the horizontal text

			RectangleF rectF;
			if (mVertical) 
			{
				rectF = new RectangleF(0f, (this.Width - sizeF.Height)/2f, this.Height, sizeF.Height);
				g.TranslateTransform( 0f, this.Height,  MatrixOrder.Prepend ); //set rotation origin
				g.RotateTransform(-90);										   //rotate
			}
			else
				rectF = new RectangleF(0f, (this.Height - sizeF.Height)/2f, this.Width, sizeF.Height);
			
			//center

			g.DrawString(displayText, this.Font, mForeBrush, rectF, mFormat);

			if (mDrawBorder)
			{
				if (mVertical)
				{
					rectF = new RectangleF(0f, 0f, this.Height, this.Width);
					g.DrawRectangle(
						new Pen(Color.Black, 1),
						1.0F, 0.0F, this.Height-1, this.Width-1);
				}
				else
				{
					rectF = new RectangleF(0f, 0f, this.Width, this.Height);
					g.DrawRectangle(
						new Pen(Color.Black, 1),
						0.0F, 0.0F, this.Width-1, this.Height-1);
				}
			}

			g.Transform = matrix;

		}

		/// <summary>
		/// redraws the label item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void labelItem_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.Draw(e.Graphics);
		}

		#endregion

	}
}
