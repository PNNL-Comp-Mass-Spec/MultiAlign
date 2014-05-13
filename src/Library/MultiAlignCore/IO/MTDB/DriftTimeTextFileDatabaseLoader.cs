#region

using System;
using System.Collections.Generic;
using System.IO;
using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;

#endregion

namespace MultiAlignCore.IO.MTDB
{
    public class DriftTimeTextFileDatabaseLoader : IMtdbLoader
    {
        public DriftTimeTextFileDatabaseLoader(string path)
        {
            DatabasePath = path;
        }

        public string DatabasePath { get; private set; }

        #region IMtdbLoader Members

        public MassTagDatabase LoadDatabase()
        {
            var database = new MassTagDatabase();
            var massTags = new List<MassTagLight>();
            var masstagMap = new Dictionary<int, List<Protein>>();

            //TODO: Put into base table file reader
            var lines = File.ReadAllLines(DatabasePath);

            var delimiters = new[] {"\t"};

            for (var i = 1; i < lines.Length; i++)
            {
                var data = lines[i].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                var tag = new MassTagLight();
                if (data.Length > 4)
                {
                    tag.Id = Convert.ToInt32(data[0]);
                    tag.PeptideSequence = data[1];
                    tag.MassMonoisotopic = Convert.ToDouble(data[2]);
                    tag.Net = Convert.ToDouble(data[3]);
                    tag.DriftTime = Convert.ToDouble(data[4]);
                }
                massTags.Add(tag);
            }

            database.AddMassTagsAndProteins(massTags, masstagMap);
            return database;
        }

        #endregion
    }
}