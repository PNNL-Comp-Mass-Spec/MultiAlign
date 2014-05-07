using System.Collections.Generic;
using Mage;
using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.IO.MTDB
{
    public sealed class MetaSampleDatbaseLoader: IMtdbLoader
    {
        private readonly Algorithms.Options.MassTagDatabaseOptions m_options;

        public MetaSampleDatbaseLoader(string path, Algorithms.Options.MassTagDatabaseOptions options)
        {
            Path        = path;
            m_options   = options;
        }

        public MassTagDatabase LoadDatabase()
        {
            var database = new MassTagDatabase();

            
            var sink = new MAGEMetaSampleDatabaseSink();
            using (var reader = new DelimitedFileReader())
            {
                reader.Delimiter    = ",";
                reader.FilePath     = Path;

                var pipeline = ProcessingPipeline.Assemble("MetaSample", reader, sink);
                pipeline.RunRoot(null);
            }

            
            var tags = sink.MassTags.FindAll(delegate(MassTagLight x) { return x.ObservationCount >= m_options.MinimumObservationCountFilter; });
            
            database.AddMassTagsAndProteins(tags, new Dictionary<int,List<Protein>>());
        
            // Fill in logic to read new type of mass tag database.
            return database;
        }


        public string Path
        {
            get;
            set;
        }
    }
}
