using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using MultiAlignEngine.Features;

namespace MultiAlignWin.Forms
{
    public partial class controlUMCData : UserControl
    {
        public controlUMCData()
        {
            InitializeComponent();
        }

        public void DisplayUMCData(clsUMC umcData)
        {
            mlistview_properties.BeginUpdate();
            ListViewItem item = new ListViewItem();
            item.Text = "Cluster Index";
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mint_cluster_index.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Dataset Index";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mint_dataset_index.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Abundance";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mdouble_abundance.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Representative M/Z";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mdouble_class_rep_mz.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Monoisotopic Mass";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mdouble_mono_mass.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Calibrated Monoisotopic Mass";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mdouble_mono_mass_calibrated.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Normalized Elution Time (NET)";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mdouble_net.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Scan";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mint_scan.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Start Scan";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mint_start_scan.ToString();
            item.SubItems.Add(subItem);            
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "End Scan";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mint_end_scan.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);


            item = new ListViewItem();
            item.Text = "UMC Index";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mint_umc_index.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            item = new ListViewItem();
            item.Text = "Highest Charge State";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mshort_class_highest_charge.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);
            
            item = new ListViewItem();
            item.Text = "Representative Charge State";
            subItem = new ListViewItem.ListViewSubItem();
            subItem.Text = umcData.mshort_class_rep_charge.ToString();
            item.SubItems.Add(subItem);
            mlistview_properties.Items.Add(item);

            mlistview_properties.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_properties.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_properties.EndUpdate();
        }
    }
}
