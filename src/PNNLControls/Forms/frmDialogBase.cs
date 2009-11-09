using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls {
	/// <summary>
	/// Simple dialog base with OK and Cancel buttons.
	/// </summary>
	public class frmDialogBase : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel mButtonPanel;
		private System.Windows.Forms.Button mOKButton;
		private System.Windows.Forms.Button mCancelButton;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmDialogBase()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmDialogBase));
			this.mButtonPanel = new System.Windows.Forms.Panel();
			this.mOKButton = new System.Windows.Forms.Button();
			this.mCancelButton = new System.Windows.Forms.Button();
			this.mButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mButtonPanel
			// 
			this.mButtonPanel.AccessibleDescription = resources.GetString("mButtonPanel.AccessibleDescription");
			this.mButtonPanel.AccessibleName = resources.GetString("mButtonPanel.AccessibleName");
			this.mButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("mButtonPanel.Anchor")));
			this.mButtonPanel.AutoScroll = ((bool)(resources.GetObject("mButtonPanel.AutoScroll")));
			this.mButtonPanel.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("mButtonPanel.AutoScrollMargin")));
			this.mButtonPanel.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("mButtonPanel.AutoScrollMinSize")));
			this.mButtonPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mButtonPanel.BackgroundImage")));
			this.mButtonPanel.Controls.Add(this.mOKButton);
			this.mButtonPanel.Controls.Add(this.mCancelButton);
			this.mButtonPanel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("mButtonPanel.Dock")));
			this.mButtonPanel.Enabled = ((bool)(resources.GetObject("mButtonPanel.Enabled")));
			this.mButtonPanel.Font = ((System.Drawing.Font)(resources.GetObject("mButtonPanel.Font")));
			this.mButtonPanel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("mButtonPanel.ImeMode")));
			this.mButtonPanel.Location = ((System.Drawing.Point)(resources.GetObject("mButtonPanel.Location")));
			this.mButtonPanel.Name = "mButtonPanel";
			this.mButtonPanel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mButtonPanel.RightToLeft")));
			this.mButtonPanel.Size = ((System.Drawing.Size)(resources.GetObject("mButtonPanel.Size")));
			this.mButtonPanel.TabIndex = ((int)(resources.GetObject("mButtonPanel.TabIndex")));
			this.mButtonPanel.Text = resources.GetString("mButtonPanel.Text");
			this.mButtonPanel.Visible = ((bool)(resources.GetObject("mButtonPanel.Visible")));
			// 
			// mOKButton
			// 
			this.mOKButton.AccessibleDescription = resources.GetString("mOKButton.AccessibleDescription");
			this.mOKButton.AccessibleName = resources.GetString("mOKButton.AccessibleName");
			this.mOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("mOKButton.Anchor")));
			this.mOKButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mOKButton.BackgroundImage")));
			this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.mOKButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("mOKButton.Dock")));
			this.mOKButton.Enabled = ((bool)(resources.GetObject("mOKButton.Enabled")));
			this.mOKButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("mOKButton.FlatStyle")));
			this.mOKButton.Font = ((System.Drawing.Font)(resources.GetObject("mOKButton.Font")));
			this.mOKButton.Image = ((System.Drawing.Image)(resources.GetObject("mOKButton.Image")));
			this.mOKButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("mOKButton.ImageAlign")));
			this.mOKButton.ImageIndex = ((int)(resources.GetObject("mOKButton.ImageIndex")));
			this.mOKButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("mOKButton.ImeMode")));
			this.mOKButton.Location = ((System.Drawing.Point)(resources.GetObject("mOKButton.Location")));
			this.mOKButton.Name = "mOKButton";
			this.mOKButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mOKButton.RightToLeft")));
			this.mOKButton.Size = ((System.Drawing.Size)(resources.GetObject("mOKButton.Size")));
			this.mOKButton.TabIndex = ((int)(resources.GetObject("mOKButton.TabIndex")));
			this.mOKButton.Text = resources.GetString("mOKButton.Text");
			this.mOKButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("mOKButton.TextAlign")));
			this.mOKButton.Visible = ((bool)(resources.GetObject("mOKButton.Visible")));
			this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
			// 
			// mCancelButton
			// 
			this.mCancelButton.AccessibleDescription = resources.GetString("mCancelButton.AccessibleDescription");
			this.mCancelButton.AccessibleName = resources.GetString("mCancelButton.AccessibleName");
			this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("mCancelButton.Anchor")));
			this.mCancelButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mCancelButton.BackgroundImage")));
			this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.mCancelButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("mCancelButton.Dock")));
			this.mCancelButton.Enabled = ((bool)(resources.GetObject("mCancelButton.Enabled")));
			this.mCancelButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("mCancelButton.FlatStyle")));
			this.mCancelButton.Font = ((System.Drawing.Font)(resources.GetObject("mCancelButton.Font")));
			this.mCancelButton.Image = ((System.Drawing.Image)(resources.GetObject("mCancelButton.Image")));
			this.mCancelButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("mCancelButton.ImageAlign")));
			this.mCancelButton.ImageIndex = ((int)(resources.GetObject("mCancelButton.ImageIndex")));
			this.mCancelButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("mCancelButton.ImeMode")));
			this.mCancelButton.Location = ((System.Drawing.Point)(resources.GetObject("mCancelButton.Location")));
			this.mCancelButton.Name = "mCancelButton";
			this.mCancelButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mCancelButton.RightToLeft")));
			this.mCancelButton.Size = ((System.Drawing.Size)(resources.GetObject("mCancelButton.Size")));
			this.mCancelButton.TabIndex = ((int)(resources.GetObject("mCancelButton.TabIndex")));
			this.mCancelButton.Text = resources.GetString("mCancelButton.Text");
			this.mCancelButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("mCancelButton.TextAlign")));
			this.mCancelButton.Visible = ((bool)(resources.GetObject("mCancelButton.Visible")));
			this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
			// 
			// frmDialogBase
			// 
			this.AcceptButton = this.mOKButton;
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.CancelButton = this.mCancelButton;
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.mButtonPanel);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "frmDialogBase";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.mButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		[System.ComponentModel.Category("Behavoir")]
		[System.ComponentModel.DefaultValue(true)]
		public bool CancelEnabled 
		{
			get 
			{
				return this.mCancelButton.Enabled;
			}
			set 
			{
				this.mCancelButton.Enabled = value;
			}
		}

		[System.ComponentModel.Category("Behavoir")]
		[System.ComponentModel.DefaultValue(true)]
		public bool OKEnabled 
		{
			get 
			{
				return this.mOKButton.Enabled;
			}
			set 
			{
				this.mOKButton.Enabled = value;
			}
		}

		private void mOKButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Hide();
		}

		private void mCancelButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Hide();
		}
	}
}
