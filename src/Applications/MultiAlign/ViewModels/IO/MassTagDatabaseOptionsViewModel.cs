using MultiAlignCore.Algorithms.Options;

namespace MultiAlign.ViewModels.IO
{
    public class MassTagDatabaseOptionsViewModel : ViewModelBase
    {
        private readonly MassTagDatabaseOptions m_options;

        public MassTagDatabaseOptionsViewModel(MassTagDatabaseOptions options)
        {
            m_options = options;
        }


        public double MinimumXcorr
        {
            get { return m_options.MinimumXCorr; }
            set
            {
                m_options.MinimumXCorr = value;
                OnPropertyChanged("MinimumXcorr");
            }
        }

        public double MinimumPmtScore
        {
            get { return m_options.MinimumPmtScore; }
            set
            {
                m_options.MinimumPmtScore = value;
                OnPropertyChanged("MinimumPmtScore");
            }
        }

        public double MinimumPeptideProphetScore
        {
            get { return m_options.MinimumPeptideProphetScore; }
            set
            {
                m_options.MinimumPeptideProphetScore = value;
                OnPropertyChanged("MinimumPeptideProphetScore");
            }
        }

        public double MinimumDiscriminant
        {
            get { return m_options.MinimumDiscriminant; }
            set
            {
                m_options.MinimumDiscriminant = value;
                OnPropertyChanged("MinimumDiscriminant");
            }
        }


        public int MinimumObservationCountFilter
        {
            get { return m_options.MinimumObservationCountFilter; }
            set
            {
                m_options.MinimumObservationCountFilter = value;
                OnPropertyChanged("MinimumObservationCountFilter");
            }
        }

        public bool OnlyLoadTagsWithDriftTime
        {
            get { return m_options.OnlyLoadTagsWithDriftTime; }
            set
            {
                m_options.OnlyLoadTagsWithDriftTime = value;
                OnPropertyChanged("OnlyLoadTagsWithDriftTime");
            }
        }

        public string ExperimentExclusionFilter
        {
            get { return m_options.ExperimentExclusionFilter; }
            set
            {
                m_options.ExperimentExclusionFilter = value;
                OnPropertyChanged("ExperimentExclusionFilter");
            }
        }


        public string ExperimentFilter
        {
            get { return m_options.ExperimentFilter; }
            set
            {
                m_options.ExperimentFilter = value;
                OnPropertyChanged("ExperimentFilter");
            }
        }

        public double MinimumNet
        {
            get { return m_options.MinimumNet; }
            set
            {
                m_options.MinimumNet = value;
                OnPropertyChanged("MinimumNet");
            }
        }

        public double MaximumNet
        {
            get { return m_options.MaximumNet; }
            set
            {
                m_options.MaximumNet = value;
                OnPropertyChanged("MaximumNet");
            }
        }

        public double MinimumMass
        {
            get { return m_options.MinimumMass; }
            set
            {
                m_options.MinimumMass = value;
                OnPropertyChanged("MinimumMass");
            }
        }

        public double MaximumMass
        {
            get { return m_options.MaximumMass; }
            set
            {
                m_options.MinimumMass = value;
                OnPropertyChanged("MaximumMass");
            }
        }
    }
}