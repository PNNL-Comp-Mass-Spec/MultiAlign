#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Factors;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Data.MetaData
{
    using System.Linq;

    using InformedProteomics.Backend.MassSpecData;

    /// <summary>
    ///     Contains information about a dataset used for analysis.r
    /// </summary>
    public class DatasetInformation : IComparable<DatasetInformation>, INotifyPropertyChanged, IDataset
    {
        private bool featuresFound;
        private bool isAligned;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public DatasetInformation()
        {
            MetaData = new Dictionary<string, string>();
            FactorInformation = new Dictionary<FactorInformation, string>();
            Factors = new List<Factor>();
            
            Sequence = null;
            InputFiles = new List<InputFile>();
            IsBaseline = false;
            PlotData = new DatasetPlotInformation();
            ScanTimes = new Dictionary<int, double>();
            FeaturesFound = false;
            IsAligned = false;
        }

        #region Properties

        public string Name { get; set; }

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
        ///     Gets or sets the Alignment data
        /// </summary>
        public AlignmentData AlignmentData { get; set; }

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
        /// Gets or sets a value indicating whether work is being performed on this dataset.
        /// </summary>
        public bool DoingWork { get; set; }

        /// <summary>
        /// Gets or sets the list of input files for this dataset.
        /// </summary>
        public List<InputFile> InputFiles { get; set; }

        /// <summary>
        /// Input File interface for use with NHibernate (IList does not have "AddRange")
        /// </summary>
        public IList<InputFile> InputFilesNHibernate
        {
            get { return this.InputFiles; }
            private set
            {
                var list = value as List<InputFile>;
                this.InputFiles = list ?? new List<InputFile>(value);
            }
        }

            /// <summary>
        ///     Gets the raw file info.
        /// </summary>
        public InputFile RawFile
        {
            get
            {
                return this.InputFiles.FirstOrDefault(inputFile => inputFile.FileType == InputFileType.Raw) ??
                       this.InputFiles.FirstOrDefault(inputFile => inputFile.FileType == InputFileType.Scans);
            }
        }

        /// <summary>
        ///     Gets the path to the raw file.
        /// </summary>
        public string RawPath
        {
            get
            {
                var rawFile = this.RawFile;
                if (rawFile != null)
                {
                    return rawFile.Path;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the path to the sequence path.
        /// </summary>
        public InputFile SequenceFile
        {
            get { return this.InputFiles.FirstOrDefault(inputFile => inputFile.FileType == InputFileType.Sequence); }
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
                Name = value;
            }
        }

        /// <summary>
        ///     Gets the archive path.
        /// </summary>
        public InputFile Features
        {
            get { return this.InputFiles.FirstOrDefault(inputFile => inputFile.FileType == InputFileType.Features); }
        }

        /// <summary>
        ///     Gets the path to the raw file.
        /// </summary>
        public string FeaturePath
        {
            get
            {
                var featureFile = this.Features;
                if (featureFile != null)
                {
                    return featureFile.Path;
                }

                return null;
            }
        }

        /// <summary>
        ///     Path to the scans file.
        /// </summary>
        public InputFile Scans
        {
            get { return this.InputFiles.FirstOrDefault(inputFile => inputFile.FileType == InputFileType.Scans); }
        }

        public bool FeaturesFound
        {
            get
            {
                return this.featuresFound;
            }

            set
            {
                featuresFound = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsAligned
        {
            get { return this.isAligned; }
            set
            {
                isAligned = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsClustered { get; set; }


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

        public void BuildScanTimes(ILcMsRun lcms)
        {
            this.ScanTimes = new Dictionary<int, double>();
            var ms1Scans = lcms.GetScanNumbers(1);
            foreach (var scan in ms1Scans)
            {
                this.ScanTimes.Add(scan, lcms.GetElutionTime(scan));
            }
        }

        public void BuildScanTimes(List<ScanSummary> scanSummaries)
        {
            Dictionary<int, double> scanTimeMap = new Dictionary<int, double>();
            foreach (var scan in scanSummaries)
            {
                scanTimeMap.Add(scan.Scan, scan.Time);
            }
            this.ScanTimes = scanTimeMap;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public class MissingRawDataException : Exception
        {
            public MissingRawDataException(string message, int groupId) : base(message)
            {
                this.GroupId = groupId;
            }

            public int GroupId { get; private set; }
        }
    }
}