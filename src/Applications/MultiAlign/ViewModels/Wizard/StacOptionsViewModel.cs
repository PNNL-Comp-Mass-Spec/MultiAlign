using MultiAlignCore.Algorithms.Options;

namespace MultiAlign.ViewModels.Wizard
{
    public class StacOptionsViewModel : ViewModelBase
    {
        private readonly StacOptions m_options;

        public StacOptionsViewModel(StacOptions options)
        {
           
            m_options = options;
        }

        public bool UsePriors
        {
            get { return m_options.UsePriors; }
            set
            {
                m_options.UsePriors = value;
                OnPropertyChanged("UsePriors");
            }
        }

        public double HistogramBinWidth
        {
            get { return m_options.HistogramBinWidth; }
            set
            {
                m_options.HistogramBinWidth = value;
                OnPropertyChanged("HistogramBinWidth");
            }
        }

        public double HistogramMultiplier
        {
            get { return m_options.HistogramMultiplier; }
            set
            {
                m_options.HistogramMultiplier = value;
                OnPropertyChanged("HistogramMultiplier");
            }
        }

        public double ShiftAmount
        {
            get { return m_options.ShiftAmount; }
            set
            {
                m_options.ShiftAmount = value;
                OnPropertyChanged("ShiftAmount");
            }
        }

        public bool ShouldCalculateHistogramFdr
        {
            get { return m_options.ShouldCalculateHistogramFDR; }
            set
            {
                m_options.ShouldCalculateHistogramFDR = value;
                OnPropertyChanged("ShouldCalculateHistogramFdr");
            }
        }

        public bool ShouldCalculateShiftFdr
        {
            get { return m_options.ShouldCalculateShiftFDR; }
            set
            {
                m_options.ShouldCalculateShiftFDR = value;
                OnPropertyChanged("ShouldCalculateShiftFdr");
            }
        }

        public bool ShouldCalculateSliC
        {
            get { return m_options.ShouldCalculateSLiC; }
            set
            {
                m_options.ShouldCalculateSLiC = value;
                OnPropertyChanged("ShouldCalculateSliC");
            }
        }

        public bool ShouldCalculateStac
        {
            get { return m_options.ShouldCalculateSTAC; }
            set
            {
                m_options.ShouldCalculateSTAC = value;
                OnPropertyChanged("ShouldCalculateStac");
            }
        }

        public bool UseDriftTime
        {
            get { return m_options.UseDriftTime; }
            set
            {
                m_options.UseDriftTime = value;
                OnPropertyChanged("UseDriftTime");
            }
        }

        public bool UseEllipsoid
        {
            get { return m_options.UseEllipsoid; }
            set
            {
                m_options.UseEllipsoid = value;
                OnPropertyChanged("UseEllipsoid");
            }
        }

        public bool Refined
        {
            get { return m_options.Refined; }
            set
            {
                m_options.Refined = value;
                OnPropertyChanged("Refined");
            }
        }

        public double DriftTimeTolerance
        {
            get { return m_options.DriftTimeTolerance; }
            set
            {
                m_options.DriftTimeTolerance = value;
                OnPropertyChanged("DriftTimeTolerance");
            }
        }

        public double MassTolerancePpm
        {
            get { return m_options.MassTolerancePPM; }
            set
            {
                m_options.MassTolerancePPM = value;
                OnPropertyChanged("MassTolerancePpm");
            }
        }

        public double NetTolerance
        {
            get { return m_options.NETTolerance; }
            set
            {
                m_options.NETTolerance = value;
                OnPropertyChanged("NetTolerance");
            }
        }
    }
}