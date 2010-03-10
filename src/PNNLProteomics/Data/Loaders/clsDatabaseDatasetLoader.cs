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
using System.ComponentModel;
using System.Data.SqlClient;

using MultiAlignEngine;
using PNNLProteomics.Data;

namespace PNNLProteomics.Data.Loaders
{                           
        /// <summary>
        /// Class for loading a dataset from a database.
        /// </summary>
        public class clsDatabaseDatasetLoader: IDisposable
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
            public clsDatabaseDatasetLoader()
            {
                mobj_loadingThread      = null;
                mobj_serverInformation  = new clsDMSServerInformation();
                marrDatasetInfo         = new ArrayList();
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
            /// <summary>
            /// Constructs the query string based off the field to use, and the type of query to run.
            /// </summary>
            /// <param name="field">Filter string to run the query on.</param>
            /// <param name="flag">Type of query to run</param>
            /// <returns>SQL Query string</returns>
            private string SetupQuery(string field, int flag)
		    {
			    string selectQry = null;
                switch(flag)
                {

                    case CONST_QUERY_DATASET_COUNT:
                        /// 
                        /// SELECT
                        /// 
                        selectQry = "SELECT COUNT (*)";
                        /// From - Inner Join
                        selectQry += " FROM dbo.T_Dataset INNER JOIN dbo.T_Analysis_Job ON ";
                        selectQry += "      dbo.T_Dataset.Dataset_ID = dbo.T_Analysis_Job.AJ_datasetID INNER JOIN ";
                        selectQry += "      dbo.t_storage_path ON dbo.T_Dataset.DS_storage_path_ID = dbo.t_storage_path.SP_path_ID ";
                        /// 
                        /// WHERE
                        /// 
                        selectQry += " WHERE  ( dbo.T_Analysis_Job.AJ_analysisToolID IN (2, 7, 10, 11, 12, 16, 18))"; 
                        selectQry += "          AND dbo.T_Dataset.Dataset_Num like \'" + field + "\'";
                        break;

                    case CONST_QUERY_DATASET_ALL:			        
                        /// 
                        /// SELECT
                        /// 
				        /*selectQry = "SELECT DISTINCT    ";
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


                        selectQry += ", dbo.T_Organisms.og_name AS Organism";
                        ///
                        /// From - Inner Join
                        /// 
				        selectQry += " FROM dbo.T_Dataset INNER JOIN dbo.T_Analysis_Job ON " ; 
				        selectQry += " dbo.T_Dataset.Dataset_ID = dbo.T_Analysis_Job.AJ_datasetID " ; 
				        selectQry += " INNER JOIN dbo.T_Experiments ON dbo.T_Dataset.Exp_ID = dbo.T_Experiments.Exp_ID" ; 
				        selectQry += " INNER JOIN dbo.T_Instrument_Name ON dbo.T_Dataset.DS_instrument_name_id = dbo.T_Instrument_Name.Instrument_ID " ; 
				        selectQry += " INNER JOIN dbo.t_storage_path ON dbo.T_Dataset.DS_storage_path_ID = dbo.t_storage_path.SP_path_ID " ;

                        selectQry += " INNER JOIN dbo.T_Organisms ON dbo.T_Experiments.EX_organism_ID = dbo.T_Organisms.Organism_ID";

				        selectQry += " left outer JOIN dbo.T_Requested_Run_History ON dbo.T_Requested_Run_History.DatasetID = dbo.T_Dataset.Dataset_ID" ; 
				        selectQry += " INNER JOIN dbo.V_Dataset_Folder_Paths ON dbo.T_Dataset.Dataset_ID = dbo.V_Dataset_Folder_Paths.Dataset_ID" ;
                        /// 
                        /// WHERE
                        /// 
                        selectQry += " WHERE  (dbo.T_Analysis_Job.AJ_analysisToolID IN (2, 7, 10, 11, 12, 16, 18))";
                        selectQry += "         AND dbo.T_Dataset.Dataset_Num LIKE \'" + field + "\'";  
                         * */
                        selectQry = "SELECT * FROM V_Analysis_Job_Export_MultiAlign WHERE datasetName LIKE \'" + field + "\' AND ToolID IN (2, 7, 10, 11, 12, 16, 18)";
                        break;
                    
                    case CONST_QUERY_DATASET_ID:
                        /*selectQry = "SELECT  DISTINCT dbo.T_Dataset.Dataset_ID AS DatasetID ";
                        selectQry += ",dbo.t_storage_path.SP_vol_name_client as volName ";
                        selectQry += ", dbo.t_storage_path.SP_path as path ";
                        selectQry += ", dbo.T_Dataset.DS_folder_name as datasetFolder ";
                        selectQry += ", dbo.T_Analysis_Job.AJ_resultsFolderName resultsFolder ";
                        selectQry += ", dbo.T_Dataset.Dataset_Num AS datasetName ";
                        selectQry += ", dbo.T_Analysis_Job.AJ_jobID as JobId ";
                        selectQry += ", dbo.T_Dataset.DS_LC_column_ID as ColumnID ";
                        selectQry += ", dbo.T_Dataset.Acq_Time_Start as AcquisitionTime";
                        selectQry += ", dbo.T_Experiments.Ex_Labelling as Labelling";
                        selectQry += ", dbo.T_Instrument_Name.IN_Name as InstrumentName";
                        selectQry += ", dbo.T_Analysis_Job.AJ_analysisToolID as ToolID";
                        selectQry += ", dbo.T_Requested_Run_History.RDS_Block as BlockNum";
                        selectQry += ", dbo.T_Requested_Run_History.RDS_Name as ReplicateName";
                        selectQry += ", dbo.T_Dataset.Exp_ID as ExperimentID";
                        selectQry += ", dbo.T_Requested_Run_History.RDS_Run_Order as RunOrder";
                        selectQry += ", dbo.T_Requested_Run_History.RDS_BatchID as BatchID";
                        selectQry += ", dbo.V_Dataset_Folder_Paths.Archive_Folder_Path AS ArchPath";
                        selectQry += ", dbo.V_Dataset_Folder_Paths.Dataset_Folder_Path AS DatasetFullPath";

                        selectQry += ", dbo.T_Organisms.og_name AS Organism";

                        selectQry += " FROM dbo.T_Dataset INNER JOIN dbo.T_Analysis_Job ON ";
                        selectQry += " dbo.T_Dataset.Dataset_ID = dbo.T_Analysis_Job.AJ_datasetID ";
                        selectQry += " INNER JOIN dbo.T_Experiments ON dbo.T_Dataset.Exp_ID = dbo.T_Experiments.Exp_ID";
                        selectQry += " INNER JOIN dbo.T_Instrument_Name ON dbo.T_Dataset.DS_instrument_name_id = dbo.T_Instrument_Name.Instrument_ID ";
                        selectQry += " INNER JOIN dbo.t_storage_path ON dbo.T_Dataset.DS_storage_path_ID = dbo.t_storage_path.SP_path_ID ";
                        
                        selectQry += " INNER JOIN dbo.T_Organisms ON dbo.T_Experiments.EX_organism_ID = dbo.T_Organisms.Organism_ID";

                        selectQry += " left outer JOIN dbo.T_Requested_Run_History ON dbo.T_Requested_Run_History.DatasetID = dbo.T_Dataset.Dataset_ID";
                        selectQry += " INNER JOIN dbo.V_Dataset_Folder_Paths ON dbo.T_Dataset.Dataset_ID = dbo.V_Dataset_Folder_Paths.Dataset_ID";
                        selectQry += " WHERE  (dbo.T_Analysis_Job.AJ_analysisToolID IN (2, 7, 10, 11, 12, 16, 18))";
                        selectQry += " AND dbo.T_Dataset.Dataset_ID = " + field;
                         * */
                        selectQry = "SELECT * FROM V_Analysis_Job_Export_MultiAlign WHERE DatasetID = " + field + " AND ToolID IN (2, 7, 10, 11, 12, 16, 18)";                        
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
                }
                finally
                {
                    mobj_loadingThread = null;
                }
            }
            public void LoadDatasetsFromDatasetNames(string filter, bool thread)
            {
                LoadDatasets(filter, thread, CONST_QUERY_DATASET_ALL);
            }
            public void LoadDatasetsFromDatasetIDs(string filter, bool thread)
            {
                LoadDatasets(filter, thread, CONST_QUERY_DATASET_ID);
            }
            /// <summary>
            /// Load the data from the filter provided.  Thread if required and run a query type based on the flag.
            /// </summary>
            /// <param name="filter">Filter that corresponds with the query flag.</param>
            /// <param name="thread">Whether to thread the loading process.</param>
            /// <param name="flag">Query flag, 0=query dataset names, 1=query by dataset id.</param>
            private void LoadDatasets(string filter, bool thread, int flag)
            {
                if (filter.Trim() != "")
                {
                    mint_queryFlag = flag;
                    mstring_filter = filter;

                    marrDatasetInfo.Clear();

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

                string query = SetupQuery(mstring_filter, mint_queryFlag);

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

                Type labelType = typeof(MultiAlignEngine.LabelingType);
                MultiAlignEngine.LabelingType[] labelTypes = (MultiAlignEngine.LabelingType[])Enum.GetValues(labelType);

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
                    MultiAlignEngine.clsDatasetInfo datasetInfo   = new MultiAlignEngine.clsDatasetInfo();
                    datasetInfo.mstrDatasetId               = Convert.ToString(row[0]);
                    alias                                   = datasetInfo.mstrDatasetId;      /// This was set by default to be the same thing as the dataset id.
                    datasetInfo.mstrVolume                  = Convert.ToString(row[1]);
                    datasetInfo.mstrInstrumentFolder        = Convert.ToString(row[2]);
                    datasetInfo.mstrDatasetPath             = Convert.ToString(row[3]);
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

                    foreach (MultiAlignEngine.LabelingType type in labelTypes)
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
                        datasetInfo.menmDeisotopingTool = MultiAlignEngine.DeisotopingTool.Decon2LS;
                    }
                    else
                    {
                        datasetInfo.menmDeisotopingTool = MultiAlignEngine.DeisotopingTool.ICR2LS;
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
                    pekFilePath                 = Path.Combine(Convert.ToString(row[17]), datasetInfo.mstrResultsFolder);                    
                    fileName                    = GetFileNameFromDatabasePath(pekFilePath);                      
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
	    
            #region IDisposable Members
            public void Dispose()
            {
                Abort();                
            }
            #endregion
        }    		   
}


   