using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.TextFiles
{
    /// <summary>
    /// Writes Mascot Generic File formatted MS/MS spectra.
    /// </summary>
    public class MgfFileReader: IMsMsSpectraReader
    {
        /// <summary>
        /// Creates a MGF file based on the spectra provided.
        /// </summary>
        /// <param name="path"></param>
        public List<MSSpectra> Read(string path)
        {
            var lines  = File.ReadAllLines(path);
            var mode        = 0;

            var spectra   = new List<MSSpectra>();
            MSSpectra currentSpectrum = null;
            var delimiter       = new[] {" "};

            for(var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].ToUpper();
                if (line.Contains("BEGIN IONS"))
                {
                    mode    = 0;
                }
                else if (line.Contains("CHARGE="))
                {
                    mode            = 1;
                    i               = i + 1;
                    currentSpectrum = new MSSpectra();
                }
                else if (line.Contains("END IONS"))
                {
                    mode            = 0;
                    if (currentSpectrum != null)
                    {
                        spectra.Add(currentSpectrum);
                    }
                }


                if (mode == 1)
                {
                    var data = line.Split(delimiter,
                                                StringSplitOptions.RemoveEmptyEntries);

                    if (data.Length < 2)
                        continue;

                    try
                    {
                        var x = Convert.ToDouble(data[0]);
                        var y = Convert.ToDouble(data[1]);
                        var datum = new XYData(x, y);
                        currentSpectrum.Peaks.Add(datum);
                    }
                    catch
                    {
                    }
                }                
            }

            return spectra;
        }
    }
}
