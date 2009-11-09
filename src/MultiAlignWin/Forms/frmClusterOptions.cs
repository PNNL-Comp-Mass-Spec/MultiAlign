using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmClusterOptions.
	/// </summary>
	public class frmClusterOptions : System.Windows.Forms.Form
	{
		#region "Controls"
		private System.Windows.Forms.Button mbtn_OK;
		private System.Windows.Forms.Button mbtn_cancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox mlist_baseline;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton mradio_max_in_dataset;
		private System.Windows.Forms.RadioButton mradioButton_sum_in_dataset;
		#endregion 
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmClusterOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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

		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.mbtn_OK = new System.Windows.Forms.Button();
            this.mbtn_cancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mradioButton_sum_in_dataset = new System.Windows.Forms.RadioButton();
            this.mradio_max_in_dataset = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.mlist_baseline = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbtn_OK
            // 
            this.mbtn_OK.Location = new System.Drawing.Point(64, 16);
            this.mbtn_OK.Name = "mbtn_OK";
            this.mbtn_OK.Size = new System.Drawing.Size(72, 23);
            this.mbtn_OK.TabIndex = 1;
            this.mbtn_OK.Text = "OK";
            this.mbtn_OK.Click += new System.EventHandler(this.mbtn_OK_Click);
            // 
            // mbtn_cancel
            // 
            this.mbtn_cancel.Location = new System.Drawing.Point(200, 16);
            this.mbtn_cancel.Name = "mbtn_cancel";
            this.mbtn_cancel.Size = new System.Drawing.Size(72, 23);
            this.mbtn_cancel.TabIndex = 2;
            this.mbtn_cancel.Text = "Cancel";
            this.mbtn_cancel.Click += new System.EventHandler(this.mbtn_cancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mradioButton_sum_in_dataset);
            this.groupBox2.Controls.Add(this.mradio_max_in_dataset);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(152, 52);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dataset Intensity";
            // 
            // mradioButton_sum_in_dataset
            // 
            this.mradioButton_sum_in_dataset.Location = new System.Drawing.Point(16, 34);
            this.mradioButton_sum_in_dataset.Name = "mradioButton_sum_in_dataset";
            this.mradioButton_sum_in_dataset.Size = new System.Drawing.Size(104, 16);
            this.mradioButton_sum_in_dataset.TabIndex = 1;
            this.mradioButton_sum_in_dataset.Text = "Sum in dataset";
            // 
            // mradio_max_in_dataset
            // 
            this.mradio_max_in_dataset.Checked = true;
            this.mradio_max_in_dataset.Location = new System.Drawing.Point(16, 16);
            this.mradio_max_in_dataset.Name = "mradio_max_in_dataset";
            this.mradio_max_in_dataset.Size = new System.Drawing.Size(128, 16);
            this.mradio_max_in_dataset.TabIndex = 0;
            this.mradio_max_in_dataset.TabStop = true;
            this.mradio_max_in_dataset.Text = "Maximum in dataset";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(336, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select baseline:";
            // 
            // mlist_baseline
            // 
            this.mlist_baseline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlist_baseline.Location = new System.Drawing.Point(0, 16);
            this.mlist_baseline.Name = "mlist_baseline";
            this.mlist_baseline.Size = new System.Drawing.Size(184, 212);
            this.mlist_baseline.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mlist_baseline);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(336, 229);
            this.panel1.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(184, 16);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(152, 213);
            this.panel3.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.mbtn_OK);
            this.panel2.Controls.Add(this.mbtn_cancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 181);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(336, 48);
            this.panel2.TabIndex = 5;
            // 
            // frmClusterOptions
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(336, 229);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmClusterOptions";
            this.Text = "Cluster Options";
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetData(string [] files, clsConfigParms cfg)
		{
			mlist_baseline.Items.Clear() ; 
			for (int i = 0 ; i < files.Length ; i++)
			{
				mlist_baseline.Items.Add(files[i]) ; 
			}
		}

		private void mbtn_OK_Click(object sender, System.EventArgs e)
		{
			string baseline = null ; 
			try
			{
				baseline = SelectedBaseline ; 
			}
			catch(Exception ex)
			{
                System.Diagnostics.Debug.WriteLine(ex.Message);
				MessageBox.Show("Please make sure you selected a baseline") ;
                Console.WriteLine(ex.Message);
				return ; 
			}

			if (baseline == null)
			{
				MessageBox.Show("Please select a baseline") ;
				return ; 
			}

			DialogResult = DialogResult.OK ; 
			this.Hide() ; 
		}

		private void mbtn_cancel_Click(object sender, System.EventArgs e)
		{		
			DialogResult = DialogResult.Cancel ; 
			this.Hide() ; 
		}
#region "Properties"

		public string SelectedBaseline
		{
			get
			{
				return (string)mlist_baseline.SelectedItem ; 
			}
		}
		public bool SumDatasetIntensities
		{
			get
			{
				return mradioButton_sum_in_dataset.Checked ; 
			}
		}

		#endregion		

	}
}
