using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue.Feature_Finding
{
    using System.Collections.Generic;

    class MSFeatureViewFactory: IFeatureWindowFactory
    {
        public void CreateNewWindow()
        {

        }
        public void CreateNewWindow(Dictionary<DatasetInformation, IList<UMCLight>> Features, bool showAlignedFeatures)
        {
            var viewModel = new MSFeatureViewModel(Features, showAlignedFeatures);
            MSFeatureView window = new MSFeatureView
            {
                DataContext = viewModel
            };

            window.Show();
        }
    }
}
