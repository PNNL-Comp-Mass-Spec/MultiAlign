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

            XmlElement element = document.CreateElement("MultiAlignWorkspace");
            document.AppendChild(element);

            XmlElement recent = document.CreateElement("RecentAnalysis");
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
            foreach (RecentAnalysisViewModel recent in analysis)
            {
                XmlElement element = document.CreateElement("Analysis");
                XmlAttribute nameAttribute = document.CreateAttribute("Name");
                nameAttribute.Value = recent.Name;
                XmlAttribute pathAttribute = document.CreateAttribute("Path");
                pathAttribute.Value = recent.Path;
                element.Attributes.Append(nameAttribute);
                element.Attributes.Append(pathAttribute);
                data.AppendChild(element);
            }
        }
    }
}