using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MultiAlignEngine.Features;
using PNNLProteomics.Filters;

namespace MultiAlignWin.Forms.Filters
{
    public partial class DataFilters : Form
    {

        /// <summary>
        /// List of controls based on their filter name.
        /// </summary>
        Dictionary<string, Type> m_availableFilterControls;
        /// <summary>
        /// List of UMC Filters.
        /// </summary>
        Dictionary<IFilter<clsUMC>, UserControl> m_umcFilterControls;
        /// <summary>
        /// List of Cluster Filters.
        /// </summary>
        Dictionary<IFilter<clsCluster>, UserControl> m_clusterFilterControls;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataFilters()
        {
            InitializeComponent();
            Init();
        }        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clusterFilters">Filters for cluster data.</param>
        /// <param name="umcFilters">Filters for umc filters.</param>
        public DataFilters(List<IFilter<clsCluster>> clusterFilters, List<IFilter<clsUMC>> umcFilters)
        {
            InitializeComponent();
            Init();

            /// 
            /// Adds the cluster filters to the list box.
            /// 
            foreach (IFilter<clsCluster> filter in clusterFilters)
            {
                AddFilter(filter.ToString(), filter);                
            }
        }
        /// <summary>
        /// Initializes the control.
        /// </summary>
        private void Init()
        {
            /// 
            /// Create a list of filter to control lists.
            /// 
            m_availableFilterControls = new Dictionary<string, Type>();

            DatasetMembersFilter filter = new DatasetMembersFilter();
            m_availableFilterControls.Add(filter.ToString(), typeof(MembersInClusterControl));

            m_clusterFilterControls = new Dictionary<IFilter<clsCluster>, UserControl>();
            m_umcFilterControls = new Dictionary<IFilter<clsUMC>, UserControl>();

            /// 
            /// Add the filter names to the list box
            /// 
            foreach (string key in m_availableFilterControls.Keys)            
                filterComboBox.Items.Add(key);

            if (filterComboBox.Items.Count > 0)
                filterComboBox.SelectedIndex = 0;

            FormClosing += new FormClosingEventHandler(DataFilters_FormClosing);
        }

        /// <summary>
        /// Gets a list of the UMC Filters.
        /// </summary>
        /// <returns></returns>
        public List<IFilter<clsUMC>> GetUMCFilters()
        {
            List<IFilter<clsUMC>> filters = new List<IFilter<clsUMC>>();
            foreach (object o in filterListBox.Items)
            {
                IFilter<clsUMC> filter = o as IFilter<clsUMC>;
                if (filter != null)
                {
                    IFilterControl<clsUMC> cluster = m_umcFilterControls[filter] as IFilterControl<clsUMC>;
                    if (cluster != null)
                    {
                        filters.Add(cluster.Filter);
                    }
                }
            }

            return filters;
        }
        /// <summary>
        /// Gets a list of cluster filters.
        /// </summary>
        /// <returns></returns>
        public List<IFilter<clsCluster>> GetClusterFilters()
        {
            List<IFilter<clsCluster>> filters = new List<IFilter<clsCluster>>();
            foreach (object o in filterListBox.Items)
            {
                IFilter<clsCluster> filter = o as IFilter<clsCluster>;
                if (filter != null)
                {
                    IFilterControl<clsCluster> cluster = m_clusterFilterControls[filter] as IFilterControl<clsCluster>;
                    if (cluster != null)
                    {
                        filters.Add(cluster.Filter);
                    }
                }
            }

            return filters;
        }

        #region Form Event Handlers 
        void DataFilters_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = false;
                Hide();
            }
        }
        /// <summary>
        /// Removes selected filters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeFilterButton_Click(object sender, EventArgs e)
        {
            if (filterListBox.SelectedItems != null && filterListBox.SelectedItems.Count > 0)
            {
                object[] items = new object[filterListBox.SelectedItems.Count];
                filterListBox.SelectedItems.CopyTo(items, 0);
                foreach (object o in items)
                {
                    IFilter<clsCluster> clusterFilter = o as IFilter<clsCluster>;
                    UserControl control = null;

                    if (clusterFilter != null)
                    {
                        if (m_clusterFilterControls.ContainsKey(clusterFilter) == true)
                        {
                            control = m_clusterFilterControls[clusterFilter];
                            m_clusterFilterControls.Remove(clusterFilter);
                        }
                    }
                    IFilter<clsUMC> umcFilter = o as IFilter<clsUMC>;
                    if (umcFilter != null)
                    {
                        if (m_umcFilterControls.ContainsKey(umcFilter) == true)
                        {                            
                            control = m_umcFilterControls[umcFilter];
                            m_umcFilterControls.Remove(umcFilter);
                        }
                    }
                    filterListBox.Items.Remove(o);
                    if (control != null && filterContainer.Panel2.Controls.Contains(control))
                    {
                        filterContainer.Panel2.Controls.Remove(control);
                    }                            
                }
            }
        }
        /// <summary>
        /// Adds a new filter to the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFilterButton_Click(object sender, EventArgs e)
        {
            if (filterComboBox.SelectedItem != null)
            {
                AddFilter(filterComboBox.SelectedItem.ToString(), null);
            }
        }
        /// <summary>
        /// Adds a filter to the list of selected filters.
        /// </summary>
        /// <param name="filter"></param>
        private void AddFilter(string name, object filter)
        {
                if (m_availableFilterControls.ContainsKey(name) == false)
                {
                    return;
                }

                Type type = m_availableFilterControls[name];
                if (type == null)
                    return ;

                /// 
                /// Create the control
                /// 

                UserControl control = Activator.CreateInstance(type, false) as UserControl;
                IFilterControl<clsUMC> umcControl = control as IFilterControl<clsUMC>;
                if (umcControl != null)
                {
                    if (filter != null)
                        umcControl.Filter = filter as IFilter<clsUMC>;

                    m_umcFilterControls.Add(umcControl.Filter, umcControl as UserControl);
                    filterListBox.Items.Add(umcControl.Filter);
                }
                else
                {
                    IFilterControl<clsCluster> clusterControl = control as IFilterControl<clsCluster>;
                    if (clusterControl != null)
                    {
                        if (filter != null)
                            clusterControl.Filter = filter as IFilter<clsCluster>;

                        m_clusterFilterControls.Add(clusterControl.Filter, clusterControl as UserControl);
                        filterListBox.Items.Add(clusterControl.Filter);
                    }
                }

                if (control != null)
                {
                    filterContainer.Panel2.SuspendLayout();
                    control.BorderStyle = BorderStyle.FixedSingle;
                    control.Dock = DockStyle.Top;
                    filterContainer.Panel2.Controls.Add(control);
                    control.BringToFront();
                    filterContainer.Panel2.ResumeLayout();
                }            
        }
        #endregion
    }
}