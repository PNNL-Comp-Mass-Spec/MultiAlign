using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmClustergram.
	/// </summary>
	public class frmClustergram : System.Windows.Forms.Form
	{
		private PNNLControls.ctlHeatMap mctlHeatMap;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private clsMultiAlignAnalysis mobjAnalysis ; 
		//private float [,] marrIntensityData = null ; 
		
		private Hashtable mhashProteinCount  ; 
		private MultiAlignEngine.PeakMatching.clsPeakMatchingResults.clsPeakMatchingTriplet [] marrPeakMatchingTriplets ;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItemMinObs; 
		private bool mblnPeakMatchClusterStatisticsCalculated = false;
		private bool mblnShowNormalizedIntensity = false ;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemSetMinObservations;
		private System.Windows.Forms.MenuItem menuItemShowNormalizedAbundance;
		private System.Windows.Forms.MenuItem menuItemClusterData;
		private int mintMinObservationCount = 1 ; 
		private int mintClusterSize = 5 ; 

		public frmClustergram()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.mctlHeatMap = new PNNLControls.ctlHeatMap() ; 
			mctlHeatMap.Dock = DockStyle.Fill ; 
			this.Controls.Add(mctlHeatMap) ; 
		}

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
                mobjAnalysis = null;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmClustergram));
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItemMinObs = new System.Windows.Forms.MenuItem();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemSetMinObservations = new System.Windows.Forms.MenuItem();
			this.menuItemShowNormalizedAbundance = new System.Windows.Forms.MenuItem();
			this.menuItemClusterData = new System.Windows.Forms.MenuItem();
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemMinObs});
			// 
			// menuItemMinObs
			// 
			this.menuItemMinObs.Index = 0;
			this.menuItemMinObs.Text = "Specify Minimum Number of Observations for a Protein";
			this.menuItemMinObs.Click += new System.EventHandler(this.menuItemMinObs_Click);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemSetMinObservations,
																					  this.menuItemShowNormalizedAbundance,
																					  this.menuItemClusterData});
			this.menuItem1.Text = "Options";
			// 
			// menuItemSetMinObservations
			// 
			this.menuItemSetMinObservations.Index = 0;
			this.menuItemSetMinObservations.Text = "Set Minimum Observations";
			this.menuItemSetMinObservations.Click += new System.EventHandler(this.menuItemSetMinObservations_Click);
			// 
			// menuItemShowNormalizedAbundance
			// 
			this.menuItemShowNormalizedAbundance.Index = 1;
			this.menuItemShowNormalizedAbundance.Text = "Show Normalized Abundance";
			this.menuItemShowNormalizedAbundance.Click += new System.EventHandler(this.menuItemShowNormalizedAbundance_Click);
			// 
			// menuItemClusterData
			// 
			this.menuItemClusterData.Index = 2;
			this.menuItemClusterData.Text = "Cluster Data";
			this.menuItemClusterData.Click += new System.EventHandler(this.menuItemClusterData_Click);
			// 
			// frmClustergram
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(616, 653);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "frmClustergram";
			this.Text = "Peptide Heatmap";

		}
		#endregion


		#region "Calculating Statistics, Setting data"
		public void CalculatePeakMatchedClusterStatistics()
		{
			// For each protein count the number of observations. 
			// go through the triplets, keeping for each protein a count of the number of 
			// matches observed. 
			marrPeakMatchingTriplets = (MultiAlignEngine.PeakMatching.clsPeakMatchingResults.clsPeakMatchingTriplet []) 
				mobjAnalysis.PeakMatchingResults.marrPeakMatchingTriplet.Clone() ; 
			Array.Sort(marrPeakMatchingTriplets) ; 
			int numTriplets = marrPeakMatchingTriplets.Length ; 
			mhashProteinCount = new System.Collections.Hashtable() ; 
			for (int tripletNum = 0 ; tripletNum < numTriplets ; tripletNum++)
			{
				if (mhashProteinCount.ContainsKey(marrPeakMatchingTriplets[tripletNum].mintProteinIndex))
				{
					mhashProteinCount[marrPeakMatchingTriplets[tripletNum].mintProteinIndex] = (int)mhashProteinCount[marrPeakMatchingTriplets[tripletNum].mintProteinIndex] + 1 ; 
				}
				else
				{
					mhashProteinCount[marrPeakMatchingTriplets[tripletNum].mintProteinIndex] = 1 ; 
				}
			}
			mblnPeakMatchClusterStatisticsCalculated = true ;
			menuItemShowNormalizedAbundance.Enabled = mobjAnalysis.UMCData.mobjClusterData.IsDataNormalized ; 
			if (menuItemShowNormalizedAbundance.Enabled)
			{
				menuItemShowNormalizedAbundance.Checked = mblnShowNormalizedIntensity ; 
			}

		}

		public void SetDefaultLayoutDataForPeakMatchedClusters(bool addToProteinNode)
		{
			if (mobjAnalysis == null || mobjAnalysis.PeakMatchingResults == null)
				return ; 
			if (!mblnPeakMatchClusterStatisticsCalculated)
				CalculatePeakMatchedClusterStatistics() ; 

			int numDatasets = mobjAnalysis.UMCData.NumDatasets ; 

			MultiAlignEngine.MassTags.clsProtein [] arrProteins = mobjAnalysis.PeakMatchingResults.marrProteins ; 
			MultiAlignEngine.MassTags.clsMassTag [] arrMassTags = mobjAnalysis.PeakMatchingResults.marrMasstags ; 
			double [] clusterIntensity = null ; 
			
			if (mblnShowNormalizedIntensity)
				clusterIntensity = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensityNormalized ; 
			else
				clusterIntensity = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensity ; 

			// now go through each protein and add to labels. 

			mctlHeatMap.HorizontalLabel.MinLeafHeight = 100;
			mctlHeatMap.VerticalLabel.MinLeafHeight = 20;

			PNNLControls.clsLabelAttributes root = mctlHeatMap.VerticalLabel.GetRoot();

			
			

			root = new PNNLControls.clsLabelAttributes();
			PNNLControls.clsLabelAttributes proteinNode = null ;
			// go through each protein. 
			int tripletNum = 0 ; 
			int numTriplets = marrPeakMatchingTriplets.Length ; 
			int lastProteinId = -1 ; 
			while(tripletNum < numTriplets)
			{
				MultiAlignEngine.PeakMatching.clsPeakMatchingResults.clsPeakMatchingTriplet peakTriplet 
					= marrPeakMatchingTriplets[tripletNum] ; 
				// if there is a new protein, add the previous guy to the heatmap
				// and start a new protein label.
				if (addToProteinNode)
				{
					if ((int) mhashProteinCount[marrPeakMatchingTriplets[tripletNum].mintProteinIndex]
						< mintMinObservationCount)
					{
						tripletNum++ ;
						continue ; 
					}
					if (lastProteinId != peakTriplet.mintProteinIndex)
					{
						proteinNode = root.AddBranch(arrProteins[peakTriplet.mintProteinIndex].mstrProteinName) ; 
						lastProteinId = peakTriplet.mintProteinIndex ; 
					}
					proteinNode.AddBranch(arrMassTags[peakTriplet.mintMassTagIndex].mstrPeptide) ; 
					tripletNum++ ; 
				}
				else
				{
					if ((int) mhashProteinCount[marrPeakMatchingTriplets[tripletNum].mintProteinIndex]
						< mintMinObservationCount)
					{
						tripletNum++ ;
						continue ; 
					}
					else
					{
						root.AddBranch(arrMassTags[peakTriplet.mintProteinIndex].mstrPeptide) ; 
						tripletNum++ ; 
					}
				}
			}

			// create the data. 
			int numAdded = 0 ; 
			int numPassingMTs = 0 ; 
			for (int i=0; i<numTriplets; i++)
			{
				if ((int) mhashProteinCount[marrPeakMatchingTriplets[i].mintProteinIndex]
					< mintMinObservationCount)
				{
					continue ; 
				}
				numPassingMTs++; 
			}
			float [,] data = new float[numPassingMTs, numDatasets];
			for (int i=0; i<numTriplets; i++)
			{
				if ((int) mhashProteinCount[marrPeakMatchingTriplets[i].mintProteinIndex]
					< mintMinObservationCount)
				{
					continue ; 
				}
				for (int j=0; j<numDatasets; j++)
				{
					data[numAdded,j] = (float) clusterIntensity[marrPeakMatchingTriplets[i].mintFeatureIndex *numDatasets + j] ; 
				}
				numAdded++; 
			}

			Cephes.clsMathUtilities.Standardize(data) ; 
			mctlHeatMap.Data = data;

			//mctlHeatMap.VerticalLabel.SortFunction = new CompareLabel(data);

			mctlHeatMap.VerticalLabel.Root = root;

			//build horizontal axis, with the datasetInfo. 
			System.Collections.ArrayList arrFileInfo = mobjAnalysis.Files ; 
			int numFactors = MultiAlignEngine.clsDatasetInfo.mintNumFactorsSpecified ; 


			root = mctlHeatMap.HorizontalLabel.Root ; 
			
			System.Collections.ArrayList arrLabels = new System.Collections.ArrayList() ; 
			System.Collections.Hashtable lev1Hash = new Hashtable() ; 

			int numValues = 0 ;
			for (int j = 0; j < numDatasets; j++)
			{
				if (((MultiAlignEngine.clsDatasetInfo)arrFileInfo[j]).AssignedFactorValues.Count == 0)
					continue ; 
				string enumVal = (string)(((MultiAlignEngine.clsDatasetInfo)arrFileInfo[j]).AssignedFactorValues[0]) ; 
				if (!lev1Hash.ContainsKey(enumVal))
				{
					arrLabels.Add(root.AddBranch(enumVal)) ; 
					lev1Hash[enumVal] = numValues++ ; 
				}
			}

			for (int j = 0; j < numDatasets; j++)
			{
				if (((MultiAlignEngine.clsDatasetInfo)arrFileInfo[j]).AssignedFactorValues.Count == 0)
				{
					root.AddBranch(mobjAnalysis.UMCData.DatasetName[j]);
				}
				else
				{
					int lev1Index = (int) lev1Hash[((MultiAlignEngine.clsDatasetInfo)arrFileInfo[j]).AssignedFactorValues[0]] ; 
					((PNNLControls.clsLabelAttributes)arrLabels[lev1Index]).AddBranch(mobjAnalysis.UMCData.DatasetName[j]);
				}
			}

			mctlHeatMap.HorizontalLabel.Root = root;
			//load labels
			mctlHeatMap.VerticalLabel.LoadLabels(0, numAdded-1);  //load all labels
			//load in axis mode instead of label mode
			mctlHeatMap.HorizontalLabel.LoadLabels(0, numDatasets-1);

			mctlHeatMap.Thumb();
		}

		public void SetAnalysis(clsMultiAlignAnalysis analysis)
		{
			mobjAnalysis = analysis ;
			SetDefaultLayoutDataForPeakMatchedClusters(true) ; 
		}

		#endregion

		#region "R clustering"
		private int[] GetClusterIndices(float[,]data)
		{
            //try
            //{
            //    STATCONNECTORSRVLib.IStatConnector RConnector;
            //    Type lType = Type.GetTypeFromProgID("StatConnectorSrv.StatConnector");
            //    RConnector = (STATCONNECTORSRVLib.IStatConnector)Activator.CreateInstance(lType);
            //    RConnector.Init("R");

            //    RConnector.SetSymbol("x", data);
            //    RConnector.SetSymbol("k", mintClusterSize);
            //    RConnector.EvaluateNoReturn("cl<-kmeans(x,k)");

            //    RConnector.EvaluateNoReturn("y <- cl$cluster");

            //    //RConnector.EvaluateNoReturn("plot(y)");


            //    object a = RConnector.GetSymbol("y");
            //    int[]indices = (int[])a;

            //    //object[] o = (object[]) RConnector.GetSymbol("cl");

            //    RConnector.Close();

            //    return indices;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message + ex.StackTrace) ; 
            //}
			return null;
		}

		private void ClusterData()
		{
			try
			{
				//build cluster indices for currently selected data

				float[,] data = (float [,])mctlHeatMap.Data.Clone() ;  //replace nulls with 0s for clustering
				int x = data.GetUpperBound(0)+1;
				int y = data.GetUpperBound(1)+1;
//				Cephes.clsMathUtilities.Standardize(data, 0) ; 
				Cephes.clsMathUtilities.Replace(data, 0) ; 
				int[] indices = GetClusterIndices(data);

				//get the label array, group by cluster, sort by group
				ArrayList branches = mctlHeatMap.VerticalLabel.Root.branches;
				if (indices.Length != branches.Count)
					return;

				PNNLControls.clsLabelAttributes newRoot = new PNNLControls.clsLabelAttributes();
				newRoot.branches = new ArrayList();
				for (int i=0; i<indices.Length; i++)
				{
					int tag = indices[i]-1;
					PNNLControls.clsLabelAttributes branch = branches[i]as PNNLControls.clsLabelAttributes;
					branch.groupTag = tag;
					PNNLControls.clsLabelAttributes cluster = null;
					while (tag > newRoot.branches.Count-1)
					{
						cluster = newRoot.AddBranch(newRoot.branches.Count.ToString());
						cluster.groupTag =  newRoot.branches.Count-1;
					}
					cluster = newRoot.branches[tag] as PNNLControls.clsLabelAttributes;
					cluster.AddBranch(branch);
				}

				//sort the clusters  on the group tag
				newRoot.branches.Sort(new PNNLControls.clsLabelAttributes.SortGroupTag());

				//			   int count = newRoot.branches.Count;
				//				for (int i=0; i<count; i++)
				//				{
				//					PNNLControls.clsLabelAttributes l =  newRoot.branches[i] as PNNLControls.clsLabelAttributes;
				//					l.branches.Sort(new PNNLControls.clsLabelAttributes.SortDataTag ());
				//				}



				mctlHeatMap.VerticalLabel.Root = newRoot ;
				//load labels
				mctlHeatMap.VerticalLabel.LoadLabels(0, branches.Count-1);  //load all labels
				//load in axis mode instead of label mode
				mctlHeatMap.HorizontalLabel.LoadLabels(0, y-1);
				mctlHeatMap.Thumb();

			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace);
			}
		}

		#endregion 

		#region "Menu Actions"
		private void menuItemMinObs_Click(object sender, System.EventArgs e)
		{
			frmInputBlockSize blockSizeForm = new frmInputBlockSize() ; 
			blockSizeForm.LabelText = "Min Observations:" ; 
			if (blockSizeForm.ShowDialog(this) == DialogResult.OK)
			{
				mintMinObservationCount = blockSizeForm.blockSize ; 
				SetDefaultLayoutDataForPeakMatchedClusters(true) ; 
			}
		}

		private void menuItemSetMinObservations_Click(object sender, System.EventArgs e)
		{
			menuItemMinObs_Click(sender, e) ; 
		}

		private void menuItemShowNormalizedAbundance_Click(object sender, System.EventArgs e)
		{
			mblnShowNormalizedIntensity = !mblnShowNormalizedIntensity ; 
			menuItemShowNormalizedAbundance.Checked = mblnShowNormalizedIntensity ; 
			SetDefaultLayoutDataForPeakMatchedClusters(true) ; 
		}
		#endregion

		private void menuItemClusterData_Click(object sender, System.EventArgs e)
		{
			frmInputBlockSize blockSizeForm = new frmInputBlockSize() ; 
			blockSizeForm.LabelText = "Number of Clusters:" ; 
			if (blockSizeForm.ShowDialog(this) == DialogResult.OK)
			{
				mintClusterSize = blockSizeForm.blockSize ; 
			}
			SetDefaultLayoutDataForPeakMatchedClusters(false) ; 
			ClusterData() ; 
		}
	}
}
