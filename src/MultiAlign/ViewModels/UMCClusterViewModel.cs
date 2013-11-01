﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels
{
    public class UMCClusterViewModel: ViewModelBase
    {
        private UMCClusterLightMatched m_cluster;
        private UMCLight m_feature;

        /// <summary>
        /// Gets or sets the UMC Feature Matched
        /// </summary>
        public UMCClusterLightMatched Cluster 
        {
            get
            {
                return m_cluster;
            }
            set
            {
                if (value != m_cluster)
                {
                    m_cluster = value;
                    OnPropertyChanged("UMCClusterLightMatched");
                }
            }
        }

        /// <summary>
        /// Gets or sets the UMC Feature
        /// </summary>
        public UMCLight UMCFeature
        {  
            get
            {
                return m_feature;   
            }
            set
            {
                if (value != m_feature)
                {
                    OnPropertyChanged("UMCFeature");
                }
            }
        }
    }
}
