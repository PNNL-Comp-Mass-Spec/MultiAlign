using System;
using System.Collections.Generic;
using System.IO;
using PNNLOmics.Annotations;

namespace MultiAlignCore.IO.TextFiles
{
    public abstract class BaseTextFileReader<T> : ITextFileReader<T>
    {
        /// <summary>
        /// Default file delimiter.
        /// </summary>
        private const char DEFAULT_DELIMITER = ',';

        /// <summary>
        /// Number of data points (data lines) loaded
        /// </summary>
        protected int mDataPointsLoaded;

        /// <summary>
        /// Number of data points (data lines) skipped
        /// </summary>
        protected int mDataPointsSkipped;

        /// <summary>
        /// Path to the delimited file being read
        /// </summary>
        /// <remarks>Null if ReadFile is called with a TextReader object</remarks>
        protected string mTextFilePath;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>Default column delimiter is a comma</remarks>
        protected BaseTextFileReader()
        {
            Delimiter = DEFAULT_DELIMITER;
            mDataPointsLoaded = 0;
            mDataPointsSkipped = 0;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the file reading delimiter.
        /// </summary>
        [UsedImplicitly]
        public char Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the file reading delimiter.
        /// </summary>
        /// <remarks>
        /// The setter only uses the first character of the string
        /// </remarks>
        public string Delimeter
        {
            get { return Delimiter.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentOutOfRangeException(value, "Column delimiter cannot be empty");

                Delimiter = value[0];
            }
        }

        /// <summary>
        /// Number of data points (data lines) loaded
        /// </summary>
        public int DataPointsLoaded
        {
            get { return mDataPointsLoaded; }
        }

        /// <summary>
        /// Number of data points (data lines) skipped
        /// </summary>
        public int DataPointsSkipped
        {
            get { return mDataPointsSkipped; }
        }

        #endregion

        /// <summary>
        /// Read data from the text file and return an enumerable list of objects of type T
        /// </summary>
        /// <param name="fileLocation">Full path to a file</param>
        /// <returns>Enumerable list of type T</returns>
        public IEnumerable<T> ReadFile(string fileLocation)
        {
            IEnumerable<T> returnEnumerable;

            mTextFilePath = fileLocation;

            try
            {
                using (TextReader textReader = new StreamReader(new FileStream(fileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    returnEnumerable = ReadFile(textReader);
                    textReader.Close();
                }
            }
            finally
            {
                mTextFilePath = null;
            }

            return returnEnumerable;
        }

        /// <summary>
        /// Read data using the TextReader and return an enumerable list of objects of type T
        /// </summary>
        /// <param name="textReader">TextReader object</param>
        /// <returns>Enumerable list of type T</returns>
        public IEnumerable<T> ReadFile(TextReader textReader)
        {
            var columnMapping = CreateColumnMapping(textReader);

            if (columnMapping.Count == 0)
            {
                throw new ApplicationException("Given file does not contain any valid column headers.");
            }

            var enumerable = SaveFileToEnumerable(textReader, columnMapping);
            return enumerable;
        }

        protected abstract Dictionary<string, int> CreateColumnMapping(TextReader textReader);
        protected abstract IEnumerable<T> SaveFileToEnumerable(TextReader textReader, Dictionary<string, int> columnMapping);

    }
}
