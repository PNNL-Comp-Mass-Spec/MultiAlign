using System.Collections.Generic;
using System.IO;

namespace MultiAlignCore.IO.TextFiles
{
	public interface ITextFileReader<T>
	{
		// TODO: Add XML Comments
		// TODO: Abstract this out from PNNLOmics entirely
		IEnumerable<T> ReadFile(string fileLocation);
		IEnumerable<T> ReadFile(TextReader textReader);
	}
}
