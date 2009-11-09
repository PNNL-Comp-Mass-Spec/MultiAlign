using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using PJLControls;


namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmHeatmapLegendEdit.
	/// </summary>
	public class frmHeatmapLegendEdit : System.Windows.Forms.Form
	{
		public PNNLControls.ctlLegendEdit legendEdit;
		private System.Windows.Forms.MenuItem mnuLegenIO;
		private System.Windows.Forms.MenuItem mnuLoadLegend;
		private System.Windows.Forms.MenuItem mnuSaveLegend;
		private System.Windows.Forms.OpenFileDialog openLegend;
		private System.Windows.Forms.MainMenu mnuMain;
		private System.Windows.Forms.SaveFileDialog saveLegend;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmHeatmapLegendEdit()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmHeatmapLegendEdit));
			this.legendEdit = new PNNLControls.ctlLegendEdit();
			this.openLegend = new System.Windows.Forms.OpenFileDialog();
			this.mnuMain = new System.Windows.Forms.MainMenu();
			this.mnuLegenIO = new System.Windows.Forms.MenuItem();
			this.mnuLoadLegend = new System.Windows.Forms.MenuItem();
			this.mnuSaveLegend = new System.Windows.Forms.MenuItem();
			this.saveLegend = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// legendEdit
			// 
			this.legendEdit.Dock = System.Windows.Forms.DockStyle.Top;
			this.legendEdit.LegendBitmap = ((System.Drawing.Bitmap)(resources.GetObject("legendEdit.LegendBitmap")));
			this.legendEdit.Location = new System.Drawing.Point(0, 0);
			this.legendEdit.MaxColor = System.Drawing.Color.White;
			this.legendEdit.MaxRange = 0F;
			this.legendEdit.MinColor = System.Drawing.Color.Black;
			this.legendEdit.MinRange = 0F;
			this.legendEdit.Name = "legendEdit";
			this.legendEdit.NaNColor = System.Drawing.SystemColors.Control;
			this.legendEdit.OverColor = System.Drawing.SystemColors.Control;
			this.legendEdit.Size = new System.Drawing.Size(424, 168);
			this.legendEdit.TabIndex = 0;
			this.legendEdit.UnderColor = System.Drawing.SystemColors.Control;
			this.legendEdit.ZScore = true;
			// 
			// openLegend
			// 
			this.openLegend.DefaultExt = "xml";
			// 
			// mnuMain
			// 
			this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuLegenIO});
			// 
			// mnuLegenIO
			// 
			this.mnuLegenIO.Index = 0;
			this.mnuLegenIO.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.mnuLoadLegend,
																					   this.mnuSaveLegend});
			this.mnuLegenIO.Text = "Legend IO";
			// 
			// mnuLoadLegend
			// 
			this.mnuLoadLegend.Index = 0;
			this.mnuLoadLegend.Text = "Load Legend";
			this.mnuLoadLegend.Click += new System.EventHandler(this.mnuLoadLegend_Click);
			// 
			// mnuSaveLegend
			// 
			this.mnuSaveLegend.Index = 1;
			this.mnuSaveLegend.Text = "SaveLegend";
			this.mnuSaveLegend.Click += new System.EventHandler(this.mnuSaveLegend_Click);
			// 
			// saveLegend
			// 
			this.saveLegend.DefaultExt = "xml";
			// 
			// frmHeatmapLegendEdit
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 294);
			this.Controls.Add(this.legendEdit);
			this.Menu = this.mnuMain;
			this.Name = "frmHeatmapLegendEdit";
			this.Text = "frmHeatmapLegendEdit";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmHeatmapLegendEdit_Closing);
			this.Load += new System.EventHandler(this.frmHeatmapLegendEdit_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmHeatmapLegendEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}

		private void frmHeatmapLegendEdit_Load(object sender, System.EventArgs e)
		{
			this.Width = this.Width - 1;
		}

		private void mnuSaveLegend_Click(object sender, System.EventArgs e)
		{
			if (saveLegend.ShowDialog() == DialogResult.OK)
				legendEdit.SaveLegend(saveLegend.FileName);
		}

		private void mnuLoadLegend_Click(object sender, System.EventArgs e)
		{
			openLegend.InitialDirectory = Application.StartupPath + "\\legends";
			if (openLegend.ShowDialog() == DialogResult.OK)
				legendEdit.LoadLegend(openLegend.FileName);
		}
	}
}
 