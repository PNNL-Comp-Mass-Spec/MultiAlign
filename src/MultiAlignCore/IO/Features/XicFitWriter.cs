using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using System.IO;
using PNNLOmics.Data;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Writes the fitted XIC values to file.
    /// </summary>
    public class XicFitWriter : IXicWriter 
    {
        public void WriteXics(string path,
                              List<UMCLight> features)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                foreach (UMCLight feature in features)
                {
                    foreach (int charge in feature.ChargeStateChromatograms.Keys)
                    {
                        writer.WriteLine("id\t{0}\tcharge\t{1}", feature.ID, charge);
                        writer.WriteLine("mz\tscan\tintensity");

                        Chromatogram gram = feature.ChargeStateChromatograms[charge];

                        foreach (PNNLOmics.Data.XYData point in gram.FitPoints)                                                
                        {
                            writer.WriteLine("{0}\t{1}t{2}", gram.Mz, point.X, point.Y);
                        }                        
                    }
                }
            }
        }
    }
}
