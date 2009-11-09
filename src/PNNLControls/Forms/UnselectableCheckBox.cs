using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	public enum TextDirection 
	{
		Horiontal, 
		Vertical
	}

	/// <summary>
	/// Makes a checkbox unselectable by setting it's style to unselectable, 
	/// and by disabling focus cues (the dashed rectangle that appears around 
	/// the active control.)  Paints like a label but also does vertical 
	/// text.
	/// </summary>
	public class UnselectableCheckBox : System.Windows.Forms.CheckBox
	{
		private const TextDirection DEFAULT_TEXT_DIRECTION = TextDirection.Horiontal;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private TextDirection mTextDirection = DEFAULT_TEXT_DIRECTION;

		public UnselectableCheckBox()
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.Selectable, false);
			this.SetStyle(ControlStyles.Opaque, false);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.UpdateStyles();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(new SolidBrush(this.BackColor), 0, 0, this.Width, this.Height);
			StringFormat f = new StringFormat();
			if (TextDirection == TextDirection.Vertical) 
			{
				f.FormatFlags |= StringFormatFlags.DirectionVertical;
			}
			if (RightToLeft == RightToLeft.Yes) 
			{
				f.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}

			if (TextAlign == ContentAlignment.BottomCenter || 
				TextAlign == ContentAlignment.BottomLeft || 
				TextAlign == ContentAlignment.BottomRight) 
			{
				f.LineAlignment = StringAlignment.Far;
			} 
			else if (TextAlign == ContentAlignment.MiddleCenter || 
				TextAlign == ContentAlignment.MiddleLeft || 
				TextAlign == ContentAlignment.MiddleRight) 
			{
				f.LineAlignment = StringAlignment.Center;
			} 
			else if (TextAlign == ContentAlignment.TopCenter || 
				TextAlign == ContentAlignment.TopLeft || 
				TextAlign == ContentAlignment.TopRight) 
			{
				f.LineAlignment = StringAlignment.Near;
			}
			if (TextAlign == ContentAlignment.BottomCenter || 
				TextAlign == ContentAlignment.MiddleCenter || 
				TextAlign == ContentAlignment.TopCenter) 
			{
				f.Alignment = StringAlignment.Center;
			} 
			else if (TextAlign == ContentAlignment.BottomLeft || 
				TextAlign == ContentAlignment.MiddleLeft || 
				TextAlign == ContentAlignment.TopLeft ) 
			{
				f.Alignment = StringAlignment.Near;
			} 
			else if (TextAlign == ContentAlignment.BottomRight || 
				TextAlign == ContentAlignment.MiddleRight || 
				TextAlign == ContentAlignment.TopRight) 
			{
				f.Alignment = StringAlignment.Far;
			}
			e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), 
				new RectangleF(0, 0, this.Width, this.Height), f);
		}


		protected override bool ShowFocusCues
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(DEFAULT_TEXT_DIRECTION)]
		[Description("Controls the direction of the text in the label")]
		public TextDirection TextDirection 
		{
			get 
			{
				return mTextDirection;
			}
			set 
			{
				mTextDirection = value;
				this.Invalidate();
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
	}
}
