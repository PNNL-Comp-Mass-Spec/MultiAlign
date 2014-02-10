using PNNLControls;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MultiAlign.ViewModels.Features
{
    public  class ChargeStateViewModel: ViewModelBase 
    {        

        public ChargeStateViewModel(int     charge,
                                    double  mz,
                                    int     scanStart,
                                    int     scanEnd)
        {
            clsColorIterator iterator      = new clsColorIterator();
            System.Drawing.Color oldColor  = iterator.GetColor(charge);
            Color newColor  = new System.Windows.Media.Color();
            newColor.A      = oldColor.A;
            newColor.R      = oldColor.R;
            newColor.G      = oldColor.G;
            newColor.B      = oldColor.B;
            ChargeColor     = new SolidColorBrush(newColor);   
            
            ChargeState     = charge;
            ScanStart       = scanStart;
            ScanEnd         = scanEnd;
            Mz              = mz;
        }
        public double Mz
        {
            get;
            private set;
        }
        public int   ChargeState
        {
            get;
            private set;  
        }
        public int ScanStart
        {
            get;
            private set;
        }
        public int ScanEnd
        {
            get;
            private set;
        }
        public Brush ChargeColor
        {
            get;
            private set;
        }
        public string ToolText
        {
            get
            {
                return string.Format("{0} m/z ({1} - {2})", Mz, ScanStart, ScanEnd);
            }
        }
    }
}
