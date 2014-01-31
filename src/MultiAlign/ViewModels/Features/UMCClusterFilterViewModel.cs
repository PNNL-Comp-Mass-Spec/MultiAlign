using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.Features;
using System.Collections.ObjectModel;
using MultiAlign.ViewModels.TreeView;
using MultiAlign.IO;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels
{
    public class UMCClusterFilterViewModel : ViewModelBase
    {
        List<UMCClusterLightMatched> m_clusters;

        public UMCClusterFilterViewModel(List<UMCClusterLightMatched> clusters)
        {
            m_clusters          = clusters;
            NetRange            = new RangeViewModel(new Range(0, 1),       "NET");
            MassRange           = new RangeViewModel(new Range(0, 4000),    "Mass");
            DriftRange          = new RangeViewModel(new Range(0, 600),     "Drift Time");
            
            TightnessRange      = new RangeViewModel(new Range(0, 10000), "Tightness");
            AmbiguityRange      = new RangeViewModel(new Range(0, 10000), "Ambiguity");
            TotalMembers        = new RangeViewModel(new Range(0, 10000), "Total Members");
            DatasetMembers      = new RangeViewModel(new Range(0, 10000), "Dataset Members");

            DatasetMembers.Minimum = 2;
            DatasetMembers.ShouldUse = true;


            MsMsTotal           = new RangeViewModel(new Range(0, 100), "Total MS/MS");
            IdentificationRange = new RangeViewModel(new Range(0, 100), "Identifications");
            AMTTagRange         = new RangeViewModel(new Range(0, 100), "AMT Tags");
        }

        public RangeViewModel AMTTagRange
        {
            get;
            private set;
        }
        
        public RangeViewModel TightnessRange
        {
            get;
            private set;
        }
        public RangeViewModel AmbiguityRange
        {
            get;
            private set;
        }

        public RangeViewModel MsMsTotal
        {
            get;
            private set;
        }
        public RangeViewModel IdentificationRange
        {
            get;
            private set;
        }
        public RangeViewModel TotalMembers
        {
            get;
            private set;
        }
        public RangeViewModel DatasetMembers
        {
            get;
            private set;
        }

        #region Mass NET Filters        
        public RangeViewModel NetRange
        {
            get;
            private set;
        }
        public RangeViewModel DriftRange
        {
            get;
            private set;
        }
        public RangeViewModel MassRange
        {
            get;
            private set;
        }
        #endregion


        public int Count
        {
            get
            {
                return m_clusters.Count;
            }
        }

        /// <summary>
        /// Filter string for clusters
        /// </summary>
        private string m_clusterIdFilter;
        /// <summary>
        /// Flag if the filter should be used
        /// </summary>
        private bool m_shouldUseClusterFilter;

        /// <summary>
        /// Filter for clusters.
        /// </summary>
        public string ClusterIdFilter
        {
            get
            {
                return m_clusterIdFilter;
            }
            set
            {
                if (value != m_clusterIdFilter)
                {
                    m_clusterIdFilter = value;
                    OnPropertyChanged("ClusterIdFilter");
                }
            }            
        }

        public bool ShouldUseClusterFilter
        {
            get
            {
                return m_shouldUseClusterFilter;
            }
            set
            {
                if (m_shouldUseClusterFilter != value)
                {
                    m_shouldUseClusterFilter = value;
                    OnPropertyChanged("ShouldUseDFilter");
               }
            }
        }
        public void Update()
        {

        }

        public IEnumerable<UMCClusterLightMatched> ApplyFilters()
        {
            IEnumerable<UMCClusterLightMatched> filtered = new List<UMCClusterLightMatched>(m_clusters);

            if (TotalMembers.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.MemberCount >= TotalMembers.Minimum && x.Cluster.MemberCount <= TotalMembers.Maximum);
            }
            if (DatasetMembers.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.DatasetMemberCount >= DatasetMembers.Minimum && x.Cluster.DatasetMemberCount <= DatasetMembers.Maximum);
            }
            if (TightnessRange.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.Tightness >= TightnessRange.Minimum && x.Cluster.Tightness <= TightnessRange.Maximum);
            }
            if (AmbiguityRange.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.AmbiguityScore >= AmbiguityRange.Minimum && x.Cluster.AmbiguityScore <= AmbiguityRange.Maximum);
            }

            if (ShouldUseClusterFilter)
            {
                // First map the cluster ID's from the text...who cares if some malformed.
                Dictionary<int, bool> hasClusters = new Dictionary<int, bool>();
                string[] data = m_clusterIdFilter.Replace("\r"," ").Replace("\n"," ").Split(' ');
                foreach (var id in data)
                {
                    int  clusterId  = 0;
                    bool worked     = int.TryParse(id, out clusterId);
                    if (worked)
                    {
                        if (!hasClusters.ContainsKey(clusterId))
                        {
                            hasClusters.Add(clusterId, true);
                        }                        
                    }
                }
                filtered = filtered.Where(x => hasClusters.ContainsKey(x.Cluster.ID));
            }

            /// MS/MS Identifications
            if (IdentificationRange.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.IdentifiedSpectraCount >= IdentificationRange.Minimum && x.Cluster.IdentifiedSpectraCount <= IdentificationRange.Maximum);
            }
            if (AMTTagRange.ShouldUse)
            {
                filtered = filtered.Where(x => x.ClusterMatches.Count >= AMTTagRange.Minimum && x.ClusterMatches.Count <= AMTTagRange.Maximum);
            }
            if (MsMsTotal.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.MsMsCount >= MsMsTotal.Minimum && x.Cluster.MsMsCount <= MsMsTotal.Maximum);
            }

            // Mass and NET Ranges
            if (MassRange.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.MassMonoisotopic >= MassRange.Minimum && x.Cluster.MassMonoisotopic <= MassRange.Maximum);
            }
            if (NetRange.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.RetentionTime >= NetRange.Minimum && x.Cluster.RetentionTime <= NetRange.Maximum);
            }
            if (DriftRange.ShouldUse)
            {
                filtered = filtered.Where(x => x.Cluster.DriftTime >= DriftRange.Minimum && x.Cluster.DriftTime <= DriftRange.Maximum);
            }            

            return filtered;
        }
    }

    public class RangeViewModel : ViewModelBase
    {
         
        private Range m_range;

        public RangeViewModel(Range range, string name)
        {
            m_range = range;
            Name    = name;
        }

        public Range  Range { get; set; }
        public double Minimum
        {
            get
            {
                return m_range.Minimum;
            }
            set
            {
                if (value != m_range.Minimum)
                {
                    m_range.Minimum = value;
                    OnPropertyChanged("Minimum");
                }
            }
        }
        public double Maximum
        {
            get
            {
                return m_range.Maximum;
            }
            set
            {
                if (value != m_range.Maximum)
                {
                    m_range.Maximum = value;
                    OnPropertyChanged("Maximum");
                }
            }
        }
        public bool   ShouldUse
        {
            get
            {
                return m_range.ShouldUse;
            }
            set
            {
                if (value != m_range.ShouldUse)
                {
                    m_range.ShouldUse = value;
                    OnPropertyChanged("ShouldUse");
                }
            }
        }
        public string Name
        {
            get;
            set;
        }
    }
    public class Range
    {
        public Range(double min, double max)
        {
            Minimum = min;
            Maximum = max;  
        }

        public double Minimum
        {
            get;
            set;
        }
        public double Maximum
        {
            get;
            set;
        }
        public bool ShouldUse
        {
            get;
            set;
        }
    }
}
