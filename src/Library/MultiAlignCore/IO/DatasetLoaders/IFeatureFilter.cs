namespace MultiAlignCore.IO.DatasetLoaders
{
    using System.Collections.Generic;

    using MultiAlignCore.Data.Features;

    public interface IFeatureFilter<T>
        where T : FeatureLight
    {
        List<T> FilterFeatures(List<T> features);

        bool ShouldKeepFeature(T feature);
    }
}
