using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
    public class UMCFilters
    {
        #region Members
        private UMCAbundanceMaxFilter m_abundanceMaxFilter;
        private UMCAbundanceSumFilter m_abundanceSumFilter;
        private UMCChargeStateFilter m_chargeStateFilter;
        private UMCDriftTimeFilter m_driftTimeFilter;
        private UMCMassCalibratedFilter m_massCalibratedFilter;
        private UMCMassFilter m_massFilter;
        private UMCMSFeatureCountFilter m_featureCountFilter;
        private UMCNETAlignedFilter m_netFilter;
        private UMCScanAlignedFilter m_scanAlignedFilter;
        private UMCScanFilter m_scanFilter;
        private UMCScoreFilter m_scoreFilter;
        #endregion

        public UMCFilters()
        {
            m_abundanceMaxFilter    = new UMCAbundanceMaxFilter();
            m_abundanceSumFilter    = new UMCAbundanceSumFilter();
            m_chargeStateFilter     = new UMCChargeStateFilter();
            m_driftTimeFilter       = new UMCDriftTimeFilter();
            m_featureCountFilter    = new UMCMSFeatureCountFilter();
            m_massCalibratedFilter  = new UMCMassCalibratedFilter();
            m_massFilter            = new UMCMassFilter();
            m_netFilter             = new UMCNETAlignedFilter();
            m_scanAlignedFilter     = new UMCScanAlignedFilter();
            m_scanFilter            = new UMCScanFilter();
            m_scoreFilter           = new UMCScoreFilter();
        }

        #region Properties
        public UMCAbundanceMaxFilter AbundanceMaxFilter
        {
            get { return m_abundanceMaxFilter; }
            set { m_abundanceMaxFilter = value; }
        }
        public UMCAbundanceSumFilter AbundanceSumFilter
        {
            get { return m_abundanceSumFilter; }
            set { m_abundanceSumFilter = value; }
        }
        public UMCChargeStateFilter ChargeStateFilter
        {
            get { return m_chargeStateFilter; }
            set { m_chargeStateFilter = value; }
        }
        public UMCDriftTimeFilter DriftTimeFilter
        {
            get { return m_driftTimeFilter; }
            set { m_driftTimeFilter = value; }
        }
        public UMCMassCalibratedFilter MassCalibratedFilter
        {
            get { return m_massCalibratedFilter; }
            set { m_massCalibratedFilter = value; }
        }
        public UMCMassFilter MassFilter
        {
            get { return m_massFilter; }
            set { m_massFilter = value; }
        }
        public UMCMSFeatureCountFilter FeatureCountFilter
        {
            get { return m_featureCountFilter; }
            set { m_featureCountFilter = value; }
        }
        public UMCNETAlignedFilter NetFilter
        {
            get { return m_netFilter; }
            set { m_netFilter = value; }
        }
        public UMCScanAlignedFilter ScanAlignedFilter
        {
            get { return m_scanAlignedFilter; }
            set { m_scanAlignedFilter = value; }
        }
        public UMCScanFilter ScanFilter
        {
            get { return m_scanFilter; }
            set { m_scanFilter = value; }
        }
        public UMCScoreFilter ScoreFilter
        {
            get { return m_scoreFilter; }
            set { m_scoreFilter = value; }
        }
        #endregion

        
        private void AddFilter(IFilter<clsUMC> filter, ref List<IFilter<clsUMC>> filters)
        {
            if (filter.Active)
                filters.Add(filter);
        }

        public List<IFilter<clsUMC>> GetFilterList()
        {
            List<IFilter<clsUMC>> filters = new List<IFilter<clsUMC>>();
            
            AddFilter(this.AbundanceMaxFilter, ref filters);
            AddFilter(this.AbundanceSumFilter, ref filters);
            AddFilter(this.ChargeStateFilter, ref filters);
            AddFilter(this.DriftTimeFilter, ref filters);
            AddFilter(this.FeatureCountFilter, ref filters);
            AddFilter(this.MassCalibratedFilter, ref filters);
            AddFilter(this.MassFilter, ref filters);
            AddFilter(this.NetFilter, ref filters);
            AddFilter(this.ScanAlignedFilter, ref filters);
            AddFilter(this.ScanFilter, ref filters);
            AddFilter(this.ScoreFilter, ref filters);
                        
            return filters;
        }  
    }

    #region Filter Classes
    /// <summary>
    /// Mass Filter
    /// </summary>
    public class UMCMassFilter : IFilter<clsUMC>
    {
        private double m_maximum;
        private double m_minimum;

        public UMCMassFilter()
        {
            m_maximum = 100000;
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
        public bool DoesPassFilter(clsUMC t)
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
    /// Mass Calibrated Filter
    /// </summary>
    public class UMCMassCalibratedFilter : IFilter<clsUMC>
    {
        private double m_maximum;
        private double m_minimum;

        public UMCMassCalibratedFilter()
        {
            m_maximum = 100000;
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(t.MassCalibrated);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Charge State Filter
    /// </summary>
    public class UMCChargeStateFilter : IFilter<clsUMC>
    {
        
        private int m_maximum;
        private int m_minimum;

        public UMCChargeStateFilter()
        {
            m_maximum = 45;
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(t.ChargeRepresentative);
        }
        #endregion
        private bool InRange(int value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Scan filter.
    /// </summary>
    public class UMCScanFilter : IFilter<clsUMC>
    {

        private int m_maximum;
        private int m_minimum;

        public UMCScanFilter()
        {
            m_maximum = 10000000;
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(t.Scan);
        }
        #endregion
        private bool InRange(int value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Scan aligned filter.
    /// </summary>
    public class UMCScanAlignedFilter : IFilter<clsUMC>
    {

        private int m_maximum;
        private int m_minimum;

        public UMCScanAlignedFilter()
        {
            m_maximum = 1000000;
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(t.ScanAligned);
        }
        #endregion
        private bool InRange(int value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// NET filter
    /// </summary>
    public class UMCNETAlignedFilter : IFilter<clsUMC>
    {

        private double m_maximum;
        private double m_minimum;

        public UMCNETAlignedFilter()
        {
            m_maximum = 1.0;
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
        public bool DoesPassFilter(clsUMC t)
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
    /// UMC Drift Time Filter
    /// </summary>
    public class UMCDriftTimeFilter : IFilter<clsUMC>
    {
        
        private double m_maximum;
        private double m_minimum;

        public UMCDriftTimeFilter()
        {
            m_maximum = 100000;
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(t.DriftTime);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// UMC MS Feature count filter.
    /// </summary>
    public class UMCMSFeatureCountFilter : IFilter<clsUMC>
    {
        private int m_maximum;
        private int m_minimum;

        public UMCMSFeatureCountFilter()
        {
            m_maximum = 10000;
            m_minimum = 1;
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(t.mshort_class_rep_charge);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Sum abundance filter.
    /// </summary>
    public class UMCAbundanceSumFilter : IFilter<clsUMC>
    {
        private double m_maximum;
        private double m_minimum;

        public UMCAbundanceSumFilter()
        {
            m_maximum = 1000000;
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(t.AbundanceSum);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// Max abundance filter.
    /// </summary>
    public class UMCAbundanceMaxFilter : IFilter<clsUMC>
    {
        private double m_maximum;
        private double m_minimum;

        public UMCAbundanceMaxFilter()
        {
            m_maximum = 10000000;
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(t.AbundanceMax);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    /// <summary>
    /// UMC Score Filter
    /// </summary>
    public class UMCScoreFilter : IFilter<clsUMC>
    {
        private double m_maximum;
        private double m_minimum;

        public UMCScoreFilter()
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
        public bool DoesPassFilter(clsUMC t)
        {
            return InRange(0);
        }
        #endregion
        private bool InRange(double value)
        {
            return value < m_maximum && value >= m_minimum;
        }
    }
    #endregion
}
