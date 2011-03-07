using System;
using System.Collections.Generic;
using System.Text;

using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;

namespace PNNLProteomics.IO
{
    /// <summary>
    /// Holds all feature access providers required by a MA analysis.
    /// </summary>
    public sealed class FeatureDataAccessProviders
    {
        private IUmcDAO         m_featureCache;
        private IUmcClusterDAO  m_clusterCache;

        public FeatureDataAccessProviders(IUmcDAO        featureCache,
                                          IUmcClusterDAO clusterCache)
        {
            m_clusterCache = clusterCache;
            m_featureCache = featureCache;
        }

        public IUmcDAO FeatureCache
        {
            get
            {
                return m_featureCache;
            }
            set
            {
                m_featureCache = value;
            }
        }
        public IUmcClusterDAO ClusterCache
        {
            get
            {
                return m_clusterCache;
            }
            set
            {
               m_clusterCache = value;
            }
        }
    }
}
