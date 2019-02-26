using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue.Feature_Finding
{
    using System.Collections.Generic;

    public interface IFeatureWindowFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(Dictionary<DatasetInformation, IList<UMCLight>> Features, bool showAlignedFeatures);
    }
}
