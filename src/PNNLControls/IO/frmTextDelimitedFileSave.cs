using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmTextDelimitedFileSave.
	/// </summary>
	public class frmTextDelimitedFileSave : frmDialogBase
	{
		private PNNLControls.ctlTextDelimitedFileSave ctlTextDelimitedFileSave;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmTextDelimitedFileSave()
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
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Type of delimiter to use between data items.
		/// </summary>
		public char Delimiter
		{
			get
			{
				return ctlTextDelimitedFileSave.Delimiter;
			}
			set
			{
				ctlTextDelimitedFileSave.Delimiter = value;
			}
		}

		/// <summary>
		/// Lines to skip before the header and data.
		/// </summary>
		public int LinesToSkip
		{
			get
			{
				return ctlTextDelimitedFileSave.LinesToSkip;
			}
			set
			{
				ctlTextDelimitedFileSave.LinesToSkip = value;
			}
		}

		/// <summary>
		/// Format to use for saving the file - column or row.
		/// </summary>
		public TextDataFileFormat Format
		{
			get
			{
				return ctlTextDelimitedFileSave.Format;
			}
			set
			{
				ctlTextDelimitedFileSave.Format = value;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmTextDelimitedFileSave));
			this.ctlTextDelimitedFileSave = new PNNLControls.ctlTextDelimitedFileSave();
			this.SuspendLayout();
			// 
			// ctlTextDelimitedFileSave
			// 
			this.ctlTextDelimitedFileSave.AccessibleDescription = resources.GetString("ctlTextDelimitedFileSave.AccessibleDescription");
			this.ctlTextDelimitedFileSave.AccessibleName = resources.GetString("ctlTextDelimitedFileSave.AccessibleName");
			this.ctlTextDelimitedFileSave.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("ctlTextDelimitedFileSave.Anchor")));
			this.ctlTextDelimitedFileSave.AutoScroll = ((bool)(resources.GetObject("ctlTextDelimitedFileSave.AutoScroll")));
			this.ctlTextDelimitedFileSave.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("ctlTextDelimitedFileSave.AutoScrollMargin")));
			this.ctlTextDelimitedFileSave.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("ctlTextDelimitedFileSave.AutoScrollMinSize")));
			this.ctlTextDelimitedFileSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ctlTextDelimitedFileSave.BackgroundImage")));
			this.ctlTextDelimitedFileSave.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("ctlTextDelimitedFileSave.Dock")));
			this.ctlTextDelimitedFileSave.Enabled = ((bool)(resources.GetObject("ctlTextDelimitedFileSave.Enabled")));
			this.ctlTextDelimitedFileSave.Font = ((System.Drawing.Font)(resources.GetObject("ctlTextDelimitedFileSave.Font")));
			this.ctlTextDelimitedFileSave.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("ctlTextDelimitedFileSave.ImeMode")));
			this.ctlTextDelimitedFileSave.Location = ((System.Drawing.Point)(resources.GetObject("ctlTextDelimitedFileSave.Location")));
			this.ctlTextDelimitedFileSave.Name = "ctlTextDelimitedFileSave";
			this.ctlTextDelimitedFileSave.Path = null;
			this.ctlTextDelimitedFileSave.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("ctlTextDelimitedFileSave.RightToLeft")));
			this.ctlTextDelimitedFileSave.Size = ((System.Drawing.Size)(resources.GetObject("ctlTextDelimitedFileSave.Size")));
			this.ctlTextDelimitedFileSave.TabIndex = ((int)(resources.GetObject("ctlTextDelimitedFileSave.TabIndex")));
			this.ctlTextDelimitedFileSave.Visible = ((bool)(resources.GetObject("ctlTextDelimitedFileSave.Visible")));
			// 
			// frmTextDelimitedFileSave
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.ctlTextDelimitedFileSave);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "frmTextDelimitedFileSave";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Controls.SetChildIndex(this.ctlTextDelimitedFileSave, 0);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
