using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlTextEditor.
	/// </summary>
	public class ctlTextEditor : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox mTextField;
		private System.Windows.Forms.Label mLabel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlTextEditor()
		{
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

		public String UserText 
		{
			get 
			{
				return this.mTextField.Text;
			} 
			set 
			{
				this.mTextField.Text = value;
			}
		}

		public String UserLabel 
		{
			get 
			{
				return this.mLabel.Text;
			}
			set 
			{
				this.mLabel.Text = value;
			}
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mTextField = new System.Windows.Forms.TextBox();
			this.mLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// mTextField
			// 
			this.mTextField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mTextField.Location = new System.Drawing.Point(8, 24);
			this.mTextField.Multiline = true;
			this.mTextField.Name = "mTextField";
			this.mTextField.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.mTextField.Size = new System.Drawing.Size(104, 56);
			this.mTextField.TabIndex = 0;
			this.mTextField.Text = "Text";
			// 
			// mLabel
			// 
			this.mLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mLabel.Location = new System.Drawing.Point(8, 8);
			this.mLabel.Name = "mLabel";
			this.mLabel.Size = new System.Drawing.Size(104, 16);
			this.mLabel.TabIndex = 1;
			this.mLabel.Text = "Edit";
			// 
			// ctlTextEditor
			// 
			this.Controls.Add(this.mLabel);
			this.Controls.Add(this.mTextField);
			this.Name = "ctlTextEditor";
			this.Size = new System.Drawing.Size(120, 88);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
