using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Collections;

namespace MultiAlignCore.IO.Parameters
{

	/// <summary>
	/// Summary description for xmlIO.
	/// </summary>
	/// 

	public class MetaNode
	{
		protected XmlNode root = null;
		protected XmlDocument doc = null;

		public MetaNode ()
		{
		}

		public MetaNode (XmlNode node, XmlDocument d)
		{
			root = node;
			doc = d;
		}

        protected XmlNode OpenXmlNode(string id, string type, int index, bool isNode, bool createNewIfMissing)
        {
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                string name = root.ChildNodes[i].Name;
                if (root.ChildNodes[i].Name == id)
                {
                    if (index >= 0)
                    {
                        string strIndex = root.ChildNodes[i].Attributes["index"].Value;
                        if (strIndex.Equals(index.ToString()))
                            return (root.ChildNodes[i]);
                    }
                    else
                        return (root.ChildNodes[i]);
                }
            }

            if (!createNewIfMissing)
                return null;

            XmlNode newNode = doc.CreateNode(XmlNodeType.Element, id, "");
            root.AppendChild(newNode);
            if (!isNode)
                newNode.InnerText = "0";
            XmlNode attr = doc.CreateNode(XmlNodeType.Attribute, "type", "");
            attr.Value = type;
            newNode.Attributes.SetNamedItem(attr);

            if (index >= 0)
            {
                attr = doc.CreateNode(XmlNodeType.Attribute, "index", "");
                attr.Value = index.ToString();
                newNode.Attributes.SetNamedItem(attr);
            }

            return (newNode);
        }

		protected XmlNode OpenXmlNodeFromArray(string id, int index)
		{
			for (int i=0; i<root.ChildNodes.Count; i++)
			{
				string name = root.ChildNodes[i].Name;
				if(root.ChildNodes[i].Name == id && i == index)
				{					
					return (root.ChildNodes[i]);										
				}
			}

            return null;
		}

		protected void RemoveChild(string id, int index)
		{
			for (int i=0; i<root.ChildNodes.Count; i++)
			{
				if(root.ChildNodes[i].Name == id)
				{
					if (index >= 0)
					{
						string strIndex = root.ChildNodes [i].Attributes["index"].Value;
						if (strIndex.Equals (index.ToString()))
						{
							root.RemoveChild(root.ChildNodes[i]);
						}
					}
					else
						root.RemoveChild(root.ChildNodes[i]);
				}
			}
			return;
		}

		public void RemoveChild(string id)
		{
			RemoveChild(id, -1);
			return;
		}

		public int ChildCount()
		{
			return(root.ChildNodes.Count);
		}

        public MetaNode OpenChildFromArray(string id, int index)
        {
            XmlNode node = OpenXmlNodeFromArray(id, index);

            MetaNode retNode = new MetaNode(node, doc);
            return (retNode);
        }
		public MetaNode OpenChild(string id, string childType, int index)
		{
			XmlNode node = OpenXmlNode(id, childType, index, true, true);

			MetaNode retNode = new MetaNode(node, doc);
			return (retNode);
		}

		public MetaNode OpenChild(string id, int index, bool createIfMissing)
		{
            XmlNode node = OpenXmlNode(id, "node", index, true, createIfMissing);

            if (node == null)
                return null;

			MetaNode retNode = new MetaNode(node, doc);
			return (retNode);
		}

        public MetaNode OpenChild(string id)
        {
            return (OpenChild(id, -1, true));
        }

        public MetaNode OpenChild(string id, bool createIfMissing)
        {
            return (OpenChild(id, -1, createIfMissing));
        }

		

		private void SetValue(string id, string val, string type, int index)
		{
			try
			{
                XmlNode node = OpenXmlNode(id, type, index, false, true);
				node.InnerText = val;
			}
			catch{}
		}

