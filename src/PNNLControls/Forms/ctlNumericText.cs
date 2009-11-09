using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlNumericText.
	/// </summary>
	public class ctlNumericText : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox txt;
		private System.Windows.Forms.Label lbl;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlNumericText()
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txt = new System.Windows.Forms.TextBox();
			this.lbl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txt
			// 
			this.txt.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txt.Location = new System.Drawing.Point(0, 36);
			this.txt.Name = "txt";
			this.txt.Size = new System.Drawing.Size(150, 20);
			this.txt.TabIndex = 0;
			this.txt.Text = "";
			this.txt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_KeyPress);
			// 
			// lbl
			// 
			this.lbl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbl.Location = new System.Drawing.Point(0, 0);
			this.lbl.Name = "lbl";
			this.lbl.Size = new System.Drawing.Size(150, 36);
			this.lbl.TabIndex = 1;
			// 
			// ctlNumericText
			// 
			this.Controls.Add(this.lbl);
			this.Controls.Add(this.txt);
			this.Name = "ctlNumericText";
			this.Size = new System.Drawing.Size(150, 56);
			this.ResumeLayout(false);

		}
		#endregion

		private void txt_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			try
			{
				string s = txt.Text + e.KeyChar.ToString();
				double d = Convert.ToDouble(s);
				MessageBox.Show(s);
			}
			catch
			{
				MessageBox.Show("error");
			}
		}
	}
}
