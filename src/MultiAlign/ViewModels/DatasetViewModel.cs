using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels
{
    public class DatasetViewModel: ViewModelBase
    {
        private DatasetInformation m_information;
        public DatasetViewModel(DatasetInformation information)
        {
            m_information = information;
        }

        public string Name
        {
            get 
            {
                string name = "";
                if (m_information != null)
                {
                    name = m_information.DatasetName;
                }
                return name;
            }
        }
    }
}
