using GalaSoft.MvvmLight;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignRogue.AMTMatching
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
                RaisePropertyChanged("MinimumXcorr");
            }
        }

        public double MinimumPmtScore
        {
            get { return m_options.MinimumPmtScore; }
            set
            {
                m_options.MinimumPmtScore = value;
                RaisePropertyChanged("MinimumPmtScore");
            }
        }

        public double MinimumPeptideProphetScore
        {
            get { return m_options.MinimumPeptideProphetScore; }
            set
            {
                m_options.MinimumPeptideProphetScore = value;
                RaisePropertyChanged("MinimumPeptideProphetScore");
            }
        }

        public double MinimumDiscriminant
        {
            get { return m_options.MinimumDiscriminant; }
            set
            {
                m_options.MinimumDiscriminant = value;
                RaisePropertyChanged("MinimumDiscriminant");
            }
        }


        public int MinimumObservationCountFilter
        {
            get { return m_options.MinimumObservationCountFilter; }
            set
            {
                m_options.MinimumObservationCountFilter = value;
                RaisePropertyChanged("MinimumObservationCountFilter");
            }
        }

        public bool OnlyLoadTagsWithDriftTime
        {
            get { return m_options.OnlyLoadTagsWithDriftTime; }
            set
            {
                m_options.OnlyLoadTagsWithDriftTime = value;
                RaisePropertyChanged("OnlyLoadTagsWithDriftTime");
            }
        }

        public string ExperimentExclusionFilter
        {
            get { return m_options.ExperimentExclusionFilter; }
            set
            {
                m_options.ExperimentExclusionFilter = value;
                RaisePropertyChanged("ExperimentExclusionFilter");
            }
        }


        public string ExperimentFilter
        {
            get { return m_options.ExperimentFilter; }
            set
            {
                m_options.ExperimentFilter = value;
                RaisePropertyChanged("ExperimentFilter");
            }
        }

        public double MinimumNet
        {
            get { return m_options.MinimumNet; }
            set
            {
                m_options.MinimumNet = value;
                RaisePropertyChanged("MinimumNet");
            }
        }

        public double MaximumNet
        {
            get { return m_options.MaximumNet; }
            set
            {
                m_options.MaximumNet = value;
                RaisePropertyChanged("MaximumNet");
            }
        }

        public double MinimumMass
        {
            get { return m_options.MinimumMass; }
            set
            {
                m_options.MinimumMass = value;
                RaisePropertyChanged("MinimumMass");
            }
        }

        public double MaximumMass
        {
            get { return m_options.MaximumMass; }
            set
            {
                m_options.MinimumMass = value;
                RaisePropertyChanged("MaximumMass");
            }
        }
    }
}