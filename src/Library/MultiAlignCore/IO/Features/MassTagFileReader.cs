using System;
using System.Collections.Generic;
using System.IO;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.TextFiles;

namespace MultiAlignCore.IO.Features
{
    using MultiAlignCore.IO.DatasetLoaders;

    /// <summary>
    /// This class is used by STACConsole
    /// </summary>
	public class MassTagFileReader : BaseTextFileReader<MassTagLight>
	{
		protected override Dictionary<string, int> CreateColumnMapping(TextReader textReader)
		{
            var columnMap = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

		    var dateLine = textReader.ReadLine();
		    if (string.IsNullOrWhiteSpace(dateLine))
                return columnMap;

            var columnTitles = dateLine.Split('\t', '\n');
			var numOfColumns = columnTitles.Length;

            for (var i = 0; i < numOfColumns; i++)
            {
                switch (columnTitles[i].Trim().ToLower())
                {
                    case "mass_tag_id":
                        columnMap.Add("MassTag.ID", i);
                        break;
                    case "mass_tag_index":
                        columnMap.Add("MassTag.Index", i);
                        break;
                    case "conformer_id":
                        columnMap.Add("MassTag.ConformationID", i);
                        break;
                    case "monoisotopic_mass":
                        columnMap.Add("MassTag.MassMonoisotopic", i);
                        break;
                    case "avg_ganet":
                        columnMap.Add("MassTag.NET", i);
                        break;
                    case "net_value_to_use":
                        columnMap.Add("MassTag.NET", i);
                        break;
                    case "high_peptide_prophet_probability":
                        columnMap.Add("MassTag.PriorProbability", i);
                        break;
                    case "cnt_ganet":
                        columnMap.Add("MassTag.ObservationCount", i);
                        break;
                    case "net_obs_count":
                        columnMap.Add("MassTag.ObservationCount", i);
                        break;
                    case "pnet":
                        columnMap.Add("MassTag.NETPredicted", i);
                        break;
                    case "avg_drift_time":
                        columnMap.Add("MassTag.DriftTime", i);
                        break;
                    case "drift_time_avg":
                        columnMap.Add("MassTag.DriftTime", i);
                        break;
                    case "high_discriminant_score":
                        columnMap.Add("MassTag.DiscriminantMax", i);
                        break;
                    case "std_ganet":
                        columnMap.Add("MassTag.NETStandardDeviation", i);
                        break;
                    case "conformer_charge":
                        columnMap.Add("MassTag.ChargeState", i);
                        break;
                    case "conformer_obs_count":
                        columnMap.Add("MassTag.ConformationObservationCount", i);
                        break;
                    default:
                        //Title not found.
                        break;
                }
            }

			return columnMap;
		}

        /// <summary>
		/// Read the entire file
		/// </summary>
		/// <param name="textReader"></param>
		/// <param name="columnMapping"></param>
        /// <returns>List of MassTagLights</returns>
        protected override IEnumerable<MassTagLight> SaveFileToEnumerable(TextReader textReader, Dictionary<string, int> columnMapping, IFeatureFilter<MassTagLight> filter = null)
		{
            var massTagList = new List<MassTagLight>();
			string line;
		    var idIndex = 0;

			// Read the rest of the Stream, 1 line at a time, and save the appropriate data into new Objects
			while ((line = textReader.ReadLine()) != null)
			{
                if (string.IsNullOrWhiteSpace(line))
                    continue;

				var columns = line.Split('\t', '\n');

			    var currentId = -99;
			    if (columnMapping.ContainsKey("MassTag.ID"))
				{
					currentId = Int32.Parse(columns[columnMapping["MassTag.ID"]]);
				}
				else
				{
					currentId = idIndex;
					idIndex++;
				}

			    var massTag = new MassTagLight
			    {
			        Id = currentId
			    };

			    if (columnMapping.ContainsKey("MassTag.Index"))
				{
					massTag.Index = Int32.Parse(columns[columnMapping["MassTag.Index"]]);
				}
				else
				{
					massTag.Index = currentId;
				}

				if (columnMapping.ContainsKey("MassTag.MassMonoisotopic")) massTag.MassMonoisotopic = double.Parse(columns[columnMapping["MassTag.MassMonoisotopic"]]);
				if (columnMapping.ContainsKey("MassTag.NET")) massTag.Net = double.Parse(columns[columnMapping["MassTag.NET"]]);
				if (columnMapping.ContainsKey("MassTag.PriorProbability")) massTag.PriorProbability = double.Parse(columns[columnMapping["MassTag.PriorProbability"]]);
				if (columnMapping.ContainsKey("MassTag.ObservationCount")) massTag.ObservationCount = ushort.Parse(columns[columnMapping["MassTag.ObservationCount"]]);
				if (columnMapping.ContainsKey("MassTag.NETPredicted")) massTag.NetPredicted = double.Parse(columns[columnMapping["MassTag.NETPredicted"]]);
				if (columnMapping.ContainsKey("MassTag.DiscriminantMax")) massTag.DiscriminantMax = double.Parse(columns[columnMapping["MassTag.DiscriminantMax"]]);
				if (columnMapping.ContainsKey("MassTag.NETStandardDeviation")) massTag.NetStandardDeviation = double.Parse(columns[columnMapping["MassTag.NETStandardDeviation"]]);
				if (columnMapping.ContainsKey("MassTag.DriftTime"))
				{
					if (!columns[columnMapping["MassTag.DriftTime"]].Equals("NULL"))
					{
						massTag.DriftTime = float.Parse(columns[columnMapping["MassTag.DriftTime"]]);
						if (columnMapping.ContainsKey("MassTag.ConformationID")) massTag.ConformationId = int.Parse(columns[columnMapping["MassTag.ConformationID"]]);
						if (columnMapping.ContainsKey("MassTag.ChargeState")) massTag.ChargeState = int.Parse(columns[columnMapping["MassTag.ChargeState"]]);
						if (columnMapping.ContainsKey("MassTag.ConformationObservationCount")) massTag.ConformationObservationCount = ushort.Parse(columns[columnMapping["MassTag.ConformationObservationCount"]]);
					}
					else
					{
						massTag.DriftTime = -99;
					}
				}

				massTagList.Add(massTag);
			}

			return massTagList;
		}
	}
}
