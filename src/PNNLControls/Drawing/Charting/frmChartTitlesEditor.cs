using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmTextEditorDialog.
	/// </summary>
	public class frmChartTitlesEditor : PNNLControls.frmDialogBase
	{
		private PNNLControls.ctlTextEditor mTitleEditor;
		private PNNLControls.ctlTextEditor mXAxisEditor;
		private PNNLControls.ctlTextEditor mYAxisEditor;
		private System.Windows.Forms.Panel mLabelsPanel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmChartTitlesEditor()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmChartTitlesEditor));
			this.mTitleEditor = new PNNLControls.ctlTextEditor();
			this.mXAxisEditor = new PNNLControls.ctlTextEditor();
			this.mYAxisEditor = new PNNLControls.ctlTextEditor();
			this.mLabelsPanel = new System.Windows.Forms.Panel();
			this.mLabelsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mTitleEditor
			// 
			this.mTitleEditor.AccessibleDescription = resources.GetString("mTitleEditor.AccessibleDescription");
			this.mTitleEditor.AccessibleName = resources.GetString("mTitleEditor.AccessibleName");
			this.mTitleEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("mTitleEditor.Anchor")));
			this.mTitleEditor.AutoScroll = ((bool)(resources.GetObject("mTitleEditor.AutoScroll")));
			this.mTitleEditor.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("mTitleEditor.AutoScrollMargin")));
			this.mTitleEditor.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("mTitleEditor.AutoScrollMinSize")));
			this.mTitleEditor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mTitleEditor.BackgroundImage")));
			this.mTitleEditor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("mTitleEditor.Dock")));
			this.mTitleEditor.Enabled = ((bool)(resources.GetObject("mTitleEditor.Enabled")));
			this.mTitleEditor.Font = ((System.Drawing.Font)(resources.GetObject("mTitleEditor.Font")));
			this.mTitleEditor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("mTitleEditor.ImeMode")));
			this.mTitleEditor.Location = ((System.Drawing.Point)(resources.GetObject("mTitleEditor.Location")));
			this.mTitleEditor.Name = "mTitleEditor";
			this.mTitleEditor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mTitleEditor.RightToLeft")));
			this.mTitleEditor.Size = ((System.Drawing.Size)(resources.GetObject("mTitleEditor.Size")));
			this.mTitleEditor.TabIndex = ((int)(resources.GetObject("mTitleEditor.TabIndex")));
			this.mTitleEditor.UserLabel = "Title";
			this.mTitleEditor.UserText = "Text";
			this.mTitleEditor.Visible = ((bool)(resources.GetObject("mTitleEditor.Visible")));
			// 
			// mXAxisEditor
			// 
			this.mXAxisEditor.AccessibleDescription = resources.GetString("mXAxisEditor.AccessibleDescription");
			this.mXAxisEditor.AccessibleName = resources.GetString("mXAxisEditor.AccessibleName");
			this.mXAxisEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("mXAxisEditor.Anchor")));
			this.mXAxisEditor.AutoScroll = ((bool)(resources.GetObject("mXAxisEditor.AutoScroll")));
			this.mXAxisEditor.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("mXAxisEditor.AutoScrollMargin")));
			this.mXAxisEditor.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("mXAxisEditor.AutoScrollMinSize")));
			this.mXAxisEditor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mXAxisEditor.BackgroundImage")));
			this.mXAxisEditor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("mXAxisEditor.Dock")));
			this.mXAxisEditor.Enabled = ((bool)(resources.GetObject("mXAxisEditor.Enabled")));
			this.mXAxisEditor.Font = ((System.Drawing.Font)(resources.GetObject("mXAxisEditor.Font")));
			this.mXAxisEditor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("mXAxisEditor.ImeMode")));
			this.mXAxisEditor.Location = ((System.Drawing.Point)(resources.GetObject("mXAxisEditor.Location")));
			this.mXAxisEditor.Name = "mXAxisEditor";
			this.mXAxisEditor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mXAxisEditor.RightToLeft")));
			this.mXAxisEditor.Size = ((System.Drawing.Size)(resources.GetObject("mXAxisEditor.Size")));
			this.mXAxisEditor.TabIndex = ((int)(resources.GetObject("mXAxisEditor.TabIndex")));
			this.mXAxisEditor.UserLabel = "X Axis Label";
			this.mXAxisEditor.UserText = "Text";
			this.mXAxisEditor.Visible = ((bool)(resources.GetObject("mXAxisEditor.Visible")));
			// 
			// mYAxisEditor
			// 
			this.mYAxisEditor.AccessibleDescription = resources.GetString("mYAxisEditor.AccessibleDescription");
			this.mYAxisEditor.AccessibleName = resources.GetString("mYAxisEditor.AccessibleName");
			this.mYAxisEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("mYAxisEditor.Anchor")));
			this.mYAxisEditor.AutoScroll = ((bool)(resources.GetObject("mYAxisEditor.AutoScroll")));
			this.mYAxisEditor.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("mYAxisEditor.AutoScrollMargin")));
			this.mYAxisEditor.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("mYAxisEditor.AutoScrollMinSize")));
			this.mYAxisEditor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mYAxisEditor.BackgroundImage")));
			this.mYAxisEditor.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("mYAxisEditor.Dock")));
			this.mYAxisEditor.Enabled = ((bool)(resources.GetObject("mYAxisEditor.Enabled")));
			this.mYAxisEditor.Font = ((System.Drawing.Font)(resources.GetObject("mYAxisEditor.Font")));
			this.mYAxisEditor.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("mYAxisEditor.ImeMode")));
			this.mYAxisEditor.Location = ((System.Drawing.Point)(resources.GetObject("mYAxisEditor.Location")));
			this.mYAxisEditor.Name = "mYAxisEditor";
			this.mYAxisEditor.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mYAxisEditor.RightToLeft")));
			this.mYAxisEditor.Size = ((System.Drawing.Size)(resources.GetObject("mYAxisEditor.Size")));
			this.mYAxisEditor.TabIndex = ((int)(resources.GetObject("mYAxisEditor.TabIndex")));
			this.mYAxisEditor.UserLabel = "Y Axis Label";
			this.mYAxisEditor.UserText = "Text";
			this.mYAxisEditor.Visible = ((bool)(resources.GetObject("mYAxisEditor.Visible")));
			// 
			// mLabelsPanel
			// 
			this.mLabelsPanel.AccessibleDescription = resources.GetString("mLabelsPanel.AccessibleDescription");
			this.mLabelsPanel.AccessibleName = resources.GetString("mLabelsPanel.AccessibleName");
			this.mLabelsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("mLabelsPanel.Anchor")));
			this.mLabelsPanel.AutoScroll = ((bool)(resources.GetObject("mLabelsPanel.AutoScroll")));
			this.mLabelsPanel.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("mLabelsPanel.AutoScrollMargin")));
			this.mLabelsPanel.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("mLabelsPanel.AutoScrollMinSize")));
			this.mLabelsPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mLabelsPanel.BackgroundImage")));
			this.mLabelsPanel.Controls.Add(this.mTitleEditor);
			this.mLabelsPanel.Controls.Add(this.mXAxisEditor);
			this.mLabelsPanel.Controls.Add(this.mYAxisEditor);
			this.mLabelsPanel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("mLabelsPanel.Dock")));
			this.mLabelsPanel.Enabled = ((bool)(resources.GetObject("mLabelsPanel.Enabled")));
			this.mLabelsPanel.Font = ((System.Drawing.Font)(resources.GetObject("mLabelsPanel.Font")));
			this.mLabelsPanel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("mLabelsPanel.ImeMode")));
			this.mLabelsPanel.Location = ((System.Drawing.Point)(resources.GetObject("mLabelsPanel.Location")));
			this.mLabelsPanel.Name = "mLabelsPanel";
			this.mLabelsPanel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mLabelsPanel.RightToLeft")));
			this.mLabelsPanel.Size = ((System.Drawing.Size)(resources.GetObject("mLabelsPanel.Size")));
			this.mLabelsPanel.TabIndex = ((int)(resources.GetObject("mLabelsPanel.TabIndex")));
			this.mLabelsPanel.Text = resources.GetString("mLabelsPanel.Text");
			this.mLabelsPanel.Visible = ((bool)(resources.GetObject("mLabelsPanel.Visible")));
			// 
			// frmChartTitlesEditor
			// 
			this.AcceptButton = null;
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.mLabelsPanel);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "frmChartTitlesEditor";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Controls.SetChildIndex(this.mLabelsPanel, 0);
			this.mLabelsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			if (this.Created) 
			{
				int height = this.mLabelsPanel.Height / 3;
				this.mTitleEditor.Height = height;
				this.mXAxisEditor.Height = height;
				this.mYAxisEditor.Height = height;
				this.mTitleEditor.Top = 0;
				this.mXAxisEditor.Top = height * 1;
				this.mYAxisEditor.Top = height * 2;
			}
		}

		public String Title 
		{
			get 
			{
				return this.mTitleEditor.UserText;
			}
			set 
			{
				this.mTitleEditor.UserText = value;
			}
		}

		public String XAxisLabel 
		{
			get 
			{
				return this.mXAxisEditor.UserText;
			}
			set 
			{
				this.mXAxisEditor.UserText = value;
			}
		}

		public String YAxisLabel 
		{
			get
			{
				return this.mYAxisEditor.UserText;
			}
			set 
			{
				this.mYAxisEditor.UserText = value;
			}
		}
	}
}
