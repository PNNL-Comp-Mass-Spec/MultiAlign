// CustomColorPicker.cs : implementation file
//
// Part of ColorPicker controls.
//
// Author      : Philip Lee (pl@pjl.nildram.co.uk)
// Date        : 14 October 2001
//
// Copyright © PJL Consultants Ltd. 2001, All Rights Reserved                      
//
// This code may be used in compiled form in any way you desire. This
// file may be redistributed unmodified by any means PROVIDING it is 
// not sold for profit without the authors written consent, and 
// providing that this notice and the authors name is included. If 
// the source code in this file is used in any commercial application 
// then a simple email would be nice.
//
// This file is provided "as is" with no expressed or implied warranty.
// The author accepts no liability for any damage, in any form, caused
// by this code. Use it at your own risk and as with all code expect bugs!
// It's been tested but I'm not perfect.
// 
// Please use and enjoy. Please let me know of any bugs/mods/improvements 
// that you have found/implemented and I will fix/incorporate them into this
// file.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

namespace ExternalControls
{
	/// <summary>
	/// A control that allows the user to select any color from the
	/// RGB color space.
	/// </summary>
	/// <remarks>
	/// The control presents a 2 dimensional slice through the 3D color cube.  The user
	/// may select Red, Green or Blue to be the z-axis where the z-axis extends into the screen and the
	/// x and y-axes are the remaining pair of colors.
	/// The control displays a 256x256 color palette calculated over the x and y axes combined
	/// with the selected z-axis value.  This palette is varied continuously as the user changes the 
	/// selected z-axis value.
	/// <br></br><br></br>
	/// The control is based on <a href="http://dotnet.securedomains.com/colorpicker/default.aspx">
	/// Peter McMahon's ColorPicker.NET</a>.
	/// I have translated it to C#, improved performance and made it into a reusable control.
	/// </remarks>
	public class CustomColorPicker : System.Windows.Forms.UserControl
	{
		static readonly Color defaultColor = Color.FromArgb(127,127,127);
		const bool defaultContinuousScroll = true;

		private System.Windows.Forms.TrackBar trackBarRed;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.NumericUpDown numericUpDownRed;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton radioButtonBlue;
		private System.Windows.Forms.RadioButton radioButtonRed;
		private System.Windows.Forms.RadioButton radioButtonGreen;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.TrackBar trackBarGreen;
		private System.Windows.Forms.TrackBar trackBarBlue;
		private System.Windows.Forms.NumericUpDown numericUpDownGreen;
		private System.Windows.Forms.NumericUpDown numericUpDownBlue;

		private System.Drawing.Bitmap imgColors = new System.Drawing.Bitmap(256, 256);

		private ZAxis zaxis = ZAxis.red;
		private bool bContinuousScrollZAxis = defaultContinuousScroll;

		private int x_val = 0;
		private int y_val = 0;
		private bool bMouseDown = false;
		private System.Windows.Forms.ToolTip toolTip1;

		/// <summary>
		/// Creates a CustomColorPicker control.
		/// It defaults to the blue/green side of the color cube (z-axis=red).
		/// </summary>
		public CustomColorPicker()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			pictureBox.Image = imgColors;

