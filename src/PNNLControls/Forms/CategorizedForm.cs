using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Provides support and helper methods to make implementing the 
	/// ICategorizedForm form interface.
	/// </summary>
	public class CategorizedForm : System.Windows.Forms.Form, ICategorizedItem
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private CategorizedItemInfo mCategorizedInfo = new CategorizedItemInfo();

		public CategorizedForm()
		{
			InitializeComponent();
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
			this.components = new System.ComponentModel.Container();
			this.Size = new System.Drawing.Size(300,300);
			this.Text = "CategorizedForm";
		}
		#endregion

		#region ICategorizedForm Members
		public CategorizedItemInfo CategorizedInfo
		{
			get
			{
				return mCategorizedInfo;
			}
		}

		public event System.EventHandler CategorizedInfoChanged;
		#endregion

		#region ICategorizedForm Helpers
		/// <summary>
		/// Gets or sets the category of the form.  Setting the category causes the 
		/// CategorizedInfoChanged event to be raised.
		/// </summary>
		protected CategoryInfo[] Category
		{
			get 
			{
				return this.CategorizedInfo.Category;
			}
			set 
			{
				this.mCategorizedInfo = new CategorizedItemInfo(mCategorizedInfo, value);
				this.OnCategorizedInfoChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or set the text of the categorized form.  Setting the text causes the 
		/// CategorizedInfoChanged event to be raised.
		/// </summary>
		protected String CategorizedText 
		{
			get 
			{
				return this.CategorizedInfo.Info.Text;
			}
			set 
			{
				this.mCategorizedInfo = new CategorizedItemInfo(mCategorizedInfo, value);
				this.OnCategorizedInfoChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets of sets the details associated with the categorized form.  Setting the details 
		/// causes the CategorizedInfoChanged event to be raised.
		/// </summary>
		protected DetailInfo[] Details 
		{
			get 
			{
				return this.CategorizedInfo.Details;
			}
			set 
			{
				this.mCategorizedInfo = new CategorizedItemInfo(mCategorizedInfo, value);
				this.OnCategorizedInfoChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the icon associated with the categorized form.  This is the image that will be 
		/// shown in ctlFileView - it does not need to be the same as the icon used for the form.
		/// Setting the icon causes the CategorizedInfoChanged event to be raised.
		/// </summary>
		protected Icon CategorizedIcon 
		{
			get 
			{
				return this.CategorizedInfo.Info.Icon;
			}
			set 
			{
				this.mCategorizedInfo = new CategorizedItemInfo(mCategorizedInfo, value);
				this.OnCategorizedInfoChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Raises the CategorizedInfoChanged event
		/// </summary>
		/// <param name="args"></param>
		protected void OnCategorizedInfoChanged(EventArgs args) 
		{
			if (this.CategorizedInfoChanged != null) 
			{
				this.CategorizedInfoChanged(this, args);
			}
		}
		#endregion
	}
}
