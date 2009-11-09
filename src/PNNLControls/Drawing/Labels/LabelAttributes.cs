using System;
using System.Collections;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for LabelAttributes.
	/// </summary>
	public class LabelAttributes
	{
		public string text;
		public ArrayList branches;
		public object root;
		public int level;
		public int index; //index in the overall data set

		public LabelItem label;

		public LabelAttributes()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
