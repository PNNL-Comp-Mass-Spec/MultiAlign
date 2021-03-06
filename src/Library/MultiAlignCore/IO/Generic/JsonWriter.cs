﻿#region

using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

#endregion

namespace MultiAlignCore.IO.Generic
{
    public class JsonWriter<T>
        where T : class
    {
        public void Write(string fileName, T data)
        {
            using (var writer = File.CreateText(fileName))
            {
                var jWriter = new JsonTextWriter(writer) {Formatting = Formatting.Indented};
                var serializer = new JsonSerializer
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                    TypeNameHandling = TypeNameHandling.All
                };
                serializer.Serialize(jWriter, data);
            }
        }
    }
}