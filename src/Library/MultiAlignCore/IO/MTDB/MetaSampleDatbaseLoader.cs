#region

using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Algorithms.Options;
using FeatureAlignment.Data.MassTags;
using Mage;
using MultiAlignCore.Algorithms.Options;

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
            var reader = new DelimitedFileReader();

            reader.Delimiter = ",";
            reader.FilePath = Path;

            var pipeline = ProcessingPipeline.Assemble("MetaSample", reader, sink);
            pipeline.RunRoot(null);



            var tags = sink.MassTags.Where(x => x.ObservationCount >= m_options.MinimumObservationCountFilter);

            database.AddMassTagsAndProteins(tags.ToList(), new Dictionary<int, List<Protein>>());

            // Fill in logic to read new type of mass tag database.
            return database;
        }


        public string Path { get; set; }
    }
}