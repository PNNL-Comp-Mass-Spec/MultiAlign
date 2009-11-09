using System;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Collections.Generic;

namespace PNNLControls.Data
{
	public class Database
	{
		static private SQLiteConnection cnn = null;
		static private SQLiteDataReader reader = null;

		/// <summary>
		/// Only contructor: creates a database with the given name.
		/// </summary>
		/// <param name="dbName"></param>
		public Database(string dbName)
		{
			// Make sure the file does not already exist
			File.Delete(dbName + ".db3");

			// Create a connection and a command
			cnn = new SQLiteConnection("Data Source=" + dbName + ".db3");
		}



		/// <summary>
		/// Opens the database for editing.
		/// </summary>
		public void Open()
		{
			cnn.Open();
			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				tempCmd.CommandText = "PRAGMA journal_mode = OFF";
				tempCmd.ExecuteNonQuery();
				tempCmd.CommandText = "PRAGMA synchronous = 0";
				tempCmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Creates a new table inside the database.
		/// </summary>
		/// <param name="newTableName"></param>
		public void CreateTable(string newTableName)
		{
			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				tempCmd.CommandText = "CREATE TABLE " + newTableName + "(tableIndex INTEGER PRIMARY KEY, name VARCHAR(50), value1 DOUBLE, value2 DOUBLE, value3 DOUBLE, value4 DOUBLE, value5 DOUBLE, value6 DOUBLE, value7 DOUBLE, value8 DOUBLE, value9 DOUBLE)";
				tempCmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Inserts new data at the end of the table (for basic users).
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="dataName"></param>
		/// <param name="dataValue"></param>
		/// 
		/*public void InsertInto(string tableName, string dataName, double dataValue)
		{
			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				tempCmd.CommandText = "INSERT INTO " + tableName + "(name, value1, value2, value3, value4, value5, value6, value7, value8, value9) VALUES('" + dataName + "', " + Convert.ToString(dataValue1) + "', " + Convert.ToString(dataValue2) + "', " + Convert.ToString(dataValue3) + "', " + Convert.ToString(dataValue4) + "', " + Convert.ToString(dataValue5) + "', " + Convert.ToString(dataValue6) + "', " + Convert.ToString(dataValue7) + "', " + Convert.ToString(dataValue8) + "', " + Convert.ToString(dataValue9) + ")";
				tempCmd.ExecuteNonQuery();
			}
		}*/

		/// <summary>
		/// Inserts new data at the end of the table (for advanced users ONLY).
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="dataName"></param>
		/// <param name="dataValue"></param>
		/// 
		public void InsertInto(string tableName, string dataName, long arrayLength)
		{
			using (SQLiteTransaction mytransaction = cnn.BeginTransaction())
			{
				using (SQLiteCommand tempCmd = cnn.CreateCommand())
				{
					tempCmd.CommandText = "INSERT INTO " + tableName + "(name, value1, value2, value3, value4, value5, value6, value7, value8, value9) VALUES (@dataName, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00)";
					tempCmd.Parameters.Add(new SQLiteParameter("@dataName"));

					for (int i = 0; i < arrayLength; i++)
					{
						tempCmd.Parameters["@dataName"].Value = dataName + i.ToString();
						tempCmd.ExecuteNonQuery();
					}
				}
				mytransaction.Commit();
			}

			#region ATTEMPTS
			/*	THIRD TRY - BEFORE MODIFIED PARAMETERS
			SQLiteTransaction mytransaction = cnn.BeginTransaction();

			using (cmd)
			{
				SQLiteParameter myparam = new SQLiteParameter();

				cmd.CommandText = "INSERT INTO " + tableName + "(name, value) VALUES (@dataName, 0.00)";
				cmd.Parameters.Add(myparam);
				myparam.ParameterName = "@dataName";


				for (int i = 0; i < 100000; i++)
				{
					myparam.Value = dataName + i.ToString();
					cmd.ExecuteNonQuery();
				}
			}
			mytransaction.Commit();
			
			
			/*	SECOND TRY
			SQLiteTransaction mytransaction = cnn.BeginTransaction();

			using (cmd)
			{
				SQLiteParameter myparam = new SQLiteParameter();

				cmd.CommandText = "INSERT INTO " + tableName + "(name, value) VALUES ('" + dataName + "', 0.00)";
				cmd.Parameters.Add(myparam);

				for (int i = 0; i < 100000; i++)
				{
					myparam.Value = i + 1;
					cmd.ExecuteNonQuery();
				}
			}
			mytransaction.Commit();


			/*	FIRST TRY
			SQLiteConnection tempCnn = new SQLiteConnection("proteinDB.db3");
			SQLiteTransaction transaction = tempCnn.BeginTransaction();

			for (int i = 0; i < 1000000; i++)
			{
				new SQLiteCommand("INSERT INTO " + tableName + "(name, value) VALUES ('" + dataName + i + "', 0.00)", tempCnn, transaction).ExecuteNonQuery();
			}
			transaction.Commit();
			*/
			#endregion
		}

		//FAILED ATTEMPT TO IMPLEMENT A SELECT METHOD; ONLY WORKS TO SELECT ALL (*)
		/// <summary>
		/// Returns the results of the query in a string.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="Parameters"></param>
		/// <returns></returns>
		public string[] Select(string tableName, string parameters)
		{
			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				tempCmd.CommandText = "SELECT " + parameters + " FROM " + tableName;
				reader = tempCmd.ExecuteReader();
			}

			List<string> results = new List<string>();

			for (int i = 0; reader.Read(); i++)
			{
				results.Add(String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6], reader[7], reader[8], reader[9], reader[10]));
			}

