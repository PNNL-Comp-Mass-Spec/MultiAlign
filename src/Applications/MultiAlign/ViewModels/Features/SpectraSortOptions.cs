using System;

namespace MultiAlign.ViewModels.Features
{
    public class SpectraSortOptions
    {
        public SpectraSortOptions(string name, Action action)
        {
            Action = action;
            Name = name;
        }

        public string Name { get; private set; }

        public Action Action { get; private set; }
    }
}