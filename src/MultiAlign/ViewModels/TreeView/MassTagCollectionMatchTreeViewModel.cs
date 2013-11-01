using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.TreeView
{
    public class MassTagCollectionMatchTreeViewModel : TreeItemViewModel
    {
        ObservableCollection<MassTagMatchTreeViewModel> m_massTags = new ObservableCollection<MassTagMatchTreeViewModel>();

        public MassTagCollectionMatchTreeViewModel(List<ClusterToMassTagMap> matches): 
            this(matches, null)
        {
        }
        public MassTagCollectionMatchTreeViewModel(List<ClusterToMassTagMap> matches, TreeItemViewModel parent)
        {
            m_parent   = parent;
            m_massTags = new ObservableCollection<MassTagMatchTreeViewModel>();
        }

        public ObservableCollection<MassTagMatchTreeViewModel> MassTags
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
