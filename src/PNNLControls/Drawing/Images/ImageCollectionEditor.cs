using System ;
using System.Drawing ;
using System.ComponentModel ;
using System.ComponentModel.Design ;
using System.Drawing.Design ;

namespace PNNLControls {
	/// <summary>
	/// Used to edit ImageCollection, just uses the builtin image editor.
	/// </summary>
	public class ImageCollectionEditor : System.ComponentModel.Design.CollectionEditor {

		public ImageCollectionEditor() : base(typeof(ImageCollection)) 
		{
		}

		/// <summary>
		/// Gets the builtin editor used for image collections.
		/// </summary>
		/// <param name="ItemType"></param>
		/// <returns></returns>
		protected override object CreateInstance(Type ItemType) {
			UITypeEditor editor = ((UITypeEditor) TypeDescriptor.GetEditor(typeof(Image), typeof(UITypeEditor)));
			return ((Image) editor.EditValue(base.Context, null));
		}
	}
}
