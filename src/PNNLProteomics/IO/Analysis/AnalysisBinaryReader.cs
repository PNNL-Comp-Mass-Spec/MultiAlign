using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using MultiAlignCore.IO;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO
{
    public class AnalysisBinaryReader: IMultiAlignAnalysisReader
    {
        /// <summary>
        /// Fired when progress is made during reading.  
        /// </summary>
        public event EventHandler<IOProgressEventArgs> Progress;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public MultiAlignAnalysis ReadAnalysis(string path)
        {

            MultiAlignAnalysis analysis = null;

            //TODO: Wrap in using construct.
            // Open the file containing the data that you want to deserialize.
            FileStream fs = new FileStream(path, System.IO.FileMode.Open);
            try
            {
                if (fs == null)
                    throw new NullReferenceException("The file stream was null.");

                clsReadProgressStream progressStream = new clsReadProgressStream(fs);
                progressStream.ProgressChanged += new clsReadProgressStream.ProgressChangedEventHandler(progressStream_ProgressChanged);

                const int defaultBufferSize = 4096;
                int onePercentSize = Convert.ToInt32(Math.Ceiling(progressStream.Length / 100.0));

                BufferedStream bs = new BufferedStream(progressStream,
                        onePercentSize > defaultBufferSize ? defaultBufferSize : onePercentSize);

                BinaryFormatter formatter = new BinaryFormatter();
                analysis = formatter.Deserialize(bs) as MultiAlignAnalysis;
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                throw e;
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }

            if (analysis != null)
            {
                if (analysis.MetaData.AnalysisName == null)
                    analysis.MetaData.AnalysisName = string.Empty;
                analysis.MetaData.AnalysisPath = path;

                if (analysis.AlignmentData == null)
                    analysis.AlignmentData = new List<classAlignmentData>();
            }
            return analysis;
        }

        /// <summary>
        /// Handles when the binary reader moves the file pointer.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="progress"></param>
        void progressStream_ProgressChanged(object o, int progress)
        {
            if (Progress != null)
                Progress(this, new IOProgressEventArgs("Reading analysis binary file.", progress));
        }
    }
}
