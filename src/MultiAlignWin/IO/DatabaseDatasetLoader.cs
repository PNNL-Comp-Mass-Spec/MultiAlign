/*////////////////////////////////////////////////////////////////////////
 *  File  : clsDatabaseDatasetLoader.cs
 *  Author: Brian LaMarche
 *  Date  : 9/08/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      Class responsible for loading dataset information from a DMS 
 *      server that contains dataset databases.
 * 
 *  Revisions:
 *      9-8-2008:
 *          - Moved data loading features from the User Interface control
 *              to this file to separate the cohesion of data and user interface layers.
 */
///////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Data;
//using System.Drawing;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;

using MultiAlignEngine;
using PNNLProteomics.Data;

namespace MultiAlignWin.IO
{                           
        /// <summary>
        /// Class for loading a dataset from a database.
        /// </summary>
        public class DatabaseDatasetLoader: IDisposable
        {            
            #region Members
            /// <summary>
            /// Constant indicating to only load the dataset id numbers.
            /// </summary>
            private const int CONST_QUERY_DATASET_COUNT  = 0;
            /// <summary>
            /// Constant indicating to load all the dataset information from the database.
            /// </summary>
            public  const int CONST_QUERY_DATASET_ALL    = 1;
            /// <summary>
            /// Corresponds with loading datasets based on id's.
            /// </summary>
            public  const int CONST_QUERY_DATASET_ID     = 2;
            /// <summary>
            /// Fired when the loader finds how many datasets are available.
            /// </summary>
            public event DelegateTotalDatasetsFound DatasetsFound;
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
            /// Arraylist containing all the present datasets.
            /// </summary>
            private List<DatasetInformation> m_datasetInfo;		    
            /// <summary>
            /// Thread for loading a lot of the dataset ID's.
            /// </summary>
            private Thread mobj_loadingThread;
            /// <summary>
            /// Filter to use on the database while looking for dataset ids.
            /// </summary>
            private DMSDatasetSearchOptions m_searchOptions;
            /// <summary>
            /// DMS connection information.
            /// </summary>
            private clsDMSServerInformation mobj_serverInformation;
            /// <summary>
            /// Determines what kind of query we want to do, on the dataset num = name, or the dataset id.
            /// </summary>
            private int mint_queryFlag;
            #endregion

            /// <summary>
            /// Default constructor for the dataset loader from a database.
            /// </summary>
            public DatabaseDatasetLoader()
            {
                mobj_loadingThread      = null;
                mobj_serverInformation  = new clsDMSServerInformation();
                m_datasetInfo           = new List<DatasetInformation>();
                m_searchOptions         = new DMSDatasetSearchOptions();
            }

            #region Properties
            public List<DatasetInformation> DataSets
            {
                get
                {
                    return m_datasetInfo;
                }
                set
                {
                    m_datasetInfo = value;
                }
            }
            /// <summary>
            /// Gets or sets the DMS server information.
            /// </summary>
            public clsDMSServerInformation ServerInformation
            {
                get
                {
                    return mobj_serverInformation;
                }
                set
                {
                    mobj_serverInformation = value;
                }
            }

            #endregion
            
            #region Query Setup
            private string CreateToolIDStringList(DMSDatasetSearchOptions options)
            {
                string toolList = "";
                foreach (DeisotopingTool id in options.ToolIDs)
                {
                    if (id != DeisotopingTool.LCMSFeatureFinder)
                    {
                        toolList += ((int)id).ToString() + ",";
                    }
                    else 
                    {
                        toolList += ((int)DeisotopingTool.Decon2ls_V2);
                    }
                }
                toolList = toolList.TrimEnd(new char[] { ',' });
                return toolList;
            }
            /// <summary>
            /// Constructs the query string based off the field to use, and the type of query to run.
            /// </summary>
            /// <param name="field">Filter string to run the query on.</param>
            /// <param name="flag">Type of query to run</param>
            /// <returns>SQL Query string</returns>
            private string BuildQueryString(DMSDatasetSearchOptions options, int flag)
		    {
			    string selectQry = null;
                string field     = "";
                string tools     = CreateToolIDStringList(options);

                switch(flag)
                {
                    case CONST_QUERY_DATASET_ALL:
                        field     = options.DatasetName;
                        selectQry = "SELECT * FROM V_Analysis_Job_Export_MultiAlign WHERE datasetName LIKE \'" + field + "\' AND ToolID IN (" +  tools + ")";
                        if (!string.IsNullOrEmpty(options.InstrumentName))
                        {
                            selectQry += " AND InstrumentName LIKE \'%" + options.InstrumentName + "%\'";
                        }
                        if (!string.IsNullOrEmpty(options.ParameterFileName))
                        {
                            selectQry += " AND ParameterFileName LIKE \'%" + options.ParameterFileName + "%\'";
                        }
                        selectQry += " AND AcquisitionTime >= '" + options.DateTime.ToShortDateString() + "'";
                        break;
                    
                    case CONST_QUERY_DATASET_ID:
                        field = options.DatasetID.ToString();
                        selectQry = "SELECT * FROM V_Analysis_Job_Export_MultiAlign WHERE DatasetID = " + field + " AND ToolID IN (" + tools + ")";

                        if (!string.IsNullOrEmpty(options.InstrumentName))
                        {
                            selectQry += " AND InstrumentName LIKE \'%" + options.InstrumentName + "%\'";
                        }
                        if (!string.IsNullOrEmpty(options.ParameterFileName))
                        {
                            selectQry += " AND ParameterFileName LIKE \'%" + options.ParameterFileName + "%\'";
                        }
                        selectQry += " AND AcquisitionTime >= '" + options.DateTime.ToShortDateString() + "'";
                        break;
			    }				    
                return selectQry;
            }
            #endregion

