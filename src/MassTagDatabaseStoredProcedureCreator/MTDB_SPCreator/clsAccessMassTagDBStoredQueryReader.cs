/*//////////////////////////////////////////////////////////// 
 *	Author: Brian LaMarche
 *  Date: 3-4-2008
 *  File: 
 *  Notes: 
 *		Handles querying the Microsoft Access database using 
 *  ADO classes for running stored queries in the database.
 * 
 *	Revisions:
 *		3-4-2008
 *			Cleaned up code.
/*////////////////////////////////////////////////////////////
using System;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;


namespace MassTagDatabaseStoredQueryCreator
{
	/// <summary>
	/// Summary description for clsMADSPQuery.
	/// </summary>
	public class clsAccessMassTagDBStoredQueryReader
	{
		private const  string STORED_QUERYNAME_MASS_TAG_PROTEIN_NAME_MAP = "GetMassTagToProteinNameMap";
		private string mstr_connectionString;
		private string mstr_getMassTagToProteinNameMapStoredQueryName;
		
		/// <summary>
		/// Default constructor for interfacing the Access database to run stored queries.
		/// </summary>
		/// <param name="filename">Full path to database to query from.</param>
		public clsAccessMassTagDBStoredQueryReader(string connectionString)
		{			
			mstr_getMassTagToProteinNameMapStoredQueryName = STORED_QUERYNAME_MASS_TAG_PROTEIN_NAME_MAP;
		}

		/// <summary>
		/// Retrieves the data from the Access database under the GetMassTagToProteinNameMap Stored Query
		/// </summary>
		/// <param name="storedQueryName">Name of stored query to run.</param>
		public DataSet GetMassTagToProteinNameMapData()
		{			
		    OleDbConnection connection = new OleDbConnection(mstr_connectionString);
		    OleDbCommand command = new OleDbCommand(mstr_getMassTagToProteinNameMapStoredQueryName,
													connection);

		    command.CommandType = System.Data.CommandType.StoredProcedure;
		    connection.Open();
		
			OleDbParameter param1 = new OleDbParameter (
											"@ConfirmedOnly",
											System.Data.OleDb.OleDbType.TinyInt);
			OleDbParameter param2 = new OleDbParameter(
											"@MinimumHighNormalizedScore",
											System.Data.OleDb.OleDbType.Single);
			OleDbParameter param3 = new OleDbParameter(
											"@MinimumPMTQualityScore",
											System.Data.OleDb.OleDbType.Single);
			OleDbParameter param4 = new OleDbParameter(
											"@MinimumHighDiscriminantScore",
											System.Data.OleDb.OleDbType.Single	
											);														
			OleDbParameter param5 = new OleDbParameter(
											"@MinimumPeptideProphetProbability",
											System.Data.OleDb.OleDbType.Single);							

			param1.Value = System.DBNull.Value;
			param2.Value = System.DBNull.Value;
			param3.Value = System.DBNull.Value;
			param4.Value = System.DBNull.Value;
			param5.Value = System.DBNull.Value;						
			command.Parameters.Add(param1);
			command.Parameters.Add(param2);
			command.Parameters.Add(param3);
			command.Parameters.Add(param4);
			command.Parameters.Add(param5);

			OleDbDataAdapter adapter = new OleDbDataAdapter(command);
			DataSet data = new DataSet();
			adapter.Fill(data);
			adapter.Dispose();
		    connection.Close();
			return data;
		}

	}
}
