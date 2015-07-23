namespace MultiAlignRogue.Feature_Finding
{
    using System.Collections.Generic;

    using MultiAlignCore.Data.MetaData;

    using PNNLOmics.Data.Features;

    class MSFeatureViewFactory: IFeatureWindowFactory
    {
        public void CreateNewWindow()
        {
            
        }
        public void CreateNewWindow(Dictionary<DatasetInformation, IList<UMCLight>> Features)
        {
            var viewModel = new MSFeatureViewModel(Features);
            MSFeatureView window = new MSFeatureView
            {
                DataContext = viewModel
            };

            window.Show();
        }
    }
}
