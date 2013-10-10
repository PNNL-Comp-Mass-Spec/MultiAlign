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

        private ObservableCollection<RecentAnalysis> LoadRecent(XmlNode data)
        {
            ObservableCollection<RecentAnalysis> allAnalysis = new ObservableCollection<RecentAnalysis>();
            foreach (XmlNode node in data.ChildNodes)
            {
                if (node.Name == "Analysis")
                {
                    RecentAnalysis analysis = new RecentAnalysis();
                    analysis.Name = node.Attributes["Name"].Value.ToString();
                    analysis.Path = node.Attributes["Path"].Value.ToString();
                    allAnalysis.Add(analysis);
                }
            }
            return allAnalysis;
        }
    }
}
