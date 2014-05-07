using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.Workspace;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCustomControls.Drawing;
using MultiAlignCore.IO;
using System;
using System.IO;
using MultiAlignCore;
using MultiAlign.Data.States;
using MultiAlign.Windows.Viewers;
using System.Linq;
using MultiAlign.ViewModels;
using MultiAlign.ViewModels.TreeView;
using System.Windows.Documents;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.Features;
using System.Collections.Generic;
using MultiAlign.ViewModels.Analysis;
using Microsoft.Win32;

namespace MultiAlign.Windows
{
    public partial class MainWindow : Window
    {               
        public MainWindow()
        {
            InitializeComponent();                            
        }      
    }
}
