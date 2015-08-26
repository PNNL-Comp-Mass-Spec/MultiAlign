﻿#region

using System.Collections.Generic;
using MultiAlignCore.Data;

#endregion

namespace MultiAlignCore.IO.RawData
{
    public interface IRawDataFileReader
    {
        /// <summary>
        ///     Reads a raw data file with MSMS spectra.
        /// </summary>
        /// <returns></returns>
        List<MSSpectra> ReadMSMSSpectra(string file);

        /// <summary>
        ///     Adds a path to a file for supporting multiple file readers.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupID"></param>
        void AddDataFile(string path, int groupID);
    }
}