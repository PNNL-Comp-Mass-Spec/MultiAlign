using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data ; 
using System.IO ; 
using System.Data.SqlClient ; 

namespace MultiAlignWin
{
	public class ctlSelectFilesFromDBJobWizardPage : Wizard.UI.InternalWizardPage
	{
		public string mstrQuery ; 
		private System.Windows.Forms.Panel panelStep;
		private System.Windows.Forms.Label labelSelect;
		private System.Windows.Forms.Panel panelFileNames;
		private System.ComponentModel.IContainer components = null;

		private ArrayList marrDatasetInfo = new ArrayList() ;
		private ArrayList marrDatasetInfoCurrent = new ArrayList() ;

		private System.Windows.Forms.ListView joblistView;
		private System.Windows.Forms.ColumnHeader datasetID;
		private System.Windows.Forms.ColumnHeader jobnum;
		private System.Windows.Forms.ColumnHeader fileName;
		private System.Windows.Forms.ColumnHeader alias;
		private System.Windows.Forms.ColumnHeader block;
		private System.Windows.Forms.ColumnHeader runOrder; 

		private System.Windows.Forms.Button mbtnSelectDatasetIDFile; 

		private ListViewItemComparer _lvwItemComparer ;

		bool defaultSelected = false ;
		int selectedFileIndex = 0 ;
		MultiAlignWin.enmSelectType selection ;
		string datasetId ;

