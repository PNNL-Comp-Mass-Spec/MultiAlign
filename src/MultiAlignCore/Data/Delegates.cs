using System;

namespace MultiAlignCore.EventModel
{
	public delegate void DelegateSetStatusMessage(int statusLevel, string status) ; 
	public delegate void DelegateSetPercentComplete(int percentDone) ; 
}
