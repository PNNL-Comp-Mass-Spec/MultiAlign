namespace MultiAlignCore.IO.TextFiles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using InformedProteomics.Backend.Data.Biology;
    using InformedProteomics.Backend.MassFeature;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.RawData;
    using MultiAlignCore.IO.DatasetLoaders;

    public class PromexFileReader : ITextFileReader<UMCLight>
    {
        /// <summary>
        /// Spectra provider.
        /// </summary>
        private readonly InformedProteomicsReader reader;

        private readonly IFeatureFilter<UMCLight> featureFilter; 

        public PromexFileReader(InformedProteomicsReader reader, IFeatureFilter<UMCLight> featureFilter = null)
        {
            this.reader = reader;
            this.featureFilter = featureFilter;
        }

        public IEnumerable<UMCLight> ReadFile(string fileLocation)
        {
            var features = LcMsFeatureAlignment.LoadProMexResult(this.reader.GroupId, fileLocation,
                this.reader.LcMsRun);

            var umcLights = new List<UMCLight> { Capacity = features.Count };

            int umcId = 0;
            int msId = 0;
            foreach (var feature in features)
            {
                var chargeState = (feature.MinCharge + feature.MaxCharge) / 2;
                var mz = (feature.Mass + (chargeState * Constants.Proton)) / chargeState;

                // Parent feature
                var umcLight = new UMCLight
                {
                    Id = umcId++,
                    GroupId = this.reader.GroupId,
                    ScanStart = feature.MinScanNum,
                    ScanEnd = feature.MaxScanNum,
                    Abundance = feature.Abundance,
                    AbundanceSum = feature.Abundance,
                    ChargeState = chargeState,
                    MinCharge = feature.MinCharge,
                    MaxCharge = feature.MaxCharge,
                    Net = feature.Net,
                    NetAligned = feature.Net,
                    NetStart = feature.MinNet,
                    NetEnd = feature.MaxNet,
                    MassMonoisotopic = feature.Mass,
                    MassMonoisotopicAligned = feature.Mass,
                    Mz = mz
                };

                for (int chargestate = feature.MinCharge; chargestate <= feature.MaxCharge; chargestate++)
                {
                    // Add min point
                    umcLight.AddChildFeature(new MSFeatureLight
                    {
                        Id = msId++,
                        GroupId = this.reader.GroupId,
                        Scan = feature.MinScanNum,
                        Abundance = feature.Abundance,
                        ChargeState = chargestate,
                        Net = feature.MinNet,
                        MassMonoisotopic = feature.Mass,
                        Mz = mz
                    });

                    // Add max point
                    umcLight.AddChildFeature(new MSFeatureLight
                    {
                        Id = msId++,
                        GroupId = this.reader.GroupId,
                        Scan = feature.MaxScanNum,
                        Abundance = feature.Abundance,
                        ChargeState = chargestate,
                        Net = feature.MaxNet,
                        MassMonoisotopic = feature.Mass,
                        Mz = mz
                    });
                }

                //umcLight.CalculateStatistics(ClusterCentroidRepresentation.Median);

                if (this.featureFilter == null || this.featureFilter.ShouldKeepFeature(umcLight))
                {
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
