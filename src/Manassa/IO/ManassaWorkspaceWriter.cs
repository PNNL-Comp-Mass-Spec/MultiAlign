using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manassa.Data;
using Manassa.Workspace;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Parameters;
using System.Collections.ObjectModel;

namespace Manassa.IO
{

    public class ManassaWorkspaceWriter
    {

        public void Write(string path, ManassaWorkspace workspace)
        {            
            MetaData data            = new MetaData("MultiAlignWorkspace");            
            WriteAnalysis(data, workspace.RecentAnalysis);
            data.WriteFile(path);
        }

        private void WriteAnalysis(MetaData data, ObservableCollection<RecentAnalysis> analysis)
        {
            ObservableCollection<RecentAnalysis> allAnalysis = new ObservableCollection<RecentAnalysis>();

            MetaNode parentNode = data.OpenChild("RecentAnalysis", true);            
            foreach(RecentAnalysis recent in analysis)
            {
                MetaNode node = parentNode.OpenChild("Analysis", true);
                node.SetValue("Name", recent.Name);
                node.SetValue("Path", recent.Path);
            }            
        }
    }
}
