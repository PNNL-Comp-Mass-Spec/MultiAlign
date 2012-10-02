using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PNNLOmics.Data;

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
