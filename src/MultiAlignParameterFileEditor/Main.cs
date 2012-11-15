using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MultiAlignCore.IO.Parameters;

namespace MultiAlignParameterFileEditor
{
    public partial class Main : Form
    {
        /// <summary>
        /// Maps an editor to the tab page it lives on.
        /// </summary>
        private Dictionary<ParameterFileEditor, TabPage> m_editorTabPageMap;
        /// <summary>
        /// Open file dialog for opening an existing parameter file.
        /// </summary>
        private OpenFileDialog m_dialog;

        /// <summary>
        /// Main constructor.
        /// </summary>
        public Main()
        {
            InitializeComponent();

            m_dialog = new OpenFileDialog();
            m_editorTabPageMap = new Dictionary<ParameterFileEditor, TabPage>();
        }

        #region Event Handlers 
        /// <summary>
        /// Creates a new parameter file control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newButton_Click(object sender, EventArgs e)
        {
            AddPage(new ParameterFileEditor(), "new");
            UpdateStatus("Created new parameter file editor.");
        }
        /// <summary>
        /// Opens an old paramter file and makes a new editor control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openButton_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = m_dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    XMLParamterFileReader reader                    = new XMLParamterFileReader();
                    MultiAlignCore.Data.MultiAlignAnalysis analysis = new MultiAlignCore.Data.MultiAlignAnalysis();
                    reader.ReadParameterFile(m_dialog.FileName, ref analysis);

                    Options = analysis.Options;

                    string path = System.IO.Path.GetFileNameWithoutExtension(m_dialog.FileName);

                    AddPage(new ParameterFileEditor(analysis.Options, m_dialog.FileName), path);
                    UpdateStatus("Opened " + m_dialog.FileName);
                }
                else
                {
                    UpdateStatus("Opening Cancelled.");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Error during opening: " + ex.Message);
            }
        }
        #endregion

        public MultiAlignCore.Data.AnalysisOptions Options
        {
            get;
            set;
        }

        #region Editor Event Handlers
        /// <summary>
        /// Displays text to the status bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void editor_Saved(object sender, ParameterFileEventArgs e)
        {
            string name  = System.IO.Path.GetFileNameWithoutExtension(e.Path);
            TabPage page = m_editorTabPageMap[sender as ParameterFileEditor];
            page.Text    = name;

            UpdateStatus("Saved parameter file to " + e.Path);

            
        }
        void editor_Closed(object sender, ParameterFileEventArgs e)
        {
            ParameterFileEditor control = sender as ParameterFileEditor;
            control.Saved  -= editor_Saved;
            control.Closed -= editor_Closed;
            control.Dispose();

            TabPage page  = m_editorTabPageMap[control];
            int lastIndex = mainTabPage.TabPages.IndexOf(page);
            mainTabPage.TabPages.Remove(page);                        
            page.Dispose();

            // If after removing a tab page, we are at the end of all tab pages,
            // then that means we should move back one page.            
            if (lastIndex == mainTabPage.TabPages.Count)
            {
                lastIndex--;
            }

            // However, if the last index is now 0, then that means we are 
            // only at the start page, which is the only one left.
            // Here, otherwise if true, we are not at the start page. 
            // So select the tab page.
            if (lastIndex > 0)
            {
                mainTabPage.SelectedIndex = lastIndex;
            }

            UpdateStatus("Closed parameter file editor");
        }
        #endregion

        #region Updates And Other Related Utility
        private void AddPage(ParameterFileEditor editor, string name)
        {
            TabPage page     = new TabPage();
            editor.Saved    += new EventHandler<ParameterFileEventArgs>(editor_Saved);
            editor.Closed   += new EventHandler<ParameterFileEventArgs>(editor_Closed);
            editor.Dock      = DockStyle.Fill;
            page.Text        = name;
            page.Controls.Add(editor);

            mainTabPage.TabPages.Add(page);
            mainTabPage.SelectedTab = page;
            m_editorTabPageMap.Add(editor, page);
        }
        /// <summary>
        /// updates the status bar with text.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            statusLabel.Text = message;
        }
        #endregion
    }
}