		public ctlSelectFilesFromDBJobWizardPage()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			_lvwItemComparer = new ListViewItemComparer();
			this.joblistView.ListViewItemSorter = _lvwItemComparer;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panelStep = new System.Windows.Forms.Panel();
			this.mbtnSelectDatasetIDFile = new System.Windows.Forms.Button();
			this.labelSelect = new System.Windows.Forms.Label();
			this.panelFileNames = new System.Windows.Forms.Panel();
			this.joblistView = new System.Windows.Forms.ListView();
			this.datasetID = new System.Windows.Forms.ColumnHeader();
			this.jobnum = new System.Windows.Forms.ColumnHeader();
			this.fileName = new System.Windows.Forms.ColumnHeader();
			this.alias = new System.Windows.Forms.ColumnHeader();
			this.block = new System.Windows.Forms.ColumnHeader();
			this.runOrder = new System.Windows.Forms.ColumnHeader();
			this.panelStep.SuspendLayout();
			this.panelFileNames.SuspendLayout();
			this.SuspendLayout();
			// 
			// Banner
			// 
			this.Banner.Name = "Banner";
			this.Banner.Subtitle = "Select datasets by using Job Numbers in DMS";
			this.Banner.Title = "Step 2. Select Job Numbers for Analysis";
			// 
			// panelStep
			// 
			this.panelStep.Controls.Add(this.mbtnSelectDatasetIDFile);
			this.panelStep.Controls.Add(this.labelSelect);
			this.panelStep.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelStep.Location = new System.Drawing.Point(0, 64);
			this.panelStep.Name = "panelStep";
			this.panelStep.Size = new System.Drawing.Size(432, 24);
			this.panelStep.TabIndex = 2;
			// 
			// mbtnSelectDatasetIDFile
			// 
			this.mbtnSelectDatasetIDFile.Dock = System.Windows.Forms.DockStyle.Left;
			this.mbtnSelectDatasetIDFile.Location = new System.Drawing.Point(240, 0);
			this.mbtnSelectDatasetIDFile.Name = "mbtnSelectDatasetIDFile";
			this.mbtnSelectDatasetIDFile.Size = new System.Drawing.Size(24, 24);
			this.mbtnSelectDatasetIDFile.TabIndex = 1;
			this.mbtnSelectDatasetIDFile.Text = "..";
			this.mbtnSelectDatasetIDFile.Click += new System.EventHandler(this.mbtnSelectJobIdFile_Click);
			// 
			// labelSelect
			// 
			this.labelSelect.Dock = System.Windows.Forms.DockStyle.Left;
			this.labelSelect.Location = new System.Drawing.Point(0, 0);
			this.labelSelect.Name = "labelSelect";
			this.labelSelect.Size = new System.Drawing.Size(240, 24);
			this.labelSelect.TabIndex = 0;
			this.labelSelect.Text = "      Select File with Analysis Job Numbers:";
			this.labelSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelFileNames
			// 
			this.panelFileNames.Controls.Add(this.joblistView);
			this.panelFileNames.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelFileNames.Location = new System.Drawing.Point(0, 88);
			this.panelFileNames.Name = "panelFileNames";
			this.panelFileNames.Size = new System.Drawing.Size(432, 136);
			this.panelFileNames.TabIndex = 3;
			// 
			// joblistView
			// 
			this.joblistView.AllowDrop = true;
			this.joblistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.datasetID,
																						  this.jobnum,
																						  this.fileName,
																						  this.alias,
																						  this.block,
																						  this.runOrder});
			this.joblistView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.joblistView.FullRowSelect = true;
			this.joblistView.GridLines = true;
			this.joblistView.Location = new System.Drawing.Point(0, 0);
			this.joblistView.Name = "joblistView";
			this.joblistView.Size = new System.Drawing.Size(432, 136);
			this.joblistView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.joblistView.TabIndex = 1;
			this.joblistView.View = System.Windows.Forms.View.Details;
			this.joblistView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.joblistView_ColumnClick);
			// 
			// datasetID
			// 
			this.datasetID.Text = "Dataset ID";
			this.datasetID.Width = 77;
			// 
			// jobnum
			// 
			this.jobnum.Text = "Job #";
			// 
			// fileName
			// 
			this.fileName.Text = "File name";
			this.fileName.Width = 222;
			// 
			// alias
			// 
			this.alias.Text = "Alias";
			this.alias.Width = 69;
			// 
			// block
			// 
			this.block.Text = "Block";
			// 
			// runOrder
			// 
			this.runOrder.Text = "Run Order";
			this.runOrder.Width = 70;
			// 
			// ctlSelectFilesFromDBJobWizardPage
			// 
			this.Controls.Add(this.panelFileNames);
			this.Controls.Add(this.panelStep);
			this.Name = "ctlSelectFilesFromDBJobWizardPage";
			this.Size = new System.Drawing.Size(432, 224);
			this.Controls.SetChildIndex(this.Banner, 0);
			this.Controls.SetChildIndex(this.panelStep, 0);
			this.Controls.SetChildIndex(this.panelFileNames, 0);
			this.panelStep.ResumeLayout(false);
			this.panelFileNames.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void joblistView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{

			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == _lvwItemComparer.SortColumn)
			{
				// Reverse the current sort direction for this column.
                if (_lvwItemComparer.Order == System.Windows.Forms.SortOrder.Ascending)
				{
                    _lvwItemComparer.Order = System.Windows.Forms.SortOrder.Descending;
				}
				else
				{
                    _lvwItemComparer.Order = System.Windows.Forms.SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				_lvwItemComparer.SortColumn = e.Column;
                _lvwItemComparer.Order = System.Windows.Forms.SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			this.joblistView.Sort();
		}

		private void AddToList(MultiAlign.clsDatasetInfo datasetInfo)
		{
			ListViewItem dataItem = new ListViewItem(datasetInfo.mstrDatasetId) ;
			dataItem.SubItems.Add(datasetInfo.mstrAnalysisJobId) ;
			dataItem.SubItems.Add(datasetInfo.mstrDatasetName) ;

			dataItem.SubItems.Add(datasetInfo.mstrAlias) ;
			if (datasetInfo.mintBlockID == 0)
				dataItem.SubItems.Add("NA") ;
			else
				dataItem.SubItems.Add(datasetInfo.mintBlockID.ToString()) ;
			if (datasetInfo.mintRunOrder == 0)
				dataItem.SubItems.Add("NA") ;
			else
				dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintRunOrder)) ;
			dataItem.Checked = datasetInfo.selected ;
			joblistView.Items.Add(dataItem) ;
		}

		
		private void AddToArrayList(MultiAlign.clsDatasetInfo datasetInfo)
		{
			marrDatasetInfo.Add(datasetInfo) ; 
		}

		private void mbtnSelectJobIdFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Multiselect = false ; 
			openFileDialog1.Filter = "*.csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 1 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				try 
				{
					// Create an instance of StreamReader to read from a file.
					// The using statement also closes the StreamReader.
					using (StreamReader sr = new StreamReader(openFileDialog1.FileName)) 
					{
						String line;
						// Read and display lines from the file until the end of 
						// the file is reached.
						while ((line = sr.ReadLine()) != null) 
						{
							marrDatasetInfoCurrent.Clear() ; 
							int index = line.IndexOf(",") ;
							datasetId = line ; 
							string alias = "" ; 
							if (index > 0)
							{
								datasetId = line.Substring(0,index) ; 
								alias = line.Substring(index+1) ; 
							}
							else
							{
								datasetId = line ; 
								alias = line ; 
							}

							SetupQuery(datasetId) ; 
							GetData(datasetId, alias) ;

							joblistView.BeginUpdate();
							if (marrDatasetInfoCurrent.Count > 1)
							{
								marrDatasetInfoCurrent.Sort() ;
								if (selectedFileIndex > marrDatasetInfoCurrent.Count - 1)
								{
									defaultSelected = false ;
								}
								selectedFileIndex = SelectDataSet() ;
								AddToList((MultiAlign.clsDatasetInfo)marrDatasetInfoCurrent[selectedFileIndex]) ;
								// Add to marrDatasetInfo
								AddToArrayList((MultiAlign.clsDatasetInfo)marrDatasetInfoCurrent[selectedFileIndex]) ; 
							}
							else if (marrDatasetInfoCurrent.Count == 1)
							{
								AddToList((MultiAlign.clsDatasetInfo)marrDatasetInfoCurrent[0]) ; 
								AddToArrayList((MultiAlign.clsDatasetInfo)marrDatasetInfoCurrent[0]) ;
							}
							else
							{
								MessageBox.Show(this, "Could not find any folders corresponding to dataset id = " + 
									datasetId, "Missing data") ; 
							}
							joblistView.EndUpdate();
						}//while
					}//using
				}//try
				catch (Exception ex) 
				{
					// Let the user know what went wrong.
					Console.WriteLine("The file could not be read:");
					Console.WriteLine(ex.Message);
				}
			}
			//MessageBox.Show(((MultiAlign.clsDatasetInfo) marrDatasetInfo[0]).mstrDatasetName);
		}

		private int SelectDataSet()
		{
			int selectedFIndex = 0 ;
			string []dataset_names = new string [marrDatasetInfoCurrent.Count] ; 
			string []file_names = new string [marrDatasetInfoCurrent.Count] ;

			for (int fileNum = 0 ; fileNum < marrDatasetInfoCurrent.Count ; fileNum++)
			{
				dataset_names[fileNum] = 
					((MultiAlign.clsDatasetInfo) marrDatasetInfoCurrent[fileNum]).mstrResultsFolder; 
				file_names[fileNum] = 
					((MultiAlign.clsDatasetInfo) marrDatasetInfoCurrent[fileNum]).mstrDatasetName;
			}

			if (!defaultSelected)
			{
				frmSelectOneDataset frmOneDataset = new frmSelectOneDataset() ; 
				frmOneDataset.SetDatasets(dataset_names) ;
				frmOneDataset.SetDatasetNames(file_names) ;
				frmOneDataset.setDatasetID = datasetId;
				if(frmOneDataset.ShowDialog(this) == DialogResult.OK)
				{
					selectedFIndex = frmOneDataset.SelectedFileIndex ; 
					selection = frmOneDataset.Selection ;
					defaultSelected = frmOneDataset.DefaultChecked ;
					if (selectedFIndex < 0)
						selectedFIndex = SwitchSelection(selection) ;
				}
				else
				{
					MessageBox.Show("Since nothing was chosen, skipping file.") ; 
				}
			}
			else
			{
				selectedFIndex = SwitchSelection(selection) ;
			}
			
			return selectedFIndex ;
		}

		private int SwitchSelection(enmSelectType selection)
		{
			int selectedFIndex = 0 ;
			
			switch (selection)
			{
				case enmSelectType.Decon2LS :
					
					for (int fileNum = 0 ; fileNum < marrDatasetInfoCurrent.Count ; fileNum++)
					{
						string datasetName = ((MultiAlign.clsDatasetInfo) marrDatasetInfoCurrent[fileNum]).mstrDatasetName;
						if (datasetName.ToLower().IndexOf("_isos.csv") > 0 )
						{
							selectedFIndex = fileNum ;
							//break ;
						}
					}
					break ;
				case enmSelectType.ICR2LS :
					for (int fileNum = 0 ; fileNum < marrDatasetInfoCurrent.Count ; fileNum++)
					{
						string datasetName = ((MultiAlign.clsDatasetInfo) marrDatasetInfoCurrent[fileNum]).mstrDatasetName;
						if (datasetName.ToLower().IndexOf(".pek") > 0 )
						{
							selectedFIndex = fileNum ;
							//break ;
						}
					}
					break ;
				case enmSelectType.NEWEST :
					selectedFIndex = marrDatasetInfoCurrent.Count - 1 ;
					break ;
				default :
					selectedFIndex = 0 ;
					break ;
			}
			return selectedFIndex ;
		}

		private string GetFileNameFromDBPath(string sourcePath)
		{
			string FileName = null ;

			System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourcePath);
			foreach (System.IO.FileInfo f in dir.GetFiles())
			{
				if (f.Name.ToLower().IndexOf(".pek") > 0)
				{
					if(f.Name.IndexOf("_ic.pek") == -1)
						FileName = f.Name ;
				}
				else if (f.Name.ToLower().IndexOf("_isos.csv") != -1)
				{
					FileName = f.Name ;
				}
			}
			return FileName ;
		}

		private void SetupQuery(string datasetId)
		{
			string selectQry = null ; 

			selectQry = "SELECT     dbo.T_Dataset.Dataset_ID AS DatasetID " ; 
			selectQry += ",dbo.t_storage_path.SP_vol_name_client as volName " ; 
			selectQry += ", dbo.t_storage_path.SP_path as path " ; 
			selectQry += ", dbo.T_Dataset.DS_folder_name as datasetFolder " ; 
			selectQry += ", dbo.T_Analysis_Job.AJ_resultsFolderName resultsFolder " ; 
			selectQry += ", dbo.T_Dataset.Dataset_Num AS datasetName " ; 
			selectQry += ", dbo.T_Analysis_Job.AJ_jobID as JobId " ;
			selectQry += ", dbo.T_Dataset.DS_LC_column_ID as ColumnID " ;
			selectQry += ", dbo.T_Dataset.Acq_Time_Start as AcquisitionTime" ;
			selectQry += ", dbo.T_Experiments.Ex_Labelling as Labelling" ;
			selectQry += ", dbo.T_Instrument_Name.IN_Name as InstrumentName" ;
			selectQry += ", dbo.T_Analysis_Job.AJ_analysisToolID as ToolID" ;
			selectQry += ", dbo.T_Requested_Run_History.RDS_Block as BlockNum" ;
			selectQry += ", dbo.T_Requested_Run_History.RDS_Name as ReplicateName" ; 
			selectQry += ", dbo.T_Dataset.Exp_ID as ExperimentID" ; 
			selectQry += ", dbo.T_Requested_Run_History.RDS_Run_Order as RunOrder" ; 
			selectQry += ", dbo.T_Requested_Run_History.RDS_BatchID as BatchID" ; 
			selectQry += ", dbo.V_Dataset_Folder_Paths.Archive_Folder_Path AS ArchPath" ; 
            selectQry += ", dbo.V_Dataset_Folder_Paths.Dataset_Folder_Path AS DatasetFullPath" ;
			selectQry += " FROM dbo.T_Dataset INNER JOIN dbo.T_Analysis_Job ON " ; 
			selectQry += " dbo.T_Dataset.Dataset_ID = dbo.T_Analysis_Job.AJ_datasetID " ; 
			selectQry += " INNER JOIN dbo.T_Experiments ON dbo.T_Dataset.Exp_ID = dbo.T_Experiments.Exp_ID" ; 
			selectQry += " INNER JOIN dbo.T_Instrument_Name ON dbo.T_Dataset.DS_instrument_name_id = dbo.T_Instrument_Name.Instrument_ID " ; 
			selectQry += " INNER JOIN dbo.t_storage_path ON dbo.T_Dataset.DS_storage_path_ID = dbo.t_storage_path.SP_path_ID " ;
			selectQry += " left outer JOIN dbo.T_Requested_Run_History ON dbo.T_Requested_Run_History.DatasetID = dbo.T_Dataset.Dataset_ID" ; 
			selectQry += " INNER JOIN dbo.V_Dataset_Folder_Paths ON dbo.T_Dataset.Dataset_ID = dbo.V_Dataset_Folder_Paths.Dataset_ID" ;
			selectQry += " WHERE  (dbo.T_Analysis_Job.AJ_analysisToolID = 2 OR dbo.T_Analysis_Job.AJ_analysisToolID = 7 OR dbo.T_Analysis_Job.AJ_analysisToolID = 10 OR dbo.T_Analysis_Job.AJ_analysisToolID = 11 OR dbo.T_Analysis_Job.AJ_analysisToolID = 12 OR dbo.T_Analysis_Job.AJ_analysisToolID = 16 OR dbo.T_Analysis_Job.AJ_analysisToolID = 18 ) " ; 
			selectQry += " AND dbo.T_Dataset.Dataset_ID = " + datasetId ;

			mstrQuery = selectQry ; 
		}


		private void GetData(string datasetId, string alias)
		{
			string pekFilePath ;
			string FileName ;

			string server = "gigasax" ; 
			string userName = "dmswebuser" ; 
			string passwd = "icr4fun" ; 
			string cString = String.Format("database=DMS5;server={0};user id={1};Password={2}", 
				server, userName, passwd);

			//string cString = "Persist Security Info=False;Integrated Security=SSPI;database=DMS5;server=gigasax";
			SqlConnection myConnection = new SqlConnection(cString);
			myConnection.Open();


			SqlCommand myCommand = new SqlCommand(mstrQuery,myConnection);
			myCommand.CommandType = CommandType.Text;
			SqlDataReader myReader = myCommand.ExecuteReader();
			try 
			{
				Type labelType = typeof(MultiAlign.LabelingType) ; 
				MultiAlign.LabelingType [] labelTypes = (MultiAlign.LabelingType []) Enum.GetValues(labelType) ; 
				while (myReader.Read()) 
				{
					MultiAlign.clsDatasetInfo datasetInfo = new MultiAlign.clsDatasetInfo() ; 
					datasetInfo.mstrDatasetId = Convert.ToString(myReader.GetInt32(0)) ; 
					datasetInfo.mstrVolume = myReader.GetString(1) ; 
					datasetInfo.mstrInstrumentFolder = myReader.GetString(2) ; 
					datasetInfo.mstrDatasetPath = myReader.GetString(3) ;
					datasetInfo.mstrResultsFolder = myReader.GetString(4) ;
					//datasetInfo.mstrDatasetName = myReader.GetString(5) ;
					datasetInfo.mstrAnalysisJobId = Convert.ToString(myReader.GetInt32(6)) ;

					datasetInfo.mintColumnID = myReader.GetInt32(7) ;
					datasetInfo.mdateAcquisitionStart = myReader.GetDateTime(8) ;
					string labelMedia = myReader.GetString(9) ; 
					labelMedia.Replace("_", "") ; 
					labelMedia.Replace(" ", "") ; 
					labelMedia.Replace("/", "") ; 
					labelMedia.Replace("-", "") ; 
					foreach (MultiAlign.LabelingType type in labelTypes)
					{
						if (type.ToString() == labelMedia)
						{
							datasetInfo.menmLabelingType = type ; 
							break ; 
						}
					}
					datasetInfo.mstrInstrment = myReader.GetString(10) ; 
					int toolId = myReader.GetInt32(11) ; 
					if (toolId == 16 || toolId == 18)
					{
						datasetInfo.menmDeisotopingTool = MultiAlign.DeisotopingTool.Decon2LS ; 
					}
					else
					{
						datasetInfo.menmDeisotopingTool = MultiAlign.DeisotopingTool.ICR2LS ; 
					}
					if (!myReader.IsDBNull(12))
					{
						datasetInfo.mintBlockID = myReader.GetInt32(12) ; 
					}
					else
					{
						datasetInfo.mintBlockID = 0 ; 
					}
					if (!myReader.IsDBNull(13))
					{
						datasetInfo.mstrReplicateName = myReader.GetString(13) ; 
					}
					else
					{
						datasetInfo.mstrReplicateName = "" ; 
					}
					if (!myReader.IsDBNull(14))
					{
						datasetInfo.mintExperimentID = myReader.GetInt32(14) ; 
					}
					else
					{
						datasetInfo.mintExperimentID = 0 ; 
					}
					if (!myReader.IsDBNull(15))
					{
						datasetInfo.mintRunOrder = myReader.GetInt32(15) ; 
					}
					else
					{
						datasetInfo.mintRunOrder = 0 ; 
					}
					if (!myReader.IsDBNull(16))
					{
						datasetInfo.mintBatchID = myReader.GetInt32(16) ; 
					}
					else
					{
						datasetInfo.mintBatchID = 0 ; 
					}

					pekFilePath = myReader.GetString(17) + "\\" + datasetInfo.mstrResultsFolder ;
					FileName = GetFileNameFromDBPath(pekFilePath) ;
					datasetInfo.mstrLocalPath = pekFilePath + "\\" + FileName ;
					datasetInfo.mstrDatasetName = FileName ;

					datasetInfo.mstrAlias = alias ; 
					datasetInfo.selected = false;
					marrDatasetInfoCurrent.Add(datasetInfo) ; 
				}//while
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message +  "DMS get information error") ;  
			}
			finally 
			{
				// always call Close when done reading.
				myReader.Close();
				// always call Close when done reading.
				myConnection.Close();
			}

		}


		public ArrayList DatasetInfo
		{
			get
			{
				return marrDatasetInfo ;
			}
			set
			{
				marrDatasetInfo = value ;
			}
		}		
	}
}
