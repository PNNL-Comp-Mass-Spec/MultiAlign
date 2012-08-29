using System.Collections.Generic;
using MultiAlignCore.Data.Factors;
using MultiAlignCore.IO.InputFiles;
using MultiAlignEngine;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Class that holds meta-data information about the analysis.
    /// </summary>
    public class AnalysisMetaData
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AnalysisMetaData()
        {
            Datasets            = new List<DatasetInformation>();
            InputFileDefinition = null;
            ParameterFile       = null;
            FactorTreeNode      = null;
        }

        /// <summary>
        /// Gets or sets the list of dataset information.
        /// </summary>
        public List<DatasetInformation> Datasets
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name of the baseline dataset.
        /// </summary>
        [DataSummaryAttribute("Baseline Dataset")]
        public DatasetInformation BaselineDataset
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the input file used.
        /// </summary>
		[DataSummaryAttribute("Input File Definition Name")]
        public string InputFileDefinition
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the parameter file used.
        /// </summary>
        [DataSummaryAttribute("Parameter File")]
        public string ParameterFile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the job id for this analysis.
        /// </summary>
        [DataSummaryAttribute("Job ID")]
        public int JobID
        {
            get;
            set;
        }        
		/// <summary>
		/// Get/Set the analysis as a hiearchy of datasets with related factor information for grouping.
		/// </summary>
		public classTreeNode FactorTreeNode
		{
			get;
            set;
		}        
        /// <summary>
        /// Gets or sets the name of the analysis.
        /// </summary>
        [DataSummaryAttribute("Analysis Name")]
        public string AnalysisName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the pathname associated with the analysis.
        /// </summary>
        [DataSummaryAttribute("Analysis Path")]
        public string AnalysisPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the initial analysis setup information.
        /// </summary>
        public InputAnalysisInfo AnalysisSetupInfo
        {
            get;
            set;
        }
    }
}
