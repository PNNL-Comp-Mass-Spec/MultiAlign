using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureFinding;
using System.Windows.Input;

namespace MultiAlign.ViewModels.Analysis
{
    public class AnalysisOptionsViewModel: ViewModelBase
    {
        private double m_massResolution;
        private AnalysisOptions m_options;
        private double m_netTolerance;
        private double m_drifTimeTolerance;
        private double m_fragmentationTolerance;
        private bool m_isIonMobility;
        private bool m_hasMsMsFragmentation;
        private FeatureAlignmentType m_alignmentType;
        private ClusteringAlgorithmType m_clusteringType;
        private FeatureFinderType m_featureFindingType;
        private long m_minimumAbundance;
        private int m_minimumFeatureLength;
        private double m_deisotopingFitScore;

        public AnalysisOptionsViewModel(AnalysisOptions options)
        {
            m_options = options;
        }


        #region Experiment Type
        public bool IsIonMobility
        {
            get
            {
                return m_isIonMobility;
            }
            set
            {
                if (m_isIonMobility != value)
                {
                    m_isIonMobility = value;
                    OnPropertyChanged("IsIonMobility");
                }
            }
        }
        public bool HasMsMsFragmentation
        {
            get
            {
                return m_hasMsMsFragmentation;
            }
            set
            {
                if (m_hasMsMsFragmentation != value)
                {
                    m_hasMsMsFragmentation = value;
                    OnPropertyChanged("HasMsMsFragmentation");
                }
            }
        }
        #endregion

        #region Instrument Tolerances
        public double MassResolution 
        {
            get
            {
                return m_massResolution;
            }
            set
            {
                if ( m_massResolution  != value)
                {
                    m_massResolution = value;
                    OnPropertyChanged("MassResolution");
                }
            }
        }
        public double NETTolerance
        {
            get
            {
                return m_netTolerance;
            }
            set
            {
                if (m_netTolerance != value)
                {
                    m_netTolerance = value;
                    OnPropertyChanged("NETTolerance");
                }
            }
        }
        public double DriftTimeTolerance
        {
            get
            {
                return m_drifTimeTolerance;
            }
            set
            {
                if (m_drifTimeTolerance != value)
                {
                    m_drifTimeTolerance = value;
                    OnPropertyChanged("DriftTimeTolerance");
                }
            }
        }
        public double FragmentationTolerance
        {
            get
            {
                return m_fragmentationTolerance;
            }
            set
            {
                if (m_fragmentationTolerance != value)
                {
                    m_fragmentationTolerance = value;
                    OnPropertyChanged("FragmentationTolerance");
                }
            }
        }
        #endregion

        #region Algorithms
        public ClusteringAlgorithmType ClusteringAlgorithm
        {
            get
            {
                return m_clusteringType;
            }
            set
            {
                if (m_clusteringType != value)
                {
                    m_clusteringType = value;
                    OnPropertyChanged("ClusteringAlgorithm");
                }
            }
        }
        public FeatureAlignmentType AlignmentAlgorithm
        {
            get
            {
                return m_alignmentType;
            }
            set
            {
                if (m_alignmentType != value)
                {
                    m_alignmentType = value;
                    OnPropertyChanged("AlignmentAlgorithm");
                }
            }
        }
        public FeatureFinderType FeatureFindingAlgorithm
        {
            get
            {
                return m_featureFindingType;
            }
            set
            {
                if (m_featureFindingType != value)
                {
                    m_featureFindingType = value;
                    OnPropertyChanged("FeatureFindingAlgorithm");
                }
            }
        }
        #endregion


        #region Feature Definition Algorithm Parameters
        public int MinimumFeatureLength 
        {
            get
            {
                return m_minimumFeatureLength;
            }
            set
            {
                if (m_minimumFeatureLength != value)
                {
                    m_minimumFeatureLength = value;
                    OnPropertyChanged("MinimumFeatureLength");
                }
            }
        }
        public double DeisotopingFitScore 
        {
            get
            {
                return m_deisotopingFitScore;
            }
            set
            {
                if (m_deisotopingFitScore != value)
                {
                    m_deisotopingFitScore = value;
                    OnPropertyChanged("DeisotopingFitScore");
                }
            }
        }
        public long MinimumAbundance
        {
            get
            {
                return m_minimumAbundance;
            }
            set
            {
                if (m_minimumAbundance != value)
                {
                    m_minimumAbundance = value;
                    OnPropertyChanged("MinimumAbundance");
                }
            }
        }
        #endregion


        #region Commands 
        public ICommand AdvancedWindow { get; set; }
        #endregion
    }
}
