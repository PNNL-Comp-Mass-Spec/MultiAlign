using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;

namespace MultiAlignCore.IO.Factors
{
    class FactorFileReader: IBaseModule, IDisposable
    {
        #region Events
        public event EventHandler<MageColumnEventArgs>  ColumnDefAvailable;
        public event EventHandler<MageDataEventArgs>    DataRowAvailable;
        public event EventHandler<MageStatusEventArgs>  StatusMessageUpdated;
        #endregion

        public FactorFileReader()
        {
        }

        #region Cleanup Methods
        public void  Cancel()
        {
            // Pass
        }

        public void  Cleanup()
        {
            Dispose();
        }
        #endregion

        #region Properties
        public string  ModuleName
        {
            get;
            set;
        }
        public ProcessingPipeline Pipeline
        {
            get;
            set;
        }
        #endregion

        #region Module Methods
        public void  Prepare()
        {
            // Pass
        }
        public void  Run(object state)
        {

        }
        public void  SetParameters(Dictionary<string,string> parameters)
        {           throw new NotImplementedException();
        }

        public void  SetPropertyByName(string key, string val)
        {

        }
        #endregion

        #region ISinkModule Members
        public void  HandleColumnDef(object sender, MageColumnEventArgs args)
        {

        }
        public void  HandleDataRow(object sender, MageDataEventArgs args)
        {

        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {

        }
        #endregion

        #region IBaseModule Members


        public event EventHandler<MageStatusEventArgs> WarningMessageUpdated;

        #endregion
    }
}
