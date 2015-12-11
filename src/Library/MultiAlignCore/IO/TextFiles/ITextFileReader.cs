using System.Collections.Generic;
using System.IO;

namespace MultiAlignCore.IO.TextFiles
{
	public interface ITextFileReader<T>
	{

        /// <summary>
        /// Read data from the text file and return an enumerable list of objects of type T
        /// </summary>
        /// <param name="fileLocation">Full path to a file</param>
        /// <returns>Enumerable list of type T</returns>
		IEnumerable<T> ReadFile(string fileLocation);

        /// <summary>
        /// Read data using the TextReader and return an enumerable list of objects of type T
        /// </summary>
        /// <param name="textReader">TextReader object</param>
        /// <returns>Enumerable list of type T</returns>
		IEnumerable<T> ReadFile(TextReader textReader);
	}
}
