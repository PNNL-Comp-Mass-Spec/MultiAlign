﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue
{
    class MSFeatureViewFactory: IFeatureWindowFactory
    {
        public void CreateNewWindow()
        {
            
        }
        public void CreateNewWindow(Dictionary<DatasetInformation, IList<UMCLight>> Features)
        {
            MSFeatureView window = new MSFeatureView
            {
                DataContext = new MSFeatureViewModel(Features)
            };
            window.Show();
        }
    }
}
