using System;

namespace MultiAlignCore.IO.Mammoth
{
    /// <summary>
    /// Exception thrown when something bad happens with a database's state. 
    /// </summary>
    public class MammothDatabaseStateException: Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message"></param>
        public MammothDatabaseStateException(string message)
            : base(message)
        {            
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MammothDatabaseStateException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
