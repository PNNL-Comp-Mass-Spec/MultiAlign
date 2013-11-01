using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MultiAlign.IO;
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;

namespace MultiAlign.ViewModels.TreeView
{

    /// <summary>
    /// Feature tree view model.
    /// </summary>
    public class MsMsTreeViewModel : TreeItemViewModel
    {
        private MSSpectra m_feature;
        

        public MsMsTreeViewModel(MSSpectra feature)
            : this(feature, null)
        {
        }


        public MsMsTreeViewModel(MSSpectra feature, TreeItemViewModel parent)
        {
            m_feature = feature;
            m_parent  = parent;                        
        }


        public string Id
        {
            get
            {
                return m_feature.ID.ToString();
            }
        }

        public double PrecursorMz
        {
            get
            {
                return m_feature.PrecursorMZ;
            }
        }
        public string Net
        {
            get
            {
                return m_feature.Peptides.ToString();
            }
        }
        public int ChargeState
        {
            get
            {
                return m_feature.ParentFeature.ChargeState;
            }
        }
        
        public override void LoadChildren()
        {
                      
        }
    }
}
