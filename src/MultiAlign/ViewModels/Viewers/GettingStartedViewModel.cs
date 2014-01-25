using System;
using System.Windows.Forms;
using System.Windows.Input;
using MultiAlign.Data;
using MultiAlign.Workspace;
using MultiAlign.Commands.Viewers;
using MultiAlign.Data.States;
using System.Collections.ObjectModel;

namespace MultiAlign.ViewModels
{
    public class GettingStartedViewModel: ViewModelBase
    {
        private MultiAlignWorkspace     m_currentWorkspace;
        private StateModeratorViewModel m_moderator;        
        public event EventHandler<OpenAnalysisArgs> ExistingAnalysisSelected;
        public event EventHandler<OpenAnalysisArgs> NewAnalysisStarted;

        public GettingStartedViewModel(MultiAlignWorkspace workspace, StateModeratorViewModel moderator)
        {
            m_moderator = moderator;

            CurrentWorkspace                         = workspace;
            LoadExistingAnalysisCommand command      = new LoadExistingAnalysisCommand();
            command.ExistingAnalysisSelected        += new EventHandler<OpenAnalysisArgs>(command_ExistingAnalysisSelected);
            LoadExistingAnalysis = command;
            
            StartNewAnalysisCommand startNew = new StartNewAnalysisCommand();
            startNew.StartNewAnalysis       += new EventHandler(startNew_StartNewAnalysis);
            StartNewAnalysis                 = startNew;

            foreach (var item in workspace.RecentAnalysis)
            {
                item.RecentAnalysisSelected += new EventHandler<OpenAnalysisArgs>(item_RecentAnalysisSelected);
            }

            RecentAnalyses = new ObservableCollection<RecentAnalysisViewModel>();
            foreach (var item in workspace.RecentAnalysis)
            {                
                RecentAnalyses.Add(item);
                item.RecentAnalysisSelected += new EventHandler<OpenAnalysisArgs>(item_RecentAnalysisSelected);
            }
        }

        public ObservableCollection<RecentAnalysisViewModel> RecentAnalyses
        {
            get;
            private set;
        }

        void item_RecentAnalysisSelected(object sender, OpenAnalysisArgs e)
        {
            // Load ...            
            if (ExistingAnalysisSelected != null)
            {
                ExistingAnalysisSelected(sender, e);
            }
            CurrentWorkspace.AddAnalysis(e.AnalysisData);
        }

        void startNew_StartNewAnalysis(object sender, EventArgs e)
        {
            // Start Wizard.
            if (NewAnalysisStarted != null)
            {
                NewAnalysisStarted(this, null);
            }
        }


        void command_ExistingAnalysisSelected(object sender, OpenAnalysisArgs e)
        {
            // Load ...
            if (ExistingAnalysisSelected != null)
            {
                ExistingAnalysisSelected(sender, e);
            }                
            CurrentWorkspace.AddAnalysis(e.AnalysisData);
        }

        public MultiAlignWorkspace CurrentWorkspace
        {
            get
            {
                return m_currentWorkspace;
            }
            set
            {
                if (m_currentWorkspace != value)
                {
                    m_currentWorkspace = value;
                    OnPropertyChanged("CurrentWorkspace");
                }
            }
        }
        

        #region Commands        
        public ICommand LoadExistingAnalysis { get; private set; }
        public ICommand StartNewAnalysis     { get; private set; }        
        #endregion

        

    }
}
