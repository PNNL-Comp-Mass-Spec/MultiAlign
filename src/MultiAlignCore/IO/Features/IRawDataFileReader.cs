using System.Collections.Generic;
using PNNLOmics.Data;

namespace MultiAlignCore.IO.Features
{
    public interface IRawDataFileReader
    {
        /// <summary>
        /// Reads a raw data file with MSMS spectra.
        /// </summary>
        /// <returns></returns>
        List<MSSpectra> ReadMSMSSpectra(string file);      
    }
}