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
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data.SqlClient;

using MultiAlignWin.Data;

namespace MultiAlignWin.Data
{
        #region Delegate Definitions
        /// <summary>
        /// Delegate method definition for adding a dataset to the listview controls and the internal array lists.
        /// </summary>
        /// <param name="dataset"></param>
        public delegate void DelegateDataSetLoaded(MultiAlign.clsDatasetInfo dataset);
        /// <summary>
        /// Delegate method for when a data set is loaded, to update the progress of the loading progress bar.
        /// </summary>
        /// <param name="progressValue"></param>
        public delegate void DelegateUpdateLoadingPercentLoaded(double percentLoaded);
        /// <summary>
        /// Delegate method definition for when dataset loading is complete.
        /// </summary>
        public delegate void DelegateUpdateLoadingComplete();
        #endregion
                    
        /// <summary>
        /// Class for loading a dataset from a database.
        /// </summary>
        public class clsDatabaseDatasetLoader
        {
            
            #region Members
            /// <summary>
            /// Constant indicating to only load the dataset id numbers.
            /// </summary>
            private const int CONST_QUERY_DATASET_IDS_ONLY    = 1;
            /// <summary>
            /// Constant indicating to load all the dataset information from the database.
            /// </summary>
            private const int CONST_QUERY_DATASET_ALL         = 0;
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
		    private ArrayList   marrDatasetInfo;		    
            /// <summary>
            /// Thread for loading a lot of the dataset ID's.
            /// </summary>
            private Thread      mobj_loadingThread;
            /// <summary>
            /// Filter to use on the database while looking for dataset ids.
            /// </summary>
            private string      mstring_filter;
            /// <summary>
            /// Server to connect to.
            /// </summary>
            private string mstring_server;
            /// <summary>
            /// User to connect to the server with.
            /// </summary>
            private string mstring_userName;
            /// <summary>
            /// Password of the user on the server.
            /// </summary>
            private string mstring_password;
            #endregion

            /// <summary>
            /// Default constructor for the dataset loader from a database.
            /// </summary>
            public clsDatabaseDatasetLoader()
            {
                mobj_loadingThread = null;

                mstring_filter      = "";
                mstring_password    = "icr4fun";
                mstring_server      = "gigasax";
                mstring_userName    ="dmswebuser";

                marrDatasetInfo = new ArrayList();
            }
            
            #region Properties
            public ArrayList DataSets
            {
                get
                {
                    return marrDatasetInfo;
                }
                set
                {
                    marrDatasetInfo = value;
                }
            }

            /// <summary>
            /// Gets or sets the server to connect to that contains the Dataset database.
            /// </summary>
            public string Server
            {
                get
                {
                    return mstring_server;
                }
                set
                {
                    mstring_server = value;
                }
            }

            /// <summary>
            /// Gets or sets the username to use when connecting to the database.
            /// </summary>
            public string UserName
            {
                get
                {
                    return mstring_userName;
                }
                set
                {
                    mstring_userName = value;
                }
            }

            /// <summary>
            /// Gets or sets the password used for the user on the server that contains the database for the datasets.
            /// </summary>
            public string Password
            {
                get
                {
                    return mstring_password;
                }
                set
                {
                    mstring_password = value;
                }
            }
            #endregion
            
            #region Query Setup 

