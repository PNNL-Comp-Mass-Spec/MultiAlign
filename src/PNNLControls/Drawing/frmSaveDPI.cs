using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmSaveDPI.
	/// </summary>
	public class frmSaveDPI : System.Windows.Forms.Form
	{
		private const double TRANSPARENCY	 = 0.6;	
		private const double NO_TRANSPARENCY = 1.0;
		private const double TRANSPARENCY_SUB_AMT = .05;
		
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TrackBar trackDPI;
		private System.Windows.Forms.Label lblDPI;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.PictureBox picturePreview;		
		
		private Bitmap m_labelBitmap	= null;		
		private Bitmap m_noLabelBitmap  = null;		
		private Bitmap m_bitmap			= null;		
		
		private float m_xresolution = 300;
		private System.Windows.Forms.NumericUpDown numDPI;
		private System.Windows.Forms.CheckBox chkSaveLabels;
		private System.Windows.Forms.Timer timerTransparency;
		private float m_yresolution = 300;

		public frmSaveDPI()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);

			InitializeComponent();	
			Load += new EventHandler(frmSaveDPI_Load);	
		
			numDPI.Maximum = trackDPI.Maximum;
			numDPI.Minimum = trackDPI.Minimum;
			
			// Save the default dialog result to be cancelled.
			DialogResult = System.Windows.Forms.DialogResult.Cancel;	
			
			timerTransparency.Enabled  = false;
			timerTransparency.Tick	  += new EventHandler(timerTransparency_Tick);			
			Move					  += new EventHandler(frmSaveDPI_MouseEnter);			
			Opacity = TRANSPARENCY;
			SetTransparencyHandlers(this);																	
		}

		private void SetTransparencyHandlers( Control parentControl)
		{
			foreach(Control childControl in parentControl.Controls)
			{
				childControl.GotFocus   += new EventHandler(frmSaveDPI_MouseEnter);				
				childControl.MouseHover += new EventHandler(frmSaveDPI_MouseEnter);
				childControl.MouseEnter += new EventHandler(frmSaveDPI_MouseEnter);				
				childControl.MouseLeave += new EventHandler(frmSaveDPI_MouseLeave);							
				SetTransparencyHandlers(childControl);
			}
		}

		/// <summary>
		/// Required thumbnail abort callback.
		/// </summary>
		/// <returns>false</returns>
		private bool ThumbnailCallback()
		{
			return false;
		}

		/// <summary>
		/// Gets the bitmap to save.
		/// </summary>
		public Bitmap Bitmap
		{
			get
			{
				return m_bitmap;	
			}						
		}

		/// <summary>
		/// Gets/Sets the bitmap without the labels attached.
		/// </summary>
		public Bitmap NonLabeledBitmap
		{
			get
			{
				return m_noLabelBitmap;	
			}
			
			set
			{	
				// Clear the old bitmap and recreate a new one		
				if (m_noLabelBitmap != null)				
					m_noLabelBitmap.Dispose();	
				m_noLabelBitmap = new Bitmap(value);	
			}
		}
		
		/// <summary>
		/// Gets/Sets the labeled bitmap.
		/// </summary>
		public Bitmap LabeledBitmap
		{
			get
			{
				return m_labelBitmap;	
			}
			
			set
			{	
				// Clear the old bitmap and recreate a new one		
				if (m_labelBitmap != null)
					m_labelBitmap.Dispose();
				
				m_labelBitmap = new Bitmap(value);
			}
		}
        
		/// <summary>
		/// Displays a thumbnail of the bitmap to save.
		/// </summary>
		/// <param name="bmp">Bitmap to display as a thumbnail for visual feedback.</param>
		private void ShowThumbnail(Bitmap bmp)
		{
			if (bmp != null)
			{		
				picturePreview.Image = bmp.GetThumbnailImage(picturePreview.Width, picturePreview.Height,
												new Image.GetThumbnailImageAbort(ThumbnailCallback),
												IntPtr.Zero);		
			}
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
				
					if (m_noLabelBitmap != null)				
						m_noLabelBitmap.Dispose();	
					if (m_labelBitmap != null)
						m_labelBitmap.Dispose();
				
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.trackDPI = new System.Windows.Forms.TrackBar();
			this.lblDPI = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.picturePreview = new System.Windows.Forms.PictureBox();
			this.numDPI = new System.Windows.Forms.NumericUpDown();
			this.chkSaveLabels = new System.Windows.Forms.CheckBox();
			this.timerTransparency = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.trackDPI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numDPI)).BeginInit();
			this.SuspendLayout();
			// 
			// trackDPI
			// 
			this.trackDPI.Location = new System.Drawing.Point(232, 32);
			this.trackDPI.Maximum = 3600;
			this.trackDPI.Minimum = 1;
			this.trackDPI.Name = "trackDPI";
			this.trackDPI.Size = new System.Drawing.Size(272, 42);
			this.trackDPI.TabIndex = 1;
			this.trackDPI.TickFrequency = 100;
			this.trackDPI.Value = 60;
			this.trackDPI.Scroll += new System.EventHandler(this.trackDPI_Scroll);
			// 
			// lblDPI
			// 
			this.lblDPI.Location = new System.Drawing.Point(240, 8);
			this.lblDPI.Name = "lblDPI";
			this.lblDPI.Size = new System.Drawing.Size(40, 16);
			this.lblDPI.TabIndex = 3;
			this.lblDPI.Text = "DPI: ";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(344, 192);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(80, 24);
			this.btnSave.TabIndex = 5;
			this.btnSave.Text = "Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(432, 192);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 24);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// picturePreview
			// 
			this.picturePreview.Location = new System.Drawing.Point(8, 8);
			this.picturePreview.Name = "picturePreview";
			this.picturePreview.Size = new System.Drawing.Size(216, 208);
			this.picturePreview.TabIndex = 7;
			this.picturePreview.TabStop = false;
			// 
			// numDPI
			// 
			this.numDPI.Location = new System.Drawing.Point(280, 8);
			this.numDPI.Name = "numDPI";
			this.numDPI.Size = new System.Drawing.Size(64, 20);
			this.numDPI.TabIndex = 8;
			this.numDPI.ValueChanged += new System.EventHandler(this.numDPI_ValueChanged);
			// 
			// chkSaveLabels
			// 
			this.chkSaveLabels.Location = new System.Drawing.Point(240, 80);
			this.chkSaveLabels.Name = "chkSaveLabels";
			this.chkSaveLabels.Size = new System.Drawing.Size(104, 16);
			this.chkSaveLabels.TabIndex = 9;
			this.chkSaveLabels.Text = "Show Labels";
			this.chkSaveLabels.CheckedChanged += new System.EventHandler(this.chkSaveLabels_CheckedChanged);
			// 
			// frmSaveDPI
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(506, 223);
			this.ControlBox = false;
			this.Controls.Add(this.chkSaveLabels);
			this.Controls.Add(this.numDPI);
			this.Controls.Add(this.picturePreview);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.lblDPI);
			this.Controls.Add(this.trackDPI);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSaveDPI";
			this.Text = "Save Image with Resolution (DPI)";
			((System.ComponentModel.ISupportInitialize)(this.trackDPI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numDPI)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// Cancel the save process for setting the resolution
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Hide();
		}

		/// <summary>
		/// Saves the settings 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			m_bitmap.SetResolution(DPI, DPI);			
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Hide();
		}

		/// <summary>
		/// Gets/Sets XY-Resolution (DPI) for bitmap image.
		/// </summary>
		public float DPI
		{
			get
			{
				return m_xresolution;
			}
			set
			{
				m_xresolution = value;
				m_yresolution = value;
			}
		}		

		/// <summary>
		/// Loading the form.  Sets the resolution according to what the user has set.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmSaveDPI_Load(object sender, EventArgs e)
		{	
			chkSaveLabels.Checked = true;					
			numDPI.Value = System.Convert.ToDecimal(DPI); 			
			lblDPI.Text  = "DPI";
			m_bitmap     = m_labelBitmap; 
			ShowThumbnail(m_bitmap);
		}

		private void trackDPI_Scroll(object sender, System.EventArgs e)
		{
			DPI = trackDPI.Value;
			numDPI.Value = System.Convert.ToDecimal(DPI);					
			if(m_bitmap != null)
				m_bitmap.SetResolution(m_xresolution, m_yresolution);				
			ShowThumbnail(m_bitmap);						
		}

		private void numDPI_ValueChanged(object sender, System.EventArgs e)
		{
			trackDPI.Value = System.Convert.ToInt32(numDPI.Value);
			DPI = trackDPI.Value;
			if(m_bitmap != null)
				m_bitmap.SetResolution(m_xresolution, m_yresolution);
			ShowThumbnail(m_bitmap);
		}

		/// <summary>
		/// Handles when the checked saved labels control is changed.  Checked will thumbnail a display labeled image.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chkSaveLabels_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkSaveLabels.Checked == true)
				m_bitmap = m_labelBitmap;
			else
				m_bitmap = m_noLabelBitmap;
			ShowThumbnail(m_bitmap);
		}


		private void timerTransparency_Tick(object sender, EventArgs e)
		{
			System.Drawing.Point p = System.Windows.Forms.Cursor.Position;						
			double subtractAmount = 0;
			if (p.X < Bounds.Left || p.X > Bounds.Left + Bounds.Width || p.Y > Bounds.Top + Bounds.Height || p.Y < Bounds.Top)
			{
				if(Bounds.Left > Owner.Right || Bounds.Right < Owner.Left || Bounds.Top > Owner.Top + Owner.Height || Bounds.Bottom < Owner.Top)
				{					
					subtractAmount = 0;;
				}
				else
				{
					subtractAmount = TRANSPARENCY_SUB_AMT;
				}
			}
			
			Opacity -= subtractAmount;
			if (Opacity < TRANSPARENCY)
				timerTransparency.Enabled = false;
		}
				
		private void frmSaveDPI_MouseEnter(object sender, EventArgs e)
		{
			Opacity = NO_TRANSPARENCY;
		}

		private void frmSaveDPI_MouseLeave(object sender, EventArgs e)
		{
			timerTransparency.Enabled = true;
		}
	}
}
