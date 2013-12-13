using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.MassTags;
using PNNLOmics.Data.MassTags;
using PNNLOmics.Data;
using System.IO;
using System.Collections;

namespace MultiAlignCore.IO.MTDB
{
    public class DriftTimeTextFileDatabaseLoader: IMtdbLoader
    {
        public DriftTimeTextFileDatabaseLoader(string path)
        {
            DatabasePath = path;
        }

        public string DatabasePath { get; private set; }

        #region IMtdbLoader Members
        public Data.MassTags.MassTagDatabase LoadDatabase()
        {
            MassTagDatabase database                    = new MassTagDatabase();
            List<MassTagLight> massTags                 = new List<MassTagLight>();
            Dictionary<int, List<Protein>> masstagMap   = new Dictionary<int, List<Protein>>();

            //TODO: Put into base table file reader
            string [] lines = File.ReadAllLines(DatabasePath);

            string [] delimiters = new string [] {"\t"};

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data        = lines[i].Split(delimiters, StringSplitOptions.RemoveEmptyEntries );
                MassTagLight tag     = new MassTagLight();
                if (data.Length > 4)
                {
                    tag.ID               = Convert.ToInt32(data[0]);
                    tag.PeptideSequence  = data[1];
                    tag.MassMonoisotopic = Convert.ToDouble(data[2]);
                    tag.RetentionTime    = Convert.ToDouble(data[3]);
                    tag.DriftTime        = Convert.ToDouble(data[4]);
                }
                massTags.Add(tag);
            }

            database.AddMassTagsAndProteins(massTags, masstagMap);
            return database;
        }

        public MassTagDatabaseOptions Options
        {
            get;
            set;
        }

        #endregion
    }
}
