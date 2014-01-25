using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using MultiAlign.Data;
using MultiAlign.ViewModels;

namespace MultiAlign.Workspace
{
    public class MultiAlignWorkspace: ViewModelBase 
    {

        public MultiAlignWorkspace()
        {
            RecentAnalysis = new ObservableCollection<RecentAnalysisViewModel>();
        }

        public ObservableCollection<RecentAnalysisViewModel> RecentAnalysis
        {
            get;
            set;
        }


        public void AddAnalysis(RecentAnalysis recent)
        {
            RecentAnalysisViewModel analysis = new RecentAnalysisViewModel(recent);

            if (RecentAnalysis.Contains(analysis))
            {
                RecentAnalysis.Remove(analysis);
            }
            RecentAnalysis.Insert(0, analysis);

            if (RecentAnalysis.Count > 10)
            {
                RecentAnalysis.RemoveAt(RecentAnalysis.Count - 1);
            }
        }
    }
}
