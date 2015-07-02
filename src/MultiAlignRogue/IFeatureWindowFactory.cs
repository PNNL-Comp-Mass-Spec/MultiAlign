using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue
{
    interface IFeatureWindowFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(Dictionary<DatasetInformation, IList<UMCLight>> Features);
    }
}
