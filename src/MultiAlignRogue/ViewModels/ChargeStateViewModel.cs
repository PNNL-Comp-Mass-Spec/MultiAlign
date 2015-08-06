using GalaSoft.MvvmLight;

namespace MultiAlignRogue.ViewModels
{
    public class ChargeStateViewModel : ViewModelBase
    {
        /// <summary>
        /// The charge state.
        /// </summary>
        private int charge;

        /// <summary>
        /// A value indicating whether this charge state has been selected.
        /// </summary>
        private bool selected;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChargeStateViewModel"/> class.
        /// </summary>
        /// <param name="chargeState">The charge state.</param>
        public ChargeStateViewModel(int chargeState)
        {
            this.ChargeState = chargeState;
            this.selected = true;
        }

        /// <summary>
        /// Gets the charge state.
        /// </summary>
        public int ChargeState
        {
            get { return this.charge; }
            private set
            {
                if (this.charge != value)
                {
                    this.charge = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this charge state has been selected.
        /// </summary>
        public bool Selected
        {
            get { return this.selected; }
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;
                    this.RaisePropertyChanged("Selected", !value, value, true);
                }
            }
        }

        /// <summary>
        /// Label for charge state.
        /// </summary>
        public string ChargeLabel
        {
            get { return string.Format("Charge {0}{1}", this.charge, this.charge > 0 ? "+" : "-"); }
        }
    }
}
