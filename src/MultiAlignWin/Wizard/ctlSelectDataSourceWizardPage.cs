using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

using MultiAlignWin.Network;
using MultiAlignWin.Data.Loaders;

namespace MultiAlignWin
{
	public partial class ctlSelectDataSourceWizardPage : Wizard.UI.InternalWizardPage
	{
		
        #region Members
        /// <summary>
        /// OpenFileDialog that will allow the user to select files to load from the disk.
        /// </summary>
        private OpenFileDialog mobj_openFileDialog;
        /// <summary>
        /// Class that tests the connection to DMS.
        /// </summary>
        private clsDMSConnectionTester      mobj_connectionTester;
        /// <summary>
        /// Loader to load files from disk.
        /// </summary>
        private clsDiskDatasetLoader        mobj_diskLoader;
        /// <summary>
        /// Loader to load files from DMS.
        /// </summary>
        private clsDatabaseDatasetLoader    mobj_dmsLoader;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ctlSelectDataSourceWizardPage()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
					
			InitializeComponent();

            mobj_connectionTester   = new clsDMSConnectionTester();
            mobj_diskLoader         = new clsDiskDatasetLoader();
            mobj_dmsLoader          = new clsDatabaseDatasetLoader();
            mobj_dmsLoader.LoadedDataset    += new DelegateDataSetLoaded(mobj_dmsLoader_LoadedDataset);
            mobj_dmsLoader.LoadingComplete  += new DelegateUpdateLoadingComplete(mobj_dmsLoader_LoadingComplete);
            mobj_dmsLoader.LoadingProgress  += new DelegateUpdateLoadingPercentLoaded(mobj_dmsLoader_LoadingProgress);
            
			SetActive       += new System.ComponentModel.CancelEventHandler(this.ctlSelectDataSourceWizardPage_SetActive);
            VisibleChanged  += new EventHandler(ctlSelectDataSourceWizardPage_VisibleChanged);
        }

        #region DMS Data Loading
        void mobj_dmsLoader_LoadingProgress(double percentLoaded)
        {
            
        }

        void mobj_dmsLoader_LoadingComplete()
        {
            
        }

        void mobj_dmsLoader_LoadedDataset(MultiAlign.clsDatasetInfo dataset)
        {

        }
        #endregion

        /// <summary>
        /// Handles when the wizard page becomes visible and invisible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ctlSelectDataSourceWizardPage_VisibleChanged(object sender, EventArgs e)
        {
                //TODO: Test DMS network connection.
        }   
     
		/// <summary>
		/// Handles when this becomes the active wizard page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void ctlSelectDataSourceWizardPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
		{			
			SetWizardButtons(Wizard.UI.WizardButtons.Back | Wizard.UI.WizardButtons.Next);
		}

	}
}


