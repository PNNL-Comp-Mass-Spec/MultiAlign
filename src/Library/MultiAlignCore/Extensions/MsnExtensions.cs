using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class MsnExtensions
    {
        public static UMCLight GetParentUmc(this MSSpectra spectrum)
        {
            if (spectrum == null) return null;

            if (spectrum.ParentFeature != null)
            {
                return spectrum.ParentFeature.GetParentUmc();
            }
            return null;
        }
    }
}