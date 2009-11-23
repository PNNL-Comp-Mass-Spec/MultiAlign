using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ExternalControls
{
	/// <summary>
	/// <B>Not implemented.</B><br></br><br></br>
	/// I intend to factor out the color panel in the CustomColorPicker into a separate control.
	/// </summary>
	public class CustomColorPanel : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.BorderStyle borderStyle = BorderStyle.FixedSingle;
		private Size borderSize = new Size(0,0);

		private System.Drawing.Bitmap imgColors = new System.Drawing.Bitmap(256, 256);
		private Color color;
		private ZAxis zaxis = ZAxis.red;
		private bool  bMouseDown = false;

		private float x_val = 128;
		private float y_val = 128;

		private bool autosize  = true;
		private bool isotropic = true;

		/// <summary>
		/// [T.B.D]
		/// </summary>
		public CustomColorPanel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			UpdatePalette();

			SetStyle( ControlStyles.ResizeRedraw, true );
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
			// CustomColorPanel
			// 
			this.Name = "CustomColorPanel";
			this.Size = new System.Drawing.Size(232, 248);

		}
		#endregion

		private void MakeRedPalette()
		{
			int red_value = color.R;

			for( int i=0; i<255; i++ )
			{
				for( int j=0; j<255; j++ )
				{
					imgColors.SetPixel(i, j, Color.FromArgb(red_value, i, j) );
				}
			}
		}

		private void MakeBluePalette()
		{
			int blue_value = color.B;

			for( int i=0; i<255; i++ )
			{
				for( int j=0; j<255; j++ )
				{
					imgColors.SetPixel(i, j, Color.FromArgb(i, j, blue_value) );
				}
			}
		}

		private void MakeGreenPalette()
		{
			int green_value = color.G;

			for( int i=0; i<255; i++ )
			{
				for( int j=0; j<255; j++ )
				{
					imgColors.SetPixel(i, j, Color.FromArgb(i, green_value, j) );
				}
			}
		}

		/// <summary>
		/// [T.B.D]
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			// todo: draw border

			e.Graphics.DrawImage( imgColors, ClientRectangle );

			if( Enabled )
			{
				Pen p = new Pen(Color.Gray);

				const int offset = 5;

				e.Graphics.DrawLine( p, x_val, 0, x_val, y_val-offset );
				e.Graphics.DrawLine( p, x_val, y_val+offset, x_val, 255 );

				e.Graphics.DrawLine( p, 0, y_val, x_val-offset, y_val );
				e.Graphics.DrawLine( p, x_val+offset, y_val, 255, y_val );

				e.Graphics.DrawRectangle( p, x_val-offset, y_val-offset, 2*offset, 2*offset );

				p.Dispose();
				p = null;
			}

			base.OnPaint(e);
		}


		/// <summary>
		/// Overrides OnPaintBackground.  Since the whole of the control is drawn
		/// in OnPaint, OnPaintBackground is overridden to do nothing.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			// no need to paint background
		}

		/// <summary>
		/// The ColorChangedEvent event handler.
		/// </summary>
		[Browsable(true), Category("ColorPicker")]
		public event ColorChangedEventHandler     ColorChanged;

		private void FireColorChangedEvent()
		{
			if( null != ColorChanged )
			{
				OnColorChanged(new ColorChangedEventArgs(color));
			}
		}

		/// <summary>
		/// Raises the ColorChanged event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnColorChanged(ColorChangedEventArgs e)
		{
			if( null != ColorChanged )
			{
				ColorChanged(this, e);
			}
		}

		/// <summary>
		/// Overrides OnEnabledChanged so the control can redraw itself 
		/// enabled/disabled.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnEnabledChanged(System.EventArgs e)
		{
			base.OnEnabledChanged(e);

			Refresh();
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			if( bMouseDown )
			{
				//SetCoords(e.X,e.Y);
			}
		}

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			bMouseDown = true;

			//SetCoords(e.X,e.Y);
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			bMouseDown = false;
		}

		/// <summary>
		/// 
		/// </summary>
		public ExternalControls.ZAxis ZAxis
		{
			get
			{
				return this.zaxis;
			}
			set
			{
				Utils.CheckValidEnumValue( "ZAxis", value, typeof(ExternalControls.ZAxis) );

				if( zaxis != value )
				{
					zaxis = value;

					UpdatePalette();

					Refresh();
				}
			}
		}

		private void UpdatePalette()
		{
			switch( zaxis )
			{
			case ZAxis.red:
				MakeRedPalette();
				break;
			case ZAxis.green:
				MakeGreenPalette();
				break;
			case ZAxis.blue:
				MakeBluePalette();
				break;
			}
		}

		/// <summary>
		/// Sets/gets the pick color.
		/// </summary>
		[Browsable(true), Category("ColorPanel")]
		public System.Drawing.Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if( value != color )
				{
					bool bUpdatePalette = false;

					switch( zaxis )
					{
					case ZAxis.blue:
						bUpdatePalette = (value.B != color.B);
						break;
					case ZAxis.red:
						bUpdatePalette = (value.R != color.R);
						break;
					case ZAxis.green:
						bUpdatePalette = (value.G != color.G);
						break;
					}

					color = value;

					if( bUpdatePalette )
					{
						UpdatePalette();
					}

					FireColorChangedEvent();

					Refresh();
				}
			}
		}

		/// <summary>
		/// Set/get the auto-size property.
		/// </summary>
		/// <remarks>
		/// Setting auto-size to <b>true</b> will fix the size of the control to 
		/// 256 x 256 (plus borders) so that every color will be displayed in one
		/// pixel.
		/// </remarks>
		[Browsable(true), Category("ColorPanel")]
		public override bool AutoSize
		{
			get
			{
				return autosize;
			}
			set
			{
				if( autosize != value )
				{
					autosize = value;

					// todo: resize control
				}
			}
		}

		/// <summary>
		/// Sets/gets the isotropic property.
		/// </summary>
		/// <remarks>
		/// If auto-size if <b>false</b> then the control can be scaled, but if
		/// isotropic is <b>true</b> then the x and y axis will be the same.
		/// </remarks>
		[Browsable(true), Category("ColorPanel")]
		public bool Isotropic
		{
			get
			{
				return isotropic;
			}
			set
			{
				if( isotropic != value )
				{
					isotropic = true;

					// todo: resize
				}
			}
		}

		/// <summary>
		/// Set/get the controls border style.
		/// </summary>
		[Browsable(true), Category("ColorPanel")]
		public new System.Windows.Forms.BorderStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				Utils.CheckValidEnumValue( "BorderStyle", value, typeof(System.Windows.Forms.BorderStyle) );

				if( borderStyle != value )
				{
					borderStyle = value;

					UpdateBorderSize();

					// AutoSizePanel();
				}
			}
		}

		/// <summary>
		/// Update the border size values based on the current border style.
		/// </summary>
		private void UpdateBorderSize()
		{
			Size bs = new Size();

			switch( borderStyle )
			{
			case BorderStyle.Fixed3D:
				bs = SystemInformation.Border3DSize;
				break;
			case BorderStyle.FixedSingle:
				bs = SystemInformation.BorderSize;
				break;
			case BorderStyle.None:
				break;
			}
			
			borderSize = bs;
		}
	}
}
