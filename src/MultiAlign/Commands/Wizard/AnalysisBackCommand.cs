using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MultiAlign.ViewModels.Analysis;

namespace MultiAlign.Commands.Wizard
{
    class AnalysisBackCommand: ICommand
    {
        private AnalysisSetupViewModel m_model;

        public AnalysisBackCommand(AnalysisSetupViewModel model)
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
            m_model.MoveBack();
        }
    }
}
