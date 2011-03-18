using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Wizard.UI
{
    public partial class WizardPage : UserControl
    {
        public WizardPage()
        {
            InitializeComponent();
        }


        #region Wizard Buttons
        protected WizardSheet GetWizard()
        {
            return ParentForm as WizardSheet;
        }
        protected void SetWizardButtons(WizardButtons buttons)
        {
            GetWizard().SetWizardButtons(buttons);
        }
        protected void PressButton(WizardButtons buttons)
        {
            GetWizard().PressButton(buttons);
        }

        #region Back Button
        public bool BackButtonEnabled
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().BackButtonEnabled;
            }
            set
            {
                GetWizard().BackButtonEnabled = value;
            }

        }
        public bool BackButtonVisible
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().BackButtonVisible;
            }
            set
            {
                GetWizard().BackButtonVisible = value;
            }
        }
        public string BackButtonText
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return "";
                }

                return GetWizard().BackButtonText;
            }
            set
            {
                GetWizard().BackButtonText = value;
            }
        }
        #endregion

        #region Next Button
        public bool NextButtonEnabled
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().NextButtonEnabled;
            }
            set
            {
                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.NextButtonEnabled = value;
                }
            }

        }
        public bool NextButtonVisible
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().NextButtonVisible;
            }
            set
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    GetWizard().NextButtonVisible = value;
                }
            }
        }
        public string NextButtonText
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return "";
                }

                return GetWizard().NextButtonText;
            }
            set
            {

                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    GetWizard().NextButtonText = value;
                }
            }
        }
        #endregion

        #region Cancel Button
        public bool CancelButtonEnabled
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().CancelButtonEnabled;
            }
            set
            {
                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.CancelButtonEnabled = value;
                }
            }

        }
        public bool CancelButtonVisible
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().CancelButtonVisible;
            }
            set
            {
                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.CancelButtonVisible = value;
                }
            }
        }
        public string CancelButtonText
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return "";
                }

                return GetWizard().CancelButtonText;
            }
            set
            {

                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.CancelButtonText = value;
                }
            }
        }
        #endregion

        #region Extra Button
        public bool ExtraButtonEnabled
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().ExtraButtonEnabled;
            }
            set
            {

                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.ExtraButtonEnabled = value;
                }
            }

        }
        public bool ExtraButtonVisible
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().ExtraButtonVisible;
            }
            set
            {
                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.ExtraButtonVisible = value;
                }
            }
        }
        public string ExtraButtonText
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return "";
                }

                return GetWizard().ExtraButtonText;
            }
            set
            {

                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.ExtraButtonText = value;
                }
            }
        }
        #endregion

        #region Finish Button
        public bool FinishButtonEnabled
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().FinishButtonEnabled;
            }
            set
            {

                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.FinishButtonEnabled = value;
                }
            }

        }
        public bool FinishButtonVisible
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return false;
                }

                return GetWizard().FinishButtonVisible;
            }
            set
            {

                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.FinishButtonVisible = value;
                }
            }
        }
        public string FinishButtonText
        {
            get
            {
                WizardSheet sheet = GetWizard();
                if (sheet == null)
                {
                    return "";
                }

                return GetWizard().FinishButtonText;
            }
            set
            {

                WizardSheet sheet = GetWizard();
                if (sheet != null)
                {
                    sheet.FinishButtonText = value;
                }
            }
        }
        #endregion

        #endregion

        [Category("Wizard")]
        public event CancelEventHandler SetActive;
        public virtual void OnSetActive(CancelEventArgs e)
        {
            if (SetActive != null)
                SetActive(this, e);
        }
        [Category("Wizard")]
        public event WizardPageEventHandler WizardNext;
        public virtual void OnWizardNext(WizardPageEventArgs e)
        {
            if (WizardNext != null)
                WizardNext(this, e);
        }
        [Category("Wizard")]
        public event WizardPageEventHandler WizardBack;
        public virtual void OnWizardBack(WizardPageEventArgs e)
        {
            if (WizardBack != null)
                WizardBack(this, e);
        }
        [Category("Wizard")]
        public event CancelEventHandler WizardFinish;
        public virtual void OnWizardFinish(CancelEventArgs e)
        {
            if (WizardFinish != null)
                WizardFinish(this, e);
        }
        [Category("Wizard")]
        public event CancelEventHandler QueryCancel;
        public virtual void OnQueryCancel(CancelEventArgs e)
        {
            if (QueryCancel != null)
                QueryCancel(this, e);
        }
        [Category("Wizard")]
        public event System.EventHandler WizardExtra;
        public virtual void OnWizardExtra(EventArgs e)
        {
            if (WizardExtra != null)
                WizardExtra(this, e);
        }
    }
}
