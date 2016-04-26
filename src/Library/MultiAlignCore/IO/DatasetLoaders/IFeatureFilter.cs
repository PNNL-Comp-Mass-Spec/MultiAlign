namespace MultiAlignCore.IO.DatasetLoaders
{
    using System.Collections.Generic;

    using MultiAlignCore.Data;

    public interface IFeatureFilter<T> : ISettingsContainer
    {
        List<T> FilterFeatures(List<T> features);

        bool ShouldKeepFeature(T feature);
    }
}
