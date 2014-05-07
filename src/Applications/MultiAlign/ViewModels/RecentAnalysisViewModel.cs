using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlign.Data;
using System.Windows.Input;
using MultiAlign.Commands.Viewers;

namespace MultiAlign.ViewModels
{
    public class RecentAnalysisViewModel
    {
        public event EventHandler<OpenAnalysisArgs> RecentAnalysisSelected;

        RecentAnalysis m_analysis;

        public RecentAnalysisViewModel(RecentAnalysis analysis)
        {
            m_analysis                        = analysis;
            LoadRecentAnalysisCommand command = new LoadRecentAnalysisCommand();
            command.RecentAnalysisSelected   += new EventHandler<OpenAnalysisArgs>(command_RecentAnalysisSelected);
            LoadRecent                        = command;
        }

        void command_RecentAnalysisSelected(object sender, OpenAnalysisArgs e)
        {
            if (RecentAnalysisSelected != null)
            {
                RecentAnalysisSelected(sender, e);
            }
        }

        public string Name
        {
            get
            {
                return m_analysis.Name;
            }
        }

        public string Path
        {
            get
            {
                return m_analysis.Path;
            }
        }

        public RecentAnalysis Analysis { get { return m_analysis; } }

        public ICommand LoadRecent
        {
            get;
            private set;
        }
    }
}
