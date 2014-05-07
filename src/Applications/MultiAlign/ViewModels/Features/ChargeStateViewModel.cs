using PNNLOmics.Annotations;
using PNNLOmicsViz.Drawing;
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


            var iterator  = new ColorTypeIterator();
            var oldColor  = iterator.GetColor(charge);
            var newColor  = new Color
            {
                A = oldColor.A,
                R = oldColor.R,
                G = oldColor.G,
                B = oldColor.B
            };
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
        [UsedImplicitly]
        public string ToolText
        {
            get
            {
                return string.Format("{0} m/z ({1} - {2})", Mz, ScanStart, ScanEnd);
            }
        }
    }
}
