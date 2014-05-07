using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MultiAlign.ViewModels.TreeView
{
    public class StatisticTreeViewItem: TreeItemViewModel
    {
        public StatisticTreeViewItem(double value, string name)
            : this(value, name, "")
        {

        }
        public StatisticTreeViewItem(double value, string name, string format)
        {
            Name   = name;
            Value  = value;
            Format = format;    
        }
        public string Format
        {
            get;
            private set;
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
            Name = name;
            Value = value;
        }

        public StringTreeViewItem(string value, string name, string format)
        {
            Name = name;
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
