using OxyPlot;

namespace MultiAlign.ViewModels.Charting
{
    public class PlotModelBase : ViewModelBase
    {
        private PlotModel m_model;

        public PlotModelBase(string name)
        {
            Model = new PlotModel();
            Name = name;
        }

        public string Name { get; set; }
        public PlotModel Model
        {
            get { return m_model; }
            set
            {
                m_model = value;
                OnPropertyChanged("Model");
            }
        }
    }
}