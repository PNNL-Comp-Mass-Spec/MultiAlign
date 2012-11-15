using System.Windows;
using MultiAlignCore.Data;
using MultiAlignCore.IO.InputFiles;

namespace Manassa.Data
{
    /// <summary>
    /// Wraps the dataset information object for use within the UI
    /// </summary>
    public class DatasetWrapperModel: DependencyObject 
    {
        public DatasetWrapperModel()
        {
            Dataset = null;
        }

        public DatasetWrapperModel(InputFile file)
        {
            DatasetInformation info = new DatasetInformation();
            
        }
       
        public DatasetInformation  Dataset
        {
            get { return (DatasetInformation)GetValue(DatasetProperty); }
            set { SetValue(DatasetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dataset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatasetProperty =
            DependencyProperty.Register("Dataset", typeof(DatasetInformation), typeof(DatasetWrapperModel));        
    }
}

