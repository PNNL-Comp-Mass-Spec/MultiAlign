using System;
using System.Windows.Input;
using MultiAlign.Data;
using MultiAlign.Data.States;
using MultiAlign.ViewModels;

namespace MultiAlign.Commands.Viewers
{
    public class LoadRecentAnalysisCommand: ICommand
    {
        public event EventHandler<OpenAnalysisArgs> RecentAnalysisSelected;        
        public LoadRecentAnalysisCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            RecentAnalysisViewModel analysis = parameter as RecentAnalysisViewModel;
            if (analysis == null)
            {
                return;
            }

            if (RecentAnalysisSelected != null)
            {
                RecentAnalysisSelected(this, new OpenAnalysisArgs(analysis.Analysis));
            }
        }
    }
}
