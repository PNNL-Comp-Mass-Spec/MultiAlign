using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
                        writer.WriteLine("mz\tfit scan\tfit intensity\tregular scan\tregular intensity");

                        Chromatogram gram = feature.ChargeStateChromatograms[charge];
                        int i = 0;
                        for (i = 0; i < gram.Points.Count; i++)
                        {
                            //(XYData point in gram.FitPoints)
                            XYData fitPoint = gram.FitPoints[i];
                            XYData regPoint = gram.Points[i];

                            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", gram.Mz, fitPoint.X, fitPoint.Y, regPoint.X, regPoint.Y);
                        }
                        for (; i < gram.FitPoints.Count; i++)
                        {
                            //(XYData point in gram.FitPoints)
                            XYData fitPoint = gram.FitPoints[i];
                            writer.WriteLine("{0}\t{1}\t{2}", gram.Mz, fitPoint.X, fitPoint.Y);
                        }
                    }
                }
            }
        }
    }
}
