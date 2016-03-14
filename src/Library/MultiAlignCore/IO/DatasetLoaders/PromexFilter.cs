namespace MultiAlignCore.IO.DatasetLoaders
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.Features;

    public class PromexFilter : IDatasetLoader, IFeatureFilter<UMCLight>
    {
        public ElutionTimePoint MinimumElutionTime { get; set; }

        public ElutionTimePoint MaximumElutionTime { get; set; }

        public List<UMCLight> FilterFeatures(List<UMCLight> features)
        {
            throw new NotImplementedException();
        }

        public bool ShouldKeepFeature(UMCLight feature)
        {
            throw new NotImplementedException();
        }

        public void ResetDefaults()
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> Load(DatasetInformation dataset, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}
