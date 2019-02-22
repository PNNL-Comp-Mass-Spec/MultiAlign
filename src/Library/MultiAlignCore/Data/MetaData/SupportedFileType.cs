#region

using MultiAlignCore.IO.InputFiles;

#endregion

namespace MultiAlignCore.Data.MetaData
{
    public class SupportedFileType
    {
        public SupportedFileType(string name,
            string extension,
            InputFileType type)
        {
            Name = name;
            InputType = type;
            Extension = extension;
        }

        /// <summary>
        /// Gets or sets the extension of the dataset type.
        /// </summary>
        public string Extension { get; private set; }

        public string Name { get; private set; }
        public InputFileType InputType { get; private set; }
    }
}