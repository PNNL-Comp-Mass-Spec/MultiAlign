using System.Collections.Generic;
using System.IO;
using MSDataFileReader;
using PNNLOmics.Data;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Adapter for reading mzXML files from a reader by Matt Monroe.
    /// </summary>
    public class MzXMLReader: IRawDataFileReader
    {
        #region IRawDataFileReader Members
        /// <summary>
        /// Reads a list of MSMS Spectra header data from the mzXML file.
        /// </summary>
        /// <param name="file">file to read.</param>
        /// <returns>List of MSMS spectra data</returns>
        public List<MSSpectra> ReadMSMSSpectra(string file)
        {
            List<MSSpectra> spectra     = new List<MSSpectra>();
            clsMzXMLFileReader reader   = new clsMzXMLFileReader();
            reader.SkipBinaryData       = true;
            bool opened                 = reader.OpenFile(file);

            if (!opened)
            {
                throw new IOException("Could not open the mzXML file " + file);
            }

            int totalScans = reader.ScanCount;

            for (int i = 0; i < totalScans; i++)
            {
                
                clsSpectrumInfo info = new clsSpectrumInfo();
                reader.GetSpectrumByScanNumber(i, ref info);
                if (info.MSLevel > 1)
                {
                    MSSpectra spectrum          = new MSSpectra();
                    spectrum.MSLevel            = info.MSLevel;
                    spectrum.RetentionTime      = info.RetentionTimeMin;
                    spectrum.Scan               = i;
                    spectrum.PrecursorMZ        = info.ParentIonMZ;
                    spectrum.TotalIonCurrent    = info.TotalIonCurrent;

                    // Need to make this a standard type of collision based off of the data.
                    spectrum.CollisionType = CollisionType.Other;                    
                    spectra.Add(spectrum);
                }
            }
            reader.CloseFile();
            return spectra;
        }

        #endregion
    }
}
