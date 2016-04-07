using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignRogue.DataLoading
{
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.DatasetLoaders;

    public class DatasetLoaderViewModelFactory
    {
        public static DatasetLoaderViewModelBase GetDatasetLoaderViewModel(IDatasetLoader loader)
        {
            if (loader is DeconToolsLoader)
            {
                return new DeconToolsLoaderViewModel(loader as DeconToolsLoader);
            }
            else if (loader is PromexFilter)
            {
                return new PromexLoaderViewModel(loader as PromexFilter);
            }
            else if (loader is LcImsFeatureFilter)
            {
                return new LcImsFeatureFinderLoaderViewModel(loader as LcImsFeatureFilter);
            }
            else
            {
                throw new ArgumentException(string.Format("No view model found for {0}.", loader.GetType()));
            }
        }
    }
}
