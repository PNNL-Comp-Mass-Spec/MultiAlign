using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignWin.Forms.Wizard
{
    public class WizardController<T> where T: class
    {
        /// <summary>
        /// Points to the current wizard page index.
        /// </summary>
        private int m_pointer;
        /// <summary>
        /// List of available wizard pages.
        /// </summary>
        private List<T> m_wizardPages;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WizardController()
        {
            m_wizardPages = new List<T>();
        }
        /// <summary>
        /// Adds a wizard page to the list of available pages.
        /// </summary>
        /// <param name="wizardPage"></param>
        public void AddWizardPage(T wizardPage)
        {
            if (wizardPage == null)
                throw new NullReferenceException("The wizard page cannot be null.");

            m_wizardPages.Add(wizardPage);
        }
        /// <summary>
        /// Moves page to previous page.
        /// </summary>
        /// <returns></returns>
        public T GetPrevious()
        {
            if (m_wizardPages.Count < 1)
                return default(T);

            if (m_pointer - 1 < 0)
                return default(T);

            m_pointer--;
            return m_wizardPages[m_pointer];
        }
        /// <summary>
        /// Moves page to next page.
        /// </summary>
        /// <returns></returns>
        public T GetNext()
        {
            if (m_wizardPages.Count < 1)
                return default(T);

            if (m_wizardPages.Count <= m_pointer + 1)
                return default(T);

            m_pointer++;
            return m_wizardPages[m_pointer];
        }
        /// <summary>
        /// Gets the current wizard page.
        /// </summary>
        /// <returns></returns>
        public T GetCurrent()
        {

            if (m_wizardPages.Count < 1)
                return default(T);

            if (m_wizardPages.Count <= m_pointer)
                return default(T);

            return m_wizardPages[m_pointer];
        }
        /// <summary>
        /// Determines if at the last page.
        /// </summary>
        /// <returns></returns>
        public bool IsLast()
        {
            return (m_wizardPages.Count == m_pointer + 1);
        }
        /// <summary>
        /// Determines if at the first page.
        /// </summary>
        /// <returns></returns>
        public bool IsFirst()
        {
            return m_pointer == 0;
        }
    }
}
