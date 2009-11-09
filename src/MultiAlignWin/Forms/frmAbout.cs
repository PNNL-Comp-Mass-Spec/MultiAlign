using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmAbout.
	/// </summary>
	public class frmAbout : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button mbtnOK;
		private System.Windows.Forms.Label mlblWizards;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmAbout()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mlblWizards.Text = "Deep Jaitly" + Environment.NewLine + "Ashoka Polpitiya" + 
				Environment.NewLine + "Brian LaMarche" + Environment.NewLine + "Derek Hopkins" ;
							
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
			System.Reflection.AssemblyName an = a.GetName();			
			lblVersion.Text = an.Version.ToString();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmAbout));
			this.lblVersion = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.mbtnOK = new System.Windows.Forms.Button();
			this.mlblWizards = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblVersion
			// 
			this.lblVersion.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblVersion.Location = new System.Drawing.Point(24, 88);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(100, 16);
			this.lblVersion.TabIndex = 1;
			this.lblVersion.Text = "Version 1.1";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(24, 136);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(248, 72);
			this.label3.TabIndex = 2;
			this.label3.Text = "MultiAlign features a comprehensive set of algorithms for High Throughput  Proteo" +
				"mics.  Download the most recent version from the NCRR website:  ";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.Location = new System.Drawing.Point(24, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(144, 23);
			this.label4.TabIndex = 3;
			this.label4.Text = "Battelle Memorial Institute";
			// 
			// mbtnOK
			// 
			this.mbtnOK.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mbtnOK.Location = new System.Drawing.Point(128, 344);
			this.mbtnOK.Name = "mbtnOK";
			this.mbtnOK.TabIndex = 5;
			this.mbtnOK.Text = "OK";
			this.mbtnOK.Click += new System.EventHandler(this.mbtnOK_Click);
			// 
			// mlblWizards
			// 
			this.mlblWizards.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mlblWizards.Location = new System.Drawing.Point(24, 296);
			this.mlblWizards.Name = "mlblWizards";
			this.mlblWizards.Size = new System.Drawing.Size(176, 40);
			this.mlblWizards.TabIndex = 6;
			this.mlblWizards.Text = "Navdeep Jaitly, Ashoka Polpitiya, Brian Lamarche";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.Location = new System.Drawing.Point(24, 272);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 16);
			this.label6.TabIndex = 7;
			this.label6.Text = "Developers:";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(248, 16);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(64, 64);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
			this.pictureBox2.Location = new System.Drawing.Point(8, 8);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(248, 72);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 9;
			this.pictureBox2.TabStop = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(40, 216);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(248, 32);
			this.label1.TabIndex = 10;
			this.label1.Text = " http://omics.pnl.gov/software";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(40, 200);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(248, 16);
			this.label2.TabIndex = 11;
			this.label2.Text = " http://ncrr.pnl.gov/software ";
			// 
			// frmAbout
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(322, 375);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.mlblWizards);
			this.Controls.Add(this.mbtnOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblVersion);
			this.Font = new System.Drawing.Font("Arial Black", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAbout";
			this.Text = "About MultiAlign";
			this.ResumeLayout(false);

		}
		#endregion

		private void mbtnOK_Click(object sender, System.EventArgs e)
		{
			this.Hide() ;
		}

		
	}
}
