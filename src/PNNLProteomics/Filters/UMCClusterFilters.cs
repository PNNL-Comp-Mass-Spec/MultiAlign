using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
    public class UMCClusterFilters
    {
        private ClusterDriftTimeFilter      m_driftTimeFilter;
        private ClusterMassFilter           m_massFilter;
        private ClusterMemberCountFilter    m_memberFilter;
        private ClusterNETFilter            m_netFilter;
        private ClusterScoreMeanFilter      m_scoreMeanFilter;
        private ClusterScoreMedianFilter    m_scoreMedianFilter;
        
        public UMCClusterFilters()
        {
            m_driftTimeFilter   = new ClusterDriftTimeFilter();
            m_massFilter        = new ClusterMassFilter();
            m_memberFilter      = new ClusterMemberCountFilter();
            m_netFilter         = new ClusterNETFilter();
            m_scoreMeanFilter   = new ClusterScoreMeanFilter();
            m_scoreMedianFilter = new ClusterScoreMedianFilter();
        }

        public ClusterDriftTimeFilter DriftTime
        {
            get
            {
                return m_driftTimeFilter;
            }
            set
            {
                m_driftTimeFilter = value;
            }
        }
        public ClusterMassFilter Mass
        {
            get
            {
                return m_massFilter;
            }
            set
            {
                m_massFilter = value;
            }
        }
        public ClusterMemberCountFilter MemberCount
        {
            get
            {
                return m_memberFilter;
            }
            set
            {
                m_memberFilter = value;
            }
        }
        public ClusterNETFilter NET
        {
            get
            {
                return m_netFilter;
            }
            set
            {
                m_netFilter = value;
            }
        }
        public ClusterScoreMeanFilter ScoreMean
        {
            get
            {
                return m_scoreMeanFilter;
            }
            set
            {
                m_scoreMeanFilter = value;
            }
        }
        public ClusterScoreMedianFilter ScoreMedian
        {
            get
            {
                return m_scoreMedianFilter;
            }
            set
            {
                m_scoreMedianFilter = value;
            }
        }        

        private void AddFilter(IFilter<clsCluster> filter, ref List<IFilter<clsCluster>> filters)
        {
            if (filter.Active)
                filters.Add(filter);
        }

        public List<IFilter<clsCluster>> GetFilterList()
        {
            List<IFilter<clsCluster>> filters = new List<IFilter<clsCluster>>();
            
            AddFilter(DriftTime, ref filters);
            AddFilter(Mass, ref filters);
            AddFilter(MemberCount, ref filters);
            AddFilter(NET, ref filters);
            AddFilter(ScoreMean, ref filters);
            AddFilter(ScoreMedian, ref filters);
            
            return filters;
        }            
    }
    

    #region Filter Classes
    /// <summary>
    /// Mass Filter
    /// </summary>
    public class ClusterMassFilter : IFilter<clsCluster>
    {
        private double m_maximum;
        private double m_minimum;

        public ClusterMassFilter()
        {
            m_maximum = 100000;
            m_minimum = 0;
        }

        public double Minimum
        {
            get
            {
                return m_minimum;
            }
            set
            {
                m_minimum = value;
            }
        }

        public double Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                m_maximum = value;
            }
        }
        private bool m_boolActive = false;
        public bool Active
        {
            get
            {
                return m_boolActive;
            }
            set
            {
                m_boolActive = value;
            }
        }
        #region IFilter<clsCluster> Members
        public bool DoesPassFilter(clsCluster t)
        {
            return InRange(t.Mass);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// NET Filter
    /// </summary>
    public class ClusterNETFilter : IFilter<clsCluster>
    {
        private double m_maximum;
        private double m_minimum;

        public ClusterNETFilter()
        {
            m_maximum = 1;
            m_minimum = 0;
        }

        public double Minimum
        {
            get
            {
                return m_minimum;
            }
            set
            {
                m_minimum = value;
            }
        }

        public double Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                m_maximum = value;
            }
        }
        private bool m_boolActive = false;
        public bool Active
        {
            get
            {
                return m_boolActive;
            }
            set
            {
                m_boolActive = value;
            }
        }

        #region IFilter<clsUMC> Members
        public bool DoesPassFilter(clsCluster t)
        {
            return InRange(t.Net);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Drift Time Filter
    /// </summary>
    public class ClusterDriftTimeFilter : IFilter<clsCluster>
    {

        private double m_maximum;
        private double m_minimum;

        public ClusterDriftTimeFilter()
        {
            m_maximum = 10000;
            m_minimum = 0;
        }

        private bool m_boolActive = false;
        public bool Active
        {
            get
            {
                return m_boolActive;
            }
            set
            {
                m_boolActive = value;
            }
        }
        public double Minimum
        {
            get
            {
                return m_minimum;
            }
            set
            {
                m_minimum = value;
            }
        }

        public double Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                m_maximum = value;
            }
        }
        #region IFilter<clsCluster> Members
        public bool DoesPassFilter(clsCluster t)
        {
            return InRange(t.mdouble_driftTime);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Member count filter.
    /// </summary>
    public class ClusterMemberCountFilter : IFilter<clsCluster>
    {
        private int m_maximum;
        private int m_minimum;

        public ClusterMemberCountFilter()
        {
            m_maximum = 10000;
            m_minimum = 0;
        }

        private bool m_boolActive = false;
        public bool Active
        {
            get
            {
                return m_boolActive;
            }
            set
            {
                m_boolActive = value;
            }
        }
        public int Minimum
        {
            get
            {
                return m_minimum;
            }
            set
            {
                m_minimum = value;
            }
        }

        public int Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                m_maximum = value;
            }
        }
        #region IFilter<clsUMC> Members
        public bool DoesPassFilter(clsCluster t)
        {
            return InRange(t.mshort_num_dataset_members);
        }
        #endregion

        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Median score filter.
    /// </summary>
    public class ClusterScoreMedianFilter : IFilter<clsCluster>
    {
        private double m_maximum;
        private double m_minimum;

        public ClusterScoreMedianFilter()
        {
            m_maximum = 1;
            m_minimum = 0;
        }

        private bool m_boolActive = false;
        public bool Active
        {
            get
            {
                return m_boolActive;
            }
            set
            {
                m_boolActive = value;
            }
        }
        public double Minimum
        {
            get
            {
                return m_minimum;
            }
            set
            {
                m_minimum = value;
            }
        }

        public double Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                m_maximum = value;
            }
        }
        #region IFilter<clsUMC> Members
        public bool DoesPassFilter(clsCluster t)
        {
            return InRange(t.MedianScore);
        }
        #endregion

        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Median score filter.
    /// </summary>
    public class ClusterScoreMeanFilter : IFilter<clsCluster>
    {
        private double m_maximum;
        private double m_minimum;

        public ClusterScoreMeanFilter()
        {
            m_maximum = 1;
            m_minimum = 0;
        }

        private bool m_boolActive = false;
        public bool Active
        {
            get
            {
                return m_boolActive;
            }
            set
            {
                m_boolActive = value;
            }
        }
        public double Minimum
        {
            get
            {
                return m_minimum;
            }
            set
            {
                m_minimum = value;
            }
        }

        public double Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                m_maximum = value;
            }
        }
        #region IFilter<clsUMC> Members
        public bool DoesPassFilter(clsCluster t)
        {
            return InRange(t.MeanScore);
        }
        #endregion

        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    #endregion 
}
