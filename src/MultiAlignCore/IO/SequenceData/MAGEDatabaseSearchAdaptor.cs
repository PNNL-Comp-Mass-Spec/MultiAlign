using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Factors;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlignCore.IO.SequenceData
{
    /// <summary>
    /// Adapter to the MAGE file library.
    /// </summary>
    public class MAGEDatabaseSearchAdaptor: PNNLOmics.Algorithms.IProgressNotifer
    {
        private void UpdateStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new PNNLOmics.Algorithms.ProgressNotifierArgs(message));
            }
        }

        public void LoadSequenceData(InputFile                   dataset,
                                     int                         datasetID,
                                     FeatureDataAccessProviders  providers)
        {

            if (dataset.FileType == InputFileType.Sequence)
            {

                IMageSink sink = null;                
                if (dataset.Path.ToLower().EndsWith("fht.txt"))
                {
                    UpdateStatus("First Hit File MAGE Sink created. ");

                    SequestFirstHitSink sequest = new SequestFirstHitSink(providers.DatabaseSequenceCache);
                    sequest.DatasetID           = datasetID;
                    sink                        = sequest;
                }
                else
                {
                    UpdateStatus("File type is not supported for this kind of sequence data. ");
                    return;
                }
                DelimitedFileReader reader  = new DelimitedFileReader();
                reader.Delimiter            = "\t";
                reader.FilePath             = dataset.Path;
                   
                ProcessingPipeline pipeline = ProcessingPipeline.Assemble("PlainFactors", reader, sink);
                pipeline.RunRoot(null);
                sink.CommitChanges();                   
            }     
                     
        }

        #region IProgressNotifer Members

        public event EventHandler<PNNLOmics.Algorithms.ProgressNotifierArgs> Progress;

        #endregion
    }
}
