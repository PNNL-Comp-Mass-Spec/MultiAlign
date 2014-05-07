using System;
using System.Windows.Forms;
using System.Windows.Input;
using MultiAlign.Data;
using MultiAlign.Data.States;
using System.Windows.Shapes;

namespace MultiAlign.Commands.Viewers
{
    public class StartNewAnalysisCommand : ICommand
    {
        public event EventHandler StartNewAnalysis;
        
        public StartNewAnalysisCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            if (StartNewAnalysis != null)
            {
                StartNewAnalysis(this, null);
            }
        }
    }
}
