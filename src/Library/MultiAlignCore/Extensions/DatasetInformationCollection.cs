#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MultiAlignCore.Data.MetaData;

#endregion

namespace MultiAlignCore.Extensions
{
    public static class DatasetInformationExtensions
    {
        /// <summary>
        ///     Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static void AddRange(this ObservableCollection<DatasetInformation> myData,
            IEnumerable<DatasetInformation> datasets)
        {
            foreach (var info in datasets)
            {
                myData.Add(info);
            }
        }

        /// <summary>
        ///     Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static List<DatasetInformation> ToList(this ObservableCollection<DatasetInformation> myData)
        {
            var information = new List<DatasetInformation>();
            foreach (var info in myData)
            {
                information.Add(info);
            }
            return information;
        }

        /// <summary>
        ///     Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static ObservableCollection<DatasetInformation> ToObservableCollection(
            this List<DatasetInformation> myData)
        {
            return new ObservableCollection<DatasetInformation>(myData);
        }
    }
}