using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Data.Alignment;

namespace MultiAlignRogue
{
    class AlignmentViewFactory : IAlignmentWindowFactory
    {
        public void CreateNewWindow()
        {

        }
        public void CreateNewWindow(classAlignmentData alignment)
        {
            AlignmentView window = new AlignmentView()
            {
                DataContext = new AlignmentViewModel(alignment)
            };
            window.Show();
        }
    }
}
