using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace IDLTools
{
	/// <summary>
	/// Summary description for frmProperties.
	/// </summary>
	public class frmProperties : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PropertyGrid props;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyGrid Props
		{
			get{return(this.props);}
		}
		public frmProperties()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.props = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// props
			// 
			this.props.CommandsVisibleIfAvailable = true;
			this.props.Dock = System.Windows.Forms.DockStyle.Fill;
			this.props.LargeButtons = false;
			this.props.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.props.Location = new System.Drawing.Point(0, 0);
			this.props.Name = "props";
			this.props.Size = new System.Drawing.Size(292, 273);
			this.props.TabIndex = 0;
			this.props.Text = "propertyGrid1";
			this.props.ViewBackColor = System.Drawing.SystemColors.Window;
			this.props.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// frmProperties
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.props);
			this.Name = "frmProperties";
			this.Text = "frmProperties";
			this.ResumeLayout(false);

		}
		#endregion

		public System.Windows.Forms.PropertyGrid Properties
		{
			get {return props;}
		}

#if DLL_LIBRARY
	//Does nothing
#else
	// Otherwise we want to build an application
		[STAThread]
		static void Main() 
		{
//			SerializeTest.frmMain f1 = new SerializeTest.frmMain();
//			object o = f1.txtB1;
//			MetaNode mn = new MetaNode(TextBox, Persistance.Direction.SetToXML);
//			Persistance pp = new Persistance();
//			MetaNode (XmlNode node, XmlDocument d)
//			pp.PersistProperties(o, mn, Persistance.Direction.SetToXML);
//			//PersistProperties(Object obj,MetaNode node, Direction dir)
//			//MetaData md = new MetaData();
//			//md.WriteFile("joey.xml");
//			Application.Run(new frmProperties());
		}
#endif 
	}
}
