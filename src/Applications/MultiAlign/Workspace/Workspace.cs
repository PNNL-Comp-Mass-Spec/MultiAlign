using System.Collections.ObjectModel;
using MultiAlign.Data;
using MultiAlign.ViewModels;

namespace MultiAlign.Workspace
{
    public class MultiAlignWorkspace : ViewModelBase
    {
        public MultiAlignWorkspace()
        {
            RecentAnalysis = new ObservableCollection<RecentAnalysisViewModel>();
        }

        public ObservableCollection<RecentAnalysisViewModel> RecentAnalysis { get; set; }


        public void AddAnalysis(RecentAnalysis recent)
        {
            var analysis = new RecentAnalysisViewModel(recent);
            RecentAnalysisViewModel model = null;

            foreach (var x in RecentAnalysis)
            {
                if (x.Analysis == recent)
                    model = x;
            }

            if (model != null)
            {
                RecentAnalysis.Remove(model);
            }

            RecentAnalysis.Insert(0, analysis);

            if (RecentAnalysis.Count > 10)
            {
                RecentAnalysis.RemoveAt(RecentAnalysis.Count - 1);
            }
        }
    }
}