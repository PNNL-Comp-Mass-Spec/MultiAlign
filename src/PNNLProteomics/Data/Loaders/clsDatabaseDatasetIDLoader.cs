using System;
using System.IO;
using System.Text;
using System.Threading; 
using System.Collections;
using System.Collections.Generic;

using MultiAlignEngine;
using PNNLProteomics.Data;

namespace PNNLProteomics.Data.Loaders
{
    public class clsDatabaseDatasetIDLoader
    {
        #region Members
        /// <summary>
        /// Fired when the loader finds how many datasets are available.
        /// </summary>
        public event DelegateTotalDatasetsFound DatasetsFound;
        /// <summary>
        /// Called when a dataset is loaded.
        /// </summary>
        public event DelegateDataSetLoaded LoadedDataset;
        /// <summary>
        /// Called when the loading progress is updated.
        /// </summary>
        public event DelegateUpdateLoadingPercentLoaded LoadingProgress;
        /// <summary>
        /// Called when the loading of all datasets is complete or if there was a major error.
        /// </summary>
        public event DelegateUpdateLoadingComplete LoadingComplete;        
        /// <summary>
        /// DMS dataset loader.
        /// </summary>
        private clsDatabaseDatasetLoader mobj_datasetLoader;
        /// <summary>
        /// Collection of key->value pairs for dataset->alias pairs. 
        /// </summary>
        private List<KeyValuePair<string, string>> mlist_datasetKeyValues;
        /// <summary>
        /// Path to the file that contains the dataset ID's and job numbers.
        /// </summary>
        private string mstring_filename;
        /// <summary>
        /// Loading thread object.
        /// </summary>
        private Thread mobj_loadingThread;
        #endregion

        /// <summary>
        /// Default constructor for the dataset ID loader class.
        /// </summary>        
        public clsDatabaseDatasetIDLoader()
        {
            mobj_datasetLoader      = new clsDatabaseDatasetLoader();
            //marrDatasetInfo         = new ArrayList();
            mlist_datasetKeyValues  = new List<KeyValuePair<string, string>>();
            mobj_loadingThread      = null;
            mstring_filename        = null;
        }

        /// <summary>
        /// Loads the dataset ID's specified in filename from DMS.
        /// </summary>
        /// <param name="filename"></param>
        public void LoadDatasets(string filename, bool thread)
        {
            if (File.Exists(filename) == false)
                return;


            CancelLoadingDatasets();
            mstring_filename = filename;
            if (thread == true)
            {
                ThreadStart threadStart = new ThreadStart(LoadDatasetsThreadStart);
                mobj_loadingThread = new Thread(threadStart);
                try
                {
                    mobj_loadingThread.Start();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Thread loading thread failed to load.  "  + ex.Message);
                    if (LoadingComplete != null)
                        LoadingComplete();
                }
            }
            else
            {
                LoadDatasetsThreadStart();
            }
        }         
        /// <summary>
        /// Cancels loading of the datasets.
        /// </summary>
        public void CancelLoadingDatasets()
        {
            try
            {
                if (mobj_loadingThread == null)
                    return;

                if (mobj_loadingThread.IsAlive)
                {
                    if (mobj_datasetLoader != null)
                        mobj_datasetLoader.CancelLoadingDatasets();
                }
                mobj_loadingThread.Abort();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            mobj_loadingThread = null;
        }
        /// <summary>
        /// Entry point for a threaded dataset load or callable from the same thread.
        /// </summary>
        private void LoadDatasetsThreadStart()
        {

            mlist_datasetKeyValues.Clear();

            /// 
            /// Read the file for each dataset and alias.
            /// 
            using (StreamReader sr = new StreamReader(mstring_filename))
            {
                string line;
                string datasetID;
                string aliasID;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new char[] { ',' });

                    if (data.Length > 0)
                    {
                        datasetID = data[0];

                        if (data.Length > 1)
                        {
                            aliasID = data[1];
                        }
                        else
                        {
                            aliasID = datasetID;
                        }
                        KeyValuePair<string, string> pair = new KeyValuePair<string, string>(datasetID, aliasID);   
                        if (mlist_datasetKeyValues.Contains(pair) == false)
                            mlist_datasetKeyValues.Add(pair);
                    }
                }                
            }
            /// 
            /// For-each dataset id, alias pair, find all the related datasets in DMS.
            /// Then construct a list here locally.
            /// Do not sink into the events of the dms dataset loader, as we are 
            /// basically overriding their event progress notifications, and 
            /// marshalling our own to the calling thread via this controlled 
            /// interface.
            ///  
            int numberLoaded    = 0;
            double percent      = 0;            
            foreach (KeyValuePair<string, string> pair in mlist_datasetKeyValues)
            {
                string dataset  = pair.Key;
                string alias    = pair.Value;

                /// 
                /// Don't thread the loading here because we should already be threaded.
                /// 
                mobj_datasetLoader.LoadDatasetsFromDatasetIDs(dataset, false);
                ArrayList list = mobj_datasetLoader.DataSets;

                if (DatasetsFound != null && list != null)
                    DatasetsFound(list.Count);
                
                foreach (object o in list)
                {
                    clsDatasetInfo info = o as clsDatasetInfo;
                    if (info != null)
                    {
                        info.mstrAlias = alias;
                                                
                        if (LoadedDataset != null)
                            LoadedDataset(info);
                     }
                }

                /// 
                /// We update the progress here because we are not sure how many datasets match our dataset id's in the database.
                /// 
                percent = Math.Min(100, Math.Max(0, 100 * Convert.ToDouble(numberLoaded++) / Convert.ToDouble(mlist_datasetKeyValues.Count)));
                if (LoadingProgress != null)
                    LoadingProgress(percent);                                    
            }
            if (LoadingComplete != null)
                LoadingComplete();
        }   
        /// <summary>
        /// Gets or sets the DMS server information.
        /// </summary>
        public clsDMSServerInformation ServerInformation
        {
            get
            {
                return mobj_datasetLoader.ServerInformation;
            }
            set
            {
                mobj_datasetLoader.ServerInformation = value;
            }
        }
          
    }
}
