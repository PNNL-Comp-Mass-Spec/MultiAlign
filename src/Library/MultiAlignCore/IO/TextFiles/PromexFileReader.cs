using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassFeature;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.RawData;

namespace MultiAlignCore.IO.TextFiles
{
    public class PromexFileReader : ITextFileReader<UMCLight>
    {
        private readonly InformedProteomicsReader reader;

        private readonly int datasetId;

        public PromexFileReader(InformedProteomicsReader reader, int datasetId)
        {
            this.reader = reader;
            this.datasetId = datasetId;
        }

        public IEnumerable<UMCLight> ReadFile(string fileLocation)
        {
            var features = LcMsFeatureAlignment.LoadProMexResult(this.datasetId, fileLocation,
                this.reader.GetReaderForGroup(0));

            var umcLights = new List<UMCLight> { Capacity = features.Count };

            int umcId = 0;
            int msId = 0;
            foreach (var feature in features)
            {
                for (int chargestate = feature.MinCharge; chargestate <= feature.MaxCharge; chargestate++)
                {
                    // Parent feature
                    var umcLight = new UMCLight
                    {
                        Id = umcId++,
                        GroupId = this.datasetId,
                        ScanStart = feature.MinScanNum,
                        ScanEnd = feature.MaxScanNum,
                        Abundance = feature.Abundance,
                        AbundanceSum = feature.Abundance,
                        ChargeState = chargestate,
                        Net = feature.Net,
                        NetAligned = feature.Net,
                    };

                    // Add min point
                    umcLight.AddChildFeature(new MSFeatureLight
                    {
                        Id = msId++,
                        GroupId = this.datasetId,
                        Scan = feature.MinScanNum,
                        Abundance = feature.Abundance,
                        ChargeState = chargestate,
                        Net = feature.MinNet
                    });

                    // Add max point
                    umcLight.AddChildFeature(new MSFeatureLight
                    {
                        Id = msId++,
                        GroupId = this.datasetId,
                        Scan = feature.MaxScanNum,
                        Abundance = feature.Abundance,
                        ChargeState = chargestate,
                        Net = feature.MinNet
                    });

                    umcLight.CalculateStatistics(ClusterCentroidRepresentation.Median);

                    umcLights.Add(umcLight);
                }
            }

            return umcLights;
        }

        public IEnumerable<UMCLight> ReadFile(TextReader textReader)
        {
            throw new NotImplementedException();
        }
    }
}
