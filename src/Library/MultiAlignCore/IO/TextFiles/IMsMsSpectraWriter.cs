using System.Collections.Generic;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.TextFiles
{
    public interface IMsMsSpectraWriter
    {
        /// <summary>
        /// Creates a MS/MS DTA file at the path provided.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="msmsFeatures"></param>
        void Write(string path, IEnumerable<MSSpectra> msmsFeatures);
        /// <summary>
        /// Appends the MS/MS spectra to the file path provided.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="msmsFeatures"></param>
        void Append(string path, IEnumerable<MSSpectra> msmsFeatures);
    }
}
