using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignEngine;
using PNNLProteomics.Data.Factors;

namespace PNNLProteomics.Data
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
            Datasets        = new List<DatasetInformation>();
            InputFile       = null;
            ParameterFile   = null;
            FactorTreeNode  = null;
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
        /// Gets or sets the input file used.
        /// </summary>
		[clsDataSummaryAttribute("Input File Name")]
        public string InputFile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the parameter file used.
        /// </summary>
        [clsDataSummaryAttribute("Parameter File")]
        public string ParameterFile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the job id for this analysis.
        /// </summary>
        [clsDataSummaryAttribute("Job ID")]
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
        [clsDataSummaryAttribute("Analysis Name")]
        public string AnalysisName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the pathname associated with the analysis.
        /// </summary>
        [clsDataSummaryAttribute("Analysis Path")]
        public string AnalysisPath
        {
            get;
            set;
        } 
    }
}
