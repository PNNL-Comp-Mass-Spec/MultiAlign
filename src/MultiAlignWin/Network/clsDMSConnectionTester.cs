/*////////////////////////////////////////////////////////////////////////
 *  File  : clsDMSConnectionTester.cs
 *  Author: Brian LaMarche
 *  Date  : 9/11/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      Class that tests the connection to DMS.
 *  Revisions:
 *      9-11-2008:
 *          - Creates a connection to DMS.
 */
///////////////////////////////////////////////////////////////////////	
using System;
using System.Text;
using System.Data;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

using MultiAlignCore.Data;

namespace MultiAlignWin.Network
{
    public delegate void DelegateConnectionToDMS(object sender, bool status);
    public delegate void DelegateConnectionToDMSMadePercent(object sender, double percentWaited);

    /// <summary>
    /// Class that tests a connection to DMS on the PNNL domain.
    /// </summary>
    public class clsDMSConnectionTester: IDisposable
    {
        /// <summary>
        /// Event that is triggered when a connection to DMS has been made or failed.
        /// </summary>
        public event DelegateConnectionToDMS            ConnectionStatus;
        /// <summary>
        /// Event fired periodically indicating how much time of the total timeout value has been spent waiting to make a connection to DMS.
        /// </summary>
        public event DelegateConnectionToDMSMadePercent ConnectionPercent;        
        /// <summary>
        /// Thread that attempts to make a connection to DMS.
        /// </summary>
        private Thread mobj_testingThread;
        /// <summary>
        /// Thread that handles the events and updates to the user.
        /// </summary>
        private Thread mobj_watchdogThread;
        /// <summary>
        /// String that contains the name of the DMS server.
        /// </summary>
        private clsDMSServerInformation mobj_serverInformation;        
        /// <summary>
        /// Flag indicating whether to keep testing the connection.
        /// </summary>
        private bool mbool_isTesting;
        /// <summary>
        /// Default constructor for a DMS connection tester.
        /// </summary>
        public clsDMSConnectionTester()
        {
            
            mobj_serverInformation  = new clsDMSServerInformation();
            mobj_testingThread      = null;
            mobj_watchdogThread     = null;
        }
        /// <summary>
        /// Constructor that sets whether a connection to DMS has been made.
        /// </summary>
        /// <param name="connectionMade"></param>
        public clsDMSConnectionTester(bool connectionMade, clsDMSServerInformation information)
        {                     
            mobj_serverInformation  = information;
            mobj_serverInformation.ConnectionExists = connectionMade;
            mobj_testingThread      = null;
            mobj_watchdogThread     = null;
        }
        /// <summary>
        /// Gets or sets the name of the DMS server.
        /// </summary>
        public clsDMSServerInformation ServerInformation
        {
            get
            {
                return mobj_serverInformation;
            }
            set
            {
                mobj_serverInformation = value;
            }
        }
        /// <summary>
        /// Aborts the testing thread called from the watchdog thread.
        /// </summary>
        private void AbortTestThread()
        {
            try
            {
                mobj_testingThread.Abort();
            }
            catch
            {

            }
            finally
            {
                mbool_isTesting     = false;
                mobj_testingThread  = null;
            }
        }
        /// <summary>
        /// Aborts testing and waiting for a connection to DMS.
        /// </summary>
        public void Abort()
        {
            if (mobj_watchdogThread == null && mobj_testingThread == null)
            {
                return;
            }

            try
            {                
                mobj_watchdogThread.Abort();
            }
            catch
            {
                
            }
            finally
            {
                AbortTestThread();        
                mobj_watchdogThread = null;
            }
        }
        /// <summary>
        /// Test the connection to DMS.
        /// </summary>
        public void TestConnection()
        {
            Abort();
            ThreadStart watchdogStart = new ThreadStart(WatchDogThread);
            mobj_watchdogThread = new Thread(watchdogStart);
            mobj_watchdogThread.Start();
        }
        /// <summary>
        /// Performs the user interface updates to the parent thread.
        /// </summary>
        private void WatchDogThread()
        {
            ThreadStart threadStart = new ThreadStart(ThreadedNetworkTest);            
            mobj_testingThread      = new Thread(threadStart);
            mbool_isTesting         = true;
            mobj_testingThread.Start();
            Thread.Sleep(100);

            int timeout             = mobj_serverInformation.ConnectionTimeout;

            DateTime startTime  = DateTime.Now;            
            TimeSpan timeSpan   = DateTime.Now.Subtract(startTime);
            double secondsLeft  = timeout - timeSpan.TotalSeconds;
            int percent         = 0;
            while (secondsLeft > 0.0 && mbool_isTesting == true)
            {
                Thread.Sleep(10);
                timeSpan    = DateTime.Now.Subtract(startTime);
                secondsLeft = timeout - timeSpan.TotalSeconds;
                percent     = Convert.ToInt32(100 * (Math.Abs(timeout - secondsLeft) / timeout));

                if (ConnectionPercent != null)
                    ConnectionPercent(this, percent);
            }

            /// 
            /// If there was a timeout the connection was not made.
            /// 
            if (secondsLeft <= 0.0)
                mobj_serverInformation.ConnectionExists = false;                                        

            if (ConnectionStatus != null)
            {
                ConnectionStatus(this, mobj_serverInformation.ConnectionExists);
            }                        
        }
        ///// <summary>
        ///// Threaded network testing.
        ///// </summary>
        private void ThreadedNetworkTest()
        {
            string connectionString = String.Format("database={0};server={1};user id={2};Password={3}",
                                                        mobj_serverInformation.DatabaseName,
                                                        mobj_serverInformation.ServerName,
                                                        mobj_serverInformation.Username,
                                                        mobj_serverInformation.Password);
                            
            SqlConnection myConnection      = new SqlConnection(connectionString);            
            try
            {
                myConnection.Open();
                myConnection.Close();
                myConnection.Dispose();
                mobj_serverInformation.ConnectionExists = true;
            }
            catch (Exception e)
            {
                mobj_serverInformation.ConnectionExists = false;
                Console.WriteLine(e.Message);                
            }
            finally
            {
                myConnection.Dispose();
                mbool_isTesting         = false;
            }
        }	
        /// <summary>
        /// Gets whether a connection to DMS has been made.
        /// </summary>
        public bool HasDMSConnection
        {
            get
            {
                return mobj_serverInformation.ConnectionExists;
            }        
        }
        #region IDisposable Members

        public void Dispose()
        {
            Abort();
        }

        #endregion
    }
}
