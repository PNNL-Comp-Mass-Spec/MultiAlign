using System;

namespace MultiAlignParameterFileEditor
{

    /// <summary>
    /// Class that handles the event arguments data when parameter files are edited..
    /// </summary>
    public class ParameterFileEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path"></param>
        public ParameterFileEventArgs(string path)
        {
            Path    = path;
            Message = "";
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path"></param>
        public ParameterFileEventArgs(string path, string message)
        {
            Path    = path;
            Message = message;
        }

        #region
        /// <summary>
        /// Gets or sets the messaeg.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the path the file was saved to.
        /// </summary>
        public string Path
        {
            get;
            private set;
        }
        #endregion
    }
}
