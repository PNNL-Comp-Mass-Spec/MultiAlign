using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlPlotParamsEditor.
	/// </summary>
	public class ctlPlotParamsEditor : System.Windows.Forms.UserControl
	{
		private clsPlotParams mPlotParams;
		private PlotParamsChangedHandler mPlotParamsChangedHandler;
		private System.Windows.Forms.ComboBox mSizeCombo;
		private System.Windows.Forms.Label mSizeLabel;
		private System.Windows.Forms.TextBox mSeriesLabelTextBox;
		private System.Windows.Forms.Label mSeriesLabelLabel;
		private System.Windows.Forms.CheckBox mShow;
		private System.Windows.Forms.GroupBox mColorInterpretationGroupBox;
		private PNNLControls.pnlColorInterpolationEditor mColorInterpolaterEditor;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel mPreviewPanel;
		private System.Windows.Forms.TrackBar mPreviewZAxisTrackBar;
		private bool mRespondToEvents = true;
		private System.Windows.Forms.CheckBox mHollowCheckBox;
		private System.Windows.Forms.GroupBox mShapeGroupBox;
		private System.Windows.Forms.ComboBox mShapeComboBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox mPenGroupBox;
		//private PNNLControls.ctlPenProviderEditor mLinePenEditor;
		private System.Windows.Forms.GroupBox mLabelGroupBox;
		private PNNLControls.ctlPenProviderEditor mPenProviderEditor;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlPlotParamsEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			for (int i = 1; i <= 10; i++) 
			{
				this.mSizeCombo.Items.Add(i);
			}
			this.mShapeComboBox.Items.Add(new DiamondShapeCreater());
			this.mShapeComboBox.Items.Add(new SquareShapeCreater());
			this.mShapeComboBox.Items.Add(new BubbleShapeCreater());
			this.mShapeComboBox.Items.Add(new TriangleShapeCreater());
			this.mShapeComboBox.Items.Add(new PointShapeCreater());
			this.mShapeComboBox.Items.Add(new PlusShapeCreater());
			this.mShapeComboBox.Items.Add(new CrossShapeCreater());
			this.mShapeComboBox.Items.Add(new StarShapeCreater());
			this.mPlotParamsChangedHandler = new PlotParamsChangedHandler(this.PlotParamsChanged);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
					if (this.PlotParams != null) 
					{
						this.PlotParams.PlotParamsChanged -= this.mPlotParamsChangedHandler;
					}
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mHollowCheckBox = new System.Windows.Forms.CheckBox();
			this.mSizeCombo = new System.Windows.Forms.ComboBox();
			this.mSizeLabel = new System.Windows.Forms.Label();
			this.mSeriesLabelLabel = new System.Windows.Forms.Label();
			this.mSeriesLabelTextBox = new System.Windows.Forms.TextBox();
			this.mShow = new System.Windows.Forms.CheckBox();
			this.mColorInterpretationGroupBox = new System.Windows.Forms.GroupBox();
			this.mColorInterpolaterEditor = new PNNLControls.pnlColorInterpolationEditor();
			this.mPreviewPanel = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.mPreviewZAxisTrackBar = new System.Windows.Forms.TrackBar();
			this.mShapeGroupBox = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.mShapeComboBox = new System.Windows.Forms.ComboBox();
			this.mPenGroupBox = new System.Windows.Forms.GroupBox();
			this.mPenProviderEditor = new PNNLControls.ctlPenProviderEditor();
			this.mLabelGroupBox = new System.Windows.Forms.GroupBox();
			this.mColorInterpretationGroupBox.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mPreviewZAxisTrackBar)).BeginInit();
			this.mShapeGroupBox.SuspendLayout();
			this.mPenGroupBox.SuspendLayout();
			this.mLabelGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// mHollowCheckBox
			// 
			this.mHollowCheckBox.Location = new System.Drawing.Point(8, 80);
			this.mHollowCheckBox.Name = "mHollowCheckBox";
			this.mHollowCheckBox.Size = new System.Drawing.Size(144, 24);
			this.mHollowCheckBox.TabIndex = 10;
			this.mHollowCheckBox.Text = "Hollow";
			this.mHollowCheckBox.CheckedChanged += new System.EventHandler(this.mHollowCheckBox_CheckedChanged);
			// 
			// mSizeCombo
			// 
			this.mSizeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mSizeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mSizeCombo.Location = new System.Drawing.Point(56, 48);
			this.mSizeCombo.MaxDropDownItems = 10;
			this.mSizeCombo.Name = "mSizeCombo";
			this.mSizeCombo.Size = new System.Drawing.Size(104, 24);
			this.mSizeCombo.TabIndex = 9;
			this.mSizeCombo.SelectedIndexChanged += new System.EventHandler(this.mSizeCombo_SelectedIndexChanged);
			// 
			// mSizeLabel
			// 
			this.mSizeLabel.Location = new System.Drawing.Point(8, 48);
			this.mSizeLabel.Name = "mSizeLabel";
			this.mSizeLabel.Size = new System.Drawing.Size(48, 24);
			this.mSizeLabel.TabIndex = 8;
			this.mSizeLabel.Text = "Size:";
			this.mSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mSeriesLabelLabel
			// 
			this.mSeriesLabelLabel.Location = new System.Drawing.Point(8, 16);
			this.mSeriesLabelLabel.Name = "mSeriesLabelLabel";
			this.mSeriesLabelLabel.Size = new System.Drawing.Size(40, 23);
			this.mSeriesLabelLabel.TabIndex = 1;
			this.mSeriesLabelLabel.Text = "Label";
			// 
			// mSeriesLabelTextBox
			// 
			this.mSeriesLabelTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mSeriesLabelTextBox.Location = new System.Drawing.Point(56, 16);
			this.mSeriesLabelTextBox.Multiline = true;
			this.mSeriesLabelTextBox.Name = "mSeriesLabelTextBox";
			this.mSeriesLabelTextBox.Size = new System.Drawing.Size(248, 42);
			this.mSeriesLabelTextBox.TabIndex = 0;
			this.mSeriesLabelTextBox.Text = "textBox1";
			this.mSeriesLabelTextBox.Validated += new System.EventHandler(this.mSeriesLabelTextBox_Validated);
			// 
			// mShow
			// 
			this.mShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mShow.Location = new System.Drawing.Point(320, 16);
			this.mShow.Name = "mShow";
			this.mShow.Size = new System.Drawing.Size(72, 40);
			this.mShow.TabIndex = 1;
			this.mShow.Text = "Show";
			this.mShow.CheckedChanged += new System.EventHandler(this.mShow_CheckedChanged);
			// 
			// mColorInterpretationGroupBox
			// 
			this.mColorInterpretationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mColorInterpretationGroupBox.Controls.Add(this.mColorInterpolaterEditor);
			this.mColorInterpretationGroupBox.Location = new System.Drawing.Point(8, 192);
			this.mColorInterpretationGroupBox.Name = "mColorInterpretationGroupBox";
			this.mColorInterpretationGroupBox.Size = new System.Drawing.Size(408, 160);
			this.mColorInterpretationGroupBox.TabIndex = 21;
			this.mColorInterpretationGroupBox.TabStop = false;
			this.mColorInterpretationGroupBox.Text = "Coloring";
			// 
			// mColorInterpolaterEditor
			// 
			this.mColorInterpolaterEditor.ColorInterpolater = null;
			this.mColorInterpolaterEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mColorInterpolaterEditor.Location = new System.Drawing.Point(3, 18);
			this.mColorInterpolaterEditor.Name = "mColorInterpolaterEditor";
			this.mColorInterpolaterEditor.Size = new System.Drawing.Size(402, 139);
			this.mColorInterpolaterEditor.TabIndex = 0;
			this.mColorInterpolaterEditor.ColorInterpolaterChanged += new System.EventHandler(this.mColorInterpolaterEditor_ColorInterpolaterChanged);
			// 
			// mPreviewPanel
			// 
			this.mPreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mPreviewPanel.Location = new System.Drawing.Point(8, 16);
			this.mPreviewPanel.Name = "mPreviewPanel";
			this.mPreviewPanel.Size = new System.Drawing.Size(208, 40);
			this.mPreviewPanel.TabIndex = 11;
			this.mPreviewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mPreviewPanel_Paint);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.mPreviewZAxisTrackBar);
			this.groupBox1.Controls.Add(this.mPreviewPanel);
			this.groupBox1.Location = new System.Drawing.Point(184, 72);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(224, 112);
			this.groupBox1.TabIndex = 22;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Preview";
			// 
			// mPreviewZAxisTrackBar
			// 
			this.mPreviewZAxisTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mPreviewZAxisTrackBar.Location = new System.Drawing.Point(8, 64);
			this.mPreviewZAxisTrackBar.Maximum = 30;
			this.mPreviewZAxisTrackBar.Name = "mPreviewZAxisTrackBar";
			this.mPreviewZAxisTrackBar.Size = new System.Drawing.Size(208, 56);
			this.mPreviewZAxisTrackBar.TabIndex = 12;
			this.mPreviewZAxisTrackBar.TickFrequency = 3;
			this.mPreviewZAxisTrackBar.Value = 15;
			this.mPreviewZAxisTrackBar.Scroll += new System.EventHandler(this.mPreviewZAxisTrackBar_Scroll);
			// 
			// mShapeGroupBox
			// 
			this.mShapeGroupBox.Controls.Add(this.label1);
			this.mShapeGroupBox.Controls.Add(this.mShapeComboBox);
			this.mShapeGroupBox.Controls.Add(this.mSizeLabel);
			this.mShapeGroupBox.Controls.Add(this.mSizeCombo);
			this.mShapeGroupBox.Controls.Add(this.mHollowCheckBox);
			this.mShapeGroupBox.Location = new System.Drawing.Point(8, 72);
			this.mShapeGroupBox.Name = "mShapeGroupBox";
			this.mShapeGroupBox.Size = new System.Drawing.Size(168, 112);
			this.mShapeGroupBox.TabIndex = 23;
			this.mShapeGroupBox.TabStop = false;
			this.mShapeGroupBox.Text = "Shape";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 23);
			this.label1.TabIndex = 12;
			this.label1.Text = "Shape:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mShapeComboBox
			// 
			this.mShapeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mShapeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mShapeComboBox.Location = new System.Drawing.Point(56, 16);
			this.mShapeComboBox.MaxDropDownItems = 10;
			this.mShapeComboBox.Name = "mShapeComboBox";
			this.mShapeComboBox.Size = new System.Drawing.Size(104, 24);
			this.mShapeComboBox.TabIndex = 11;
			this.mShapeComboBox.SelectedIndexChanged += new System.EventHandler(this.mShapeComboBox_SelectedIndexChanged);
			// 
			// mPenGroupBox
			// 
			this.mPenGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mPenGroupBox.Controls.Add(this.mPenProviderEditor);
			this.mPenGroupBox.Location = new System.Drawing.Point(8, 352);
			this.mPenGroupBox.Name = "mPenGroupBox";
			this.mPenGroupBox.Size = new System.Drawing.Size(400, 216);
			this.mPenGroupBox.TabIndex = 24;
			this.mPenGroupBox.TabStop = false;
			this.mPenGroupBox.Text = "Line Parameters";
			// 
			// mPenProviderEditor
			// 
			this.mPenProviderEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mPenProviderEditor.Location = new System.Drawing.Point(3, 18);
			this.mPenProviderEditor.Name = "mPenProviderEditor";
			this.mPenProviderEditor.Size = new System.Drawing.Size(394, 195);
			this.mPenProviderEditor.TabIndex = 0;
			this.mPenProviderEditor.PenChanged += new System.EventHandler(this.mPenProviderEditor_PenChanged);
			// 
			// mLabelGroupBox
			// 
			this.mLabelGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mLabelGroupBox.Controls.Add(this.mSeriesLabelTextBox);
			this.mLabelGroupBox.Controls.Add(this.mSeriesLabelLabel);
			this.mLabelGroupBox.Controls.Add(this.mShow);
			this.mLabelGroupBox.Location = new System.Drawing.Point(8, 8);
			this.mLabelGroupBox.Name = "mLabelGroupBox";
			this.mLabelGroupBox.Size = new System.Drawing.Size(400, 64);
			this.mLabelGroupBox.TabIndex = 25;
			this.mLabelGroupBox.TabStop = false;
			this.mLabelGroupBox.Text = "Main";
			// 
			// ctlPlotParamsEditor
			// 
			this.Controls.Add(this.mLabelGroupBox);
			this.Controls.Add(this.mPenGroupBox);
			this.Controls.Add(this.mShapeGroupBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.mColorInterpretationGroupBox);
			this.Name = "ctlPlotParamsEditor";
			this.Size = new System.Drawing.Size(416, 600);
			this.mColorInterpretationGroupBox.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mPreviewZAxisTrackBar)).EndInit();
			this.mShapeGroupBox.ResumeLayout(false);
			this.mPenGroupBox.ResumeLayout(false);
			this.mLabelGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void PlotParamsChanged(Object sender, PNNLControls.PlotParamsChangedEventArgs args) 
		{
			this.mPreviewPanel.Invalidate();
		}

		public void ColorInterpolaterChanged(Object sender, EventArgs args) 
		{
			if (this.mRespondToEvents) 
			{
				this.PlotParams.Coloring = this.mColorInterpolaterEditor.ColorInterpolater;
				this.mPreviewPanel.Invalidate();
			}
		}

		public clsPlotParams PlotParams 
		{
			get 
			{
				return this.mPlotParams;
			}
			set 
			{
				if (this.mPlotParams != null) 
				{
					this.mPlotParams.PlotParamsChanged -= this.mPlotParamsChangedHandler;
				}
				this.mPlotParams = value;
				if (this.mPlotParams != null) 
				{
					this.mPlotParams.PlotParamsChanged += this.mPlotParamsChangedHandler;
				}
				this.RefreshFromParams();
			}
		}

		private void RefreshFromParams() 
		{
			this.mRespondToEvents = false;
			if (this.PlotParams != null) 
			{
				this.mSeriesLabelTextBox.Text = this.PlotParams.Name;
				foreach (ShapeCreater creater in this.mShapeComboBox.Items) 
				{
					if (creater.Matches(this.PlotParams.Shape))
					{
						this.mShapeComboBox.SelectedItem = creater;
						break;
					}
				}
				this.mShow.Checked = this.PlotParams.Visible;
				this.mSizeCombo.SelectedItem = this.PlotParams.Shape.Size;
				this.mColorInterpolaterEditor.ColorInterpolater = this.PlotParams.Coloring;
				this.mPenProviderEditor.PenProvider = this.PlotParams.LinePen;
				this.mPreviewPanel.Invalidate();
			}
			this.mRespondToEvents = true;
		}


		private void mShow_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.PlotParams.Visible = this.mShow.Checked;
				this.RefreshFromParams();
			}
		}

		private void mPreviewPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (this.PlotParams != null) 
			{
				Graphics g = e.Graphics;
				GraphicsContainer container = g.BeginContainer();
				g.DrawLine(this.PlotParams.LinePen.Pen, 5, this.mPreviewPanel.Height / 2, 
					this.mPreviewPanel.Width - 5, this.mPreviewPanel.Height / 2);
				g.TranslateTransform(this.mPreviewPanel.Width / 2, this.mPreviewPanel.Height / 2);
				
				if (this.PlotParams.Coloring is BoundedColorInterpolater) 
				{
					BoundedColorInterpolater bci = (BoundedColorInterpolater) this.PlotParams.Coloring;
					float zValue = bci.LowValue + (bci.HighValue - bci.LowValue) * 
						((float) this.mPreviewZAxisTrackBar.Value / this.mPreviewZAxisTrackBar.Maximum);
					this.PlotParams.DrawShape(g, zValue);
				}
				else 
				{
					this.PlotParams.DrawShape(g);
				}
				g.EndContainer(container);
			}
		}


		private void mPreviewZAxisTrackBar_Scroll(object sender, System.EventArgs e)
		{
			this.mPreviewPanel.Invalidate();
		}

		private void mSizeCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				clsShape shape = this.PlotParams.Shape;
				shape.Size = (int) this.mSizeCombo.SelectedItem;
				this.PlotParams.Shape = shape;
				this.RefreshFromParams();
			}
		}

		private void mHollowCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				clsShape shape = this.PlotParams.Shape;
				shape.Hollow = this.mHollowCheckBox.Checked;
				this.PlotParams.Shape = shape;
				this.RefreshFromParams();
			}
		}

		private void mShapeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				clsShape shape = this.PlotParams.Shape;
				shape = ((ShapeCreater) this.mShapeComboBox.SelectedItem).CreateShape(
					shape.Size, shape.Hollow);
				this.PlotParams.Shape = shape;
				this.RefreshFromParams();
			}
		}

		private void mPenProviderEditor_PenChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.PlotParams.LinePen = this.mPenProviderEditor.PenProvider;
				this.RefreshFromParams();
			}
		}

		private void mColorInterpolaterEditor_ColorInterpolaterChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.PlotParams.Coloring = this.mColorInterpolaterEditor.ColorInterpolater;
			}
		}

		private void mSeriesLabelTextBox_Validated(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.PlotParams.Name = this.mSeriesLabelTextBox.Text;
			}
		}
	}

	#region "Shape Creaters"
	abstract class ShapeCreater 
	{
		private String mName;
		public ShapeCreater(String name) 
		{
			this.mName = name;
		}
		public abstract clsShape CreateShape(int size, bool hollow);
		public abstract bool Matches(clsShape shape);
		public override string ToString()
		{
			return mName;
		}
	}
	class PointShapeCreater : ShapeCreater 
	{
		public PointShapeCreater() : base("Point") {}
		public override clsShape CreateShape(int size, bool hollow)
		{
			return new PointShape(size, hollow);
		}
		public override bool Matches(clsShape shape)
		{
			return shape is PointShape;
		}
	}
	class DiamondShapeCreater : ShapeCreater 
	{
		public DiamondShapeCreater() : base("Diamond") {}
		public override clsShape CreateShape(int size, bool hollow)
		{
			return new DiamondShape(size, hollow);
		}
		public override bool Matches(clsShape shape)
		{
			return shape is DiamondShape;
		}
	}
	class SquareShapeCreater : ShapeCreater 
	{
		public SquareShapeCreater() : base("Square") {}
		public override clsShape CreateShape(int size, bool hollow)
		{
			return new SquareShape(size, hollow);
		}
		public override bool Matches(clsShape shape)
		{
			return shape is SquareShape;
		}
	}
	class BubbleShapeCreater : ShapeCreater 
	{
		public BubbleShapeCreater() : base("Bubble") {}
		public override clsShape CreateShape(int size, bool hollow)
		{
			return new BubbleShape(size, hollow);
		}
		public override bool Matches(clsShape shape)
		{
			return shape is BubbleShape;
		}
	}
	class PlusShapeCreater : ShapeCreater 
	{
		public PlusShapeCreater() : base("Plus") {}
		public override clsShape CreateShape(int size, bool hollow)
		{
			return new PlusShape(size, hollow);
		}
		public override bool Matches(clsShape shape)
		{
			return shape is PlusShape;
		}
	}
	class CrossShapeCreater : ShapeCreater 
	{
		public CrossShapeCreater() : base("Cross") {}
		public override clsShape CreateShape(int size, bool hollow)
		{
			return new CrossShape(size, hollow);
		}
		public override bool Matches(clsShape shape)
		{
			return shape is CrossShape;
		}
	}
	class StarShapeCreater : ShapeCreater 
	{
		public StarShapeCreater() : base("Star") {}
		public override clsShape CreateShape(int size, bool hollow)
		{
			return new StarShape(size, hollow);
		}
		public override bool Matches(clsShape shape)
		{
			return shape is StarShape;
		}
	}
	class TriangleShapeCreater : ShapeCreater 
	{
		public TriangleShapeCreater() : base("Triangle") {}
		public override clsShape CreateShape(int size, bool hollow)
		{
			return new TriangleShape(size, hollow);
		}
		public override bool Matches(clsShape shape)
		{
			return shape is TriangleShape;
		}
	}
	#endregion
}
