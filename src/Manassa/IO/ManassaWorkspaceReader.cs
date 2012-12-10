using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manassa.Data;
using Manassa.Workspace;
using MultiAlignCore.IO;
using System.Collections.ObjectModel;
using MultiAlignCore.IO.Parameters;
using System.Xml;
using System.IO;

namespace Manassa.IO
{

    public class ManassaWorkspaceReader
    {
        public ManassaWorkspace Read(string path)
        {
            ManassaWorkspace workspace  = new ManassaWorkspace();
                        
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
                    analysis.Name = node.Attributes.GetNamedItem("Name").ToString();
                    analysis.Path = node.Attributes.GetNamedItem("Path").ToString();
                    allAnalysis.Add(analysis);
                }
            }
            return allAnalysis;
        }
    }
}
