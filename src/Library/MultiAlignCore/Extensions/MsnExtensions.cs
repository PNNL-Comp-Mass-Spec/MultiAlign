using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class MsnExtensions
    {
        public static UMCLight GetParentUmc(this MSSpectra spectrum)
        {
            return spectrum?.ParentFeature?.GetParentUmc();
        }
    }
}