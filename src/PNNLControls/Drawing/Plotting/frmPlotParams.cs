using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmPlotParams.
	/// </summary>
	public class frmPlotParams : PNNLControls.frmDialogBase
	{
		private PNNLControls.ctlPlotParamsEditor mPlotParamsEditor;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmPlotParams()
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
			this.mPlotParamsEditor = new PNNLControls.ctlPlotParamsEditor();
			this.SuspendLayout();
			// 
			// mPlotParamsEditor
			// 
			this.mPlotParamsEditor.AutoScroll = true;
			this.mPlotParamsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mPlotParamsEditor.Location = new System.Drawing.Point(0, 0);
			this.mPlotParamsEditor.Name = "mPlotParamsEditor";
			this.mPlotParamsEditor.PlotParams = null;
			this.mPlotParamsEditor.Size = new System.Drawing.Size(360, 573);
			this.mPlotParamsEditor.TabIndex = 0;
			// 
			// frmPlotParams
			// 
			this.AcceptButton = null;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(360, 621);
			this.Controls.Add(this.mPlotParamsEditor);
			this.Name = "frmPlotParams";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Series Display Editor";
			this.Controls.SetChildIndex(this.mPlotParamsEditor, 0);
			this.ResumeLayout(false);

		}
		#endregion

		public clsPlotParams PlotParams 
		{
			get 
			{
				return this.mPlotParamsEditor.PlotParams;
			}
			set 
			{
				this.mPlotParamsEditor.PlotParams = value;
			}
		}
	}
}
