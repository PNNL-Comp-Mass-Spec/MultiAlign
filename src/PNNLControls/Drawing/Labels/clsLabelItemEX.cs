using System;
using System.Collections;
using System.Windows.Forms;
using PNNLControls;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsEXHierarchalLabel.
	/// </summary>
	public class ctlLabelItemEX : PNNLControls.ctlLabelItem
	{
		private Stack m_aliasStack = new Stack();
		
		public ctlLabelItemEX()
		{
			m_aliasStack.Push(base.Text);			
		}
		
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				m_aliasStack.Push(base.Text);
				base.Text = value;
			}
		}

		public void UndoText()
		{
			if (m_aliasStack.Count > 1)
				base.Text = m_aliasStack.Pop() as string;
		}

	}
}
