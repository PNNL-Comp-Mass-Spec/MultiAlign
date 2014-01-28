using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using MultiAlign.Data;
using MultiAlign.Data.States;
using MultiAlignCore.IO.Features;
using System.IO;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.Extensions;
using MultiAlign.ViewModels.TreeView;
using System.Threading.Tasks;

namespace MultiAlign.ViewModels.Wizard
{
    public class AnalysisLoadingViewModel: ViewModelBase 
    {
        /// <summary>
        /// Fired when an analysis has loaded.
        /// </summary>
        public event EventHandler<AnalysisStatusArgs> AnalysisLoaded;
        /// <summary>
        /// 
        /// </summary>
        private string m_status;
        /// <summary>
        /// 
        /// </summary>
        private StateModeratorViewModel m_stateModerator;
        /// <summary>
        /// Loads the analysis from disk.
        /// </summary>
        private Task m_loadingTask;

        public AnalysisLoadingViewModel()
        {
        }
        
        /// <summary>
        /// Updates the status.
        /// </summary>
        public string Status 
        {
            get
            {
                return m_status;
            }
            set
            {
                if (m_status != value)
                {
                    m_status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        /// <summary>
        /// Loads the analysis.
        /// </summary>
        /// <param name="recentAnalysis"></param>
        public void LoadAnalysis(RecentAnalysis recentAnalysis)
        {
            if (recentAnalysis == null)
            {
                OnStatus("Cannot open analysis file.");
                return;
            }


            Action loadAnalysis = delegate
            {
                string filename = Path.Combine(recentAnalysis.Path, recentAnalysis.Name);
                
                OnStatus("Gaining access to the analysis database...");
                FeatureDataAccessProviders providers    = DataAccessFactory.CreateDataAccessProviders(filename, false);            
                MultiAlignAnalysis analysis             = new MultiAlignAnalysis();                        
                analysis.MetaData.AnalysisPath          = recentAnalysis.Path;
                analysis.MetaData.AnalysisName          = recentAnalysis.Name;
                analysis.MetaData.AnalysisSetupInfo     = null; 
                analysis.DataProviders                  = providers;

                OnStatus("Detecting your clusters...");
                analysis.Clusters                       = providers.ClusterCache.FindAll();

                OnStatus("Updating your datasets...");
                analysis.MetaData.Datasets              = providers.DatasetCache.FindAll().ToObservableCollection();

                OnStatus("Securing mass tags...");
                MassTagDatabaseLoaderCache provider     = new MassTagDatabaseLoaderCache();
                provider.Provider                       = analysis.DataProviders.MassTags;
                analysis.MassTagDatabase                = provider.LoadDatabase();

                OnStatus("Analysis Loaded...");
                ThreadSafeDispatcher.Invoke(() =>
                {
                    if (AnalysisLoaded != null)
                    {
                        AnalysisLoaded(this, new AnalysisStatusArgs(analysis));
                    }
                });
            };


            m_loadingTask = new Task(loadAnalysis);
            m_loadingTask.Start();            
        }

        private void OnStatus(string message)
        {
            ThreadSafeDispatcher.Invoke(() =>
                {
                    ApplicationStatusMediator.SetStatus(message);
                    Status = message;
                });
        }
    }
}
