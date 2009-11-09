/*////////////////////////////////////////////////////////////
 * 
 *	Author: Brian LaMarche
 *  Date: 3-4-2008
 *  File: clsAccessStoredQueryCreator.
 *  Notes: 
 *		Creats a generic Stored Query in Access. 
 *	Revisions:
 *		3-7-2008
 *			Moved code from clsMADSPCreator to here for creating generic
 *		Stored queries.
/*////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;

namespace MassTagDatabaseStoredQueryCreator
{
	/// <summary>
	/// Class that can interface a Microsoft Access Database using Jet 4.0 for creating stored queries
	/// given a stored query string.
	/// </summary>
	public class clsAccessStoredQueryCreator
	{			
		/// <summary>
		/// connection string to the database.
		/// </summary>
		private string mstr_connectionString;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public clsAccessStoredQueryCreator()
		{
			mstr_connectionString = null;	
		}

		/// <summary>
		/// Default constructor for the Access Database stored query creator.  
		/// </summary>
		/// <param name="filename">Full path to the database location.</param>
		public clsAccessStoredQueryCreator(string databasePath)
		{
			mstr_connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + databasePath;
		}
	
		/// <summary>
		/// Reads a file with the stored query in it.
		/// </summary>
		/// <param name="filename">File to read.</param>
		/// <returns>Command string found in the file.  String is unmodified raw text.</returns>
		private string ReadFile(string filename)
		{
			string command		= "";	
			FileInfo file		= new FileInfo(filename);

			if (file.Exists == false)
				throw new FileNotFoundException("Cannot read file in clsAccessToredQueryCreator.", filename);

			TextReader reader	= file.OpenText();
			command				= reader.ReadToEnd();
			reader.Close();	
		
			return command;
		}

		/// <summary>
		/// Create the stored query specified in the argument command.  Throws a InvalidOperatorException if the class was not 
		/// given a database connection string.
		/// </summary>
		/// <param name="command">Stored query command to add to database.</param>
		/// <returns>True if success, false if exception was thrown.</returns>
		public bool CreateStoredQuery(string command)
		{
			if (mstr_connectionString == null || mstr_connectionString == "")
				throw new InvalidOperationException("The database class was not supplied a connection string.");

			try
			{			 
				command			= command.Replace("ALTER", "CREATE");
				command			= command.Replace("@", "");
				command			= command.Replace("dbo.", "");
				CreateStoredProcedure(mstr_connectionString, command);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Create a stored query in the access database using the Stored Procedure text found in the file path argument.  Throws FileNotFoundException if 
		/// path does not exist.
		/// </summary>
		/// <param name="filePath">Path of stored query to add to database.</param>
		/// <returns>True if creating the stored procedure was a success.  False if the creation failed.  This method internally handles database exceptions thrown.</returns>
		public bool CreateStoredQueryFromFile(string filePath)
		{
			return CreateStoredQuery(ReadFile(filePath));
		}
	
		/// <summary>
		/// Creates a stored procedure from a generic sqlString and binds to a connection using the connection string supplied.
		/// </summary>
		/// <param name="connectionString">Connection string to database.</param>
		/// <param name="storedQueryCommandString">Stored Query string to </param>
		public void CreateStoredProcedure(string connectionString, string storedQueryCommandString)
		{		
			OleDbConnection connection		= new OleDbConnection(connectionString);
			OleDbCommand command			= new OleDbCommand();
			OleDbDataAdapter adapater		= new OleDbDataAdapter();			
			IDbCommand    comm				= command;
			IDbConnection conn				= connection;
			conn.Open();
			comm.Connection					= conn;
			comm.CommandText				= storedQueryCommandString;
			comm.ExecuteNonQuery();
			conn.Close();
		}
	}
}
