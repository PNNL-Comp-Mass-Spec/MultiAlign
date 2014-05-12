#region

using System.ComponentModel;

#endregion

namespace MultiAlignCore.IO.InputFiles
{
    /// <summary>
    ///     Holds information about input files.
    /// </summary>
    public class InputFile : INotifyPropertyChanged
    {
        private string m_path;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public InputFile()
        {
            Path = null;
            FileType = InputFileType.NotRecognized;
        }

        private void OnNotify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///     Gets or sets the path to the input file.
        /// </summary>
        public string Path
        {
            get { return m_path; }
            set
            {
                if (m_path != value)
                {
                    m_path = value;
                    OnNotify("Path");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the input file type.
        /// </summary>
        public InputFileType FileType { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}