			ResetColor();
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
			this.components = new System.ComponentModel.Container();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.radioButtonBlue = new System.Windows.Forms.RadioButton();
			this.numericUpDownRed = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownGreen = new System.Windows.Forms.NumericUpDown();
			this.radioButtonGreen = new System.Windows.Forms.RadioButton();
			this.trackBarRed = new System.Windows.Forms.TrackBar();
			this.trackBarBlue = new System.Windows.Forms.TrackBar();
			this.radioButtonRed = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.trackBarGreen = new System.Windows.Forms.TrackBar();
			this.numericUpDownBlue = new System.Windows.Forms.NumericUpDown();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownGreen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarRed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBlue)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarGreen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownBlue)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox.Location = new System.Drawing.Point(8, 8);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(256, 256);
			this.pictureBox.TabIndex = 11;
			this.pictureBox.TabStop = false;
			this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
			this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
			this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
			this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
			// 
			// radioButtonBlue
			// 
			this.radioButtonBlue.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButtonBlue.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(192)), ((System.Byte)(192)));
			this.radioButtonBlue.Location = new System.Drawing.Point(96, 0);
			this.radioButtonBlue.Name = "radioButtonBlue";
			this.radioButtonBlue.Size = new System.Drawing.Size(48, 24);
			this.radioButtonBlue.TabIndex = 3;
			this.radioButtonBlue.Text = "Blue";
			this.radioButtonBlue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.radioButtonBlue, "Set the Z axis to blue");
			this.radioButtonBlue.CheckedChanged += new System.EventHandler(this.radioButtonBlue_CheckedChanged);
			// 
			// numericUpDownRed
			// 
			this.numericUpDownRed.Location = new System.Drawing.Point(280, 240);
			this.numericUpDownRed.Maximum = new System.Decimal(new int[] {
																			 255,
																			 0,
																			 0,
																			 0});
			this.numericUpDownRed.Name = "numericUpDownRed";
			this.numericUpDownRed.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownRed.TabIndex = 7;
			this.numericUpDownRed.ValueChanged += new System.EventHandler(this.numericUpDownRed_ValueChanged);
			this.numericUpDownRed.Leave += new System.EventHandler(this.numericUpDownRed_Leave);
			// 
			// numericUpDownGreen
			// 
			this.numericUpDownGreen.Location = new System.Drawing.Point(328, 240);
			this.numericUpDownGreen.Maximum = new System.Decimal(new int[] {
																			   255,
																			   0,
																			   0,
																			   0});
			this.numericUpDownGreen.Name = "numericUpDownGreen";
			this.numericUpDownGreen.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownGreen.TabIndex = 8;
			this.numericUpDownGreen.ValueChanged += new System.EventHandler(this.numericUpDownGreen_ValueChanged);
			// 
			// radioButtonGreen
			// 
			this.radioButtonGreen.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButtonGreen.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(192)), ((System.Byte)(0)));
			this.radioButtonGreen.Location = new System.Drawing.Point(48, 0);
			this.radioButtonGreen.Name = "radioButtonGreen";
			this.radioButtonGreen.Size = new System.Drawing.Size(48, 24);
			this.radioButtonGreen.TabIndex = 2;
			this.radioButtonGreen.Text = "Green";
			this.radioButtonGreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.radioButtonGreen, "Set the Z axis to green");
			this.radioButtonGreen.CheckedChanged += new System.EventHandler(this.radioButtonGreen_CheckedChanged);
			// 
			// trackBarRed
			// 
			this.trackBarRed.LargeChange = 16;
			this.trackBarRed.Location = new System.Drawing.Point(287, 32);
			this.trackBarRed.Maximum = 255;
			this.trackBarRed.Name = "trackBarRed";
			this.trackBarRed.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBarRed.Size = new System.Drawing.Size(42, 208);
			this.trackBarRed.TabIndex = 4;
			this.trackBarRed.TickFrequency = 16;
			this.trackBarRed.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBarRed.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trackBarRed_KeyUp);
			this.trackBarRed.Leave += new System.EventHandler(this.trackBarRed_Leave);
			this.trackBarRed.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarRed_MouseUp);
			this.trackBarRed.ValueChanged += new System.EventHandler(this.trackBarRed_ValueChanged);
			// 
			// trackBarBlue
			// 
			this.trackBarBlue.LargeChange = 16;
			this.trackBarBlue.Location = new System.Drawing.Point(384, 32);
			this.trackBarBlue.Maximum = 255;
			this.trackBarBlue.Name = "trackBarBlue";
			this.trackBarBlue.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBarBlue.Size = new System.Drawing.Size(42, 208);
			this.trackBarBlue.TabIndex = 6;
			this.trackBarBlue.TickFrequency = 16;
			this.trackBarBlue.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBarBlue.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trackBarBlue_KeyUp);
			this.trackBarBlue.Leave += new System.EventHandler(this.trackBarBlue_Leave);
			this.trackBarBlue.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarBlue_MouseUp);
			this.trackBarBlue.ValueChanged += new System.EventHandler(this.trackBarBlue_ValueChanged);
			// 
			// radioButtonRed
			// 
			this.radioButtonRed.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButtonRed.BackColor = System.Drawing.Color.OrangeRed;
			this.radioButtonRed.Checked = true;
			this.radioButtonRed.Name = "radioButtonRed";
			this.radioButtonRed.Size = new System.Drawing.Size(48, 24);
			this.radioButtonRed.TabIndex = 1;
			this.radioButtonRed.TabStop = true;
			this.radioButtonRed.Text = "Red";
			this.radioButtonRed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.radioButtonRed, "Set the Z axis to red");
			this.radioButtonRed.CheckedChanged += new System.EventHandler(this.radioButtonRed_CheckedChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.radioButtonBlue,
																				 this.radioButtonRed,
																				 this.radioButtonGreen});
			this.panel1.Location = new System.Drawing.Point(280, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(144, 24);
			this.panel1.TabIndex = 10;
			// 
			// trackBarGreen
			// 
			this.trackBarGreen.LargeChange = 16;
			this.trackBarGreen.Location = new System.Drawing.Point(336, 32);
			this.trackBarGreen.Maximum = 255;
			this.trackBarGreen.Name = "trackBarGreen";
			this.trackBarGreen.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBarGreen.Size = new System.Drawing.Size(42, 208);
			this.trackBarGreen.TabIndex = 5;
			this.trackBarGreen.TickFrequency = 16;
			this.trackBarGreen.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBarGreen.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trackBarGreen_KeyUp);
			this.trackBarGreen.Leave += new System.EventHandler(this.trackBarGreen_Leave);
			this.trackBarGreen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarGreen_MouseUp);
			this.trackBarGreen.ValueChanged += new System.EventHandler(this.trackBarGreen_ValueChanged);
			// 
			// numericUpDownBlue
			// 
			this.numericUpDownBlue.Location = new System.Drawing.Point(376, 240);
			this.numericUpDownBlue.Maximum = new System.Decimal(new int[] {
																			  255,
																			  0,
																			  0,
																			  0});
			this.numericUpDownBlue.Name = "numericUpDownBlue";
			this.numericUpDownBlue.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownBlue.TabIndex = 9;
			this.numericUpDownBlue.ValueChanged += new System.EventHandler(this.numericUpDownBlue_ValueChanged);
			// 
			// CustomColorPicker
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pictureBox,
																		  this.numericUpDownRed,
																		  this.trackBarRed,
																		  this.trackBarGreen,
																		  this.trackBarBlue,
																		  this.numericUpDownGreen,
																		  this.numericUpDownBlue,
																		  this.panel1});
			this.Name = "CustomColorPicker";
			this.Size = new System.Drawing.Size(448, 272);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownGreen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarRed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBlue)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.trackBarGreen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownBlue)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void MakePalette()
		{
			switch( zaxis )
			{
			case ZAxis.red:
				MakeRedPalette();
				break;
			case ZAxis.blue:
				MakeBluePalette();
				break;
			case ZAxis.green:
				MakeGreenPalette();
				break;
			}
		}
	
		private void MakeRedPalette()
		{
			int red_value = trackBarRed.Value;

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
			int blue_value = trackBarBlue.Value;

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
			int green_value = trackBarGreen.Value;

			for( int i=0; i<255; i++ )
			{
				for( int j=0; j<255; j++ )
				{
					imgColors.SetPixel(i, j, Color.FromArgb(i, green_value, j) );
				}
			}
		}

		private void trackBarRedReleased()
		{
			if( (zaxis == ZAxis.red) && !bContinuousScrollZAxis)
			{
				MakeRedPalette();
				pictureBox.Refresh();
			}
		}

		// KeyUp not received for UpDown controls - beta2 bug?
		private void numericUpDownRed_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			trackBarRedReleased();
		}

		// Leave not receive for UpDown controls - beta2 bug?
		private void numericUpDownRed_Leave(object sender, System.EventArgs e)
		{
			trackBarRedReleased();
		}

		private void numericUpDownRed_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			trackBarRedReleased();
		}

		private void trackBarRed_Leave(object sender, System.EventArgs e)
		{
			trackBarRedReleased();
		}

		private void trackBarRed_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			trackBarRedReleased();
		}

		private void trackBarRed_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			trackBarRedReleased();
		}

		private void trackBarRed_ValueChanged(object sender, System.EventArgs e)
		{
			numericUpDownRed.Value = trackBarRed.Value;

			Application.DoEvents();

			if( (zaxis == ZAxis.red) && bContinuousScrollZAxis )
			{
				MakeRedPalette();
			}

			GetCoords();

			pictureBox.Refresh();

			FireColorChangedEvent();
		}

		private void numericUpDownRed_ValueChanged(object sender, System.EventArgs e)
		{
			trackBarRed.Value = (int)numericUpDownRed.Value;
		}

		private void trackBarBlueReleased()
		{
			if( zaxis == ZAxis.blue && !bContinuousScrollZAxis )
			{
				MakeBluePalette();
				pictureBox.Refresh();
			}
		}

		private void trackBarBlue_Leave(object sender, System.EventArgs e)
		{
			trackBarBlueReleased();
		}
		
		private void trackBarBlue_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			trackBarBlueReleased();
		}

		private void trackBarBlue_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			trackBarBlueReleased();
		}

		private void trackBarBlue_ValueChanged(object sender, System.EventArgs e)
		{
			numericUpDownBlue.Value = trackBarBlue.Value;
			Application.DoEvents();

			if( zaxis == ZAxis.blue && bContinuousScrollZAxis )
			{
				MakeBluePalette();
			}

			GetCoords();
			pictureBox.Refresh();

			FireColorChangedEvent();
		}

		private void numericUpDownBlue_ValueChanged(object sender, System.EventArgs e)
		{
			trackBarBlue.Value = (int)numericUpDownBlue.Value;
		}

		private void trackBarGreenReleased()
		{
			if( zaxis == ZAxis.green && !bContinuousScrollZAxis )
			{
				MakeGreenPalette();
				pictureBox.Refresh();
			}
		}
		
		private void trackBarGreen_Leave(object sender, System.EventArgs e)
		{
			trackBarGreenReleased();
		}

		private void trackBarGreen_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			trackBarGreenReleased();
		}

		private void trackBarGreen_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			trackBarGreenReleased();
		}

		private void trackBarGreen_ValueChanged(object sender, System.EventArgs e)
		{
			numericUpDownGreen.Value = trackBarGreen.Value;
			Application.DoEvents();

			if( zaxis == ZAxis.green && bContinuousScrollZAxis )
			{
				MakeGreenPalette();
			}

			GetCoords();
			pictureBox.Refresh();

			FireColorChangedEvent();
		}

		private void GetCoords()
		{
			switch( zaxis )
			{
			case ZAxis.green:
				x_val = trackBarRed.Value;
				y_val = trackBarBlue.Value;
				break;
			case ZAxis.red:
				x_val = trackBarGreen.Value;
				y_val = trackBarBlue.Value;
				break;
			case ZAxis.blue:
				x_val = trackBarRed.Value;
				y_val = trackBarGreen.Value;
				break;
			}
		}

		private void SetCoordBound( ref int coord )
		{
			if( coord < 0 )
				coord = 0;
			if( coord > 255 )
				coord = 255;
		}

		private void SetCoords(int x, int y)
		{
			SetCoordBound( ref x );
			SetCoordBound( ref y );

			switch( zaxis )
			{
			case ZAxis.green:
				trackBarRed.Value   = x;
				trackBarBlue.Value  = y;
				break;
			case ZAxis.red:
				trackBarGreen.Value = x;
				trackBarBlue.Value  = y;
				break;
			case ZAxis.blue:
				trackBarRed.Value   = x;
				trackBarGreen.Value = y;
				break;
			}
		}

		private void numericUpDownGreen_ValueChanged(object sender, System.EventArgs e)
		{
			trackBarGreen.Value = (int)numericUpDownGreen.Value;
		}

		private void radioButtonRed_CheckedChanged(object sender, System.EventArgs e)
		{
			if(radioButtonRed.Checked)
			{
				zaxis = ZAxis.red;
				MakeRedPalette();
				GetCoords();
				pictureBox.Refresh();
			}
		}

		private void radioButtonGreen_CheckedChanged(object sender, System.EventArgs e)
		{
			if(radioButtonGreen.Checked)
			{
				zaxis = ZAxis.green;
				MakeGreenPalette();
				GetCoords();
				pictureBox.Refresh();
			}
		}

		private void radioButtonBlue_CheckedChanged(object sender, System.EventArgs e)
		{
			if(radioButtonBlue.Checked)
			{
				zaxis = ZAxis.blue;
				MakeBluePalette();
				GetCoords();
				pictureBox.Refresh();
			}
		}

		private void pictureBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
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
		}

		private void pictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( bMouseDown )
			{
				SetCoords(e.X,e.Y);
			}
		}

		private void pictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			bMouseDown = true;
			SetCoords(e.X,e.Y);
		}

		private void pictureBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			bMouseDown = false;
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
				System.Drawing.Color color = Color.FromArgb(
					trackBarRed.Value,
					trackBarGreen.Value,
					trackBarBlue.Value );

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
				System.Drawing.Color color = Color.FromArgb(
					trackBarRed.Value,
					trackBarGreen.Value,
					trackBarBlue.Value );

				ColorChanged(this, e);
			}
		}


		/// <summary>
		/// Sets/gets the pick color.
		/// </summary>
		[Browsable(true), Category("ColorPanel"), Description("Get/set the pick color.")]
		public System.Drawing.Color Color
		{
			get
			{
				return Color.FromArgb(
					trackBarRed.Value,
					trackBarGreen.Value,
					trackBarBlue.Value );
			}
			set
			{
				if( value != Color.FromArgb(
					trackBarRed.Value,
					trackBarGreen.Value,
					trackBarBlue.Value ) )
				{
					trackBarRed.Value   = value.R;
					trackBarBlue.Value  = value.B;
					trackBarGreen.Value = value.G;

					FireColorChangedEvent();
				}
			}
		}

		/// <summary>
		/// Design time support to reset the Color property to it's default value.
		/// </summary>
		public void ResetColor()
		{
			Color = defaultColor;
		}

		/// <summary>
		/// Design time support to indicate whether the Color property should be serialized.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeColor()
		{
			return Color != defaultColor;
		}

		/// <summary>
		/// Enable/disable smooth scrolling of the z-axis of the color cube.
		/// You may want to disable this on slower systems since continuously recomputing
		/// the color palette is fairly processor intensive.
		/// </summary>
		[Browsable(true), Category("ColorPanel")]
		[Description("Enable/disable continuous z-axis color."), DefaultValue(defaultContinuousScroll)]
		public bool EnableContinuousScrollZ
		{
			get
			{
				return bContinuousScrollZAxis;
			}
			set
			{
				bContinuousScrollZAxis = value;
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

			trackBarBlue.Enabled       = Enabled;
			trackBarRed.Enabled        = Enabled;
			trackBarGreen.Enabled      = Enabled;

			numericUpDownBlue.Enabled  = Enabled;
			numericUpDownRed.Enabled   = Enabled;
			numericUpDownGreen.Enabled = Enabled;

			radioButtonBlue.Enabled    = Enabled;
			radioButtonRed.Enabled     = Enabled;
			radioButtonGreen.Enabled   = Enabled;

			pictureBox.Enabled = Enabled;

			Refresh();
		}

		/// <summary>
		/// Overrides OnLoad.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(System.EventArgs e)
		{
			MakePalette();
			Refresh();

			base.OnLoad(e);
		}
	}
}