			if (reader != null)
			{ reader.Close(); }

			return results.ToArray();
		}

		/// <summary>
		/// Returns the results of the query in a string.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="Parameters"></param>
		/// <returns></returns>
		public string[] SelectWhere(string selectParameters, string tableName, string whereParameters)
		{
			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				tempCmd.CommandText = "SELECT " + selectParameters + " FROM " + tableName + " WHERE " + whereParameters;
				reader = tempCmd.ExecuteReader();
			}

			List<string> results = new List<string>();

			for (int i = 0; reader.Read(); i++)
			{
				results.Add(String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6], reader[7], reader[8], reader[9], reader[10]));
			}

			if (reader != null)
			{ reader.Close(); }

			return results.ToArray();

			#region NO PARAMETERS
			/*
						cmd.CommandText = "SELECT " + selectParameters + " FROM " + tableName + " WHERE " + whereParameters;
			reader = cmd.ExecuteReader();

			List<string> results = new List<string>();

			for (int i = 0; reader.Read(); i++)
			{
				results.Add(String.Format("{0}, {1}, {2}", reader[0], reader[1], reader[2]));
			}

			if (reader != null)
			{ reader.Close(); }

			return results.ToArray();
			*/
			#endregion
		}


		/// <summary>
		/// Modifies the specified data in the database.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="commands"></param>
		/// <param name="parameters"></param>
		public void Update(string tableName, string commands, string parameters)
		{
			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				tempCmd.CommandText = "UPDATE @tableName SET @commands WHERE @parameters";
				tempCmd.Parameters.Add(new SQLiteParameter("@tableName"));
				tempCmd.Parameters.Add(new SQLiteParameter("@commands"));
				tempCmd.Parameters.Add(new SQLiteParameter("@parameters"));

				tempCmd.Parameters["@tableName"].Value = tableName;
				tempCmd.Parameters["@commands"].Value = commands;
				tempCmd.Parameters["@parameters"].Value = parameters;

				tempCmd.ExecuteNonQuery();
			}

			#region NO PARAMETERS
			/*
			cmd.CommandText = "UPDATE " + tableName + " SET " + commands + " WHERE " + parameters;
			cmd.ExecuteNonQuery();
			*/
			#endregion
		}


		/// <summary>
		/// Stores the buffer in the database.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="commands"></param>
		/// <param name="parameters"></param>
		public void UpdateCache(classProtein[] data, long startIndex, long length)
		{
			SQLiteTransaction mytransaction = cnn.BeginTransaction();

			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				SQLiteParameter myparam = new SQLiteParameter();

				tempCmd.CommandText = "UPDATE proteinList1 SET name=@dataName, value1=@dataValue1, value2=@dataValue2, value3=@dataValue3, value4=@dataValue4, value5=@dataValue5, value6=@dataValue6, value7=@dataValue7, value8=@dataValue8, value9=@dataValue9 WHERE tableIndex=@indexValue";
				tempCmd.Parameters.Add(new SQLiteParameter("@dataName"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue1"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue2"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue3"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue4"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue5"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue6"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue7"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue8"));
				tempCmd.Parameters.Add(new SQLiteParameter("@dataValue9"));
				tempCmd.Parameters.Add(new SQLiteParameter("@indexValue"));

				for (int i = 0; i < length; i++)
				{
					tempCmd.Parameters["@dataName"].Value = data[i].Name;
					tempCmd.Parameters["@dataValue1"].Value = data[i].Value1;
					tempCmd.Parameters["@dataValue2"].Value = data[i].Value2;
					tempCmd.Parameters["@dataValue3"].Value = data[i].Value3;
					tempCmd.Parameters["@dataValue4"].Value = data[i].Value4;
					tempCmd.Parameters["@dataValue5"].Value = data[i].Value5;
					tempCmd.Parameters["@dataValue6"].Value = data[i].Value6;
					tempCmd.Parameters["@dataValue7"].Value = data[i].Value7;
					tempCmd.Parameters["@dataValue8"].Value = data[i].Value8;
					tempCmd.Parameters["@dataValue9"].Value = data[i].Value9;
					tempCmd.Parameters["@indexValue"].Value = Convert.ToString(startIndex + i + 1);
					tempCmd.ExecuteNonQuery();
				}
			}
			mytransaction.Commit();
			mytransaction.Dispose();

			#region FIRST ATTEMPT
			/*
			SQLiteTransaction mytransaction = cnn.BeginTransaction();

			using (cmd)
			{
				SQLiteParameter myparam = new SQLiteParameter();

				for (int i = 0; i < length; i++)
				{
					cmd.CommandText = "UPDATE proteinList1 SET name='" + data[i].Name + "', value=" + data[i].Value + " WHERE tableIndex=" + Convert.ToString(startIndex + i + 1);
					cmd.ExecuteNonQuery();
				}
			}
			mytransaction.Commit();
			*/
			#endregion
		}



		/// <summary>
		/// Deletes the specified row(s) in the database.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="parameters"></param>
		public void Delete(string tableName, string parameters)
		{
			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				tempCmd.CommandText = "DELETE FROM " + tableName + " WHERE " + parameters;
				tempCmd.ExecuteNonQuery();
			}
		}


		/// <summary>
		/// Outputs the contents of the table.
		/// </summary>
		/// <param name="tableName"></param>
		public void Output(string tableName)
		{
			using (SQLiteCommand tempCmd = cnn.CreateCommand())
			{
				tempCmd.CommandText = "SELECT * FROM " + tableName;

				reader = tempCmd.ExecuteReader();
			}

			while (reader.Read())
			{
				Console.WriteLine(String.Format("tableIndex = {0}, name = {1}, value1 = {2}, value2 = {3}, value3 = {4}, value4 = {5}, value5 = {6}, value6 = {7}, value7 = {8}, value8 = {9}, value9 = {10}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6], reader[7], reader[8], reader[9], reader[10]));
			}

			if (reader != null)
			{ reader.Close(); }
		}

		/// <summary>
		/// Closes the database.
		/// </summary>
		public void Close()
		{
			if (cnn != null)
			{ cnn.Close(); }
		}
	}
}




		/*	NOT NEEDED?
		/// <summary>
		/// Inserts new data at the end of the table with an index.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="index"></param>
		/// <param name="dataName"></param>
		/// <param name="dataValue"></param>
		public void InsertInto(string tableName, int index, string dataName, double dataValue)
		{
			cmd.CommandText = "INSERT INTO " + tableName + "(tableIndex, name, value) VALUES(" + index.ToString() + ", '" + dataName + "', " + dataValue.ToString() + ")";
			cmd.ExecuteNonQuery();
		}
		*/