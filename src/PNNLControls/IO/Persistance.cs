using System;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;

namespace IDLTools
{
    // create custom attribute to be assigned to class members
    [AttributeUsage(AttributeTargets.Class |
         AttributeTargets.Constructor |
         AttributeTargets.Field |
         AttributeTargets.Method |
         AttributeTargets.Property,
         AllowMultiple = true)]
    public class Persist : System.Attribute
    {
        public enum PersistanceType
        {
            PERSIST
        }
        // attribute constructor for 
        // positional parameters
        public Persist(PersistanceType type)
        {
            this.Type = type;
        }

        public Persist()
        {
        }


        // accessor
        public PersistanceType type
        {
            get
            {
                return Type;
            }
        }

        // private member data 
        private PersistanceType Type;
    }

	/// <summary>
	/// Summary description for Persistance.
	/// </summary>
	public class Persistance : MetaData 
	{

		public enum Direction
		{
			GetFromXML,
			SetToXML
		}

		public enum PersistType
		{
			Persist_Properties,
			Persist_UI
		}

		public Persistance()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public object TestCreate(string name)
		{
			object o=null;
			try
			{
				Type t = Type.GetType(name);
				// Create an instance of the "MyDynamicClass" class.
				o = Activator.CreateInstance(t);
			}
			catch{}

			return o;

		}

		public void PersistList(ref ArrayList list, Type elementType, MetaNode node, Direction dir)
		{
			try
			{
				int count = 0;

				if (dir==Direction.GetFromXML) 
				{
					
					count =(int) node.GetValue("count");
					list = new ArrayList();
					for (int i=0; i<count; i++)
					{
						object o = Activator.CreateInstance(elementType);
						list.Add(o);
					}
				}
				else
				{
					node.SetValue ("count", list.Count);
				}

				for (int i=0; i<list.Count; i++)
				{
					MetaNode child = node.OpenChild ("element", i);
					PersistProperties(list[i], child, dir);
				}

			}
			catch(Exception e) 
			{
				throw e;
			}
		}

		/********************************************************************
		 This routine saves or loads properties recursively.  Properties to be saved 
		 are marked with the "Persist" attribute.  Three types of properties are supported.
		 If the property returns a simple type, it is handled as such.  Array lists are 
		 assumed to be arrays of simple types.  Finally, if a property returns an object we
		 recursively process the properties of the object.
		 
		 direction is GetFromXML, SetToXML
		********************************************************************/
		public bool PersistProperties(Object obj,MetaNode node, Direction dir)
		{
			Type t = obj.GetType();
			bool retVal = false;

			foreach(PropertyInfo prop in obj.GetType().GetProperties())
			{
				object[] customAttributes =
					prop.GetCustomAttributes(typeof(Persist),true);

				if(customAttributes.Length > 0)
				{
					try
					{
						//try getting the property from the xml class
						if ( prop.CanWrite && dir==Direction.GetFromXML) 
						{
							Object currentValue=null;

							Type tt = prop.PropertyType;
                            
							if (tt==typeof(ArrayList))
							{
								currentValue = t.InvokeMember(
									prop.Name,
									//BindingFlags.DeclaredOnly | 
									BindingFlags.Public | BindingFlags.NonPublic | 
									BindingFlags.Instance | BindingFlags.GetProperty, null, obj, null);
								ArrayList a = currentValue as ArrayList;
								for (int i=0; i<a.Count; i++)
									a[i] = node.GetValue(prop.Name, i);
							}
							else
								currentValue = node.GetValue(prop.Name);

							// if there's a value returned,
							if ( currentValue != null )
							{
								// then assign the value to the property
								t.InvokeMember(
									prop.Name,
									BindingFlags.Default | BindingFlags.SetProperty,
									null,
									obj,
									new object [] {currentValue}
									);
								retVal=true;
							}
							else	//try getting properties within the object
							{
								currentValue = t.InvokeMember(
									prop.Name,
									//BindingFlags.DeclaredOnly | 
									BindingFlags.Public | BindingFlags.NonPublic | 
									BindingFlags.Instance | BindingFlags.GetProperty, null, obj, null);
								MetaNode child = node.OpenChild(prop.Name);
								retVal = PersistProperties(currentValue, child, dir);
							}
							
						} 
						//try setting the property to xml
						else if ( prop.CanRead && dir==Direction.SetToXML) 
						{
							Object currentValue = t.InvokeMember(
								prop.Name,
								//BindingFlags.DeclaredOnly | 
								BindingFlags.Public | BindingFlags.NonPublic | 
								BindingFlags.Instance | BindingFlags.GetProperty, null, obj, null);

							Type type = currentValue.GetType();
							if(type.IsValueType || type == typeof (string) || type==typeof(ArrayList))
								node.SetValue (prop.Name, currentValue);
							else	//try saving properties within the object
							{
								MetaNode child = node.OpenChild(prop.Name);
								PersistProperties(currentValue, child, dir);
							}

							retVal = true;
						} 
					}
					catch(Exception e) 
					{
						throw e;
					}
				}
			}

			//after all properties for the object have been processed, process the 
			//control set, if any.

			Control c = obj as Control;
			bool childrenSaved = false;
			if (c!=null)
				childrenSaved = ProcessControlSet(c, node, dir, PersistType.Persist_Properties);

			if (childrenSaved)
				retVal = true;

			return(retVal);
		}

