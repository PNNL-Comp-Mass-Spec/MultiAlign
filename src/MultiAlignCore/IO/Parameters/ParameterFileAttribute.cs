using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.IO.Parameters
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ParameterFileAttribute : Attribute
    {

        public ParameterFileAttribute(string name, string groupName, string fullDescription)
        {
            Name = name;
            GroupName = groupName;
            FullDecription = fullDescription;
        }
        public ParameterFileAttribute(string name, string groupName)
        {
            Name     = name;
            GroupName       = groupName;
            FullDecription  = "";
        }
        
        public string FullDecription 
        { 
            get; 
            private set; 
        }
        public string Name 
        { 
            get; 
            private set; 
        }
        public string GroupName 
        { 
            get; 
            private set; 
        }
    }
}
