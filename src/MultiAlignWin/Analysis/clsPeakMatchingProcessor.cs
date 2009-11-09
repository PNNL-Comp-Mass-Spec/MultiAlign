using System;

namespace MultiAlignWinNew
{
	/// <summary>
	/// Summary description for clsPeakMatchingProcessor.
	/// </summary>
	public class clsPeakMatchingProcessor
	{
		public enum enmStatus 
		{
			IDLE=0, MS_ALIGNMENT_INITIALIZED, MASSTAG_ALIGNMENT_INITIALIZED, ALIGNING_TO_MS, 
			ALIGNING_TO_MASSTAG, DONE, ERROR 
		} ;
		private enmStatus menmStatus = enmStatus.IDLE; 

		#region "Properties"
		public enmStatus Status
		{
			get
			{
				return menmStatus ; 
			}
		}
		#endregion

		public clsPeakMatchingProcessor()
		{
		}
	}
}
