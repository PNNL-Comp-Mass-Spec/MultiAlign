using System;
using System.Collections;
using System.Windows.Forms;


namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsLabelCollection.
	/// </summary>
	public class clsLabelCollection: System.Collections.ArrayList
	{
		/// <summary>
		/// holds properties common to all labels in the collection
		/// </summary>
		private ctlLabelItem mLabelProps = new ctlLabelItem();

		//field to draw labels in
		private Panel mPanel = null;
		private bool mVertical = false;
		private int mMinEdge = 0;
		private int mMaxEdge = 0;
		private int mEdgeToEdge = 1;

		//limit the number of labels that can be drawn and the size of the
		//field that can be created
		private const int MaxScrollDim = 1000;

		public int[] alignment = null;

		public  ctlLabelItem LevelProperties
		{
			get{return mLabelProps;}
			set{mLabelProps = value;}
		}

		private int PanelLongDim 
		{
			get
			{
				if (mVertical)
					return (mPanel.Height);
				else
					return (mPanel.Width);
			}
		}

		public int AverageShortDim
		{
			get
			{
				int longSide = Math.Max(1,PanelLongDim);

				if (this.Count==0)
					return 0;
				else
					return (int) (longSide / this.Count);
			}
		}

		private void ApplyProperties(ctlLabelItem lbl)
		{
			lbl.Font = mLabelProps.Font;
		}

		private void ApplyProperties()
		{
			for (int i=0; i<this.Count; i++)
			{
				ApplyProperties(this[i] as ctlLabelItem);
			}
		}

		public void Edit()
		{
			mLabelProps.ShowEdit();
			ApplyProperties();
		}

		public void BuildAlignment ()
		{
			try
			{
				alignment = new int[this.Count+1];

				for (int i=0; i<this.Count; i++)
				{
					clsLabelAttributes lblPtr = this[i] as clsLabelAttributes;

					if (mVertical)
					{
						alignment[i] = Math.Max(lblPtr.label.Top, 0);
					}
					else
					{
						alignment[i] = Math.Max(lblPtr.label.Left, 0);
					}
					if (alignment[i]<0) 
						alignment[i] = 0;
				}

				//cap the alignment array
				if (mVertical)
					alignment[this.Count] = mPanel.Height-1;
				else
					alignment[this.Count] = mPanel.Width-1;

				mMinEdge = int.MaxValue;
				mMaxEdge = int.MinValue;

				for (int i=0; i<alignment.Length; i++)
				{
					if (alignment[i]<mMinEdge) 
						mMinEdge=alignment[i];
					if (alignment[i]>mMaxEdge) 
						mMaxEdge=alignment[i];
				}
				mEdgeToEdge = mMaxEdge-mMinEdge+1;
				if (mEdgeToEdge<=0) mEdgeToEdge=1;
			}
			catch(Exception ex){
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}		
		}

		public int SnapToAlignment(int pos)
		{
			int index = (int)(((double) pos / (double) mEdgeToEdge) * (double)alignment.Length);
			return(alignment[index]);
		}

		public clsLabelCollection(Panel panel, bool vertical)
		{
			mPanel = panel;
			mVertical = vertical;
		}

		public int Add(ctlLabelItem value)
		{
			ApplyProperties(value);
			return base.Add (value);
		}

	}
}
