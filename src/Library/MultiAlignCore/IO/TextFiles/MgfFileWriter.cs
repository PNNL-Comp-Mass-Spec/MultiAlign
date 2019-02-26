using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FeatureAlignment.Data;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.TextFiles
{
    /// <summary>
    /// Writes Mascot Generic File formatted MS/MS spectra.
    /// </summary>
    public class MgfFileWriter: IMsMsSpectraWriter
    {
        /// <summary>
        /// Creates a MGF file based on the spectra provided.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="msmsFeatures"></param>
        public void Write(string path, IEnumerable<MSSpectra> msmsFeatures)
        {
            var baseName = Path.GetFileNameWithoutExtension(path);
            using (TextWriter writer = File.CreateText(path))
            {
                Write(writer, baseName, msmsFeatures);
            }
        }

        /// <summary>
        /// Creates the MGF file based on the spectra provided.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="msmsFeatures"></param>
        public void Append(string path, IEnumerable<MSSpectra> msmsFeatures)
        {
            var baseName = Path.GetFileNameWithoutExtension(path);
            using (TextWriter writer = File.AppendText(path))
            {
                Write(writer, baseName, msmsFeatures);
            }
        }

        private void Write(TextWriter writer, string baseName, IEnumerable<MSSpectra> msmsFeatures)
        {
            foreach (var feature in msmsFeatures)
            {
                var sb = new StringBuilder();

                sb.Append("BEGIN IONS");
                sb.Append(Environment.NewLine);
                sb.Append(string.Format("TITLE={0}.1.dta", feature.Scan));
                sb.Append(Environment.NewLine);

                sb.Append(string.Format("PEPMASS={0}", feature.PrecursorMz));
                sb.Append(Environment.NewLine);

                sb.Append(string.Format("CHARGE={0}+", feature.PrecursorChargeState));
                sb.Append(Environment.NewLine);

                foreach (var peak in feature.Peaks)
                {
                    sb.Append(Math.Round(peak.X, 5));
                    sb.Append(" ");
                    sb.Append(peak.Y);
                    sb.Append(Environment.NewLine);
                }
                sb.Append("END IONS");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                writer.WriteLine(sb.ToString());
            }
        }
    }
}
