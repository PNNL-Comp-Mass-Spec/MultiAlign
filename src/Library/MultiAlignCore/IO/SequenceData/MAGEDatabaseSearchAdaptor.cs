#region

using System;
using Mage;
using PNNLOmics.Algorithms;

#endregion

namespace MultiAlignCore.IO.SequenceData
{
    /// <summary>
    ///     Adapter to the MAGE file library.
    /// </summary>
    public class MAGEDatabaseSearchAdaptor : IProgressNotifer
    {
        private void UpdateStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }

        public void LoadSequenceData(string path,
            int datasetID,
            IDatabaseSearchSequenceDAO databaseSequenceCache)
        {
            IMageSink sink = null;
            if (path.ToLower().EndsWith("fht.txt"))
            {
                UpdateStatus("First Hit File MAGE Sink created. ");

                var sequest = new SequestFirstHitSink(databaseSequenceCache);
                sequest.DatasetID = datasetID;
                sink = sequest;
            }
            else
            {
                UpdateStatus("File type is not supported for this kind of sequence data. ");
                return;
            }
            var reader = new DelimitedFileReader();
            
            reader.Delimiter = "\t";
            reader.FilePath = path;

            var pipeline = ProcessingPipeline.Assemble("PlainFactors", reader, sink);
            pipeline.RunRoot(null);
            sink.CommitChanges();
            
        }

        #region IProgressNotifer Members

        public event EventHandler<ProgressNotifierArgs> Progress;

        #endregion
    }
}