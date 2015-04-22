#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Factors;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Data;

#endregion

namespace MultiAlignCore.Data.MetaData
{
    /// <summary>
    ///     Contains information about a dataset used for analysis.r
    /// </summary>
    public class DatasetInformation : IComparable<DatasetInformation>
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public DatasetInformation()
        {
            MetaData = new Dictionary<string, string>();
            FactorInformation = new Dictionary<FactorInformation, string>();
            Factors = new List<Factor>();
            Scans = null;
            Raw = null;
            Sequence = null;
            Features = null;
            IsBaseline = false;
            DatasetSummary = new DatasetSummary();
            PlotData = new DatasetPlotInformation();
            ScanTimes = new Dictionary<int, double>();
        }

        /// <summary>
        ///     Gets or sets the dataset summary.
        /// </summary>
        public DatasetSummary DatasetSummary { get; set; }

        #region Properties

        public string Alias { get; set; }

        /// <summary>
        ///     Gets or sets whether this dataset is a baseline or not.
        /// </summary>
        public bool IsBaseline { get; set; }
        
        /// <summary>
        ///     Gets or sets a mapping of scans to retention times 
        /// </summary>
        public Dictionary<int, double> ScanTimes { get; set; }

        /// <summary>
        ///     Gets or sets
        /// </summary>
        public Dictionary<string, string> MetaData { get; set; }

        /// <summary>
        ///     Gets or sets
        /// </summary>
        public Dictionary<FactorInformation, string> FactorInformation { get; set; }

        /// <summary>
        ///     Gets or sets
        /// </summary>
        public List<Factor> Factors { get; set; }

        /// <summary>
        ///     Gets or sets the key used for access to the db.
        /// </summary>
        public int DMSDatasetID { get; set; }

        /// <summary>
        ///     Gets or sets the ID of the dataset
        /// </summary>
        public int DatasetId { get; set; }

        /// <summary>
        ///     Job tracking ID of the dataset.
        /// </summary>
        public int JobID { get; set; }

        /// <summary>
        ///     Gets or sets the path to the features file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Gets or sets the path to the raw file.
        /// </summary>
        public string RawPath
        {
            get
            {
                var path = "";
                if (Raw != null)
                {
                    path = Raw.Path;
                }
                return path;
            }
            set
            {
                if (Raw == null)
                {
                    Raw = new InputFile();
                    Raw.FileType = InputFileType.Raw;
                }
                Raw.Path = value;
            }
        }

        /// <summary>
        ///     Gets or sets the path to the sequence path.
        /// </summary>
        public string SequencePath
        {
            get
            {
                var path = "";
                if (Sequence != null)
                {
                    path = Sequence.Path;
                }
                return path;
            }
            set
            {
                if (Sequence == null)
                {
                    Sequence = new InputFile();
                    Sequence.FileType = InputFileType.Sequence;
                }
                Sequence.Path = value;
            }
        }

        /// <summary>
        ///     Gets or sets the parameter file
        /// </summary>
        public string ParameterFile { get; set; }

        private string m_datasetName;

        /// <summary>
        ///     Gets or sets the name of the dataset
        /// </summary>
        public string DatasetName
        {
            get { return m_datasetName; }
            set
            {
                m_datasetName = value;
                Alias = value;
            }
        }

        /// <summary>
        ///     Gets or sets the archive path.
        /// </summary>
        public InputFile Features { get; set; }

        /// <summary>
        ///     Path to the scans file.
        /// </summary>
        public InputFile Scans { get; set; }

        /// <summary>
        ///     Path to the Raw data file.
        /// </summary>
        public InputFile Raw { get; set; }

        /// <summary>
        ///     Path to the Raw data file.
        /// </summary>
        public InputFile Sequence { get; set; }

        public DatasetPlotInformation PlotData { get; set; }

        #endregion

        #region Comparison Methods 

        public override bool Equals(object obj)
        {
            var dataset = obj as DatasetInformation;

            return dataset != null && DatasetId.Equals(dataset.DatasetId);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash*23 + DatasetId.GetHashCode();

            return hash;
        }

        public int CompareTo(DatasetInformation other)
        {
            return DatasetId.CompareTo(other.DatasetId);
        }

        #endregion

