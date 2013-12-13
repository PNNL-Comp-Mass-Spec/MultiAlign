using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlign.ViewModels.TreeView
{
    public class StatisticTreeViewItem: TreeItemViewModel
    {

        public StatisticTreeViewItem(double value, string name)
        {
            Name  = name;
            Value = value;
        }

        public double Value 
        { 
            get; 
            private set; 
        }        

        public override void LoadChildren()
        {            
        }
    }
    public class StringTreeViewItem : TreeItemViewModel
    {

        public StringTreeViewItem(string value, string name)
        {
            Name  = name;
            Value = value;
        }

        public string Value
        {
            get;
            private set;
        }

        public override void LoadChildren()
        {
        }
    }
}
