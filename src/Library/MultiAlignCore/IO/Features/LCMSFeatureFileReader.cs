using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using PNNLOmics.Data.Features;
using MultiAlignEngine.Features;

namespace MultiAlignCore.IO.Features
{
	public class LCMSFeatureFileReader
	{
		private StreamReader m_umcFileReader;
		private Dictionary<String, int> m_columnMap;
		private List<UMCLight> m_umcList;

		#region Constructors
		/// <summary>
		/// Constructor for passing in a StreamReader object
		/// </summary>
		/// <param name="streamReader">StreamReader object for UMC csv file to be read</param>
		public LCMSFeatureFileReader(StreamReader streamReader)
		{
			m_umcFileReader = streamReader;
			m_columnMap     = CreateColumnMapping();
			m_umcList       = SaveDataToUmcList();
		}

		/// <summary>
		/// Constructor for passing in a String containing the location of the UMC csv file
		/// </summary>
		/// <param name="filePath">String containing the location of the UMC csv file</param>
        public LCMSFeatureFileReader(string filePath)
		{
			m_umcFileReader = new StreamReader(filePath);
			m_columnMap     = CreateColumnMapping();
			m_umcList       = SaveDataToUmcList();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Returns the umcList contained in this class
		/// </summary>
		public List<UMCLight> GetUmcList()
		{
			return m_umcList;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Fills in the Column Map with the appropriate values.
		/// The Map will have a Column Property (e.g. Umc.Mass) mapped to a Column Number.
		/// </summary>
		/// <returns>The column map as a Dictionary object</returns>
		private Dictionary<String, int> CreateColumnMapping()
		{
			Dictionary<String, int> columnMap = new Dictionary<String, int>();

			String[] columnTitles = m_umcFileReader.ReadLine().Split('\t', '\n');
			int numOfColumns = columnTitles.Length;

			for (int i = 0; i < numOfColumns; i++)
			{
				switch (columnTitles[i].Trim())
				{
                    case "Saturated_Member_Count":
                        columnMap.Add("Umc.SaturatedCount", i);
                        break;
					case "UMCIndex":
						columnMap.Add("Umc.Id", i);
						break;
					case "Feature_index":
						columnMap.Add("Umc.Id", i);
						break;
					case "Feature_Index":
						columnMap.Add("Umc.Id", i);
						break;
					case "ScanStart":
						columnMap.Add("Umc.ScanStart", i);
						break;
					case "Scan_Start":
						columnMap.Add("Umc.ScanStart", i);
						break;
					case "ScanEnd":
						columnMap.Add("Umc.ScanEnd", i);
						break;
					case "Scan_End":
						columnMap.Add("Umc.ScanEnd", i);
						break;
					case "ScanClassRep":
						columnMap.Add("Umc.Scan", i);
						columnMap.Add("Umc.ScanAligned", i);
						break;
					case "Scan":
						columnMap.Add("Umc.Scan", i);
						columnMap.Add("Umc.ScanAligned", i);
						break;
					case "NETClassRep":
						columnMap.Add("Umc.Net", i);
						break;
					case "UMCMonoMW":
						columnMap.Add("Umc.Mass", i);
						columnMap.Add("Umc.MassCalibrated", i);
						break;
					case "monoisotopic_mass":
						columnMap.Add("Umc.Mass", i);
						columnMap.Add("Umc.MassCalibrated", i);
                        break;
					case "Monoisotopic_Mass":
						columnMap.Add("Umc.Mass", i);
						columnMap.Add("Umc.MassCalibrated", i);
						break;
                    case "UMCMWStDev":
                        columnMap.Add("Umc.MassStandardDeviation", i);
                        break;
					case "UMCMZForChargeBasis":
						columnMap.Add("Umc.MZForCharge", i);
						break;
					case "Class_Rep_MZ":
						columnMap.Add("Umc.MZForCharge", i);
						break;
					case "UMCAbundance":
						columnMap.Add("Umc.AbundanceSum", i);
						break;
					case "Abundance":
						columnMap.Add("Umc.AbundanceSum", i);
						break;
					case "MaxAbundance":
						columnMap.Add("Umc.AbundanceMax", i);
						break;
					case "Max_Abundance":
						columnMap.Add("Umc.AbundanceMax", i);
						break;
					case "ClassStatsChargeBasis":
						columnMap.Add("Umc.ChargeRepresentative", i);
						break;
					case "Class_Rep_Charge":
						columnMap.Add("Umc.ChargeRepresentative", i);
						break;
					case "ChargeStateMax":
						columnMap.Add("Umc.ChargeMax", i);
						break;
					case "UMCMemberCount":
						columnMap.Add("Umc.SpectralCount", i);
						break;
					case "UMC_Member_Count":
						columnMap.Add("Umc.SpectralCount", i);
						break;
					case "Drift_Time":
						columnMap.Add("Umc.DriftTime", i);
						break;
					case "Drift_Time_Uncorrected":
						columnMap.Add("Umc.DriftTimeUncorrected", i);
						break;
					case "Avg_Interference_Score":
						columnMap.Add("Umc.AverageInterferenceScore", i);
						break;
					case "Conformation_Fit_Score":
						columnMap.Add("Umc.ConformationFitScore", i);
						break;
					case "Decon2ls_Fit_Score":
						columnMap.Add("Umc.AverageDeconFitScore", i);
						break;
					case "Members_Percentage":
						columnMap.Add("Umc.MembersPercentageScore", i);
						break;
					case "Combined_Score":
						columnMap.Add("Umc.CombinedScore", i);
						break;
					default:
						//Title not found.
						break;
				}
			}

			return columnMap;
		}

		

		/// <summary>
		/// Saves the data from a UMC csv file to an array of clsUMC Objects.
		/// </summary>
		private List<UMCLight> SaveDataToUmcList()
		{
            List<UMCLight>  umcList     = new List<UMCLight>();
			string          line;
            UMCLight        umc;
			int             previousId  = -99;
			int             currentId   = -99;
			int             idIndex     = 0;

            int minScan = int.MaxValue;
            int maxScan = int.MinValue;

			// Read the rest of the Stream, 1 line at a time, and save the appropriate data into new Objects
			while ((line = m_umcFileReader.ReadLine()) != null)
			{
				String[] columns = line.Split(',', '\t', '\n');

				if (m_columnMap.ContainsKey("Umc.Id"))
				{
					currentId = Int32.Parse(columns[m_columnMap["Umc.Id"]]);
				}
				else
				{
					currentId = idIndex;
					idIndex++;
				}

				
				/// If the UMC ID matches the previous UMC ID, then skip the UMC data.
				///		- It is the same UMC, different peptide, and we have already stored this UMC data.
				if (previousId != currentId)
				{
                    umc = new UMCLight();
					umc.ID = currentId;
					if (m_columnMap.ContainsKey("Umc.ScanStart"))				umc.ScanStart       = int.Parse(columns[m_columnMap["Umc.ScanStart"]]);
					if (m_columnMap.ContainsKey("Umc.ScanEnd"))					umc.ScanEnd         = int.Parse(columns[m_columnMap["Umc.ScanEnd"]]);
					if (m_columnMap.ContainsKey("Umc.Scan"))					umc.Scan            = int.Parse(columns[m_columnMap["Umc.Scan"]]);					
					if (m_columnMap.ContainsKey("Umc.Net"))						umc.NET             = Double.Parse(columns[m_columnMap["Umc.Net"]]);
					if (m_columnMap.ContainsKey("Umc.Mass"))					umc.MassMonoisotopic    = Double.Parse(columns[m_columnMap["Umc.Mass"]]);					
                    if (m_columnMap.ContainsKey("Umc.AbundanceSum"))
                    {
                        try
                        {
                            // To handle bugs from Feature Finder.
                            string data = columns[m_columnMap["Umc.AbundanceSum"]];                            
                            if (data.StartsWith("-"))
                            {
                                umc.AbundanceSum = 0;
                            }
                            else
                            {
                                umc.AbundanceSum = long.Parse(data, NumberStyles.AllowDecimalPoint);
                            }
                        }
                        catch(Exception ex)
                        {
                            throw ex;
                        }

                    }
                    if (m_columnMap.ContainsKey("Umc.AbundanceMax"))
                    {
                        umc.Abundance = long.Parse(columns[m_columnMap["Umc.AbundanceMax"]]);                        
                    }
                    if (m_columnMap.ContainsKey("Umc.ChargeRepresentative"))
                    {
                        umc.ChargeState = int.Parse(columns[m_columnMap["Umc.ChargeRepresentative"]]);                        
                    }
					if (m_columnMap.ContainsKey("Umc.SpectralCount"))				umc.SpectralCount           = int.Parse(columns[m_columnMap["Umc.SpectralCount"]]);
					if (m_columnMap.ContainsKey("Umc.MZForCharge"))					umc.Mz                      = Double.Parse(columns[m_columnMap["Umc.MZForCharge"]]);
					if (m_columnMap.ContainsKey("Umc.DriftTime"))					umc.DriftTime               = double.Parse(columns[m_columnMap["Umc.DriftTime"]]);
					if (m_columnMap.ContainsKey("Umc.AverageInterferenceScore"))
                    {
                        double d = Double.Parse(columns[m_columnMap["Umc.AverageInterferenceScore"]]);
                        if (double.IsNegativeInfinity(d))
                        {
                            d = Convert.ToDouble(Decimal.MinValue/100);
                        }
                        else if (double.IsPositiveInfinity(d))
                        {
                            d = Convert.ToDouble(double.MaxValue/100);
                        }
                        umc.AverageInterferenceScore = d;
                    }
					if (m_columnMap.ContainsKey("Umc.ConformationFitScore"))		umc.ConformationFitScore = Double.Parse(columns[m_columnMap["Umc.ConformationFitScore"]]);
					if (m_columnMap.ContainsKey("Umc.AverageDeconFitScore"))		umc.AverageDeconFitScore = Double.Parse(columns[m_columnMap["Umc.AverageDeconFitScore"]]);
                    if (m_columnMap.ContainsKey("Umc.SaturatedCount"))              umc.SaturatedMemberCount = int.Parse(columns[m_columnMap["Umc.SaturatedCount"]]);
					umcList.Add(umc);

                    minScan = Math.Min(umc.Scan, minScan);
                    maxScan = Math.Max(umc.Scan, maxScan);

					previousId = currentId;
				}
			}
            
            foreach(UMCLight x in umcList)
            {
                x.MassMonoisotopicAligned   = x.MassMonoisotopic;
                x.ScanAligned               = x.Scan;
                x.NET                       = (Convert.ToDouble(x.Scan - minScan) / Convert.ToDouble(maxScan - minScan));
                x.NETAligned                = x.NET;
                x.RetentionTime             = (Convert.ToDouble(x.Scan - minScan) / Convert.ToDouble(maxScan - minScan));
            }            
			return umcList;
		}
		#endregion
	}
}