		private bool ProcessControlSet(Control c, MetaNode node, Direction dir, PersistType pType)
		{ 
			bool retVal = false;
			try 
			{
				foreach (Control cntrl in c.Controls)
				{
					try
					{
						//only add children that are supported.
						MetaNode child = null;
						if (cntrl.Name == "") 
							child = node;
						else
							child = node.OpenChild(cntrl.Name);

						bool success = false;

						if (pType == PersistType.Persist_Properties)
							success = this.PersistProperties(cntrl,child, dir);
						else
							success = this.PersistUI(cntrl, child, dir);

						//if we didn't save or load from this node, remove the node.
						if (!success)
							node.RemoveChild(cntrl.Name);
						else
							retVal = true;
					}
					catch{}
				}
			}
			catch {}
			return (retVal);
		}

		public bool PersistUI(Control ctl,MetaNode node, Direction dir)
		{
			bool retVal = false;

			if (dir==Direction.GetFromXML)
				retVal=LoadContextUI(node, ctl);
			else
				retVal=SaveContextUI(node, ctl);

			return retVal;
		}

		public bool SaveContextUI(MetaNode node, Form frm)
		{ 			
			node.SetValue ("Left", frm.Left);
			node.SetValue ("Top", frm.Top);
			node.SetValue ("WindowState", frm.WindowState.ToString());
			node.SetValue ("Visible", frm.Visible);
			node.SetValue ("Width", frm.Size.Width);
			node.SetValue ("Height", frm.Size.Height);

			ProcessControlSet((Control) frm, node, Direction.SetToXML, PersistType.Persist_UI);

			return(true);
		}

		public bool LoadContextUI(MetaNode node, Control obj)
		{ 
			MetaNode child = null;
			bool retVal = true;

			try
			{
				string name = obj.GetType().Name;

				switch (name)
				{
					case "CheckedListBox":
						CheckedListBox cl = (CheckedListBox) obj;
						for (int i = 0; i < cl.Items.Count; i++)
						{
							cl.SetItemCheckState (i,(CheckState) node.GetValue ("Checked", i));
						}
						break;
					case "TextBox": 
						TextBox t = (TextBox) obj;
						t.Text = (string) node.GetValue ("Text");
						break;

					case "CheckBox": 
						CheckBox c = (CheckBox) obj;
						c.Checked = (bool) node.GetValue("Checked");
						break;

					case "ComboBox": 
						ComboBox cbo = (ComboBox) obj;
						cbo.SelectedIndex = (int) node.GetValue("SelectedIndex");
						break;

					case "numEdit": goto case "NumericUpDown";

					case "NumericUpDown": 
						NumericUpDown n = (NumericUpDown) obj;
						decimal val = (decimal) node.GetValue ("Value");
						if (val>n.Maximum) n.Maximum = val + (decimal) .01;
						if (val<n.Minimum) n.Minimum = val - (decimal) .01;
						n.Value = val;
						break;

					case "RadioButton": 
						RadioButton r = (RadioButton) obj;
						r.Checked = (bool) node.GetValue ("Checked");
						break;

					case "PictureBox": 
						PictureBox pic = (PictureBox) obj;
						child = node.OpenChild ("BackColor");
						int R = (int) child.GetValue ("R");
						int G = (int) child.GetValue ("G");
						int B = (int) child.GetValue ("B");
						pic.BackColor = System.Drawing.Color.FromArgb(R, G, B);
						break;

					case "ToolBar": 
						ToolBar tb = (ToolBar) obj;
						child = node.OpenChild(tb.Name);
						foreach (ToolBarButton b in tb.Buttons)
							b.Pushed = (bool) child.GetValue ("btn",tb.Buttons.IndexOf(b));
						break;

//					case "Panel": 
//						Panel p = (Panel) obj;
//						child = node.OpenChild ("Size");
//						p.Width = (int)child.GetValue("Width");
//						p.Height = (int)child.GetValue("Height");
//
//						ProcessControlSet((Control)p, node, Direction.GetFromXML, PersistType.Persist_UI);
//						break;

					default:  
						try
						{
							retVal = ProcessControlSet((Control)obj, node, Direction.GetFromXML, PersistType.Persist_UI);
						}
						catch (Exception e){MessageBox.Show(e.Message);}
						break;
				}
			}
			catch{retVal = false;}

			return (retVal);
		}

