namespace MultiAlign.ViewModels.Datasets
{
    public class DatasetResolveMatchViewModel: ViewModelBase
    {
        private bool m_selected;
        
        public DatasetResolveMatchViewModel(DatasetInformationViewModel model, string newPath)
        {
            NewPath         = newPath;
            Dataset         = model;
            IsSelected      = true;
            
        }

        public DatasetInformationViewModel Dataset { get; private set; }

        public string NewPath
        {
            get; private set; 
        }


        public bool IsSelected
        {
            get { return m_selected; }
            set
            {
                if (m_selected == value)
                    return;

                m_selected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }
}