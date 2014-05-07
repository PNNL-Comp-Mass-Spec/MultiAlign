using System.Collections.Generic;
using System.Linq;
using OxyPlot;

namespace MultiAlign.ViewModels.Charting
{
    public class MarkerTypeIterator
    {
        private readonly IList<MarkerType> m_markers;
        public MarkerTypeIterator()
        {
            m_markers = new List<MarkerType>
            {
                MarkerType.Circle,
                MarkerType.Cross,
                MarkerType.Diamond,
                MarkerType.Plus,
                MarkerType.Square,
                MarkerType.Triangle                
            };
        }

        public MarkerType GetMarker(int i)
        {
            return m_markers[i % m_markers.Count()];
        }
    }
}