        /// <summary>
        ///     Cleans dataset names of extensions in case the data as not loaded from DMS, but manually.
        /// </summary>
        /// <returns></returns>
        public static string ExtractDatasetName(string path)
        {
            var datasetName = path;

            var supportedTypes = SupportedFileTypes;

            var newPath = path.ToLower();
            foreach (var extension in supportedTypes)
            {
                var ext = extension.Extension.ToLower();
                if (newPath.EndsWith(ext))
                {
                    datasetName = datasetName.Substring(0, newPath.Length - ext.Length);
                    break;
                }
            }
            return System.IO.Path.GetFileNameWithoutExtension(datasetName);
        }

        public static List<DatasetInformation> CreateDatasetsFromInputFile(List<InputFile> inputFiles)
        {
            var datasets = new List<DatasetInformation>();

            var datasetMap = new Dictionary<string, List<InputFile>>();

            foreach (var file in inputFiles)
            {
                var name = System.IO.Path.GetFileName(file.Path);
                var datasetName = ExtractDatasetName(name);
                var isEntryMade = datasetMap.ContainsKey(datasetName);
                if (!isEntryMade)
                {
                    datasetMap.Add(datasetName, new List<InputFile>());
                }
                datasetMap[datasetName].Add(file);
            }

            var i = 0;
            foreach (var datasetName in datasetMap.Keys)
            {
                var files = datasetMap[datasetName];
                var datasetInformation = new DatasetInformation();
                datasetInformation.DatasetId = i++;
                datasetInformation.DatasetName = datasetName;

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
                datasets.Add(datasetInformation);
            }
            return datasets;
        }


        private static readonly List<SupportedDatasetType> SupportedTypes = new List<SupportedDatasetType>();

        /// <summary>
        ///     Retrieves the supported file types by multialign.
        /// </summary>
        /// <returns></returns>
        public static List<SupportedDatasetType> SupportedFileTypes
        {
            get
            {
                if (SupportedTypes.Count < 1)
                {
                    SupportedTypes.Add(new SupportedDatasetType("Decon Tools Isos", "_isos.csv", InputFileType.Features));
                    SupportedTypes.Add(new SupportedDatasetType("LCMS Feature Finder", "_LCMSFeatures.txt",
                        InputFileType.Features));
                    SupportedTypes.Add(new SupportedDatasetType("Sequest First Hit", ".fht", InputFileType.Sequence));
                    SupportedTypes.Add(new SupportedDatasetType("Thermo Raw", ".raw", InputFileType.Raw));
                    SupportedTypes.Add(new SupportedDatasetType("mzXML", ".mzxml", InputFileType.Raw));
                    SupportedTypes.Add(new SupportedDatasetType("MSGF+ First Hit", "_msgfdb_fht.txt",
                        InputFileType.Sequence));
                    SupportedTypes.Add(new SupportedDatasetType("MSGF+ First Hit", "_msgfdb_fht_MSGF.txt",
                        InputFileType.Sequence));
                    SupportedTypes.Add(new SupportedDatasetType("MSGF+ First Hit", "_fht_msgf.txt",
                        InputFileType.Sequence));
                    SupportedTypes.Add(new SupportedDatasetType("MSGF+ Tab Delimited", "_msgf.tsv",
                        InputFileType.Sequence));
                }
                return SupportedTypes;
            }
        }

        /// <summary>
        ///     Determiens the file type based on the supported file types within MultiAlign.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static InputFileType GetInputFileType(string path)
        {
            var t = InputFileType.NotRecognized;

            var newPath = path.ToLower();
            foreach (var type in SupportedTypes)
            {
                var lower = type.Extension.ToLower();
                if (newPath.EndsWith(lower))
                {
                    t = type.InputType;
                    break;
                }
            }
            return t;
        }

        /// <summary>
        ///     Adds a new dataset to the list.
        /// </summary>
        /// <returns>A list of added datasets</returns>
        public static List<DatasetInformation> ConvertInputFilesIntoDatasets(List<InputFile> inputFiles)
        {
            var addedSets = new List<DatasetInformation>();
            var datasetMap = new Dictionary<string, DatasetInformation>();
            var inputMap = new Dictionary<string, List<InputFile>>();

            foreach (var file in inputFiles)
            {
                var name = System.IO.Path.GetFileName(file.Path);
                var datasetName = ExtractDatasetName(name);
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
                var datasetInformation = new DatasetInformation {DatasetId = i++, DatasetName = datasetName};

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

                // Add the dataset
                if (!doesDatasetExist)
                {
                    addedSets.Add(datasetInformation);
                }
            }

            // Reformat their Id's
            var id = 0;
            foreach (var x in addedSets)
            {
                x.DatasetId = id++;
            }
            return addedSets;
        }
    }
}