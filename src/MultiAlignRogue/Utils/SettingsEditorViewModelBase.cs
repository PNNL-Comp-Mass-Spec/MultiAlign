namespace MultiAlignRogue.Utils
{
    using System;
    using System.Reflection;

    using System.Windows.Input;

    using GalaSoft.MvvmLight;

    using MultiAlignCore.Data;

    using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

    /// <summary>
    /// This class is a base class for view models that are for altering settings.
    /// This contains a RestoreDefaults command that automatically updates all fields when executed.
    /// </summary>
    public class SettingsEditorViewModelBase : ViewModelBase
    {
        /// <summary>
        /// The settings container model that this view model edits.
        /// </summary>
        private readonly ISettingsContainer model;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsEditorViewModelBase" /> class. 
        /// </summary>
        /// <param name="model">The settings container model that this view model edits.</param>
        public SettingsEditorViewModelBase(ISettingsContainer model)
        {
            this.model = model;
            this.RestoreDefaultsCommand = new RelayCommand(this.RestoreDefaults);
        }

        /// <summary>
        /// Gets a command that restores all settings back to their original values.
        /// </summary>
        public ICommand RestoreDefaultsCommand { get; private set; }

        /// <summary>
        /// Updates all properties, call RaisePropertyChanged on all view model properties
        /// to ensure that the GUI updates.
        /// </summary>
        protected virtual void UpdateAll()
        {
            Type objType = this.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (var property in properties)
            {
                this.RaisePropertyChanged(property.Name);
            }
        }

        /// <summary>
        /// Implementation for <see cref="RestoreDefaultsCommand" />.
        /// Restores all settings back to their original values.
        /// </summary>
        private void RestoreDefaults()
        {
            if (this.model != null)
            {
                this.model.RestoreDefaults();
                this.UpdateAll();
            }
        }
    }
}
