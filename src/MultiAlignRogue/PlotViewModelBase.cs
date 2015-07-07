using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OxyPlot;

namespace MultiAlignRogue
{
    public class PlotViewModelBase : ViewModelBase
    {
        public List<OxyColor> Colors { get; set; }

        public PlotViewModelBase()
        {
            Colors = new List<OxyColor>();
            Colors.Add(OxyColors.Red);
            Colors.Add(OxyColors.Blue);
            Colors.Add(OxyColors.Purple);
            Colors.Add(OxyColors.Green);
            Colors.Add(OxyColors.Black);
            Colors.Add(OxyColors.Yellow);
            Colors.Add(OxyColors.Pink);
            Colors.Add(OxyColors.Brown);
            Colors.Add(OxyColors.Cyan);
            Colors.Add(OxyColors.Indigo);
            Colors.Add(OxyColors.Orange);
        }
    }
}
