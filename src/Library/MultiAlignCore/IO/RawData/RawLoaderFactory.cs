using PNNLOmics.Data;
using System.IO;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Class in charge of creating RAW Data file readers.
    /// </summary>
    public class RawLoaderFactory
    {      
        /// <summary>
        /// Constructs a raw data file reader for reading the instrument (or equivalent) mass spectra.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        public static ISpectraProvider CreateFileReader(string name)
        {
            if (name == null)
                return null;

            ISpectraProvider reader     = null;
            string extension            = Path.GetExtension(name);

            // Otherwise create a new one.
            switch (extension.ToLower())
            {
                case ".raw":
                    reader = new ThermoRawDataFileReader();
                    break;
                case ".mzxml":
                    reader = new MzXMLReader();
                    break;
            }

            return reader;
        }
    }
}
