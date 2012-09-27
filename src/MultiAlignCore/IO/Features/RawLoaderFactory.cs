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
        private static Dictionary<string, ISpectraProvider> m_readers = new Dictionary<string, ISpectraProvider>();
        /// <summary>
        /// Registers a provider if not created before.
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="provider"></param>
        private static void RegisterProviderAsSingleton(string extension, ISpectraProvider provider)
        {
            if (!m_readers.ContainsKey(extension))
            {
                m_readers.Add(extension, provider);
            }
        }
        /// <summary>
        /// Clears any used providers.
        /// </summary>
        public static void ClearProviderSingletons()
        {
            m_readers.Clear();
        }
        /// <summary>
        /// Constructs a raw data file reader for reading the instrument (or equivalent) mass spectra.
        /// </summary>
        /// <param name="name">Name of the file path.</param>
        /// <returns></returns>
        public static ISpectraProvider CreateFileReader(string name)
        {
            return CreateFileReader(name, true);
        }         
        /// <summary>
        /// Constructs a raw data file reader for reading the instrument (or equivalent) mass spectra.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        public static ISpectraProvider CreateFileReader(string name, bool useSingleton)
        {
            ISpectraProvider reader     = null;
            string extension            = Path.GetExtension(name);

            // Determine if we have a reader already made if we are using a singleton pattern.
            if (useSingleton)
            {
                if (m_readers.ContainsKey(extension))
                {
                    return m_readers[extension];
                }
            }

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
            if (useSingleton)
            {
                RegisterProviderAsSingleton(extension, reader);
            }
            return reader;
        }
    }
}
