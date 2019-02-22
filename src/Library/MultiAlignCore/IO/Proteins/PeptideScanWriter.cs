#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.IO.Proteins
{
    /// <summary>
    /// Writes the features that have MS/MS spectra
    /// </summary>
    public class PeptideScanWriter
    {
        public void Write(string path, IEnumerable<UMCLight> features)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                writer.WriteLine(
                    "Dataset, Feature_Id, MS_Feature_Id, MsMs_Feature_ID, Ms_Charge, NET, NET_Aligned, MsMs_PrecursorMz, MsMs_Scan, Peptide_Sequence, Peptide_Score");
                var totalFragments = 0;
                var totalPeptides = 0;
                var totalMultipleFragments = 0;
                var totalMultipleNonIdentified = 0;

                foreach (var feature in features)
                {
                    var totalFeatureFragments = 0;
                    var totalFeatureIdentifiedFragments = 0;

                    foreach (var msFeature in feature.MsFeatures)
                    {
                        var builder = new StringBuilder();

                        foreach (var spectrum in msFeature.MSnSpectra)
                        {
                            totalFeatureFragments++;
                            totalFragments++;
                            if (spectrum.Peptides.Count > 0)
                            {
                                totalPeptides++;
                                totalFeatureIdentifiedFragments++;
                            }

                            builder.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                                feature.GroupId,
                                feature.Id,
                                msFeature.Id,
                                spectrum.Id,
                                msFeature.ChargeState,
                                feature.Net,
                                feature.NetAligned,
                                spectrum.PrecursorMz,
                                spectrum.Scan));

                            foreach (var peptide in spectrum.Peptides)
                            {
                                builder.Append(string.Format(",{0},{1}", peptide.Sequence,
                                    peptide.Score));
                            }

                            // After we write, clear the data so we dont append multiple lines
                            writer.WriteLine(builder.ToString());
                            builder.Clear();
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
                writer.WriteLine(
                    "Total Features, Total MsMs, Total Identified, Total Multiple MsMs, Total Multiple MsMs Without Ids");
                writer.WriteLine("{0},{1},{2},{3},{4}", features.Count(), totalFragments, totalPeptides,
                    totalMultipleFragments, totalMultipleNonIdentified);
            }
        }
    }
}