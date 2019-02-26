using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeatureAlignment.Data;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.TextFiles
{
    public class SkylineTransitionFileReader : ISequenceFileReader
    {
        public IEnumerable<Peptide> Read(string path)
        {
            var peptides  = new List<Peptide>();
            var lines      = File.ReadAllLines(path).ToList();

            var precursorMap = new Dictionary<string, List<List<string>>>();

            foreach (var line in lines)
            {
                var lineData = line.Split(',').ToList();
                if (lineData.Count < 6)
                {
                    continue;
                }

                if (!lineData[5].StartsWith("y"))
                {
                    if (!lineData[5].StartsWith("b"))
                    {
                        continue;
                    }
                }

                if (!precursorMap.ContainsKey(lineData[0]))
                {
                    precursorMap.Add(lineData[0], new List<List<string>>());
                }
                precursorMap[lineData[0]].Add(lineData);
            }

            foreach (var key in precursorMap.Keys)
            {
                var data     = precursorMap[key];
                var peptide             = new Peptide();
                peptide                     = new Peptide();
                peptide.Sequence            = data[0][3];
                var spectrum          = new MSSpectra();
                spectrum.PrecursorMz        = Convert.ToDouble(key);

                foreach (var line in data)
                {
                    var fragment = Convert.ToDouble(line[1]);
                    var point    = new XYData(fragment, 100);
                    spectrum.Peaks.Add(point);
                }
                spectrum.Peptides = new List<Peptide>();
                spectrum.Peptides.Add(peptide);
                peptide.Spectrum = spectrum;
                peptides.Add(peptide);
            }
            return peptides;
        }
    }
}
