using System;
using System.Drawing.Drawing2D;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlPenEditor.
	/// </summary>
	public class ctlPenProviderEditor : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private PenProvider mPenProvider;
		private System.Windows.Forms.Label mColorLabel;
		private System.Windows.Forms.Label mWidthLabel;
		private System.Windows.Forms.Label mDashStyleLabel;
		private System.Windows.Forms.Panel mPreviewPanel;
		private System.Windows.Forms.Label mDashCapLabel;
		private System.Windows.Forms.ComboBox mWidthCombo;
		private System.Windows.Forms.ComboBox mDashStyleCombo;
		private System.Windows.Forms.ComboBox mDashCapCombo;
		private System.Windows.Forms.Label mStartCapLabel;
		private System.Windows.Forms.Label mEndCapLabel;
		private System.Windows.Forms.ComboBox mStartCapCombo;
		private System.Windows.Forms.ComboBox mEndCapCombo;
		private System.Windows.Forms.Label mPreviewLabel;
		private ExternalControls.ColorPicker mColorPicker;
		private bool mRespondToEvents = true;

		[System.ComponentModel.Browsable(true)]
		public event EventHandler PenChanged;

		public ctlPenProviderEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			for (int i = 1; i <= 10; i++) 
			{
				this.mWidthCombo.Items.Add(i);
			}

			this.mDashStyleCombo.Items.Add(DashStyle.Dash);
			this.mDashStyleCombo.Items.Add(DashStyle.DashDot);
			this.mDashStyleCombo.Items.Add(DashStyle.DashDotDot);
			this.mDashStyleCombo.Items.Add(DashStyle.Dot);
			this.mDashStyleCombo.Items.Add(DashStyle.Solid);

			this.mDashCapCombo.Items.Add(DashCap.Flat);
			this.mDashCapCombo.Items.Add(DashCap.Triangle);
			this.mDashCapCombo.Items.Add(DashCap.Round);

			this.mStartCapCombo.Items.Add(LineCap.ArrowAnchor);
			this.mStartCapCombo.Items.Add(LineCap.DiamondAnchor);
			this.mStartCapCombo.Items.Add(LineCap.Flat);
			this.mStartCapCombo.Items.Add(LineCap.NoAnchor);
			this.mStartCapCombo.Items.Add(LineCap.Round);
			this.mStartCapCombo.Items.Add(LineCap.RoundAnchor);
			this.mStartCapCombo.Items.Add(LineCap.Square);
			this.mStartCapCombo.Items.Add(LineCap.SquareAnchor);
			this.mStartCapCombo.Items.Add(LineCap.Triangle);

			this.mEndCapCombo.Items.Add(LineCap.ArrowAnchor);
			this.mEndCapCombo.Items.Add(LineCap.DiamondAnchor);
			this.mEndCapCombo.Items.Add(LineCap.Flat);
			this.mEndCapCombo.Items.Add(LineCap.NoAnchor);
			this.mEndCapCombo.Items.Add(LineCap.Round);
			this.mEndCapCombo.Items.Add(LineCap.RoundAnchor);
			this.mEndCapCombo.Items.Add(LineCap.Square);
			this.mEndCapCombo.Items.Add(LineCap.SquareAnchor);
			this.mEndCapCombo.Items.Add(LineCap.Triangle);
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
			this.mColorLabel = new System.Windows.Forms.Label();
			this.mWidthCombo = new System.Windows.Forms.ComboBox();
			this.mWidthLabel = new System.Windows.Forms.Label();
			this.mDashStyleLabel = new System.Windows.Forms.Label();
			this.mDashStyleCombo = new System.Windows.Forms.ComboBox();
			this.mDashCapCombo = new System.Windows.Forms.ComboBox();
			this.mPreviewPanel = new System.Windows.Forms.Panel();
			this.mDashCapLabel = new System.Windows.Forms.Label();
			this.mStartCapLabel = new System.Windows.Forms.Label();
			this.mEndCapLabel = new System.Windows.Forms.Label();
			this.mStartCapCombo = new System.Windows.Forms.ComboBox();
			this.mEndCapCombo = new System.Windows.Forms.ComboBox();
			this.mPreviewLabel = new System.Windows.Forms.Label();
			this.mColorPicker = new ExternalControls.ColorPicker();
			this.SuspendLayout();
			// 
			// mColorLabel
			// 
			this.mColorLabel.Location = new System.Drawing.Point(8, 8);
			this.mColorLabel.Name = "mColorLabel";
			this.mColorLabel.Size = new System.Drawing.Size(80, 23);
			this.mColorLabel.TabIndex = 1;
			this.mColorLabel.Text = "Color:";
			this.mColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mWidthCombo
			// 
			this.mWidthCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mWidthCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mWidthCombo.Location = new System.Drawing.Point(88, 40);
			this.mWidthCombo.Name = "mWidthCombo";
			this.mWidthCombo.Size = new System.Drawing.Size(128, 24);
			this.mWidthCombo.TabIndex = 2;
			this.mWidthCombo.SelectedIndexChanged += new System.EventHandler(this.mWidthCombo_SelectedIndexChanged);
			// 
			// mWidthLabel
			// 
			this.mWidthLabel.Location = new System.Drawing.Point(8, 40);
			this.mWidthLabel.Name = "mWidthLabel";
			this.mWidthLabel.Size = new System.Drawing.Size(80, 23);
			this.mWidthLabel.TabIndex = 3;
			this.mWidthLabel.Text = "Width:";
			this.mWidthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mDashStyleLabel
			// 
			this.mDashStyleLabel.Location = new System.Drawing.Point(8, 72);
			this.mDashStyleLabel.Name = "mDashStyleLabel";
			this.mDashStyleLabel.Size = new System.Drawing.Size(80, 23);
			this.mDashStyleLabel.TabIndex = 4;
			this.mDashStyleLabel.Text = "Dash Style:";
			this.mDashStyleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mDashStyleCombo
			// 
			this.mDashStyleCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mDashStyleCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mDashStyleCombo.Location = new System.Drawing.Point(88, 72);
			this.mDashStyleCombo.Name = "mDashStyleCombo";
			this.mDashStyleCombo.Size = new System.Drawing.Size(128, 24);
			this.mDashStyleCombo.TabIndex = 5;
			this.mDashStyleCombo.SelectedIndexChanged += new System.EventHandler(this.mDashStyleCombo_SelectedIndexChanged);
			// 
			// mDashCapCombo
			// 
			this.mDashCapCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mDashCapCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mDashCapCombo.Location = new System.Drawing.Point(88, 104);
			this.mDashCapCombo.Name = "mDashCapCombo";
			this.mDashCapCombo.Size = new System.Drawing.Size(128, 24);
			this.mDashCapCombo.TabIndex = 6;
			this.mDashCapCombo.SelectedIndexChanged += new System.EventHandler(this.mDashCapCombo_SelectedIndexChanged);
			// 
			// mPreviewPanel
			// 
			this.mPreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mPreviewPanel.Location = new System.Drawing.Point(224, 32);
			this.mPreviewPanel.Name = "mPreviewPanel";
			this.mPreviewPanel.Size = new System.Drawing.Size(72, 160);
			this.mPreviewPanel.TabIndex = 7;
			this.mPreviewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mPreviewPanel_Paint);
			// 
			// mDashCapLabel
			// 
			this.mDashCapLabel.Location = new System.Drawing.Point(8, 104);
			this.mDashCapLabel.Name = "mDashCapLabel";
			this.mDashCapLabel.Size = new System.Drawing.Size(80, 23);
			this.mDashCapLabel.TabIndex = 8;
			this.mDashCapLabel.Text = "Dash Cap:";
			this.mDashCapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mStartCapLabel
			// 
			this.mStartCapLabel.Location = new System.Drawing.Point(8, 136);
			this.mStartCapLabel.Name = "mStartCapLabel";
			this.mStartCapLabel.Size = new System.Drawing.Size(80, 23);
			this.mStartCapLabel.TabIndex = 9;
			this.mStartCapLabel.Text = "Start Cap:";
			this.mStartCapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mEndCapLabel
			// 
			this.mEndCapLabel.Location = new System.Drawing.Point(8, 168);
			this.mEndCapLabel.Name = "mEndCapLabel";
			this.mEndCapLabel.Size = new System.Drawing.Size(80, 23);
			this.mEndCapLabel.TabIndex = 10;
			this.mEndCapLabel.Text = "End Cap:";
			this.mEndCapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mStartCapCombo
			// 
			this.mStartCapCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mStartCapCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mStartCapCombo.Location = new System.Drawing.Point(88, 136);
			this.mStartCapCombo.Name = "mStartCapCombo";
			this.mStartCapCombo.Size = new System.Drawing.Size(128, 24);
			this.mStartCapCombo.TabIndex = 11;
			this.mStartCapCombo.SelectedIndexChanged += new System.EventHandler(this.mStartCapCombo_SelectedIndexChanged);
			// 
			// mEndCapCombo
			// 
			this.mEndCapCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mEndCapCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mEndCapCombo.Location = new System.Drawing.Point(88, 168);
			this.mEndCapCombo.Name = "mEndCapCombo";
			this.mEndCapCombo.Size = new System.Drawing.Size(128, 24);
			this.mEndCapCombo.TabIndex = 12;
			this.mEndCapCombo.SelectedIndexChanged += new System.EventHandler(this.mEndCapCombo_SelectedIndexChanged);
			// 
			// mPreviewLabel
			// 
			this.mPreviewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mPreviewLabel.Location = new System.Drawing.Point(224, 8);
			this.mPreviewLabel.Name = "mPreviewLabel";
			this.mPreviewLabel.Size = new System.Drawing.Size(72, 23);
			this.mPreviewLabel.TabIndex = 13;
			this.mPreviewLabel.Text = "Preview";
			this.mPreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mColorPicker
			// 
			this.mColorPicker.Location = new System.Drawing.Point(88, 8);
			this.mColorPicker.Name = "mColorPicker";
			this.mColorPicker.Size = new System.Drawing.Size(128, 21);
			this.mColorPicker.TabIndex = 14;
			this.mColorPicker.ColorChanged += new ExternalControls.ColorChangedEventHandler(this.mColorPicker_ColorChanged);
			// 
			// ctlPenProviderEditor
			// 
			this.Controls.Add(this.mColorPicker);
			this.Controls.Add(this.mPreviewLabel);
			this.Controls.Add(this.mEndCapCombo);
			this.Controls.Add(this.mStartCapCombo);
			this.Controls.Add(this.mEndCapLabel);
			this.Controls.Add(this.mStartCapLabel);
			this.Controls.Add(this.mDashCapLabel);
			this.Controls.Add(this.mPreviewPanel);
			this.Controls.Add(this.mDashCapCombo);
			this.Controls.Add(this.mDashStyleCombo);
			this.Controls.Add(this.mDashStyleLabel);
			this.Controls.Add(this.mWidthLabel);
			this.Controls.Add(this.mWidthCombo);
			this.Controls.Add(this.mColorLabel);
			this.Name = "ctlPenProviderEditor";
			this.Size = new System.Drawing.Size(304, 200);
			this.ResumeLayout(false);

		}
		#endregion

		//Mark as being invisible in design mode
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public PenProvider PenProvider
		{
			get 
			{
				return this.mPenProvider;
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("PenProvider");
				}
				this.mPenProvider = value;
				this.RefreshFromPen();
				this.RaisePenChanged();
			}
		}

		private void RaisePenChanged() 
		{
			if (this.PenChanged != null) 
			{
				this.PenChanged(this, EventArgs.Empty);
			}
		}

		private void RefreshFromPen() 
		{
			if (this.mPenProvider != null) 
			{
				this.mRespondToEvents = false;
				this.mColorPicker.Color = this.mPenProvider.Color;
				this.mWidthCombo.SelectedItem = (int) Math.Ceiling(this.mPenProvider.Width);
				this.mDashStyleCombo.SelectedItem = this.mPenProvider.DashStyle;
				this.mDashCapCombo.SelectedItem = this.mPenProvider.DashCap;
				this.mEndCapCombo.SelectedItem = this.mPenProvider.EndCap;
				this.mStartCapCombo.SelectedItem = this.mPenProvider.StartCap;
				this.mPreviewPanel.Invalidate();
				this.mRespondToEvents = true;
			}
		}

		private void mPreviewPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (this.mPenProvider != null) 
			{
				int x = this.mPreviewPanel.Width / 2;
				float startY = this.mPenProvider.Width;
				float endY = this.mPreviewPanel.Height - this.mPenProvider.Width;
				e.Graphics.DrawLine(this.mPenProvider.Pen, x, startY, x, endY);
			}
		}

		private void mWidthCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.mPenProvider.Width = (int) this.mWidthCombo.SelectedItem;
				this.RefreshFromPen();
				this.RaisePenChanged();
			}
		}

		private void mDashStyleCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.mPenProvider.DashStyle = (DashStyle) this.mDashStyleCombo.SelectedItem;
				this.RefreshFromPen();
				this.RaisePenChanged();
			}
		}

		private void mDashCapCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.mPenProvider.DashCap = (DashCap) this.mDashCapCombo.SelectedItem;
				this.RefreshFromPen();
				this.RaisePenChanged();
			}
		}

		private void mStartCapCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.mPenProvider.StartCap = (LineCap) this.mStartCapCombo.SelectedItem;
				this.RefreshFromPen();
				this.RaisePenChanged();
			}
		}

		private void mEndCapCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.mPenProvider.EndCap = (LineCap) this.mEndCapCombo.SelectedItem;
				this.RefreshFromPen();
				this.RaisePenChanged();
			}
		}

		private void mColorPicker_ColorChanged(object sender, ExternalControls.ColorChangedEventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				this.mPenProvider.Color = this.mColorPicker.Color;
				this.RefreshFromPen();
				this.RaisePenChanged();
			}
		}
	}
}
