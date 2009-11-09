using System;
using System.ComponentModel;
using System.Windows.Forms;

using MultiAlignEngine.Features;

namespace MultiAlignWin.Forms
{
    public partial class formUMCData : Form
    {
        /// <summary>
        /// Form that display's a single UMC's data.
        /// </summary>
        /// <param name="umc"></param>
        public formUMCData(clsUMC umc)
        {
            InitializeComponent();

            mcontrol_umcData.DisplayUMCData(umc);
        }

        private void mbutton_ok_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}