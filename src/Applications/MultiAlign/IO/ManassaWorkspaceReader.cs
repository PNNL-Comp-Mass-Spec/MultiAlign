using System.Collections.ObjectModel;
using System.Xml;
using MultiAlign.Data;
using MultiAlign.ViewModels;
using MultiAlign.Workspace;

namespace MultiAlign.IO
{
    public class MultiAlignWorkspaceReader
    {
        public MultiAlignWorkspace Read(string path)
        {
            var workspace = new MultiAlignWorkspace();

            var document = new XmlDocument();
            document.Load(path);

            XmlNode element = document.SelectSingleNode("MultiAlignWorkspace");
            XmlNode data = element.SelectSingleNode("RecentAnalysis");
            workspace.RecentAnalysis = LoadRecent(data);

            return workspace;
        }

        private ObservableCollection<RecentAnalysisViewModel> LoadRecent(XmlNode data)
        {
            var allAnalysis = new ObservableCollection<RecentAnalysisViewModel>();
            foreach (XmlNode node in data.ChildNodes)
            {
                if (node.Name == "Analysis")
                {
                    string name = node.Attributes["Name"].Value;
                    string path = node.Attributes["Path"].Value;

                    var analysis = new RecentAnalysis(path, name);
                    allAnalysis.Add(new RecentAnalysisViewModel(analysis));
                }
            }
            return allAnalysis;
        }
    }
}