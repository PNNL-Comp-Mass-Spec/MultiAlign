using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


using PNNLProteomics.IO;
using PNNLProteomics.Data;

namespace PNNLProteomics.IO
{
    public class AnalysisBinaryWriter: IMultiAlignAnalysisWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="analysis"></param>
        public void WriteAnalysis(string path, MultiAlignAnalysis analysis)
        {
            // To serialize the hashtable and its key/value pairs,  
            // you must first open a stream for writing. 
            // In this case, use a file stream.
            FileStream fs = new FileStream(path, System.IO.FileMode.Create);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, analysis);
                fs.Close();
            }
            catch (SerializationException e)
            {
                throw e;   
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }
    }
}
