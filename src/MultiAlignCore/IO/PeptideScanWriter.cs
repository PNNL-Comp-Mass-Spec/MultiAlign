using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;
using System.IO;
using PNNLOmics.Data;

namespace MultiAlignCore.IO
{
    /// <summary>
    /// Writes the features that have MS/MS spectra 
    /// </summary>
    public class PeptideScanWriter
    {
        public void Write(string path, List<UMCLight> features)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                writer.WriteLine("Dataset, Feature_Id, MS_Feature_Id, MsMs_Feature_ID, Ms_Charge, NET, NET_Aligned, MsMs_PrecursorMz, MsMs_Scan, Peptide_Sequence, Peptide_Score");
                int totalFragments = 0;
                int totalPeptides = 0;
                int totalMultipleFragments = 0;
                int totalMultipleNonIdentified = 0;

                foreach (UMCLight feature in features)
                {
                    int totalFeatureFragments = 0;
                    int totalFeatureIdentifiedFragments = 0;

                    foreach (MSFeatureLight msFeature in feature.MSFeatures)
                    {
                        StringBuilder builder = new StringBuilder();

                        foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                        {

                            totalFeatureFragments++;
                            totalFragments++;
                            if (spectrum.Peptides.Count > 0)
                            {
                                totalPeptides++;
                                totalFeatureIdentifiedFragments++;
                            }

                            builder.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                                                                        feature.GroupID,
                                                                        feature.ID,
                                                                        msFeature.ID,
                                                                        spectrum.ID,
                                                                        msFeature.ChargeState,
                                                                        feature.NET,
                                                                        feature.NETAligned,
                                                                        spectrum.PrecursorMZ,
                                                                        spectrum.Scan));

                            foreach (Peptide peptide in spectrum.Peptides)
                            {
                                builder.Append(string.Format(",{0},{1}", peptide.Sequence,
                                                           peptide.Score));
                            }
                            writer.WriteLine(builder.ToString());
                        }
                    }

                    if (totalFeatureFragments > 1)
                    {
                        totalMultipleFragments++;

                        if (totalFeatureIdentifiedFragments < 1)
                        {
                            totalMultipleNonIdentified++;
                        }
                    }
                }

                writer.WriteLine();
                writer.WriteLine("Total Features, Total MsMs, Total Identified, Total Multiple MsMs, Total Multiple MsMs Without Ids");
                writer.WriteLine("{0},{1},{2},{3},{4}", features.Count, totalFragments, totalPeptides, totalMultipleFragments, totalMultipleNonIdentified);
            }
        }
    }
}
