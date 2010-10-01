using System;
using System.Collections.Generic;

using MultiAlignEngine;
using PNNLProteomics.Data.Factors;
using PNNLProteomics.MultiAlign.Hibernate.Domain;
using Iesi.Collections.Generic;

namespace PNNLProteomics.Data
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

		//TODO: Encapsulate this data
		[clsDataSummaryAttribute("Dataset ID")]
		public string mstrDatasetId;
		[clsDataSummaryAttribute("Volume")]
		public string mstrVolume;
		[clsDataSummaryAttribute("Instrument Folder")]
		public string mstrInstrumentFolder;
		[clsDataSummaryAttribute("Results Folder")]
		public string mstrResultsFolder;
		[clsDataSummaryAttribute("Dataset Name")]
		private string mstrDatasetName;
		[clsDataSummaryAttribute("Analysis Job ID")]
		public string mstrAnalysisJobId;
		[clsDataSummaryAttribute("Local Path")]
		public string mstrLocalPath;
		private string m_archivePath;
		[clsDataSummaryAttribute("Alias")]
		public string mstrAlias;
		[clsDataSummaryAttribute("Column ID")]
		public int mintColumnID;
		[clsDataSummaryAttribute("Acquisition Start")]
		public DateTime mdateAcquisitionStart;
		[clsDataSummaryAttribute("Instrument")]
		public string mstrInstrment;
		[clsDataSummaryAttribute("Labeling Type")]
		public LabelingType menmLabelingType;
		[clsDataSummaryAttribute("Deisotoping Tool")]
		public DeisotopingTool menmDeisotopingTool;
		[clsDataSummaryAttribute("Comment")]
		public string mstrComment;
		[clsDataSummaryAttribute("Operator")]
		public string mstrOperator;
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
		public string mstrReplicateName;
		/// <summary>
		/// order in which things were run in a block.
		/// </summary>
		[clsDataSummaryAttribute("Run Order")]
		public int mintRunOrder;
		/// <summary>
		/// Name for blocking factor. Its a one to one mapping with mintBlockID for datasets in a batch
		/// </summary>
		[clsDataSummaryAttribute("Blocking Factor")]
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
			mintBatchID = 0;
			mintRunOrder = 0;

			m_factorInformation = new Dictionary<FactorInformation, string>();
			m_factorList = new List<Factor>();
			m_parameterFileName = "";
			m_archivePath = "";
		}

		#region Properties
		/// <summary>
		/// Gets or sets the ID of the dataset
		/// </summary>
		[clsDataSummaryAttribute("Dataset ID")]
		public string DatasetId
		{
			get { return mstrDatasetId; }
			set { mstrDatasetId = value; }
		}
		/// <summary>
		/// Gets or sets the ID of the Analysis Job
		/// </summary>
		[clsDataSummaryAttribute("Analysis Job ID")]
		public string JobId
		{
			get { return mstrAnalysisJobId; }
			set { mstrAnalysisJobId = value; }
		}
		/// <summary>
		/// Gets or sets the name of the dataset
		/// </summary>
		[clsDataSummaryAttribute("Dataset Name")]
		public string DatasetName
		{
			get { return mstrDatasetName; }
			set { mstrDatasetName = value; }
		}
		/// <summary>
		/// Gets or sets the archive path.
		/// </summary>
		[clsDataSummaryAttribute("Archive Path")]
		public string ArchivePath
		{
			get
			{
				return m_archivePath;
			}
			set
			{
				m_archivePath = value;
			}
		}
		/// <summary>
		/// Gets or sets the name of the parameter file used to peak pick the data.
		/// </summary>
		[clsDataSummaryAttribute("Parameter File Path")]
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
		/// Gets or sets the list of Factor objects for this dataset.
		/// </summary>
		public IList<Factor> FactorList
		{
			get
			{
				IList<Factor> factorList = new List<Factor>();
				foreach(KeyValuePair<FactorInformation, string> kvp in m_factorInformation)
				{
					Factor factor = new Factor();
					factor.Dataset = this;
					factor.FactorName = kvp.Key.FactorName;
					factor.FactorValue = kvp.Value;
					factorList.Add(factor);
				}

				return factorList;
			}
			set { m_factorList = value; }
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

			hash = hash * 23 + mstrDatasetId.GetHashCode();
			hash = hash * 23 + mstrAnalysisJobId.GetHashCode();

			return hash;
		}

		#region IComparable<DatasetInformation> Members
		public int CompareTo(DatasetInformation other)
		{
			return mstrAnalysisJobId.CompareTo(other.mstrAnalysisJobId);
		}
		#endregion
	}
}
