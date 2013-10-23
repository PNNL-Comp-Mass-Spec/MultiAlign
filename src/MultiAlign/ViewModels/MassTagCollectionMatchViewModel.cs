using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels
{
    public class MassTagCollectionMatchViewModel : TreeItemViewModel
    {
        ObservableCollection<MassTagMatchViewModel> m_massTags = new ObservableCollection<MassTagMatchViewModel>();

        public MassTagCollectionMatchViewModel(List<ClusterToMassTagMap> matches): 
            this(matches, null)
        {
        }
        public MassTagCollectionMatchViewModel(List<ClusterToMassTagMap> matches, TreeItemViewModel parent)
        {
            m_parent   = parent;
            m_massTags = new ObservableCollection<MassTagMatchViewModel>();
        }

        public ObservableCollection<MassTagMatchViewModel> MassTags
        {
            get
            {
                return m_massTags;
            }
        }

        public override void LoadChildren()
        {
        }
    }
}
