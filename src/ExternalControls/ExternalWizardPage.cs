using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Wizard.UI
{
	//public class ExternalWizardPage : Wizard.UI.WizardPage
	public class ExternalWizardPage : Wizard.UI.WizardPage
	{
		private System.Windows.Forms.Panel panelSideBar;
		private System.ComponentModel.IContainer components = null;

		public ExternalWizardPage()
		{
			// This call is required by the Windows Form Designer.
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
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panelSideBar = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelSideBar
            // 
            this.panelSideBar.BackgroundImage = global::ExternalControls.Properties.Resources.ExampleSidebar;
            this.panelSideBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSideBar.Location = new System.Drawing.Point(0, 0);
            this.panelSideBar.Name = "panelSideBar";
            this.panelSideBar.Size = new System.Drawing.Size(160, 248);
            this.panelSideBar.TabIndex = 0;
            // 
            // ExternalWizardPage
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.panelSideBar);
            this.Name = "ExternalWizardPage";
            this.Size = new System.Drawing.Size(424, 248);
            this.ResumeLayout(false);

		}
		#endregion
	}
}

