using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.Alignment;

namespace MultiAlignRogue
{
    interface IAlignmentWindowFactory
    {
        void CreateNewWindow();
        void CreateNewWindow(classAlignmentData alignment);
    }
}
