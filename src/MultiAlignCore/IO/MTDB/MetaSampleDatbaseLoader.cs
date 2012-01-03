using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.MassTags;
using PNNLOmics.Data.MassTags;
using Mage;

namespace MultiAlignCore.IO.MTDB
{
    public class MetaSampleDatbaseLoader: IMtdbLoader
    {
        public MetaSampleDatbaseLoader(string path)
        {
            Path = path;
        }

        public MassTagDatabase LoadDatabase()
        {
            MassTagDatabase database = new MassTagDatabase();

            MAGEMetaSampleDatabaseSink sink = new MAGEMetaSampleDatabaseSink();
            using (DelimitedFileReader reader = new DelimitedFileReader())
            {
                reader.Delimiter    = ",";
                reader.FilePath     = Path;

                ProcessingPipeline pipeline = ProcessingPipeline.Assemble("MetaSample", reader, sink);
                pipeline.RunRoot(null);
            }

            
            List<MassTagLight> tags = sink.MassTags.FindAll(delegate(MassTagLight x) { return x.ObservationCount >= Options.MinimumObservationCountFilter; });
            
            database.AddMassTagsAndProteins(tags, new Dictionary<int,List<PNNLOmics.Data.Protein>>());
        
            // Fill in logic to read new type of mass tag database.
            return database;
        }


        public MassTagDatabaseOptions Options
        {
            get;
            set;
        }
        public string Path
        {
            get;
            set;
        }
    }
}
