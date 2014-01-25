using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MultiAlign.ViewModels.Analysis;
using MultiAlign.Data;

namespace MultiAlign.Commands.Wizard
{
    public class AnalysisNextCommand: ICommand
    {
        private AnalysisSetupViewModel m_model;
        public AnalysisNextCommand(AnalysisSetupViewModel model)
        {
            m_model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            m_model.MoveNext();
        }
    }
}
