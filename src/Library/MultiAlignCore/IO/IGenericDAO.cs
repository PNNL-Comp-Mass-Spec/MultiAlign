namespace MultiAlignCore.IO
{
    using System;
    using System.Collections.Generic;
    using InformedProteomics.Backend.Utils;

    /// <summary>
    /// This is an interface describes the operations that can be performed on a CRUD-like data access object.
    /// </summary>
    /// <typeparam name="T">The type of data that is persisted with this data access object.</typeparam>
    public interface IGenericDAO<T>
    {
        /// <summary>
        /// Persist a single element.
        /// </summary>
        /// <param name="t">The element to persist.</param>
        void Add(T t);

        /// <summary>
        /// Persist many elements.
        /// </summary>
        /// <param name="tList">The elements to persist.</param>
        /// <param name="progress">Progress reporter for reporting progress percentage and status updates.</param>
        void AddAll(ICollection<T> tList, IProgress<ProgressData> progress = null);

        /// <summary>
        /// Update the state of a single element.
        /// </summary>
        /// <param name="t">The element to update.</param>
        void Update(T t);

        /// <summary>
        /// Update the state of many elements.
        /// </summary>
        /// <param name="tList">The elements to update.</param>
        /// <param name="progress">Progress reporter for reporting progress percentage and status updates.</param>
        void UpdateAll(ICollection<T> tList, IProgress<ProgressData> progress = null);

        /// <summary>
        /// Delete a single element.
        /// </summary>
        /// <param name="t">The element to delete.</param>
        void Delete(T t);

        /// <summary>
        /// Delete many elements.
        /// </summary>
        /// <param name="tList">The elements to delete.</param>
        void DeleteAll(ICollection<T> tList);

        /// <summary>
        /// Find an element with the provided identification number.
        /// </summary>
        /// <param name="id">The identification number to find.</param>
        /// <returns>The element, if found.</returns>
        T FindById(int id);

        /// <summary>
        /// Find all elements of the persistent type.
        /// </summary>
        /// <returns>The elements that were found.</returns>
        List<T> FindAll();
    }
}