		public bool SaveContextUI(MetaNode node, Control obj)
		{ 			
			MetaNode child = null;

			switch (obj.GetType().Name)
			{
				case "CheckedListBox":
					CheckedListBox cl = (CheckedListBox) obj;
					for (int i = 0; i < cl.Items.Count; i++)
						node.SetValue ("Checked", cl.GetItemCheckState(i), i);
					break;
				case "TextBox": 
					TextBox t = (TextBox) obj;
					node.SetValue ("Text", t.Text);
					break;

				case "numEdit": goto case "NumericUpDown";

				case "NumericUpDown": 
					NumericUpDown n = (NumericUpDown) obj;
					node.SetValue ("Value", n.Value);
					break;

				case "RadioButton": 
					RadioButton r = (RadioButton) obj;
					node.SetValue ("Checked", r.Checked);
					break;

				case "CheckBox": 
					CheckBox c = (CheckBox) obj;
					node.SetValue ("Checked", c.Checked);
					break;

				case "ComboBox": 
					ComboBox cbo = (ComboBox) obj;
					node.SetValue ("SelectedIndex", cbo.SelectedIndex);
					break;

				case "PictureBox": 
					PictureBox pic = (PictureBox) obj;
					child = node.OpenChild ("BackColor");
					child.SetValue ("R", pic.BackColor.R);
					child.SetValue ("G", pic.BackColor.G);
					child.SetValue ("B", pic.BackColor.B);
					break;

				case "ToolBar": 
					ToolBar tb = (ToolBar) obj;
					foreach (ToolBarButton b in tb.Buttons)
						node.SetValue ("btn", b.Pushed, tb.Buttons.IndexOf(b));
					break;

//				case "Panel": 
//					Panel p = (Panel) obj;
//					child = node.OpenChild ("Size");
//					child.SetValue ("Width", p.Width);
//					child.SetValue ("Height", p.Height);
//
//					ProcessControlSet((Control) p, node, Direction.SetToXML, PersistType.Persist_UI);
//					break;

					
				default:
					// not a leaf node. try saving control list, if it has one
					ProcessControlSet(obj, node, Direction.SetToXML, PersistType.Persist_UI);
					break;
					
			}
			if (node.ChildCount()>0)
				return(true);
			else 
				return(false);
		}

		public bool LoadContextUI(MetaNode node, Form frm)
		{ 		
			bool visible = true;
			bool retVal = true;

			try
			{
				int Left = (int)node.GetValue("Left");
				int Top = (int)node.GetValue("Top");

				int Width = (int)node.GetValue("Width");
				int Height = (int)node.GetValue("Height");

				frm.Location = new System.Drawing.Point(Left, Top);
				frm.Size = new System.Drawing.Size(Width, Height);

				visible = (bool)node.GetValue("Visible");
			
				string state =  (string) node.GetValue("WindowState");

			switch (state)
				{
					case "Maximized":  frm.WindowState = FormWindowState.Maximized; break;
					case "Minimized":  frm.WindowState = FormWindowState.Minimized; break;
					case "Normal":  frm.WindowState = FormWindowState.Normal; break;
				}
			}
			catch{retVal = false;}

			ProcessControlSet((Control)frm, node, Direction.GetFromXML, PersistType.Persist_UI);

			if (visible)
				frm.Show();
			else
				frm.Hide();

			return (retVal);
		}
	}
}
