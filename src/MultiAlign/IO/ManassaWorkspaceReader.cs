using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlign.Data;
using MultiAlign.Workspace;
using MultiAlignCore.IO;
using System.Collections.ObjectModel;
using MultiAlignCore.IO.Parameters;
using System.Xml;
using System.IO;
using MultiAlign.ViewModels;

namespace MultiAlign.IO
{

    public class MultiAlignWorkspaceReader
    {
        public MultiAlignWorkspace Read(string path)
        {
            MultiAlignWorkspace workspace  = new MultiAlignWorkspace();
                        
            XmlDocument document = new XmlDocument();
            document.Load(path);
                
            XmlNode element = document.SelectSingleNode("MultiAlignWorkspace");
            XmlNode data    = element.SelectSingleNode("RecentAnalysis");
            workspace.RecentAnalysis    = LoadRecent(data);
                    
            return workspace;
        }

        private ObservableCollection<RecentAnalysisViewModel> LoadRecent(XmlNode data)
        {
            ObservableCollection<RecentAnalysisViewModel> allAnalysis = new ObservableCollection<RecentAnalysisViewModel>();
            foreach (XmlNode node in data.ChildNodes)
            {
                if (node.Name == "Analysis")
                {
                    string name = node.Attributes["Name"].Value.ToString();
                    string path = node.Attributes["Path"].Value.ToString();

                    RecentAnalysis analysis = new RecentAnalysis(path, name);
                    allAnalysis.Add(new RecentAnalysisViewModel(analysis));
                }
            }
            return allAnalysis;
        }
    }
}
