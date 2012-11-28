using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manassa.Data;
using Manassa.Workspace;
using MultiAlignCore.IO;
using System.Collections.ObjectModel;
using MultiAlignCore.IO.Parameters;

namespace Manassa.IO
{

    public class ManassaWorkspaceReader
    {
        public ManassaWorkspace Read(string path)
        {
            ManassaWorkspace workspace  = new ManassaWorkspace();
            MetaData data               = new MetaData("MultiAlignWorkspace");
            data.ReadFile(path);

            workspace.RecentAnalysis    = LoadAnalysis(data);

            return workspace;
        }

        private ObservableCollection<RecentAnalysis> LoadAnalysis(MetaData data)
        {
            ObservableCollection<RecentAnalysis> allAnalysis = new ObservableCollection<RecentAnalysis>();

            MetaNode parentNode = data.OpenChild("RecentAnalysis", false);
            int totalAnalysis   = parentNode.ChildCount();
            for (int i = 0; i < totalAnalysis; i++)
            {
                MetaNode node = parentNode.OpenChildFromArray("Analysis", i);
                RecentAnalysis analysis = new RecentAnalysis();
                string analysisName = node.GetValue("Name").ToString();
                string analysisPath = node.GetValue("Path").ToString();

                analysis.Name = analysisName;
                analysis.Path = analysisPath;
                allAnalysis.Add(analysis);
            }
            return allAnalysis;
        }
    }
}
