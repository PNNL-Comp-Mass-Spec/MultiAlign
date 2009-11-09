using System;

namespace PNNLProteomics.EventModel
{
	/// <summary>
	/// Summary description for IMessageClass.
	/// </summary>
	public interface IMessageClass
	{
		/// <summary>
		/// Delegate provided by calling classes to be called to set percent complete
		/// </summary>
		event DelegateSetPercentComplete PercentComplete ; 
		/// <summary>
		/// Delegate provided by calling classes to be called to set status message
		/// </summary>
		event DelegateSetStatusMessage StatusMessage ; 
		/// <summary>
		/// Delegate provided by calling classes to be called to set form title based on status
		/// </summary>
		event DelegateSetStatusMessage TitleMessage ; 
	}
}
