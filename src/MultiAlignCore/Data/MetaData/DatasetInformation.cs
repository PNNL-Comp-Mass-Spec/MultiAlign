using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MultiAlignCore.Data.Factors;
using MultiAlignEngine;
using System.IO;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.InputFiles;
using System.ComponentModel;
using PNNLOmics.Data;
using System.Text.RegularExpressions;

namespace MultiAlignCore.Data
{
	/// <summary>
	/// Contains information about a dataset used for analysis.r
	/// </summary>	
	public class DatasetInformation : IComparable<DatasetInformation>
	{

        /// <summary>
		/// Default constructor.
		/// </summary>
		public DatasetInformation()
		{
			MetaData            = new Dictionary<string,string>();

			FactorInformation   = new Dictionary<FactorInformation, string>();
			Factors             = new List<Factor>();

            Scans               = null;
            Raw                 = null;
            Sequence            = null;
            Features            = null;
            Peaks               = null;
            IsBaseline          = false;

            DatasetSummary = new PNNLOmics.Data.DatasetSummary();
            PlotData = new DatasetPlotInformation();
		}
        /// <summary>
        /// Gets or sets the dataset summary.
        /// </summary>
        public DatasetSummary DatasetSummary
        {
            get;
            set;
        }

        #region Properties
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets whether this dataset is a baseline or not.
        /// </summary>
        public bool IsBaseline
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets 
        /// </summary>
        public Dictionary<string, string> MetaData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets 
        /// </summary>
        public Dictionary<FactorInformation, string> FactorInformation
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets 
        /// </summary>
        public List<Factor> Factors
        {
            get;
            set;
        }
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
        public int DatasetId
        {
            get;
            set;
        }
        /// <summary>
        /// Job tracking ID of the dataset.
        /// </summary>
        public int JobID
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path to the features file.
        /// </summary>
        public string Path
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path to the raw file.
        /// </summary>
        public string RawPath
        {
            get
            {
                string path = "";
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
                    Raw          = new InputFile();
                    Raw.FileType = InputFileType.Raw;
                }
                Raw.Path = value;      
            }
        }
        /// <summary>
        /// Gets or sets the path to the sequence path.
        /// </summary>
        public string SequencePath
        {
            get
            {
                string path = "";
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
        /// Gets or sets the parameter file 
        /// </summary>
        public string ParameterFile
        {
            get;
            set;
        }
        private string m_datasetName;
		/// <summary>
		/// Gets or sets the name of the dataset
		/// </summary>
        public string DatasetName
        {
            get
            {
                return m_datasetName;
            }
            set
            {
                m_datasetName = value;
                Alias = value;
            }
        }

		/// <summary>
		/// Gets or sets the archive path.
		/// </summary>
        public InputFile Features
        {
            get;
            set;
        }
        /// <summary>
        /// Path to the scans file.
        /// </summary>
        public InputFile Scans
        {
            get;
            set;
        }
        /// <summary>
        /// Path to the Raw data file.
        /// </summary>
        public InputFile Raw
        {
            get;
            set;
        }
        /// <summary>
        /// Path to the Raw data file.
        /// </summary>
        public InputFile Sequence
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path to the peaks file.
        /// </summary>
        public InputFile Peaks
        {
            get;
            set;
        }
        public DatasetPlotInformation PlotData
        {
            get;
            set;
        }
		#endregion
        
        #region Comparison Methods 
        public override bool Equals(object obj)
        {
            DatasetInformation dataset = obj as DatasetInformation;

            if (dataset == null)
            {
                return false;
            }
            else if (!this.DatasetId.Equals(dataset.DatasetId))
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 23 + DatasetId.GetHashCode();

            return hash;
        }
		public int CompareTo(DatasetInformation other)
		{
            return DatasetId.CompareTo(other.DatasetId);
		}
		#endregion

       
        /// <summary>
        /// Cleans dataset names of extensions in case the data as not loaded from DMS, but manually.
        /// </summary>
        /// <returns></returns>
        public static string ExtractDatasetName(string path)
        {
            string  datasetName = path;

            List<SupportedDatasetType> supportedTypes = DatasetInformation.SupportedFileTypes;

            string newPath = path.ToLower();
            foreach (SupportedDatasetType extension in supportedTypes)
            {
                string ext  = extension.Extension.ToLower();
                if (newPath.EndsWith(ext))
                {
                    datasetName = datasetName.Substring(0, datasetName.Length - ext.Length);                    
                }
            }
            return System.IO.Path.GetFileNameWithoutExtension(datasetName);            
        }
        public static List<DatasetInformation> CreateDatasetsFromInputFile(List<InputFile> inputFiles)
        {
            List<DatasetInformation> datasets = new List<DatasetInformation>();

            Dictionary<string, List<InputFile>> datasetMap = new Dictionary<string, List<InputFile>>();

            foreach (InputFile file in inputFiles)
            {
                string name         = System.IO.Path.GetFileName(file.Path);
                string datasetName  = DatasetInformation.ExtractDatasetName(name);
                bool isEntryMade    = datasetMap.ContainsKey(datasetName);
                if (!isEntryMade)
                {
                    datasetMap.Add(datasetName, new List<InputFile>());
                }
                datasetMap[datasetName].Add(file);
            }

            int i = 0;
            foreach (string datasetName in datasetMap.Keys)
            {
                List<InputFile> files = datasetMap[datasetName];
                DatasetInformation datasetInformation = new DatasetInformation();
                datasetInformation.DatasetId = i++;
                datasetInformation.DatasetName = datasetName;

                foreach (InputFile file in files)
                {
                    switch (file.FileType)
                    {
                        case InputFileType.Features:
                            datasetInformation.Features = file;
                            datasetInformation.Path     = file.Path;
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private static readonly List<SupportedDatasetType> m_supportedTypes = new List<SupportedDatasetType>();
        
        /// <summary>
        /// Retrieves the supported file types by multialign.
        /// </summary>
        /// <returns></returns>
        public static List<SupportedDatasetType> SupportedFileTypes
        {            
            get
            {
                if (m_supportedTypes.Count < 1)
                {                            
                    m_supportedTypes.Add(new SupportedDatasetType("Decon Tools Isos", "_isos.csv",          InputFileType.Features));
                    m_supportedTypes.Add(new SupportedDatasetType("LCMS Feature Finder", "_LCMSFeatures.txt", InputFileType.Features));
                    m_supportedTypes.Add(new SupportedDatasetType("Sequest First Hit", ".fht", InputFileType.Sequence));
                    m_supportedTypes.Add(new SupportedDatasetType("Thermo Raw", ".raw", InputFileType.Raw));
                    m_supportedTypes.Add(new SupportedDatasetType("mzXML", ".mzxml", InputFileType.Raw));
                    m_supportedTypes.Add(new SupportedDatasetType("MSGF+ First Hit", "_msgfdb_fht.txt", InputFileType.Sequence));
                    m_supportedTypes.Add(new SupportedDatasetType("MSGF+ First Hit", "_fht_msgf.txt", InputFileType.Sequence));
                    m_supportedTypes.Add(new SupportedDatasetType("DeconTools XIC Peak File", "_peaks.txt", InputFileType.Peaks));                                                                                                                                                                                                    
                }                  
                return m_supportedTypes;
            }            
        }
        /// <summary>
        /// Determiens the file type based on the supported file types within MultiAlign.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static InputFileType GetInputFileType(string path)
        {
            InputFileType t = InputFileType.NotRecognized;

            string newPath = path.ToLower();
            foreach (SupportedDatasetType type in m_supportedTypes)
            {
                string lower = type.Extension.ToLower();
                if (newPath.EndsWith(lower))
                {
                    t = type.InputType;
                    break;
                }
            }
            return t;
        }
    }

    public class SupportedDatasetType
    {
        public SupportedDatasetType(string name,
                                    string extension,                                
                                    InputFileType type)
        {
            Name        = name;
            InputType   = type;
            Extension   = extension;
        }

        /// <summary>
        /// Gets or sets the extension of the dataset type.
        /// </summary>
        public string Extension { get; private set; }

        public string Name
        {
            get;
            private set;
        }
        public InputFileType InputType 
        { 
            get;
            private set; 
        }
    }
}