            #region Data Loading
            /// <summary>
            /// Aborts the loading thread if it's currently threaded.
            /// </summary>
            public void Abort()
            {
                try
                {
                    if (mobj_loadingThread != null)
                    {
                        mobj_loadingThread.Abort();
                    }
                }
                catch
                {
                    // Pass, we don't care.
                }
                finally
                {
                    mobj_loadingThread = null;
                }
            }
            public void LoadDatasetsFromDatasetNames(DMSDatasetSearchOptions options, bool thread)
            {
                LoadDatasets(options, thread, CONST_QUERY_DATASET_ALL);
            }
            public void LoadDatasetsFromDatasetIDs(string filter, bool thread)
            {
                DMSDatasetSearchOptions options = new DMSDatasetSearchOptions();
                options.DatasetID                       = filter;
                LoadDatasets(options, thread, CONST_QUERY_DATASET_ID);
            }
            /// <summary>
            /// Load the data from the filter provided.  Thread if required and run a query type based on the flag.
            /// </summary>
            /// <param name="filter">Filter that corresponds with the query flag.</param>
            /// <param name="thread">Whether to thread the loading process.</param>
            /// <param name="flag">Query flag, 0=query dataset names, 1=query by dataset id.</param>
            private void LoadDatasets(DMSDatasetSearchOptions options, bool thread, int flag)
            {
                
                    mint_queryFlag  = flag;
                    m_searchOptions = options;

                    m_datasetInfo.Clear();

                    if (thread == true)
                    {                        
                        /// 
                        /// Create a new thread to do the loading.  
                        /// 
                        ThreadStart threadStart = new ThreadStart(LoadDatasetsThreadStart);
                        /// If another loading thread already exists, kill it.
                        if (mobj_loadingThread != null)
                        {
                            CancelLoadingDatasets();
                        }

                        /// Create a new thread to start.                            
                        mobj_loadingThread = new Thread(threadStart);

                        try
                        {
                            mobj_loadingThread.Start();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine("The loading thread failed to load. " + ex.Message);
                            if (LoadingComplete != null)
                                LoadingComplete();
                        }
                   }
                   else
                   {
                       CancelLoadingDatasets();
                       LoadDatasetsThreadStart();
                   }
                                               
            }            
            /// <summary>
            /// Threaded start in charge of loading the datasets from the database.
            /// </summary>
            /// <param name="filter"></param>
            private void LoadDatasetsThreadStart()
            {
                string cString  = String.Format("database={0};server={1};user id={2};Password={3}",
                                                mobj_serverInformation.DatabaseName,
                                                mobj_serverInformation.ServerName,
                                                mobj_serverInformation.Username,
                                                mobj_serverInformation.Password);

                string query = BuildQueryString(m_searchOptions, mint_queryFlag);

                /// 
                /// Create a connection to the database.
                /// 
                SqlConnection myConnection = new SqlConnection(cString);
                myConnection.Open();

                /// 
                /// Create an adapter and fill it with the data we found.
                /// 
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, myConnection);
                try
                {
                    DataTable results = new DataTable();
                    dataAdapter.Fill(results);
                    LoadDataAdapterResultsIntoArray(results);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("An error occured while trying to load data from the database: {0}", ex.Message));
                }
                finally
                {
                    if (LoadingComplete != null)
                        LoadingComplete();

                    myConnection.Close();
                    myConnection.Dispose();
                }
            }
            /// <summary>
            /// Loads the data from the data table into the marr dataset array.
            /// </summary>
            /// <param name="table"></param>
            /// <param name="pekFilePath"></param>
            /// <param name="FileName"></param>
            private void LoadDataAdapterResultsIntoArray(DataTable table)
            {
                string pekFilePath;               

                Type labelType = typeof(LabelingType);
                LabelingType[] labelTypes = (LabelingType[])Enum.GetValues(labelType);

                int     numberLoaded  = 0;
                double  percentLoaded = 0;

                /// 
                /// For each result stored in a row of the datatable found, load it 
                /// into a dataset info class.
                /// 
                if (DatasetsFound != null && table.Rows != null)
                    DatasetsFound(table.Rows.Count);

                foreach (DataRow row in table.Rows)
                {
                    string alias;
                    DatasetInformation datasetInfo          = new DatasetInformation();
                    datasetInfo.mstrDatasetId               = Convert.ToString(row[0]);
                    alias                                   = datasetInfo.mstrDatasetId;      /// This was set by default to be the same thing as the dataset id.
                    datasetInfo.mstrVolume                  = Convert.ToString(row[1]);
                    datasetInfo.mstrInstrumentFolder        = Convert.ToString(row[2]);
                    datasetInfo.DatasetName                 = Convert.ToString(row[3]);
                    datasetInfo.mstrResultsFolder           = Convert.ToString(row[4]);
                    datasetInfo.mstrAnalysisJobId           = Convert.ToString(row[6]);
                    datasetInfo.mintColumnID                = Convert.ToInt32(row[7]);
                    try
                    {
                        datasetInfo.mdateAcquisitionStart = Convert.ToDateTime(row[8]);
                    }
                    catch
                    {
                        datasetInfo.mdateAcquisitionStart = DateTime.MinValue;
                    }
                    string labelMedia                       = Convert.ToString(row[9]);

                    labelMedia.Replace("_", "");
                    labelMedia.Replace(" ", "");
                    labelMedia.Replace("/", "");
                    labelMedia.Replace("-", "");

                    foreach (LabelingType type in labelTypes)
                    {
                        if (type.ToString() == labelMedia)
                        {
                            datasetInfo.menmLabelingType = type;
                            break;
                        }
                    }

                    datasetInfo.mstrInstrment = Convert.ToString(row[10]);
                    int toolId = Convert.ToInt32(row[11]);
                    
                    // Tool Type -- Check to make sure we want to load this data type
                    // we do a check again later.  Here we do a check so we don't ping the archive.
                    datasetInfo.menmDeisotopingTool = DMSDatasetSearchOptions.MapToolIDToDeisotopingTool(toolId);
                    if (m_searchOptions.ToolIDs.Contains(datasetInfo.menmDeisotopingTool) == false)
                    {
                        if (!m_searchOptions.ToolIDs.Contains(DeisotopingTool.LCMSFeatureFinder))
                            continue;
                    }

                    /// 
                    /// Block ID
                    /// 
                    if (row.IsNull(12) == false)
                    {
                        datasetInfo.mintBlockID = Convert.ToInt32(row[12]);
                    }
                    else
                    {
                        datasetInfo.mintBlockID = 0;
                    }

                    /// 
                    /// Replicate Name
                    /// 

                    if (row.IsNull(13) == false)
                    {
                        datasetInfo.mstrReplicateName = Convert.ToString(row[13]);
                    }
                    else
                    {
                        datasetInfo.mstrReplicateName = "";
                    }

                    /// 
                    /// Experiment ID
                    /// 
                    if (row.IsNull(14) == false)
                    {
                        datasetInfo.mintExperimentID = Convert.ToInt32(row[14]);
                    }
                    else
                    {
                        datasetInfo.mintExperimentID = 0;
                    }

                    /// 
                    /// Run Order
                    /// 
                    if (row.IsNull(15) == false)
                    {
                        datasetInfo.mintRunOrder = Convert.ToInt32(row[15]);
                    }
                    else
                    {
                        datasetInfo.mintRunOrder = 0;
                    }

                    /// 
                    /// Batch ID
                    /// 
                    if (row.IsNull(16) == false)
                    {
                        datasetInfo.mintBatchID = Convert.ToInt32(row[16]);
                    }
                    else
                    {
                        datasetInfo.mintBatchID = 0;
                    }
                    datasetInfo.mstrAlias = alias;
                    datasetInfo.Selected = false;

                    /// 
                    /// Batch ID
                    /// 
                    if (row.IsNull(21) == false)
                    {
                        datasetInfo.ParameterFileName = Convert.ToString(row[21]);
                    }
                    else
                    {
                        datasetInfo.ParameterFileName = "";
                    }
                    datasetInfo.mstrAlias = alias;
                    datasetInfo.Selected = false;



                    /// 
                    /// Data File Path
                    /// 
                    pekFilePath        = Path.Combine(Convert.ToString(row[17]), datasetInfo.mstrResultsFolder);                    
                    List<string> files = GetFileNameFromDatabasePath(pekFilePath);

                    foreach (string fileName in files)
                    {
                        DatasetInformation info = new DatasetInformation();
                        if (info != null)
                        {                                                        
                            info.mdateAcquisitionStart  = datasetInfo.mdateAcquisitionStart;
                            
                            string extension            = System.IO.Path.GetExtension(fileName);
                            info.menmDeisotopingTool    = datasetInfo.menmDeisotopingTool;
                            info.menmLabelingType       = datasetInfo.menmLabelingType;
                            info.mintBatchID            = datasetInfo.mintBatchID;
                            info.mintBlockID            = datasetInfo.mintBlockID;
                            info.mintColumnID           = datasetInfo.mintColumnID;
                            info.mintExperimentID       = datasetInfo.mintExperimentID;
                            info.mintRunOrder           = datasetInfo.mintRunOrder;
                            info.mstrAlias              = datasetInfo.mstrAlias;
                            info.mstrAnalysisJobId      = datasetInfo.mstrAnalysisJobId;
                            info.mstrBlockingFactor     = datasetInfo.mstrBlockingFactor;
                            info.mstrComment            = datasetInfo.mstrComment;
                            info.mstrDatasetId          = datasetInfo.mstrDatasetId;
                            info.mstrInstrment          = datasetInfo.mstrInstrment;
                            info.mstrInstrumentFolder   = datasetInfo.mstrInstrumentFolder;
                            info.mstrOperator           = datasetInfo.mstrOperator;
                            info.mstrReplicateName      = datasetInfo.mstrReplicateName;
                            info.mstrResultsFolder      = datasetInfo.mstrResultsFolder;
                            info.mstrVolume             = datasetInfo.mstrVolume;
                            info.Selected               = datasetInfo.Selected;                            
                            info.ArchivePath            = Path.Combine(pekFilePath, fileName);
                            info.ParameterFileName      = datasetInfo.ParameterFileName;
                            info.DatasetName            = fileName;


                            // Check the file extension
                            if (!string.IsNullOrEmpty(m_searchOptions.FileExtension) &&
                                    !fileName.EndsWith(m_searchOptions.FileExtension))
                            {
                                continue;
                            }

                            if (System.IO.Path.GetExtension(fileName) == ".txt")
                            {
                                info.menmDeisotopingTool = DeisotopingTool.LCMSFeatureFinder;
                            }

                            if (m_searchOptions.ToolIDs.Contains(info.menmDeisotopingTool) == false)
                                continue;

                            m_datasetInfo.Add(info);
                            


                            /// 
                            /// Give the user a pointer to the loaded dataset
                            /// 
                            if (LoadedDataset != null)
                                LoadedDataset(info);
                        }
                    }


                    /// 
                    /// Update the user on how many are loaded
                    /// 
                    numberLoaded++;
                    percentLoaded = Convert.ToDouble(numberLoaded) / Convert.ToDouble(table.Rows.Count);
                    if (LoadingProgress != null)
                        LoadingProgress(100.0*percentLoaded);
                }
            }       
            /// <summary>
            /// Cancels loading of the datasets.
            /// </summary>
            public void CancelLoadingDatasets()
            {
                try
                {
                    mobj_loadingThread.Abort();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }

                mobj_loadingThread = null;
            }
            /// <summary>
            /// Gets the file name from the database path provided.
            /// </summary>
            /// <param name="sourcePath"></param>
            /// <returns></returns>
		    private List<string> GetFileNameFromDatabasePath(string sourcePath)
		    {
                List<string> names  = new List<string>();			    
                string[] files      = Directory.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly);
                string path = "";
                /// 
                /// Search first for pek file names
                /// 
                foreach (string fileName in files)
                {
                    if (fileName.IndexOf("_ic.pek") == -1 && fileName.ToLower().Contains(".pek"))
                    {
                        path = System.IO.Path.GetFileName(fileName);
                        names.Add(path);
                    }
                    else if (fileName.ToLower().Contains("_isos.csv"))
                    {
                        path = System.IO.Path.GetFileName(fileName);
                        names.Add(path);
                    }
                    else if (fileName.ToLower().Contains("_lcmsfeatures.txt"))
                    {
                        path = System.IO.Path.GetFileName(fileName);
                        names.Add(path);                         
                    }
                }                
                return names;
		    }    		
		    #endregion		
	    
            #region IDisposable Members
            public void Dispose()
            {
                Abort();                
            }
            #endregion
        }    		   
}


   