using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmDataFactorListView.
	/// </summary>
	public class frmDataFactorListView : PNNLControls.frmDialogBase
	{
		private MultiAlignWin.ctlDefineFactorsWizardPage ctlDefineFactorsWizardPage;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmDataFactorListView()
		{
			InitializeComponent();
			Init();
		}

		private void Init()
		{
			ctlDefineFactorsWizardPage.Banner.Visible = false;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmDataFactorListView));
			this.ctlDefineFactorsWizardPage = new MultiAlignWin.ctlDefineFactorsWizardPage(null);
			this.SuspendLayout();
			// 
			// ctlDefineFactorsWizardPage
			// 
			this.ctlDefineFactorsWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlDefineFactorsWizardPage.Location = new System.Drawing.Point(0, 0);
			this.ctlDefineFactorsWizardPage.Name = "ctlDefineFactorsWizardPage";
			this.ctlDefineFactorsWizardPage.Size = new System.Drawing.Size(1064, 733);
			this.ctlDefineFactorsWizardPage.TabIndex = 0;
			// 
			// frmDataFactorListView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1064, 781);
			this.Controls.Add(this.ctlDefineFactorsWizardPage);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmDataFactorListView";
			this.Text = "Factor Definitions";
			this.Controls.SetChildIndex(this.ctlDefineFactorsWizardPage, 0);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