            /// <summary>
            /// Sets up the select query.
            /// </summary>
            /// <param name="field"></param>
            /// <param name="flag"></param>
            /// <returns></returns>
            private string SetupQuery(string field, int flag)
		    {
			    string selectQry = null;
                switch(flag)
                {
                    case CONST_QUERY_DATASET_ALL:			        
                        /// 
                        /// SELECT
                        /// 
				        selectQry = "SELECT DISTINCT    ";
                        selectQry += "  dbo.T_Dataset.Dataset_ID AS DatasetID " ;                               // 0
				        selectQry += ", dbo.t_storage_path.SP_vol_name_client as volName " ;                    // 1 
				        selectQry += ", dbo.t_storage_path.SP_path as path " ;                                  // 2
				        selectQry += ", dbo.T_Dataset.DS_folder_name as datasetFolder " ;                       // 3
				        selectQry += ", dbo.T_Analysis_Job.AJ_resultsFolderName resultsFolder " ;               // 4
				        selectQry += ", dbo.T_Dataset.Dataset_Num AS datasetName " ;                            // 5
				        selectQry += ", dbo.T_Analysis_Job.AJ_jobID as JobId " ;                                // 6
				        selectQry += ", dbo.T_Dataset.DS_LC_column_ID as ColumnID " ;                           // 7
				        selectQry += ", dbo.T_Dataset.Acq_Time_Start as AcquisitionTime" ;                      // 8
				        selectQry += ", dbo.T_Experiments.Ex_Labelling as Labelling" ;                          // 9
				        selectQry += ", dbo.T_Instrument_Name.IN_Name as InstrumentName" ;                      //10
				        selectQry += ", dbo.T_Analysis_Job.AJ_analysisToolID as ToolID" ;                       //11
				        selectQry += ", dbo.T_Requested_Run_History.RDS_Block as BlockNum" ;                    //12
				        selectQry += ", dbo.T_Requested_Run_History.RDS_Name as ReplicateName" ;                //13
				        selectQry += ", dbo.T_Dataset.Exp_ID as ExperimentID" ;                                 //14
				        selectQry += ", dbo.T_Requested_Run_History.RDS_Run_Order as RunOrder" ;                //15
				        selectQry += ", dbo.T_Requested_Run_History.RDS_BatchID as BatchID" ;                   //16
				        selectQry += ", dbo.V_Dataset_Folder_Paths.Archive_Folder_Path AS ArchPath" ;           //17 
				        selectQry += ", dbo.V_Dataset_Folder_Paths.Dataset_Folder_Path AS DatasetFullPath" ;    //18
                        ///
                        /// From - Inner Join
                        /// 
				        selectQry += " FROM dbo.T_Dataset INNER JOIN dbo.T_Analysis_Job ON " ; 
				        selectQry += " dbo.T_Dataset.Dataset_ID = dbo.T_Analysis_Job.AJ_datasetID " ; 
				        selectQry += " INNER JOIN dbo.T_Experiments ON dbo.T_Dataset.Exp_ID = dbo.T_Experiments.Exp_ID" ; 
				        selectQry += " INNER JOIN dbo.T_Instrument_Name ON dbo.T_Dataset.DS_instrument_name_id = dbo.T_Instrument_Name.Instrument_ID " ; 
				        selectQry += " INNER JOIN dbo.t_storage_path ON dbo.T_Dataset.DS_storage_path_ID = dbo.t_storage_path.SP_path_ID " ;
				        selectQry += " left outer JOIN dbo.T_Requested_Run_History ON dbo.T_Requested_Run_History.DatasetID = dbo.T_Dataset.Dataset_ID" ; 
				        selectQry += " INNER JOIN dbo.V_Dataset_Folder_Paths ON dbo.T_Dataset.Dataset_ID = dbo.V_Dataset_Folder_Paths.Dataset_ID" ;
                        /// 
                        /// WHERE
                        /// 
				        selectQry += " WHERE  (dbo.T_Analysis_Job.AJ_analysisToolID = 2 OR ";
                        selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 7 OR ";
                        selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 10 OR ";
                        selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 11 OR ";
                        selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 12 OR ";
                        selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 16 OR ";
                        selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 18 ) ";
                        selectQry += "         AND dbo.T_Dataset.Dataset_Num LIKE \'" + field + "\'";                            
                        break;

                    case CONST_QUERY_DATASET_IDS_ONLY:
                        /// 
                        /// SELECT
                        /// 
			            selectQry = "SELECT DISTINCT    dbo.T_Dataset.Dataset_ID AS DatasetID " ;
                        /// From - Inner Join
			            selectQry += " FROM dbo.T_Dataset INNER JOIN dbo.T_Analysis_Job ON " ; 
			            selectQry += "      dbo.T_Dataset.Dataset_ID = dbo.T_Analysis_Job.AJ_datasetID INNER JOIN " ; 
			            selectQry += "      dbo.t_storage_path ON dbo.T_Dataset.DS_storage_path_ID = dbo.t_storage_path.SP_path_ID " ;
                        /// 
                        /// WHERE
                        /// 
			            selectQry += " WHERE  ( dbo.T_Analysis_Job.AJ_analysisToolID = 2 OR";
                        selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 7  OR ";
                        selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 10 OR ";
                        selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 11 OR ";
                        selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 12 OR ";
                        selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 16 OR ";
                        selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 18 ) "; 
			            selectQry += "          AND dbo.T_Dataset.Dataset_Num like \'" + field + "\'" ; 
                        break;
			    }				    
                return selectQry;
            }
            #endregion
            
            #region Data Loading 
             
            /// <summary>
            /// Loads the datasets from the filter provided by the UI.  The datasets should have already been loaded to the marrDatasetID's array.
            /// </summary>
            public void LoadDatasets(string filter, bool thread)
            {
                if (filter != "")
                {
                    mstring_filter = filter;
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
                            System.Diagnostics.Trace.WriteLine("The loading thread failed to load.");
                            if (LoadingComplete != null)
                                LoadingComplete();
                        }
                   }
                   else
                   {
                       LoadDatasetsThreadStart();
                   }
                }                                 
            }            

            /// <summary>
            /// Threaded start in charge of loading the datasets from the database.
            /// </summary>
            /// <param name="filter"></param>
            private void LoadDatasetsThreadStart()
            {
                string cString  = String.Format("database=DMS5;server={0};user id={1};Password={2}",
                                                mstring_server,
                                                mstring_userName,
                                                mstring_password);

                string query = SetupQuery(mstring_filter, CONST_QUERY_DATASET_ALL);

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
                string fileName;

                Type labelType = typeof(MultiAlign.LabelingType);
                MultiAlign.LabelingType[] labelTypes = (MultiAlign.LabelingType[])Enum.GetValues(labelType);

                int     numberLoaded  = 0;
                double  percentLoaded = 0;

                /// 
                /// For each result stored in a row of the datatable found, load it 
                /// into a dataset info class.
                /// 
                foreach (DataRow row in table.Rows)
                {
                    string alias;
                    MultiAlign.clsDatasetInfo datasetInfo = new MultiAlign.clsDatasetInfo();
                    datasetInfo.mstrDatasetId = Convert.ToString(row[0]);
                    alias = datasetInfo.mstrDatasetId;      /// This was set by default to be the same thing as the dataset id.
                    datasetInfo.mstrVolume = Convert.ToString(row[1]);
                    datasetInfo.mstrInstrumentFolder = Convert.ToString(row[2]);
                    datasetInfo.mstrDatasetPath = Convert.ToString(row[3]);
                    datasetInfo.mstrResultsFolder = Convert.ToString(row[4]);
                    datasetInfo.mstrAnalysisJobId = Convert.ToString(row[6]);
                    datasetInfo.mintColumnID = Convert.ToInt32(row[7]);
                    datasetInfo.mdateAcquisitionStart = Convert.ToDateTime(row[8]);
                    string labelMedia = Convert.ToString(row[9]);

                    labelMedia.Replace("_", "");
                    labelMedia.Replace(" ", "");
                    labelMedia.Replace("/", "");
                    labelMedia.Replace("-", "");

                    foreach (MultiAlign.LabelingType type in labelTypes)
                    {
                        if (type.ToString() == labelMedia)
                        {
                            datasetInfo.menmLabelingType = type;
                            break;
                        }
                    }

                    datasetInfo.mstrInstrment = Convert.ToString(row[10]);
                    int toolId = Convert.ToInt32(row[11]);
                    /// 
                    /// Tool Type
                    /// 
                    if (toolId == 16 || toolId == 18)
                    {
                        datasetInfo.menmDeisotopingTool = MultiAlign.DeisotopingTool.Decon2LS;
                    }
                    else
                    {
                        datasetInfo.menmDeisotopingTool = MultiAlign.DeisotopingTool.ICR2LS;
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

                    /// 
                    /// Data File Path
                    /// 
                    pekFilePath = Path.Combine(Convert.ToString(row[17]), datasetInfo.mstrResultsFolder);                    
                    fileName = GetFileNameFromDatabasePath(pekFilePath);                      
                    datasetInfo.mstrLocalPath   = Path.Combine(pekFilePath, fileName);
                    datasetInfo.mstrDatasetName = fileName;
                    

                    datasetInfo.mstrAlias = alias;
                    datasetInfo.selected  = false;

                    marrDatasetInfo.Add(datasetInfo);

                    /// 
                    /// Give the user a pointer to the loaded dataset
                    /// 
                    if (LoadedDataset != null)
                        LoadedDataset(datasetInfo);

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
            /// Gets the number of dataset ID's stored in the database.
            /// </summary>
            /// <param name="query"></param>
            /// <returns>-1 if the call fails to make a connection to the database, or 0 or more indicating the number of datasets found </returns>
		    public int GetDatasetIDCount(string filter)
		    {
                int datasetCount = 0;
			    string server = "gigasax" ; 
			    string userName = "dmswebuser" ; 
			    string passwd = "icr4fun" ; 
			    string cString = String.Format("database=DMS5;server={0};user id={1};Password={2}", 
					                            server,
                                                userName,
                                                passwd);

                /// 
                /// Only get the dataset id's, by setting flag = 0
                /// 
                string query = SetupQuery(filter, CONST_QUERY_DATASET_IDS_ONLY);

			    SqlConnection myConnection = new SqlConnection(cString);

                /// 
                /// Try to open a connection to the database, if it fails, return -1
                /// 
			    try
			    {
				    myConnection.Open();
			    }
			    catch(Exception e)
			    {
                    System.Diagnostics.Trace.WriteLine(e.Message);
				    return -1;
			    }
    						  
                DataTable table         = new DataTable();
                SqlDataAdapter adapter  = new SqlDataAdapter(query, myConnection);
			    try 
			    {
                    adapter.Fill(table);
                    datasetCount = table.Rows.Count;
			    }
			    catch(Exception ex)
			    {
				    Console.WriteLine(ex.Message +  "DMS get information error") ;  
			    }
			    finally 
			    {				    
				    // always call Close when done reading.
				    myConnection.Close();
			    }

                return datasetCount;
		    }
            /// <summary>
            /// Gets the file name from the database path provided.
            /// </summary>
            /// <param name="sourcePath"></param>
            /// <returns></returns>
		    private string GetFileNameFromDatabasePath(string sourcePath)
		    {
			    string fileName = null;                
                string[] pekInfo  = Directory.GetFiles(sourcePath,"*.pek", SearchOption.TopDirectoryOnly);                
                /// 
                /// Search first for pek file names
                /// 
                if (pekInfo.Length > 0)
                {
                    foreach (string pekFileName in pekInfo)
                    {
                        if (pekFileName.IndexOf("_ic.pek") == -1)
                        {
                            FileInfo info = new FileInfo(pekFileName);
                            fileName = info.Name;
                            break;
                        }
                    }
                }
                else
                {
                    /// Then for isos.
                    string[] isosInfo = Directory.GetFiles(sourcePath, "*_isos.csv", SearchOption.TopDirectoryOnly);
                    if (isosInfo.Length > 0)
                    {
                        FileInfo info = new FileInfo(isosInfo[0]);
                        fileName = info.Name;
                    }
                }
			    return fileName ;
		    }    		
		    #endregion		
	    }    		   
}


   