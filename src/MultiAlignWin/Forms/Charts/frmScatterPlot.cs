using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PNNLControls;

using PNNLProteomics.Data.Analysis;
using MultiAlignWin.Forms.Factors;

using PNNLProteomics.Data.Factors;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmScatterPlot.
	/// </summary>
	public class frmScatterPlot : System.Windows.Forms.Form
	{
		private MultiAlignAnalysis mobjAnalysis ; 
		private bool mblnShowNormalizedIntensity = false ;
		private bool mblnIdentifiedOnly = false ; 
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemShowNormalizedAbundance;
		private System.Windows.Forms.MenuItem menuItemIdentifiedOnly;
		private PNNLControls.ctlScatterPlot mctlScatterPlot;
		private System.Windows.Forms.MenuItem mnuChangeFactorOrder;
		//private frmLabelGroupEdit m_factorEdit = new frmLabelGroupEdit();

        private float[,] m_data;
        private IContainer components;

		public frmScatterPlot()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			mctlScatterPlot = new PNNLControls.ctlScatterPlot() ; 
			mctlScatterPlot.Dock = DockStyle.Fill ; 
			Controls.Add(mctlScatterPlot) ; 
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScatterPlot));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemShowNormalizedAbundance = new System.Windows.Forms.MenuItem();
            this.menuItemIdentifiedOnly = new System.Windows.Forms.MenuItem();
            this.mnuChangeFactorOrder = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
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
            this.menuItemShowNormalizedAbundance,
            this.menuItemIdentifiedOnly,
            this.mnuChangeFactorOrder});
            this.menuItem1.Text = "Options";
            // 
            // menuItemShowNormalizedAbundance
            // 
            this.menuItemShowNormalizedAbundance.Index = 0;
            this.menuItemShowNormalizedAbundance.Text = "Show Normalized Intensity";
            this.menuItemShowNormalizedAbundance.Click += new System.EventHandler(this.menuItemShowNormalizedAbundance_Click);
            // 
            // menuItemIdentifiedOnly
            // 
            this.menuItemIdentifiedOnly.Index = 1;
            this.menuItemIdentifiedOnly.Text = "Show Identified Only";
            this.menuItemIdentifiedOnly.Click += new System.EventHandler(this.menuItemIdentifiedOnly_Click);
            // 
            // mnuChangeFactorOrder
            // 
            this.mnuChangeFactorOrder.Index = 2;
            this.mnuChangeFactorOrder.Text = "Change Factor Order";
            this.mnuChangeFactorOrder.Click += new System.EventHandler(this.mnuChangeFactorOrder_Click);
            // 
            // frmScatterPlot
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(849, 523);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "frmScatterPlot";
            this.Text = "Correlation Plot";
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="identified_only"></param>
		private void SetScatterPlotData(bool identified_only)
		{		
			if (mobjAnalysis == null || (identified_only && mobjAnalysis.PeakMatchingResults == null))
				return ; 

			ctlScatterPlot scatterPlot = mctlScatterPlot ; 
			clsLabelAttributes labelRoot		= new clsLabelAttributes();
			classTreeNodeConverter rootConverter	= new classTreeNodeConverter();
			classTreeNode root;
			
			//root = mobjAnalysis.BuildFactorTreeNode();			
			//rootConverter.BuildLabels(root, ref labelRoot);

			int numDatasets = mobjAnalysis.UMCData.NumDatasets;		
			int numClusters = mobjAnalysis.UMCData.mobjClusterData.NumClusters ; 
	
			scatterPlot.VerticalLabel.Root	 = labelRoot;
			scatterPlot.HorizontalLabel.Root = labelRoot;

			scatterPlot.HorizontalLabel.MinLeafHeight = 100;
			scatterPlot.VerticalLabel.MinLeafHeight = 100;			
						
			scatterPlot.HorizontalLabel.Init();  			
			scatterPlot.VerticalLabel.Init();  

			MultiAlignEngine.PeakMatching.clsPeakMatchingResults.clsPeakMatchingTriplet [] arrPeakMatchingTriplets = null ; 
			MultiAlignEngine.MassTags.clsProtein [] arrProteins = null ; 
			MultiAlignEngine.MassTags.clsMassTag [] arrMassTags = null ; 
			
			int numTriplets = 0 ; 
			if (mobjAnalysis.PeakMatchingResults != null)
			{
				arrPeakMatchingTriplets = mobjAnalysis.PeakMatchingResults.marrPeakMatchingTriplet ; 
				numTriplets = arrPeakMatchingTriplets.Length ; 
				arrProteins = mobjAnalysis.PeakMatchingResults.marrProteins ; 
				arrMassTags = mobjAnalysis.PeakMatchingResults.marrMasstags ; 
			}

			double [] clusterIntensity = null ; 			
			if (mblnShowNormalizedIntensity)
				clusterIntensity = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensityNormalized ; 
			else
				clusterIntensity = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensity ; 

			// create the data. 
			float [,] data = null ;
			
			if (identified_only)
			{
				data = new float[numTriplets, numDatasets];
			}
			else
			{
				data = new float[numClusters, numDatasets];
			}

            long[] mapping = null; // this.mobjAnalysis.FactorTree.DatasetMapping;

			int numAdded = 0 ; 
			if (identified_only)
			{
				for (int i=0; i<numTriplets; i++)
				{
					int clusterNum = arrPeakMatchingTriplets[i].mintFeatureIndex ; 
					for (int j=0; j<numDatasets; j++)
					{
						data[numAdded,mapping[j]] = (float) clusterIntensity[clusterNum*numDatasets + j] ; 
					}
					numAdded++; 
				}
			}
			else
			{
				for (int i=0; i < numClusters ; i++)
				{
					for (int j=0; j<numDatasets && j < mapping.Length; j++)
					{
						
						data[i,mapping[j]] = (float) clusterIntensity[i*numDatasets + j] ; 
					}
				}
			}

			m_data = data;
			scatterPlot.InitScatterPlot(data, labelRoot);
		}		

		public void SetAnalysis(MultiAlignAnalysis analysis)
		{
			mobjAnalysis = analysis ;
			menuItemShowNormalizedAbundance.Enabled = mobjAnalysis.UMCData.mobjClusterData.IsDataNormalized ; 
			if (!mobjAnalysis.PeakMatchedToMassTagDB)
			{
				mblnIdentifiedOnly = false ; 
				menuItemIdentifiedOnly.Enabled = false ; 
			}

			SetScatterPlotData(mblnIdentifiedOnly) ; 
		}

		private void menuItemShowNormalizedAbundance_Click(object sender, System.EventArgs e)
		{
			mblnShowNormalizedIntensity = !mblnShowNormalizedIntensity ; 
			menuItemShowNormalizedAbundance.Checked = mblnShowNormalizedIntensity ; 
			SetScatterPlotData(mblnIdentifiedOnly) ; 
		}

		private void menuItemIdentifiedOnly_Click(object sender, System.EventArgs e)
		{
			mblnIdentifiedOnly = !mblnIdentifiedOnly ; 
			menuItemIdentifiedOnly.Checked = mblnIdentifiedOnly ; 
			SetScatterPlotData(mblnIdentifiedOnly) ; 		
		}

		private void mnuChangeFactorOrder_Click(object sender, System.EventArgs e)
		{	
            //m_factorEdit.FactorTree = mobjAnalysis.FactorTree;

            //DialogResult result = m_factorEdit.ShowDialog();	
            //if (result == DialogResult.OK)
            //{	
            //    clsLabelAttributes labelRoot = new clsLabelAttributes();
            //    classTreeNodeConverter rootConverter = new classTreeNodeConverter();

            //    m_factorEdit.BuildTree();
            //    mobjAnalysis.FactorTree = m_factorEdit.FactorTree;	
            //    SetScatterPlotData(mblnIdentifiedOnly);

            //}
		}

	}
}
