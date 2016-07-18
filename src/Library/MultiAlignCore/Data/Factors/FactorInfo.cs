using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin.Data
{
    /// <summary>
    /// Summary description for clsFactorInfo.
    /// </summary>
    public class clsFactorInfo
    {
        public string strFactor;
        public ArrayList marrValues = new ArrayList();


        public clsFactorInfo()
        {
            //
            // TODO: Add constructor logic here
            //
            strFactor = null;
            marrValues.Clear();
        }

        public string[] FactorValues
        {
            get
            {
                string [] values = new string[marrValues.Count];
                if (marrValues.Count == 0)
                    return null;
                else
                {
                    for (int i = 0; i < marrValues.Count; i++)
                        values[i] = marrValues[i].ToString();
                    return values;
                }
            }
        }

        public int vCount
        {
            get {return marrValues.Count;}
        }
    }
}
