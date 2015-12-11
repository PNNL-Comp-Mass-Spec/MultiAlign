using MultiAlignCore.Algorithms.Options;

namespace MultiAlignRogue.Utils
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using MultiAlignCore.Data;

    public class DataLoadingSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis analysis;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="analysis"></param>
        public DataLoadingSettingsViewModel(
            MultiAlignAnalysis analysis)
        {

            this.analysis = analysis;

            this.RestoreDefaultsCommand = new RelayCommand(this.RestoreDefaults);
        }

        public RelayCommand RestoreDefaultsCommand { get; private set; }


        public bool ShouldUseIsotopicFitFilter
        {
            get { return this.analysis.Options.DataLoadOptions.UseIsotopicFitFilter; }
            set
            {
                this.analysis.Options.DataLoadOptions.UseIsotopicFitFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumIsotopicFit
        {
            get { return this.analysis.Options.DataLoadOptions.MaximumIsotopicFit; }
            set
            {
                this.analysis.Options.DataLoadOptions.MaximumIsotopicFit = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseAbundance
        {
            get { return this.analysis.Options.DataLoadOptions.UseAbundanceFilter; }
            set
            {
                this.analysis.Options.DataLoadOptions.UseAbundanceFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinimumAbundance
        {
            get { return this.analysis.Options.DataLoadOptions.MinimumAbundance; }
            set
            {
                this.analysis.Options.DataLoadOptions.MinimumAbundance = value;
                this.RaisePropertyChanged();
            }
        }

        public double MaximumAbundance
        {
            get { return this.analysis.Options.DataLoadOptions.MaximumAbundance; }
            set
            {
                this.analysis.Options.DataLoadOptions.MaximumAbundance = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseLCScanFilter
        {
            get { return this.analysis.Options.DataLoadOptions.UseLCScanFilter; }
            set
            {
                this.analysis.Options.DataLoadOptions.UseLCScanFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public int MinimumLCScan
        {
            get { return this.analysis.Options.DataLoadOptions.MinimumLCScan; }
            set
            {
                this.analysis.Options.DataLoadOptions.MinimumLCScan = value;
                this.RaisePropertyChanged();
            }
        }

        public int MaximumLCScan
        {
            get { return this.analysis.Options.DataLoadOptions.MaximumLCScan; }
            set
            {
                this.analysis.Options.DataLoadOptions.MaximumLCScan = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShouldUseMaximumDataCountFilter
        {
            get { return this.analysis.Options.DataLoadOptions.UseMaximumDataCountFilter; }
            set
            {
                this.analysis.Options.DataLoadOptions.UseMaximumDataCountFilter = value;
                this.RaisePropertyChanged();
            }
        }

        public int MaximumPointsToLoad
        {
            get { return this.analysis.Options.DataLoadOptions.MaximumPointsToLoad; }
            set
            {
                this.analysis.Options.DataLoadOptions.MaximumPointsToLoad = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Reset the settings the default values
        /// </summary>
        public void RestoreDefaults()
        {
            var defaultOptions = new MultiAlignAnalysisOptions();

            ShouldUseIsotopicFitFilter = defaultOptions.DataLoadOptions.UseIsotopicFitFilter;
            MaximumIsotopicFit = defaultOptions.DataLoadOptions.MaximumIsotopicFit;

            ShouldUseAbundance = defaultOptions.DataLoadOptions.UseAbundanceFilter;
            MinimumAbundance = defaultOptions.DataLoadOptions.MinimumAbundance;
            MaximumAbundance = defaultOptions.DataLoadOptions.MaximumAbundance;

            ShouldUseLCScanFilter = defaultOptions.DataLoadOptions.UseLCScanFilter;
            MinimumLCScan = defaultOptions.DataLoadOptions.MinimumLCScan;
            MaximumLCScan = defaultOptions.DataLoadOptions.MaximumLCScan;

            ShouldUseMaximumDataCountFilter = defaultOptions.DataLoadOptions.UseMaximumDataCountFilter;
            MaximumPointsToLoad = defaultOptions.DataLoadOptions.MaximumPointsToLoad;

        }
    }
}
