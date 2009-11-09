using System;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsPlotRange.
	/// </summary>
	public class clsPlotRange
	{
		//x and y axis ranges
		public float mflt_xstart = float.MaxValue;
		public float mflt_xend = float.MinValue;
		public float mflt_ystart = float.MaxValue;
		public float mflt_yend = float.MinValue;
		public float m_xDataRange = 0;
		public float m_yDataRange = 0;

		public clsPlotRange()
		{
		}

		public void CalculateRanges()
		{
			m_xDataRange = mflt_xend - mflt_xstart ; 
			m_yDataRange = mflt_yend - mflt_ystart ; 
		}
	}
}
