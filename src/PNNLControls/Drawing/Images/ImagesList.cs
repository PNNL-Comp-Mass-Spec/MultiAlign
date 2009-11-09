using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace PNNLControls
{
	/// <summary>
	/// Like an ImageList, but doesn't copy the images into the list, and 
	/// preserves transparency and animation of the images.  Can be put on 
	/// the design surface.
	/// </summary>
	public class ImagesList : System.ComponentModel.Component
	{
		/// <summary>
		/// The collection of images.
		/// </summary>
		private ImageCollection mImagesCollection = new ImageCollection();

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ImagesList(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();
		}

		public ImagesList()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();
		}

		public ImagesList(System.ComponentModel.IContainer container, ImageCollection collection) 
			: this (container)
		{
			if (collection == null) 
			{
				throw new ArgumentNullException("collection");
			}
			this.mImagesCollection = collection;
		}

		public ImagesList(ImageCollection collection) 
			: this ()
		{
			if (collection == null) 
			{
				throw new ArgumentNullException("collection");
			}
			this.mImagesCollection = collection;
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

		/// <summary>
		/// The images in the list.
		/// </summary>
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public ImageCollection Images 
		{
			get 
			{
				return mImagesCollection;
			}
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
