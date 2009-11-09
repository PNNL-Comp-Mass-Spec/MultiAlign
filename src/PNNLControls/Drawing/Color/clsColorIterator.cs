using System;
using System.Drawing ; 

namespace PNNLControls
{
	/// <summary>
	/// clsColorIterator can be used to provide a sequence of colors based on indices. This can be used by 
	/// forms that want to give random starting colors to series that are plotted
	/// </summary>
	public class clsColorIterator
	{
		Color [] marr_clr ;
		public clsColorIterator()
		{
			marr_clr = new Color[] {Color.Red, Color.Blue, Color.Black, Color.Green, Color.Salmon, Color.Pink, Color.Navy, Color.DarkSlateBlue} ;
		}
		public Color GetColor(int index)
		{
			return marr_clr[index % marr_clr.Length] ; 
		}
	}
}
