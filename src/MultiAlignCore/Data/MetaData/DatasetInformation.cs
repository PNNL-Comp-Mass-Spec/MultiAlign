using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MultiAlignCore.Data.Factors;
using MultiAlignEngine;
using System.IO;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.InputFiles;

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
            IsBaseline          = false;

            PlotData = new DatasetPlotInformation();
		}

        #region Properties
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
		/// <summary>
		/// Gets or sets the name of the dataset
		/// </summary>
        public string DatasetName
        {
            get;
            set;
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
            string datasetName  = path.Replace("_isos.csv", "").ToLower();
            datasetName         = datasetName.Replace(".scans", "");
            datasetName         = datasetName.Replace("_fht", "");            
            datasetName         = datasetName.Replace("lcmsfeatures.txt", "");
            datasetName         = System.IO.Path.GetFileNameWithoutExtension(datasetName);
            return datasetName;                
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
	}
}
