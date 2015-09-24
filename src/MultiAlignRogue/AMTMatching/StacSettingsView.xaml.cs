using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiAlignRogue.AMTMatching
{
    /// <summary>
    /// Interaction logic for StacSettingsView.xaml
    /// </summary>
    public partial class StacSettingsView : UserControl
    {
        public StacSettingsView()
        {
            InitializeComponent();

            this.Loaded += (o, e) => this.SetAmtTagRowHeight();
            this.AmtTagDatabaseProgressBar.IsVisibleChanged += (o, e) => this.SetAmtTagRowHeight();
        }

        private void SetAmtTagRowHeight()
        {
            if (this.AmtTagDatabaseProgressBar.Visibility == Visibility.Visible)
            {
                this.AmtTagDatabaseRow.Height = new GridLength(85, GridUnitType.Pixel);
            }
            else
            {
                this.AmtTagDatabaseRow.Height = new GridLength(55, GridUnitType.Pixel);
            }
        }
    }
}
