using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MultiAlignWin.Diagnostics
{
    public partial class frmException : Form
    {
        /// <summary>
        /// Default Constructor for the exception class.
        /// </summary>
        public frmException()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets message exception text.
        /// </summary>
        public string ExceptionMessage
        {
            get
            {
                return txtExceptionMessage.Text;
            }
            set
            {
                txtExceptionMessage.Text = value;
            }
        }     
    }

    
}