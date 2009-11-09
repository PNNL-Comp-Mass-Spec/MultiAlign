using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace PNNLControls
{

	/// <summary>
	/// Summary description for ExpandPanel.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(ExpandPanelConverter))]
	public class ExpandPanel : System.Windows.Forms.Panel, System.ComponentModel.ISupportInitialize
	{
		#region Constants
		private const int DEFAULT_EXPANSION_TIME = 250;
		private const int DEFAULT_HEADER_HEIGHT = 20;
		private const int DEFAULT_ANIMATION_STEP_TIME = 30;
		private const int DEFAULT_EXPAND_IMAGE_WIDTH = 16;
		private const int DEFAULT_HEADER_IMAGE_WIDTH = 16;
		private const ExpandDirection DEFAULT_EXPAND_DIRECTION = ExpandDirection.Down;
		private const HeaderLocation DEFAULT_HEADER_LOCATION = HeaderLocation.Top;
		private const HeaderTextSwitchingMode DEFAULT_HEADER_TEXT_MODE = HeaderTextSwitchingMode.AppendShowHide;
		private const HeaderImageSwitchingMode DEFAULT_IMAGE_SWITCHING_MODE = HeaderImageSwitchingMode.AutoList;
		private const int DEFAULT_MOUSE_EXPAND_HEIGHT = 3;
		#endregion
   
		#region "Events"
		/// <summary>
		/// Signals that the state of the expansion panel has changed
		/// </summary>
		public event EventHandler ExpansionStateChange;
		/// <summary>
		/// Signals that an expansion has ended
		/// </summary>
		public event EventHandler Expanded;
		/// <summary>
		/// Signals that a expansion is beginning
		/// </summary>
		public event EventHandler Expanding;
		/// <summary>
		/// Signals that a contraction has finished
		/// </summary>
		public event EventHandler Contracted;
		/// <summary>
		/// Signals that a contraction is beginning.
		/// </summary>
		public event EventHandler Contracting;

		/// <summary>
		/// Raises the ExpansionStateChange event
		/// </summary>
		protected virtual void OnExpansionStateChange()
		{
			EventHandler h = this.ExpansionStateChange;
			if (h != null)
			{
				h(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Raises the Expanded event
		/// </summary>
		protected virtual void OnExpanded()
		{
			this.UpdateMouseExpandCursor();
			EventHandler h = this.Expanded;
			if (h != null)
			{
				h(this, EventArgs.Empty);
			}
			this.OnExpansionStateChange();
		}

		/// <summary>
		/// Raises the Expanding event
		/// </summary>
		protected virtual void OnExpanding()
		{
			EventHandler h = this.Expanding;
			if (h != null)
			{
				h(this, EventArgs.Empty);
			}
			this.OnExpansionStateChange();
		}

		/// <summary>
		/// Raises the Contracted Event
		/// </summary>
		protected virtual void OnContracted()
		{
			EventHandler h = this.Contracted;
			if (h != null)
			{
				h(this, EventArgs.Empty);
			}
			this.OnExpansionStateChange();
		}
               
		/// <summary>
		/// Raises the Contracting Event
		/// </summary>
		protected virtual void OnContracting()
		{
			this.UpdateMouseExpandCursor();
			EventHandler h = this.Contracting;
			if (h != null)
			{
				h(this, EventArgs.Empty);
			}
			this.OnExpansionStateChange();
		}
		#endregion

		/// <summary>
		/// Timer used for expanding/contracting control.
		/// </summary>
		private System.Windows.Forms.Timer animationTimer;

		private bool inExpandContract = false;

		/// <summary>
		/// The picture box positioned at the right/bottom of the header.
		/// </summary>
		private System.Windows.Forms.PictureBox expandPictureBox;
		/// <summary>
		/// The picture box positioned at the left/top of the header
		/// </summary>
		private System.Windows.Forms.PictureBox controlPictureBox;
		/// <summary>
		/// The header panel - contains the header button and picture boxes.
		/// </summary>
		private System.Windows.Forms.Panel mHeaderPanel;
		/// <summary>
		/// The button in the middle of the header that draws the title.
		/// </summary>
		private UnselectableCheckBox headerButton;

		/// <summary>
		/// The context menu for the header.
		/// </summary>
		private ContextMenu mHeaderContextMenu;
		/// <summary>
		/// The menu item to adjust height
		/// </summary>
		private MenuItem mSetHeightMenuItem;

		/// <summary>
		/// Used as a container for the cursor associated with resizing the panel with 
		/// the mouse.  Positioned on the side towards which the panel expands.
		/// </summary>
		private System.Windows.Forms.Panel mHeaderMouseExpandPanel;
		/// <summary>
		/// Same as above, but positioned in the header panel instead of the main panel.
		/// </summary>
		private System.Windows.Forms.Panel mHeaderMouseExpandPanel2;
		/// <summary>
		/// The image list shown on the right or bottom of the header that contains
		/// the expansion/contraction chevrons.
		/// </summary>
		private ImagesList mExpandPictureImagesList;
		/// <summary>
		/// The image list shown at the top or left of the header that contains any custom 
		/// graphics.
		/// </summary>
		private ImagesList mHeaderPictureImageList; 

		private Color mHeaderBackColor = Color.Empty;
		private Color mHeaderForeColor = Color.Empty;
		private Font mHeaderFont = null;

		/// <summary>
		/// The text for the header used when it is contracted/contracting.
		/// </summary>
		private String mHeaderExpandText = "";
		/// <summary>
		/// The text for the header used when it is expanding/expanded.
		/// </summary>
		private String mHeaderContractText = "";
		/// <summary>
		/// The current text of the header.
		/// </summary>
		private String mHeaderText = "";
		/// <summary>
		/// Tells whether the control is in an ISupportInitialize initialization phase.
		/// </summary>
		private bool mInited = true;

		/// <summary>
		/// The height/width of the area around the border within which the mouse 
		/// can be used to control the expansion size of the control.
		/// </summary>
		private int mMouseExpandHeight = DEFAULT_MOUSE_EXPAND_HEIGHT;

		/// <summary>
		/// The location of the header of the control
		/// </summary>
		private HeaderLocation mHeaderLocation = DEFAULT_HEADER_LOCATION;
		/// <summary>
		/// The expand direction of the control.
		/// </summary>
		private ExpandDirection mExpandDirection = DEFAULT_EXPAND_DIRECTION;
		/// <summary>
		/// The width/height of the control.
		/// </summary>
		private int mHeaderHeight = DEFAULT_HEADER_HEIGHT;

		private HeaderImageSwitchingMode mExpandImageMode = DEFAULT_IMAGE_SWITCHING_MODE;
		private HeaderImageSwitchingMode mHeaderImageMode = DEFAULT_IMAGE_SWITCHING_MODE;
		private HeaderTextSwitchingMode mHeaderTextMode = DEFAULT_HEADER_TEXT_MODE;

		/// <summary>
		/// The current expansion state of the control.
		/// </summary>
		private ExpandMode mExpandMode = ExpandMode.Expanded;

		/// <summary>
		/// The amount of time (milliseconds) that an expand takes place over.
		/// </summary>
		private int mExpandTime = DEFAULT_EXPANSION_TIME;
		/// <summary>
		/// The height of the control when expanded.
		/// </summary>
		private int mExpandedContentHeight;
		/// <summary>
		/// The time that the last contraction/expansion nominally started.
		/// </summary>
		private DateTime mExpansionStartTime;

		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Creates an expand panel with the given ExpandedContentHeight
		/// </summary>
		/// <param name="expandedContentHeight"></param>
		public ExpandPanel(int expandedContentHeight)
		{
			this.mExpandedContentHeight = expandedContentHeight;
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.UpdateStyles();

			this.LoadExpandImageList();
			this.UpdateStateImmediate();
			this.SetHeaderAndImageBounds();
			this.SetHeaderContextMenu();
		}

		public ExpandPanel() : this(150)
		{
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ExpandPanel));

			this.controlPictureBox = new System.Windows.Forms.PictureBox();
			this.expandPictureBox = new System.Windows.Forms.PictureBox();
			this.headerButton = new PNNLControls.UnselectableCheckBox();
			this.mHeaderContextMenu = new ContextMenu();
			this.mSetHeightMenuItem = new MenuItem();
			this.mHeaderPanel = new System.Windows.Forms.Panel();
			this.mHeaderMouseExpandPanel = new System.Windows.Forms.Panel();
			this.mHeaderMouseExpandPanel2 = new System.Windows.Forms.Panel();
			this.animationTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			//
			// controlPictureBox
			//
			this.controlPictureBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.controlPictureBox.BackColor = Color.Transparent;
			this.controlPictureBox.Enabled = true;
			this.controlPictureBox.Location = new System.Drawing.Point(2, 2);
			this.controlPictureBox.Name = "expandPictureBox";
			this.controlPictureBox.Size = new System.Drawing.Size(16, 16);
			this.controlPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
			this.controlPictureBox.TabStop = false;
			this.controlPictureBox.MouseUp += new MouseEventHandler(Header_MouseUp);
			//
			// expandPictureBox
			//
			this.expandPictureBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.expandPictureBox.BackColor = Color.Transparent;
			this.expandPictureBox.Enabled = true;
			this.expandPictureBox.Location = new System.Drawing.Point(180, 2);
			this.expandPictureBox.Name = "expandPictureBox";
			this.expandPictureBox.Size = new System.Drawing.Size(16, 16);
			this.expandPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
			this.expandPictureBox.TabIndex = 1;
			this.expandPictureBox.TabStop = false;
			this.expandPictureBox.MouseUp += new MouseEventHandler(Header_MouseUp);
			//
			// headerButton
			//
			this.headerButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.headerButton.Appearance = System.Windows.Forms.Appearance.Button;
			this.headerButton.BackColor = Color.Transparent;
			this.headerButton.Checked = true;
			this.headerButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.headerButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.headerButton.Name = "headerButton";
			this.headerButton.TabIndex = 0;
			this.headerButton.Text = "Advanced Settings (Show)";
			this.headerButton.TextAlign = ContentAlignment.TopCenter;
			this.headerButton.CheckedChanged += new System.EventHandler(this.headerButton_CheckedChanged);
			//
			// mSetHeightMenuItem
			//
			this.mSetHeightMenuItem.Text = "Set Height";
			this.mSetHeightMenuItem.Click += new EventHandler(mSetHeightMenuItem_Click);
			//
			// mHeaderContextMenu
			//
			this.mHeaderContextMenu.MenuItems.Add(this.mSetHeightMenuItem);
			//
			// mHeaderPanel
			//
			this.mHeaderPanel.Anchor = AnchorStyles.None;
			this.mHeaderPanel.Location = new Point(0, 0);
			this.mHeaderPanel.Size = new Size(this.Width, this.HeaderHeight);
			this.mHeaderPanel.Resize += new EventHandler(mHeaderPanel_Resize);
			this.mHeaderPanel.Paint += new PaintEventHandler(mHeaderPanel_Paint);
			this.mHeaderPanel.MouseUp += new MouseEventHandler(Header_MouseUp);
			this.mHeaderPanel.Layout += new LayoutEventHandler(mHeaderPanel_Layout);
			//
			// mHeaderMouseExpandPanel
			//
			this.mHeaderMouseExpandPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.mHeaderMouseExpandPanel.BackColor = Color.Transparent;
			this.mHeaderMouseExpandPanel.Location = new Point(0, 0);
			this.mHeaderMouseExpandPanel.Size = new Size(this.Width, 2);
			this.mHeaderMouseExpandPanel.Visible = false;
			this.mHeaderMouseExpandPanel.MouseDown += new MouseEventHandler(mHeaderMouseExpandPanel_MouseDown);
			this.mHeaderMouseExpandPanel.MouseUp += new MouseEventHandler(mHeaderMouseExpandPanel_MouseUp);
			this.mHeaderMouseExpandPanel.MouseMove += new MouseEventHandler(mHeaderMouseExpandPanel_MouseMove);
			//
			// mHeaderMouseExpandPanel2
			//
			this.mHeaderMouseExpandPanel2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.mHeaderMouseExpandPanel2.BackColor = Color.Transparent;
			this.mHeaderMouseExpandPanel2.Location = new Point(0, 0);
			this.mHeaderMouseExpandPanel2.Size = new Size(this.Width, 2);
			this.mHeaderMouseExpandPanel2.Visible = false;
			this.mHeaderMouseExpandPanel2.MouseDown += new MouseEventHandler(mHeaderMouseExpandPanel_MouseDown);
			this.mHeaderMouseExpandPanel2.MouseUp += new MouseEventHandler(mHeaderMouseExpandPanel_MouseUp);
			this.mHeaderMouseExpandPanel2.MouseMove += new MouseEventHandler(mHeaderMouseExpandPanel_MouseMove);
			//
			//
			// animationTimer
			//
			this.animationTimer.Enabled = true;
			this.animationTimer.Interval = 30;
			this.animationTimer.Tick += new System.EventHandler(this.animationTimer_Tick);
			//
			// ExpandPanel
			//
			this.mHeaderPanel.Controls.AddRange(new Control[] {controlPictureBox, this.expandPictureBox, this.headerButton, mHeaderMouseExpandPanel2});
			this.Controls.AddRange(new Control[] {mHeaderPanel, mHeaderMouseExpandPanel});
			this.Paint += new PaintEventHandler(ExpandPanel_Paint);
			this.ResumeLayout(false);
		}
		#endregion

		#region "Properties"
		#region "Overrides"
		/// <summary>
		/// The area in which child controls can reside.  Bounds returned excludes the header 
		/// and the lines drawn around the edges of the control.  Docked controls will obey these 
		/// bounds.
		/// </summary>
		public override Rectangle DisplayRectangle
		{
			get
			{
				switch (this.HeaderLocation) 
				{
					case HeaderLocation.Top :
						return new Rectangle(1, this.HeaderHeight, this.Width - 2, this.Height - this.HeaderHeight - 1);
					case HeaderLocation.Bottom : 
						return new Rectangle(1, 0, this.Width - 2, this.Height - this.HeaderHeight - 1);
					case HeaderLocation.Right : 
						return new Rectangle(1, 1, this.Width - this.HeaderHeight - 1, this.Height - 2);
					case HeaderLocation.Left : 
						return new Rectangle(this.HeaderHeight, 1, this.Width - this.HeaderHeight - 2, this.Height - 2);
					default :
						return new Rectangle(0, 0, this.Width, this.Height);
				}
			}
		}
		#endregion

		#region "Expansion Options"
		/// <summary>
		/// Tells whether the panel is expanded or not.  Get is equivalent to ExpandMode == ExpandMode.Expanded.
		/// </summary>
		[System.ComponentModel.DefaultValue(true)]
		[System.ComponentModel.Category("Appearance")]
		[System.ComponentModel.Description("Whether the panel is in the expanded or contracted state")]
		public bool IsExpanded
		{
			get
			{
				return this.ExpandMode == ExpandMode.Expanded;
			}
			set
			{
				// Set the checked state of the header button to the given value.
				// This will cause the header button handler to get run, doing 
				// the expand or contract of the panel.
				this.headerButton.Checked = value;
			}
		}

		/// <summary>
		/// Gets the current expanding/contracting mode of the control
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public ExpandMode ExpandMode
		{
			get
			{
				return mExpandMode;
			}
		}

		/// <summary>
		/// Gets or sets the expanded content height (or width) of the control.  This is the height of the 
		/// control minus the header.  Setting the height of the control when it's in the expanded 
		/// state will also set this 
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public int ExpandedContentHeight
		{
			get
			{
				return this.mExpandedContentHeight;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("ExpandedContentHeight", value, "Expanded content height must be >= 0");

				}
				this.mExpandedContentHeight = value;
				this.UpdateBoundsForState();
			}
		}

		[System.ComponentModel.Category("Expansion")]
		[System.ComponentModel.DefaultValue(DEFAULT_EXPANSION_TIME)]
		[Description("The amount of time (in milliseconds) for the panel to go from " + 
			 "fully expanded to fully contracted.")]
		public int ExpandTime
		{
			get
			{
				return mExpandTime;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("ExpandTime", value, "Must be >= 0");
				}
				this.mExpandTime = value;
			}
		}

		[System.ComponentModel.DefaultValue(DEFAULT_ANIMATION_STEP_TIME)]
		[Category("Expansion")]
		[Description("The amount of time between animation steps while expanding / contracting/")]
		public int ExpandAnimationStepTime
		{
			get
			{
				return this.animationTimer.Interval;
			}
			set
			{
				if (value < 10)
				{
					throw new ArgumentOutOfRangeException("ExpandAnimationStepTime", value,
						"Animation step time must be > 10 ms");
				}
				this.animationTimer.Interval = value;
			}
		}

		[DefaultValue(DEFAULT_MOUSE_EXPAND_HEIGHT)]
		[Description("The width/height of the area around the border within which the " + 
			 "mouse can be to enable resizing.")]
		[Category("Behavior")]
		public int MouseExpandHeight 
		{
			get 
			{
				return mMouseExpandHeight;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Must be > 0", "MouseExpandHeight");
				}
				mMouseExpandHeight = value;
				this.SetMouseExpanderBounds();
			}
		}

		[System.ComponentModel.Category("Behavior")]
		[System.ComponentModel.DefaultValue(false)]
		[Description("Controls whether the control is resizable using the mouse on the " + 
			 "side of the expand direction.")]
		public bool MouseResizeEnabled 
		{
			get 
			{
				return this.mHeaderMouseExpandPanel.Visible;
			}
			set 
			{
				this.mHeaderMouseExpandPanel.Visible = value;
				this.mHeaderMouseExpandPanel2.Visible = value;
				this.SetHeaderContextMenu();
			}
		}

		[Category("Behavior")]
		[Description("The direction the control expands in.")]
		[DefaultValue(DEFAULT_EXPAND_DIRECTION)]
		public ExpandDirection ExpandDirection 
		{
			get 
			{
				return mExpandDirection;
			}
			set 
			{
				mExpandDirection = value;
				UpdateMouseExpandCursor();
				SetMouseExpanderBounds();
				LoadExpandImageList();
				UpdateExpandImage();
			}
		}

		[Category("Appearance")]
		[Description("Which side of the control the header appears on.")]
		[DefaultValue(DEFAULT_HEADER_LOCATION)]
		public HeaderLocation HeaderLocation 
		{
			get 
			{
				return mHeaderLocation;
			}
			set 
			{
				mHeaderLocation = value;
				this.SetHeaderAndImageBounds();
				this.PerformLayout();
			}
		}
		#endregion

		#region "Header Text"
		[System.ComponentModel.Category("Header")]
		[System.ComponentModel.DefaultValue("")]
		[Description("The text of the header.")]
		public String HeaderText
		{
			get
			{
				switch (this.HeaderTextMode)
				{
					case HeaderTextSwitchingMode.AppendShowHide :
						return this.mHeaderText;
					case HeaderTextSwitchingMode.ExpandContractText :
						if (this.ExpandMode == ExpandMode.Contracted || this.ExpandMode == ExpandMode.Contracting)

						{
							return this.HeaderExpandText;
						}
						else
						{
							return this.HeaderContractText;
						}
					default:
						return this.headerButton.Text;
				}
			}
			set
			{
				this.mHeaderText = value;
				this.UpdateHeaderText();
			}
		}

		[System.ComponentModel.Category("Header")]
		[System.ComponentModel.DefaultValue(ExpandPanel.DEFAULT_HEADER_TEXT_MODE)]
		[Description("Controls how text is switched in the header depending on the " + 
			 "expansion state of the control.")]
		public HeaderTextSwitchingMode HeaderTextMode
		{
			get
			{
				return this.mHeaderTextMode;
			}
			set
			{
				this.mHeaderTextMode = value;
				this.UpdateHeaderText();
			}
		}

		[System.ComponentModel.Category("Header")]
		[System.ComponentModel.DefaultValue("")]
		[Description("The text of the header when expanded, if the mode is set to ExpandContractText")]
		public String HeaderExpandText
		{
			get
			{
				return this.mHeaderExpandText;
			}
			set
			{
				this.mHeaderExpandText = value;
				this.UpdateHeaderText();
			}
		}

		[System.ComponentModel.Category("Header")]
		[System.ComponentModel.DefaultValue("")]
		[Description("The text of the header when contracted, if the mode is set to ExpandContractText")]
		public String HeaderContractText
		{
			get
			{
				return this.mHeaderContractText;
			}
			set
			{
				this.mHeaderContractText = value;
				this.UpdateHeaderText();
			}
		}
		#endregion

		#region "Images"
		/// <summary>
		/// Controls how the expand image is positioned in the picture box holder.
		/// </summary>
		[System.ComponentModel.Category("Images")]
		[System.ComponentModel.Description("Controls how the expand image is positioned " + 
			 "within the picture box at the right/bottom of the header.")]
		[System.ComponentModel.DefaultValue(PictureBoxSizeMode.StretchImage)]
		public PictureBoxSizeMode ExpandImageSizeMode 
		{
			get 
			{
				return expandPictureBox.SizeMode;
			}
			set 
			{
				expandPictureBox.SizeMode = value;
			}
		}

		[Category("Images")]
		[Description("Controls how the image for the control is positioned " + 
			 " within the picture box at the left / top of the header.")]
		[DefaultValue(PictureBoxSizeMode.StretchImage)]
		public PictureBoxSizeMode HeaderImageSizeMode 
		{
			get 
			{
				return controlPictureBox.SizeMode;
			}
			set 
			{
				controlPictureBox.SizeMode = value;
			}
		}

		/// <summary>
		/// The width / height of the expand image picture box.
		/// </summary>
		[System.ComponentModel.Category("Images")]
		[System.ComponentModel.DefaultValue(DEFAULT_EXPAND_IMAGE_WIDTH)]
		[System.ComponentModel.Description("The width of the expand image picture box.")]
		public int ExpandImageWidth
		{
			get
			{
				return this.expandPictureBox.Width;
			}
			set
			{
				this.expandPictureBox.Width = value;
				this.SetHeaderAndImageBounds();
			}
		}

		[Category("Images")]
		[DefaultValue(DEFAULT_EXPAND_IMAGE_WIDTH)]
		[Description("Ths width of the image at the left/top of the header.")]
		public int HeaderImageWidth 
		{
			get 
			{
				return this.controlPictureBox.Width;
			} 
			set 
			{
				this.controlPictureBox.Width = value;
				this.SetHeaderAndImageBounds();
			}
		}

		[System.ComponentModel.Category("Images")]
		[System.ComponentModel.DefaultValue(null)]
		[System.ComponentModel.Description("The set of images used in the upper-right " + 
			 "corner of the control, if ExpandImageMode is AutoList or CustomList.")]
		public ImagesList ExpandImageList
		{
			get
			{
				return this.mExpandPictureImagesList;
			}
			set
			{
				this.mExpandPictureImagesList = value;
				this.LoadExpandImageList();
				this.UpdateExpandImage();
			}
		}

		private bool ShouldSerializeExpandImageList() 
		{
			return this.ExpandImageMode != HeaderImageSwitchingMode.AutoList;
		}

		[System.ComponentModel.Category("Images")]
		[System.ComponentModel.DefaultValue(null)]
		[System.ComponentModel.Description("The image used in the right / bottom of the " + 
			 "control when the mode is Custom.")]
		public Image ExpandImage
		{
			get
			{
				return this.expandPictureBox.Image;
			}
			set
			{
				this.expandPictureBox.Image = value;
				this.UpdateExpandImage();
			}
		}

		/// <summary>
		/// Only need VS to serialize it to code if the mode is custom
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializeExpandImage() 
		{
			return this.ExpandImageMode == HeaderImageSwitchingMode.Custom;
		}

		[Category("Images")]
		[DefaultValue(null)]
		[Description("The image used in the left / top of the control when the mode is Custom.")]
		public Image HeaderImage 
		{
			get 
			{
				return this.controlPictureBox.Image;
			}
			set 
			{
				this.controlPictureBox.Image = value;
				this.UpdateHeaderImage();
			}
		}

		[System.ComponentModel.DefaultValue(DEFAULT_IMAGE_SWITCHING_MODE)]
		[System.ComponentModel.Category("Images")]
		[Description("Controls how images are switched in the picture on the right / bottom of the header.")]
		public HeaderImageSwitchingMode ExpandImageMode
		{
			get
			{
				return this.mExpandImageMode;
			}
			set
			{
				this.mExpandImageMode = value;
				this.LoadExpandImageList();
				this.UpdateExpandImage();
			}
		}

		[System.ComponentModel.DefaultValue(DEFAULT_IMAGE_SWITCHING_MODE)]
		[System.ComponentModel.Category("Images")]
		public HeaderImageSwitchingMode HeaderImageMode
		{
			get
			{
				return this.mHeaderImageMode;
			}
			set
			{
				this.mHeaderImageMode = value;
				this.UpdateHeaderImage();
			}
		}

		/// <summary>
		/// The image list used for the left or top of the header.
		/// </summary>
		[System.ComponentModel.DefaultValue(null)]
		[System.ComponentModel.Category("Images")]
		[System.ComponentModel.Description("The list of images used on the left/top of the header " + 
			 " if the mode is set to CustomList or AutoList.")]
		public ImagesList HeaderImagesList 
		{
			get 
			{
				return this.mHeaderPictureImageList;
			}
			set 
			{
				this.mHeaderPictureImageList = value;
				this.UpdateHeaderImage();
			}
		}

		/// <summary>
		/// The background image for the header.
		/// </summary>
		[System.ComponentModel.Description("The background image for the header.")]
		[System.ComponentModel.DefaultValue(null)]
		[System.ComponentModel.Category("Header")]
		public Image HeaderBackgroundImage
		{
			get
			{
				return this.mHeaderPanel.BackgroundImage;
			}
			set
			{
				this.mHeaderPanel.BackgroundImage = value;
			}
		}
		#endregion

		#region "Header Passthroughs"
		#region "Font"
		[System.ComponentModel.Category("Header")]
		[Description("The font used in the header.")]
		public Font HeaderFont
		{
			get
			{
				return this.mHeaderPanel.Font;
			}
			set
			{
				this.mHeaderFont = value;
				this.mHeaderPanel.Font = value;
			}
		}

		public void ResetHeaderFont()
		{
			this.HeaderFont = null;
		}

		public bool ShouldSerializeHeaderFont()
		{
			return this.mHeaderFont != null;
		}

		[System.ComponentModel.Category("Header")]
		[System.ComponentModel.DefaultValue(TextDirection.Horiontal)]
		[Description("The direction of the text in the header.")]
		public TextDirection HeaderTextDirection 
		{
			get 
			{
				return this.headerButton.TextDirection;
			}
			set 
			{
				this.headerButton.TextDirection = value;
				this.Invalidate();
			}
		}
		#endregion

		#region "Coloring"
		[System.ComponentModel.Category("Header")]
		[Description("The backcolor for the header.")]
		public Color HeaderBackColor
		{
			get
			{
				return this.mHeaderPanel.BackColor;
			}
			set
			{
				this.mHeaderBackColor = value;
				this.mHeaderPanel.BackColor = value;
			}
		}

		private void ResetHeaderBackColor()
		{
			this.HeaderBackColor = Color.Empty;
		}

		private bool ShouldSerializeHeaderBackColor()
		{
			return !this.mHeaderBackColor.Equals(Color.Empty);
		}
		
		[System.ComponentModel.Category("Header")]
		[Description("The forecolor of the header.")]
		public Color HeaderForeColor 
		{
			get 
			{
				return this.mHeaderPanel.ForeColor;
			}
			set 
			{
				this.mHeaderPanel.ForeColor = value;
				this.mHeaderForeColor = value;
			}
		}

		private void ResetHeaderForeColor() 
		{
			this.HeaderForeColor = Color.Empty;
		}

		private bool ShouldSerializeHeaderForeColor()
		{
			return !this.mHeaderForeColor.Equals(Color.Empty);
		}
		#endregion

		[System.ComponentModel.Category("Appearance")]
		[System.ComponentModel.DefaultValue(DEFAULT_HEADER_HEIGHT)]
		[Description("The height/width of the header.")]
		public int HeaderHeight
		{
			get
			{
				return this.mHeaderHeight;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("HeaderHeight", value, "Header must have positive height");

				}
				this.mHeaderHeight = value;
				this.SetHeaderAndImageBounds();
			}
		}

		[System.ComponentModel.Category("Header")]
		[System.ComponentModel.DefaultValue(ContentAlignment.MiddleLeft)]
		[Description("The alignment of text in the header")]
		public ContentAlignment HeaderTextAlign
		{
			get
			{
				return this.headerButton.TextAlign;
			}
			set
			{
				this.headerButton.TextAlign = value;
			}
		}

		[System.ComponentModel.AmbientValue(RightToLeft.Inherit)]
		[System.ComponentModel.DefaultValue(RightToLeft.Inherit)]
		[System.ComponentModel.Category("Header")]
		[Description("Right to left of the header")]
		public RightToLeft HeaderRightToLeft
		{
			get
			{
				return this.headerButton.RightToLeft;
			}
			set
			{
				this.headerButton.RightToLeft = value;
			}
		}

		#endregion
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			this.animationTimer.Enabled = false;
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region "Header Text and Image Updating"
		/// <summary>
		/// Updates the header text and images to reflect the current expansion state
		/// of the control.
		/// </summary>
		private void UpdateHeaderTextAndImageForMode() 
		{
			this.UpdateHeaderImage();
			this.UpdateHeaderText();
			this.UpdateExpandImage();
		}

		/// <summary>
		/// Updates the text of the header given the current state
		/// </summary>
		private void UpdateHeaderText()
		{
			//Console.WriteLine("Header Text Mode: {0}", this.HeaderTextMode);
			if (this.mInited) 
			{
				switch (this.HeaderTextMode)
				{
					case HeaderTextSwitchingMode.AppendShowHide :
						if (this.ExpandMode == ExpandMode.Contracted || this.ExpandMode == ExpandMode.Contracting)

						{
							this.headerButton.Text = this.mHeaderText + " (Show)";
						}
						else
						{
							this.headerButton.Text = this.mHeaderText + " (Hide)";
						}
						break;
					case HeaderTextSwitchingMode.ExpandContractText :
						if (this.ExpandMode == ExpandMode.Contracted || this.ExpandMode == ExpandMode.Contracted)

						{
							this.headerButton.Text = this.HeaderExpandText;
						}
						else
						{
							this.headerButton.Text = this.HeaderContractText;
						}
						break;
					case HeaderTextSwitchingMode.Custom :
						// do nothing
						break;
				}
			}
		}

		private void UpdateHeaderImage() 
		{
			this.UpdatePictureBoxFromImageList(this.controlPictureBox, this.HeaderImagesList);
		}

		private void UpdateExpandImage() 
		{
			this.UpdatePictureBoxFromImageList(this.expandPictureBox, ExpandImageList);
		}

		/// <summary>
		/// Loads up the automatic image list for the current expand direction, 
		/// if the mode is AutoList
		/// </summary>
		private void LoadExpandImageList() 
		{
			if (this.ExpandImageMode == HeaderImageSwitchingMode.AutoList) 
			{
				switch (this.ExpandDirection) 
				{
					case ExpandDirection.Up : 
						this.mExpandPictureImagesList = ExpandPanelAutoPictureUtils.UpImagesList;
						break;
					case ExpandDirection.Down : 
						this.mExpandPictureImagesList = ExpandPanelAutoPictureUtils.DownImagesList;
						break;
					case ExpandDirection.Left : 
						this.mExpandPictureImagesList = ExpandPanelAutoPictureUtils.LeftImagesList;
						break;
					case ExpandDirection.Right : 
						this.mExpandPictureImagesList = ExpandPanelAutoPictureUtils.RightImagesList;
						break;
				}
			}
		}

		private void UpdatePictureBoxFromImageList(PictureBox box, ImagesList imagesList) 
		{
			box.Image = this.GetImageForStateFromImagesList(imagesList);
		}

		/// <summary>
		/// Gets the correct image from a list for the given expand/contract state.
		/// </summary>
		/// <param name="list"></param>
		private Image GetImageForStateFromImagesList(ImagesList list) 
		{
			int index = 0;
			if (Enabled) 
			{
				switch (this.ExpandMode) 
				{
					case ExpandMode.Contracted :
						index = 1;
						break;
					case ExpandMode.Contracting :
						index = 5;
						break;
					case ExpandMode.Expanded : 
						index = 0;
						break;
					case ExpandMode.Expanding : 
						index = 4;
						break;
				}
			} 
			else 
			{
				switch (this.ExpandMode) 
				{
					case ExpandMode.Contracted :
						index = 3;
						break;
					case ExpandMode.Contracting :
						index = 7;
						break;
					case ExpandMode.Expanded : 
						index = 2;
						break;
					case ExpandMode.Expanding : 
						index = 6;
						break;
				}
			}
			return GetImageForIndexFromImagesList(list, index);
		}

		/// <summary>
		/// Updates the cursor used for using the mouse to expand the control.
		/// </summary>
		private void UpdateMouseExpandCursor() 
		{
			if (this.IsExpanded) 
			{
				if (IsHorizontalExpand()) 
				{
					mHeaderMouseExpandPanel.Cursor = Cursors.VSplit;
					mHeaderMouseExpandPanel2.Cursor = Cursors.VSplit;
				} 
				else 
				{
					mHeaderMouseExpandPanel.Cursor = Cursors.HSplit;
					mHeaderMouseExpandPanel2.Cursor = Cursors.HSplit;
				}
			} 
			else 
			{
				// otherwise the mouse expand panels should be invisible, so use the 
				// same cursor as for this control.
				mHeaderMouseExpandPanel.Cursor = this.Cursor;
				mHeaderMouseExpandPanel2.Cursor = this.Cursor;
			}
		}

		/// <summary>
		/// Gets an expand image from a list for the given index, falling back 
		/// on lower index defaults if necessary.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="index"></param>
		private Image GetImageForIndexFromImagesList(ImagesList list, int index) 
		{
			if (list == null) 
			{
				return null;
			}
			if (index >= list.Images.Count) 
			{
				switch (index) 
				{
					case 0 : 
						return null;
					case 1 :
					case 2 :
					case 4 :
						return GetImageForIndexFromImagesList(list, 0); 
					case 3 :
					case 5 :
						return GetImageForIndexFromImagesList(list, 3);
					case 6 : 
						return GetImageForIndexFromImagesList(list, 2);
					case 7 :
						return GetImageForIndexFromImagesList(list, 3);
					default : 
						return null;
				}
			}
			else 
			{
				return list.Images[index];
			}
		}
		#endregion

		#region "Expansion/Contraction"
		/// <summary>
		/// Starts a contraction
		/// </summary>
		/// <param name="contractionStart"></param>
		protected virtual void BeginContraction(System.DateTime contractionStart)
		{
			inExpandContract = true;
			this.animationTimer.Start();
			//Console.WriteLine("Began contraction");
			this.mExpansionStartTime = contractionStart;
			this.mExpandMode = ExpandMode.Contracting;
			this.OnContracting();
			this.UpdateHeaderTextAndImageForMode();
			if (this.ShouldImmediatelyExpand())
			{
				EndContraction();
			}
			//Console.WriteLine("Began end contraction");
		}

		/// <summary>
		/// Ends a contraction
		/// </summary>
		protected virtual void EndContraction()
		{
			inExpandContract = false;
			this.animationTimer.Stop();
			//Console.WriteLine("End contraction {0}", this.Name);
			// Set the height of the control to the height of the header
			this.mExpandMode = ExpandMode.Contracted;
			this.UpdateBoundsForState();
			this.UpdateHeaderTextAndImageForMode();
			this.OnContracted();
		}

		/// <summary>
		/// Starts an expansion.
		/// </summary>
		/// <param name="expansionStart"></param>
		protected virtual void BeginExpansion(System.DateTime expansionStart)
		{
			inExpandContract = true;
			this.animationTimer.Start();
			//Console.WriteLine("Began expansion");
			this.mExpansionStartTime = expansionStart;
			this.mExpandMode = ExpandMode.Expanding;
			this.OnExpanding();
			this.UpdateHeaderTextAndImageForMode();
			if (this.ShouldImmediatelyExpand())
			{
				EndExpansion();
			}
		}

		/// <summary>
		/// Ends an expansion.
		/// </summary>
		protected virtual void EndExpansion()
		{
			inExpandContract = false;
			this.animationTimer.Stop();
			//Console.WriteLine("End expansion {0}", this.Name);
			// Set the height of the control to the height of the header and expanded height
			this.mExpandMode = ExpandMode.Expanded;
			this.UpdateBoundsForState();
			this.UpdateHeaderTextAndImageForMode();
			this.OnExpanded();
		}

		/// <summary>
		/// Tells whether an expansion should take place immediately instead
		/// of doing the drop-down animation.
		/// </summary>
		/// <returns></returns>
		protected virtual bool ShouldImmediatelyExpand()
		{
			return (this.DesignMode || this.ExpandTime < this.ExpandAnimationStepTime || !this.mInited);
		}

		/// <summary>
		/// Programmatic method to animate the control to it's expanded state.  If the 
		/// control is already expanded, nothing is done.   If the control is expanding, this 
		/// expansion continues.  If the control is contracting or contracted, an expandsion 
		/// starts.
		/// </summary>
		public void Expand() 
		{
			this.headerButton.Checked = true;
		}

		/// <summary>
		/// Actually starts an expansion.
		/// </summary>
		private void ExpandInternal() 
		{
			//Console.WriteLine("Expand");
			switch (ExpandMode) 
			{
				case ExpandMode.Expanded :
					break;
				case ExpandMode.Expanding :
					break;
				case ExpandMode.Contracting :
					// The time that the contraction "started" is equal to the
					// current time minus the time remaining in the contraction
					DateTime startTime = DateTime.Now;
					int millisecondsToSubtract;
					if (this.ExpandedContentHeight <= 0)
					{
						millisecondsToSubtract = this.ExpandTime;
					}
					else
					{
						millisecondsToSubtract = this.ExpandTime * (ContentHeight) / (ExpandedContentHeight);
					}
					startTime = startTime.Subtract(new TimeSpan(0,0,0,0,millisecondsToSubtract));
					this.BeginExpansion(startTime);
					break;
				case ExpandMode.Contracted : 
					this.BeginExpansion(DateTime.Now);
					break;
			}
		}

		/// <summary>
		/// Like Expand(), but the other way.
		/// </summary>
		public void Contract() 
		{
			this.headerButton.Checked = false;
		}

		/// <summary>
		/// Actually starts a contraction.
		/// </summary>
		private void ContractInternal() 
		{
			switch (ExpandMode) 
			{
				case ExpandMode.Expanded :
					this.BeginContraction(DateTime.Now);
					break;
				case ExpandMode.Expanding :
					// The time that the expansion "started" is equal to the
					// current time minus the time remaining in the expansion
					DateTime startTime = DateTime.Now;
					int millisecondsToSubtract;
					if (this.ExpandedContentHeight <= 0)
					{
						millisecondsToSubtract = this.ExpandTime;
					}
					else
					{
						millisecondsToSubtract = this.ExpandTime - this.ExpandTime *
							(this.ContentHeight) / (this.ExpandedContentHeight);
					}
					startTime = startTime.Subtract(new TimeSpan(0,0,0,0,millisecondsToSubtract));

					this.BeginContraction(startTime);
					break;
				case ExpandMode.Contracting :
					break;
				case ExpandMode.Contracted : 
					break;
			}
		}

		/// <summary>
		/// Sets the bounds of the header panel and lays out its contents.
		/// </summary>
		private void SetHeaderAndImageBounds() 
		{
			switch (HeaderLocation) 
			{
				case HeaderLocation.Top : 
					this.mHeaderPanel.Bounds = new Rectangle(0, 0, this.Width, this.HeaderHeight);
					break;
				case HeaderLocation.Bottom :
					this.mHeaderPanel.Bounds = new Rectangle(0, this.Height - this.HeaderHeight, this.Width, this.HeaderHeight);
					break;
				case HeaderLocation.Left : 
					this.mHeaderPanel.Bounds = new Rectangle(0, 0, this.HeaderHeight, this.Height);
					break;
				case HeaderLocation.Right : 
					this.mHeaderPanel.Bounds = new Rectangle(this.Width - this.HeaderHeight, 0, this.HeaderHeight, this.Height);
					break;
			}
			this.LayoutHeaderPanel();
		}

		/// <summary>
		/// Gets the fraction the control is currently expanded/contracted, with 
		/// 1 meaning fully expanded and 0 meaning fully contracted.
		/// </summary>
		/// <returns></returns>
		private double GetExpandFractionForState() 
		{
			double fractionExpanded = 0;
			TimeSpan sinceStart = DateTime.Now.Subtract(mExpansionStartTime);
			switch (ExpandMode) 
			{
				case ExpandMode.Expanded :
					fractionExpanded = 1;
					break;
				case ExpandMode.Expanding : 
					if (this.ExpandTime != 0 && this.ExpandedContentHeight > 0) 
					{
						int contentHeight = (ExpandedContentHeight) 
							* MillisecondsFromTimeSpan(sinceStart) / this.ExpandTime;
						fractionExpanded = (double) contentHeight / this.ExpandedContentHeight;
					} 
					else 
					{
						fractionExpanded = 1;
					}
					break;
				case ExpandMode.Contracting : 
					if (this.ExpandTime != 0 && this.ExpandedContentHeight > 0) 
					{
						int contentHeight = ExpandedContentHeight - (ExpandedContentHeight)
							* MillisecondsFromTimeSpan(sinceStart) / this.ExpandTime;
						fractionExpanded = (double) contentHeight / this.ExpandedContentHeight;
					} 
					else 
					{
						fractionExpanded = 0;
					}
					break;
				case ExpandMode.Contracted :
					fractionExpanded = 0;
					break;
			}
			return Math.Min(Math.Max(fractionExpanded, 0), 1);
		}

		/// <summary>
		/// Updates the bounds based on the current state and expand direction.
		/// </summary>
		private void UpdateBoundsForState() 
		{
			Size size;
			if (this.IsHorizontalExpand()) 
			{
				size = new Size(this.GetExpandedDirectionSizeForState(), this.Height);
			} 
			else 
			{
				size = new Size(this.Width, this.GetExpandedDirectionSizeForState());
			}
			//Console.WriteLine("Expanded height in UpdateBoundsForState {0} {1}", this.Name, size.Height);
			//Console.WriteLine("Size for state {0}", size);
			Rectangle bounds = this.Bounds;
			switch (ExpandDirection) 
			{
				case ExpandDirection.Down : 
					// keep width the same and top edge fixed.
					bounds = new Rectangle(this.Location, size);
					break;
				case ExpandDirection.Up : 
					// keep width the same and bottom edge fixed.
					bounds = new Rectangle(new Point(this.Left, this.Bottom - size.Height), size);
					break;
				case ExpandDirection.Left : 
					// keep height the same and right edge fixed.
					bounds = new Rectangle(new Point(this.Right - size.Width, this.Top), size);
					break;
				case ExpandDirection.Right :
					// keep height the same and left edge fixed.
					bounds = new Rectangle(this.Location, size);
					break;
			}
			Console.WriteLine("UpdateBounds {0} {1} {2} {3}", bounds.Left, 
				bounds.Top, bounds.Right, bounds.Bottom);
			base.SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, BoundsSpecified.None);
			if (this.Parent != null) 
			{
				this.Parent.PerformLayout(this, "None");
			}
		}

		/// <summary>
		/// Gets the size of the control for the currently set ExpandedContentHeight and 
		/// state.
		/// </summary>
		/// <returns></returns>
		private int GetExpandedDirectionSizeForState() 
		{
			double fractionExpanded = this.GetExpandFractionForState();
			if (this.IsHorizontalExpand()) 
			{
				return ((int) Math.Round(fractionExpanded * ExpandedContentHeight + HeaderHeight, 0));
			} 
			else 
			{
				return ((int) Math.Round(fractionExpanded * ExpandedContentHeight + HeaderHeight, 0));
			}
		}

		/// <summary>
		/// Sets the bounds for the two panels that handle mouse expand clicks.
		/// </summary>
		private void SetMouseExpanderBounds() 
		{
			// The bounds for the mouse expand panel that is a child of the main panel.
			Rectangle bounds = new Rectangle(0, 0, 0, 0);
			// The bounds for the mouse expand panel that is a child of the header panel.
			Rectangle bounds2 = new Rectangle(0, 0, 0, 0);
			switch (ExpandDirection) 
			{
				case ExpandDirection.Down :
					bounds = new Rectangle(0, this.Height - MouseExpandHeight, this.Width, MouseExpandHeight);
					break;
				case ExpandDirection.Up :
					bounds = new Rectangle(0, 0, this.Width, MouseExpandHeight);
					break;
				case ExpandDirection.Right :
					bounds = new Rectangle(this.Width - MouseExpandHeight, 0, MouseExpandHeight, this.Height);
					break;
				case ExpandDirection.Left : 
					bounds = new Rectangle(0, 0, MouseExpandHeight, this.Height);
					break;
			}
			// translate for the offset of the header panel relative to this control as a whole
			bounds2 = new Rectangle(bounds.Left - mHeaderPanel.Left, bounds.Top - mHeaderPanel.Left, 
				bounds.Width, bounds.Height);
			this.mHeaderMouseExpandPanel.Bounds = bounds;
			this.mHeaderMouseExpandPanel2.Bounds = bounds;
		}

		/// <summary>
		/// Gets the height of the content - in the direction of the expand.
		/// </summary>
		private int ContentHeight
		{
			get
			{
				if (IsHorizontalExpand()) 
				{
					return this.Width - this.HeaderHeight;
				} 
				else 
				{
					return this.Height - this.HeaderHeight;
				}
			}
		}

		/// <summary>
		/// Responds to the header being pressed, changing the expansion/contraction
		/// mode of the control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void headerButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (DesignMode) 
			{
				return;
			}
			//Console.WriteLine("Checked Changed");
			bool show = this.headerButton.Checked;
			this.mHeaderPanel.Invalidate();
			
			if (show) 
			{
				this.ExpandInternal();
			} 
			else 
			{
				this.ContractInternal();
			}
			//Console.WriteLine("End check changed");
		}

		/// <summary>
		/// Updates control height
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void animationTimer_Tick(object sender, System.EventArgs e)
		{
			if (inExpandContract) 
			{
				//Console.WriteLine("Animation Tick");
				// Determine if the amount of time that the expansion has been 
				// going on for is more than the time set for an expansion to take place.
				TimeSpan elapsed = DateTime.Now.Subtract(this.mExpansionStartTime);
				bool finished = elapsed > new TimeSpan(0, 0, 0, 0, this.ExpandTime) || this.ShouldImmediatelyExpand();

				switch (this.ExpandMode)
				{
					case ExpandMode.Expanding :
						//Console.WriteLine("Animation Tick Expanding");
						// update the height of the control to the correct value
						if (finished)
						{
							this.EndExpansion();
						}
						else
						{
							UpdateBoundsForState();
						}
						break;
					case ExpandMode.Contracting :
						if (finished)
						{
							this.EndContraction();
						}
						else
						{
							UpdateBoundsForState();
						}
						break;
				}
			}
		}

		#endregion

		#region Helpers
		/// <summary>
		/// Immediately updates the state to that specified by the Checked value
		/// of the headerButton, skipping animation if necessary.
		/// </summary>
		private void UpdateStateImmediate()
		{
			//Console.WriteLine("Updating state");
			if (this.headerButton.Checked)
			{
				this.EndExpansion();
			}
			else
			{
				this.EndContraction();
			}
			this.UpdateHeaderTextAndImageForMode();
		}

		/// <summary>
		/// Redirects click on non-header button parts of the header to change 
		/// the button state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Header_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Clicks == 1 && e.Button == MouseButtons.Left) 
			{
				this.headerButton.Checked = !this.headerButton.Checked;
			}
		}

		private bool IsHorizontalExpand() 
		{
			return ExpandDirection == ExpandDirection.Left || ExpandDirection == ExpandDirection.Right;
		}

		/// <summary>
		/// Sets the context menu on the panel to include the SetHeight item, if no 
		/// other menu is set, and mouse resizing is enabled.
		/// </summary>
		private void SetHeaderContextMenu() 
		{
			ContextMenu menu = null;
			if (this.MouseResizeEnabled) 
			{
				menu = this.mHeaderContextMenu;
			}
			this.mHeaderPanel.ContextMenu = menu;
			this.headerButton.ContextMenu = menu;
			this.mHeaderMouseExpandPanel2.ContextMenu = menu;
			this.controlPictureBox.ContextMenu = menu;
			this.expandPictureBox.ContextMenu = menu;
			if (this.ContextMenu == null || this.ContextMenu == this.mHeaderContextMenu) 
			{
				this.ContextMenu = menu;
			}
		}

		/// <summary>
		/// Gets the total number of milliseconds in a time span
		/// </summary>
		/// <param name="ts"></param>
		/// <returns></returns>
		private int MillisecondsFromTimeSpan(TimeSpan ts)
		{
			return ((((ts.Days * 24) + ts.Hours) * 60 + ts.Minutes) * 60 + ts.Seconds) * 1000 + ts.Milliseconds;
		}
		#endregion

		#region ISupportInitialize Members

		public void BeginInit()
		{
			this.mInited = false;
		}

		public void EndInit()
		{
			this.mInited = true;
			this.LoadExpandImageList();
			this.UpdateStateImmediate();
		}

		#endregion

		#region "Internal layout"
		/// <summary>
		/// Lays out the parts of the header panel based on the current settings of this 
		/// control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LayoutHeaderPanel() 
		{
			if (HeaderLocation == HeaderLocation.Top || HeaderLocation == HeaderLocation.Bottom) 
			{
				// if header is on top or bottom of control, then layout as
				// /----------------------------------------\
				// |HeaderImage | Header Text | ExpandImage |
				// \----------------------------------------/
				this.expandPictureBox.Bounds = 
					new Rectangle(this.Width - this.ExpandImageWidth - 2, 2, this.ExpandImageWidth, this.HeaderHeight - 4);
				this.controlPictureBox.Bounds = 
					new Rectangle(2, 2, this.HeaderImageWidth, this.HeaderHeight - 4);
				int headerWidth = Math.Max(this.Width - this.ExpandImageWidth - this.HeaderImageWidth - 8, 0);
				this.headerButton.Bounds = 
					new Rectangle(this.HeaderImageWidth + 4, 2, headerWidth, this.HeaderHeight - 4);
			} 
			else 
			{
				// if header is on right or left of control, then layout as
				// /-------------------\
				// |   Header Image    |
				// |-------------------|
				// |   Header Label    |
				// |-------------------|
				// |   Expand Image    |
				// \-------------------/
				this.expandPictureBox.Bounds = 
					new Rectangle(2, this.Height - this.ExpandImageWidth - 2, this.HeaderHeight - 4, this.ExpandImageWidth);
				this.controlPictureBox.Bounds = 
					new Rectangle(2, 2, this.HeaderHeight - 4, this.HeaderImageWidth);
				int headerHeight = Math.Max(this.Height - this.ExpandImageWidth - this.HeaderImageWidth - 8, 0);
				this.headerButton.Bounds = 
					new Rectangle(2, this.HeaderImageWidth + 4, this.HeaderHeight - 4, headerHeight);
			}
		}

		private void mHeaderPanel_Layout(object sender, LayoutEventArgs e)
		{
			this.LayoutHeaderPanel();
		}
		#endregion

		#region "Painting"
		/// <summary>
		/// Draws a border around the header panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mHeaderPanel_Paint(object sender, PaintEventArgs e)
		{
			using (Pen p = new Pen(ControlPaint.Dark(this.mHeaderPanel.BackColor, 1))) 
			{
				ButtonBorderStyle style;
				Color c;
				int width;
				if (headerButton.Checked) 
				{
					style = ButtonBorderStyle.Solid;
					c = ControlPaint.Dark(HeaderBackColor);
					width = 1;
				}
				else 
				{
					c = HeaderBackColor;
					style = ButtonBorderStyle.Outset;
					width = 1;
				}
				p.Width = 1;
				//e.Graphics.DrawRectangle(p, 0, 0, mHeaderPanel.Width - 1, mHeaderPanel.Height - 1);
				ControlPaint.DrawBorder(e.Graphics, new Rectangle(new Point(0, 0), mHeaderPanel.Size), 
					c, width, style, 
					c, width, style, 
					c, width, style, 
					c, width, style);
					
			}
		}

		/// <summary>
		/// Draws the border around the entire panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExpandPanel_Paint(object sender, PaintEventArgs e)
		{
			int width = 1;
			ButtonBorderStyle style = ButtonBorderStyle.Solid;
			Color c = ControlPaint.Dark(this.BackColor);

			Graphics g = e.Graphics;
			GraphicsContainer container = g.BeginContainer();
			Region r = g.Clip;
			r.Exclude(this.mHeaderPanel.Bounds);
			g.Clip = r;
			ControlPaint.DrawBorder(g, new Rectangle(new Point(0, 0), this.Size),
				c, width, style,
				c, width, style,
				c, width, style,
				c, width, style);

			g.EndContainer(container);
		}

		private void mHeaderPanel_Resize(object sender, EventArgs e)
		{
			mHeaderPanel.Invalidate();
		}
		#endregion

		#region "Mouse Expansion"
		/// <summary>
		/// The point at which the current mouse expand drag began (in 
		/// whichever control the drag started in).
		/// </summary>
		private Point mMouseExpandDragStartPoint;
		/// <summary>
		/// True if in a mouse resize drag, false otherwise.
		/// </summary>
		private bool mInMouseExpand = false;

		/// <summary>
		/// Begins a mouse expand if necessary.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mHeaderMouseExpandPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && e.Clicks == 1  && this.IsExpanded) 
			{
				mInMouseExpand = true;
				mMouseExpandDragStartPoint = new Point(e.X, e.Y);
			}
		}

		/// <summary>
		/// Updates the size of the control while a mouse expand is taking place
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mHeaderMouseExpandPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (mInMouseExpand) 
			{
				this.SetSizeFromMouseExpand(new Point(e.X, e.Y));
			}
		}

		/// <summary>
		/// Ends a mouse expand.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mHeaderMouseExpandPanel_MouseUp(object sender, MouseEventArgs e)
		{
			if (mInMouseExpand) 
			{
				mInMouseExpand = false;
				this.SetSizeFromMouseExpand(new Point(e.X, e.Y));
			}
		}

		/// <summary>
		/// Sets the size based on the amount moved during a mouse expansion.
		/// </summary>
		/// <param name="at"></param>
		private void SetSizeFromMouseExpand(Point at) 
		{
			int mouseMoved = 0;
			switch (ExpandDirection) 
			{
				case ExpandDirection.Down : 
					mouseMoved = at.Y - mMouseExpandDragStartPoint.Y;
					break;
				case ExpandDirection.Up : 
					mouseMoved = mMouseExpandDragStartPoint.Y - at.Y;
					break;
				case ExpandDirection.Left : 
					mouseMoved = mMouseExpandDragStartPoint.X - at.X;
					break;
				case ExpandDirection.Right : 
					mouseMoved = at.X - mMouseExpandDragStartPoint.X;
					break;
			}
			this.ExpandedContentHeight = Math.Max(this.ExpandedContentHeight + mouseMoved, 0);
		}
		#endregion

		#region "Overrides"
		/// <summary>
		/// Overrides SetBoundsCore to decide whether to allow the bounds to be set, and whether 
		/// to update ExpandedContentHeight.  If ExpandMode == Expanded, then update the 
		/// ExpandedContentHeight of the control.  In any other state, ignore the given 
		/// bounds (supplied by an outside source, such as docking or anchoring), and 
		/// procede to make the control the height indicated by the current expansion state 
		/// and how long since the expansion/contraction started.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="specified"></param>
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			switch (ExpandMode) 
			{
				case ExpandMode.Contracted :
					break;
				case ExpandMode.Expanded :
					if (IsHorizontalExpand()) 
					{
						this.mExpandedContentHeight = Math.Max(0, width - HeaderHeight);
					} 
					else 
					{
						this.mExpandedContentHeight = Math.Max(0, height - HeaderHeight);
					}
					break;
				case ExpandMode.Contracting :
					break;
				case ExpandMode.Expanding :
					break;
			}
			if (IsHorizontalExpand()) 
			{
				// Don't change which edges are specified, because it messes up anchoring 
				// (and possibly docking).
				// specified = specified | BoundsSpecified.Width;
				width = this.GetExpandedDirectionSizeForState();
			} 
			else 
			{
				// specified = specified | BoundsSpecified.Height;
				height = this.GetExpandedDirectionSizeForState();
			}
			Size size = new Size(width, height);
			Rectangle bounds = new Rectangle(x, y, width, height);
			switch (ExpandDirection) 
			{
				case ExpandDirection.Down : 
					// keep width the same and top edge fixed.
					bounds = new Rectangle(new Point(x, y), size);
					break;
				case ExpandDirection.Up : 
					// keep width the same and bottom edge fixed.
					bounds = new Rectangle(new Point(x, y + height - size.Height), size);
					break;
				case ExpandDirection.Left : 
					// keep height the same and right edge fixed.
					bounds = new Rectangle(new Point(x + width - size.Width, y), size);
					break;
				case ExpandDirection.Right : 
					// keep height the same and left edge fixed.
					bounds = new Rectangle(new Point(x, y), size);
					break;
			}
			//Console.WriteLine("Bottom SetBoundsCore: {0}", bounds.Bottom);
			base.SetBoundsCore(bounds.Left, bounds.Top, bounds.Width, bounds.Height, specified);
			// Update the bounds of managed sub components
			this.SetHeaderAndImageBounds();
			this.SetMouseExpanderBounds();
		}

		/// <summary>
		/// Updates images when disabled / enabled.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			this.UpdateHeaderTextAndImageForMode();
		}
		#endregion

		private void mSetHeightMenuItem_Click(object sender, EventArgs e)
		{
			frmExpandPanelHeight form = new frmExpandPanelHeight();
			form.EditingHeight = this.ExpandedContentHeight;
			form.ShowDialog();
			if (form.DialogResult == DialogResult.OK) 
			{
				this.ExpandedContentHeight = form.EditingHeight;
			}
			form.Dispose();
		}
	}

	internal class ExpandPanelAutoPictureUtils 
	{
		private static readonly ImagesList upImagesList = 
			LoadImagesList(new String[] {"ExpandImages.Down.Normal.gif", 
											"ExpandImages.Up.Normal.gif",
											"ExpandImages.Down.Disabled.gif", 
											"ExpandImages.Up.Disabled.gif",
											"ExpandImages.Up.Animation.gif", 
											"ExpandImages.Down.Animation.gif"});
		private static readonly ImagesList leftImagesList = 
			LoadImagesList(new String[] {"ExpandImages.Right.Normal.gif", 
											"ExpandImages.Left.Normal.gif",
											"ExpandImages.Right.Disabled.gif", 
											"ExpandImages.Left.Disabled.gif",
											"ExpandImages.Left.Animation.gif", 
											"ExpandImages.Right.Animation.gif"});
		private static readonly ImagesList rightImagesList =
			LoadImagesList(new String[] {"ExpandImages.Left.Normal.gif", 
											"ExpandImages.Right.Normal.gif",
											"ExpandImages.Left.Disabled.gif", 
											"ExpandImages.Right.Disabled.gif",
											"ExpandImages.Right.Animation.gif", 
											"ExpandImages.Left.Animation.gif"});
		private static readonly ImagesList downImagesList = 
			LoadImagesList(new String[] {"ExpandImages.Up.Normal.gif", 
											"ExpandImages.Down.Normal.gif",
											"ExpandImages.Up.Disabled.gif", 
											"ExpandImages.Down.Disabled.gif",
											"ExpandImages.Down.Animation.gif", 
											"ExpandImages.Up.Animation.gif"});


		internal static ImagesList UpImagesList 
		{
			get 
			{
				return upImagesList;
			}
		}
		internal static ImagesList RightImagesList 
		{
			get 
			{
				return rightImagesList;
			}
		}
		internal static ImagesList LeftImagesList 
		{
			get 
			{
				return leftImagesList;
			}
		}
		internal static ImagesList DownImagesList 
		{
			get 
			{
				return downImagesList;
			}
		}

		private static ImagesList LoadImagesList(IEnumerable names) 
		{
			ArrayList images = new ArrayList();
			foreach (String s in names) 
			{
				images.Add(IconUtils.LoadImageFromAssemblyType(typeof(ExpandPanelAutoPictureUtils), s));
			}
			return new ImagesList(new ImageCollection(ArrayList.ReadOnly(images)));
		}
	}
	/// <summary>
	/// Specifies how images for the control are switched according to its 
	/// expansion/contraction state.
	/// </summary>
	public enum HeaderImageSwitchingMode
	{
		/// <summary>
		/// Specifies that the image index of the list being used should be
		/// actively changed by this control.  The indexes of images used are
		/// 0 - Expanded
		/// 1 - Contracted (defaults to 0)
		/// 2 - Expanded Disabled (defaults to 0)
		/// 3 - Contracted Disabled (defaults to 1)
		/// 4 - Expanding (defaults to 0)
		/// 5 - Contracting (defaults to 1)
		/// 6 - Expanding Disabled (defaults to 2)
		/// 7 - Contracting Disable (defaults to 3)
		/// The list is changed based on the expand direction of the control.
		/// </summary>
		AutoList,
		/// <summary>
		/// Like AutoList, but uses a custom image list.  The list is not changed 
		/// based on the expand direction of the control.
		/// </summary>
		CustomList,
		/// <summary>
		/// Specifies that the image is set by the caller and the index is not
		/// automatically changed.
		/// </summary>
		Custom
	}

	/// <summary>
	/// Specifies how text for the control is switched according to its 
	/// expansion/contraction state.
	/// </summary>
	public enum HeaderTextSwitchingMode
	{
		/// <summary>
		/// Specifies that the text of the header is set by appending (Hide) and (Show)
		/// to the text label.
		/// </summary>
		AppendShowHide,
		/// <summary>
		/// Specifies that the ExpandText and ContractText settings are used as the text of the
		/// header.
		/// </summary>
		ExpandContractText,
		/// <summary>
		/// Specifies that control of the header text is handled by the client.
		/// </summary>
		Custom
	}

	/// <summary>
	/// Enumeration for specifying the expansion/contraction state of the control.
	/// </summary>
	public enum ExpandMode
	{
		Expanded,
		Expanding,
		Contracted,
		Contracting
	}

	/// <summary>
	/// Enumeration for specifying the location of the header of an ExpandPanel
	/// </summary>
	public enum HeaderLocation 
	{
		Top, 
		Bottom, 
		Left, 
		Right, 
	}

	/// <summary>
	/// Enumeration for specifying which direction the panel expands towards.
	/// </summary>
	public enum ExpandDirection 
	{
		Up, 
		Down, 
		Left, 
		Right
	}

	/// <summary>
	/// Prevents opening in designer ExpandPanel in designer, thereby messing up organization.
	/// </summary>
	class Blank
	{
	}


}
