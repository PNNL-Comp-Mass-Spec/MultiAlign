using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using System.Collections.ObjectModel;

namespace MultiAlign.ViewModels
{

    /// <summary>
    /// 
    /// </summary>
    public class MassTagMatchViewModel : TreeItemViewModel
    {
        private ClusterToMassTagMap m_match;

        public MassTagMatchViewModel(ClusterToMassTagMap match)
            : this(match, null)
        {

        }

        public MassTagMatchViewModel(ClusterToMassTagMap match, UMCClusterViewModel parent)
        {
            m_match = match;
            m_parent = parent;
        }

        public string Name
        {
            get
            {
                return m_match.MassTag.MassTag.PeptideSequence;
            }
        }
        public double Stac
        {
            get
            {
                return m_match.StacScore;
            }
        }
        public double StacUp
        {
            get
            {
                return m_match.StacUP;
            }
        }


        public override void LoadChildren()
        {

        }
    }
}
