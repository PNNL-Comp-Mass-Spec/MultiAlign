namespace MultiAlignRogue.Utils
{
    using System.Collections.Generic;

    using GalaSoft.MvvmLight;

    using OxyPlot;

    public class PlotViewModelBase : ViewModelBase
    {
        public List<OxyColor> Colors { get; set; }

        public PlotViewModelBase()
        {
            this.Colors = new List<OxyColor>();
            this.Colors.Add(OxyColors.Red);
            this.Colors.Add(OxyColors.Blue);
            this.Colors.Add(OxyColors.Purple);
            this.Colors.Add(OxyColors.Green);
            this.Colors.Add(OxyColors.Black);
            this.Colors.Add(OxyColors.Yellow);
            this.Colors.Add(OxyColors.Pink);
            this.Colors.Add(OxyColors.Brown);
            this.Colors.Add(OxyColors.Cyan);
            this.Colors.Add(OxyColors.Indigo);
            this.Colors.Add(OxyColors.Orange);
        }
    }
}
