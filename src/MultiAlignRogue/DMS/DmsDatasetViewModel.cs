// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DmsDatasetViewModel.cs" company="Pacific Northwest National Laboratory">
//   2015 Pacific Northwest National Laboratory
// </copyright>
// <author>Christopher Wilkins</author>
// <summary>
//   This class is a view model for editing a DMS data set.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiAlignRogue.DMS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using GalaSoft.MvvmLight;

    /// <summary>
    /// This class is a view model for editing a DMS data set.
    /// </summary>
    public class DmsDatasetViewModel : ViewModelBase
    {
        /// <summary>
        /// The list of available files.
        /// </summary>
        private readonly List<string> availableFiles; 

        /// <summary>
        /// The id of the DMS data set.
        /// </summary>
        private int datasetId;

        /// <summary>
        /// The name of the DMS data set.
        /// </summary>
        private string dataset;

        /// <summary>
        /// The name of the type of experiment.
        /// </summary>
        private string experiment;

        /// <summary>
        /// The name of the organism.
        /// </summary>
        private string organism;

        /// <summary>
        /// The type of instrument used.
        /// </summary>
        private string instrument;

        /// <summary>
        /// The date and time that this data set was created.
        /// </summary>
        private DateTime created;

        /// <summary>
        /// The path for the folder that this data set is in.
        /// </summary>
        private string datasetFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DmsDatasetViewModel"/> class. 
        /// </summary>
        /// <param name="datasetInfo">Existing data set to edit.</param>
        public DmsDatasetViewModel(DmsLookupUtility.UdtDatasetInfo? datasetInfo = null)
        {
            if (datasetInfo != null)
            {
                this.DatasetId = datasetInfo.Value.DatasetId;
                this.Dataset = datasetInfo.Value.Dataset;
                this.Experiment = datasetInfo.Value.Experiment;
                this.Organism = datasetInfo.Value.Organism;
                this.Instrument = datasetInfo.Value.Instrument;
                this.Created = datasetInfo.Value.Created;
                this.DatasetFolderPath = datasetInfo.Value.DatasetFolderPath;
            }

            this.availableFiles = new List<string>();
        }

        /// <summary>
        /// Gets the the DMS DatasetInfo model.
        /// </summary>
        public DmsLookupUtility.UdtDatasetInfo UdtDatasetInfo
        {
            get
            {
                DmsLookupUtility.UdtDatasetInfo udtDatasetInfo;
                udtDatasetInfo.DatasetId = this.DatasetId;
                udtDatasetInfo.Dataset = this.Dataset;
                udtDatasetInfo.Experiment = this.Experiment;
                udtDatasetInfo.Organism = this.Organism;
                udtDatasetInfo.Instrument = this.Instrument;
                udtDatasetInfo.Created = this.Created;
                udtDatasetInfo.DatasetFolderPath = this.DatasetFolderPath;
                return udtDatasetInfo;
            }
        }

        /// <summary>
        /// A value that indicates whether this dataset has been selected.
        /// </summary>
        private bool selected;

        /// <summary>
        /// Gets or sets a value that indicates whether this dataset has been selected.
        /// </summary>
        public bool Selected
        {
            get { return this.selected; }
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;
                    this.RaisePropertyChanged("Selected", !value, value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the id of the DMS data set.
        /// </summary>
        public int DatasetId
        {
            get { return this.datasetId; }
            set
            {
                if (this.datasetId != value)
                {
                    this.datasetId = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the DMS data set.
        /// </summary>
        public string Dataset
        {
            get { return this.dataset; }
            set
            {
                if (this.dataset != value)
                {
                    this.dataset = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the type of experiment.
        /// </summary>
        public string Experiment
        {
            get { return this.experiment; }
            set
            {
                if (this.experiment != value)
                {
                    this.experiment = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the organism.
        /// </summary>
        public string Organism
        {
            get { return this.organism; }
            set
            {
                if (this.organism != value)
                {
                    this.organism = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of instrument used.
        /// </summary>
        public string Instrument
        {
            get { return this.instrument; }
            set
            {
                if (this.instrument != value)
                {
                    this.instrument = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the date and time that this data set was created.
        /// </summary>
        public DateTime Created
        {
            get { return this.created; }
            set
            {
                if (this.created != value)
                {
                    this.created = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the path for the folder that this data set is in.
        /// </summary>
        public string DatasetFolderPath
        {
            get { return this.datasetFolderPath; }
            set
            {
                if (this.datasetFolderPath != value)
                {
                    this.datasetFolderPath = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public List<string> GetAvailableFiles()
        {
            if (this.availableFiles.Count == 0)
            {
                if (!Directory.Exists(this.DatasetFolderPath))
                {
                    return this.availableFiles;
                }

                var directories = Directory.GetDirectories(this.DatasetFolderPath, "DLS*");
                if (directories.Length == 0)
                {
                    return this.availableFiles;
                }

                var folderPath = directories[0];
                var scanFiles = Directory.GetFiles(folderPath, "*_scans.csv");
                var isosFiles = Directory.GetFiles(folderPath, "*_isos.csv");
                var rawFiles = Directory.GetFiles(this.DatasetFolderPath, "*.raw");

                this.availableFiles.AddRange(scanFiles);
                this.availableFiles.AddRange(isosFiles);
                this.availableFiles.AddRange(rawFiles);
            }

            return this.availableFiles;
        }
    }
}
