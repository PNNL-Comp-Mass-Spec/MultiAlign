using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmExpandPanelHeight.
	/// </summary>
	public class frmExpandPanelHeight : PNNLControls.frmDialogBase
	{
		private int mHeight = 0;
		private System.Windows.Forms.TextBox mHeightTextBox;
		private System.Windows.Forms.Label mHeightLabel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmExpandPanelHeight()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.EditingHeight = 0;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		public int EditingHeight 
		{
			get 
			{
				return mHeight;
			}
			set 
			{
				this.mHeight = value;
				this.mHeightTextBox.Text = value.ToString();
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmExpandPanelHeight));
			this.mHeightTextBox = new System.Windows.Forms.TextBox();
			this.mHeightLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// mHeightTextBox
			// 
			this.mHeightTextBox.Location = new System.Drawing.Point(64, 8);
			this.mHeightTextBox.Name = "mHeightTextBox";
			this.mHeightTextBox.TabIndex = 0;
			this.mHeightTextBox.Text = "";
			this.mHeightTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.mHeightTextBox_Validating);
			// 
			// mHeightLabel
			// 
			this.mHeightLabel.Location = new System.Drawing.Point(0, 8);
			this.mHeightLabel.Name = "mHeightLabel";
			this.mHeightLabel.Size = new System.Drawing.Size(56, 22);
			this.mHeightLabel.TabIndex = 1;
			this.mHeightLabel.Text = "Height :";
			this.mHeightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// frmExpandPanelHeight
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(168, 87);
			this.Controls.Add(this.mHeightLabel);
			this.Controls.Add(this.mHeightTextBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmExpandPanelHeight";
			this.Text = "Expand Height";
			this.Controls.SetChildIndex(this.mHeightTextBox, 0);
			this.Controls.SetChildIndex(this.mHeightLabel, 0);
			this.ResumeLayout(false);

		}
		#endregion

		private void mHeightTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try 
			{
				int height = int.Parse(this.mHeightTextBox.Text);
				if (height >= 0) 
				{
					this.mHeight = height;
				}
				this.EditingHeight = mHeight;
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.StackTrace + " " + ex.Message)  ;
			}
		}

	}
}
