using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlign.ViewModels
{
    public class SpectraSortOptions
    {
        public SpectraSortOptions(string name, Action action)
        {
            Action = action;
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public Action Action
        {
            get;
            private set;
        }
    }
}
