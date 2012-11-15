using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

using System.Linq;
using System.Text;

namespace Manassa.Data
{
    /// <summary>
    /// Class that holds information about a recent analysis.
    /// </summary>
    public class RecentAnalysis: DependencyObject
    {

        /// <summary>
        /// Gets or sets the path of the analysis.
        /// </summary>
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(RecentAnalysis));



        /// <summary>
        /// Gets or sets the name of the analysis.
        /// </summary>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(RecentAnalysis));


    }
}
