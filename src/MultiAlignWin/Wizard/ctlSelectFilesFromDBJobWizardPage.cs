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


	}
}
