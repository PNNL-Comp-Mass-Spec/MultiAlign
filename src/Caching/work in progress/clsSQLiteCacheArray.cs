using System;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace PNNLControls.Data
{
    public enum enmSQLiteDataCacheMode
    {
        Create,
        Open,
        OpenCreate
    }
      
    /// <summary>
    /// SQLite Cached Array class for caching data into memory and serialization into a database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class clsSQLiteCacheArray<T>:  clsCacheArray<T> , IDisposable where T:  new()
    {
        public const long DEFAULT_CACHE_LENGTH = 100;
        private const string DEFAULT_PRIMARY_KEY_TYPE_STRING = "tableIndex integer primary key";
        private const string INTEGER = "INTEGER";
        private const string LONG = "LONG";
        private const string STRING = "VARCHAR(256)";
        private const string FLOAT = "FLOAT";
        private const string DOUBLE = "DOUBLE";

        /// <summary>
        /// Array list of the property info for type T.
        /// </summary>
        private ArrayList marr_properties; 
        /// <summary>
        /// Hash table for mapping the C# type (long,int,string) to the SQLite equivalent.
        /// </summary>        
        private Hashtable mobj_propertyToSQLTypeHash;
        /// <summary>
        /// Flag if the table exists or was created.
        /// </summary>
        private bool mbln_createdTables = false;        
        /// <summary>
        /// Database connection object.
        /// </summary>
        private SQLiteConnection mobj_connection;

        /// <summary>
        /// Substitutes for SELECT * which isnt allowed in SQLite.
        /// </summary>
        private string mstr_selectString;
		private string mstr_name;
        /// <summary>
        /// Constructor for clsSQLiteCacheArray.  Cache length is initialized to the default cache length.
        /// </summary>
        public clsSQLiteCacheArray():
            base(DEFAULT_CACHE_LENGTH, 0)
        {
            InitClass();
            mobj_connection = null;            
        }

        /// <summary>
        /// Initializes a clsSQLiteCacheArray with the specified cache length.
        /// </summary>
        /// <param name="cacheLength">Specifies the length of the cache to use.</param>
        public clsSQLiteCacheArray(long cacheLength):
            base(cacheLength, 0)
        {
            InitClass();
            mobj_connection = null;
        }

        protected override void Page(long index, bool retrieve, bool cache)
        {
            try
            {
                base.Page(index, retrieve, cache);
            }
            catch(IndexOutOfRangeException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught - closing database connection " + ex.Message);
                mobj_connection.Close();
            }
        }
        
        /// <summary>
        /// Constructor for opening/creating a database at the specified database path.
        /// </summary>
        /// <param name="databasePath">path to the database.</param>
        /// <param name="cacheCreateMode">creation mode</param>
        /// <param name="cacheLength"></param>
        /// <param name="arrayLength"></param>
        public clsSQLiteCacheArray(string databasePath, enmSQLiteDataCacheMode cacheCreateMode, long cacheLength, long arrayLength):
            base(cacheLength, arrayLength)
        {            
            InitClass();

            /// If we need to fill the database, we do it with the default values of the class we are caching.
            T dummy = new T();
            
            switch(cacheCreateMode)
            {
                /// User wants to open an existing table.
                case enmSQLiteDataCacheMode.Open:

                    /// 
                    ///  The user screwed up and supplied a bad array length, 
                    ///  So we stomp on their dreams.
                    ///  
                    if (arrayLength != 0)
                    {
                        throw new InvalidArraySizeException("Cannot open the database and specify the array length.  The array length parameter must be 0 for this mode.");
                    }

                    mobj_connection = new SQLiteConnection();
                    InitConnection(databasePath, false);
                    Open();
                    
					
                    if (TableExists(mstr_name) == false)
                        throw new Exception(string.Format("The table {0} does not exist.", mstr_name));                    
                    mlng_arrayLength = GetTableLength();
                    break;
					

                /// User wants to create a new table.
                case enmSQLiteDataCacheMode.Create:
                    mobj_connection = new SQLiteConnection();
                    InitConnection(databasePath, true);
                    Open();

                    if (TableExists(mstr_name) == true)
                    {
                        mobj_connection.Close();
                        mobj_connection.Dispose();
                        throw new Exception(string.Format("The table {0} already exists in the database.", mstr_name));
                    }
                    CreateTable();
                    
                    /// Fill in the database if we have just created it.
                    Fill(dummy, arrayLength);
                    mlng_arrayLength = GetTableLength();

                    break;  
                /// User wants to create a new table if it does not exist. 
                case enmSQLiteDataCacheMode.OpenCreate:
                    mobj_connection = new SQLiteConnection();
                    InitConnection(databasePath, true);
                    Open();

                    /// If the table does not exist then we have to fill it in
                    if (TableExists(mstr_name) == false)
                    {
                        CreateTable();
                        /// Fill in the database if we have just created it.
                        if (arrayLength > 0)
                        {
                            Fill(dummy, arrayLength);
                            mlng_arrayLength = GetTableLength();
                        }
                    }

                    break;
            }        
        }

        public clsSQLiteCacheArray(SQLiteConnection connection, long cacheLength, long arrayLength):
            base(cacheLength, 0)
        {                      
            InitClass();
            mobj_connection = connection.Clone() as SQLiteConnection;

            //TODO: Checks For connection constructor.
            ///
            /// Check to see if the table exists in the connection - if not then create it with the array length
            /// Otherwise if it does exist, make sure the array length is zero, otherwise throw an exception
            /// 
        }       

        /// <summary>
        /// Common initialization calls.
        /// </summary>
        protected void InitClass()
        {
            CreatePropertyNames();
            CreatePropertyToSQLTypeHashMap();
        }

        /// <summary>
        /// Creates the mapping between database type and C# value data types.
        /// </summary>
        private void CreatePropertyToSQLTypeHashMap()
        {
            mobj_propertyToSQLTypeHash = new Hashtable();
            int i = 0; long j = 0; string s = string.Empty; float f = 0.0f; double d = 0.0;

            mobj_propertyToSQLTypeHash.Add(i.GetType().FullName, INTEGER);
            mobj_propertyToSQLTypeHash.Add(j.GetType().FullName, LONG);
            mobj_propertyToSQLTypeHash.Add(s.GetType().FullName, STRING);
            mobj_propertyToSQLTypeHash.Add(f.GetType().FullName, FLOAT);
            mobj_propertyToSQLTypeHash.Add(d.GetType().FullName, DOUBLE);
        }

        /// <summary>
        /// Initializes the database connection object.  Sets the database path and tells SQLite if its a new database.        
        /// </summary>
        /// <param name="databsaePath">File path to the database file</param>
        /// <param name="newDatabase">True if the database is new, false if not.  False does not drop existing tables.</param>
        private void InitConnection(string databasePath, bool newDatabase)
        {
            string connectionString;            
            connectionString = "Data Source=" + databasePath + ";";
            connectionString += "New=" + newDatabase.ToString() + ";";
            connectionString += "Compress=TRUE;";            
            mobj_connection = new SQLiteConnection(connectionString);            
        }
        
        /// <summary>
        /// Closes the database connection.
        /// </summary>
        public void Close()
        {            
            mobj_connection.Close();
            mbln_createdTables = false;
        }

        /// <summary>
        /// Sets the connection string of the database.
        /// </summary>
        public string ConnectionString
        {
            get { return mobj_connection.ConnectionString; }
            set { mobj_connection.ConnectionString = value; }
        }

        /// <summary>
        /// Check to see if the database already exists.
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        private bool TableExists(string tablename)
        {
            string commandString = string.Format("SELECT name FROM sqlite_master where (name='{0}')", tablename);            
            SQLiteCommand command = mobj_connection.CreateCommand();
            command.CommandText = commandString;

            SQLiteDataReader reader = command.ExecuteReader();
            
            bool result = false;
            while (reader.Read() && result == false)
            {
                object objName = reader.GetValue(0);
                if (tablename == objName as string)
                {
                    result = true;
                }
            }

            reader.Close();
            reader.Dispose();
            command.Dispose();

            return result;
        }

        /// <summary>
        /// Opens the database.
        /// </summary>
        public void Open()
        {
			mobj_connection.Open();
			using (SQLiteCommand tempCmd = mobj_connection.CreateCommand())
			{
				tempCmd.CommandText = "PRAGMA journal_mode = OFF";
				tempCmd.ExecuteNonQuery();
				tempCmd.CommandText = "PRAGMA synchronous = 0";
				tempCmd.ExecuteNonQuery();
			}
        }       

        /// <summary>
        /// Retrieves the length of the array from the database itself.
        /// </summary>
        /// <returns>Length of table in entries.</returns>
        private long GetTableLength()
        {            
            SQLiteCommand command = mobj_connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) AS COUNT FROM " + mstr_name;
            SQLiteDataReader reader = command.ExecuteReader();

            object[] valueOfCount = new object[1];
            reader.GetValues(valueOfCount);

            long length = Convert.ToInt64(valueOfCount[0]);                
            return length;
        }

        /// <summary>
        /// Based off of the classes type T, creates an array list of the properties without having to continuously reflect the type.
        /// </summary>
        private void CreatePropertyNames()
        {
            /* Create a table name for now */
            Type objectType = typeof(T);
            mstr_name = objectType.Name;
            mstr_name = mstr_name.Trim();
  
            /* 
             * Reflect the properties searching for readable - value type properties only 
             * Specification only allows for value types not reference types.
             */
            marr_properties = new ArrayList();
            mstr_selectString = string.Empty;
            foreach (PropertyInfo propertyInfo in objectType.GetProperties())
            {
                if (propertyInfo.CanRead == false || propertyInfo.CanWrite == false)
                {
                    throw new MemberAccessException("Could not access property info from class type " +
                                objectType.Name + " for property " + propertyInfo.Name);
                }
                Type propertyType = propertyInfo.PropertyType;
                string propertyName = propertyInfo.Name;
                if (propertyInfo.PropertyType.IsPublic == true &&
                    propertyInfo.PropertyType.IsValueType == false &&
                    propertyInfo.PropertyType != typeof(string))
                {
                    throw new InvalidTypeException("The property " + propertyInfo.Name +
                        " is not a value type.  Tables can only be created with value types for cached arrays.");
                }
                mstr_selectString += propertyName + " , ";
                marr_properties.Add(propertyInfo);          
            }
            mstr_selectString = mstr_selectString.Substring(0, mstr_selectString.Length - 3);
        }

        /// <summary>
        /// Creates a table based off the type T in the inherent database.  Fields are created by reflecting public value type (and string) properties.
        /// </summary>        
        private void CreateTable()
        {
            /* Create a table name for now */
            Type objectType = typeof(T);
            
            /* Only create the table once */
            if (mbln_createdTables != false)
                return;

            string commandText = "CREATE TABLE IF NOT EXISTS ";
            commandText += mstr_name;
            commandText += " ( " + DEFAULT_PRIMARY_KEY_TYPE_STRING;
            
            /* 
             * Reflect the properties searching for readable - value type properties only 
             * Specification only allows for value types not reference types.
             */
            foreach (PropertyInfo property in marr_properties)
            {                
                /// 
                /// Make sure we can cache this type by mapping it from C# types to a sqlDataType.
                /// 
                string mappedType = mobj_propertyToSQLTypeHash[property.PropertyType.FullName] as string;
                if (mappedType == null)
                {
                    throw new InvalidTypeException("The type mapping from the internal data type to the SQLite datatype could not be found when creating a table.");
                }                
                commandText += " , " + property.Name + " " + mappedType;                
            }
            commandText += " ) ";

            /* 
             * Now that we know no exceptions can be thrown from the above create the command - run the 
             * command and cleanup. 
             */
            SQLiteCommand command = mobj_connection.CreateCommand();            
            command.CommandText = commandText;
            command.ExecuteNonQuery();
            command.Dispose();
            mbln_createdTables = true;            
        }

        /// <summary>
        /// Gets the connection state of the database.
        /// </summary>
        public ConnectionState ConnectionState
        {
            get { return mobj_connection.State; }            
        }

        /// <summary>
        /// Gets the ServerVersion of the database.  Different from the version of the underlying structures saved in the database.
        /// </summary>
        public string ServerVersion
        {
            get { return mobj_connection.ServerVersion; }
        }

        /// <summary>
        /// Returns the length or size of the array.
        /// </summary>
        public override long Length
        {
            get { return base.Length; }            
        }
        
        /// <summary>
        /// Creates an insertion string based off the objects properties and data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		private string CreateInsertionString(T data)
		{
			Type objectType = data.GetType();
			string insertCommandString = string.Empty;

			insertCommandString = "INSERT INTO ";
			insertCommandString += mstr_name;
			insertCommandString += " ( ";

			string columnNames = string.Empty;
			string values = " VALUES ( ";

			foreach (PropertyInfo property in marr_properties)
			{
				columnNames += property.Name + " , ";

				string valueOf;
				object o = property.GetValue(data, BindingFlags.GetProperty, null, null, null);
				if (property.PropertyType == typeof(string))
				{
					valueOf = "'" + o.ToString() + "'";
				}
				else
				{
					valueOf = o.ToString();
				}
				values += valueOf + " , ";
			}
			columnNames = columnNames.Substring(0, columnNames.Length - 3);
			values = values.Substring(0, values.Length - 3);

			insertCommandString += columnNames + " ) " + values + " ) ";
			return insertCommandString;
		}


		/// <summary>
        /// Inserts data into the database for a new item.
        /// </summary>
        /// <param name="data">Data to insert.</param>
        protected virtual void Insert(T data)
        {
            string commandText = CreateInsertionString(data);
            if (commandText == string.Empty)
                return;

            SQLiteCommand command = mobj_connection.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates an insertion string based off the objects properties and data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string CreateUpdateString(T data, long index)
        {
            Type objectType = typeof(T);
            string updateCommandString = string.Empty;

            updateCommandString = "UPDATE " + mstr_name + " SET ";

            string valueString = string.Empty;
            foreach (PropertyInfo property in marr_properties)
            {
                valueString += property.Name + "= ";
                string valueOf;
                object o = property.GetValue(data, BindingFlags.GetProperty, null, null, null);
                if (property.PropertyType == typeof(string))
                {
                    valueOf = "'" + o.ToString() + "'";
                }
                else
                {
                    valueOf = o.ToString();
                }
                valueString += valueOf + " , ";
            }
            updateCommandString += valueString.Substring(0, valueString.Length - 3);
            updateCommandString += " WHERE tableIndex=" + (index + 1).ToString();
            return updateCommandString;
        }

        /// <summary>
        /// Updates the database with data at index.
        /// </summary>
        /// <param name="data">Data to cache.</param>
        /// <param name="index">Index to update.</param>
        protected virtual void Update(T data, long index)
        {
            string commandString = CreateUpdateString(data, index);
            if (commandString == string.Empty)
                return;

            SQLiteCommand command = mobj_connection.CreateCommand();
            command.CommandText = commandString;
            command.ExecuteNonQuery();            
        }

		protected virtual void UpdateCache(T[] data, long index, long length)
		{
			using (SQLiteTransaction transaction = mobj_connection.BeginTransaction())
			{
				using (SQLiteCommand command = mobj_connection.CreateCommand())
				{
					for (int i = 0; i < length; i++)
					{
						string commandString = CreateUpdateString(data[i], index + i);
						if (commandString == string.Empty)
						{ return; }

						command.CommandText = commandString;
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

        private void Fill(T data, long length)
        {

			using (SQLiteTransaction transaction = mobj_connection.BeginTransaction())
			{
				using (SQLiteCommand command = mobj_connection.CreateCommand())
				{
					string commandText = CreateInsertionString(data);
					if (commandText == string.Empty)
					{ return; }
					command.CommandText = commandText;

					for (long i = 0; i < length; i++)
					{
						command.ExecuteNonQuery();
						mlng_arrayLength++;
					}
				}
				transaction.Commit();
			}

			#region ORIGINAL
			/*
            for (long i = 0; i < length; i++)
            {
                Insert(data);
                mlng_arrayLength++;
            }*/
			#endregion
		}

        private void Fill(long length)
		{
            for (long i = 0; i < length; i++)
            {
                T data = new T();
                Insert(data);
                mlng_arrayLength++;
            }
		} 

        /// <summary>
        /// Cache the array data to the stream.
        /// </summary>
        /// <param name="data">Data of type T to cache</param>
        /// <param name="startIndex">Start index to cache</param>
        /// <param name="length">Length to cache</param>
        protected override void Cache(T[] data, long startIndex, long length)
        {
            if (startIndex + length > mlng_arrayLength)
                throw new IndexOutOfRangeException("Cache indices are greater than the length of the array");

			UpdateCache(data, startIndex, length);
        }

        /// <summary>
        /// Retrieve an array of T starting at index of amount elements.
        /// </summary>
        /// <param name="index">Index of data stream to pull from</param>
        /// <param name="amount">Amount to retrieve.</param>
        /// <returns>Array of type T.</returns>
        protected override void Retrieve(T[] data, long index, long length)
        {
            SQLiteCommand command = mobj_connection.CreateCommand();
            string commandText  = "SELECT " + mstr_selectString + " FROM " + mstr_name + " WHERE ";
            /// 
            /// Indexes in the database are offset 1, so 0 C# index = 1 db, 1 C#  = 2 db.  Use
            /// <= for the high index to pull out the right value.
            /// 
			commandText += String.Format("(tableIndex > {0}) and (tableIndex <= {1})", index, index + length);             
            command.CommandText = commandText;
            SQLiteDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                object [] valueArray = new object[reader.FieldCount];
                reader.GetValues(valueArray);
                T datum = new T();                
                for(int j = 0; j < reader.FieldCount; j++)
                {
                    PropertyInfo property = marr_properties[j] as PropertyInfo;                    
                    property.SetValue(datum, Convert.ChangeType(valueArray[j], property.PropertyType), BindingFlags.SetProperty, null, null, null);                    
                }
                data[i] = datum;
                i++;
            }
            reader.Close();
            reader.Dispose();
        }

        public void DropAllRowsFrom(long index)
        {
            SQLiteCommand command = mobj_connection.CreateCommand();
            string commandText = "DELETE FROM " + mstr_name + " WHERE ";
            /// 
            /// Indexes in the database are offset 1, so 0 C# index = 1 db, 1 C#  = 2 db.  Use
            /// <= for the high index to pull out the right value.
            /// 
			commandText += String.Format("(tableIndex > {0})", index);            
            command.CommandText = commandText;
            command.ExecuteNonQuery();           
            
            //command.Dispose();
        }

        /// <summary>
        /// Resizes the cache based off the array.
        /// </summary>
        /// <param name="arrayLength"></param>
        public override void Resize(long arrayLength)
        {
            if (arrayLength == mlng_arrayLength)
                return;

            if (arrayLength <= 0)
                throw new Exception("Array length must be greater than zero.");

            if (mlng_arrayLength > arrayLength)
            {
                DropAllRowsFrom(arrayLength);
            }
            else
            {
                Fill(arrayLength - mlng_arrayLength);
            }

            base.Resize(arrayLength);
        }


        #region IDisposable Members
        /// <summary>
        /// Handles disposing of object and base.
        /// </summary>
        public override void Dispose()
        {
            
            if (mobj_connection == null)
                return;
            
            /// 
            /// Dispose the base - let it enforce the caching for us.
            /// 
            base.Dispose();

            /// 
            /// And close the database connections
            /// 
            if (mobj_connection.State != ConnectionState.Closed ||
                mobj_connection.State != ConnectionState.Broken)
            {                
             //   mobj_connection.Close();                
            }
            mobj_connection.Dispose();
        }

        #endregion
    }


     public class InvalidTypeException : Exception
    {
        private string mstr_message;
        public InvalidTypeException(string message)
        {
            mstr_message = message;
        }

        public override string Message
        {
            get
            {
                return mstr_message;
            }
        }
    }

    public class InvalidDataCacheModeException : Exception
    {
        private string mstr_message;
        public InvalidDataCacheModeException(string message)
        {
            mstr_message = message;
        }

        public override string Message
        {
            get
            {
                return mstr_message;
            }
        }
    }


    public class InvalidArraySizeException : Exception
    {
        private string mstr_message;
        public InvalidArraySizeException(string message)
        {
            mstr_message = message;
        }

        public override string Message
        {
            get
            {
                return mstr_message;
            }
        }
    }
}
