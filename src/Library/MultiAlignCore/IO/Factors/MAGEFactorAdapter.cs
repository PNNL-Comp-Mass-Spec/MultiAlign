#region

using System.Collections.ObjectModel;
using Mage;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;

#endregion

namespace MultiAlignCore.IO.Factors
{
    /// <summary>
    /// Adapter to the MAGE file library.
    /// </summary>
    public class MAGEFactorAdapter
    {
        /// <summary>
        /// Query string for SQL
        /// </summary>
        private readonly string m_sqlQuery;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MAGEFactorAdapter()
        {
            m_sqlQuery = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE Dataset ";

            Server = "gigasax";
            Database = "DMS5";
        }

        /// <summary>
        /// Gets or sets the server name
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string Database { get; set; }

        public void LoadFactorsFromDMS(ObservableCollection<DatasetInformation> datasets,
            FeatureDataAccessProviders providers)
        {
            if (datasets.Count < 1)
            {
                return;
            }

            var sink = new MultiAlignFactorSink(datasets,
                providers.DatasetCache,
                providers.FactorCache,
                providers.FactorAssignmentCache);
            foreach (var info in datasets)
            {
                var query = m_sqlQuery + string.Format(" = '{0}'", info.DatasetName);
                var reader = new SQLReader { Server = Server, Database = Database, SQLText = query };

                var pipeline = ProcessingPipeline.Assemble("PlainFactors", reader, sink);
                pipeline.RunRoot(null);
            }
            sink.CommitChanges();
        }

        public void LoadFactorsFromFile(string path, ObservableCollection<DatasetInformation> datasets,
            FeatureDataAccessProviders providers)
        {
            if (datasets.Count < 1)
            {
                return;
            }

            var sink = new MultiAlignFactorSink(datasets,
                providers.DatasetCache,
                providers.FactorCache,
                providers.FactorAssignmentCache);

            var reader = new DelimitedFileReader();
            reader.Delimiter = "\t";
            reader.FilePath = path;

            var pipeline = ProcessingPipeline.Assemble("PlainFactors", reader, sink);
            pipeline.RunRoot(null);
            sink.CommitChanges();
        }
    }
}