using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestFactors.Factors
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 10; i++)
            {
                //MultiAlignWin.Forms.controlDatasetInformation d = new MultiAlignWin.Forms.controlDatasetInformation(null, null);
                //d.Dock = DockStyle.Top;
                //panel1.Controls.Add(d);
            }
        }
    }
}