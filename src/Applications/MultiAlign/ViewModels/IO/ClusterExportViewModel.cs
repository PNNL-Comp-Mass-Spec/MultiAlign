using MultiAlign.Commands;
using MultiAlign.IO;
using MultiAlign.ViewModels.TreeView;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace MultiAlign.ViewModels.IO
{
    public class ClusterExportViewModel: ViewModelBase
    {
        private string m_outputPath;
        private bool m_isClustersFiltered;
        private IFeatureClusterWriter m_exporter;
        private IEnumerable<UMCClusterLightMatched>          m_allClusters;
        private IEnumerable<DatasetInformation> m_datasets;
        private ObservableCollection<UMCClusterTreeViewModel> m_filteredClusters;
        private string m_status;

        /// <summary>
        /// This allows us to dynamically build the filter string for an exporter....
        /// </summary>
        Dictionary<string, IFeatureClusterWriter> m_writerExtensionMap;

        public ClusterExportViewModel(IEnumerable<UMCClusterLightMatched> allClusters,
                                      ObservableCollection<UMCClusterTreeViewModel> filteredClusters)
        {
            m_allClusters       = allClusters;
            m_filteredClusters  = filteredClusters;            
            m_datasets          = SingletonDataProviders.GetAllInformation();
            Exporters           = new ObservableCollection<IFeatureClusterWriter>();
            string filters      = "";

            m_writerExtensionMap = new Dictionary<string, IFeatureClusterWriter>();            
            foreach(var exporter in UMCClusterExporterFactory.Create())
            {
                exporter.ShouldLoadClusterData = true;

                if (filters.Length > 1)
                    filters += "| ";
                filters += string.Format(" {0}  (*{1}) | *{1}", exporter.Name,
                                                            exporter.Extension
                                                            );

                m_writerExtensionMap.Add(exporter.Extension, exporter);

                Exporters.Add(exporter);
            }
            filters += string.Format("|  All Files (*.*) | (*.*)");
            

            BrowseSave = new BrowseSaveFileCommand((string x) =>
                                                   {
                                                        OutputPath = x;
                                                   }, filters);

            /// Action for saving all clusters
            Action saveAction = delegate()
            {
                SelectedExporter.Path           = OutputPath;
                List<UMCClusterLight> clusters  = new List<UMCClusterLight>();
                if (IsFilteredClusters)
                {                    
                    foreach(var x in m_filteredClusters)                 
                        clusters.Add(x.Cluster.Cluster);                 
                }
                else 
                {
                    foreach (var x in m_allClusters)
                        clusters.Add(x.Cluster);
                }

                try
                {
                    SelectedExporter.WriteClusters(clusters, m_datasets.ToList());
                    Status = "The file was saved: " + SelectedExporter.Path;
                }
                catch(Exception ex)
                {
                    Status = "Could not save the file: " + ex.Message;                    
                }
            };


            SaveData = new BaseCommandBridge(saveAction, CanSave);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(OutputPath);
        }

        /// <summary>
        /// Gets or sets the status message
        /// </summary>
        public string Status
        {
            get
            {
                return m_status;
            }
            set
            {
                if (m_status != value)
                {
                    m_status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public ICommand BrowseSave { get; private set; }
        public ICommand SaveData { get; private set; }

        public string OutputPath
        {
            get
            {
                return m_outputPath;
            }
            set
            {
                if (m_outputPath != value)
                {
                    m_outputPath = value;
                    SaveData.CanExecute(value);
                    OnPropertyChanged("OutputPath");

                    if (value != null)
                    {
                        // We have to do it this way because of the stupid numerous extensions
                        // but this can break if we have a _isos.csv and .csv ...fix!
                        foreach(var extension in m_writerExtensionMap.Keys)
                        { 
                            if (value.EndsWith(extension))
                            {
                                SelectedExporter = m_writerExtensionMap[extension];
                            }
                        }
                    }
                }
            }
        }

        public bool IsFilteredClusters
        {

            get
            {
                return m_isClustersFiltered;
            }
            set
            {
                if (m_isClustersFiltered != value)
                {
                    m_isClustersFiltered = value;
                    OnPropertyChanged("IsFilteredClusters");
                }
            }
        }

        public ObservableCollection<IFeatureClusterWriter> Exporters
        {
            get;private set;
        }

        public IFeatureClusterWriter SelectedExporter
        {
            get
            {
                return m_exporter;
            }
            set
            {
                if (m_exporter != value)
                {
                    m_exporter = value;
                    OnPropertyChanged("SelectedExporter");
                }
            }
        }
    }
}
