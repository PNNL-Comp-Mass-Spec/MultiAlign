namespace MultiAlignCore.IO.DatasetLoaders
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.Features;

    public class LcImsFeatureFilter : IDatasetLoader, IFeatureFilter<UMCLight>, ISettingsContainer
    {
        public LcImsFeatureFilter(FeatureDataAccessProviders dataProviders)
        {
            
        }

        public List<UMCLight> FilterFeatures(List<UMCLight> features)
        {
            throw new NotImplementedException();
        }

        public bool ShouldKeepFeature(UMCLight feature)
        {
            throw new NotImplementedException();
        }

        public void RestoreDefaults()
        {
            throw new NotImplementedException();
        }

        public void Load(DatasetInformation dataset, IUmcDAO dataAccessProvider, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public List<UMCLight> Load(DatasetInformation dataset, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}
