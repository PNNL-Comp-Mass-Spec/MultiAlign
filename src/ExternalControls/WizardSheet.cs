using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace Wizard.UI
{
	/// <summary>
	/// Summary description for WizardSheet.
	/// </summary>
	public class WizardSheet : System.Windows.Forms.Form
    {
        #region Members
        private Wizard.UI.EtchedLine etchedLine1;
        private Button backButton;
        private Button nextButton;
        private Button finishButton;
        private Button cancelButton;
        private Panel buttonPanel;
        private Panel pagePanel;
        private Button extraButton;
        private IList _pages = new ArrayList();
        private WizardPage _activePage;
        #endregion
        private ExternalControls.controlStepOverview mcontrol_overallSteps;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public WizardSheet()
		{
			InitializeComponent();
        }

        #region Windows Generated 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardSheet));
            this.backButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.mcontrol_overallSteps = new ExternalControls.controlStepOverview();
            this.extraButton = new System.Windows.Forms.Button();
            this.etchedLine1 = new Wizard.UI.EtchedLine();
            this.pagePanel = new System.Windows.Forms.Panel();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // backButton
            // 
            this.backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.backButton.Location = new System.Drawing.Point(500, 44);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 0;
            this.backButton.Text = "< &Back";
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.nextButton.Location = new System.Drawing.Point(581, 44);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(68, 23);
            this.nextButton.TabIndex = 1;
            this.nextButton.Text = "&Next >";
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // finishButton
            // 
            this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.finishButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.finishButton.Location = new System.Drawing.Point(655, 45);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 2;
            this.finishButton.Text = "&Finish";
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(655, 44);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonPanel.Controls.Add(this.mcontrol_overallSteps);
            this.buttonPanel.Controls.Add(this.extraButton);
            this.buttonPanel.Controls.Add(this.etchedLine1);
            this.buttonPanel.Controls.Add(this.cancelButton);
            this.buttonPanel.Controls.Add(this.backButton);
            this.buttonPanel.Controls.Add(this.finishButton);
            this.buttonPanel.Controls.Add(this.nextButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 569);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(733, 75);
            this.buttonPanel.TabIndex = 4;
            // 
            // mcontrol_overallSteps
            // 
            this.mcontrol_overallSteps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcontrol_overallSteps.Location = new System.Drawing.Point(12, 6);
            this.mcontrol_overallSteps.Name = "mcontrol_overallSteps";
            this.mcontrol_overallSteps.Size = new System.Drawing.Size(718, 33);
            this.mcontrol_overallSteps.TabIndex = 6;
            // 
            // extraButton
            // 
            this.extraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.extraButton.BackColor = System.Drawing.SystemColors.Control;
            this.extraButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extraButton.Location = new System.Drawing.Point(12, 43);
            this.extraButton.Name = "extraButton";
            this.extraButton.Size = new System.Drawing.Size(68, 24);
            this.extraButton.TabIndex = 5;
            this.extraButton.Text = "Extra";
            this.extraButton.UseVisualStyleBackColor = false;
            this.extraButton.Visible = false;
            this.extraButton.Click += new System.EventHandler(this.extraButton_Click);
            // 
            // etchedLine1
            // 
            this.etchedLine1.Dock = System.Windows.Forms.DockStyle.Top;
            this.etchedLine1.Edge = Wizard.UI.EtchEdge.Top;
            this.etchedLine1.Location = new System.Drawing.Point(0, 0);
            this.etchedLine1.Name = "etchedLine1";
            this.etchedLine1.Size = new System.Drawing.Size(733, 8);
            this.etchedLine1.TabIndex = 4;
            // 
            // pagePanel
            // 
            this.pagePanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pagePanel.Location = new System.Drawing.Point(0, 0);
            this.pagePanel.Name = "pagePanel";
            this.pagePanel.Size = new System.Drawing.Size(733, 569);
            this.pagePanel.TabIndex = 5;
            // 
            // WizardSheet
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(733, 644);
            this.Controls.Add(this.pagePanel);
            this.Controls.Add(this.buttonPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WizardSheet";
            this.Text = "WizardSheet";
            this.Load += new System.EventHandler(this.WizardSheet_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.WizardSheet_Closing);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
        #endregion

        #region Wizard Events 
        protected virtual void OnQueryCancel(CancelEventArgs e)
        {
            QueryCancel(e);
        }

        private void QueryCancel(CancelEventArgs e)
        {
            _activePage.OnQueryCancel(e);
        }		
        private void WizardSheet_Load(object sender, System.EventArgs e)
		{
			if (_pages.Count != 0)
			{
				ResizeToFit();
				SetActivePage(0);
			}
			else
				SetWizardButtons(WizardButtons.None);
		}
		private void WizardSheet_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!cancelButton.Enabled)
                e.Cancel = true;
            else if (!finishButton.Enabled)
                OnQueryCancel(e);
        }
        #endregion

        #region Page Change Methods
        private void ResizeToFit()
        {
            Size maxPageSize = new Size(buttonPanel.Width, 0);

            foreach (WizardPage page in _pages)
            {
                if (page.Width > maxPageSize.Width)
                    maxPageSize.Width = page.Width;
                if (page.Height > maxPageSize.Height)
                    maxPageSize.Height = page.Height;
            }

            foreach (WizardPage page in _pages)
            {
                page.Size = maxPageSize;
            }

            Size extraSize = this.Size;
            extraSize -= pagePanel.Size;

            Size newSize = maxPageSize + extraSize;
            this.Size = newSize;
        }        
        public IList        Pages
		{
			get { return _pages; }
		}
		private int         GetActiveIndex()
		{
			WizardPage activePage = GetActivePage();

			for (int i = 0; i < _pages.Count; ++i)
			{
				if (activePage == _pages[i])
					return i;
			}

			return -1;
		}
		private WizardPage  GetActivePage()
		{
			return _activePage;
		}	
		private WizardPage FindPage(string pageName)
		{
			foreach (WizardPage page in _pages)
			{
				if (page.Name == pageName)
					return page;
			}

			return null;
		}
        public  void  SetActivePage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _pages.Count)
                throw new ArgumentOutOfRangeException("pageIndex");

            WizardPage page = (WizardPage)_pages[pageIndex];
            SetActivePage(page);
        }
        private void  SetActivePage(string newPageName)
		{
			WizardPage newPage = FindPage(newPageName);

			if (newPage == null)
				throw new Exception(string.Format("Can't find page named {0}", newPageName));

			SetActivePage(newPage);
		}
        
        /// <summary>
        /// Displays the list of steps that will be performed.
        /// </summary>
        /// <param name="steps"></param>
        public void DisplayListOfSteps(List<string> steps)
        {
            mcontrol_overallSteps.DisplayListOfSteps(steps);
        }

        /// <summary>
        /// Displays the current step.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="stepName"></param>
        public void SetStep(int index, string stepName)
        {
            mcontrol_overallSteps.SetStep(index, stepName);
        }

		private void  SetActivePage(WizardPage newPage)
		{
			WizardPage oldActivePage = _activePage;

			// If this page isn't in the Controls collection, add it.
			// This is what causes the Load event, so we defer
			// it as late as possible.
			if (!pagePanel.Controls.Contains(newPage))
				pagePanel.Controls.Add(newPage);
			
			// Show this page.
			newPage.Visible = true;

			_activePage = newPage;
			CancelEventArgs e = new CancelEventArgs();
			newPage.OnSetActive(e);

			if (e.Cancel)
			{
				newPage.Visible = false;
				_activePage = oldActivePage;
			}

			// Hide all of the other pages.
			foreach (WizardPage page in _pages)
			{
				if (page != _activePage)
					page.Visible = false;
			}
		}
		internal void SetWizardButtons(WizardButtons buttons)
		{
			// The Back button is simple.
			backButton.Enabled = ((buttons & WizardButtons.Back) != 0);

			// The Next button is a bit more complicated. If we've got a Finish button, then it's disabled and hidden.
			if ((buttons & WizardButtons.Finish) != 0)
			{
				finishButton.Visible = true;
				finishButton.Enabled = true;

				nextButton.Visible = false;
				nextButton.Enabled = false;

				this.AcceptButton = finishButton;
			}
			else
			{
				finishButton.Visible = false;
				finishButton.Enabled = false;

				nextButton.Visible = true;
				nextButton.Enabled = ((buttons & WizardButtons.Next) != 0);

				this.AcceptButton = nextButton;
			}
		}
		private WizardPageEventArgs PreChangePage(int delta)
		{
			// Figure out which page is next.
			int activeIndex = GetActiveIndex();
			int nextIndex = activeIndex + delta;

			if (nextIndex < 0 || nextIndex >= _pages.Count)
				nextIndex = activeIndex;

			// Fill in the event args.
			WizardPage newPage = (WizardPage)_pages[nextIndex];

			WizardPageEventArgs e = new WizardPageEventArgs();
			e.NewPage = newPage.Name;
			e.Cancel = false;

			return e;
		}
		private void PostChangePage(WizardPageEventArgs e)
		{
			if (!e.Cancel)
				SetActivePage(e.NewPage);
        }
        #endregion

        #region Button Handlers
        private void nextButton_Click(object sender, System.EventArgs e)
		{
			WizardPageEventArgs wpea = PreChangePage(+1);
			_activePage.OnWizardNext(wpea);
			PostChangePage(wpea);
		}
		private void backButton_Click(object sender, System.EventArgs e)
		{
			WizardPageEventArgs wpea = PreChangePage(-1);
			_activePage.OnWizardBack(wpea);
			PostChangePage(wpea);
		}
		private void finishButton_Click(object sender, System.EventArgs e)
		{
			CancelEventArgs cea = new CancelEventArgs();
			_activePage.OnWizardFinish(cea);
			if (cea.Cancel)
				return;

			this.DialogResult = DialogResult.OK;
			this.Close();
        }
        private void extraButton_Click(object sender, System.EventArgs e)
        {
            _activePage.OnWizardExtra(e);
        }
        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            /// 
            /// Make sure the user wants to close.
            /// 
            CancelEventArgs args = new CancelEventArgs(false);                        
            QueryCancel(args);

            if (args.Cancel == false)
            {
                this.Close();
            }
        }
        internal void PressButton(WizardButtons buttons)
        {
            if ((buttons & WizardButtons.Finish) == WizardButtons.Finish)
                finishButton.PerformClick();
            else if ((buttons & WizardButtons.Next) == WizardButtons.Next)
                nextButton.PerformClick();
            else if ((buttons & WizardButtons.Back) == WizardButtons.Back)
                backButton.PerformClick();
        }    
        #endregion
                                   
        #region Back Button
        internal bool BackButtonEnabled
        {
            get
            {
                return backButton.Enabled;
            }
            set
            {
                backButton.Enabled = value;
            }
            
        }
        internal bool BackButtonVisible
        {
            get
            {
                return backButton.Visible;
            }
            set
            {
                backButton.Visible = value;
            }
        }
        internal string BackButtonText
        {
            get
            {
                return backButton.Text;
            }
            set
            {
                backButton.Text = value;
            }
        }
        #endregion

        #region Next Button
        internal bool NextButtonEnabled
        {
            get
            {
                return nextButton.Enabled;
            }
            set
            {
                nextButton.Enabled = value;
            }

        }
        internal bool NextButtonVisible
        {
            get
            {
                return nextButton.Visible;
            }
            set
            {
                nextButton.Visible = value;
            }
        }
        internal string NextButtonText
        {
            get
            {
                return nextButton.Text;
            }
            set
            {
                nextButton.Text = value;
            }
        }
        #endregion

        #region Cancel Button
        internal bool CancelButtonEnabled
        {
            get
            {
                return cancelButton.Enabled;
            }
            set
            {
                cancelButton.Enabled = value;
            }

        }
        internal bool CancelButtonVisible
        {
            get
            {
                return cancelButton.Visible;
            }
            set
            {
                cancelButton.Visible = value;
            }
        }
        internal string CancelButtonText
        {
            get
            {
                return cancelButton.Text;
            }
            set
            {
                cancelButton.Text = value;
            }
        }
        #endregion

        #region Extra Button
        internal bool ExtraButtonEnabled
        {
            get
            {
                return extraButton.Enabled;
            }
            set
            {
                extraButton.Enabled = value;
            }

        }
        internal bool ExtraButtonVisible
        {
            get
            {
                return extraButton.Visible;
            }
            set
            {
                extraButton.Visible = value;
            }
        }
        internal string ExtraButtonText
        {
            get
            {
                return extraButton.Text;
            }
            set
            {
                extraButton.Text = value;
            }
        }
        #endregion

        #region Finish Button
        internal bool FinishButtonEnabled
        {
            get
            {
                return finishButton.Enabled;
            }
            set
            {
                finishButton.Enabled = value;
            }

        }
        internal bool FinishButtonVisible
        {
            get
            {
                return finishButton.Visible;
            }
            set
            {
                finishButton.Visible = value;
            }
        }
        internal string FinishButtonText
        {
            get
            {
                return finishButton.Text;
            }
            set
            {
                finishButton.Text = value;
            }
        }
        #endregion


        //#region Next Button
        //internal void EnableNextButton(bool enableNextButton)
        //{
        //    nextButton.Enabled = enableNextButton;
        //}
        //public string SetNextButtonString
        //{
        //    set
        //    {
        //        this.nextButton.Text = value;
        //    }
        //}				
        //#endregion

        //#region Finish Button
        //internal bool FinishButtonVisible
        //{
        //    set
        //    {
        //        finishButton.Visible = value;
        //    }
        //}
        //#endregion

        //#region Cancel Button
        //internal void EnableCancelButton(bool enableCancelButton)
        //{
        //    cancelButton.Enabled = enableCancelButton;
        //}
        //#endregion

        //#region Extra Button
        //public bool ShowExtraButton
        //{
        //    set
        //    {
        //        this.extraButton.Visible = value;
        //    }
        //}
        //public string SetExtraButtonString
        //{
        //    set
        //    {
        //        this.extraButton.Text = value;
        //    }
        //}
        //internal void EnableExtraButton(bool enableExtraButton) //Ashoka
        //{
        //    extraButton.Enabled = enableExtraButton;
        //}
       
        //#endregion


    }

	[Flags]
	public enum WizardButtons
	{
		None = 0x0000,
		Back = 0x0001,
		Next = 0x0002,
		Finish = 0x0004,
	}
}
