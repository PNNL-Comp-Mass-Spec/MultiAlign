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

        /// <summary>
        /// Adds a new dataset to the list. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns>A list of added datasets</returns>
        public List<DatasetInformation> AddInputFiles(List<InputFile> inputFiles)
        {
            List<DatasetInformation> addedSets = new List<DatasetInformation>();
            Dictionary<string, DatasetInformation> datasetMap = new Dictionary<string, DatasetInformation>();
            Datasets.ForEach(x => datasetMap.Add(x.DatasetName, x));


            Dictionary<string, List<InputFile>> inputMap = new Dictionary<string, List<InputFile>>();

            foreach (InputFile file in inputFiles)
            {
                string name = System.IO.Path.GetFileName(file.Path);
                string datasetName = DatasetInformation.ExtractDatasetName(name);
                bool isEntryMade = inputMap.ContainsKey(datasetName);
                if (!isEntryMade)
                {
                    inputMap.Add(datasetName, new List<InputFile>());
                }

                inputMap[datasetName].Add(file);
            }

            int i = 0;
            foreach (string datasetName in inputMap.Keys)
            {
                List<InputFile> files                   = inputMap[datasetName];
                DatasetInformation datasetInformation   = new DatasetInformation();
                datasetInformation.DatasetId            = i++;
                datasetInformation.DatasetName          = datasetName;

                bool doesDatasetExist = datasetMap.ContainsKey(datasetName);

                // Here we map the old dataset if it existed already.
                if (datasetMap.ContainsKey(datasetName))
                {
                    datasetInformation = datasetMap[datasetName]; 
                }

                foreach (InputFile file in files)
                {
                    switch (file.FileType)
                    {
                        case InputFileType.Features:
                            datasetInformation.Features = file;
                            datasetInformation.Path = file.Path;
                            break;
                        case InputFileType.Scans:
                            datasetInformation.Scans = file;
                            break;
                        case InputFileType.Raw:
                            datasetInformation.Raw = file;
                            break;
                        case InputFileType.Sequence:
                            datasetInformation.Sequence = file;
                            break;
                    }
                }

                /// Add the dataset
                if (!doesDatasetExist)
                {                    
                    addedSets.Add(datasetInformation);
                    Datasets.Add(datasetInformation);
                }
            }

            // Reformat their Id's
            int id = 0;
            Datasets.ForEach(x => x.DatasetId = id++);          
            return addedSets;
        }
    }
}
