using MultiAlignCore.Data.Features;

namespace MultiAlignRogue.Feature_Finding
{
    using System.Collections.Generic;

    using MultiAlignCore.Data.MetaData;


    public interface IFeatureWindowFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(Dictionary<DatasetInformation, IList<UMCLight>> Features, bool showAlignedFeatures);
    }
}
