using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmTracWebBugReport.
	/// </summary>
	public class frmTracWebBugReport : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlUserInfo;
		private System.Windows.Forms.Panel pnlDescription;
		private System.Windows.Forms.Panel pnlButtons;
		private System.Windows.Forms.GroupBox grpBoxUserInfo;
		private System.Windows.Forms.Label lblEmail;
		private System.Windows.Forms.Label lblRqstType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox grpBoxFullDes;
		private System.Windows.Forms.TextBox mtxtBoxSummary;
		private System.Windows.Forms.TextBox mtxtBoxEmail;
		private System.Windows.Forms.ComboBox mcmbBoxRequest;
		private System.Windows.Forms.Label mlblDisclaimer;
		private System.Windows.Forms.RichTextBox mrichTxtBoxFull;
		private System.Windows.Forms.Label mlblbtns;
		private System.Windows.Forms.Button mbtnSend;
		private System.Windows.Forms.Button mbtnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private string MultiAlignSite = @"http://prismtrac.pnl.gov/multialignwin/" ;
		private string mstrUserEmail ;
		private string mstrShortSummary ;
		private string mstrDescription ;
		private System.Windows.Forms.Label label2;
		private string version = "1.0" ;

		public frmTracWebBugReport()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmTracWebBugReport));
			this.pnlUserInfo = new System.Windows.Forms.Panel();
			this.grpBoxFullDes = new System.Windows.Forms.GroupBox();
			this.mrichTxtBoxFull = new System.Windows.Forms.RichTextBox();
			this.pnlDescription = new System.Windows.Forms.Panel();
			this.grpBoxUserInfo = new System.Windows.Forms.GroupBox();
			this.mlblDisclaimer = new System.Windows.Forms.Label();
			this.mcmbBoxRequest = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lblRqstType = new System.Windows.Forms.Label();
			this.lblEmail = new System.Windows.Forms.Label();
			this.mtxtBoxSummary = new System.Windows.Forms.TextBox();
			this.mtxtBoxEmail = new System.Windows.Forms.TextBox();
			this.pnlButtons = new System.Windows.Forms.Panel();
			this.mbtnCancel = new System.Windows.Forms.Button();
			this.mbtnSend = new System.Windows.Forms.Button();
			this.mlblbtns = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.pnlUserInfo.SuspendLayout();
			this.grpBoxFullDes.SuspendLayout();
			this.pnlDescription.SuspendLayout();
			this.grpBoxUserInfo.SuspendLayout();
			this.pnlButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlUserInfo
			// 
			this.pnlUserInfo.Controls.Add(this.grpBoxFullDes);
			this.pnlUserInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlUserInfo.Location = new System.Drawing.Point(0, 144);
			this.pnlUserInfo.Name = "pnlUserInfo";
			this.pnlUserInfo.Size = new System.Drawing.Size(552, 319);
			this.pnlUserInfo.TabIndex = 0;
			// 
			// grpBoxFullDes
			// 
			this.grpBoxFullDes.Controls.Add(this.mrichTxtBoxFull);
			this.grpBoxFullDes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpBoxFullDes.Location = new System.Drawing.Point(0, 0);
			this.grpBoxFullDes.Name = "grpBoxFullDes";
			this.grpBoxFullDes.Size = new System.Drawing.Size(552, 319);
			this.grpBoxFullDes.TabIndex = 1;
			this.grpBoxFullDes.TabStop = false;
			this.grpBoxFullDes.Text = "Full Description";
			// 
			// mrichTxtBoxFull
			// 
			this.mrichTxtBoxFull.Location = new System.Drawing.Point(8, 24);
			this.mrichTxtBoxFull.Name = "mrichTxtBoxFull";
			this.mrichTxtBoxFull.Size = new System.Drawing.Size(536, 240);
			this.mrichTxtBoxFull.TabIndex = 3;
			this.mrichTxtBoxFull.Text = "";
			// 
			// pnlDescription
			// 
			this.pnlDescription.Controls.Add(this.grpBoxUserInfo);
			this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlDescription.Location = new System.Drawing.Point(0, 0);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(552, 144);
			this.pnlDescription.TabIndex = 1;
			// 
			// grpBoxUserInfo
			// 
			this.grpBoxUserInfo.Controls.Add(this.mlblDisclaimer);
			this.grpBoxUserInfo.Controls.Add(this.mcmbBoxRequest);
			this.grpBoxUserInfo.Controls.Add(this.label1);
			this.grpBoxUserInfo.Controls.Add(this.lblRqstType);
			this.grpBoxUserInfo.Controls.Add(this.lblEmail);
			this.grpBoxUserInfo.Controls.Add(this.mtxtBoxSummary);
			this.grpBoxUserInfo.Controls.Add(this.mtxtBoxEmail);
			this.grpBoxUserInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpBoxUserInfo.Location = new System.Drawing.Point(0, 0);
			this.grpBoxUserInfo.Name = "grpBoxUserInfo";
			this.grpBoxUserInfo.Size = new System.Drawing.Size(552, 144);
			this.grpBoxUserInfo.TabIndex = 0;
			this.grpBoxUserInfo.TabStop = false;
			this.grpBoxUserInfo.Text = "User Information and Summary";
			// 
			// mlblDisclaimer
			// 
			this.mlblDisclaimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mlblDisclaimer.Location = new System.Drawing.Point(8, 104);
			this.mlblDisclaimer.Name = "mlblDisclaimer";
			this.mlblDisclaimer.Size = new System.Drawing.Size(536, 23);
			this.mlblDisclaimer.TabIndex = 6;
			this.mlblDisclaimer.Text = "Your personal information will be used only for the purpose of providing an ident" +
				"ifier for your bug report, as well as providing a contact address for the develo" +
				"per in case of further questions.";
			this.mlblDisclaimer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// mcmbBoxRequest
			// 
			this.mcmbBoxRequest.Items.AddRange(new object[] {
																"Bug Report",
																"Feature Request"});
			this.mcmbBoxRequest.Location = new System.Drawing.Point(288, 32);
			this.mcmbBoxRequest.Name = "mcmbBoxRequest";
			this.mcmbBoxRequest.Size = new System.Drawing.Size(256, 21);
			this.mcmbBoxRequest.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Short Summary of the Issue";
			// 
			// lblRqstType
			// 
			this.lblRqstType.Location = new System.Drawing.Point(288, 16);
			this.lblRqstType.Name = "lblRqstType";
			this.lblRqstType.Size = new System.Drawing.Size(144, 16);
			this.lblRqstType.TabIndex = 3;
			this.lblRqstType.Text = "Type of Request";
			// 
			// lblEmail
			// 
			this.lblEmail.Location = new System.Drawing.Point(8, 16);
			this.lblEmail.Name = "lblEmail";
			this.lblEmail.Size = new System.Drawing.Size(144, 16);
			this.lblEmail.TabIndex = 2;
			this.lblEmail.Text = "Your Email Address";
			// 
			// mtxtBoxSummary
			// 
			this.mtxtBoxSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mtxtBoxSummary.Location = new System.Drawing.Point(8, 72);
			this.mtxtBoxSummary.Name = "mtxtBoxSummary";
			this.mtxtBoxSummary.Size = new System.Drawing.Size(536, 20);
			this.mtxtBoxSummary.TabIndex = 2;
			this.mtxtBoxSummary.Text = "";
			// 
			// mtxtBoxEmail
			// 
			this.mtxtBoxEmail.Location = new System.Drawing.Point(8, 32);
			this.mtxtBoxEmail.Name = "mtxtBoxEmail";
			this.mtxtBoxEmail.Size = new System.Drawing.Size(264, 20);
			this.mtxtBoxEmail.TabIndex = 0;
			this.mtxtBoxEmail.Text = "";
			// 
			// pnlButtons
			// 
			this.pnlButtons.Controls.Add(this.label2);
			this.pnlButtons.Controls.Add(this.mbtnCancel);
			this.pnlButtons.Controls.Add(this.mbtnSend);
			this.pnlButtons.Controls.Add(this.mlblbtns);
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlButtons.Location = new System.Drawing.Point(0, 463);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(552, 80);
			this.pnlButtons.TabIndex = 2;
			// 
			// mbtnCancel
			// 
			this.mbtnCancel.Location = new System.Drawing.Point(440, 48);
			this.mbtnCancel.Name = "mbtnCancel";
			this.mbtnCancel.Size = new System.Drawing.Size(80, 23);
			this.mbtnCancel.TabIndex = 5;
			this.mbtnCancel.Text = "Cancel";
			this.mbtnCancel.Click += new System.EventHandler(this.mbtnCancel_Click);
			// 
			// mbtnSend
			// 
			this.mbtnSend.Location = new System.Drawing.Point(344, 48);
			this.mbtnSend.Name = "mbtnSend";
			this.mbtnSend.Size = new System.Drawing.Size(80, 23);
			this.mbtnSend.TabIndex = 4;
			this.mbtnSend.Text = "Send Report";
			this.mbtnSend.Click += new System.EventHandler(this.mbtnSend_Click);
			// 
			// mlblbtns
			// 
			this.mlblbtns.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mlblbtns.Location = new System.Drawing.Point(8, 8);
			this.mlblbtns.Name = "mlblbtns";
			this.mlblbtns.Size = new System.Drawing.Size(320, 40);
			this.mlblbtns.TabIndex = 0;
			this.mlblbtns.Text = "Clicking \'Send Report\' will open a new web browser window to the MultiAlign Websi" +
				"te <https://prismtrac.pnl.gov/multialignwin> and fill in a request ticket for yo" +
				"u to submit.";
			this.mlblbtns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(8, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(320, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Available only within PNNL network.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// frmTracWebBugReport
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(552, 543);
			this.Controls.Add(this.pnlUserInfo);
			this.Controls.Add(this.pnlDescription);
			this.Controls.Add(this.pnlButtons);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmTracWebBugReport";
			this.Text = "Report Bugs / Features to Developers";
			this.Load += new System.EventHandler(this.frmTracWebBugReport_Load);
			this.pnlUserInfo.ResumeLayout(false);
			this.grpBoxFullDes.ResumeLayout(false);
			this.pnlDescription.ResumeLayout(false);
			this.grpBoxUserInfo.ResumeLayout(false);
			this.pnlButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmTracWebBugReport_Load(object sender, System.EventArgs e)
		{
			this.mcmbBoxRequest.Text = "Bug Report";
			this.mcmbBoxRequest.SelectedIndex = 0 ;
			if (mstrUserEmail != null && mstrUserEmail.Length > 0)
				mtxtBoxEmail.Text = mstrUserEmail ;
		}

		private void mbtnSend_Click(object sender, System.EventArgs e)
		{
			string summary = mtxtBoxSummary.Text ;
			if (summary == null || summary.Length == 0)
				MessageBox.Show("Specify your problem first!","Problem summary ?") ;
			else
			{
				string tracLink = this.BuildTracLink();
				if (this.WebSiteIsAvailable(MultiAlignSite)) 
				{
					System.Diagnostics.Process.Start(tracLink);
				}
				else 
				{
					MessageBox.Show("Unable to connection to\r\n" + MultiAlignSite +  "\r\n(Are you able to access the PNNL Network?", 
						"No Response from Bug Tracking Site", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				this.DialogResult = DialogResult.OK;
				this.Hide() ;
			}
		}

		private void mbtnCancel_Click(object sender, System.EventArgs e)
		{
			this.Hide() ;
		}

		private bool WebSiteIsAvailable(string linkText) 
		{
			// ----------------------------------------------------
			//  Attempt to get a response from the passed URL
			//  Pass:       linkText    URL to site being checked
			//  Return:     True        The site responded
			//              False       The site did not respond
			// ----------------------------------------------------
			//      Date    Developer            Code Change
			//   ---------- -------------------- ------------------
			//   12/11/2005 G Gilbert            Original code
			// ----------------------------------------------------
			// ----------------------------------------------------
			//  Local Constant/Variable Declarations
			// ----------------------------------------------------
			System.Uri URL_Object = new System.Uri(linkText);
			System.Net.WebRequest URL_WebRequest;
			System.Net.WebResponse URL_WebResponse;
			bool Response_Result;
			// ----------------------------------------------------
			//  Attempt to get a response from the URL
			// ----------------------------------------------------
			try 
			{
				URL_WebRequest = System.Net.WebRequest.Create(URL_Object);
				URL_WebResponse = URL_WebRequest.GetResponse();
				Response_Result = true;
			}
			catch (Exception Any_Error) 
			{
				Console.WriteLine(Any_Error.ToString()) ;
				Response_Result = false;
			}
			URL_WebResponse = null;
			URL_WebRequest = null;
			URL_Object = null;
			return Response_Result;
		}


		private string BuildTracLink() 
		{
			System.Text.StringBuilder addressSB = new System.Text.StringBuilder();
			System.Text.StringBuilder optionsSB = new System.Text.StringBuilder();
			this.mstrUserEmail = this.mtxtBoxEmail.Text;
			this.mstrShortSummary = this.mtxtBoxSummary.Text;
			this.mstrDescription = this.mrichTxtBoxFull.Text;

			addressSB.Append(MultiAlignSite) ;
			if (!addressSB.ToString().EndsWith("/")) 
			{
				addressSB.Append("/") ;
			}
			addressSB.Append("simpleticket") ;

			optionsSB.Append("type=") ;
			switch (this.mcmbBoxRequest.Text) 
			{
				case "Bug Report":
					optionsSB.Append("defect") ;
					break;
				case "Feature Request":
					optionsSB.Append("enhancement") ;
					break;
			}

            optionsSB.Append("&version=" + System.Web.HttpUtility.UrlEncode(version));

			if (this.mstrUserEmail.Length > 0)
			{
				if ((optionsSB.Length > 0) && !optionsSB.ToString().EndsWith("&"))
				{
					optionsSB.Append("&");
				}
				optionsSB.Append("reporter=") ;
				optionsSB.Append(this.mstrUserEmail);
			}

			if (this.mstrShortSummary.Length > 0)
			{
				if ((optionsSB.Length > 0) && !optionsSB.ToString().EndsWith("&"))
				{
					optionsSB.Append("&");
				}
				optionsSB.Append("summary=") ;
                optionsSB.Append(System.Web.HttpUtility.UrlEncode(this.mstrShortSummary));
			}

			if (this.mstrDescription.Length > 0) 
			{
				if ((optionsSB.Length > 0) && !optionsSB.ToString().EndsWith("&"))
				{
					optionsSB.Append("&");
				}
				optionsSB.Append("description=") ;
                optionsSB.Append(System.Web.HttpUtility.UrlEncode(this.mstrDescription));
			}
			string tracLink = addressSB.ToString() + "?" + optionsSB.ToString();
			return tracLink;
		}
		

		
	}
}
