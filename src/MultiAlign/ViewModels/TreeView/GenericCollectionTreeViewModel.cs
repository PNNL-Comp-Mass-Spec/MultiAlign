using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MultiAlign.ViewModels.TreeView
{

    public class GenericCollectionTreeViewModel: TreeItemViewModel
    {
        protected ObservableCollection<TreeItemViewModel> m_items;

        public GenericCollectionTreeViewModel()
        {
            m_items = new ObservableCollection<TreeItemViewModel>();
        }

        public GenericCollectionTreeViewModel(List<TreeItemViewModel> data):
            this()
        {                        
            data.ForEach(x => Items.Add(x));
        }


        public void AddStatistic(string name, double value)
        {
            StatisticTreeViewItem x = new StatisticTreeViewItem(value, name);
            m_items.Add(x);
        }
        public void AddStatistic(string name, double value, string format)
        {
            StatisticTreeViewItem x = new StatisticTreeViewItem(value, name, format);
            m_items.Add(x);
        }

        public void AddString(string name, string value)
        {
            StringTreeViewItem x = new StringTreeViewItem(value, name);
            m_items.Add(x);
        }


        public ObservableCollection<TreeItemViewModel> Items
        {
            get
            {
                return m_items;
            }
        }        

        public override void LoadChildren()
        {
        }
    }
}
