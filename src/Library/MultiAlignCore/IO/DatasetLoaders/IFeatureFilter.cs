namespace MultiAlignCore.IO.DatasetLoaders
{
    using System.Collections.Generic;

    public interface IFeatureFilter<T>
    {
        List<T> FilterFeatures(List<T> features);

        bool ShouldKeepFeature(T feature);
    }
}
