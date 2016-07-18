using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignRogue
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using Xceed.Wpf.AvalonDock;
    using Xceed.Wpf.AvalonDock.Layout;
    using Xceed.Wpf.AvalonDock.Layout.Serialization;
    public class ViewSettingsSerializer : LayoutSerializer
    {
        public ViewSettingsSerializer(DockingManager manager) : base(manager)
    {
        }

        public void Serialize(XmlWriter writer)
        {
            new XmlSerializer(typeof(LayoutRoot)).Serialize(writer, (object)this.Manager.Layout);
        }

        public void Serialize(TextWriter writer)
        {
            new XmlSerializer(typeof(LayoutRoot)).Serialize(writer, (object)this.Manager.Layout);
        }

        public void Serialize(Stream stream)
        {
            new XmlSerializer(typeof(LayoutRoot)).Serialize(stream, (object)this.Manager.Layout);
        }

        public void Serialize(string filepath)
        {
            using (StreamWriter streamWriter = new StreamWriter(filepath))
                this.Serialize((TextWriter)streamWriter);
        }

        public void Deserialize(Stream stream)
        {
            try
            {
                this.StartDeserialization();
                LayoutRoot layout = new XmlSerializer(typeof(LayoutRoot)).Deserialize(stream) as LayoutRoot;
                this.FixupLayout(layout);
                this.Manager.Layout = layout;
            }
            finally
            {
                this.EndDeserialization();
            }
        }

        public void Deserialize(TextReader reader)
        {
            try
            {
                this.StartDeserialization();
                LayoutRoot layout = new XmlSerializer(typeof(LayoutRoot)).Deserialize(reader) as LayoutRoot;
                this.FixupLayout(layout);
                this.Manager.Layout = layout;
            }
            finally
            {
                this.EndDeserialization();
            }
        }

        public void Deserialize(XmlReader reader)
        {
            try
            {
                this.StartDeserialization();
                LayoutRoot layout = new XmlSerializer(typeof(LayoutRoot)).Deserialize(reader) as LayoutRoot;
                this.FixupLayout(layout);
                this.Manager.Layout = layout;
            }
            finally
            {
                this.EndDeserialization();
            }
        }

        public void Deserialize(string filepath)
        {
            using (StreamReader streamReader = new StreamReader(filepath))
                this.Deserialize((TextReader)streamReader);
        }
    }
}
