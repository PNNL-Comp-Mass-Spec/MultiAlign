using System;
using System.Windows.Forms;

using PNNLProteomics.Data.Factors;
using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin.Forms
{
    public partial class FactorValueFilterControl : UserControl
    {
        /// <summary>
        /// Displays factor information for a given factor values.
        /// </summary>
        /// <param name="analysis"></param>
        public FactorValueFilterControl(MultiAlignAnalysis analysis, FactorInformation information)
        {
            InitializeComponent();
            UpdateUserInterface(information);
        }

        private void UpdateUserInterface(FactorInformation information)
        {
            
        }
    }
}
