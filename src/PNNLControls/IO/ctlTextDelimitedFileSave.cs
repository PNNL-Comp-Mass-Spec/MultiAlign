using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	
	public enum TextDataFileFormat
	{
		ROW,
		COLUMN
	}

	/// <summary>
	/// Summary description for ctlTextDelimitedFileSave.
	/// </summary>
	public class ctlTextDelimitedFileSave : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.GroupBox groupFormat;
		private System.Windows.Forms.Label lblNumberOfImportRows;
		private System.Windows.Forms.RadioButton radioRow;
		private System.Windows.Forms.RadioButton radioColumn;
		private System.Windows.Forms.NumericUpDown numLinesToSkip;
		private System.Windows.Forms.GroupBox groupDelimiters;
		private System.Windows.Forms.RadioButton radioOther;
		private System.Windows.Forms.TextBox textOther;
		private System.Windows.Forms.RadioButton radioSpace;
		private System.Windows.Forms.RadioButton radioSemicolon;
		private System.Windows.Forms.RadioButton radioTab;
		private System.Windows.Forms.RadioButton radioComma;
		
		private clsTextDelimitedFileWriter m_fileWriter;
		private TextDataFileFormat m_format;
		
		private char m_delimiter;
		private string m_path;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlTextDelimitedFileSave()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			m_fileWriter = new clsTextDelimitedFileWriter();
		}

		public ctlTextDelimitedFileSave(string path)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			m_fileWriter = new clsTextDelimitedFileWriter();
			m_path = path;
		}

		public char Delimiter
		{
			get
			{
				/// 
				/// Find the delimiter to use
				/// 
				if (radioComma.Checked == true)
				{
					m_delimiter = ',';
				}
				else if (radioSemicolon.Checked == true)
				{
					m_delimiter = ';';
				}
				else if (radioOther.Checked == true)
				{
					char [] arr = textOther.Text.ToCharArray();
					if (arr.Length <= 0)
					{
						arr		= new char[1];
						arr[0]	= ' ';
					}
					m_delimiter = arr[0];
				}
				else if (radioSpace.Checked == true)
				{
					m_delimiter = ' ';
				}
				else if (radioTab.Checked == true)
				{
					m_delimiter = '\t';
				}
				else
				{
					m_delimiter = ',';
				}

				return m_delimiter;
			}
			set
			{
				m_delimiter = value;
			}
		}
	
		public void WriteFile()
		{
			m_delimiter = Delimiter;
			m_fileWriter.Delimiter = m_delimiter;
			m_fileWriter.Write(m_path, radioColumn.Checked);
		}

		public void WriteFile(string path, bool columns)
		{
			m_delimiter = Delimiter;
			m_fileWriter.Delimiter = m_delimiter;
			m_fileWriter.Write(path, columns);
		}

		public void WriteFile(string path)
		{
			m_delimiter = Delimiter;
			m_fileWriter.Delimiter = m_delimiter;
			m_fileWriter.Write(path, radioColumn.Checked);
		}
		
		public int LinesToSkip
		{
			get
			{
				return Convert.ToInt32(numLinesToSkip.Value);
			}
			set
			{
				numLinesToSkip.Value = value;
			}
		}
		public string Path
		{
			get
			{
				return m_path;
			}
			set
			{
				m_path = value;
			}
		}

		public ArrayList Headers
		{
			get
			{
				return this.m_fileWriter.Headers;
			}
			set
			{
				m_fileWriter.Headers = value;
			}
		}

		public TextDataFileFormat Format
		{
			get
			{
				if (radioColumn.Checked == true)
				{
					m_format = TextDataFileFormat.COLUMN;
				}
				else
				{
					m_format = TextDataFileFormat.ROW;
				}

				return m_format;
			}
			set
			{
				m_format = value;
				if (m_format == TextDataFileFormat.COLUMN)
				{
					radioColumn.Checked = true;
				}
				else
				{
					radioRow.Checked = true;
				}
			}
		}

		public Hashtable Data
		{
			get
			{
				return m_fileWriter.Data;
			}
			set
			{
				m_fileWriter.Data = value;
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
			this.groupFormat = new System.Windows.Forms.GroupBox();
			this.lblNumberOfImportRows = new System.Windows.Forms.Label();
			this.radioRow = new System.Windows.Forms.RadioButton();
			this.radioColumn = new System.Windows.Forms.RadioButton();
			this.numLinesToSkip = new System.Windows.Forms.NumericUpDown();
			this.groupDelimiters = new System.Windows.Forms.GroupBox();
			this.radioOther = new System.Windows.Forms.RadioButton();
			this.textOther = new System.Windows.Forms.TextBox();
			this.radioSpace = new System.Windows.Forms.RadioButton();
			this.radioSemicolon = new System.Windows.Forms.RadioButton();
			this.radioTab = new System.Windows.Forms.RadioButton();
			this.radioComma = new System.Windows.Forms.RadioButton();
			this.groupFormat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numLinesToSkip)).BeginInit();
			this.groupDelimiters.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupFormat
			// 
			this.groupFormat.Controls.Add(this.lblNumberOfImportRows);
			this.groupFormat.Controls.Add(this.radioRow);
			this.groupFormat.Controls.Add(this.radioColumn);
			this.groupFormat.Controls.Add(this.numLinesToSkip);
			this.groupFormat.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupFormat.Location = new System.Drawing.Point(0, 0);
			this.groupFormat.Name = "groupFormat";
			this.groupFormat.Size = new System.Drawing.Size(256, 128);
			this.groupFormat.TabIndex = 9;
			this.groupFormat.TabStop = false;
			this.groupFormat.Text = "Format";
			// 
			// lblNumberOfImportRows
			// 
			this.lblNumberOfImportRows.Location = new System.Drawing.Point(16, 24);
			this.lblNumberOfImportRows.Name = "lblNumberOfImportRows";
			this.lblNumberOfImportRows.Size = new System.Drawing.Size(128, 24);
			this.lblNumberOfImportRows.TabIndex = 8;
			this.lblNumberOfImportRows.Text = "Lines to Skip in Header";
			// 
			// radioRow
			// 
			this.radioRow.Checked = true;
			this.radioRow.Location = new System.Drawing.Point(136, 56);
			this.radioRow.Name = "radioRow";
			this.radioRow.Size = new System.Drawing.Size(96, 16);
			this.radioRow.TabIndex = 7;
			this.radioRow.TabStop = true;
			this.radioRow.Text = "Row Format";
			// 
			// radioColumn
			// 
			this.radioColumn.Location = new System.Drawing.Point(16, 56);
			this.radioColumn.Name = "radioColumn";
			this.radioColumn.Size = new System.Drawing.Size(112, 16);
			this.radioColumn.TabIndex = 6;
			this.radioColumn.Text = "Column Format";
			// 
			// numLinesToSkip
			// 
			this.numLinesToSkip.Location = new System.Drawing.Point(144, 24);
			this.numLinesToSkip.Name = "numLinesToSkip";
			this.numLinesToSkip.Size = new System.Drawing.Size(88, 20);
			this.numLinesToSkip.TabIndex = 5;
			// 
			// groupDelimiters
			// 
			this.groupDelimiters.Controls.Add(this.radioOther);
			this.groupDelimiters.Controls.Add(this.textOther);
			this.groupDelimiters.Controls.Add(this.radioSpace);
			this.groupDelimiters.Controls.Add(this.radioSemicolon);
			this.groupDelimiters.Controls.Add(this.radioTab);
			this.groupDelimiters.Controls.Add(this.radioComma);
			this.groupDelimiters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupDelimiters.Location = new System.Drawing.Point(0, 128);
			this.groupDelimiters.Name = "groupDelimiters";
			this.groupDelimiters.Size = new System.Drawing.Size(256, 152);
			this.groupDelimiters.TabIndex = 7;
			this.groupDelimiters.TabStop = false;
			this.groupDelimiters.Text = "Delimiters";
			// 
			// radioOther
			// 
			this.radioOther.Location = new System.Drawing.Point(16, 120);
			this.radioOther.Name = "radioOther";
			this.radioOther.Size = new System.Drawing.Size(96, 16);
			this.radioOther.TabIndex = 12;
			this.radioOther.Text = "Other";
			// 
			// textOther
			// 
			this.textOther.Location = new System.Drawing.Point(120, 120);
			this.textOther.MaxLength = 1;
			this.textOther.Name = "textOther";
			this.textOther.Size = new System.Drawing.Size(56, 20);
			this.textOther.TabIndex = 11;
			this.textOther.Text = "";
			// 
			// radioSpace
			// 
			this.radioSpace.Location = new System.Drawing.Point(16, 96);
			this.radioSpace.Name = "radioSpace";
			this.radioSpace.Size = new System.Drawing.Size(96, 16);
			this.radioSpace.TabIndex = 10;
			this.radioSpace.Text = "Space";
			// 
			// radioSemicolon
			// 
			this.radioSemicolon.Location = new System.Drawing.Point(16, 72);
			this.radioSemicolon.Name = "radioSemicolon";
			this.radioSemicolon.Size = new System.Drawing.Size(96, 16);
			this.radioSemicolon.TabIndex = 9;
			this.radioSemicolon.Text = "Semicolon";
			// 
			// radioTab
			// 
			this.radioTab.Location = new System.Drawing.Point(16, 48);
			this.radioTab.Name = "radioTab";
			this.radioTab.Size = new System.Drawing.Size(96, 16);
			this.radioTab.TabIndex = 8;
			this.radioTab.Text = "Tab";
			// 
			// radioComma
			// 
			this.radioComma.Checked = true;
			this.radioComma.Location = new System.Drawing.Point(16, 24);
			this.radioComma.Name = "radioComma";
			this.radioComma.Size = new System.Drawing.Size(96, 16);
			this.radioComma.TabIndex = 7;
			this.radioComma.TabStop = true;
			this.radioComma.Text = "Comma";
			// 
			// ctlTextDelimitedFileSave
			// 
			this.Controls.Add(this.groupDelimiters);
			this.Controls.Add(this.groupFormat);
			this.Name = "ctlTextDelimitedFileSave";
			this.Size = new System.Drawing.Size(256, 280);
			this.groupFormat.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numLinesToSkip)).EndInit();
			this.groupDelimiters.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
