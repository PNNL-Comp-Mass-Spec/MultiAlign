namespace MultiAlignCore.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    using InformedProteomics.Backend.Data;
    using InformedProteomics.Backend.MassSpecData;

    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.InputFiles;

    public class DatasetLoader
    {
        /// <summary>
        /// The supported file types by MultiAlign.
        /// </summary>
        private static readonly List<SupportedFileType> supportedTypes = new List<SupportedFileType>();

        public enum SupportedDatasetTypes
        {
            DeconTools,
            LcImsFeatureFinder,
            Promex
        }

        /// <summary>
        /// The list of files required for each feature file type.
        /// </summary>
        private static readonly List<SupportedFileCombination> supportedFileCombinations = new List<SupportedFileCombination>();

        /// <summary>
        /// The list of datasets that can be used together.
        /// </summary>
        private static readonly Dictionary<SupportedDatasetTypes, List<SupportedDatasetTypes>> supportedDatasetCombinations;

        public DatasetLoader()
        {

        }

        static DatasetLoader()
        {
            supportedDatasetCombinations = new Dictionary<SupportedDatasetTypes, List<SupportedDatasetTypes>>();

            supportedDatasetCombinations.Add(
                SupportedDatasetTypes.DeconTools,
                new List<SupportedDatasetTypes>
            {
                SupportedDatasetTypes.DeconTools,
                SupportedDatasetTypes.Promex,
            });

            supportedDatasetCombinations.Add(
                SupportedDatasetTypes.Promex,
                new List<SupportedDatasetTypes>
            {
                SupportedDatasetTypes.DeconTools,
                SupportedDatasetTypes.Promex,
            });

            supportedDatasetCombinations.Add(SupportedDatasetTypes.LcImsFeatureFinder, new List<SupportedDatasetTypes>());
        }

        /// <summary>
        /// Gets the list of files required for each feature file type.
        /// </summary>
        public static List<SupportedFileCombination> SupportedFileCombinations
        {
            get
            {
                if (supportedFileCombinations.Count < 1)
                {
                    // Decon tools:     FeatureFile && (Scans || Raw)
                    var deconCombo = new SupportedFileCombination(SupportedFileTypes.FirstOrDefault(sft => sft.Extension == "_isos.csv"))
                    {
                        DatasetType = SupportedDatasetTypes.DeconTools
                    };
                    deconCombo.AtLeastOneOf.Add(InputFileType.Scans);
                    deconCombo.AtLeastOneOf.Add(InputFileType.Raw);
                    supportedFileCombinations.Add(deconCombo);

                    // Promex:          FeatureFile && Raw
                    var promexCombo = new SupportedFileCombination(SupportedFileTypes.FirstOrDefault(sft => sft.Extension == ".ms1ft"))
                    {
                        DatasetType = SupportedDatasetTypes.Promex
                    };
                    promexCombo.RequiredTypes.Add(InputFileType.Raw);
                    supportedFileCombinations.Add(promexCombo);

                    // LC-IMS Feature Finder:       FeatureFile && Scans
                    var lcImsCombo = new SupportedFileCombination(SupportedFileTypes.FirstOrDefault(sft => sft.Extension == "_LCMSFeatures.txt"))
                    {
                        DatasetType = SupportedDatasetTypes.LcImsFeatureFinder
                    };
                    lcImsCombo.RequiredTypes.Add(InputFileType.Scans);
                    lcImsCombo.OptionalTypes.Add(InputFileType.Raw);
                    supportedFileCombinations.Add(lcImsCombo);
                }

                return supportedFileCombinations;
            }
        }

        /// <summary>
        /// Gets the supported file types by MultiAlign.
        /// </summary>
        public static List<SupportedFileType> SupportedFileTypes
        {
            get
            {
                if (supportedTypes.Count < 1)
                {
                    supportedTypes.Add(new SupportedFileType("Decon Tools Isos", "_isos.csv", InputFileType.Features));
                    supportedTypes.Add(new SupportedFileType("Decon Tools scans", "_scans.csv", InputFileType.Scans));
                    supportedTypes.Add(new SupportedFileType("Promex Features", ".ms1ft", InputFileType.Features));
                    supportedTypes.Add(new SupportedFileType("LCMS Feature Finder", "_LCMSFeatures.txt", InputFileType.Features));
                    supportedTypes.Add(new SupportedFileType("Sequest First Hit", ".fht", InputFileType.Sequence));
                    supportedTypes.Add(new SupportedFileType("MSGF+ First Hit", "_msgfdb_fht.txt", InputFileType.Sequence));
                    supportedTypes.Add(new SupportedFileType("MSGF+ First Hit", "_msgfdb_fht_MSGF.txt", InputFileType.Sequence));
                    supportedTypes.Add(new SupportedFileType("MSGF+ First Hit", "_fht_msgf.txt", InputFileType.Sequence));
                    supportedTypes.Add(new SupportedFileType("MSGF+ Tab Delimited", "_msgf.tsv", InputFileType.Sequence));
                    supportedTypes.Add(new SupportedFileType("MSPathFinder Results", "_IcTda.tsv", InputFileType.Sequence));

                    // Add supported RAW file types.
                    foreach (var fileTypeInfo in MassSpecDataReaderFactory.MassSpecDataTypes)
                    {
                        foreach (var extension in fileTypeInfo.Item2)
                        {
                            supportedTypes.Add(new SupportedFileType(fileTypeInfo.Item1, extension, InputFileType.Raw));
                        }
                    }
                }
                return supportedTypes;
            }
        }

        public static string SupportedFileFilter
        {
            get
            {
                var featureFileFormatString = "Feature Files (*.csv)(*.ms1ft)|*.csv;*.ms1ft";
                var decon2lsFormatString = "Decon2ls Files (*.csv)|*.csv";
                var promexFileFormatString = "Promex Files (*.ms1ft)|*.ms1ft;";
                var sequenceFileFormatString = "Sequence Files (*.csv)(*.txt)|*.csv;*.txt";

                var allSupportedList = "All Supported|*.txt;*.csv;*.ms1ft;";
                foreach (var fileType in MassSpecDataReaderFactory.MassSpecDataTypes)
                {
                    foreach (var extension in fileType.Item2)
                    {
                        allSupportedList += (extension.StartsWith(".") ? "*" : String.Empty) + extension + ";";
                    }
                }

                var rawFormatString = MassSpecDataReaderFactory.MassSpecDataTypeFilterString;

                return String.Format("{0}|{1}|{2}|{3}|{4}|{5}", featureFileFormatString, allSupportedList, sequenceFileFormatString, decon2lsFormatString, promexFileFormatString, rawFormatString);
            }
        }

        /// <summary>
        /// Gets the error message for dataset validation failure.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Cleans dataset names of extensions in case the data as not loaded from DMS, but manually.
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

        /// <summary>
        /// Gets valid datasets from a list of file paths.
        /// </summary>
        /// <param name="paths">The list of file paths.</param>
        /// <param name="findAdditionalFiles">A value indicating whether additional files should be found for each dataset.</param>
        /// <returns>The list of valid datasets.</returns>
        public List<DatasetInformation> GetValidDatasets(IEnumerable<string> paths, bool findAdditionalFiles = true)
        {
            return this.GetValidDatasets(this.GetInputFilesFromPath(paths));
        }

        /// <summary>
        /// Gets valid datasets from a provided folder.
        /// </summary>
        /// <param name="folderPath">The folder to get the input files for.</param>
        /// <param name="extensions">The extensions to find files for.</param>
        /// <param name="findAdditionalFiles">A value indicating whether additional files should be found for each dataset.</param>
        /// <returns>The list of valid datasets.</returns>
        public List<DatasetInformation> GetValidDatasets(string folderPath, List<string> extensions, SearchOption options, bool findAdditionalFiles = true)
        {
            var datasetList = new List<InputFile>();
            var candidates = new List<string>();

            foreach (var extension in extensions)
            {
                var paths = Directory.GetFiles(folderPath, "*" + extension, options);
                candidates.AddRange(paths);
            }


            foreach (var path in candidates)
            {
                var type = this.GetInputFileType(path);

                if (type == InputFileType.NotRecognized)
                {
                    continue;
                }

                var file = new InputFile {
                    Path = path,
                    FileType = type
                };

                datasetList.Add(file);
            }

            return this.GetValidDatasets(datasetList);
        }

        /// <summary>
        /// Gets valid datasets from a list of input files.
        /// </summary>
        /// <param name="files">The list of input files.</param>
        /// <param name="findAdditionalFiles">A value indicating whether additional files should be found for each dataset.</param>
        /// <returns>The list of valid datasets.</returns>
        public List<DatasetInformation> GetValidDatasets(List<InputFile> files, bool findAdditionalFiles = true)
        {
            var datasets = this.CreateDatasetsFromInputFile(files, findAdditionalFiles);
            var validDatasets = new List<DatasetInformation>();

            // make sure we only show each message once.
            var combinationTypes = new HashSet<SupportedFileCombination>();
            var noFeatureFileFound = false;

            foreach (var dataset in datasets)
            {
                var combo = this.GetDatasetCombination(dataset.InputFiles);
                noFeatureFileFound |= combo == null;
                if (combo != null && combo.IsValid(dataset.InputFiles))
                {   // Valid dataset.
                    validDatasets.Add(dataset);
                    dataset.DatasetType = combo.DatasetType;
                }
                else if (combo != null && !combinationTypes.Contains(combo))
                {   // Invalid dataset, add this combination type to show the message for it later.
                    combinationTypes.Add(combo);
                }
            }

            // datasets are valid if feature files were found for all datasets, and all datasets were valid.
            if (noFeatureFileFound || combinationTypes.Any())
            {
                this.ErrorMessage = this.GetInvalidDatasetMessage(noFeatureFileFound, combinationTypes);
            }

            return validDatasets;
        }

        /// <summary>
        /// Checks to see if the types are compatible with the dataset.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool IsValidDatasetCombo(SupportedDatasetTypes dataset, IEnumerable<SupportedDatasetTypes> types)
        {
            return types.Aggregate(true, (current, type) => current & IsValidDatasetCombo(dataset, type));
        }

        /// <summary>
        /// Checks to see if dataset2 is compatible with dataset1.
        /// </summary>
        /// <param name="dataset1"></param>
        /// <param name="dataset2"></param>
        /// <returns></returns>
        private static bool IsValidDatasetCombo(SupportedDatasetTypes dataset1, SupportedDatasetTypes dataset2)
        {
            if (supportedDatasetCombinations.TryGetValue(dataset1, out var datasets))
            {
                return datasets.Contains(dataset2);
            }

            return false;
        }

        private List<InputFile> FindAdditionalDatasetFiles(InputFile inputFile)
        {
            var directory = Path.GetDirectoryName(inputFile.Path);
            var fileName = inputFile.Path.TrimEnd(inputFile.Extension.ToCharArray());

            var additionalFiles = new List<InputFile>();

            if (String.IsNullOrEmpty(directory) || String.IsNullOrEmpty(fileName))
            {   // Invalid path
                return additionalFiles;
            }

            var filePaths = Directory.GetFiles(directory);
            foreach (var filePath in filePaths)
            {
                var supportedFileType = this.GetSupportedDatasetType(filePath);
                if (supportedFileType != null &&
                    supportedFileType.InputType != InputFileType.NotRecognized &&
                    (inputFile.FileType != InputFileType.Features || supportedFileType.InputType != InputFileType.Features) &&
                    filePath.Contains(fileName) &&
                    filePath != inputFile.Path)
                {   // File is part of the chosen dataset.
                    additionalFiles.Add(new InputFile
                    {
                        Path = filePath,
                        FileType = supportedFileType.InputType,
                        Extension = supportedFileType.Extension
                    }) ;
                }
            }

            return additionalFiles;
        }

        private string GetInvalidDatasetMessage(bool noFeatureFileFound, IEnumerable<SupportedFileCombination> datasetCombinations)
        {
            // Build error message. To avoid spamming the user, we'll show all error messages in one message box.
            var errorMessageBuilder = new StringBuilder();

            // Error message for missing feature file.
            if (noFeatureFileFound)
            {
                errorMessageBuilder.AppendLine("One or more of your datasets did not include feature files. All datasets must include a feature file.");
            }

            errorMessageBuilder.AppendLine();
            foreach (var combinationType in datasetCombinations)
            {
                errorMessageBuilder.AppendLine(combinationType.RequiredMessage());
            }

            return errorMessageBuilder.ToString();
        }

        /// <summary>
        /// Given a list of files, get the dataset combination that is required for the dataset type.
        /// </summary>
        /// <param name="files">List of files to get dataset combination for.</param>
        /// <returns>The dataset combination.</returns>
        private SupportedFileCombination GetDatasetCombination(List<InputFile> files)
        {
            var supportedComboMap = SupportedFileCombinations.ToDictionary(c => c.BaseType.Extension);
            var featureFile = files.FirstOrDefault(file => file.FileType == InputFileType.Features);

            if (featureFile == null || !supportedComboMap.ContainsKey(featureFile.Extension))
            {   // There were no feature files selected, or the feature file did not have a recognized extension.
                return null;
            }

            return supportedComboMap[featureFile.Extension];
        }

        private List<InputFile> GetInputFilesFromPath(IEnumerable<string> filePaths)
        {
            var files = new List<InputFile>();
            foreach (var filePath in filePaths)
            {
                var supportedType = this.GetSupportedDatasetType(filePath);
                if (supportedType != null)
                {
                    files.Add(new InputFile { Path = filePath, FileType = supportedType.InputType, Extension = supportedType.Extension });
                }
            }

            return files;
        }

        /// <summary>
        /// Determines the file type based on the supported file types within MultiAlign.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private InputFileType GetInputFileType(string path)
        {
            var t = InputFileType.NotRecognized;
            var supportedType = this.GetSupportedDatasetType(path);
            if (supportedType != null)
            {
                t = supportedType.InputType;
            }

            return t;
        }

        /// <summary>
        /// Determines the file type based on the supported file types within MultiAlign.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private SupportedFileType GetSupportedDatasetType(string path)
        {
            var newPath = path.ToLower();
            foreach (var type in SupportedFileTypes)
            {
                var lower = type.Extension.ToLower();
                if (newPath.EndsWith(lower))
                {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a new dataset to the list.
        /// </summary>
        /// <returns>A list of added datasets</returns>
        private List<DatasetInformation> ConvertInputFilesIntoDatasets(List<InputFile> inputFiles)
        {
            var addedSets = new List<DatasetInformation>();
            var datasetMap = new Dictionary<string, DatasetInformation>();
            var inputMap = new Dictionary<string, List<InputFile>>();

            foreach (var file in inputFiles)
            {
                var name = Path.GetFileName(file.Path);
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
                var datasetInformation = new DatasetInformation { DatasetId = i++, DatasetName = datasetName };

                var doesDatasetExist = datasetMap.ContainsKey(datasetName);

                // Here we map the old dataset if it existed already.
                if (datasetMap.ContainsKey(datasetName))
                {
                    datasetInformation = datasetMap[datasetName];
                }

                datasetInformation.InputFiles.AddRange(files);

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

        private List<DatasetInformation> CreateDatasetsFromInputFile(List<InputFile> inputFiles, bool findAdditionalFiles = false)
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
                var datasetInformation = new DatasetInformation { DatasetId = i++, DatasetName = datasetName };

                // Get additional files
                if (findAdditionalFiles)
                {
                    // Try to use the location of the feature file first, otherwise just use first available file.
                    var featureFile = files.FirstOrDefault(file => file.FileType == InputFileType.Features);
                    var fileToUse = featureFile ?? files.FirstOrDefault();
                    if (fileToUse != null)
                    {
                        files.AddRange(this.FindAdditionalDatasetFiles(fileToUse));
                    }
                }

                datasetInformation.InputFiles.AddRange(files);
                datasets.Add(datasetInformation);
            }
            return datasets;
        }
    }
}
