using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PNNLControls
{
	public class ChartMarkerLayer : System.Collections.IEnumerable
	{
		/// <summary>
		/// The chart that this is the marker layer for.  MarkerLayers can only be created 
		/// for a single chart, and each chart has only one marker layer.
		/// </summary>
		private ctlChartBase mChart;

		private ArrayList mMarkersList = new ArrayList();

		/// <summary>
		/// The list of which markers are currently eligible to be drawn, because the 
		/// point to which each is attached is within the viewport of the chart.
		/// </summary>
		private ArrayList mCurrentlyVisibleMarkersList = new ArrayList();

		private Font mFont = new Font(FontFamily.GenericSerif, 12);
		private int mMaxMarkerWidth = 100;

		internal ChartMarkerLayer(ctlChartBase chart) 
		{
			if (chart == null) 
			{
				throw new ArgumentNullException("chart");
			}
			mChart = chart;
		}

		/// <summary>
		/// Gets the font used for added markers.  Sets the font for all markers 
		/// in the collection.
		/// </summary>
		public Font Font 
		{
			get 
			{
				return (Font) mFont.Clone();
			} 
			set 
			{
				mFont.Dispose();
				if (value == null) 
				{
					throw new ArgumentNullException();
				}
				mFont = (Font) value.Clone();
				foreach (ChartMarker marker in mMarkersList) 
				{
					marker.Font = mFont;
				}
				this.Update();
			}
		}


		public int MaxMarkerWidth 
		{
			get 
			{
				return mMaxMarkerWidth;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentOutOfRangeException("MaxMarkerWidth", value, "Maximum marker width must be > 0");
				}
				this.mMaxMarkerWidth = value;
				this.Update();
			}
		}

		public void Add(ChartMarker marker) 
		{
			marker.Font = this.Font;
			this.mMarkersList.Add(marker);
			this.Update();
		}

		public void AddAll(System.Collections.IEnumerable markers) 
		{
			foreach (ChartMarker marker in markers) 
			{
				this.Add(marker);
			}
		}

		public void Remove(ChartMarker marker) 
		{
			this.mMarkersList.Remove(marker);
			this.Update();
		}

		public void RemoveAll(System.Collections.IEnumerable markers) 
		{
			foreach (ChartMarker marker in markers) 
			{
				this.Remove(marker);
			}
		}

		public void Draw(Graphics g) 
		{
			// Draw markers from high to low priority.  Low priority marker will thus 
			// be drawn underneath high-priority markers.
			this.mCurrentlyVisibleMarkersList.Sort(new MarkerComparerLowPriority());
			foreach (ChartMarker marker in this.mCurrentlyVisibleMarkersList) 
			{
				if (marker.Visible) 
				{
					marker.Draw(g);
				}
			}
		}

		private void Update() 
		{
			this.mChart.PerformLayout();
		}

		public void Layout(Graphics g) 
		{
			//Console.WriteLine("Layout of Marker layer");
			RectangleF viewPort = this.mChart.ViewPort;
			this.mCurrentlyVisibleMarkersList.Clear();
			// Loop through markers, selecting all of those which are within the 
			// current viewport of the chart.
			for(int i = 0; i < mMarkersList.Count; i++)
			{
				ChartMarker marker = (ChartMarker) mMarkersList[i];
				// set the index of the marker to allow for stable sorting
				marker.SetVisible(false);
				marker.Index = i;
				if ((marker.Visibility != MarkerVisibility.Never) && viewPort.Contains(marker.ChartLocation)) 
				{
					this.mCurrentlyVisibleMarkersList.Add(marker);
				} 
			}

			// sort markers in order of highest priority first
			mCurrentlyVisibleMarkersList.Sort(new MarkerComparerHighPriority());

			foreach (ChartMarker marker in this.mCurrentlyVisibleMarkersList) 
			{
				Size markerSize = marker.GetSize(g, new Size(this.MaxMarkerWidth, int.MaxValue));

				int x = (int) mChart.GetScreenPixelX(marker.ChartLocation.X);
				int y = (int) mChart.GetScreenPixelY(marker.ChartLocation.Y);

				Rectangle firstTrialBounds = GetBounds(marker, markerSize, new Point(x, y));
				if (GoodBounds(marker, firstTrialBounds)) 
				{
					SetBounds(marker, firstTrialBounds, false);
					continue;
				}
				// If the alignment is fixed, then we have to decide on whether to show the 
				// marker or not.  If it must be shown then show it.  Otherwise, since the 
				// bounds are not good, don't show it (don't have to do anything in this 
				// case, since the marker is already set to not visible.)
				if (marker.FixedAlignment) 
				{
					if (marker.Visibility == MarkerVisibility.Always) 
					{
						SetBounds(marker, firstTrialBounds, false);
					} 
					continue;
				}
			}

			// For each marker from "most important" to "least important"

			// Try current - 
			// If works, accept
			// If fixed, accept or reject, move to next marker
			// Find place for marker
			// Accept or reject.
		}

		private void SetBounds(ChartMarker marker, Rectangle bounds, bool updateAlignment) 
		{
			marker.Bounds = bounds;
			marker.SetVisible(true);
		}

		/// <summary>
		/// Gets the bounds for a marker given the current offset and alignment of 
		/// the marker.
		/// </summary>
		/// <param name="marker"></param>
		/// <param name="alignment"></param>
		/// <param name="spacing"></param>
		/// <returns></returns>
		private Rectangle GetBounds(ChartMarker marker, Size markerSize, Point at) 
		{
			int left = 0;
			int top = 0;
			if (marker.Alignment == ContentAlignment.BottomCenter || 
				marker.Alignment == ContentAlignment.BottomLeft || 
				marker.Alignment == ContentAlignment.BottomRight) 
			{
				top = at.Y + marker.VerticalOffset;
			} 
			else if (marker.Alignment == ContentAlignment.MiddleCenter ||
				marker.Alignment == ContentAlignment.MiddleLeft || 
				marker.Alignment == ContentAlignment.MiddleRight) 
			{
				top = at.Y + marker.VerticalOffset - markerSize.Height / 2;
			} 
			else if (marker.Alignment == ContentAlignment.TopCenter ||
				marker.Alignment == ContentAlignment.TopLeft || 
				marker.Alignment == ContentAlignment.TopRight) 
			{
				top = at.Y - markerSize.Height - marker.VerticalOffset;
			}

			if (marker.Alignment == ContentAlignment.BottomLeft ||
				marker.Alignment == ContentAlignment.MiddleLeft ||
				marker.Alignment == ContentAlignment.TopLeft) 
			{
				left = at.X - markerSize.Width - marker.HorizontalOffset;
			} 
			else if (marker.Alignment == ContentAlignment.BottomCenter ||
				marker.Alignment == ContentAlignment.MiddleCenter ||
				marker.Alignment == ContentAlignment.TopCenter) 
			{
				left = at.X - markerSize.Width / 2 - marker.HorizontalOffset;
			} 
			else if (marker.Alignment == ContentAlignment.BottomRight ||
				marker.Alignment == ContentAlignment.MiddleRight ||
				marker.Alignment == ContentAlignment.TopRight) 
			{
				left = at.X + marker.HorizontalOffset;
			}
			Rectangle bounds = new Rectangle(new Point(left, top), markerSize);
			//Console.WriteLine("Bounds: {0}", bounds);
			return bounds;
		}

		private bool GoodBounds(ChartMarker marker, Rectangle bounds) 
		{
			return true;
		}

		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return mMarkersList.GetEnumerator();
		}
		#endregion
	}

	/// <summary>
	/// Summary description for ChartMarker.
	/// </summary>
	public abstract class ChartMarker
	{
		public const int MIN_MARKER_PRIORITY = 0;
		public const int MAX_MARKER_PRIORITY = 100;

		public event EventHandler MarkerSettingsChanged;

		private MarkerVisibility mVisibility = MarkerVisibility.Optional;
		private Size mSpacing = new Size(5, 5);
		private int mPriority = (MIN_MARKER_PRIORITY + MAX_MARKER_PRIORITY) / 2;
		private bool mVisible = false;

		private Rectangle mBounds = new Rectangle(0, 0, 0, 0);
		private PointF mChartLocation;

		private ContentAlignment mAlignment = ContentAlignment.MiddleCenter;
		private bool mFixedAlignment = false;
		private bool mOverlapsSeries = false;
		private bool mAllowsOverlaps = false;
		private int mIndex = -1;


		private Font mFont = new Font(FontFamily.GenericSerif, 8);
		private Color mColor = Color.Black;

		public ChartMarker(PointF location)
		{
			this.mChartLocation = location;
		}

		protected void OnMarkerSettingsChanged() 
		{
			if (this.MarkerSettingsChanged != null) 
			{
				this.MarkerSettingsChanged(this, EventArgs.Empty);
			}
		}

		#region "Spacing"
		/// <summary>
		/// Gets or sets the number of pixels that the marker is offset from the onscreen coordinates 
		/// of the chart location.  The Width field of the spacing is used to control the horizontal offset, 
		/// and the Height field controls the vertical offset.
		/// </summary>
		public Size Spacing 
		{
			get 
			{
				return mSpacing;
			}
			set 
			{
				mSpacing = value;
				this.OnMarkerSettingsChanged();
			}
		}

		/// <summary>
		/// Gets or sets the Height field of spacing.
		/// </summary>
		public int VerticalOffset 
		{
			get 
			{
				return mSpacing.Height;
			}
			set 
			{
				this.Spacing = new Size(this.HorizontalOffset, value);
			}
		}

		/// <summary>
		/// Gets or sets the Width field of Spacing.
		/// </summary>
		public int HorizontalOffset 
		{
			get 
			{
				return mSpacing.Width;
			}
			set 
			{
				this.Spacing = new Size(value, this.VerticalOffset);
			}
		}
		#endregion

		/// <summary>
		/// Allows the MarkerLayer to set the index of the marker within its list.  This allows 
		/// for a stable sort of the markers by priority, without having to actually write a 
		/// stable sort routine.
		/// </summary>
		internal int Index 
		{
			get 
			{
				return mIndex;
			}
			set 
			{
				mIndex = value;
			}
		}

		/// <summary>
		/// Tells whether the marker is currently drawn on the chart.  Returns false if the 
		/// point with which the marker is associated is not shown on the chart, or if the 
		/// ChartMarkerLayer has determined that the marker can not be drawn on the chart because 
		/// doing so would violate some overlap restrictions.
		/// </summary>
		public bool Visible 
		{
			get 
			{
				return mVisible;
			}
		}

		/// <summary>
		/// Provides a way for the ChartMarkerLayer to set whether the marker is Visible, 
		/// without making this a public method.
		/// </summary>
		/// <param name="value"></param>
		internal void SetVisible(bool value) 
		{
			mVisible = value;
		}

		/// <summary>
		/// Gets or sets the bounds of the marker, relative to the upper-left of the 
		/// chart area.
		/// </summary>
		internal Rectangle Bounds 
		{
			get 
			{
				return mBounds;
			}
			set 
			{
				this.mBounds = value;
				this.OnMarkerSettingsChanged();
			}
		}

		#region "Layout options"

		/// <summary>
		/// Gets or sets the alignment of the marker relative to the on-screen location of the 
		/// chart point it is associated with.
		/// </summary>
		public ContentAlignment Alignment 
		{
			get 
			{
				return mAlignment;
			}
			set 
			{
				mAlignment = value;
				this.OnMarkerSettingsChanged();
			}
		}

		/// <summary>
		/// Controls whether the ChartMarkerLayer is allowed to change the alignment and offsets 
		/// of the chart marker.  If true, the ChartMarkerLayer is forced to use the current settings
		/// of Alignment and Spacing.  If false, the ChartMarkerLayer will first see if the current 
		/// settings work for positioning the 
		/// </summary>
		public bool FixedAlignment 
		{
			get 
			{
				return mFixedAlignment;
			}
			set 
			{
				mFixedAlignment = value;
				this.OnMarkerSettingsChanged();
			}
		}

		/// <summary>
		/// Gets or sets the priority of the marker.  Markers are arranged on the chart 
		/// from highest to lowest priority.
		/// </summary>
		public int Priority 
		{
			get 
			{
				return mPriority;
			}
			set 
			{
				if (value > MAX_MARKER_PRIORITY || value < MIN_MARKER_PRIORITY) 
				{
					throw new ArgumentOutOfRangeException("Priority", value, "Marker priority must be between " + 
						MIN_MARKER_PRIORITY + " and " + MAX_MARKER_PRIORITY + " (inclusive).");
				}
				mPriority = value;
				this.OnMarkerSettingsChanged();
			}
		}

		/// <summary>
		/// Tells whether other markers are allowed to overlap this marker.  If false, an attempt 
		/// will be made to find a space for the marker where other markers do not overlap it.  If no 
		/// such place can be found and the Visibility setting is Optional, the marker will not be 
		/// displayed.  If Visibility is Always and no place can be found for the marker, the 
		/// best location (smallest overlap) is used.  This may result in the marker being overlapped 
		/// by other markers.
		/// </summary>
		public virtual bool AllowsOverlap 
		{
			get 
			{
				return mAllowsOverlaps;
			}
			set {
				this.mAllowsOverlaps = value;
				this.OnMarkerSettingsChanged();
			}
		}

		/// <summary>
		/// Tells whether this marker is allowed to overlap the plotted series data.  If false, an 
		/// attempt will be made to find a space for the marker where it does not overlap the 
		/// plotted series data.  If no such place can be found consistent with the alignment 
		/// and spacing considerations, and the Visibility setting is Optional, then the marker will
		/// not be shown.  If Visibility is Always and no place can be found for the marker, the best 
		/// location is used.  This may result in the marker overlapping the plotted series.
		/// </summary>
		public virtual bool OverlapsSeries 
		{
			get 
			{
				return this.mOverlapsSeries;
			}
			set 
			{
				this.mOverlapsSeries = value;
				this.OnMarkerSettingsChanged();
			}
		}

		/// <summary>
		/// Gets or sets the visibility of the marker.  If set to Always, the MarkerLayer must show 
		/// the marker (if it is associated with a visible point of the chart), doing the best possible 
		/// layout job.  If set to Optional, the marker may or may not be shown, depending on the 
		/// ability of the MarkerLayer to find a suitable place for it.  If set the Never, the marker 
		/// is never displayed.
		/// </summary>
		public virtual MarkerVisibility Visibility 
		{
			get 
			{
				return mVisibility;
			}
			set 
			{
				mVisibility = value;
				this.OnMarkerSettingsChanged();
			}
		}
		#endregion

		/// <summary>
		/// Gets the size of the marker, given the maximum size that it must fit within.
		/// </summary>
		public abstract Size GetSize(Graphics g, Size maximums);

		#region "Appearance"
		/// <summary>
		/// Gets or sets the font used for any text in the marker.
		/// </summary>
		public Font Font
		{
			get 
			{
				return (Font) mFont.Clone();
			}
			set 
			{
				this.mFont = (Font) value.Clone();
				this.OnMarkerSettingsChanged();
			}
		}

		/// <summary>
		/// Gets or sets the Color used in the foreground by the marker.
		/// </summary>
		public Color Color 
		{
			get 
			{
				return mColor;
			}
			set 
			{
				mColor = value;
				this.OnMarkerSettingsChanged();
			}
		}
		#endregion

		/// <summary>
		/// Gets or sets the chart coordinates of the anchor point of the 
		/// marker.
		/// </summary>
		public PointF ChartLocation 
		{
			get 
			{
				return this.mChartLocation;
			}
			set 
			{
				this.mChartLocation = value;
				this.OnMarkerSettingsChanged();
			}
		}

		/// <summary>
		/// Draw the marker within the currently set bounds on the given surface.
		/// </summary>
		/// <param name="g"></param>
		public void Draw(Graphics g) 
		{				
			GraphicsContainer container = g.BeginContainer();
			g.IntersectClip(this.Bounds);
			this.DrawNormalInternal(g);
			g.EndContainer(container);

		}

		/// <summary>
		/// Draws the normal form of the marker onto the given graphics.  The marker 
		/// is drawn within the currently set bounds.
		/// </summary>
		/// <param name="g"></param>
		protected abstract void DrawNormalInternal(Graphics g);
	}

	public class TextMarker : PNNLControls.ChartMarker
	{
		private String mText;

		public TextMarker(String text, PointF chartLocation) : base (chartLocation)
		{
			this.Text = text;
		}

		public String Text 
		{
			get 
			{
				return mText;
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("Text");
				}
				mText = value;
				this.OnMarkerSettingsChanged();
			}
		}
		protected override void DrawNormalInternal(Graphics g)
		{
			g.FillRectangle(Brushes.SkyBlue, this.Bounds);
			g.DrawString(this.Text, this.Font, Brushes.Black, this.Bounds, StringFormat.GenericDefault);
		}

		public override Size GetSize(Graphics g, Size maximums)
		{
			StringFormat format = StringFormat.GenericDefault;
			SizeF size = g.MeasureString(this.Text, this.Font, maximums.Width, StringFormat.GenericDefault);
			return new Size((int) Math.Ceiling(size.Width), (int) Math.Ceiling(size.Height));
		}
	}

	#region "Marker Comparers"
	class MarkerComparer 
	{
		protected int TotalScore(ChartMarker marker) 
		{
			int mult = (int) marker.Visibility;
			return marker.Priority + ChartMarker.MAX_MARKER_PRIORITY * mult;
		}
	}

	class MarkerComparerHighPriority : MarkerComparer, IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			ChartMarker cmx = x as ChartMarker;
			ChartMarker cmy = y as ChartMarker;
			if (cmx == null) 
			{
				return 1;
			}
			if (cmy == null) 
			{
				return -1;
			}
			int ret = TotalScore(cmx) - TotalScore(cmy);
			// for stable sort, if elements are equal, the one with the 
			// smallest index comes first.
			if (ret == 0) 
			{
				ret = cmy.Index - cmx.Index;
			}
			return ret;
		}

		#endregion
	}

	class MarkerComparerLowPriority : MarkerComparer, IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			ChartMarker cmx = x as ChartMarker;
			ChartMarker cmy = y as ChartMarker;
			if (cmx == null) 
			{
				return 1;
			}
			if (cmy == null) 
			{
				return -1;
			}
			int ret = TotalScore(cmx) - TotalScore(cmy);
			if (ret == 0) 
			{
				ret = cmx.Index - cmy.Index;
			}
			return ret;
		}

		#endregion
	}
	#endregion

	public enum MarkerVisibility
	{
		/// <summary>
		/// Whenever the chart coordinates with which the marker is associated are visible, the 
		/// marker is displayed.
		/// </summary>
		Always = 2, 
		/// <summary>
		/// Whenever the chart coordinates with which the marker is associated are visible, the 
		/// marker will attempt to be displayed.
		/// </summary>
		Optional = 1, 
		/// <summary>
		/// The marker is never visible on the chart.
		/// </summary>
		Never = 0
	}
}
