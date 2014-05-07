using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.Commands.Viewers;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.Workspace;

namespace MultiAlign.ViewModels.Viewers
{
    public class GettingStartedViewModel: ViewModelBase
    {
        private MultiAlignWorkspace                 m_currentWorkspace;
        private StateModeratorViewModel             m_moderator;        
        public event EventHandler<OpenAnalysisArgs> ExistingAnalysisSelected;
        public event EventHandler<OpenAnalysisArgs> NewAnalysisStarted;
        /// <summary>
        /// Path to the workspace
        /// </summary>
        private readonly string m_workspacePath;


        public GettingStartedViewModel(string workspacePath,  StateModeratorViewModel moderator)
        {
            m_moderator     = moderator;
            m_workspacePath = workspacePath;

            CreateCommands();
            LoadWorkspace(workspacePath);

            foreach (var item in CurrentWorkspace.RecentAnalysis)            
                item.RecentAnalysisSelected += item_RecentAnalysisSelected;
                       
        }

        private void CreateCommands()
        {
            Action removeRecentList = delegate
            {                
                RecentAnalyses.Clear();
                SaveWorkSpace();
            };

            ClearRecentList = new BaseCommand(removeRecentList);
            
            var command      = new LoadExistingAnalysisCommand();
            command.ExistingAnalysisSelected        += command_ExistingAnalysisSelected;
            LoadExistingAnalysis                    = command;
            
            var startNew = new BaseCommand(startNew_StartNewAnalysis, BaseCommand.AlwaysPass);            
            StartNewAnalysis                 = startNew;
        }

        /// <summary>
        /// Loads a current workspace.
        /// </summary>
        private void LoadWorkspace(string path)
        {
            if (path == null)
                return;

            if (System.IO.File.Exists(path))
            {
                ApplicationStatusMediator.SetStatus("Loading workspace");
                var reader = new MultiAlignWorkspaceReader();
                try
                {
                    CurrentWorkspace = reader.Read(path);
                }
                catch
                {
                    ApplicationStatusMediator.SetStatus(string.Format("Could not load the default workspace: {0}"));
                }
            }
            else
            {
               CurrentWorkspace = new MultiAlignWorkspace();
            }
        }

        private void SaveWorkSpace()
        {
            var writer = new MultiAlignWorkspaceWriter();
            try
            {
                writer.Write(m_workspacePath, m_currentWorkspace);
            }
            catch
            {
            }
        }

        public ObservableCollection<RecentAnalysisViewModel> RecentAnalyses
        {
            get
            {
                return m_currentWorkspace.RecentAnalysis;
            }            
        }

        void item_RecentAnalysisSelected(object sender, OpenAnalysisArgs e)
        {
            // Load ...            
            if (ExistingAnalysisSelected != null)
            {
                ExistingAnalysisSelected(sender, e);
            }
            CurrentWorkspace.AddAnalysis(e.AnalysisData);
            SaveWorkSpace();
        }

        void startNew_StartNewAnalysis()
        {            
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
            SaveWorkSpace();
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
        public ICommand ClearRecentList { get; private set; }           
        #endregion

        public  void AddAnalysis(RecentAnalysis recent)
        {
            CurrentWorkspace.AddAnalysis(recent);            
            SaveWorkSpace();
        }
    }
}
