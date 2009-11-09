namespace MultiAlignWin.Diagnostics
{
    partial class frmException
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmException));
            this.txtExceptionMessage = new System.Windows.Forms.RichTextBox();
            this.btnSendErrorReport = new System.Windows.Forms.Button();
            this.labelHeader = new System.Windows.Forms.Label();
            this.panelSideBar = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // txtExceptionMessage
            // 
            this.txtExceptionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptionMessage.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtExceptionMessage.Location = new System.Drawing.Point(12, 33);
            this.txtExceptionMessage.Name = "txtExceptionMessage";
            this.txtExceptionMessage.ReadOnly = true;
            this.txtExceptionMessage.Size = new System.Drawing.Size(760, 529);
            this.txtExceptionMessage.TabIndex = 0;
            this.txtExceptionMessage.Text = "";
            // 
            // btnSendErrorReport
            // 
            this.btnSendErrorReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendErrorReport.BackColor = System.Drawing.SystemColors.Control;
            this.btnSendErrorReport.Enabled = false;
            this.btnSendErrorReport.Location = new System.Drawing.Point(670, 568);
            this.btnSendErrorReport.Name = "btnSendErrorReport";
            this.btnSendErrorReport.Size = new System.Drawing.Size(102, 20);
            this.btnSendErrorReport.TabIndex = 4;
            this.btnSendErrorReport.Text = "Send Error Report";
            this.btnSendErrorReport.UseVisualStyleBackColor = false;
            this.btnSendErrorReport.Visible = false;
            // 
            // labelHeader
            // 
            this.labelHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHeader.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.Location = new System.Drawing.Point(12, 6);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(760, 24);
            this.labelHeader.TabIndex = 2;
            this.labelHeader.Text = "MultiAlign encountered an error and needs to shutdown.";
            this.labelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelSideBar
            // 
            this.panelSideBar.BackgroundImage = global::MultiAlignWin.Properties.Resources.ExampleSidebar;
            this.panelSideBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSideBar.Location = new System.Drawing.Point(779, 0);
            this.panelSideBar.Name = "panelSideBar";
            this.panelSideBar.Size = new System.Drawing.Size(160, 593);
            this.panelSideBar.TabIndex = 5;
            // 
            // frmException
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(939, 593);
            this.Controls.Add(this.panelSideBar);
            this.Controls.Add(this.labelHeader);
            this.Controls.Add(this.txtExceptionMessage);
            this.Controls.Add(this.btnSendErrorReport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmException";
            this.Text = "MultiAlign Error";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtExceptionMessage;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.Button btnSendErrorReport;
        private System.Windows.Forms.Panel panelSideBar;
    }
}