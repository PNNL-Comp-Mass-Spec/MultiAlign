using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiAlignCore.Data.MassTags;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for MassTagControl.xaml
    /// </summary>
    public partial class MassTagViewer : UserControl
    {
        private MassTagDatabase m_database;

        public MassTagViewer()
        {
            InitializeComponent();
        }

        public MassTagDatabase Database
        {
            get
            {
                return m_database; 
            }
            set
            {
                m_database = value;

                if (value != null)
                {
                    m_massTagGrid.MassTags = value.MassTags;
                    m_proteinGrid.Proteins = value.AllProteins;

                    m_massTagPlot.AddMassTags(value.MassTags);
                    m_massTagPlot.AutoViewPort();                    
                }                
            }
        }
    }
}
