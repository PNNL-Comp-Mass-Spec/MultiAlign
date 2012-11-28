using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Manassa.Data;

namespace Manassa.Workspace
{
    public class ManassaWorkspace: DependencyObject
    {

        public ManassaWorkspace()
        {
            RecentAnalysis = new ObservableCollection<RecentAnalysis>();
        }

        public ObservableCollection<RecentAnalysis> RecentAnalysis
        {
            get { return (ObservableCollection<RecentAnalysis>)GetValue(RecentAnalysisProperty); }
            set { SetValue(RecentAnalysisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecentAnalysis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecentAnalysisProperty =
            DependencyProperty.Register("RecentAnalysis", typeof(ObservableCollection<RecentAnalysis>), typeof(ManassaWorkspace)
            );


        public void AddAnalysis(RecentAnalysis analysis)
        {
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
