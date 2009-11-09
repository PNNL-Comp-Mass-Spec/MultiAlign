using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PNNLControls
{
	public class ctlHLabelLevelProperties : PNNLControls.ctlLabelItem
	{
		private System.ComponentModel.IContainer components = null;

		public ctlHLabelLevelProperties()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		/// <summary>
		/// Minimum Height in pixels
		/// if mForceDraw = true this property is enforced
		/// </summary>
		/// 
		int mMinHeight = int.MinValue;
		[System.ComponentModel.DefaultValue(int.MinValue)]
		[System.ComponentModel.Description("Minimum height for a label on this level")]
		public int MinHeight 
		{
			get{return this.mMinHeight;}
			set{this.mMinHeight = value;}
		}

		/// <summary>
		/// Minimum Height in pixels
		/// if mForceDraw = true this property is enforced
		/// </summary>
		/// 
		int mMinWidth = int.MinValue;
		[System.ComponentModel.DefaultValue(int.MinValue)]
		[System.ComponentModel.Description("Minimum width for a label on this level")]
		public int MinWidth 
		{
			get{return this.mMinWidth;}
			set{this.mMinWidth = value;}
		}

		/// <summary>
		/// Level in Hierarchy, leaf level being 0
		/// </summary>
		/// 
		bool mForceDraw = false;
		[System.ComponentModel.DefaultValue(false)]
		[System.ComponentModel.Description("Force this level to be drawn at minimum")]
		public bool ForceDraw 
		{
			get{return this.mForceDraw;}
			set{this.mForceDraw = value;}
		}
	}
}

