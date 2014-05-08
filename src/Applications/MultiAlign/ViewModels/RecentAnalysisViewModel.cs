using System;
using System.Windows.Input;
using MultiAlign.Commands.Viewers;
using MultiAlign.Data;

namespace MultiAlign.ViewModels
{
    public class RecentAnalysisViewModel
    {
        private readonly RecentAnalysis m_analysis;

        public RecentAnalysisViewModel(RecentAnalysis analysis)
        {
            m_analysis = analysis;
            var command = new LoadRecentAnalysisCommand();
            command.RecentAnalysisSelected += command_RecentAnalysisSelected;
            LoadRecent = command;
        }

        public string Name
        {
            get { return m_analysis.Name; }
        }

        public string Path
        {
            get { return m_analysis.Path; }
        }

        public RecentAnalysis Analysis
        {
            get { return m_analysis; }
        }

        public ICommand LoadRecent { get; private set; }
        public event EventHandler<OpenAnalysisArgs> RecentAnalysisSelected;

        private void command_RecentAnalysisSelected(object sender, OpenAnalysisArgs e)
        {
            if (RecentAnalysisSelected != null)
            {
                RecentAnalysisSelected(sender, e);
            }
        }
    }
}