using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLControls.Data
{
    /// <summary>
    /// Cache array base class for creating a caching algorithm for array lists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class clsCacheArray<T> : IDisposable where T : new()
    {
        protected string mstr_version;
        protected string mstr_name;
        protected long mlng_arrayLength = -1;

        private long mlng_cacheLength;
        private long mlng_lowIndex = -1;
        private long mlng_highIndex = -1;
        private T[] marr_dataCache;

        /// <summary>
        /// Constructor for the cache array.
        /// </summary>
        /// <param name="cacheLength">Length of the cache to set.</param>
        public clsCacheArray(long cacheLength, long arraysize)
        {
            if (cacheLength <= 0)
                throw new Exception("Cache length must be greater than one.");

            mstr_version = "version not set.";
            mlng_arrayLength = arraysize;
            mlng_highIndex = cacheLength;
            mlng_lowIndex = 0;
            mlng_cacheLength = cacheLength;
            marr_dataCache = new T[cacheLength];
            for (long i = 0; i < cacheLength; i++)
            {
                marr_dataCache[i] = new T();
            }
        }

        /// <summary>
        /// Handles figuring out if the derived class needs to cache and retrieve new data.
        /// </summary>
        /// <param name="index"></param>
        protected virtual void Page(long index, bool retrieve, bool cache)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Paging cache {0},{1},{2}", index, retrieve, cache));
            /// 
            /// If we are accessing outside of the cache 
            /// Make sure we cache then retrieve.
            /// 
            if (index >= mlng_highIndex)
            {
                /// Cache the current data array.
                if (cache)
                    Cache(marr_dataCache, mlng_lowIndex, mlng_highIndex - mlng_lowIndex);

                /// 
                /// This will move the current index to the beginning of the cache.
                ///                                 
                mlng_lowIndex = mlng_highIndex;
                mlng_highIndex = Math.Min(mlng_arrayLength, mlng_lowIndex + mlng_cacheLength);

                ///
                /// If the index is at the end of the array, then we need to cache back enough data,
                /// the high index will be one greater than the array itself, however, this means 
                /// that the next get/set/dispose call will cache the data and the database will be synched.
                /// 
                if (index >= mlng_arrayLength)
                {
                    System.Diagnostics.Debug.WriteLine("Paging beyond the end of the array.");
                    IndexOutOfRangeException access = new IndexOutOfRangeException(String.Format("Tried to access data outside of the array bounds.  Index: {0} Length: {1}", index, mlng_arrayLength));
                    throw access;
                }
                System.Diagnostics.Debug.WriteLine(String.Format("Paging array at index: {0} low index: {1} high index: {2}", index, mlng_lowIndex, mlng_highIndex));
                /// Retrieve the next set of data.
                if (retrieve)
                {
                    for (long i = 0; i < marr_dataCache.Length; i++)
                    {
                        marr_dataCache[i] = default(T);
                    }
                    Retrieve(marr_dataCache, mlng_lowIndex, mlng_highIndex - mlng_lowIndex);
                }
            }
            else if (index < mlng_lowIndex)
            {
                /// Cache the current data array.
                if (cache)
                    Cache(marr_dataCache, mlng_lowIndex, mlng_highIndex - mlng_lowIndex);

                /// 
                /// This will move the current index to the end of the cache.
                ///                                 
                mlng_highIndex = index;
                mlng_lowIndex = Math.Max(0, mlng_highIndex - mlng_cacheLength);

                ///
                /// If the index is at the end of the array, then we need to cache back enough data,
                /// the high index will be one greater than the array itself, however, this means 
                /// that the next get/set/dispose call will cache the data and the database will be synched.
                /// 
                if (index >= mlng_arrayLength)
                {
                    System.Diagnostics.Debug.WriteLine("Paging beyond the end of the array.");
                    IndexOutOfRangeException access = new IndexOutOfRangeException(String.Format("Tried to access data outside of the array bounds.  Index: {0} Length: {1}", index, mlng_arrayLength));
                    throw access;
                }
                System.Diagnostics.Debug.WriteLine(String.Format("Paging array at index: {0} low index: {1} high index: {2}", index, mlng_lowIndex, mlng_highIndex));
                /// Retrieve the next set of data.
                if (retrieve)
                {
                    for (long i = 0; i < marr_dataCache.Length; i++)
                    {
                        marr_dataCache[i] = default(T);
                    }
                    Retrieve(marr_dataCache, mlng_lowIndex, mlng_highIndex - mlng_lowIndex);
                }
            }
        }

        /// <summary>
        /// Access the data by supplying the index.
        /// </summary>
        /// <param name="index">The index of the object in the array.</param>
        /// <returns>Returns the object in the array.</returns>
        public virtual T this[long index]
        {
            get
            {
                Page(index, true, true);
                return marr_dataCache[Convert.ToInt32(index - mlng_lowIndex)];
            }
            set
            {
                Page(index, true, true);
                marr_dataCache[Convert.ToInt32(index - mlng_lowIndex)] = value;
            }
        }
        /// <summary>
        /// Access the data in the array between start and end
        /// </summary>
        /// <param name="start">Starting index of data array to access</param>
        /// <param name="end">Ending index of data array to access.</param>
        /// <returns>Data between start and end as Array of type T</returns>
        public virtual T[] this[long start, long end]
        {
            get { throw new Exception("Has not been implemented"); }
            set { throw new Exception("Has not been implemented"); }
        }

        /// <summary>
        /// Cache the array data to the stream.
        /// </summary>
        /// <param name="data">Data of type T to cache</param>
        /// <param name="startIndex">Start index to cache</param>
        /// <param name="length">Length to cache</param>
        protected abstract void Cache(T[] data, long startIndex, long length);
        /// <summary>
        /// Retrieve an array of T starting at index of amount elements.
        /// </summary>
        /// <param name="index">Index of data stream to pull from</param>
        /// <param name="amount">Amount to retrieve.</param>
        /// <returns>Array of type T.</returns>
        protected abstract void Retrieve(T[] data, long index, long length);

        /// <summary>
        /// Resizes the array with the newly specified array length.
        /// </summary>
        /// <param name="newArraySize">Length of the array to resize to.</param>
        public virtual void Resize(long arrayLength)
        {
            mlng_arrayLength = arrayLength;

            if (mlng_lowIndex < mlng_arrayLength)
                mlng_lowIndex = mlng_arrayLength - 1;

            // Paging will figure out what to do with the caching scheme.
            Page(mlng_lowIndex, true, true);
        }

        /// <summary>
        /// Changes the cache size.  
        /// </summary>
        public virtual long CacheLength
        {
            get { return mlng_cacheLength; }
            set
            {
                if (value <= 0)
                    throw new Exception("Cannot create an array with cache length less than one.");

                Page(mlng_lowIndex, true, false);
                mlng_cacheLength = value;
                mlng_highIndex = Math.Min(mlng_arrayLength, mlng_lowIndex + mlng_cacheLength);
                marr_dataCache = new T[value];
                for (long i = 0; i < value; i++)
                    marr_dataCache[i] = new T();

                Page(mlng_lowIndex, true, false);
            }
        }

        /// <summary>
        /// Length of the cache - total number of objects to hold
        /// </summary>
        public virtual long Length
        {
            get { return mlng_arrayLength; }
        }

        /// <summary>
        /// Low index of the objects in the cache.
        /// </summary>
        protected long LowIndex
        {
            get { return mlng_lowIndex; }
            set { mlng_lowIndex = value; }
        }

        /// <summary>
        /// High index of the objects in the cache.
        /// </summary>
        protected long HighIndex
        {
            get { return mlng_highIndex; }
            set { mlng_highIndex = value; }
        }

        /// <summary>
        /// File format version information.
        /// </summary>
        public string Version
        {
            get { return mstr_version; }
            set { mstr_version = value; }
        }

        /// <summary>
        /// Name of the data cache object.
        /// </summary>
        public string Name
        {
            get { return mstr_name; }
        }

        #region IDisposable Members


        /// <summary>
        /// Handle disposing 
        /// </summary>
        public virtual void Dispose()
        {
            /// 
            /// Make sure the data is cached before its disposed of.
            ///             
            Cache(marr_dataCache, mlng_lowIndex, mlng_highIndex - mlng_lowIndex);
        }

        #endregion
    }
}
