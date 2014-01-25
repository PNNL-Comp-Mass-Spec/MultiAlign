using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.Data;
using System.Drawing;

namespace MultiAlignParameterFileEditor
{
    public partial class ParameterFileEditor : UserControl
    {
        /// <summary>
        /// Default button size.
        /// </summary>
        private const int BUTTON_SIZE = 40;

        #region Events
        /// <summary>
        /// Fired when a file is saved.
        /// </summary>
        public event EventHandler<ParameterFileEventArgs> Saved;
        /// <summary>
        /// Fired when the closed button is pressed.
        /// </summary>
        public event EventHandler<ParameterFileEventArgs> Closed;
        /// <summary>
        /// Saving failed.
        /// </summary>
        public event EventHandler<ParameterFileEventArgs> SavedFailed;
        #endregion

        #region
        /// <summary>
        /// Path to the parameter file.
        /// </summary>
        private string m_path;
        /// <summary>
        /// Internal options being edited.
        /// </summary>
        private AnalysisOptions m_options;
        /// <summary>
        /// Maps the parameter file group attribute to the object that holds 
        /// the parameters to be edited.
        /// </summary>
        private Dictionary<ParameterFileGroupAttribute, object> m_parameterMap;
        /// <summary>
        /// Maps the button to a parameter file group attribute.
        /// </summary>
        private Dictionary<Button, ParameterFileGroupAttribute> m_buttonParameterMap;
        /// <summary>
        /// colors for buttons based on group id's
        /// </summary>
        private List<Color> m_colorEditor;
        #endregion

        /// <summary>
        /// Constructor when a new parameter file is created.
        /// </summary>
        public ParameterFileEditor()
        {
            InitializeComponent();

            m_options            = new AnalysisOptions();
            m_path               = "";
            m_parameterMap       = new Dictionary<ParameterFileGroupAttribute, object>();
            m_buttonParameterMap = new Dictionary<Button, ParameterFileGroupAttribute>();

            GroupColors          = new List<Color>();
            GroupColors.AddRange(new Color[] {  Color.White,
                                                Color.LightGray,
                                                Color.LightGray,
                                                Color.LightSalmon,
                                                Color.DodgerBlue,
                                                Color.DodgerBlue,
                                                Color.LightYellow,
                                                Color.LightGreen
                                            });
            LoadOptions();
        }
        /// <summary>
        /// Constructor when a parameter file is loaded.
        /// </summary>
        /// <param name="options"></param>
        public ParameterFileEditor(AnalysisOptions options, string path)
        {
            InitializeComponent();

            SetOptions(options, path);
        }
        public void SetOptions(AnalysisOptions options, string path)
        {
            m_options            = options;
            m_path               = path;
            m_parameterMap       = new Dictionary<ParameterFileGroupAttribute, object>();
            m_buttonParameterMap = new Dictionary<Button, ParameterFileGroupAttribute>();

            GroupColors = new List<Color>();
            GroupColors.AddRange(new Color[] {  Color.White,
                                                Color.LightGray,
                                                Color.LightGray,
                                                Color.LightSalmon,
                                                Color.DodgerBlue,
                                                Color.DodgerBlue,
                                                Color.LightYellow,
                                                Color.LightGreen
                                            });

            LoadOptions();
        }

        public void HideCloseButton()
        {
            closeButton.Visible = false;
        }
        public void ShowCloseButton()
        {
            closeButton.Visible = true;
        }
        public void HideSaveButton()
        {
            saveButton.Visible = false;
        }
        public void ShowSaveButton()
        {
            saveButton.Visible = true;
        }

        public bool SaveButtonVisible
        {
            get { return saveButton.Visible; }
            set { saveButton.Visible = value; }
        }
        public bool CloseButtonVisible
        {
            get { return closeButton.Visible; }
            set { closeButton.Visible = value; }
        }

        #region Properties
        /// <summary>
        /// Gets or sets the group colors.
        /// </summary>
        public List<Color> GroupColors
        {
            get;
            set;
        }
        #endregion

