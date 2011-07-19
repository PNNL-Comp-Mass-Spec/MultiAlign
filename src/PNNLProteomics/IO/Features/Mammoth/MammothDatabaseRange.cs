using System;

using PNNLOmics.Algorithms;

namespace PNNLProteomics.IO.Mammoth
{
    /// <summary>
    /// Options for searching a UMC database.
    /// </summary>
    public class MammothDatabaseRange
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="massMin"></param>
        /// <param name="massMax"></param>
        /// <param name="NETMin"></param>
        /// <param name="NETMax"></param>
        public MammothDatabaseRange(double massMin,
                                     double massMax,
                                     double NETMin,
                                     double NETMax,
                                     double driftTimeMin,
                                     double driftTimeMax,
                                     int singleChargeState)
        {
            MassMaximum      = massMax;
            MassMinimum      = massMin;
            NETMinimum       = NETMin;
            NETMaximum       = NETMax;
            DriftTimeMaximum = driftTimeMax;
            DriftTimeMinimum = driftTimeMin;
            SingleChargeState = singleChargeState;
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="massMin"></param>
        /// <param name="massMax"></param>
        /// <param name="NETMin"></param>
        /// <param name="NETMax"></param>
        public MammothDatabaseRange(double massMin,
                                     double massMax,
                                     double NETMin,
                                     double NETMax,
                                     double driftTimeMin,
                                     double driftTimeMax) :
                                            this(massMin,
                                              massMax,
                                              NETMin,
                                              NETMax,
                                              driftTimeMin,
                                              driftTimeMax,
                                              -1)
        {
        }

        #region Properties
        /// <summary>
        /// Gets or sets the mass maximum value.
        /// </summary>
        public double MassMaximum { get; set; }
        /// <summary>
        /// Gets or sets the mass minimum value.
        /// </summary>
        public double MassMinimum { get; set; }
        /// <summary>
        /// Gets or sets the NET maximum value.
        /// </summary>
        public double NETMaximum { get; set; }
        /// <summary>
        /// Gets or sets the NET minimum value.
        /// </summary>
        public double NETMinimum { get; set; }
        /// <summary>
        /// Gets or sets the maximum drift time value.
        /// </summary>
        public double DriftTimeMaximum { get; set; }
        /// <summary>
        /// Gets or sets the minimum drift time value.
        /// </summary>
        public double DriftTimeMinimum { get; set; }
        #endregion

        /// <summary>
        /// Gets or sets the charge state to cluster.   CS <= 0 indicates all charge states.
        /// </summary>
        public int SingleChargeState
        {
            get;set;
        }

        #region IComparable<DatabaseSearchOptions> Members
        /// <summary>
        /// Compares this against the supplied object to see if they are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            MammothDatabaseRange other = obj as MammothDatabaseRange;
            if (other == null)
                return false;

            if (!other.DriftTimeMinimum.Equals(DriftTimeMinimum))
                return false;
            if (!other.DriftTimeMaximum.Equals(DriftTimeMaximum))
                return false;
            if (!other.NETMinimum.Equals(NETMinimum))
                return false;
            if (!other.NETMaximum.Equals(NETMaximum))
                return false;
            if (!other.MassMinimum.Equals(MassMinimum))
                return false;
            if (!other.MassMaximum.Equals(MassMaximum))
                return false;

            return true;
        }
        #endregion
    }
}
