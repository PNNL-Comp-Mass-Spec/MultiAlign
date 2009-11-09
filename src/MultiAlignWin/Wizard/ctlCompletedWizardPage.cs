using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Wizard.UI;

namespace MultiAlignWin
{
	public class ctlCompletedWizardPage : Wizard.UI.ExternalWizardPage
	{
		private Label label1;
		private PictureBox pictureBox1;
		private IContainer components = null;

        

        public ctlCompletedWizardPage( )
        {
			InitializeComponent();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlCompletedWizardPage));
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(168, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Analysis Completed!";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(462, 299);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 96);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // ctlCompletedWizardPage
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Name = "ctlCompletedWizardPage";
            this.Size = new System.Drawing.Size(550, 419);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.ctlCompletedWizardPage_SetActive);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.pictureBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void ctlCompletedWizardPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SetWizardButtons(WizardButtons.Finish);
            ExtraButtonVisible  = false;
            CancelButtonVisible = false;
            BackButtonEnabled   = false;
            BackButtonVisible   = false;
            NextButtonEnabled   = false;
		}

	}
}

