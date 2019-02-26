using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;

namespace MultiAlignCore.IO.TextFiles
{
    using System.IO;
    using System.Windows;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;

    public class LcImsFeatureFileReader : ITextFileReader<UMCLight>
    {
        private int datasetId = 0;

        private IScanSummaryProvider provider;

        public LcImsFeatureFileReader(IScanSummaryProvider provider = null, int datasetId = 0)
        {
            this.provider = provider;
            this.datasetId = datasetId;
        }

        public IEnumerable<UMCLight> ReadFile(string fileLocation)
        {
            var headers = new Dictionary<string, int>();
            var umcs = new List<UMCLight>();

            int msFeatureId = 0;

            int lineCount = 0;
            foreach (var line in File.ReadLines(fileLocation))
            {
                var parts = line.Split('\t');
                if (lineCount++ == 0)
                {   // Get the headers if this if the first line
                    for (int i = 0; i < parts.Length; i++)
                    {
                        headers.Add(parts[i], i);
                    }

                    continue;
                }

                var minLcScan = Convert.ToInt32(parts[headers["Scan_Start"]]);
                var maxLcScan = Convert.ToInt32(parts[headers["Scan_End"]]);
                var minNet = 0.0;
                var maxNet = 0.0;
                if (this.provider != null)
                {
                    var minScanSum = provider.GetScanSummary(minLcScan);
                    minNet = minScanSum.Net;

                    var maxScanSum = provider.GetScanSummary(maxLcScan);
                    maxNet = maxScanSum.Net;
                }

                var umc = new UMCLight
                {
                    GroupId = this.datasetId,
                    Id = Convert.ToInt32(parts[headers["Feature_Index"]]),
                    ChargeState = Convert.ToInt32(parts[headers["Class_Rep_Charge"]]),
                    MassMonoisotopic = Convert.ToDouble(parts[headers["Monoisotopic_Mass"]]),
                    MassMonoisotopicAligned = Convert.ToDouble(parts[headers["Monoisotopic_Mass"]]),
                    Mz = Convert.ToDouble(parts[headers["Class_Rep_MZ"]]),
                    Abundance = Convert.ToDouble(parts[headers["Abundance"]]),
                    ScanStart = minLcScan,
                    NetStart = minNet,
                    ScanEnd = maxLcScan,
                    NetEnd = maxNet,
                    Net = (minNet + maxNet) / 2,
                    NetAligned = (minNet + maxNet) / 2,
                    ScanAligned = (minLcScan + maxLcScan) / 2,
                    ImsScanStart = Convert.ToInt32(parts[headers["IMS_Scan_Start"]]),
                    ImsScanEnd = Convert.ToInt32(parts[headers["IMS_Scan_End"]]),
                    DriftTime = Convert.ToDouble(parts[headers["Drift_Time"]]),
                    ConformationFitScore =
                        Convert.ToDouble(parts[headers["Conformation_Fit_Score"]]),
                    AverageDeconFitScore = Convert.ToDouble(parts[headers["Average_Isotopic_Fit"]]),
                    SaturatedMemberCount =
                        Convert.ToInt32(parts[headers["Saturated_Member_Count"]]),
                };

                // min feature
                umc.AddChildFeature(new MSFeatureLight
                {
                    GroupId = datasetId,
                    Id = msFeatureId++,
                    ChargeState = umc.ChargeState,
                    MassMonoisotopic = umc.MassMonoisotopic,
                    MassMonoisotopicAligned = umc.MassMonoisotopicAligned,
                    Scan = umc.ScanStart,
                    Net = umc.NetStart,
                    NetAligned = umc.NetStart,
                    ImsScanStart = umc.ImsScanStart,
                    ImsScanEnd = umc.ImsScanEnd,
                    DriftTime = umc.DriftTime,
                });

                // max feature
                umc.AddChildFeature(new MSFeatureLight
                {
                    GroupId = datasetId,
                    Id = msFeatureId++,
                    ChargeState = umc.ChargeState,
                    MassMonoisotopic = umc.MassMonoisotopic,
                    MassMonoisotopicAligned = umc.MassMonoisotopicAligned,
                    Scan = umc.ScanEnd,
                    Net = umc.NetEnd,
                    NetAligned = umc.NetEnd,
                    ImsScanStart = umc.ImsScanStart,
                    ImsScanEnd = umc.ImsScanEnd,
                    DriftTime = umc.DriftTime,
                });

                umcs.Add(umc);
            }

            return umcs;
        }

        public IEnumerable<UMCLight> ReadFile(TextReader textReader)
        {
            throw new NotImplementedException();
        }
    }
}
