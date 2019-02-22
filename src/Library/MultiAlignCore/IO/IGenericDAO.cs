#region

using System;
using System.Collections.Generic;
using InformedProteomics.Backend.Utils;

#endregion

namespace MultiAlignCore.IO
{
    public interface IGenericDAO<T>
    {
        void Add(T t);
        void AddAll(ICollection<T> tList, IProgress<PRISM.ProgressData> progress = null);
        void AddAllStateless(ICollection<T> tList, IProgress<PRISM.ProgressData> progress = null);
        void Update(T t);
        void UpdateAll(ICollection<T> tList, IProgress<PRISM.ProgressData> progress = null);
        void UpdateAllStateless(ICollection<T> tList, IProgress<PRISM.ProgressData> progress = null);
        void Delete(T t);
        void DeleteAll(ICollection<T> tList);
        void DeleteAllStateless(ICollection<T> tList);
        T FindById(int id);
        List<T> FindAll();
    }
}