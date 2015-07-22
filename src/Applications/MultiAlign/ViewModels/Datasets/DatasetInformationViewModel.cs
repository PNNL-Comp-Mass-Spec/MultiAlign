using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MultiAlign.Commands.Datasets;
using MultiAlign.Commands.Plotting;
using MultiAlign.ViewModels.Plotting;
using MultiAlignCore.Data.MetaData;

namespace MultiAlign.ViewModels.Datasets
{
    using MultiAlign.Commands;

    public class DatasetInformationViewModel : ViewModelBase
    {
        private readonly DatasetInformation m_information;
        private bool m_expand;
        private bool m_isSelected;

        public DatasetInformationViewModel(DatasetInformation information)
        {
            m_information = information;
            var data = information.PlotData;
            PlotData = new ObservableCollection<PlotViewModel>();

            RequestRemovalCommand = new BaseCommand(
                () =>
                    {
                        if (RemovalRequested != null)
                        {
                            RemovalRequested(this, EventArgs.Empty);
                        }
                    });

            if (data != null)
            {
                PlotData.Add(new PlotViewModel(data.Alignment,
                    "Alignment",
                    new PictureDisplayCommand(data.Alignment, "Alignment" + information.DatasetName)));
                PlotData.Add(new PlotViewModel(data.Features,
                    "Features",
                    new FeatureDisplayCommand(information)));

                PlotData.Add(new PlotViewModel(data.MassErrorHistogram, "Mass Error Histogram"));
                PlotData.Add(new PlotViewModel(data.NetErrorHistogram, "NET Error Histogram"));
                PlotData.Add(new PlotViewModel(data.MassScanResidual, "Mass vs Scan Residuals"));
                PlotData.Add(new PlotViewModel(data.MassMzResidual, "Mass vs m/z Residuals"));
                PlotData.Add(new PlotViewModel(data.NetResiduals, "NET Residuals"));
            }


            ModifyDatasetCommand = new ShowDatasetDetailCommand();
        }

        public event EventHandler RemovalRequested;

        public BaseCommand RequestRemovalCommand { get; private set; }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (value == m_isSelected)
                    return;

                m_isSelected = value;
                OnPropertyChanged("IsSelected");

                if (Selected != null)
                    Selected(this, null);
            }
        }

        public DatasetInformation Dataset
        {
            get { return m_information; }
        }

        public int DatasetId
        {
            get { return m_information.DatasetId; }
            set
            {
                if (m_information != null)
                {
                    m_information.DatasetId = value;
                    OnPropertyChanged("DatasetId");
                }
            }
        }

        public string Name
        {
            get
            {
                var name = "";
                if (m_information != null)
                {
                    name = m_information.DatasetName;
                }
                return name;
            }
        }

        public string DisplayName
        {
            get
            {
                ///stupid WPF content __ http://stackoverflow.com/questions/7861699/can-not-see-underscore-in-wpf-content
                return Name.Replace("_", "__");
            }
        }

        public int Id
        {
            get
            {
                var id = 0;
                if (m_information != null)
                {
                    id = m_information.DatasetId;
                }
                return id;
            }
        }

        public ObservableCollection<PlotViewModel> PlotData { get; private set; }

        public bool ShouldExpand
        {
            get { return m_expand; }
            set
            {
                if (value != m_expand)
                {
                    m_expand = value;
                    OnPropertyChanged("ShouldExpand");
                }
            }
        }

        public ICommand ModifyDatasetCommand { get; set; }
        public event EventHandler Selected;
    }
}