using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MultiAlign.ViewModels.Analysis;

namespace MultiAlign.Commands.Wizard
{
    public class AnalysisCancelCommand: ICommand
    {
        private AnalysisSetupViewModel m_model;

        public AnalysisCancelCommand(AnalysisSetupViewModel model)
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
            m_model.Cancel();
        }
    }
}
