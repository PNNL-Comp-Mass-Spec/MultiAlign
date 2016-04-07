using System.Collections.Generic;
using System.IO;

namespace MultiAlignCore.IO.TextFiles
{
    using MultiAlignCore.IO.DatasetLoaders;

    public interface ITextFileReader<T>
	{

        /// <summary>
        /// Read data from the text file and return an enumerable list of objects of type T
        /// </summary>
        /// <param name="fileLocation">Full path to a file</param>
        /// <returns>Enumerable list of type T</returns>
		IEnumerable<T> ReadFile(string fileLocation, IFeatureFilter<T> filter = null);

        /// <summary>
        /// Read data using the TextReader and return an enumerable list of objects of type T
        /// </summary>
        /// <param name="textReader">TextReader object</param>
        /// <returns>Enumerable list of type T</returns>
		IEnumerable<T> ReadFile(TextReader textReader, IFeatureFilter<T> filter = null);
	}
}
