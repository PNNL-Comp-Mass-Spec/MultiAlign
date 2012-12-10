using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manassa.Data;
using Manassa.Workspace;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Parameters;
using System.Xml;
using System.Collections.ObjectModel;
using System.IO;

namespace Manassa.IO
{

    public class ManassaWorkspaceWriter
    {

        public void Write(string path, ManassaWorkspace workspace)
        {            
            XmlDocument document = new XmlDocument();

            XmlElement element   = document.CreateElement("MultiAlignWorkspace");
            document.AppendChild(element);

            XmlElement recent    = document.CreateElement("RecentAnalysis");
            element.AppendChild(recent);

            WriteRecent(document, recent, workspace.RecentAnalysis);

            using (XmlTextWriter writer = new XmlTextWriter(File.CreateText(path)))
            {
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
                writer.Close();
            }
        }

        private void WriteRecent(XmlDocument document,  XmlElement data, ObservableCollection<RecentAnalysis> analysis)
        {                            
            foreach(RecentAnalysis recent in analysis)
            {
                XmlElement element          = document.CreateElement("Analysis");
                XmlAttribute nameAttribute  = document.CreateAttribute("Name");
                nameAttribute.Value = recent.Name;
                XmlAttribute pathAttribute  = document.CreateAttribute("Path");
                pathAttribute.Value = recent.Path;
                element.Attributes.Append(nameAttribute);
                element.Attributes.Append(pathAttribute);
                data.AppendChild(element);
            }            
        }
    }
}
