
namespace MultiAlignCore.Algorithms.FeatureFinding
{
    public class FeatureFinderFactory
    {
        /// <summary>
        /// Creates a new feature finder based on a known type of algorithm.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IFeatureFinder CreateFeatureFinder(FeatureFinderType type)
        {
            IFeatureFinder finder = null;
            switch (type)
            {
                case FeatureFinderType.SingleLinkage:
                    finder = new UMCFeatureFinder();
                    break;
                case FeatureFinderType.TreeBased:
                    finder = new UMCTreeFeatureFinder();
                    break;
                default:
                    break;
            }

            return finder;            
        }
    }

    public enum FeatureFinderType
    {
        SingleLinkage,
        TreeBased
    }
}
