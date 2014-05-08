using System.Windows;
using MultiAlign.ViewModels.Datasets;
using MultiAlign.Windows.Viewers.Datasets;

namespace MultiAlign.Commands.Datasets
{
    /// <summary>
    ///     Handles showing the details of a particular dataset.
    /// </summary>
    public sealed class ShowDatasetDetailCommand : BaseCommand
    {
        public ShowDatasetDetailCommand() :
            base(null, AlwaysPass)
        {
        }

        /// <summary>
        ///     Displays a window for modifying information about a dataset.
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            var information = parameter as DatasetInformationViewModel;

            if (information == null) return;
            var newWindow = new Window {Width = 650, Height = 350};

            var viewer = new DatasetInputFileEditor {DataContext = information};
            newWindow.Content = viewer;
            newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            newWindow.ShowDialog();
        }
    }
}