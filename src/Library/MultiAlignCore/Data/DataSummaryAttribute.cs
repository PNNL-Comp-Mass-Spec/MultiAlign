using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Data
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DataSummaryAttribute : Attribute
    {
        public DataSummaryAttribute(string name)
        {
            Name = name;            
        }
        
        public string Name
        {
            get;
            private set;
        }        
    }
}

