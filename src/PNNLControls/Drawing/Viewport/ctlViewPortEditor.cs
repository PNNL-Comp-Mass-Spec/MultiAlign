using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Provides an editing control for a chart Viewport, for use with the 
	/// PropertyGrid.
	/// </summary>
	public class ctlViewPortEditor : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox textXLow;
		private System.Windows.Forms.TextBox textXHigh;
		private System.Windows.Forms.Label labelX;
		private System.Windows.Forms.Label labelXTo;
		private System.Windows.Forms.Label labelY;
		private System.Windows.Forms.Label labelYTo;
		private System.Windows.Forms.TextBox textYLow;
		private System.Windows.Forms.TextBox textYHigh;
		private RectangleF selectedValue;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlViewPortEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		public RectangleF SelectedValue
		{
			get 
			{
				return this.selectedValue;
			}
			set 
			{
				this.textXLow.Text = value.Left.ToString();
				this.textXHigh.Text = value.Right.ToString();
				this.textYLow.Text = value.Top.ToString();
				this.textYHigh.Text = value.Bottom.ToString();
				this.selectedValue = value;
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
			this.textXLow = new System.Windows.Forms.TextBox();
			this.textXHigh = new System.Windows.Forms.TextBox();
			this.textYLow = new System.Windows.Forms.TextBox();
			this.textYHigh = new System.Windows.Forms.TextBox();
			this.labelX = new System.Windows.Forms.Label();
			this.labelXTo = new System.Windows.Forms.Label();
			this.labelY = new System.Windows.Forms.Label();
			this.labelYTo = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textXLow
			// 
			this.textXLow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textXLow.Location = new System.Drawing.Point(32, 8);
			this.textXLow.Name = "textXLow";
			this.textXLow.Size = new System.Drawing.Size(56, 20);
			this.textXLow.TabIndex = 0;
			this.textXLow.Text = "textBox1";
			this.textXLow.Validating += new System.ComponentModel.CancelEventHandler(this.textXLow_Validating);
			// 
			// textXHigh
			// 
			this.textXHigh.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textXHigh.Location = new System.Drawing.Point(32, 32);
			this.textXHigh.Name = "textXHigh";
			this.textXHigh.Size = new System.Drawing.Size(56, 20);
			this.textXHigh.TabIndex = 1;
			this.textXHigh.Text = "textBox2";
			this.textXHigh.Validating += new System.ComponentModel.CancelEventHandler(this.textXHigh_Validating);
			// 
			// textYLow
			// 
			this.textYLow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textYLow.Location = new System.Drawing.Point(32, 64);
			this.textYLow.Name = "textYLow";
			this.textYLow.Size = new System.Drawing.Size(56, 20);
			this.textYLow.TabIndex = 2;
			this.textYLow.Text = "textBox3";
			this.textYLow.Validating += new System.ComponentModel.CancelEventHandler(this.textYLow_Validating);
			// 
			// textYHigh
			// 
			this.textYHigh.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textYHigh.Location = new System.Drawing.Point(32, 88);
			this.textYHigh.Name = "textYHigh";
			this.textYHigh.Size = new System.Drawing.Size(56, 20);
			this.textYHigh.TabIndex = 3;
			this.textYHigh.Text = "textBox4";
			this.textYHigh.Validating += new System.ComponentModel.CancelEventHandler(this.textYHigh_Validating);
			// 
			// labelX
			// 
			this.labelX.Location = new System.Drawing.Point(8, 12);
			this.labelX.Name = "labelX";
			this.labelX.Size = new System.Drawing.Size(24, 16);
			this.labelX.TabIndex = 4;
			this.labelX.Text = "X:";
			// 
			// labelXTo
			// 
			this.labelXTo.Location = new System.Drawing.Point(8, 36);
			this.labelXTo.Name = "labelXTo";
			this.labelXTo.Size = new System.Drawing.Size(16, 16);
			this.labelXTo.TabIndex = 5;
			this.labelXTo.Text = "to";
			// 
			// labelY
			// 
			this.labelY.Location = new System.Drawing.Point(8, 68);
			this.labelY.Name = "labelY";
			this.labelY.Size = new System.Drawing.Size(24, 16);
			this.labelY.TabIndex = 6;
			this.labelY.Text = "Y:";
			// 
			// labelYTo
			// 
			this.labelYTo.Location = new System.Drawing.Point(8, 92);
			this.labelYTo.Name = "labelYTo";
			this.labelYTo.Size = new System.Drawing.Size(16, 16);
			this.labelYTo.TabIndex = 7;
			this.labelYTo.Text = "to";
			// 
			// ctlViewPortEditor
			// 
			this.Controls.Add(this.labelYTo);
			this.Controls.Add(this.labelY);
			this.Controls.Add(this.labelXTo);
			this.Controls.Add(this.labelX);
			this.Controls.Add(this.textYHigh);
			this.Controls.Add(this.textYLow);
			this.Controls.Add(this.textXHigh);
			this.Controls.Add(this.textXLow);
			this.Name = "ctlViewPortEditor";
			this.Size = new System.Drawing.Size(96, 112);
			this.ResumeLayout(false);

		}
		#endregion

		private void DecodeValue() 
		{
			try 
			{
				float xLow = float.Parse( this.textXLow.Text);
				float xHigh = float.Parse( this.textXHigh.Text);
				float yLow = float.Parse( this.textYLow.Text);
				float yHigh = float.Parse( this.textYHigh.Text);
				if (xLow >= xHigh || yLow >= yHigh) 
				{
					this.SelectedValue = selectedValue;
					return;
				}
				this.SelectedValue = new RectangleF(xLow, yLow, xHigh - xLow, yHigh - yLow);
			}
			catch (Exception e) 
			{
				MessageBox.Show(e.Message, "Error");
				this.SelectedValue = selectedValue;
			}
		}

		private void textYHigh_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.DecodeValue();
		}

		private void textYLow_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.DecodeValue();
		}

		private void textXHigh_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.DecodeValue();
		}

		private void textXLow_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.DecodeValue();
		}
	}
}
