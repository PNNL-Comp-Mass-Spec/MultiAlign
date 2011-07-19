using System;

namespace PNNLProteomics.EventModel
{
	public delegate void DelegateSetStatusMessage(int statusLevel, string status) ; 
	public delegate void DelegateSetPercentComplete(int percentDone) ; 
}
