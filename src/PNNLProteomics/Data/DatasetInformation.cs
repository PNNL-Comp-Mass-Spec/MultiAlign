using System;
using System.Collections.Generic;

using MultiAlignEngine;
using PNNLProteomics.Data.Factors;

namespace PNNLProteomics.Data
{
    /// <summary>
    /// Contains information about a dataset used for analysis.r
    /// </summary>
    [Serializable]
    public class DatasetInformation: IComparable<DatasetInformation>
    {	
		 public static int mintNumFactorsSpecified  = 0; 
		 public const int MAX_LEVELS                = 100; 

		 private List<string>		        mlist_assignedValues;
         /// <summary>
         /// Dictionary that maps a factor object to it's factor value.
         /// </summary>
        private Dictionary<FactorInformation, string> m_factorInformation;

	     //TODO: Encapsulate this data
		 [clsDataSummaryAttribute("Dataset ID")]
		 public string mstrDatasetId; 
		 [clsDataSummaryAttribute("Volume")]
		 public string  mstrVolume; 
		 [clsDataSummaryAttribute("Instrument Folder")]
		 public string  mstrInstrumentFolder; 
		 [clsDataSummaryAttribute("Dataset Path")]
		 public string  mstrDatasetPath; 
		 [clsDataSummaryAttribute("Results Folder")]
		 public string  mstrResultsFolder;
		 [clsDataSummaryAttribute("Dataset Name")]
         private string mstrDatasetName;
		 [clsDataSummaryAttribute("Analysis Job ID")]
		 public string  mstrAnalysisJobId;
		 [clsDataSummaryAttribute("Local Path")]
		 public string  mstrLocalPath; 
		 [clsDataSummaryAttribute("Alias")]
		 public string  mstrAlias;
		 [clsDataSummaryAttribute("Column ID")]
         public int mintColumnID; 
		 [clsDataSummaryAttribute("Acquisition Start")]
         public DateTime mdateAcquisitionStart; 
		 [clsDataSummaryAttribute("Instrument")]
		 public string  mstrInstrment; 
		 [clsDataSummaryAttribute("Labeling Type")]
         public LabelingType menmLabelingType; 
		 [clsDataSummaryAttribute("Deisotoping Tool")]
         public DeisotopingTool menmDeisotopingTool; 
		 [clsDataSummaryAttribute("Comment")]
		 public string  mstrComment; 
		 [clsDataSummaryAttribute("Operator")]
		 public string  mstrOperator; 
	  	 /// <summary>
		 /// batch in which things were run.
		 /// </summary>
		 [clsDataSummaryAttribute("Batch ID")]
         public int mintBatchID;
		 /// <summary>
		 /// block inside the batch in which 
		 /// </summary>
		 [clsDataSummaryAttribute("Block ID")]
		 public int mintBlockID; 
		 /// <summary>
		 /// Experiment from which this sample is generated. 
		 /// </summary>
		 [clsDataSummaryAttribute("Experiment ID")]
         public int mintExperimentID; 
		 /// <summary>
		 /// Alternative name for the experiment concatenated with a _[replicate num].
		 /// </summary>
		 [clsDataSummaryAttribute("Replicate Name")]
		 public string  mstrReplicateName;
		 /// <summary>
		 /// order in which things were run in a block.
		 /// </summary>
		 [clsDataSummaryAttribute("Run Order")]
         public int mintRunOrder; 
		 /// <summary>
		 /// Name for blocking factor. Its a one to one mapping with mintBlockID for datasets in a batch
		 /// </summary>
		 [clsDataSummaryAttribute("Blocking Factor")]
		 public string  mstrBlockingFactor;

         //TODO: Clean this up and remove it
         private bool m_isSelected;
		 
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DatasetInformation()
	    {
		    mintBatchID			    = 0;
		    mintRunOrder		    = 0;

		    mlist_assignedValues	= new List<string>();
            m_factorInformation = new Dictionary<FactorInformation, string>();
        }

        #region Properties

        public string DatasetName
        {
            get { return mstrDatasetName; }
            set { mstrDatasetName = value; }
        }
        /// <summary>
        /// Gets or sets the list of factors for this dataset.
        /// </summary>
		public Dictionary<FactorInformation, string> Factors
		{
            get
            {
                return m_factorInformation;
            }
            set
            {
                m_factorInformation = value;
            }			
		}		
        /// <summary>
        /// Gets or sets the assigned factor values.
        /// </summary>
		public List<string> AssignedFactorValues
		{			
            get
            {
			    return mlist_assignedValues;
            }
            set
            {
                mlist_assignedValues = value;		
            }
		}					
		/// <summary>
		/// Gets whether this sample was part of a block.
		/// </summary>
		public bool Blocked
		{
            get
            {
				return mintBatchID > 0; 
            }
        }
        /// <summary>
        /// Gets or sets whether the dataset is selected.
        /// </summary>
        public bool Selected
        {
            get
            {
                return m_isSelected;
            }
            set
            {
                m_isSelected = value;
            }
        }
        #endregion
	

        #region IComparable<DatasetInformation> Members
        public int  CompareTo(DatasetInformation other)
        {
 	        return mstrAnalysisJobId.CompareTo(other.mstrAnalysisJobId);
        }
        #endregion
    }
}