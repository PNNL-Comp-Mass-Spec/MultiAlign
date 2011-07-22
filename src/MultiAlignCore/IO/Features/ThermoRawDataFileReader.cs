using System.Collections.Generic;
using PNNLOmics.Data;
using System.IO;
using ThermoRawFileReaderDLL.FinniganFileIO;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Adapter for the Thermo Finnigan file format reader made by Matt Monroe.
    /// </summary>
    public class ThermoRawDataFileReader: IRawDataFileReader
    {
        #region IRawDataFileReader Members

        public List<MSSpectra> ReadMSMSSpectra(string file)
        {


            List<MSSpectra> spectra = new List<MSSpectra>();
                        
            XRawFileIO rawReader    = new XRawFileIO();
            bool opened             = rawReader.OpenRawFile(file);

            if (!opened)
            {
                throw new IOException("Could not open the Thermo raw file " + file);
            }

            int numberOfScans = rawReader.GetNumScans();
            for (int i = 0; i < numberOfScans; i++)
            {

                FinniganFileReaderBaseClass.udtScanHeaderInfoType header = new FinniganFileReaderBaseClass.udtScanHeaderInfoType();
                rawReader.GetScanInfo(i, ref header);

                if (header.MSLevel > 1)
                {
                    MSSpectra spectrum          = new MSSpectra();
                    spectrum.MSLevel            = header.MSLevel;
                    spectrum.RetentionTime      = header.RetentionTime;
                    spectrum.Scan               = i;
                    spectrum.PrecursorMZ        = header.ParentIonMZ;
                    spectrum.TotalIonCurrent    = header.TotalIonCurrent;

                    // Need to make this a standard type of collision based off of the data.
                    spectrum.CollisionType      = CollisionType.Other;

                    spectra.Add(spectrum);
                }
            }
            return spectra;
        }
        #endregion
    }
}