		/*
		 * modified from:
		 * 		Reflecting Data to NET Classes: Part II - From XML Documents
		 * 															   On: March 07, 2002
		 * 		By: Tin Lam (tin@netismtoday.com)
		 * 		http://www.netismtoday.com
		*/
		/// <summary>
		///		This method will extract a value from the XML node. It first select the first
		///		xml element that match the xmlName specified in the first parameter. If there's
		///		no match, then it will select the first element that has the xmlName specified as
		///		its attribute. Then check to see which type it is, and parse/convert/box to that type.
		/// </summary>
		/// <param name="xmlName">the name of the xml element/attribute to match</param>
		/// <param name="proType">the string indicating which type the xmlName should be boxed to</param>
		/// <param name="xmlDoc">the xml node</param>
		/// <returns>the boxed object containing the value from the xml node</returns>
		public Object GetValue(string id, int index) 
		{
			string type = GetType (id, index);

			try 
			{
				type            = type.Replace("System.", "");
                string[] types  = type.Split(',');
                type            = types[0];

				// first select the first element of the name xmlName
				string xmlValue = GetString (id, index);

				// convert, box and return the value of the specific type
				if ( type == "Byte" )		return Byte.Parse( xmlValue.Trim() );
				if ( type == "Char" )		return Char.Parse( xmlValue.Trim() );
				if ( type == "Decimal" )		return Decimal.Parse( xmlValue.Trim() );
				if ( type == "Double" )		return Double.Parse( xmlValue.Trim() );
				if ( type == "Int16" )		return Int16.Parse( xmlValue.Trim() );
				if ( type == "Int32" )		return Int32.Parse( xmlValue.Trim() );
				if ( type == "Int64" )		return Int64.Parse( xmlValue.Trim() );
				if ( type == "SByte" )		return SByte.Parse( xmlValue.Trim() );
				if ( type == "Single" )		return Single.Parse( xmlValue.Trim() );
				if ( type == "UInt16" )		return UInt16.Parse( xmlValue.Trim() );
				if ( type == "UInt32" )		return UInt32.Parse( xmlValue.Trim() );
				if ( type == "UInt64" )		return UInt64.Parse( xmlValue.Trim() );
				if ( type == "DateTime" )	return DateTime.Parse( xmlValue.Trim() );				
				if ( type == "String" )		return xmlValue;
				
			
				if ( type == "Boolean" ) 
				{
					switch( xmlValue.Trim().ToLower() ) 
					{
						case "+":
						case "1":
						case "ok":
						case "right":
						case "on":
						case "true":
						case "t":
						case "yes":
						case "y":
							return true;
						default:
							return false;
					}
                }
                /// 
                /// Attempt to create an enumeration
                /// 
                try
                {
                    //type        = "MultiAlignEngine.Alignment.enmAlignmentType";
                    type = GetType(id, index);
                    Type t = System.Type.GetType(type);
                    if (t != null && t.IsEnum)
                    {
                        return Enum.Parse(t, xmlValue.Trim());
                    }
                    else if (t != null)
                    {
                        // It's not null, it was found, but is not a type we support.
                        return null;
                    }
                    else
                    {
                        // Slow but works, we need the fully qualified name for this to work.
                        foreach (AssemblyName name in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                        {
                            string aname = name.FullName;

                            Type xt = Type.GetType(type + "," + aname, false);
                            if (xt != null)
                            {
                                if (xt.IsEnum)
                                {
                                    return Enum.Parse(xt, xmlValue.Trim());
                                }
                            }
                        }
                        throw new Exception("Could not parse the Parameter file: " + type);
                    }                    
                }
                catch (Exception ex)
                {
                    throw ex;                    
                }
				return null;
			} 
			catch (Exception e)
			{
                System.Diagnostics.Trace.WriteLine("Exception in XMLIO Class::GetValue. " + e.Message);
				return null;
			}
		}

		public Object GetValue(string id) 
		{
			return (GetValue(id, -1));
		}


		public void SetValue(string id, Object obj, int index)
		{
			//string typeName = obj.GetType().AssemblyQualifiedName; //.Replace ("System.", "");
            string typeName = obj.GetType().FullName.Replace ("System.", "");

			if(obj.GetType()==typeof(ArrayList))
			{
				ArrayList a = obj as ArrayList;
				for (int i=0; i<a.Count; i++)
				{
					SetValue (id, a[i], i);
				}
			}
			else
				SetValue(id, obj.ToString(), typeName, index);
		}

		public void SetValue(string id, Object obj)
		{
			SetValue(id, obj, -1);
		}

		public string GetType(string id, int index)
		{
            XmlNode node = OpenXmlNode(id, "", index, false, true);

			if (node == null)
				return ("");
			else if (node.Attributes.Count != 0)
			{
				if (node.Attributes[0].Name == "type")
					return (node.Attributes[0].Value.ToString());
			}
			return ("");
		}

		private string GetString(string id, int index)
		{
            XmlNode node = OpenXmlNode(id, "", index, false, true);

			if (node == null)
				return ("");
			else
				return (node.InnerText);
		}

		private string GetString(string id)
		{
			return (GetString(id,-1));
		}
	}

	public class MetaData : MetaNode
	{
		private string m_name;

		public MetaData()
		{
			m_name = "MetaData";
			Init(m_name);
		}
		public MetaData(string name)
		{
			m_name = name;
			Init(name);
		}

		public void ReadFile(string fName)
		{
			try
			{
                using (StreamReader sr = File.OpenText(fName))
                {
                    String xStr = sr.ReadToEnd();
                    doc.LoadXml(xStr);
                    sr.Close();
                }

				root = doc;
                root = OpenXmlNode(m_name, "root", -1, true, true);//doc.LastChild;
			}
			catch (Exception ex)
            {
                throw ex;
            }
		}

		public void WriteFile(string fName)
		{
			try
			{

                using (XmlTextWriter writer = new XmlTextWriter(File.CreateText(fName)))
                {
                    writer.Formatting = Formatting.Indented;
                    doc.Save(writer);
                    writer.Close();
                }
			}
			catch{}
		}

		private void Init(string rootName)
		{
			try
			{
				doc = new XmlDocument();

				XmlNode dec = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
				doc.InsertAfter(dec, null);
				root = doc.CreateNode(XmlNodeType.Element, rootName, "");
				doc.InsertAfter(root, dec);
			}
			catch{}
		}
	}
}
