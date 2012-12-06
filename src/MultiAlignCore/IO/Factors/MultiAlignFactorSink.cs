using System;
using System.Collections.Generic;
using Mage;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Factors;
using MultiAlignCore.IO.Features;
using System.Collections.ObjectModel;

namespace MultiAlignCore.IO.Factors
{
    /// <summary>
    /// You can make your own object that can be included 
    /// as the last module in a Mage pipeline and receive the data stream directly.
    /// </summary>
    internal class MultiAlignFactorSink : ISinkModule
    {
        private int                                                      m_factorCount;
        private List<ExperimentalFactor>                                 m_factors;
        private List<DatasetToExperimentalFactorMap>                     m_factorAssignments;
        private Dictionary<string, DatasetInformation>                   m_datasets;
        private Dictionary<string, Dictionary<string, int>>              m_factorMaps;
        
        private IDatasetDAO             m_datasetProvider;
        private IFactorDAO              m_factorProvider;
        private IDatasetToFactorMapDAO  m_datasetFactorMapProvider;
        /// <summary>
        /// Maps a column 
        /// </summary>
        private Dictionary<string, int> m_columnMapping;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="datasets">Datasets to store data about.</param>
        public MultiAlignFactorSink(ObservableCollection<DatasetInformation> datasets, 
                                    IDatasetDAO datasetProvider, 
                                    IFactorDAO factorProvider,
                                    IDatasetToFactorMapDAO datasetToFactorMapProvider)
        {
            m_factorMaps                = new Dictionary<string, Dictionary<string, int>>();
            m_factors                   = new List<ExperimentalFactor>();
            m_factorAssignments         = new List<DatasetToExperimentalFactorMap>();
            m_datasets                  = new Dictionary<string,DatasetInformation>();
            m_datasetProvider           = datasetProvider;
            m_factorProvider            = factorProvider;
            m_datasetFactorMapProvider  = datasetToFactorMapProvider;
            m_columnMapping             = new Dictionary<string, int>();
            foreach (DatasetInformation info in datasets)
            {
                m_datasets.Add(info.DatasetName, info);
            }
        }
        /// <summary>
        /// Handles the column definitions for a factor module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            m_columnMapping.Clear();

            for(int i = 0; i < args.ColumnDefs.Count; i++)                
            {
                MageColumnDef def = args.ColumnDefs[i];
                m_columnMapping.Add(def.Name.Trim(), i);
            }
            // ignore the column definitions.
        }
        /// <summary>
        /// Gets the datasets used.
        /// </summary>
        public Dictionary<string, DatasetInformation> Datasets
        {
            get
            {
                return m_datasets;
            }
        }

        /// <summary>
        /// Handles converting the rows to factor objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args == null)
            {
                throw new NullReferenceException("The factors are invalid.");
            }

            if (args.Fields == null)
            {
                return;
                throw new NullReferenceException("The factor rows are invalid.");
            }

            if (args.Fields.Length < 4)
            {
                return;
                throw new ArgumentException("The number of columns for the factors are invalid.");
            }

            string datasetName = "";
            if (m_columnMapping.ContainsKey("Dataset"))
            {
                datasetName = Convert.ToString(args.Fields[m_columnMapping["Dataset"]]).ToLower().Replace("\"", "");
            }
            else
            {
                return;
            }
            int datasetId = -1;
            if (m_columnMapping.ContainsKey("Dataset_ID"))
            {
                datasetId = Convert.ToInt32(args.Fields[m_columnMapping["Dataset_ID"]].ToString().Replace("\"",""));
            }
            else
            {
                return;
            }
            string factor = "";
            if (m_columnMapping.ContainsKey("Factor"))
            {
                factor = Convert.ToString(args.Fields[m_columnMapping["Factor"]]).Replace("\"","");
            }
            else
            {
                return;
            }
            string value = "";
            if (m_columnMapping.ContainsKey("Value"))
            {
                value = Convert.ToString(args.Fields[m_columnMapping["Value"]]).Replace("\"", "");
            }
            else
            {
                return;
            }

            ExperimentalFactor factorMap    = new ExperimentalFactor();
            factorMap.Value                 = value;
            factorMap.Name                  = factor;

            DatasetInformation info = null;
            // Update the dataset ID.
            if (m_datasets.ContainsKey(datasetName))
            {
                info                                 = m_datasets[datasetName];
                m_datasets[datasetName].DMSDatasetID = datasetId;
            }
            else
            {
                return;
            }


            // Make sure we haven't seen this factor map before.
            bool shouldAdd = true;
            if (m_factorMaps.ContainsKey(factor))
            {
                if (m_factorMaps[factor].ContainsKey(value))
                {
                    shouldAdd = false;
                }
            }
            else
            {
                m_factorMaps.Add(factor, new Dictionary<string, int>());
            }

            int factorID = 0;
            // Add it to the list and map of factors to dump into the database.
            if (shouldAdd)
            {
                factorMap.FactorID  = m_factorCount++;
                m_factorMaps[factor].Add(value, factorMap.FactorID);
                factorID            = factorMap.FactorID; 
                m_factors.Add(factorMap);
            }
            else
            {
                 factorID = m_factorMaps[factor][value];

            }


            DatasetToExperimentalFactorMap datasetFactorMap = new DatasetToExperimentalFactorMap();
            datasetFactorMap.DatasetID                      = info.DatasetId;
            datasetFactorMap.FactorID                       = factorID;
            m_factorAssignments.Add(datasetFactorMap);
        }

        /// <summary>
        /// Commits the factor data to the repository.
        /// </summary>
        public void CommitChanges()
        {
            // Update factors
            m_factorProvider.AddAll(m_factors);
                        
            // Update datasets
            List<DatasetInformation> datasets = new List<DatasetInformation>();
            foreach (DatasetInformation info in m_datasets.Values)
            {
                datasets.Add(info);
            }
            m_datasetProvider.UpdateAll(datasets);

            // Update factor assignments
            m_datasetFactorMapProvider.AddAll(m_factorAssignments);
        }
    }
}