        #region Control Event Handlers
        /// <summary>
        /// Selects an option loading it in the property editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void newButton_MouseClick(object sender, MouseEventArgs e)
        {
            SelectOption(sender as Button);
        }
        /// <summary>
        /// closes the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, new ParameterFileEventArgs(m_path));
            }
        }
        /// <summary>
        /// Saves the parameter file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (m_path == "")
            {
                SaveAs("");   
            }
            else
            {
                Save(m_path);
            }
        }
        /// <summary>
        /// Saves the file to the path the user has to set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsButton_Click(object sender, EventArgs e)
        {
            SaveAs(m_path);
        }
        #endregion 

        #region Reflection and Decoration
        /// <summary>
        /// Updates the colors of the buttons based on their group id.
        /// </summary>
        private void ClearColors()
        {
            foreach(Button button in m_buttonParameterMap.Keys)
            {
                ParameterFileGroupAttribute attr    = m_buttonParameterMap[button];
                button.BackColor                    = Color.WhiteSmoke; 
            }
        }
        /// <summary>
        /// Processes a composite object looking for custom attributes.
        /// </summary>
        /// <param name="options"></param>
        private void ProcessGroups(object options)
        {
            System.Diagnostics.Debug.Assert(options != null);

            Type type                   = options.GetType();
            PropertyInfo [] properties  = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {                    
                if (property.CanRead)
                {                    
                    object[] customAttributes = property.GetCustomAttributes(typeof(ParameterFileGroupAttribute), true);

                    foreach (object attribute in customAttributes)
                    {
                        object potential = null;                       
                        potential        = property.GetValue(   options,
                                                                BindingFlags.GetProperty,
                                                                null,
                                                                null,
                                                                null);

                        
                        ParameterFileGroupAttribute attr = attribute as ParameterFileGroupAttribute;

                        Button newButton        = new Button();
                        newButton.Text          = attr.Name;
                        newButton.Dock          = DockStyle.Top;
                        newButton.MouseClick   += new MouseEventHandler(newButton_MouseClick);
                        newButton.Height        = BUTTON_SIZE;
                        newButton.BackColor     = Color.WhiteSmoke;

                        m_buttonParameterMap.Add(newButton, attr);
                        m_parameterMap.Add(attr, potential);

                        // Add the new button, but make sure it shows up below the other options.
                        parameterSelectPanel.Controls.Add(newButton);
                        newButton.BringToFront();                                             
                    }
                }
            }            
        }
        #endregion

        #region Loading, Saving, and Selecting
        /// <summary>
        /// Saves the file to the path provided.
        /// </summary>
        /// <param name="path"></param>
        private void Save(string path)
        {
            XMLParameterFileWriter writer   = new XMLParameterFileWriter();
            // Fake the analysis.
            MultiAlignAnalysis analysis     = new MultiAlignAnalysis();
            analysis.Options                = m_options;

            try
            {
                writer.WriteParameterFile(path, analysis);
                m_path = path;
                if (Saved != null)
                {
                    Saved(this, new ParameterFileEventArgs(path));
                }
            }
            catch (Exception ex)
            {
                if (SavedFailed != null)
                {
                    SavedFailed(this, new ParameterFileEventArgs(path, ex.Message));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void SaveAs(string path)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            // Determine if we have a preset from a previous save.
            if (path != "")
            {
                dialog.FileName = path;
            }
            
            // Then go and save it if the user says its ok.
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Save(dialog.FileName);
            }
        }
        /// <summary>
        /// Loads the option selected through a button.
        /// </summary>
        /// <param name="button"></param>
        private void SelectOption(Button button)
        {
            System.Diagnostics.Debug.Assert(button != null);

            ParameterFileGroupAttribute attribute   = m_buttonParameterMap[button];
            object data                             = m_parameterMap[attribute];
            optionDescription.Text                  = attribute.FullDecription;
            parameterEditor.SelectedObject          = data;

            ClearColors();
            button.BackColor = Color.White;
        }
        /// <summary>
        /// Loads the options to the user interface.
        /// </summary>
        private void LoadOptions()
        {
            // Free up the data we were working with.
            parameterSelectPanel.Controls.Clear();
            m_buttonParameterMap.Clear();
            m_parameterMap.Clear();

            // Reflect the properties.
            ProcessGroups(m_options);

            // Then Load into the parameter editor. HACKED!
            Button button = null;
            foreach (Button lessage in m_buttonParameterMap.Keys)
            {
                button = lessage;
                break;
            }
            SelectOption(button);
        }
        #endregion        

        #region HTML Exporting 
        private string BuildParameter(  string category,
                                        string parameter, 
                                        string value, 
                                        string description)
        {
            return string.Format("<tr><td>{3}</td><td>{0}</td><td>{1}</td><td>{2}</td></tr>", parameter, value, description, category);
        }
         
        private string BuildParameter(  string category,
                                        string parameter, 
                                        string value, 
                                        string description,
                                        string style)
        {
            return string.Format("<tr {4}><td>{3}</td><td>{0}</td><td>{1}</td><td>{2}</td></tr>", parameter, value, description, category, style);
        }
        
        private void exportHTML_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog   = new SaveFileDialog();
            dialog.DefaultExt       = ".html";
            dialog.Filter           = "HTML files (*.html)|*.html"; 

            // Determine if we have a preset from a previous save.
            if (m_path != "")
            {
                dialog.FileName = m_path;
            }

            // Then go and save it if the user says its ok.
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                List<string> html = new List<string>();
                html.Add("<html>");
                html.Add("<body>");
                
                foreach (ParameterFileGroupAttribute attr in m_parameterMap.Keys)
                {
                    html.Add("<table border=\"1\" cellpadding=\"2\" >");
                    html.Add(string.Format("<caption >{0}</caption>", attr.Name));
                    html.Add(BuildParameter("Category", "Parameter Name", "Value", "Description", "style=\"background-color:#FFCC66;\""));
                    object data = m_parameterMap[attr];
                    if (data == null) continue;

                    Type   type = data.GetType();

                    PropertyInfo[] properties = type.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.CanRead)
                        {
                            // Get the value
                            object[] attributes   = property.GetCustomAttributes(typeof(ParameterFileAttribute), false);
                            if (attributes.Length < 1) continue;

                            object   value        = null; 
                            string   name         = "";
                            // Get the object value and name
                            foreach (object attribute in attributes)
                            {
                                ParameterFileAttribute parameterAttribute = attribute as ParameterFileAttribute;

                                value = property.GetValue(data,
                                                                  BindingFlags.GetProperty,
                                                                  null,
                                                                  null,
                                                                  null);

                                name        = parameterAttribute.Name;
                            }
                            // Get the descriptions
                            attributes          = property.GetCustomAttributes(typeof(DescriptionAttribute), false);
                            string description  = "";
                            foreach (object attribute in attributes)
                            {
                                DescriptionAttribute descriptionAttribute = attribute as DescriptionAttribute;
                                description                               = descriptionAttribute.Description;
                            }

                            // Get the group name
                            attributes = property.GetCustomAttributes(typeof(CategoryAttribute), false);
                            string category = "";
                            foreach (object attribute in attributes)
                            {
                                CategoryAttribute groupAttribute = attribute as CategoryAttribute;
                                category = groupAttribute.Category;
                            }
                            
                            string htmlTag = BuildParameter(category, name, value.ToString(), description);
                            html.Add(htmlTag);
                        }
                    }
                    html.Add("</table>");
                    html.Add("<br>");
                }

                using (System.IO.TextWriter writer = System.IO.File.CreateText(dialog.FileName))
                {
                    foreach (string tag in html)
                    {
                        writer.WriteLine(tag);
                    }
                }
            }

        }
        #endregion
    }
}

