#region

using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

#endregion

namespace MultiAlignCore.IO.Generic
{
    public class JsonReader<T>
        where T : class
    {
        public T Read(string fileName)
        {
            using (var reader = File.OpenText(fileName))
            {
                var jReader = new JsonTextReader(reader);

                var serializer = new JsonSerializer
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                    TypeNameHandling = TypeNameHandling.All,
                };

                //TODO: Handle when there is an error with loading a device.
                var data = serializer.Deserialize(jReader, typeof (T)) as T;
                return data;
            }
        }
    }
}