#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using MultiAlignCore.Data.Factors;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.InputFiles;

#endregion

namespace MultiAlignCore.Data.MetaData
{
    /// <summary>
    ///     Class that holds meta-data information about the analysis.
    /// </summary>
    public class AnalysisMetaData : INotifyPropertyChanged
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public AnalysisMetaData()
        {
            Datasets = new ObservableCollection<DatasetInformation>(); //new List<DatasetInformation>();
            InputFileDefinition = null;
            ParameterFile = null;
            FactorTreeNode = null;
            Database = new InputDatabase(MassTagDatabaseFormat.None);
        }

        public InputDatabase Database { get; set; }

        /// <summary>
        ///     Gets or sets the list of dataset information.
        /// </summary>
        public ObservableCollection<DatasetInformation> Datasets { get; set; }

        private DatasetInformation m_baseline;

        /// <summary>
        ///     Gets or sets the name of the baseline dataset.
        /// </summary>
        [DataSummary("Baseline Dataset")]
        public DatasetInformation BaselineDataset
        {
            get { return m_baseline; }
            set
            {
                /// Here we say, is the baseline the same?
                /// If not then let's update it.
                if (m_baseline != value)
                {
                    // Since it's not the same, then it could be 
                    // an older dataset, in which case we want to change
                    // the old one to no longer be tagged as the baseline.
                    if (m_baseline != null)
                    {
                        m_baseline.IsBaseline = false;
                    }
                    // Then update
                    m_baseline = value;

                    // Then update the new
                    if (m_baseline != null)
                    {
                        m_baseline.IsBaseline = true;
                    }

                    OnNotify("BaselineDataset");
                }
            }
        }

        /// <summary>
        ///     Notify the listener that our internal data has changed.
        /// </summary>
        /// <param name="name"></param>
        private void OnNotify(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        ///     Gets or sets the input file used.
        /// </summary>
        [DataSummary("Input File Definition Name")]
        public string InputFileDefinition { get; set; }

        /// <summary>
        ///     Gets or sets the parameter file used.
        /// </summary>
        [DataSummary("Parameter File")]
        public string ParameterFile { get; set; }

        /// <summary>
        ///     Gets or sets the job id for this analysis.
        /// </summary>
        [DataSummary("Job ID")]
        public int JobID { get; set; }

        /// <summary>
        ///     Get/Set the analysis as a hiearchy of datasets with related factor information for grouping.
        /// </summary>
        public TreeNode FactorTreeNode { get; set; }

        /// <summary>
        ///     Gets or sets the name of the analysis.
        /// </summary>
        [DataSummary("Analysis Name")]
        public string AnalysisName { get; set; }

        /// <summary>
        ///     Gets or sets the pathname associated with the analysis.
        /// </summary>
        [DataSummary("Analysis Path")]
        public string AnalysisPath { get; set; }

        /// <summary>
        ///     Gets or sets the initial analysis setup information.
        /// </summary>
        public InputAnalysisInfo AnalysisSetupInfo { get; set; }

        /// <summary>
        ///     Adds a new dataset to the list.
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <returns>A list of added datasets</returns>
        public List<DatasetInformation> AddInputFiles(List<InputFile> inputFiles)
        {
            var addedSets = new List<DatasetInformation>();
            var datasetMap = new Dictionary<string, DatasetInformation>();

            foreach (var x in Datasets)
            {
                datasetMap.Add(x.DatasetName, x);
            }

            var inputMap = new Dictionary<string, List<InputFile>>();

            foreach (var file in inputFiles)
            {
                var name = Path.GetFileName(file.Path);
                var datasetName = DatasetInformation.ExtractDatasetName(name);
                var isEntryMade = inputMap.ContainsKey(datasetName);
                if (!isEntryMade)
                {
                    inputMap.Add(datasetName, new List<InputFile>());
                }

                inputMap[datasetName].Add(file);
            }

            var i = 0;
            foreach (var datasetName in inputMap.Keys)
            {
                var files = inputMap[datasetName];
                var datasetInformation = new DatasetInformation();
                datasetInformation.DatasetId = i++;
                datasetInformation.DatasetName = datasetName;

                var doesDatasetExist = datasetMap.ContainsKey(datasetName);

                // Here we map the old dataset if it existed already.
                if (datasetMap.ContainsKey(datasetName))
                {
                    datasetInformation = datasetMap[datasetName];
                }

                foreach (var file in files)
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
            var id = 0;

            foreach (var x in Datasets)
            {
                x.DatasetId = id++;
            }

            return addedSets;
        }

        /// <summary>
        ///     Finds the dataset information for the dataset ID provided.
        /// </summary>
        /// <param name="datasetId"></param>
        /// <returns></returns>
        public DatasetInformation FindDatasetInformation(int datasetId)
        {
            DatasetInformation info = null;
            foreach (var datasetInfo in Datasets)
            {
                if (datasetInfo.DatasetId == datasetId)
                {
                    info = datasetInfo;
                    break;
                }
            }
            return info;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}