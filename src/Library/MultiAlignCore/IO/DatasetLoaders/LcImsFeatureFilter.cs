namespace MultiAlignCore.IO.DatasetLoaders
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;

    public class LcImsFeatureFilter : IDatasetLoader, IFeatureFilter<UMCLight>
    {
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
