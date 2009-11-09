using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PNNLControls.Forms
{

    #region WM - Window Messages
    public enum WM
    {
        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        WM_QUERYOPEN = 0x0013,
        WM_ERASEBKGND = 0x0014,

    }
    #endregion

    #region RECT
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
    #endregion

    public class ctlListViewFlickerless : ListView
    {
        /// 
        /// Special thanks for the Code Project Article for flickerless ListView updates
        /// http://www.codeproject.com/KB/list/listviewff.aspx
        /// 

        /// <summary>
        /// Flag indicating if we are updating an item.
        /// </summary>
        private bool mbool_isUpdatingItem;
        /// <summary>
        /// The listview item that we are updating.
        /// </summary>
        private int mint_itemUpdating;

        #region Imported User32.DLL functions
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool ValidateRect(IntPtr handle, ref RECT rect);
        #endregion


        /// <summary>
        /// Default constructor for a list view control.
        /// </summary>
        public ctlListViewFlickerless():
            base()
        {
            
        }
                
        /// <summary>
        /// Override the window's paint procedure so we can get rid of the listview flicker when it's called to erase and redraw it.
        /// </summary>
        /// <param name="messg"></param>
        protected override void WndProc(ref Message messg)
        {
            if (mbool_isUpdatingItem)
            {
                // We do not want to erase the background, 
                // turn this message into a null-message
                if ((int)WM.WM_ERASEBKGND == messg.Msg)
                {
                    messg.Msg = (int)WM.WM_NULL;
                }
                else if ((int)WM.WM_PAINT == messg.Msg)
                {
                    RECT vrect = GetWindowRECT();
                    // validate the entire window                

                    ValidateRect(Handle, ref vrect);

                    //Invalidate only the new item

                    Invalidate(Items[mint_itemUpdating].Bounds);
                }
            }
            base.WndProc(ref messg);
        }

        /// <summary>
        /// Update a single item.
        /// </summary>
        /// <param name="itemIndex"></param>
        public void UpdateItem(int itemIndex)
        {
            mbool_isUpdatingItem    = true;
            mint_itemUpdating       = itemIndex;
            Update();
            mbool_isUpdatingItem    = false;            
        }


        // Get the listview's rectangle and return it as a RECT structure
        private RECT GetWindowRECT()
        {
            RECT rect   = new RECT();
            rect.left   = this.Left;
            rect.right  = this.Right;
            rect.top    = this.Top;
            rect.bottom = this.Bottom;
            return rect;
        }
    }
}
