/*////////////////////////////////////////////////////////////////////////
 *  File  : clsDMSServerInformation.cs
 *  Author: Brian LaMarche
 *  Date  : 9/11/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      Class that holds information about a connection to DMS.
 *  Revisions:
 *      9-11-2008:
 *          - Created file.
 */
///////////////////////////////////////////////////////////////////////

#region

using System.ComponentModel;

#endregion

namespace MultiAlignCore.Data.MetaData
{
    /// <summary>
    ///     Class that tests a connection to DMS on the PNNL domain.
    /// </summary>
    public class clsDMSServerInformation
    {
        /// <summary>
        ///     Flag indicating if a connection to DMS has been made.
        /// </summary>
        private bool mbool_hasConnectionToDMS;

        /// <summary>
        ///     String that contains the name of the DMS server.
        /// </summary>
        private string mstring_dmsServerName;

        /// <summary>
        ///     String that contains the name of the user to authenticate to DMS with.
        /// </summary>
        private string mstring_username;

        /// <summary>
        ///     String that contains the password to authenticate to DMS with.
        /// </summary>
        private string mstring_password;

        /// <summary>
        ///     Name of the database.
        /// </summary>
        private string mstring_databaseName;

        /// <summary>
        ///     The timeoutvalue for connecting to the data server.
        /// </summary>
        private int mint_connectionTimeout;

        /// <summary>
        ///     Default constructor for DMS server information
        /// </summary>
        public clsDMSServerInformation()
        {
        }

        /// <summary>
        ///     Copy constructor for DMS server information.
        /// </summary>
        /// <param name="copy"></param>
        public clsDMSServerInformation(clsDMSServerInformation copy)
        {
            mint_connectionTimeout = copy.ConnectionTimeout;
            mstring_dmsServerName = copy.ServerName;
            mstring_databaseName = copy.DatabaseName;
            mstring_username = copy.Username;
            mstring_password = copy.Password;
            mbool_hasConnectionToDMS = copy.ConnectionExists;
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the name of the DMS server.
        /// </summary>
        public string ServerName
        {
            get { return mstring_dmsServerName; }
            set { mstring_dmsServerName = value; }
        }

        /// <summary>
        ///     Gets or sets the username to connect to DMS with.
        /// </summary>
        [Browsable(false)]
        public string Username
        {
            get { return mstring_username; }
            set { mstring_username = value; }
        }

        /// <summary>
        ///     Gets or sets the password to connect to DMS with.
        /// </summary>
        [Browsable(false)]
        public string Password
        {
            get { return mstring_password; }
            set { mstring_password = value; }
        }

        /// <summary>
        ///     Gets or sets the timeout value for connecting to the database.
        /// </summary>
        public int ConnectionTimeout
        {
            get { return mint_connectionTimeout; }
            set { mint_connectionTimeout = value; }
        }

        /// <summary>
        ///     Gets or sets the database name.
        /// </summary>
        public string DatabaseName
        {
            get { return mstring_databaseName; }
            set { mstring_databaseName = value; }
        }

        /// <summary>
        ///     Gets or sets a flag indicating whether a connection at some point has been made and the settings are valid.
        /// </summary>
        public bool ConnectionExists
        {
            get { return mbool_hasConnectionToDMS; }
            set { mbool_hasConnectionToDMS = value; }
        }

        #endregion
    }
}