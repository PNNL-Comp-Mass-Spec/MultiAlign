using System;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsTagGroup.
	/// </summary>
	public class clsTagGroup
	{
		private int mStartPixel = 0;
		private int mStopPixel = 0;
		private int[] mTags = null;


		public int StartPixel
		{
			get{return mStartPixel;}
			set{mStartPixel = value;}
		}

		public int StopPixel
		{
			get{return mStopPixel;}
			set{mStopPixel = value;}
		}

		public int[] Tags
		{
			get{return mTags;}
			set{mTags = value;}
		}

		public clsTagGroup()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
