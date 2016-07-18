using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using MultiAlign.ViewModels;
using MultiAlign.Workspace;

namespace MultiAlign.IO
{
    public class MultiAlignWorkspaceWriter
    {
        public void Write(string path, MultiAlignWorkspace workspace)
        {
            var document = new XmlDocument();

            var element = document.CreateElement("MultiAlignWorkspace");
            document.AppendChild(element);

            var recent = document.CreateElement("RecentAnalysis");
            element.AppendChild(recent);

            WriteRecent(document, recent, workspace.RecentAnalysis);

            using (var writer = new XmlTextWriter(File.CreateText(path)))
            {
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
                writer.Close();
            }
        }

        private void WriteRecent(XmlDocument document, XmlElement data,
            ObservableCollection<RecentAnalysisViewModel> analysis)
        {
            foreach (var recent in analysis)
            {
                var element = document.CreateElement("Analysis");
                var nameAttribute = document.CreateAttribute("Name");
                nameAttribute.Value = recent.Name;
                var pathAttribute = document.CreateAttribute("Path");
                pathAttribute.Value = recent.Path;
                element.Attributes.Append(nameAttribute);
                element.Attributes.Append(pathAttribute);
                data.AppendChild(element);
            }
        }
    }
}