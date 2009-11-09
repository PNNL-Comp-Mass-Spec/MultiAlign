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
    public class clsSQLiteCacheList<T>:  clsCacheArray<T> , IDisposable, ICollection<T> where T:  new()
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
        public clsSQLiteCacheList():
            base(DEFAULT_CACHE_LENGTH/*, 0*/)
        {
            InitClass();
            mobj_connection = null;            
        }

        /// <summary>
        /// Initializes a clsSQLiteCacheArray with the specified cache length.
        /// </summary>
        /// <param name="cacheLength">Specifies the length of the cache to use.</param>
        public clsSQLiteCacheList(long cacheLength):
            base(cacheLength/*, 0*/)
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
        public clsSQLiteCacheList(string databasePath, enmSQLiteDataCacheMode cacheCreateMode, long cacheLength):
            base(cacheLength/*, arrayLength*/)
        {            
            InitClass();

            /// If we need to fill the database, we do it with the default values of the class we are caching.
            T dummy = new T();
            
            switch(cacheCreateMode)
            {
                /// User wants to open an existing table.
                case enmSQLiteDataCacheMode.Open:

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
						mlng_arrayLength = GetTableLength();
                    }

                    break;
            }        
        }

        public clsSQLiteCacheList(SQLiteConnection connection, long cacheLength, long arrayLength):
            base(cacheLength/*, 0*/)
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
        /// Opens the database and executes the optimizing PRAGMA statements.
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

		/// <summary>
		/// Updates the cache to the current index using a transaction.
		/// </summary>
		/// <param name="data">Data to cache.</param>
		/// <param name="index">Index at which to start.</param>
		/// <param name="length">Number of objects past the index to cache.</param>
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
			using (SQLiteCommand command = mobj_connection.CreateCommand())
			{
				string commandText = "DELETE FROM " + mstr_name + " WHERE ";
				/// 
				/// Indexes in the database are offset 1, so 0 C# index = 1 db, 1 C#  = 2 db.  Use
				/// <= for the high index to pull out the right value.
				/// 
				commandText += String.Format("(tableIndex > {0})", index);
				command.CommandText = commandText;
				command.ExecuteNonQuery();
			}
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

		#region ICollection<T> Members

		/// <summary>
		/// Adds an item to the ICollection(T).
		/// </summary>
		/// <param name="item">The object to add to the ICollection(T).</param>
		public void Add(T item)
		{
			string commandText = CreateInsertionString(item);
			if (commandText == string.Empty)
				return;

			using (SQLiteCommand command = mobj_connection.CreateCommand())
			{
				command.CommandText = commandText;
				command.ExecuteNonQuery();
			}
			mlng_arrayLength++;
		}

		/// <summary>
		/// Removes all items from the ICollection(T).
		/// </summary>
		public void Clear()
		{
			using (SQLiteCommand command = mobj_connection.CreateCommand())
			{
				string commandText = "DELETE * FROM " + mstr_name;
				command.CommandText = commandText;
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Determines whether the ICollection(T) contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the ICollection(T).</param>
		/// <returns>Returns true if item is found in the ICollection(T); otherwise, false.</returns>
		public bool Contains(T item)
		{
			Type objectType = item.GetType();
			string selectCommandString = string.Empty;

			selectCommandString = "SELECT * FROM " + mstr_name + " WHERE ";

			List<string> columnValues = new List<string>();

			foreach (PropertyInfo property in marr_properties)
			{
				string tempString = string.Empty;
				tempString = property.Name + " = ";

				//string valueOf;
				object o = property.GetValue(item, BindingFlags.GetProperty, null, null, null);
				if (property.PropertyType == typeof(string))
				{
					tempString += "'" + o.ToString() + "'";
				}
				else
				{
					tempString += o.ToString();
				}

				columnValues.Add(tempString);
			}

			for (int i = 0; i < columnValues.Count; i++)
			{
				selectCommandString += columnValues[i];

				if (i != columnValues.Count)
				{ selectCommandString += " AND "; }
			}

			bool result = false;
			using (SQLiteCommand command = mobj_connection.CreateCommand())
			{
				command.CommandText = selectCommandString;
				command.ExecuteNonQuery();

				using (SQLiteDataReader reader = command.ExecuteReader())
				{
					if (reader.Read())
						result = true;
					else
						result = false;

					reader.Close();
				}
			}
			return result;
		}

		/// <summary>
		/// NOT IMPLEMENTED: Supposed to copy the elements of the ICollection(T) to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection(T). The Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the number of elements contained in the ICollection(T).
		/// </summary>
		public int Count
		{
			get
			{
				int temp = -1;
				using (SQLiteCommand command = mobj_connection.CreateCommand())
				{
					string commandText = "SELECT COUNT(*) FROM " + mstr_name;
					command.CommandText = commandText;
					command.ExecuteNonQuery();

					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						temp = (int)reader[0];
						reader.Close();
					}
				}
				return temp;
			}
		}

		/// <summary>
		/// NOT IMPLEMENTED: Supposed to get a value indicating whether the ICollection(T) is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the ICollection(T).
		/// </summary>
		/// <param name="item">The object to remove from the ICollection(T).</param>
		/// <returns>True if item was successfully removed from the ICollection(T); otherwise, false. This method also returns false if item is not found in the original ICollection(T).</returns>
		public bool Remove(T item)
		{
			Type objectType = item.GetType();
			string deleteCommandString = string.Empty;

			deleteCommandString = "DELETE FROM " + mstr_name + " WHERE ";

			long itemIndex = -1;
			List<string> columnValues = new List<string>();

			foreach (PropertyInfo property in marr_properties)
			{
				string tempString = string.Empty;
				tempString = property.Name + " = ";

				//gets the index of the item, to be able to change the index for each subsequent object
				if (property.Name == "tableIndex")
					itemIndex = (int)property.GetValue(item, BindingFlags.GetProperty, null, null, null);

				//string valueOf;
				object o = property.GetValue(item, BindingFlags.GetProperty, null, null, null);
				if (property.PropertyType == typeof(string))
				{
					tempString += "'" + o.ToString() + "'";
				}
				else
				{
					tempString += o.ToString();
				}

				columnValues.Add(tempString);
			}

			for (int i = 0; i < columnValues.Count; i++)
			{
				deleteCommandString += columnValues[i];

				if (i != columnValues.Count)
				{ deleteCommandString += " AND "; }
			}

			using (SQLiteCommand command = mobj_connection.CreateCommand())
			{
				command.CommandText = deleteCommandString;
				command.ExecuteNonQuery();
			}

			if (!Contains(item))
			{
				mlng_arrayLength--;

				using (SQLiteTransaction mytransaction = mobj_connection.BeginTransaction())
				{

					using (SQLiteCommand command = mobj_connection.CreateCommand())
					{
						command.CommandText = "UPDATE " + mstr_name + " SET tableIndex=@newTableIndex WHERE tableIndex=@oldTableIndex";
						command.Parameters.Add(new SQLiteParameter("@newTableIndex"));
						command.Parameters.Add(new SQLiteParameter("@oldTableIndex"));

						for (long i = 0; i < (mlng_arrayLength - itemIndex); i++)
						{
							command.Parameters["@newTableIndex"].Value = (itemIndex + i);
							command.Parameters["@oldTableIndex"].Value = (itemIndex + i + 1);
							command.ExecuteNonQuery();
						}
					}
					mytransaction.Commit();
				}

				
				return true;
			}
			else
				return false;
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
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
