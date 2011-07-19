using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

using MultiAlignEngine;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;

using MultiAlignCore.Data;
using PNNLControls;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmDatasetSummary.
	/// </summary>
	public class DatasetSummary : PNNLControls.frmDialogBase
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem mnuFileSave;
        private IContainer components;		
		private MultiAlignWin.ctlSummaryPages ctlSummaryPages;

        private DatasetInformation mobj_dataInfo;
        private clsAlignmentOptions mobj_alignment;

        /// <summary>
        /// Constructor that displays the dataset information for a given dataset.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="alignment"></param>
        /// <param name="cluster"></param>
        public DatasetSummary(DatasetInformation data,
                                 clsAlignmentOptions alignment)
		{
			InitializeComponent();
            mobj_dataInfo    = data;
            mobj_alignment   = alignment;

            UpdateDisplay();
		}
		/// <summary>
		/// Updates the display of the dataset summary.
		/// </summary>
		private void UpdateDisplay()
		{
			ctlSummaryPages.Clear();

			if (mobj_dataInfo != null)                							
			    ctlSummaryPages.CreateSummary("Dataset Info",      mobj_dataInfo);
            if (mobj_alignment != null)
                ctlSummaryPages.CreateSummary("Alignment Options", mobj_alignment);								
		}		

		/// <summary>
		/// Saves the dataset information to the path provided 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="delimiter"></param>
		/// <param name="linesToSkip"></param>
		public void Save(string path, char delimiter, int linesToSkip)
		{
			ctlSummaryPages.Save(path, delimiter, linesToSkip);
        }

        #region Windows Generated
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatasetSummary));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuFileSave = new System.Windows.Forms.MenuItem();
            this.ctlSummaryPages = new MultiAlignWin.ctlSummaryPages();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileSave});
            this.menuItem1.Text = "File";
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Index = 0;
            this.mnuFileSave.Text = "Save";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // ctlSummaryPages
            // 
            this.ctlSummaryPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlSummaryPages.Location = new System.Drawing.Point(0, 0);
            this.ctlSummaryPages.Name = "ctlSummaryPages";
            this.ctlSummaryPages.Size = new System.Drawing.Size(696, 400);
            this.ctlSummaryPages.TabIndex = 1;
            // 
            // frmDatasetSummary
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(696, 448);
            this.Controls.Add(this.ctlSummaryPages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "frmDatasetSummary";
            this.Text = "Dataset Summary:";
            this.Controls.SetChildIndex(this.ctlSummaryPages, 0);
            this.ResumeLayout(false);

		}
		#endregion
        #endregion

        /// <summary>
        /// Handles when the user clicks the menu to save the data to a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void mnuFileSave_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.AddExtension = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = "*.txt";
			dialog.DereferenceLinks = true;
			dialog.ValidateNames = true;
			dialog.Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*" ;
			dialog.FilterIndex = 1;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				frmTextDelimitedFileSave saveForm = new frmTextDelimitedFileSave();
				saveForm.Icon = this.Icon;
				saveForm.Text = "Save Format Options";
				saveForm.LinesToSkip = 2;
				saveForm.Format = TextDataFileFormat.COLUMN;
				if (saveForm.ShowDialog() == DialogResult.OK)
				{
					ctlSummaryPages.Save(dialog.FileName, saveForm.Delimiter, saveForm.LinesToSkip);
				}
				saveForm.Dispose();
			}		
			dialog.Dispose();									  
		}
	}
}
