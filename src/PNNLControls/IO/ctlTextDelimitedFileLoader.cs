using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlTextDelimitedFile.
	/// </summary>
	public class ctlTextDelimitedFileLoader : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ListView listPreview;
		private System.Windows.Forms.GroupBox groupPreview;
		private System.Windows.Forms.GroupBox groupFormat;
		private System.Windows.Forms.NumericUpDown numLinesToSkip;
		private System.Windows.Forms.Label lblNumberOfImportRows;
		private System.Windows.Forms.RadioButton radioRow;
		private System.Windows.Forms.RadioButton radioColumn;
		private System.Windows.Forms.RadioButton radioComma;
		private System.Windows.Forms.TextBox textOther;
		private System.Windows.Forms.GroupBox groupDelimiters;
		private System.Windows.Forms.RadioButton radioTab;
		private System.Windows.Forms.RadioButton radioSemicolon;
		private System.Windows.Forms.RadioButton radioSpace;
		private System.Windows.Forms.RadioButton radioOther;

		private clsTextDelimitedFileReader m_fileReader;
		
		private char m_delimiter;
		private string m_path;
		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlTextDelimitedFileLoader()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			m_fileReader = new clsTextDelimitedFileReader();
		}

		public ctlTextDelimitedFileLoader(string path)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			m_fileReader = new clsTextDelimitedFileReader();
			LoadFile(path);
		}

		public ArrayList Headers
		{
			get
			{
				return m_fileReader.Headers;
			}
			set
			{
				m_fileReader.Headers = value;
			}
		}

		public Hashtable Data
		{
			get
			{
				return m_fileReader.Data;
			}
			set
			{
				m_fileReader.Data = value;
			}
		}

		private void ResizeColumns()
		{
			if (listPreview.Columns.Count <= 0)
				return;

			int width = listPreview.Width / listPreview.Columns.Count;
			foreach(ColumnHeader col in listPreview.Columns)
			{
				col.Width = width;
			}
			listPreview.Refresh();
		}

		private void LoadRows(string path)
		{
			listPreview.Clear();
			listPreview.Columns.Clear();
			m_fileReader.Read(path, false);
		
			int numCols = -1;
			foreach(string header in m_fileReader.Headers)
			{
				ArrayList data = m_fileReader.Data[header] as ArrayList;
				ListViewItem item = new ListViewItem();
				item.Text = header;
				foreach(string dataSubText in data)
				{
					item.SubItems.Add(dataSubText);
				}
				numCols = Math.Max(data.Count, numCols);
				listPreview.Items.Add(item);
			}

			/// 
			/// Listview control needs help
			///  Column widths are not updated when you add an item
			///  
			listPreview.Columns.Add("Header", -1,System.Windows.Forms.HorizontalAlignment.Left);
			for(int i = 0; i < numCols; i++)
			{
				listPreview.Columns.Add( "Value" + i.ToString(),-1, System.Windows.Forms.HorizontalAlignment.Left);
			}

			ResizeColumns();
		}

		private void LoadColumns(string path)
		{
			listPreview.Clear();
			listPreview.Columns.Clear();
			m_fileReader.Read(path, true);

			int maxCount = -1;
			foreach(string header in m_fileReader.Headers)
			{
				listPreview.Columns.Add(header,-1,System.Windows.Forms.HorizontalAlignment.Left);
				ArrayList data = m_fileReader.Data[header] as ArrayList;
				maxCount = Math.Max(maxCount, data.Count);
			}

			for(int i = 0; i < maxCount; i++)
			{
				ListViewItem item = new ListViewItem();
				string firstColHeader = m_fileReader.Headers[0] as string;
				ArrayList dataFirst = m_fileReader.Data[firstColHeader] as ArrayList;
				if (dataFirst.Count <= i)
				{
					item.Text = string.Empty;
				}
				else
				{
					item.Text = dataFirst[i] as string;
				}

				for(int j = 1; j < m_fileReader.Headers.Count; j++)
				{
					string header  = m_fileReader.Headers[j] as string; 
					ArrayList data = m_fileReader.Data[header] as ArrayList;
					if (data.Count <= i)
						item.SubItems.Add(string.Empty);
					else
						item.SubItems.Add(data[i] as string);
				}
				listPreview.Items.Add(item);
			}

			ResizeColumns();
		}

		public void LoadFile(string path)
		{
			m_path = path;				
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

			m_fileReader.LinesBeforeHeader = Convert.ToInt32(numLinesToSkip.Value);
			m_fileReader.Delimiter = m_delimiter;
			m_fileReader.Clear();

			if (radioRow.Checked == true)
			{
				LoadRows(path);
			}
			else
			{
				LoadColumns(path);
			}
		}

		/// <summary>
		/// Path to file to load.
		/// </summary>
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
			this.groupDelimiters = new System.Windows.Forms.GroupBox();
			this.radioOther = new System.Windows.Forms.RadioButton();
			this.textOther = new System.Windows.Forms.TextBox();
			this.radioSpace = new System.Windows.Forms.RadioButton();
			this.radioSemicolon = new System.Windows.Forms.RadioButton();
			this.radioTab = new System.Windows.Forms.RadioButton();
			this.radioComma = new System.Windows.Forms.RadioButton();
			this.groupPreview = new System.Windows.Forms.GroupBox();
			this.listPreview = new System.Windows.Forms.ListView();
			this.groupFormat = new System.Windows.Forms.GroupBox();
			this.lblNumberOfImportRows = new System.Windows.Forms.Label();
			this.radioRow = new System.Windows.Forms.RadioButton();
			this.radioColumn = new System.Windows.Forms.RadioButton();
			this.numLinesToSkip = new System.Windows.Forms.NumericUpDown();
			this.groupDelimiters.SuspendLayout();
			this.groupPreview.SuspendLayout();
			this.groupFormat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numLinesToSkip)).BeginInit();
			this.SuspendLayout();
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
			this.groupDelimiters.Location = new System.Drawing.Point(0, 88);
			this.groupDelimiters.Name = "groupDelimiters";
			this.groupDelimiters.Size = new System.Drawing.Size(776, 208);
			this.groupDelimiters.TabIndex = 1;
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
			this.radioOther.CheckedChanged += new System.EventHandler(this.radioOther_CheckedChanged);
			// 
			// textOther
			// 
			this.textOther.Location = new System.Drawing.Point(120, 120);
			this.textOther.MaxLength = 1;
			this.textOther.Name = "textOther";
			this.textOther.Size = new System.Drawing.Size(56, 20);
			this.textOther.TabIndex = 11;
			this.textOther.Text = " ";
			this.textOther.TextChanged += new System.EventHandler(this.textOther_TextChanged);
			// 
			// radioSpace
			// 
			this.radioSpace.Location = new System.Drawing.Point(16, 96);
			this.radioSpace.Name = "radioSpace";
			this.radioSpace.Size = new System.Drawing.Size(96, 16);
			this.radioSpace.TabIndex = 10;
			this.radioSpace.Text = "Space";
			this.radioSpace.CheckedChanged += new System.EventHandler(this.radioSpace_CheckedChanged);
			// 
			// radioSemicolon
			// 
			this.radioSemicolon.Location = new System.Drawing.Point(16, 72);
			this.radioSemicolon.Name = "radioSemicolon";
			this.radioSemicolon.Size = new System.Drawing.Size(96, 16);
			this.radioSemicolon.TabIndex = 9;
			this.radioSemicolon.Text = "Semicolon";
			this.radioSemicolon.CheckedChanged += new System.EventHandler(this.radioSemicolon_CheckedChanged);
			// 
			// radioTab
			// 
			this.radioTab.Location = new System.Drawing.Point(16, 48);
			this.radioTab.Name = "radioTab";
			this.radioTab.Size = new System.Drawing.Size(96, 16);
			this.radioTab.TabIndex = 8;
			this.radioTab.Text = "Tab";
			this.radioTab.CheckedChanged += new System.EventHandler(this.radioTab_CheckedChanged);
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
			this.radioComma.CheckedChanged += new System.EventHandler(this.radioComma_CheckedChanged);
			// 
			// groupPreview
			// 
			this.groupPreview.Controls.Add(this.listPreview);
			this.groupPreview.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupPreview.Location = new System.Drawing.Point(0, 296);
			this.groupPreview.Name = "groupPreview";
			this.groupPreview.Size = new System.Drawing.Size(776, 376);
			this.groupPreview.TabIndex = 5;
			this.groupPreview.TabStop = false;
			this.groupPreview.Text = "Preview";
			// 
			// listPreview
			// 
			this.listPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listPreview.Location = new System.Drawing.Point(3, 16);
			this.listPreview.Name = "listPreview";
			this.listPreview.Size = new System.Drawing.Size(770, 357);
			this.listPreview.TabIndex = 3;
			this.listPreview.View = System.Windows.Forms.View.Details;
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
			this.groupFormat.Size = new System.Drawing.Size(776, 88);
			this.groupFormat.TabIndex = 6;
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
			this.radioColumn.CheckedChanged += new System.EventHandler(this.radioColumn_CheckedChanged);
			// 
			// numLinesToSkip
			// 
			this.numLinesToSkip.Location = new System.Drawing.Point(144, 24);
			this.numLinesToSkip.Name = "numLinesToSkip";
			this.numLinesToSkip.Size = new System.Drawing.Size(88, 20);
			this.numLinesToSkip.TabIndex = 5;
			this.numLinesToSkip.ValueChanged += new System.EventHandler(this.numLinesToSkip_ValueChanged);
			// 
			// ctlTextDelimitedFile
			// 
			this.Controls.Add(this.groupDelimiters);
			this.Controls.Add(this.groupPreview);
			this.Controls.Add(this.groupFormat);
			this.Name = "ctlTextDelimitedFile";
			this.Size = new System.Drawing.Size(776, 672);
			this.groupDelimiters.ResumeLayout(false);
			this.groupPreview.ResumeLayout(false);
			this.groupFormat.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numLinesToSkip)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void radioComma_CheckedChanged(object sender, System.EventArgs e)
		{
			LoadFile(m_path);
		}

		private void radioTab_CheckedChanged(object sender, System.EventArgs e)
		{
			LoadFile(m_path);
		}

		private void radioSemicolon_CheckedChanged(object sender, System.EventArgs e)
		{
			LoadFile(m_path);
		}

		private void radioSpace_CheckedChanged(object sender, System.EventArgs e)
		{
			LoadFile(m_path);
		}

		private void radioOther_CheckedChanged(object sender, System.EventArgs e)
		{
			LoadFile(m_path);
		}

		private void numLinesToSkip_ValueChanged(object sender, System.EventArgs e)
		{
			LoadFile(m_path);
		}

		private void radioColumn_CheckedChanged(object sender, System.EventArgs e)
		{
			LoadFile(m_path);
		}

		private void textOther_TextChanged(object sender, System.EventArgs e)
		{
			LoadFile(m_path);
		}
	}
}
