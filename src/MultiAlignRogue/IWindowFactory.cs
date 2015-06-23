using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue
{
    interface IWindowFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(Dictionary<DatasetInformation, IList<UMCLight>> Features, List<DatasetInformation> selectedFiles);
    }
}
