using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using MultiAlignCore.Drawing;
using OxyPlot;

namespace MultiAlignRogue.Utils
{
    public class PlotSavingViewModel : ViewModelBase
    {
        private string path = string.Empty;
        private int width;
        private int height;
        private double dpi;
        private bool isQuadTree = false;
        private bool showAllData = false;

        private PlotModel plot;

        private PlotSavingViewModel(PlotModel plot, string defaultPath, int defaultWidth, int defaultHeight, bool isQuadTree = false)
        {
            this.plot = plot;
            this.Path = System.IO.Path.ChangeExtension(defaultPath, ".png");
            this.Width = defaultWidth;
            this.Height = defaultHeight;
            this.IsQuadTree = isQuadTree;
            this.DPI = 96;
            this.BrowseFilePathCommand = new RelayCommand(this.BrowseFilePathImpl);
            this.SaveCommand = new RelayCommand(this.SaveImpl,
                                                  () => !string.IsNullOrWhiteSpace(this.Path));
        }

        /// <summary>
        /// Gets a command that opens the file browser to select output file path.
        /// </summary>
        public RelayCommand BrowseFilePathCommand { get; private set; }

        /// <summary>
        /// Gets a command that creates the project.
        /// </summary>
        public RelayCommand SaveCommand { get; private set; }

        /// <summary>
        /// Gets a command that cancels the project creation.
        /// </summary>
        public RelayCommand CancelCommand { get; private set; }

        /// <summary>
        /// An event that is triggered when the user has clicked Create.
        /// </summary>
        public event EventHandler Complete;

        public static void SavePlot(PlotModel model, int width, int height, string defaultName, bool isQuadTree = false)
        {
            var psvm = new PlotSavingViewModel(model, defaultName, width, height, isQuadTree);

            var window = new PlotSaving
            {
                DataContext = psvm
            };

            psvm.Complete += (s, e) =>
            {
                window.Close();
            };
            window.ShowDialog();
        }

        public bool IsQuadTree
        {
            get { return this.isQuadTree; }
            private set
            {
                if (this.isQuadTree != value)
                {
                    this.isQuadTree = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged("ShowAllVisibility");
                }
            }
        }

        public Visibility ShowAllVisibility
        {
            get
            {
                if (this.isQuadTree)
                {
                    return Visibility.Visible;
                }
                return Visibility.Hidden;
            }
        }

        public int Width
        {
            get { return this.width; }
            private set
            {
                if (this.width != value)
                {
                    this.width = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int Height
        {
            get { return this.height; }
            private set
            {
                if (this.height != value)
                {
                    this.height = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double DPI
        {
            get { return this.dpi; }
            private set
            {
                if (!this.dpi.Equals(value))
                {
                    this.dpi = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Path
        {
            get { return this.path; }
            private set
            {
                if (!this.path.Equals(value))
                {
                    this.path = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Implementation for <see cref="BrowseFilePathCommand" />.
        /// Opens the file browser to select output file path.
        /// </summary>
        private void BrowseFilePathImpl()
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".png",
                Filter = @"Supported Files|*.png",
                AddExtension = true,
                CheckPathExists = true,
                FileName = this.Path,
            };

            var result = saveFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                if (!string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                {
                    this.Path = saveFileDialog.FileName;
                }
                else if (Directory.Exists(saveFileDialog.FileName))
                {
                    this.Path = System.IO.Path.Combine(saveFileDialog.FileName, this.Path);
                }
                if (!this.Path.ToLower().EndsWith(".png"))
                {
                    this.Path = System.IO.Path.ChangeExtension(this.Path, ".png");
                }
            }
        }

        /// <summary>
        /// Implementation for <see cref="SaveCommand" />.
        /// Creates the project.
        /// </summary>
        private void SaveImpl()
        {
            // Path verification
            var file = new FileInfo(Path);
            if (!Directory.Exists(file.DirectoryName))
            {
                Directory.CreateDirectory(file.DirectoryName);
            }

            // TODO: Save plot, according to settings - QuadTree stuff...

            var encoder = new PngPlotModelEncoder();
            var size = new Size(Width, Height);

            if (DPI.Equals(96))
            {
                encoder.SaveImage(this.plot, Path, size);
            }
            else
            {
                encoder.SaveImageHighRes(this.plot, Path, size, dpi);
            }

            if (this.Complete != null)
            {
                this.Complete(this, EventArgs.Empty);
            }
        }
    }
}
