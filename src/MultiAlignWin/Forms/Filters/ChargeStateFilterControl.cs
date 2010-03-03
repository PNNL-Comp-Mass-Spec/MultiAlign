using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MultiAlignWin.Forms.Filters
{
    public partial class ChargeStateFilterControl : UserControl
    {
        public ChargeStateFilterControl()
        {
            InitializeComponent();

        }

        public override string ToString()
        {
            return "Charge State Filter";
        }
    }
}
