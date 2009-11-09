using System;
using System.Collections;

namespace PNNLProteomics.Data.Factors
{
	public class classTreeNode
	{
		private classTreeNode m_parent;
		private ArrayList   m_children;		
		private long		m_level;
		private long		m_idNumber;
		private long		m_groupIDNumber;		
		private string		m_groupIDName;
		private string		m_name;
		
		public classTreeNode()
		{
			Init();
		}
		
		public classTreeNode(classTreeNode parent)
		{
			Init();
			m_parent = parent;
		}

		protected void Init()
		{			
			m_level			= -1;
			m_idNumber		= -1;
			m_groupIDNumber = -1;
			m_name			= string.Empty;
			m_groupIDName	= string.Empty;

			m_parent = null;
			m_children = new ArrayList();
		}

		public ArrayList Children
		{
			get
			{
				return this.m_children;
			}
			set
			{
				this.m_children = value;
			}
		}


		public bool HasChildren()
		{
			return (m_children != null) && (m_children.Count > 0);
		}

		public long GroupIDNumber
		{
			get
			{
				return m_groupIDNumber;
			}
			set
			{
				m_groupIDNumber = value;
			}
		}

		public string GroupIDName
		{
			get
			{
				return m_groupIDName;
			}
			set
			{
				m_groupIDName = value;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public long Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}

		public long IDNumber
		{
			get
			{
				return m_idNumber;
			}
			set
			{
				m_idNumber = value;
			}
		}

		public classTreeNode Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				m_parent = value;
			}
		}
	}
}
