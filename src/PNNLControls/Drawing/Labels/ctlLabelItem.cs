using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

using IDLTools;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlLabelItem.
	/// </summary>
	public class ctlLabelItem : System.Windows.Forms.Label 
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
	
		private  ToolTip m_toolTip = new ToolTip();

		public class clsEdit
		{
			private ctlLabelItem lbl = new ctlLabelItem();
			
			public String Text
			{
				get{return(lbl.Text);}
				set{lbl.Text=value;}
			}

			public Font Font
			{
				get{return(lbl.Font);}
				set{lbl.Font=value;}
			}

			public clsEdit(ctlLabelItem parentLabel)
			{
				lbl = parentLabel;
			}
		}

		public ctlLabelItem()
		{
			try
			{
				// Activates double buffering
				SetStyle(ControlStyles.UserPaint, true);
				SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				SetStyle(ControlStyles.DoubleBuffer, true);

				// This call is required by the Windows.Forms Form Designer.
				InitializeComponent();

				// TODO: Add any initialization after the InitializeComponent call
			}catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}			

			m_toolTip.SetToolTip(this, "");
			m_toolTip.InitialDelay = 2;
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

		public void EditLabelProperties()
		{
			frmPropertyGrid frm = new frmPropertyGrid();
			frm.SelectedObject = this;
			frm.ShowDialog();
			this.clsLabelAttributes.text = this.Text;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ctlLabelItem
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
		clsLabelAttributes mLabelAttributes = null; 

		/// <summary>
		/// index of low leaf of this label.  Index is in relation to the overall hierarchy
		/// </summary>
		//int mLowLeaf = 0;

		/// <summary>
		/// index of low leaf of this label.  Index is in relation to the overall hierarchy
		/// </summary>
		//int mHighLeaf = 0;

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

		public void ShowEdit()
		{
			frmProperties frm = new frmProperties();
			clsEdit edit = new clsEdit(this);
			frm.Props.SelectedObject = edit;
			frm.ShowDialog();
			this.Invalidate();
		}

		/// <summary>
		/// a pointer to a labelAttributes class is maintained.  This allows us to maintain the
		/// relationship of a label to a larger, undrawn set of labels
		/// </summary>
		public clsLabelAttributes clsLabelAttributes 
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

		protected override void OnTextChanged(EventArgs e)
		{
			this.m_toolTip.SetToolTip(this,Text);
			base.OnTextChanged (e);
		}


		#region Rendering Methods

		/// <summary>
		/// returns the text that will fit in the label given the contstraints of label and font size
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		private string GetDisplayText(Graphics g)
		{
			try
			{
				float dpiY = (float) g.DpiY;
				float height = 1f;
				float width = 1f;
				string displayText = this.Text;

				if (mVertical)
				{
					height = this.ClientRectangle.Width;
					width = this.ClientRectangle.Height;
				}
				else
				{
					height = this.ClientRectangle.Height;
					width = this.ClientRectangle.Width;
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
					return displayText;
				}

				// trim the string
				sizeF = g.MeasureString(".", this.Font);
				int nChars = (int)(width / sizeF.Width);

				if (nChars >= displayText.Length)
				{
					return displayText;
				}

				if (nChars<=0)  
				{
					return "";
				}
				else
				{
					if (mFormat.Alignment == StringAlignment.Far)
					{
						return (".." + displayText.Substring(displayText.Length-nChars, nChars));
					}
					else
					{
						return (displayText.Substring(0, nChars) + "..");
					}
				}
			}catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
				return "";
			}
		}

		/// <summary>
		/// autosizes the font for the label text
		/// </summary>
		/// <param name="g"></param>
		private void AutoSizeLabel (Graphics g)
		{
			try
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
			}catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// draws the label, either horizontally or vertically
		/// </summary>
		/// <param name="g"></param>
		public void Draw(Graphics g)
		{
			try
			{
				string displayText = GetDisplayText(g);
				//string displayText = this.Text;

				SizeF sizeF = g.MeasureString(displayText, this.Font);
		
				Matrix matrix = g.Transform;

				g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle); //overwrite the horizontal text

				RectangleF rectF;
				if (mVertical) 
				{
					rectF = new RectangleF(0f, (this.Width - sizeF.Height)/2f, this.Height, sizeF.Height);
					g.TranslateTransform( 0f, this.Height,  MatrixOrder.Prepend ); //set rotation origin
					g.RotateTransform(-90);										   //rotate
				}
				else
					rectF = new RectangleF(0f, (this.Height - sizeF.Height)/2f, this.Width, sizeF.Height);

				g.DrawString(displayText, this.Font, mForeBrush, rectF, mFormat);

				g.Transform = matrix;

			}catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}

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
