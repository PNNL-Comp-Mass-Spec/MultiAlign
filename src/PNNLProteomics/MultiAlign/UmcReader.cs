using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PNNLProteomics.MultiAlign.Hibernate.Domain;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;
using Iesi.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLProteomics.MultiAlign.Hibernate;

namespace PNNLProteomics.MultiAlign
{
	public class UmcReader
	{
		private StreamReader m_umcFileReader;
		private Dictionary<String, int> m_columnMap;
		private List<clsUMC> m_umcList;

		#region Constructors
		/// <summary>
		/// Constructor for passing in a StreamReader object
		/// </summary>
		/// <param name="streamReader">StreamReader object for UMC csv file to be read</param>
		public UmcReader(StreamReader streamReader)
		{
			m_umcFileReader = streamReader;
			m_columnMap = CreateColumnMapping();
			m_umcList = SaveDataToUmcList();
		}

		/// <summary>
		/// Constructor for passing in a String containing the location of the UMC csv file
		/// </summary>
		/// <param name="filePath">String containing the location of the UMC csv file</param>
		public UmcReader(String filePath)
		{
			m_umcFileReader = new StreamReader(filePath);
			m_columnMap = CreateColumnMapping();
			m_umcList = SaveDataToUmcList();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Saves the data to a SQLite Database.
		/// </summary>
		public void SaveDataToSQLite()
		{
			UmcDAOHibernate umcDAOHibernate = new UmcDAOHibernate();
			umcDAOHibernate.AddAll(m_umcList);
		}

		/// <summary>
		/// Saves the data to a SQLite Database.
		/// </summary>
		/// <param name="dbLocation">String containing the location of the SQLite DB</param>
		public void SaveDataToSQLite(String dbLocation)
		{
			NHibernateUtil.SetDbLocation(dbLocation);
			UmcDAOHibernate umcDAOHibernate = new UmcDAOHibernate();
			umcDAOHibernate.AddAll(m_umcList);
		}

		/// <summary>
		/// Returns the umcList contained in this class
		/// </summary>
		public List<clsUMC> GetUmcList()
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
				switch (columnTitles[i])
				{
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
		private List<clsUMC> SaveDataToUmcList()
		{
			List<clsUMC> umcList = new List<clsUMC>();
			String line;
			clsUMC umc;
			int previousId = -99;
			int currentId = -99;
			int idIndex = 0;

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
					umc = new clsUMC();
					umc.Id = currentId;
					if (m_columnMap.ContainsKey("Umc.ScanStart"))				umc.ScanStart = Int16.Parse(columns[m_columnMap["Umc.ScanStart"]]);
					if (m_columnMap.ContainsKey("Umc.ScanEnd"))					umc.ScanEnd = Int16.Parse(columns[m_columnMap["Umc.ScanEnd"]]);
					if (m_columnMap.ContainsKey("Umc.Scan"))					umc.Scan = Int16.Parse(columns[m_columnMap["Umc.Scan"]]);
					if (m_columnMap.ContainsKey("Umc.ScanAligned"))				umc.ScanAligned = Int16.Parse(columns[m_columnMap["Umc.ScanAligned"]]);
					if (m_columnMap.ContainsKey("Umc.Net"))						umc.Net = Double.Parse(columns[m_columnMap["Umc.Net"]]);
					if (m_columnMap.ContainsKey("Umc.Mass"))					umc.Mass = Double.Parse(columns[m_columnMap["Umc.Mass"]]);
					if (m_columnMap.ContainsKey("Umc.MassCalibrated"))			umc.MassCalibrated = Double.Parse(columns[m_columnMap["Umc.MassCalibrated"]]);
					if (m_columnMap.ContainsKey("Umc.AbundanceSum"))			umc.AbundanceSum = Double.Parse(columns[m_columnMap["Umc.AbundanceSum"]]);
					if (m_columnMap.ContainsKey("Umc.AbundanceMax"))			umc.AbundanceMax = Double.Parse(columns[m_columnMap["Umc.AbundanceMax"]]);
					if (m_columnMap.ContainsKey("Umc.ChargeRepresentative"))	umc.ChargeRepresentative = (short)Int16.Parse(columns[m_columnMap["Umc.ChargeRepresentative"]]);
					if (m_columnMap.ContainsKey("Umc.ChargeMax"))				umc.ChargeMax = (short)Int16.Parse(columns[m_columnMap["Umc.ChargeMax"]]);
					if (m_columnMap.ContainsKey("Umc.SpectralCount"))			umc.SpectralCount = (short)Int16.Parse(columns[m_columnMap["Umc.SpectralCount"]]);
                    if (m_columnMap.ContainsKey("Umc.MZForCharge"))             umc.mdouble_class_rep_mz = Double.Parse(columns[m_columnMap["Umc.MZForCharge"]]);
					umcList.Add(umc);
					previousId = currentId;
				}
			}
			return umcList;
		}
		#endregion
	}
}
