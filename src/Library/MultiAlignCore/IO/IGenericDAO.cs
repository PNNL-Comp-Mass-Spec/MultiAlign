#region

using System.Collections.Generic;

#endregion

namespace MultiAlignCore.IO
{
    public interface IGenericDAO<T>
    {
        void Add(T t);
        void AddAll(ICollection<T> tList);
        void Update(T t);
        void UpdateAll(ICollection<T> tList);
        void Delete(T t);
        void DeleteAll(ICollection<T> tList);
        T FindById(int id);
        List<T> FindAll();
    }
}