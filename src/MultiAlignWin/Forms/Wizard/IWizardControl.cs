using System;
using System.Collections.Generic;

using System.Windows.Forms;


namespace MultiAlignWin.Forms.Wizard
{

    public interface IWizardControl<T>
    {      
        /// <summary>
        /// Sets the title.
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Sets this as the current active page.
        /// </summary>
        void SetAsActivePage();
        /// <summary>
        /// Gets or sets the current MultiAlign analysis.
        /// </summary>
        T Data { get; set; }
        /// <summary>
        /// Determines if the current page is complete and can be moved forward.
        /// </summary>
        /// <returns></returns>
        bool IsComplete();
    }

    public class WizardEventArgs: EventArgs
    {                
    }
}
