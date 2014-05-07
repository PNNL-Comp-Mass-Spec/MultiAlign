using System;
using MultiAlign.Data;
using MultiAlign.ViewModels;

namespace MultiAlign.Commands.Viewers
{
    public sealed class LoadRecentAnalysisCommand: BaseCommand
    {
        public event EventHandler<OpenAnalysisArgs> RecentAnalysisSelected;

        public LoadRecentAnalysisCommand()
            : base(null, AlwaysPass)
        {
            
        }

        public override void Execute(object parameter)
        {
            var analysis = parameter as RecentAnalysisViewModel;
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
