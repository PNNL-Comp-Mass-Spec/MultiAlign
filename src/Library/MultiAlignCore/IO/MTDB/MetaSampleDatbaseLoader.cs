#region

using System.Collections.Generic;
using Mage;
using MultiAlignCore.Algorithms.Options;
using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;

#endregion

namespace MultiAlignCore.IO.MTDB
{
    public sealed class MetaSampleDatbaseLoader : IMtdbLoader
    {
        private readonly MassTagDatabaseOptions m_options;

        public MetaSampleDatbaseLoader(string path, MassTagDatabaseOptions options)
        {
            Path = path;
            m_options = options;
        }

        public MassTagDatabase LoadDatabase()
        {
            var database = new MassTagDatabase();


            var sink = new MAGEMetaSampleDatabaseSink();
            using (var reader = new DelimitedFileReader())
            {
                reader.Delimiter = ",";
                reader.FilePath = Path;

                var pipeline = ProcessingPipeline.Assemble("MetaSample", reader, sink);
                pipeline.RunRoot(null);
            }


            var tags =
                sink.MassTags.FindAll(
                    delegate(MassTagLight x) { return x.ObservationCount >= m_options.MinimumObservationCountFilter; });

            database.AddMassTagsAndProteins(tags, new Dictionary<int, List<Protein>>());

            // Fill in logic to read new type of mass tag database.
            return database;
        }


        public string Path { get; set; }
    }
}