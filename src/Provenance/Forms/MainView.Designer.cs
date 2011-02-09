namespace FOX
{
    partial class MainView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PNNLControls.PenProvider penProvider9 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider10 = new PNNLControls.PenProvider();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            PNNLControls.PenProvider penProvider1 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider2 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider3 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider4 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider5 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider6 = new PNNLControls.PenProvider();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.m_distanceMapPanel = new System.Windows.Forms.Panel();
            this.m_histogramTab = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.m_distanceHistogram = new PNNLControls.Drawing.Charting.ctlHistogram();
            this.m_massHistogram = new PNNLControls.Drawing.Charting.ctlHistogram();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_NETHistogram = new PNNLControls.Drawing.Charting.ctlHistogram();
            this.m_driftTimeHistogram = new PNNLControls.Drawing.Charting.ctlHistogram();
            this.fileMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_statusStrip = new System.Windows.Forms.StatusStrip();
            this.m_statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.m_histogramTab.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_distanceHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_massHistogram)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_NETHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_driftTimeHistogram)).BeginInit();
            this.fileMenuStrip.SuspendLayout();
            this.m_statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.m_histogramTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(912, 565);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.m_distanceMapPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(882, 539);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Distance Map";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // m_distanceMapPanel
            // 
            this.m_distanceMapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_distanceMapPanel.Location = new System.Drawing.Point(8, 6);
            this.m_distanceMapPanel.Name = "m_distanceMapPanel";
            this.m_distanceMapPanel.Size = new System.Drawing.Size(739, 521);
            this.m_distanceMapPanel.TabIndex = 0;
            // 
            // m_histogramTab
            // 
            this.m_histogramTab.Controls.Add(this.panel2);
            this.m_histogramTab.Controls.Add(this.panel1);
            this.m_histogramTab.Location = new System.Drawing.Point(4, 22);
            this.m_histogramTab.Name = "m_histogramTab";
            this.m_histogramTab.Padding = new System.Windows.Forms.Padding(3);
            this.m_histogramTab.Size = new System.Drawing.Size(904, 539);
            this.m_histogramTab.TabIndex = 1;
            this.m_histogramTab.Text = "Histograms";
            this.m_histogramTab.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.m_distanceHistogram);
            this.panel2.Controls.Add(this.m_massHistogram);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(446, 533);
            this.panel2.TabIndex = 6;
            // 
            // m_distanceHistogram
            // 
            this.m_distanceHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_distanceHistogram.AutoSizeFonts = false;
            this.m_distanceHistogram.AutoViewPortXBase = 0F;
            this.m_distanceHistogram.AutoViewPortYBase = 0F;
            this.m_distanceHistogram.AxisAndLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.m_distanceHistogram.AxisAndLabelMaxFontSize = 15;
            this.m_distanceHistogram.AxisAndLabelMinFontSize = 8;
            this.m_distanceHistogram.AxisVisible = true;
            this.m_distanceHistogram.BinSize = 1F;
            this.m_distanceHistogram.ChartBackgroundColor = System.Drawing.Color.White;
            this.m_distanceHistogram.ChartLayout.LegendFraction = 0.2F;
            this.m_distanceHistogram.ChartLayout.LegendLocation = PNNLControls.ChartLegendLocation.Right;
            this.m_distanceHistogram.ChartLayout.MaxLegendHeight = 150;
            this.m_distanceHistogram.ChartLayout.MaxLegendWidth = 250;
            this.m_distanceHistogram.ChartLayout.MaxTitleHeight = 50;
            this.m_distanceHistogram.ChartLayout.MinLegendHeight = 50;
            this.m_distanceHistogram.ChartLayout.MinLegendWidth = 75;
            this.m_distanceHistogram.ChartLayout.MinTitleHeight = 15;
            this.m_distanceHistogram.ChartLayout.TitleFraction = 0.1F;
            this.m_distanceHistogram.DefaultZoomHandler.Active = true;
            this.m_distanceHistogram.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.m_distanceHistogram.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            penProvider9.Color = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            penProvider9.Width = 1F;
            this.m_distanceHistogram.GridLinePen = penProvider9;
            this.m_distanceHistogram.HasLegend = false;
            this.m_distanceHistogram.HilightColor = System.Drawing.Color.Magenta;
            this.m_distanceHistogram.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider10.Color = System.Drawing.Color.Black;
            penProvider10.Width = 1F;
            this.m_distanceHistogram.Legend.BorderPen = penProvider10;
            this.m_distanceHistogram.Legend.Bounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.m_distanceHistogram.Legend.ColumnWidth = 125;
            this.m_distanceHistogram.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.m_distanceHistogram.Legend.MaxFontSize = 12F;
            this.m_distanceHistogram.Legend.MinFontSize = 6F;
            this.m_distanceHistogram.LegendVisible = false;
            this.m_distanceHistogram.Location = new System.Drawing.Point(3, 262);
            this.m_distanceHistogram.Margins.BottomMarginFraction = 0.1F;
            this.m_distanceHistogram.Margins.BottomMarginMax = 72;
            this.m_distanceHistogram.Margins.BottomMarginMin = 30;
            this.m_distanceHistogram.Margins.DefaultMarginFraction = 0.05F;
            this.m_distanceHistogram.Margins.DefaultMarginMax = 15;
            this.m_distanceHistogram.Margins.DefaultMarginMin = 5;
            this.m_distanceHistogram.Margins.LeftMarginFraction = 0.2F;
            this.m_distanceHistogram.Margins.LeftMarginMax = 150;
            this.m_distanceHistogram.Margins.LeftMarginMin = 72;
            this.m_distanceHistogram.Name = "m_distanceHistogram";
            this.m_distanceHistogram.PadViewPortX = 0F;
            this.m_distanceHistogram.PadViewPortY = 0F;
            this.m_distanceHistogram.Size = new System.Drawing.Size(425, 258);
            this.m_distanceHistogram.TabIndex = 3;
            this.m_distanceHistogram.Title = "Euclidean Distance";
            this.m_distanceHistogram.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_distanceHistogram.TitleMaxFontSize = 15F;
            this.m_distanceHistogram.TitleMinFontSize = 6F;
            this.m_distanceHistogram.TitleVisible = true;
            this.m_distanceHistogram.VerticalExpansion = 1F;
            this.m_distanceHistogram.ViewPort = ((System.Drawing.RectangleF)(resources.GetObject("m_distanceHistogram.ViewPort")));
            this.m_distanceHistogram.XAxisLabel = "";
            this.m_distanceHistogram.YAxisLabel = "Frequency";
            // 
            // m_massHistogram
            // 
            this.m_massHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_massHistogram.AutoSizeFonts = false;
            this.m_massHistogram.AutoViewPortXBase = 0F;
            this.m_massHistogram.AutoViewPortYBase = 0F;
            this.m_massHistogram.AxisAndLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.m_massHistogram.AxisAndLabelMaxFontSize = 15;
            this.m_massHistogram.AxisAndLabelMinFontSize = 8;
            this.m_massHistogram.AxisVisible = true;
            this.m_massHistogram.BinSize = 1F;
            this.m_massHistogram.ChartBackgroundColor = System.Drawing.Color.White;
            this.m_massHistogram.ChartLayout.LegendFraction = 0.2F;
            this.m_massHistogram.ChartLayout.LegendLocation = PNNLControls.ChartLegendLocation.Right;
            this.m_massHistogram.ChartLayout.MaxLegendHeight = 150;
            this.m_massHistogram.ChartLayout.MaxLegendWidth = 250;
            this.m_massHistogram.ChartLayout.MaxTitleHeight = 50;
            this.m_massHistogram.ChartLayout.MinLegendHeight = 50;
            this.m_massHistogram.ChartLayout.MinLegendWidth = 75;
            this.m_massHistogram.ChartLayout.MinTitleHeight = 15;
            this.m_massHistogram.ChartLayout.TitleFraction = 0.1F;
            this.m_massHistogram.DefaultZoomHandler.Active = true;
            this.m_massHistogram.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.m_massHistogram.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            penProvider1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            penProvider1.Width = 1F;
            this.m_massHistogram.GridLinePen = penProvider1;
            this.m_massHistogram.HasLegend = false;
            this.m_massHistogram.HilightColor = System.Drawing.Color.Magenta;
            this.m_massHistogram.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider2.Color = System.Drawing.Color.Black;
            penProvider2.Width = 1F;
            this.m_massHistogram.Legend.BorderPen = penProvider2;
            this.m_massHistogram.Legend.Bounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.m_massHistogram.Legend.ColumnWidth = 125;
            this.m_massHistogram.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.m_massHistogram.Legend.MaxFontSize = 12F;
            this.m_massHistogram.Legend.MinFontSize = 6F;
            this.m_massHistogram.LegendVisible = false;
            this.m_massHistogram.Location = new System.Drawing.Point(5, 3);
            this.m_massHistogram.Margins.BottomMarginFraction = 0.1F;
            this.m_massHistogram.Margins.BottomMarginMax = 72;
            this.m_massHistogram.Margins.BottomMarginMin = 30;
            this.m_massHistogram.Margins.DefaultMarginFraction = 0.05F;
            this.m_massHistogram.Margins.DefaultMarginMax = 15;
            this.m_massHistogram.Margins.DefaultMarginMin = 5;
            this.m_massHistogram.Margins.LeftMarginFraction = 0.2F;
            this.m_massHistogram.Margins.LeftMarginMax = 150;
            this.m_massHistogram.Margins.LeftMarginMin = 72;
            this.m_massHistogram.Name = "m_massHistogram";
            this.m_massHistogram.PadViewPortX = 0F;
            this.m_massHistogram.PadViewPortY = 0F;
            this.m_massHistogram.Size = new System.Drawing.Size(435, 253);
            this.m_massHistogram.TabIndex = 4;
            this.m_massHistogram.Title = "Mass Histogram";
            this.m_massHistogram.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_massHistogram.TitleMaxFontSize = 50F;
            this.m_massHistogram.TitleMinFontSize = 6F;
            this.m_massHistogram.TitleVisible = true;
            this.m_massHistogram.VerticalExpansion = 1F;
            this.m_massHistogram.ViewPort = ((System.Drawing.RectangleF)(resources.GetObject("m_massHistogram.ViewPort")));
            this.m_massHistogram.XAxisLabel = "Mass (PPM)";
            this.m_massHistogram.YAxisLabel = "Frequency";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_NETHistogram);
            this.panel1.Controls.Add(this.m_driftTimeHistogram);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(449, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(452, 533);
            this.panel1.TabIndex = 5;
            // 
            // m_NETHistogram
            // 
            this.m_NETHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_NETHistogram.AutoSizeFonts = false;
            this.m_NETHistogram.AutoViewPortXBase = 0F;
            this.m_NETHistogram.AutoViewPortYBase = 0F;
            this.m_NETHistogram.AxisAndLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.m_NETHistogram.AxisAndLabelMaxFontSize = 15;
            this.m_NETHistogram.AxisAndLabelMinFontSize = 8;
            this.m_NETHistogram.AxisVisible = true;
            this.m_NETHistogram.BinSize = 1F;
            this.m_NETHistogram.ChartBackgroundColor = System.Drawing.Color.White;
            this.m_NETHistogram.ChartLayout.LegendFraction = 0.2F;
            this.m_NETHistogram.ChartLayout.LegendLocation = PNNLControls.ChartLegendLocation.Right;
            this.m_NETHistogram.ChartLayout.MaxLegendHeight = 150;
            this.m_NETHistogram.ChartLayout.MaxLegendWidth = 250;
            this.m_NETHistogram.ChartLayout.MaxTitleHeight = 50;
            this.m_NETHistogram.ChartLayout.MinLegendHeight = 50;
            this.m_NETHistogram.ChartLayout.MinLegendWidth = 75;
            this.m_NETHistogram.ChartLayout.MinTitleHeight = 15;
            this.m_NETHistogram.ChartLayout.TitleFraction = 0.1F;
            this.m_NETHistogram.DefaultZoomHandler.Active = true;
            this.m_NETHistogram.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.m_NETHistogram.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            penProvider3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            penProvider3.Width = 1F;
            this.m_NETHistogram.GridLinePen = penProvider3;
            this.m_NETHistogram.HasLegend = false;
            this.m_NETHistogram.HilightColor = System.Drawing.Color.Magenta;
            this.m_NETHistogram.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider4.Color = System.Drawing.Color.Black;
            penProvider4.Width = 1F;
            this.m_NETHistogram.Legend.BorderPen = penProvider4;
            this.m_NETHistogram.Legend.Bounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.m_NETHistogram.Legend.ColumnWidth = 125;
            this.m_NETHistogram.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.m_NETHistogram.Legend.MaxFontSize = 12F;
            this.m_NETHistogram.Legend.MinFontSize = 6F;
            this.m_NETHistogram.LegendVisible = false;
            this.m_NETHistogram.Location = new System.Drawing.Point(6, 3);
            this.m_NETHistogram.Margins.BottomMarginFraction = 0.1F;
            this.m_NETHistogram.Margins.BottomMarginMax = 72;
            this.m_NETHistogram.Margins.BottomMarginMin = 30;
            this.m_NETHistogram.Margins.DefaultMarginFraction = 0.05F;
            this.m_NETHistogram.Margins.DefaultMarginMax = 15;
            this.m_NETHistogram.Margins.DefaultMarginMin = 5;
            this.m_NETHistogram.Margins.LeftMarginFraction = 0.2F;
            this.m_NETHistogram.Margins.LeftMarginMax = 150;
            this.m_NETHistogram.Margins.LeftMarginMin = 72;
            this.m_NETHistogram.Name = "m_NETHistogram";
            this.m_NETHistogram.PadViewPortX = 0F;
            this.m_NETHistogram.PadViewPortY = 0F;
            this.m_NETHistogram.Size = new System.Drawing.Size(443, 253);
            this.m_NETHistogram.TabIndex = 1;
            this.m_NETHistogram.Title = "NET Histogram";
            this.m_NETHistogram.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_NETHistogram.TitleMaxFontSize = 50F;
            this.m_NETHistogram.TitleMinFontSize = 6F;
            this.m_NETHistogram.TitleVisible = true;
            this.m_NETHistogram.VerticalExpansion = 1F;
            this.m_NETHistogram.ViewPort = ((System.Drawing.RectangleF)(resources.GetObject("m_NETHistogram.ViewPort")));
            this.m_NETHistogram.XAxisLabel = "NET";
            this.m_NETHistogram.YAxisLabel = "Frequency";
            // 
            // m_driftTimeHistogram
            // 
            this.m_driftTimeHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_driftTimeHistogram.AutoSizeFonts = false;
            this.m_driftTimeHistogram.AutoViewPortXBase = 0F;
            this.m_driftTimeHistogram.AutoViewPortYBase = 0F;
            this.m_driftTimeHistogram.AxisAndLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.m_driftTimeHistogram.AxisAndLabelMaxFontSize = 15;
            this.m_driftTimeHistogram.AxisAndLabelMinFontSize = 8;
            this.m_driftTimeHistogram.AxisVisible = true;
            this.m_driftTimeHistogram.BinSize = 1F;
            this.m_driftTimeHistogram.ChartBackgroundColor = System.Drawing.Color.White;
            this.m_driftTimeHistogram.ChartLayout.LegendFraction = 0.2F;
            this.m_driftTimeHistogram.ChartLayout.LegendLocation = PNNLControls.ChartLegendLocation.Right;
            this.m_driftTimeHistogram.ChartLayout.MaxLegendHeight = 150;
            this.m_driftTimeHistogram.ChartLayout.MaxLegendWidth = 250;
            this.m_driftTimeHistogram.ChartLayout.MaxTitleHeight = 50;
            this.m_driftTimeHistogram.ChartLayout.MinLegendHeight = 50;
            this.m_driftTimeHistogram.ChartLayout.MinLegendWidth = 75;
            this.m_driftTimeHistogram.ChartLayout.MinTitleHeight = 15;
            this.m_driftTimeHistogram.ChartLayout.TitleFraction = 0.1F;
            this.m_driftTimeHistogram.DefaultZoomHandler.Active = true;
            this.m_driftTimeHistogram.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.m_driftTimeHistogram.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            penProvider5.Color = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            penProvider5.Width = 1F;
            this.m_driftTimeHistogram.GridLinePen = penProvider5;
            this.m_driftTimeHistogram.HasLegend = false;
            this.m_driftTimeHistogram.HilightColor = System.Drawing.Color.Magenta;
            this.m_driftTimeHistogram.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider6.Color = System.Drawing.Color.Black;
            penProvider6.Width = 1F;
            this.m_driftTimeHistogram.Legend.BorderPen = penProvider6;
            this.m_driftTimeHistogram.Legend.Bounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.m_driftTimeHistogram.Legend.ColumnWidth = 125;
            this.m_driftTimeHistogram.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.m_driftTimeHistogram.Legend.MaxFontSize = 12F;
            this.m_driftTimeHistogram.Legend.MinFontSize = 6F;
            this.m_driftTimeHistogram.LegendVisible = false;
            this.m_driftTimeHistogram.Location = new System.Drawing.Point(3, 262);
            this.m_driftTimeHistogram.Margins.BottomMarginFraction = 0.1F;
            this.m_driftTimeHistogram.Margins.BottomMarginMax = 72;
            this.m_driftTimeHistogram.Margins.BottomMarginMin = 30;
            this.m_driftTimeHistogram.Margins.DefaultMarginFraction = 0.05F;
            this.m_driftTimeHistogram.Margins.DefaultMarginMax = 15;
            this.m_driftTimeHistogram.Margins.DefaultMarginMin = 5;
            this.m_driftTimeHistogram.Margins.LeftMarginFraction = 0.2F;
            this.m_driftTimeHistogram.Margins.LeftMarginMax = 150;
            this.m_driftTimeHistogram.Margins.LeftMarginMin = 72;
            this.m_driftTimeHistogram.Name = "m_driftTimeHistogram";
            this.m_driftTimeHistogram.PadViewPortX = 0F;
            this.m_driftTimeHistogram.PadViewPortY = 0F;
            this.m_driftTimeHistogram.Size = new System.Drawing.Size(444, 268);
            this.m_driftTimeHistogram.TabIndex = 2;
            this.m_driftTimeHistogram.Title = "Drift Time Histogram";
            this.m_driftTimeHistogram.TitleFont = new System.Drawing.Font("Microsoft New Tai Lue", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_driftTimeHistogram.TitleMaxFontSize = 50F;
            this.m_driftTimeHistogram.TitleMinFontSize = 6F;
            this.m_driftTimeHistogram.TitleVisible = true;
            this.m_driftTimeHistogram.VerticalExpansion = 1F;
            this.m_driftTimeHistogram.ViewPort = ((System.Drawing.RectangleF)(resources.GetObject("m_driftTimeHistogram.ViewPort")));
            this.m_driftTimeHistogram.XAxisLabel = "Drift Time (ms)";
            this.m_driftTimeHistogram.YAxisLabel = "Frequency";
            // 
            // fileMenuStrip
            // 
            this.fileMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.fileMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.fileMenuStrip.Name = "fileMenuStrip";
            this.fileMenuStrip.Size = new System.Drawing.Size(912, 24);
            this.fileMenuStrip.TabIndex = 1;
            this.fileMenuStrip.Text = "File";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(104, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // m_statusStrip
            // 
            this.m_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_statusLabel,
            this.m_progressBar});
            this.m_statusStrip.Location = new System.Drawing.Point(0, 589);
            this.m_statusStrip.Name = "m_statusStrip";
            this.m_statusStrip.Size = new System.Drawing.Size(912, 22);
            this.m_statusStrip.TabIndex = 8;
            this.m_statusStrip.Text = "statusStrip1";
            // 
            // m_statusLabel
            // 
            this.m_statusLabel.Name = "m_statusLabel";
            this.m_statusLabel.Size = new System.Drawing.Size(42, 17);
            this.m_statusLabel.Text = "Ready.";
            // 
            // m_progressBar
            // 
            this.m_progressBar.Name = "m_progressBar";
            this.m_progressBar.Size = new System.Drawing.Size(400, 16);
            this.m_progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.m_progressBar.Visible = false;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 611);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.fileMenuStrip);
            this.Controls.Add(this.m_statusStrip);
            this.MainMenuStrip = this.fileMenuStrip;
            this.Name = "MainView";
            this.Text = "FOX";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.m_histogramTab.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_distanceHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_massHistogram)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_NETHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_driftTimeHistogram)).EndInit();
            this.fileMenuStrip.ResumeLayout(false);
            this.fileMenuStrip.PerformLayout();
            this.m_statusStrip.ResumeLayout(false);
            this.m_statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage m_histogramTab;
        private System.Windows.Forms.MenuStrip fileMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel m_distanceMapPanel;
        private System.Windows.Forms.Panel panel2;
        private PNNLControls.Drawing.Charting.ctlHistogram m_distanceHistogram;
        private PNNLControls.Drawing.Charting.ctlHistogram m_massHistogram;
        private System.Windows.Forms.Panel panel1;
        private PNNLControls.Drawing.Charting.ctlHistogram m_NETHistogram;
        private PNNLControls.Drawing.Charting.ctlHistogram m_driftTimeHistogram;
        private System.Windows.Forms.StatusStrip m_statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel m_statusLabel;
        private System.Windows.Forms.ToolStripProgressBar m_progressBar;
    }
}

