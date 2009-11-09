using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Supports viewing and editing of ColorInterpolators.
	/// </summary>
	public class pnlColorInterpolationEditor : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox mMinBoundTextBox;
		private System.Windows.Forms.Label mMinBoundLabel;
		private System.Windows.Forms.Label mMaxBoundLabel;
		private System.Windows.Forms.TextBox mMaxBoundTextBox;
		private System.Windows.Forms.GroupBox mTypeGroupBox;
		private System.Windows.Forms.GroupBox mBoundsGroupBox;
		private System.Windows.Forms.Button mEditButton;
		private System.Windows.Forms.Panel mColorInterpolationPanel;
		private ColorInterpolater mInterpolater;
		private System.Windows.Forms.TextBox mGradationsTextBox;
		private System.Windows.Forms.Label mGradationsLabel;
		private System.Windows.Forms.GroupBox mColorsGroupBox;
		private PNNLControls.pnlColorInterpolationViewer mColorInterpolationViewer;

		[System.ComponentModel.Browsable(true)]
		public event System.EventHandler ColorInterpolaterChanged;

		private bool mRespondToEvents = true;

		private System.Windows.Forms.ComboBox mTypeComboBox;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public pnlColorInterpolationEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Fill the type combo box with classes that know how to create 
			// color interpolators.
			this.mTypeComboBox.Items.Add(new LogColorInterpolaterCreater());
			this.mTypeComboBox.Items.Add(new LinearColorInterpolaterCreater());
			this.mTypeComboBox.Items.Add(new SolidColorInterpolaterCreater());
		}

		/// <summary>
		/// Raises the ColorInterpolatorChanged event.
		/// </summary>
		private void RaiseColorInterpolaterChanged() 
		{
			if (this.ColorInterpolaterChanged != null) 
			{
				this.ColorInterpolaterChanged(this, new System.EventArgs());
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
			this.mColorInterpolationViewer = new PNNLControls.pnlColorInterpolationViewer();
			this.mMinBoundTextBox = new System.Windows.Forms.TextBox();
			this.mMinBoundLabel = new System.Windows.Forms.Label();
			this.mMaxBoundLabel = new System.Windows.Forms.Label();
			this.mMaxBoundTextBox = new System.Windows.Forms.TextBox();
			this.mTypeGroupBox = new System.Windows.Forms.GroupBox();
			this.mTypeComboBox = new System.Windows.Forms.ComboBox();
			this.mBoundsGroupBox = new System.Windows.Forms.GroupBox();
			this.mEditButton = new System.Windows.Forms.Button();
			this.mColorInterpolationPanel = new System.Windows.Forms.Panel();
			this.mGradationsTextBox = new System.Windows.Forms.TextBox();
			this.mGradationsLabel = new System.Windows.Forms.Label();
			this.mColorsGroupBox = new System.Windows.Forms.GroupBox();
			this.mTypeGroupBox.SuspendLayout();
			this.mBoundsGroupBox.SuspendLayout();
			this.mColorInterpolationPanel.SuspendLayout();
			this.mColorsGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// mColorInterpolationViewer
			// 
			this.mColorInterpolationViewer.Colors = new System.Drawing.Color[] {
																				   System.Drawing.Color.Red,
																				   System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(192)), ((System.Byte)(192))),
																				   System.Drawing.Color.White,
																				   System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(255)), ((System.Byte)(255))),
																				   System.Drawing.Color.Blue};
			this.mColorInterpolationViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mColorInterpolationViewer.Location = new System.Drawing.Point(0, 0);
			this.mColorInterpolationViewer.Name = "mColorInterpolationViewer";
			this.mColorInterpolationViewer.Size = new System.Drawing.Size(326, 54);
			this.mColorInterpolationViewer.TabIndex = 0;
			this.mColorInterpolationViewer.Vertical = false;
			// 
			// mMinBoundTextBox
			// 
			this.mMinBoundTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mMinBoundTextBox.Location = new System.Drawing.Point(48, 24);
			this.mMinBoundTextBox.Name = "mMinBoundTextBox";
			this.mMinBoundTextBox.Size = new System.Drawing.Size(184, 22);
			this.mMinBoundTextBox.TabIndex = 3;
			this.mMinBoundTextBox.Text = ".01";
			// 
			// mMinBoundLabel
			// 
			this.mMinBoundLabel.Location = new System.Drawing.Point(8, 24);
			this.mMinBoundLabel.Name = "mMinBoundLabel";
			this.mMinBoundLabel.Size = new System.Drawing.Size(40, 16);
			this.mMinBoundLabel.TabIndex = 4;
			this.mMinBoundLabel.Text = "Min";
			this.mMinBoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mMaxBoundLabel
			// 
			this.mMaxBoundLabel.Location = new System.Drawing.Point(8, 56);
			this.mMaxBoundLabel.Name = "mMaxBoundLabel";
			this.mMaxBoundLabel.Size = new System.Drawing.Size(32, 16);
			this.mMaxBoundLabel.TabIndex = 5;
			this.mMaxBoundLabel.Text = "Max";
			this.mMaxBoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mMaxBoundTextBox
			// 
			this.mMaxBoundTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mMaxBoundTextBox.Location = new System.Drawing.Point(48, 56);
			this.mMaxBoundTextBox.Name = "mMaxBoundTextBox";
			this.mMaxBoundTextBox.Size = new System.Drawing.Size(184, 22);
			this.mMaxBoundTextBox.TabIndex = 6;
			this.mMaxBoundTextBox.Text = "1";
			// 
			// mTypeGroupBox
			// 
			this.mTypeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mTypeGroupBox.Controls.Add(this.mTypeComboBox);
			this.mTypeGroupBox.Location = new System.Drawing.Point(0, 64);
			this.mTypeGroupBox.Name = "mTypeGroupBox";
			this.mTypeGroupBox.Size = new System.Drawing.Size(80, 80);
			this.mTypeGroupBox.TabIndex = 7;
			this.mTypeGroupBox.TabStop = false;
			this.mTypeGroupBox.Text = "Type";
			// 
			// mTypeComboBox
			// 
			this.mTypeComboBox.Location = new System.Drawing.Point(8, 16);
			this.mTypeComboBox.Name = "mTypeComboBox";
			this.mTypeComboBox.Size = new System.Drawing.Size(64, 24);
			this.mTypeComboBox.TabIndex = 0;
			this.mTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.mTypeComboBox_SelectedIndexChanged);
			// 
			// mBoundsGroupBox
			// 
			this.mBoundsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mBoundsGroupBox.Controls.Add(this.mMinBoundLabel);
			this.mBoundsGroupBox.Controls.Add(this.mMaxBoundLabel);
			this.mBoundsGroupBox.Controls.Add(this.mMinBoundTextBox);
			this.mBoundsGroupBox.Controls.Add(this.mMaxBoundTextBox);
			this.mBoundsGroupBox.Location = new System.Drawing.Point(88, 64);
			this.mBoundsGroupBox.Name = "mBoundsGroupBox";
			this.mBoundsGroupBox.Size = new System.Drawing.Size(240, 80);
			this.mBoundsGroupBox.TabIndex = 8;
			this.mBoundsGroupBox.TabStop = false;
			this.mBoundsGroupBox.Text = "Bounds";
			this.mBoundsGroupBox.Leave += new System.EventHandler(this.mBoundsGroupBox_Leave);
			// 
			// mEditButton
			// 
			this.mEditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mEditButton.Location = new System.Drawing.Point(8, 24);
			this.mEditButton.Name = "mEditButton";
			this.mEditButton.Size = new System.Drawing.Size(88, 32);
			this.mEditButton.TabIndex = 9;
			this.mEditButton.Text = "Edit Colors";
			this.mEditButton.Click += new System.EventHandler(this.mEditButton_Click);
			// 
			// mColorInterpolationPanel
			// 
			this.mColorInterpolationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mColorInterpolationPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mColorInterpolationPanel.Controls.Add(this.mColorInterpolationViewer);
			this.mColorInterpolationPanel.Location = new System.Drawing.Point(0, 0);
			this.mColorInterpolationPanel.Name = "mColorInterpolationPanel";
			this.mColorInterpolationPanel.Size = new System.Drawing.Size(328, 56);
			this.mColorInterpolationPanel.TabIndex = 10;
			// 
			// mGradationsTextBox
			// 
			this.mGradationsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mGradationsTextBox.Location = new System.Drawing.Point(8, 104);
			this.mGradationsTextBox.Name = "mGradationsTextBox";
			this.mGradationsTextBox.Size = new System.Drawing.Size(88, 22);
			this.mGradationsTextBox.TabIndex = 11;
			this.mGradationsTextBox.Text = "1";
			this.mGradationsTextBox.Leave += new System.EventHandler(this.mGradationsTextBox_Leave);
			// 
			// mGradationsLabel
			// 
			this.mGradationsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mGradationsLabel.Location = new System.Drawing.Point(8, 80);
			this.mGradationsLabel.Name = "mGradationsLabel";
			this.mGradationsLabel.Size = new System.Drawing.Size(88, 16);
			this.mGradationsLabel.TabIndex = 12;
			this.mGradationsLabel.Text = "Gradations";
			// 
			// mColorsGroupBox
			// 
			this.mColorsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mColorsGroupBox.Controls.Add(this.mEditButton);
			this.mColorsGroupBox.Controls.Add(this.mGradationsLabel);
			this.mColorsGroupBox.Controls.Add(this.mGradationsTextBox);
			this.mColorsGroupBox.Location = new System.Drawing.Point(336, 0);
			this.mColorsGroupBox.Name = "mColorsGroupBox";
			this.mColorsGroupBox.Size = new System.Drawing.Size(104, 144);
			this.mColorsGroupBox.TabIndex = 13;
			this.mColorsGroupBox.TabStop = false;
			this.mColorsGroupBox.Text = "Colors";
			// 
			// pnlColorInterpolationEditor
			// 
			this.Controls.Add(this.mColorsGroupBox);
			this.Controls.Add(this.mColorInterpolationPanel);
			this.Controls.Add(this.mBoundsGroupBox);
			this.Controls.Add(this.mTypeGroupBox);
			this.Name = "pnlColorInterpolationEditor";
			this.Size = new System.Drawing.Size(440, 144);
			this.mTypeGroupBox.ResumeLayout(false);
			this.mBoundsGroupBox.ResumeLayout(false);
			this.mColorInterpolationPanel.ResumeLayout(false);
			this.mColorsGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets the interpolator being edited.
		/// </summary>
		public PNNLControls.ColorInterpolater ColorInterpolater 
		{
			get 
			{
				return this.mInterpolater;
			}
			set 
			{
				if (value != null) 
				{
					this.mInterpolater = value;
					this.LoadFromInterpolater();
					this.RaiseColorInterpolaterChanged();
				}
			}
		}
		
		/// <summary>
		/// Loads the settings of the interpolator into the display.
		/// </summary>
		private void LoadFromInterpolater() 
		{
			this.mRespondToEvents = false;
			ColorInterpolater interp = this.mInterpolater;
			foreach (ColorInterpolaterCreater creater in this.mTypeComboBox.Items) 
			{
				if (creater.Matches(this.mInterpolater)) 
				{
					this.mTypeComboBox.SelectedItem = creater;
					break;
				}
			}

			if (interp is BoundedColorInterpolater) 
			{
				this.mBoundsGroupBox.Enabled = true;
				this.mMaxBoundTextBox.Text = ((BoundedColorInterpolater) interp).HighValue.ToString();
				this.mMinBoundTextBox.Text = ((BoundedColorInterpolater) interp).LowValue.ToString();
			}
			else 
			{
				this.mBoundsGroupBox.Enabled = false;
			}
			this.mGradationsTextBox.Text = interp.Gradations.ToString();
			this.mColorInterpolationViewer.Colors = interp.InterpolatedColors;
			this.mRespondToEvents = true;
		}

		private void mEditButton_Click(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				try 
				{
					System.Drawing.Design.UITypeEditor editor = 
						new System.ComponentModel.Design.ArrayEditor(typeof(Color[]));
					DialogContextAndProvider dcp = new DialogContextAndProvider();
					// Have to provide both a context and a service provider (dcp implements 
					// both these interfaces).  If this isn't done, Microsoft's code throws 
					// an error.
					Object editedValues = editor.EditValue(dcp, dcp, this.mInterpolater.Colors);
					this.mInterpolater.Colors = (Color[]) editedValues;
					this.mColorInterpolationViewer.Colors = this.mInterpolater.InterpolatedColors;
					this.RaiseColorInterpolaterChanged();
				} 
				catch (Exception ex) 
				{
					MessageBox.Show(ex.Message, "Error");
				}
				this.LoadFromInterpolater();
			}
		}

		private void mGradationsTextBox_Leave(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				try 
				{
					int gradations = int.Parse(this.mGradationsTextBox.Text);
					this.mInterpolater.Gradations = gradations;
					this.RaiseColorInterpolaterChanged();
				} 
				catch (Exception ex) 
				{
					System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
				}
				this.LoadFromInterpolater();
			}
		}

		private void mTypeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				try 
				{
					this.mInterpolater = ((ColorInterpolaterCreater) this.mTypeComboBox.SelectedItem).CreateInterpolater(this.mInterpolater);
					this.RaiseColorInterpolaterChanged();
				} 
				catch (Exception ex) 
				{
					System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
				}
				this.LoadFromInterpolater();
			}
		}

		private void mBoundsGroupBox_Leave(object sender, System.EventArgs e)
		{
			if (this.mRespondToEvents) 
			{
				try 
				{
					float max = float.Parse(this.mMaxBoundTextBox.Text);
					float min = float.Parse(this.mMinBoundTextBox.Text);
					((BoundedColorInterpolater) this.mInterpolater).SetBounds(min, max);
					this.RaiseColorInterpolaterChanged();
				} 
				catch (Exception ex) 
				{
					System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
				}
				this.LoadFromInterpolater();
			}
		}
	}

	/// <summary>
	/// Provides the needed ITypeDescriptorContext and IServiceProvider members 
	/// needed to show the ArrayEditor dialog.
	/// </summary>
	public class DialogContextAndProvider : System.ComponentModel.ITypeDescriptorContext
	{
		public DialogContextAndProvider() 
		{
		}
		#region ITypeDescriptorContext Members

		public void OnComponentChanged()
		{
			return;
		}

		public IContainer Container
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// This method must return true to indicate that changes made 
		/// to the object being edited are accepted.
		/// </summary>
		/// <returns></returns>
		public bool OnComponentChanging()
		{
			return true;
		}

		public object Instance
		{
			get
			{
				return null;
			}
		}

		public PropertyDescriptor PropertyDescriptor
		{
			get
			{
				//Console.WriteLine("Getting property descriptor");
				return null;
			}
		}

		#endregion

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			//Console.WriteLine("Type requested: {0}", serviceType);
			if (serviceType == typeof(System.Windows.Forms.Design.IWindowsFormsEditorService)) 
			{
				//Console.WriteLine("Returning IWFES");
				return new DialogWindowsFromsEditorService();
			}
			return null;
		}

		#endregion
	}

	
	/// <summary>
	/// Provides support for showing an ArrayEditor dialog.  Because this is all 
	/// we need to do, the other methods (which are much harder to do - browse the Internet)
	/// are left unimplemented.
	/// </summary>
	public class DialogWindowsFromsEditorService : System.Windows.Forms.Design.IWindowsFormsEditorService 
	{
		public void DropDownControl(Control control) 
		{
			throw new NotImplementedException("Can not handle drop-down controls");
		}

		public void CloseDropDown() 
		{
			throw new NotImplementedException("Can not handle drop-down controls");
		}

		public DialogResult ShowDialog(Form dialog) 
		{
			// See how easy this is compared to the other methods
			return dialog.ShowDialog();
		}
	}

	#region "Color Interpolater Creaters"
	/// <summary>
	/// Abstract base class.  Subclasses are put into the type drop-down.
	/// </summary>
	internal abstract class ColorInterpolaterCreater 
	{
		private String mName;
		public ColorInterpolaterCreater(String name) 
		{
			this.mName = name;
		}
		public override string ToString()
		{
			return mName;
		}
		public abstract bool Matches(ColorInterpolater interp);
		public abstract ColorInterpolater CreateInterpolater(ColorInterpolater baseInterp);
	}

	internal class LogColorInterpolaterCreater : ColorInterpolaterCreater 
	{
		public LogColorInterpolaterCreater() : base ("Log") {}
		public override ColorInterpolater CreateInterpolater(ColorInterpolater baseInterp)
		{
			LogColorInterpolater interp = new LogColorInterpolater();
			interp.Colors = baseInterp.Colors;
			interp.Gradations = baseInterp.Gradations;
			if (baseInterp is BoundedColorInterpolater) 
			{
				interp.SetBounds(((BoundedColorInterpolater) baseInterp).LowValue, 
					((BoundedColorInterpolater) baseInterp).HighValue);
			}
			return interp;
		}
		public override bool Matches(ColorInterpolater interp)
		{
			return interp is LogColorInterpolater;
		}
	}

	internal class LinearColorInterpolaterCreater : ColorInterpolaterCreater 
	{
		public LinearColorInterpolaterCreater() : base ("Linear") {}
		public override ColorInterpolater CreateInterpolater(ColorInterpolater baseInterp)
		{
			LinearColorInterpolater interp = new LinearColorInterpolater();
			interp.Colors = baseInterp.Colors;
			interp.Gradations = baseInterp.Gradations;
			if (baseInterp is BoundedColorInterpolater) 
			{
				interp.SetBounds(((BoundedColorInterpolater) baseInterp).LowValue, 
					((BoundedColorInterpolater) baseInterp).HighValue);
			}
			return interp;
		}
		public override bool Matches(ColorInterpolater interp)
		{
			return interp is LinearColorInterpolater;
		}
	}

	internal class SolidColorInterpolaterCreater : ColorInterpolaterCreater 
	{
		public SolidColorInterpolaterCreater() : base ("Solid") {}
		public override ColorInterpolater CreateInterpolater(ColorInterpolater baseInterp)
		{
			SolidColorInterpolater interp = new SolidColorInterpolater(baseInterp.Colors);
			return interp;
		}
		public override bool Matches(ColorInterpolater interp)
		{
			return interp is SolidColorInterpolater;
		}
	}
	#endregion
}
