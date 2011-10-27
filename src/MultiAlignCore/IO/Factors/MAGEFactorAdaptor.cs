using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Factors;
using MultiAlignCore.IO.Features;

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
        private string m_sqlQuery;        

        /// <summary>
        /// Constructor.
        /// </summary>
        public MAGEFactorAdapter()     
        {
             m_sqlQuery = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE Dataset ";

             Server   = "gigasax";
             Database = "DMS5";     
        }

        /// <summary>
        /// Gets or sets the server name
        /// </summary>
        public string Server
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string Database
        {
            get;         
            set;
        }
        
        public void LoadFactorsFromDMS(List<DatasetInformation> datasets, FeatureDataAccessProviders providers)
        {
            if (datasets.Count < 1)
            { 
                return;
            }

            MultiAlignFactorSink sink = new MultiAlignFactorSink(datasets,
                                                                providers.DatasetCache,
                                                                providers.FactorCache,
                                                                providers.FactorAssignmentCache);
            foreach (DatasetInformation info in datasets)
            {
                string query        = m_sqlQuery + string.Format(" = '{0}'", info.DatasetName);
                MSSQLReader reader  = new MSSQLReader();
                reader.Server       = Server;
                reader.Database     = Database;
                reader.SQLText      = query;
                
                ProcessingPipeline pipeline = ProcessingPipeline.Assemble("PlainFactors", reader, sink);
                pipeline.RunRoot(null);                                
            }
            sink.CommitChanges();
        }   
        public void LoadFactorsFromFile(string path, List<DatasetInformation> datasets, FeatureDataAccessProviders providers)
        {
            if (datasets.Count < 1)
            { 
                return;
            }

            MultiAlignFactorSink sink = new MultiAlignFactorSink(datasets,
                                                                providers.DatasetCache,
                                                                providers.FactorCache,
                                                                providers.FactorAssignmentCache); 
                
            DelimitedFileReader reader = new DelimitedFileReader();
            reader.Delimiter    = "\t";
            reader.FilePath     = path;
            
            ProcessingPipeline pipeline = ProcessingPipeline.Assemble("PlainFactors", reader, sink);
            pipeline.RunRoot(null);
            sink.CommitChanges();                                      
        }       
    }
}
