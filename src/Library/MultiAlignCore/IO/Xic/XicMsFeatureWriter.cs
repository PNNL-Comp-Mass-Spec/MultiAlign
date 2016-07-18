using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MultiAlignCore.Extensions;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Creates an XIC based on the old way features were reported.
    /// </summary>
    public class XicMsFeatureWriter : IXicWriter
    {
        public void WriteXics(string path,
                              List<UMCLight> features)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                foreach (UMCLight feature in features)
                {
                    Dictionary<int, List<XYZData>> chargeMap = feature.CreateChargeSIC();

                    foreach (int charge in chargeMap.Keys)
                    {
                        writer.WriteLine("id\t{0}\tcharge\t{1}", feature.ID, charge);
                        writer.WriteLine("mz\tscan\tintensity");


                        List<XYZData> gram = chargeMap[charge];
                        foreach (XYZData point in gram)
                        {
                            writer.WriteLine("{0}\t{1}t{2}", point.Z, point.X, point.Y);
                        }
                    }
                }
            }
        }
    }
}
