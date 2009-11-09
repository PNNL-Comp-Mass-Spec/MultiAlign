using System ;
using System.Collections ;
using System.ComponentModel ;
using System.Drawing ;
using System.Drawing.Imaging ;
using System.Windows.Forms ;

namespace PNNLControls {
	/// <summary>
	/// Wraps an arraylist into a list of images.
	/// </summary>
	[Editor(typeof(ImageCollectionEditor),typeof(System.Drawing.Design.UITypeEditor))]
	[Serializable]
	public class ImageCollection : System.Collections.ICollection, System.Collections.IList
	{
		private ArrayList mImages = new ArrayList();

		public ImageCollection() 
		{
		}

		public ImageCollection(ArrayList images) 
		{
			foreach (Object o in images) 
			{
				if (!(o is Image) && !(o == null)) 
				{
					throw new ArgumentException("All members of images must be of Image type.", "images");
				}
			}
			mImages = images;
		}

		public int Add(Icon icon) 
		{
			if (icon == null) 
			{
				throw new ArgumentNullException("icon");
			}
			return mImages.Add(icon.ToBitmap());
		}

		public int Add(Image image) 
		{
			if (image == null) 
			{
				throw new ArgumentNullException("image");
			}
			return mImages.Add(image);
		}

		public void AddRange(Image [] images) 
		{
			foreach(Image image in images) 
			{
				Add(image) ;
			}
		}

		public Image this[int index] 
		{
			get 
			{
				return (Image) mImages[index];
			}
			set 
			{
				mImages[index] = value;
			}
		}


		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				return mImages.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			mImages.CopyTo(array, index);			
		}

		public object SyncRoot
		{
			get
			{
				return mImages.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return mImages.GetEnumerator();
		}

		#endregion

		#region IList Members

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return mImages[index];
			}
			set
			{
				this.RemoveAt(index);
				this.Insert(index, value);
			}
		}

		public void RemoveAt(int index)
		{
			mImages.RemoveAt(index);
		}

		public void Insert(int index, object value)
		{
			//MessageBox.Show("Typeof insert " + value.GetType());
			if ((value is Image) && value != null) 
			{
				mImages.Insert(index, (Image) value);
			} 
			else if ((value is Icon) && value != null) 
			{
				mImages.Insert(index, ((Icon) value).ToBitmap());
			} 
			else 
			{
				throw new ArgumentException("Value must be an non-null Image or Icon", "value");
			}
		}

		public void Remove(object value)
		{
			mImages.Remove(value);
		}

		public bool Contains(object value)
		{
			return mImages.Contains(value);
		}

		public void Clear()
		{
			mImages.Clear();
		}

		public int IndexOf(object value)
		{
			return mImages.IndexOf(value);
		}

		int IList.Add(object value)
		{
			//MessageBox.Show("Typeof add " + value.GetType());
			if ((value is Image) && value != null) 
			{
				return this.Add((Image) value);
			} 
			else if ((value is Icon) && value != null) 
			{
				return this.Add((Icon) value);
			}
			else 
			{
				throw new ArgumentException("Value must be an non-null Image or Icon", "value");
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return mImages.IsFixedSize;
			}
		}
		#endregion
	}

}
