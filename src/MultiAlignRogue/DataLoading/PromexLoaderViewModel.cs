using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignRogue.DataLoading
{
    using GalaSoft.MvvmLight;

    using MultiAlignCore.IO.DatasetLoaders;

    using DatasetLoader = MultiAlignCore.Data.DatasetLoader;

    public class PromexLoaderViewModel : DatasetLoaderViewModelBase
    {
        public PromexLoaderViewModel(PromexFilter loader)
        {
            this.DatasetLoader = loader;
        }
    }
}
