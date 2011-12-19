using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Factors;
using MultiAlignEngine;
using System.IO;

namespace MultiAlignCore.Data
{
	/// <summary>
	/// Contains information about a dataset used for analysis.r
	/// </summary>
	[Serializable]
	public class DatasetInformation : IComparable<DatasetInformation>
	{
		public static int mintNumFactorsSpecified = 0;
		public const int MAX_LEVELS = 100;

		/// <summary>
		/// Dictionary that maps a factor object to it's factor value.
		/// </summary>
		private Dictionary<FactorInformation, string> m_factorInformation;				
		[DataSummaryAttribute("Volume")]
		public string mstrVolume;
		[DataSummaryAttribute("Instrument Folder")]
		public string mstrInstrumentFolder;
		[DataSummaryAttribute("Results Folder")]
		public string mstrResultsFolder;
		[DataSummaryAttribute("Dataset Name")]
		private string mstrDatasetName;
		[DataSummaryAttribute("Analysis Job ID")]
		public string mstrAnalysisJobId;	
		private string m_path;
		[DataSummaryAttribute("Alias")]
		public string mstrAlias;
		[DataSummaryAttribute("Column ID")]
		public int mintColumnID;
		[DataSummaryAttribute("Acquisition Start")]
		public DateTime mdateAcquisitionStart;
		[DataSummaryAttribute("Instrument")]
		public string mstrInstrment;
        [DataSummaryAttribute("Labeling Type")]
        public string LabelingType
        {
            get;
            set;
        }
		[DataSummaryAttribute("Deisotoping Tool")]
		public DeisotopingTool menmDeisotopingTool;
		[DataSummaryAttribute("Comment")]
		public string mstrComment;
		[DataSummaryAttribute("Operator")]
		public string mstrOperator;
		/// <summary>
		/// batch in which things were run.
		/// </summary>
		[DataSummaryAttribute("Batch ID")]
		public int mintBatchID;
		/// <summary>
		/// block inside the batch in which 
		/// </summary>
		[DataSummaryAttribute("Block ID")]
		public int mintBlockID;
		/// <summary>
		/// Experiment from which this sample is generated. 
		/// </summary>
		[DataSummaryAttribute("Experiment ID")]
		public int mintExperimentID;
		/// <summary>
		/// Alternative name for the experiment concatenated with a _[replicate num].
		/// </summary>
		[DataSummaryAttribute("Replicate Name")]
		public string mstrReplicateName;
		/// <summary>
		/// order in which things were run in a block.
		/// </summary>
		[DataSummaryAttribute("Run Order")]
		public int mintRunOrder;
		/// <summary>
		/// Name for blocking factor. Its a one to one mapping with mintBlockID for datasets in a batch
		/// </summary>
		[DataSummaryAttribute("Blocking Factor")]
		public string mstrBlockingFactor;
		/// <summary>
		/// Name of the parameter file used.
		/// </summary>
		private string m_parameterFileName;

		//TODO: Clean this up and remove it
		private bool m_isSelected;

		/// <summary>
		/// The List of Factor Objects
		/// </summary>
		private IList<Factor> m_factorList;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public DatasetInformation()
		{
			mintBatchID  = 0;
			mintRunOrder = 0;

			m_factorInformation = new Dictionary<FactorInformation, string>();
			m_factorList = new List<Factor>();
			m_parameterFileName = "";
			m_path = "";
		}

		#region Properties
        /// <summary>
        /// Gets or sets the key used for access to the db.
        /// </summary>
        public int DMSDatasetID
        {
            get;
            set;
        }
		/// <summary>
		/// Gets or sets the ID of the dataset
		/// </summary>
        [DataSummaryAttribute("Dataset ID")]
        public int DatasetId
        {
            get;
            set;
        }
		/// <summary>
		/// Gets or sets the ID of the Analysis Job
		/// </summary>
		[DataSummaryAttribute("Analysis Job ID")]
		public string JobId
		{
			get { return mstrAnalysisJobId; }
			set { mstrAnalysisJobId = value; }
		}
		/// <summary>
		/// Gets or sets the name of the dataset
		/// </summary>
		[DataSummaryAttribute("Dataset Name")]
		public string DatasetName
		{
			get { return mstrDatasetName; }
			set { mstrDatasetName = value; }
		}
		/// <summary>
		/// Gets or sets the archive path.
		/// </summary>
		[DataSummaryAttribute("Path")]
		public string Path
		{
			get
			{
				return m_path;
			}
			set
			{
				m_path = value;
			}
		}
		/// <summary>
		/// Gets or sets the name of the parameter file used to peak pick the data.
		/// </summary>
		[DataSummaryAttribute("Parameter File Path")]
		public string ParameterFileName
		{
			get
			{
				return m_parameterFileName;
			}
			set
			{
				m_parameterFileName = value;
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

		public override bool Equals(object obj)
		{
			DatasetInformation dataset = (DatasetInformation)obj;

			if (dataset == null)
			{
				return false;
			}
			else if (!this.DatasetId.Equals(dataset.DatasetId))
			{
				return false;
			}
			else
			{
				return this.JobId.Equals(dataset.JobId);
			}
		}

		public override int GetHashCode()
		{
			int hash = 17;

			hash = hash * 23 + DatasetId.GetHashCode();
			hash = hash * 23 + mstrAnalysisJobId.GetHashCode();

			return hash;
		}

		#region IComparable<DatasetInformation> Members
		public int CompareTo(DatasetInformation other)
		{
			return mstrAnalysisJobId.CompareTo(other.mstrAnalysisJobId);
		}
		#endregion

        /// <summary>
        /// Cleans dataset names of extensions in case the data as not loaded from DMS, but manually.
        /// </summary>
        /// <returns></returns>
        public static string CleanNameDatasetNameOfExtensions(string path)
        {
            string datasetName  = path.Replace("_isos.csv", "");
            datasetName         = datasetName.Replace(".scans", "");
            datasetName         = datasetName.Replace("LCMSFeatures.txt", "");
            datasetName         = System.IO.Path.GetFileNameWithoutExtension(datasetName);
            return datasetName;                
        }
	}
}
