namespace MultiAlignCore.IO.DatasetLoaders
{
    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;

    public class DatasetLoaderFactory
    {
        public static IDatasetLoader GetDatasetLoader(DatasetInformation dataset)
        {
            var path = dataset.FeaturePath;
            if (path.EndsWith(".ms1ft"))
            {
                return new PromexFilter();
            }
            else if (path.EndsWith("_LCMSFeatures.txt"))
            {
                return new LcImsFeatureFilter();
            }
            else
            {
                return new DeconToolsLoader(new DeconToolsFilter());
            }
        }
    }
}
