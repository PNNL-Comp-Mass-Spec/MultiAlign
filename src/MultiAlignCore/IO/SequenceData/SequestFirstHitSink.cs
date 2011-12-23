using System;
using System.Collections.Generic;
using Mage;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Factors;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.SequenceData;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.Data.SequenceData;

namespace MultiAlignCore.IO.SequenceData
{
    /// <summary>
    /// You can make your own object that can be included 
    /// as the last module in a Mage pipeline and receive the data stream directly.
    /// </summary>
    public class SequestFirstHitSink: ISinkModule, IMageSink
    {
        private int m_count; 
        private List<DatabaseSearchSequence> m_sequences;
        private IDatabaseSearchSequenceDAO   m_database;
        /// <summary>
        /// Maps a column 
        /// </summary>
        private Dictionary<string, int> m_columnMapping;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="datasets">Datasets to store data about.</param>
        public SequestFirstHitSink(IDatabaseSearchSequenceDAO databaseInterface)
        {                 
            m_sequences         = new List<DatabaseSearchSequence>();
            m_columnMapping     = new Dictionary<string, int>(); 
            m_database          = databaseInterface;
        }
        /// <summary>
        /// Handles the column definitions for a factor module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            m_sequences.Clear();
            m_count = 0;
            m_columnMapping.Clear();

            for(int i = 0; i < args.ColumnDefs.Count; i++)                
            {
                MageColumnDef def = args.ColumnDefs[i];
                m_columnMapping.Add(def.Name.Trim(), i);
            }            
        }

        /// <summary>
        /// Handles converting the rows to factor objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args == null)
            {
                throw new NullReferenceException("The factors are invalid.");
            }

            if (args.Fields == null)
            {
                return;
                throw new NullReferenceException("The factor rows are invalid.");
            }

            if (args.Fields.Length < 4)
            {
                return;
                throw new ArgumentException("The number of columns for the factors are invalid.");
            }

            DatabaseSearchSequence sequence = new DatabaseSearchSequence();
            
            if (m_columnMapping.ContainsKey("Peptide"))
            {
                sequence.Sequence = Convert.ToString(args.Fields[m_columnMapping["Peptide"]]);
            }
            else
            {
                return;
            }
            
                                                   
            if (m_columnMapping.ContainsKey("ScanNum"))
            {
                sequence.Scan = Convert.ToInt32(args.Fields[m_columnMapping["ScanNum"]]);
            }
            else
            {
                return;
            }

            
            if (m_columnMapping.ContainsKey("Reference"))
            {
               sequence.Reference  = args.Fields[m_columnMapping["Reference"]].ToString();
            }
            else
            {
                return;
            }

            if (m_columnMapping.ContainsKey("XCorr"))
            {
                sequence.Score = Convert.ToDouble(args.Fields[m_columnMapping["XCorr"]]);
            }
            else
            {
                return;
            }
            if (m_columnMapping.ContainsKey("DelCn2"))
            {
                sequence.DeltaScore = Convert.ToDouble(args.Fields[m_columnMapping["DelCn2"]]);
            }
            else
            {
                return;
            }
            if (m_columnMapping.ContainsKey("DelCn"))
            {
                sequence.DeltaScore2 = Convert.ToDouble(args.Fields[m_columnMapping["DelCn"]]);
            }
            else
            {
                return;
            }
            if (m_columnMapping.ContainsKey("RankSp"))
            {
                sequence.Rank = Convert.ToInt32(args.Fields[m_columnMapping["RankSp"]]);
            }
            else
            {
                return;
            }
            if (m_columnMapping.ContainsKey("ChargeState"))
            {
                sequence.Charge = Convert.ToInt32(args.Fields[m_columnMapping["ChargeState"]]);
            }
            else
            {
                return;
            }

            if (m_columnMapping.ContainsKey("MH"))
            {
                sequence.Mass = Convert.ToDouble(args.Fields[m_columnMapping["MH"]]);
            }
            else
            {
                return;
            }
            if (m_columnMapping.ContainsKey("NumTrypticEnds"))
            {
                sequence.TrypticEnds = Convert.ToInt32(args.Fields[m_columnMapping["NumTrypticEnds"]]);
            }
            else
            {
                return;
            }
            
            sequence.ID      = m_count ++;
            sequence.GroupID = DatasetID;    
            m_sequences.Add(sequence);
        }

        /// <summary>
        /// Gets or sets the current Group ID to use.
        /// </summary>
        public int DatasetID
        {
            get;set;
        }

        /// <summary>
        /// Commits the factor data to the repository.
        /// </summary>
        public void CommitChanges()
        {                                   
            m_database.AddAll(m_sequences);
        }
    }
}