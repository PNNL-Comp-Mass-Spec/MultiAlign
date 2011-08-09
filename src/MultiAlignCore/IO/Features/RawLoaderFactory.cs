using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <returns></returns>
        public static IRawDataFileReader CreateFileReader(string name)
        {
            IRawDataFileReader reader   = null;
            string extension            = Path.GetExtension(name);
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
