#region

using System;
using System.Collections.Generic;
using FeatureAlignment.Data.MassTags;
using Mage;

#endregion

namespace MultiAlignCore.IO.MTDB
{
    internal class MAGEMetaSampleDatabaseSink : ISinkModule
    {
        /// <summary>
        /// Maps a column
        /// </summary>
        private readonly Dictionary<string, int> m_columnMapping;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MAGEMetaSampleDatabaseSink()
        {
            MassTags = new List<MassTagLight>();
            m_columnMapping = new Dictionary<string, int>();
        }

        /// <summary>
        /// Handles the column definitions for a factor module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            m_columnMapping.Clear();

            for (var i = 0; i < args.ColumnDefs.Count; i++)
            {
                var def = args.ColumnDefs[i];
                m_columnMapping.Add(def.Name.Trim(), i);
            }
            // ignore the column definitions.
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
                throw new NullReferenceException("The mass tags are invalid.");
            }

            if (args.Fields == null)
            {
                return;
                throw new NullReferenceException("The mass tag database rows are invalid.");
            }

            if (args.Fields.Length < 11)
            {
                return;
                throw new ArgumentException("The number of columns for the mass tags are invalid.");
            }

            var tag = new MassTagLight();

            if (m_columnMapping.ContainsKey("Mass"))
            {
                tag.MassMonoisotopic = Convert.ToDouble(args.Fields[m_columnMapping["Mass"]]);
            }
            else
            {
                return;
            }

            if (m_columnMapping.ContainsKey("NET"))
            {
                tag.NetAverage = Convert.ToDouble(args.Fields[m_columnMapping["NET"]]);
                tag.Net = tag.NetAverage;
            }
            else
            {
                return;
            }
            if (m_columnMapping.ContainsKey("Dataset_Member_Count"))
            {
                tag.ObservationCount = Convert.ToInt32(args.Fields[m_columnMapping["Dataset_Member_Count"]]);
            }
            else
            {
                return;
            }

            if (m_columnMapping.ContainsKey("Cluster_ID"))
            {
                tag.Id = Convert.ToInt32(args.Fields[m_columnMapping["Cluster_ID"]]);
            }
            else
            {
                return;
            }

            if (m_columnMapping.ContainsKey("Drift_Time"))
            {
                tag.DriftTime = Convert.ToDouble(args.Fields[m_columnMapping["Drift_Time"]]);
            }
            else
            {
                return;
            }

            if (m_columnMapping.ContainsKey("Charge"))
            {
                tag.ChargeState = Convert.ToInt32(args.Fields[m_columnMapping["Charge"]]);
            }
            else
            {
                return;
            }

            if (m_columnMapping.ContainsKey("Score"))
            {
                tag.Score = Convert.ToDouble(args.Fields[m_columnMapping["Score"]]);
            }
            else
            {
                return;
            }

            MassTags.Add(tag);
        }

        /// <summary>
        /// Gets a list of the mass tags
        /// </summary>
        public List<MassTagLight> MassTags { get; private set; }
    }
}