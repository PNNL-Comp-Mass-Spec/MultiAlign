/*////////////////////////////////////////////////////////////////////////
 *  File  : clsDiskDatasetLoader.cs
 *  Author: Brian LaMarche
 *  Date  : 3/10/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      Loads MultiAlign datasets from disk.
 * 
 *  Revisions:
 *      9-11-2008: 
 *          - Moved code from the user interface to this class.
 *          - Created class.  Moved code from the wizard control page.
 *  
 */
///////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using MultiAlignEngine;

namespace PNNLProteomics.Data.Loaders
{
    public class clsDiskDatasetLoader: IDisposable
    {
        /// <summary>
        /// Fired when the loader finds how many datasets are available.
        /// </summary>
        public event DelegateTotalDatasetsFound         DatasetsFound;
        /// <summary>
        /// Called when a dataset is loaded.
        /// </summary>
        public event DelegateDataSetLoaded              LoadedDataset;
        /// <summary>
        /// Called when the loading progress is updated.
        /// </summary>
        public event DelegateUpdateLoadingPercentLoaded LoadingProgress;
        /// <summary>
        /// Called when the loading of all datasets is complete or if there was a major error.
        /// </summary>
        public event DelegateUpdateLoadingComplete      LoadingComplete;		
        /// <summary>
        /// Array of the names of the files to load.
        /// </summary>
        private string[] marr_fileNames;
        /// <summary>
        /// List of the datasets loaded.
        /// </summary>
        private ArrayList mlist_data;
        /// <summary>
        /// Thread in charge of 
        /// </summary>
        private Thread mobj_thread;
        /// <summary>
        /// Default constructor for loading datasets from disk.
        /// </summary>
        public clsDiskDatasetLoader()
        {
            marr_fileNames  = null;
            mobj_thread     = null;
            mlist_data      = new ArrayList();
            marr_fileNames  = null;
        }
        /// <summary>
        /// Aborts the loading thread if it's currently threaded.
        /// </summary>
        public void Abort()
        {
            try
            {
                if (mobj_thread != null)
                {
                    mobj_thread.Abort();
                }
            }
            catch
            {
            }
            finally
            {
                mobj_thread = null;
            }
        }
        /// <summary>
        /// Load the data from the disk.  Returns a list of datasets loaded.
        /// </summary>
        /// <param name="fileNames">Name of the files to load.</param>
        /// <param name="threaded">Flag indicating whether or not to thread the loading process.  Events indicate loading progress and when it completes.</param>        
        public void LoadData(string [] fileNames, bool threaded)
        {
            mlist_data.Clear();
            if (threaded == true)
            {
                Abort();
                ThreadStart threadStart   = new ThreadStart(LoadDataThread);
                /// 
                /// Copy the file names in case someone does something to the array reference.
                /// 
                marr_fileNames      = new string[fileNames.Length];
                fileNames.CopyTo(marr_fileNames, 0);

                /// 
                /// Create the thread and start it.
                /// 
                mobj_thread = new Thread(threadStart);
                mobj_thread.Start();
            }
            else
            {
                LoadDataThread();
            }
        }
        /// <summary>
        /// Threaded start for loading all of the datasets on disk.        
        /// </summary>
        private void LoadDataThread()
        {
            /// 
            /// Foreach filename create a new dataset class
            /// 
            int numLoaded        = 0;
            double percentLoaded = 0;

            if (DatasetsFound != null)
                DatasetsFound(marr_fileNames.Length);

            foreach(string filePath in marr_fileNames)
            {
                FileInfo info               = new FileInfo(filePath);
                string fileName             = info.Name;
                
                /// 
                /// Build the alias
                ///                 
                int index = fileName.ToLower().LastIndexOf(".pek");
                if (index < 0)
                    index = fileName.ToLower().LastIndexOf("_isos.csv");

                string fileNameAlias = null;
                if (index > 0)
                {
                    fileNameAlias = fileName.Substring(0, index);
                }
                else
                {
                    fileNameAlias = fileName;
                }

                clsDatasetInfo datasetInfo      = new clsDatasetInfo();
                datasetInfo.mstrDatasetName     = fileName;
                datasetInfo.mstrAlias           = fileNameAlias;
                datasetInfo.mstrDatasetId       = "NA";
                datasetInfo.mstrAnalysisJobId   = "NA";
                datasetInfo.mintBlockID         = 0;
                datasetInfo.mintRunOrder        = 0;
                datasetInfo.menmDeisotopingTool = DeisotopingTool.NA;
                datasetInfo.mstrLocalPath       = filePath;

                numLoaded++;
                percentLoaded = 100.0 * (Convert.ToDouble(numLoaded) / Convert.ToDouble(marr_fileNames.Length));
                if (LoadedDataset != null)                
                    LoadedDataset(datasetInfo);
                if (LoadingProgress != null)                
                    LoadingProgress(percentLoaded);
                mlist_data.Add(datasetInfo);
            }
            if (LoadingComplete != null)
                LoadingComplete();            
        }
        #region IDisposable Members
        public void Dispose()
        {
            Abort();
        }
        #endregion
    }
}
