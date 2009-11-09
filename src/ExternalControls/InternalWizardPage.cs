using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Wizard.UI
{
	public class InternalWizardPage : Wizard.UI.WizardPage
	{
		public Wizard.UI.WizardBanner Banner;
        private PictureBox mpictureBox;
		private System.ComponentModel.IContainer components = null;

		public InternalWizardPage()
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
            this.Banner = new Wizard.UI.WizardBanner();
            this.mpictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Banner.Dock = System.Windows.Forms.DockStyle.Top;
            this.Banner.Location = new System.Drawing.Point(134, 0);
            this.Banner.Name = "Banner";
            this.Banner.Size = new System.Drawing.Size(569, 60);
            this.Banner.Subtitle = "Subtitle";
            this.Banner.TabIndex = 0;
            this.Banner.Title = "Title";
            // 
            // mpictureBox
            // 
            this.mpictureBox.BackgroundImage = global::ExternalControls.Properties.Resources.ExampleSidebar;
            this.mpictureBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.mpictureBox.Image = global::ExternalControls.Properties.Resources.ExampleSidebar;
            this.mpictureBox.Location = new System.Drawing.Point(0, 0);
            this.mpictureBox.Name = "mpictureBox";
            this.mpictureBox.Size = new System.Drawing.Size(134, 579);
            this.mpictureBox.TabIndex = 1;
            this.mpictureBox.TabStop = false;
            // 
            // InternalWizardPage
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.Banner);
            this.Controls.Add(this.mpictureBox);
            this.DoubleBuffered = true;
            this.Name = "InternalWizardPage";
            this.Size = new System.Drawing.Size(703, 579);
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
	}
}

