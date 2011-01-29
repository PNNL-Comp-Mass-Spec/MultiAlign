using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using PNNLProteomics.EventModel;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmStatus.
	/// </summary>
	public class frmStatus : System.Windows.Forms.Form
    {
		private System.Windows.Forms.ProgressBar mbar_progress;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button mbtn_cancel;

		private System.Windows.Forms.Label mlbl_status;

		private int mint_percent_done = 0 ;
        private int mint_step_size = 2; 		

		/// <summary>
		/// Delegate provided to external classes to set percent complete
		/// </summary>
		public DelegateSetPercentComplete mevntPercentComplete ; 
		/// <summary>
		/// Delegate provided to external classes to set status message
		/// </summary>
		public DelegateSetStatusMessage mevntStatusMessage ; 
		/// <summary>
		/// Delegate provided to external classes to set form's title
		/// </summary>
		public DelegateSetStatusMessage mevntTitleMessage ; 

		/// <summary>
		/// private delegate called on Invoke for setting percent complete.
		/// </summary>
		private DelegateSetPercentComplete mevntPercentCompletePriv ; 
		/// <summary>
		/// private delegate called using Invoke to set status message.
		/// </summary>
		private DelegateSetStatusMessage mevntStatusMessagePriv ; 
		/// <summary>
		/// private delegate called using Invoke to set form's title.
		/// </summary>
		private DelegateSetStatusMessage mevntTitleMessagePriv ;
        private Label mlabel_percentComplete; 


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void Init()
		{
			mbar_progress.Minimum = 0 ; 
			mbar_progress.Maximum = 100 ; 
			mbar_progress.Step = mint_step_size ;

			mevntStatusMessage = new DelegateSetStatusMessage(this.UpdateStatusMessageInv) ; 
			mevntTitleMessage = new DelegateSetStatusMessage(this.UpdateFormTitleInv) ; 
			mevntPercentComplete = new DelegateSetPercentComplete(this.UpdatePercentCompleteInv) ; 

			mevntStatusMessagePriv = new DelegateSetStatusMessage(this.UpdateStatusMessage) ; 
			mevntTitleMessagePriv = new DelegateSetStatusMessage(this.UpdateFormTitle) ; 
			mevntPercentCompletePriv = new DelegateSetPercentComplete(this.UpdatePercentComplete) ; 
		}

		public frmStatus()
		{
			try
			{
				//
				// Required for Windows Form Designer support
				//
				InitializeComponent();

				//
				// TODO: Add any constructor code after InitializeComponent call
				//
				Init() ; 
			}
			catch (Exception ex)
			{
				
			}

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStatus));
            this.mbar_progress = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.mbtn_cancel = new System.Windows.Forms.Button();
            this.mlbl_status = new System.Windows.Forms.Label();
            this.mlabel_percentComplete = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mbar_progress
            // 
            this.mbar_progress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mbar_progress.Location = new System.Drawing.Point(12, 65);
            this.mbar_progress.Name = "mbar_progress";
            this.mbar_progress.Size = new System.Drawing.Size(402, 26);
            this.mbar_progress.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(402, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "Status:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mbtn_cancel
            // 
            this.mbtn_cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbtn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbtn_cancel.Location = new System.Drawing.Point(168, 145);
            this.mbtn_cancel.Name = "mbtn_cancel";
            this.mbtn_cancel.Size = new System.Drawing.Size(90, 24);
            this.mbtn_cancel.TabIndex = 3;
            this.mbtn_cancel.Text = "Cancel";
            // 
            // mlbl_status
            // 
            this.mlbl_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlbl_status.Location = new System.Drawing.Point(12, 23);
            this.mlbl_status.Name = "mlbl_status";
            this.mlbl_status.Size = new System.Drawing.Size(402, 39);
            this.mlbl_status.TabIndex = 4;
            this.mlbl_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mlabel_percentComplete
            // 
            this.mlabel_percentComplete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_percentComplete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_percentComplete.Location = new System.Drawing.Point(12, 103);
            this.mlabel_percentComplete.Name = "mlabel_percentComplete";
            this.mlabel_percentComplete.Size = new System.Drawing.Size(402, 21);
            this.mlabel_percentComplete.TabIndex = 5;
            this.mlabel_percentComplete.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frmStatus
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.mbtn_cancel;
            this.ClientSize = new System.Drawing.Size(423, 180);
            this.Controls.Add(this.mlabel_percentComplete);
            this.Controls.Add(this.mbar_progress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mlbl_status);
            this.Controls.Add(this.mbtn_cancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStatus";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Status";
            this.ResumeLayout(false);

		}
		#endregion


		#region "Functions for setting title message, status messages, and percentage complete"
		private void UpdatePercentComplete(int val)
		{
			mbar_progress.Value = val ;
            mlabel_percentComplete.Text = string.Format("{0}%", val);
		}
		private void UpdateStatusMessage(int statusLevel, string message)
		{
			this.mlbl_status.Text = message ; 
		}
		private void UpdateFormTitle(int statusLevel, string title)
		{
			try
			{
				this.Text = title ; 
			}
			catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
			}
		}


		#endregion

		#region "Invoke functions of setting title, status and percent complete values"
		private void UpdatePercentCompleteInv(int percent_done)
		{
			try
			{
				if (Math.Abs(percent_done - mint_percent_done) > mint_step_size )
				{
					mint_percent_done = percent_done ; 
					Invoke(mevntPercentCompletePriv, new object []{mint_percent_done}) ; 
				}
			}
			catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
			}
		}
		private void UpdateStatusMessageInv(int messageLevel, string message)
		{
			try
			{
				if (!IsHandleCreated)
					return ; 
				Invoke(mevntStatusMessagePriv, new object [] {messageLevel, message}) ; 
			}
			catch (ApplicationException e)
			{
                System.Diagnostics.Trace.WriteLine(e.Message);				
			}
		}

		private void UpdateFormTitleInv(int statusLevel, string title)
		{
			try
			{
				if (!IsHandleCreated)
					return ; 
				Invoke(mevntTitleMessagePriv, new object [] {statusLevel, title}) ; 
			}
			catch (ApplicationException e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);				 
			}
		}

		#endregion 	
	}
}
