using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace PNNLControls
{
	/// <summary>
	/// Defines the methods necessary to find out about categorized items.
	/// </summary>
	public interface ICategorizedItem
	{
		/// <summary>
		/// Gets the categorized information for a form.
		/// </summary>
		CategorizedItemInfo CategorizedInfo 
		{
			get;
		}

		/// <summary>
		/// Informs listeners that the CategorizedInfo for this form has changed.
		/// </summary>
		event EventHandler CategorizedInfoChanged;
	}

	/// <summary>
	/// Base class for various levels of categorization.
	/// </summary>
	public class CategorizedInfo 
	{
		private String mText;
		private Icon mIcon;

		public CategorizedInfo(String text) 
		{
			if (text == null) 
			{
				throw new ArgumentNullException(text);
			}
			mText = text;
		}

		public CategorizedInfo(String text, Icon icon) : this(text)
		{
			mIcon = icon;
		}

		public override int GetHashCode()
		{
			return this.Text.GetHashCode();
		}


		public override bool Equals(object obj)
		{
			CategoryInfo ci = obj as CategoryInfo;
			if (ci == null) 
			{
				return false;
			}

			bool equals = this.Text.Equals(ci.Text);
			// Category are compared without regard to icons, so two categories with the 
			// same name and different icons at the same level will be merged.
			//			if (this.Icon != null) 
			//			{
			//				equals = equals & this.Icon.Equals(ci.Icon);
			//			}
			return equals;
		}


		/// <summary>
		/// Gets the text associated with the info
		/// </summary>
		public String Text 
		{
			get 
			{
				return mText;
			}
		}

		/// <summary>
		/// Gets the icon associated with the category.  Can be null.
		/// </summary>
		public Icon Icon 
		{
			get 
			{
				return mIcon;
			}
		}
	}

	/// <summary>
	/// Extension of CategorizedInfo specifically for categories.  Adds nothing.
	/// </summary>
	public class CategoryInfo : CategorizedInfo 
	{
		public CategoryInfo(String name) : base(name) {}
		public CategoryInfo(String name, Icon icon) : base(name, icon) {}
	}

	/// <summary>
	/// Extension of CategorizedInfo specifically for details.  Adds nothing.
	/// </summary>
	public class DetailInfo : CategorizedInfo 
	{
		public DetailInfo(String text) : base(text) {}
		public DetailInfo(String text, Icon icon) : base(text, icon) {}
	}

	/// <summary>
	/// Extension of CategorizedInfo specifically for entries.  Adds nothing.
	/// </summary>
	public class MainInfo : CategorizedInfo 
	{
		public MainInfo(String text) : base(text) {}
		public MainInfo(String text, Icon icon) : base(text, icon) {}
	}

	/// <summary>
	/// Defines the necessary information about a form to display it in the 
	/// ctlFileView.  This class is immutable.
	/// </summary>
	public class CategorizedItemInfo
	{
		#region "Instance Variables"
		/// <summary>
		/// The category of this form info.  Each entry corresponds to one level
		/// in the category tree.
		/// </summary>
		private CategoryInfo[] mCategory;

		/// <summary>
		/// The main info about this item.
		/// </summary>
		private MainInfo mInfo = new MainInfo("Window");

		/// <summary>
		/// Any details associated with the item.
		/// </summary>
		private DetailInfo[] mDetails = new DetailInfo[0];
		#endregion

		#region "Constructors"

		public CategorizedItemInfo() 
		{
			this.mCategory = new CategoryInfo[] {new CategoryInfo("Misc")};
		}

		/// <summary>
		/// Creates a new info with everything the same except the category.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="category"></param>
		public CategorizedItemInfo(CategorizedItemInfo info, CategoryInfo[] category) : this(info)
		{
			if (category == null) 
			{
				throw new ArgumentNullException("category");
			}
			this.mCategory = category;
		}

		/// <summary>
		/// Creates a new info with everything the copied from the info.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="category"></param>
		public CategorizedItemInfo(CategorizedItemInfo info) 
		{
			this.mCategory = info.Category;
			this.mInfo = info.mInfo;
			this.mDetails = info.Details;
		}

		/// <summary>
		/// Creates a new info with everything the same except the text of the main category.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="category"></param>
		public CategorizedItemInfo(CategorizedItemInfo info, String text) : this(info) 
		{
			if (text == null) 
			{
				throw new ArgumentNullException("text");
			}
			this.mInfo = new MainInfo(text, info.Info.Icon);
		}

		/// <summary>
		/// Creates a new info with everything the same except the details.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="category"></param>
		public CategorizedItemInfo(CategorizedItemInfo info, DetailInfo[] details) : this(info)
		{
			if (details == null) 
			{
				throw new ArgumentNullException("details");
			}
			this.mDetails = (DetailInfo []) details.Clone();
		}

		/// <summary>
		/// Creates a new info with everything the same except the icon and selected icons.  
		/// Currently the selected icon is ignored.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="category"></param>
		public CategorizedItemInfo(CategorizedItemInfo info, Icon icon, Icon selectedIcon) : 
			this(info) 
		{
			this.mInfo = new MainInfo(info.Info.Text, icon);
		}

		/// <summary>
		/// Creates a new info with everything the same except the icon.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="category"></param>
		public CategorizedItemInfo(CategorizedItemInfo info, Icon icon) : 
			this(info) 
		{
			this.mInfo = new MainInfo(info.Info.Text, icon);
		}
		#endregion

		#region "Properties"
		public CategoryInfo[] Category
		{
			get
			{
				return (CategoryInfo[]) this.mCategory.Clone();
			}
		}

		public MainInfo Info
		{
			get
			{
				return this.mInfo;
			}
		}

		public DetailInfo[] Details
		{
			get
			{
				return (DetailInfo[]) this.mDetails.Clone();
			}
		}
		#endregion
	}
}
