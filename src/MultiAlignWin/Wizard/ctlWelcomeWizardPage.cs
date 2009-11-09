using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MultiAlignWin
{
	public class ctlWelcomeWizardPage : Wizard.UI.ExternalWizardPage
	{
		private System.Windows.Forms.Panel panelMessage;
		private System.Windows.Forms.Label labelWelcome;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.ComponentModel.IContainer components = null;

        

		public ctlWelcomeWizardPage( )
        {
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this.SetActive +=new CancelEventHandler(ctlWelcomeWizardPage_SetActive);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlWelcomeWizardPage));
            this.panelMessage = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelWelcome = new System.Windows.Forms.Label();
            this.panelMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMessage
            // 
            this.panelMessage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelMessage.Controls.Add(this.pictureBox1);
            this.panelMessage.Controls.Add(this.label1);
            this.panelMessage.Controls.Add(this.labelWelcome);
            this.panelMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMessage.Location = new System.Drawing.Point(160, 0);
            this.panelMessage.Name = "panelMessage";
            this.panelMessage.Size = new System.Drawing.Size(264, 248);
            this.panelMessage.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(176, 80);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 96);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(216, 48);
            this.label1.TabIndex = 1;
            this.label1.Text = "This wizard will guide you through the selection of files and parameters to perfo" +
                "rm a Multialign Analysis.";
            // 
            // labelWelcome
            // 
            this.labelWelcome.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWelcome.Location = new System.Drawing.Point(0, 0);
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(264, 40);
            this.labelWelcome.TabIndex = 0;
            this.labelWelcome.Text = "Welcome to the Analysis Wizard";
            this.labelWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ctlWelcomeWizardPage
            // 
            this.Controls.Add(this.panelMessage);
            this.Name = "ctlWelcomeWizardPage";
            this.Controls.SetChildIndex(this.panelMessage, 0);
            this.panelMessage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void ctlWelcomeWizardPage_SetActive(object sender, CancelEventArgs e)
		{
			SetWizardButtons(Wizard.UI.WizardButtons.Next);
		}
	}
}

