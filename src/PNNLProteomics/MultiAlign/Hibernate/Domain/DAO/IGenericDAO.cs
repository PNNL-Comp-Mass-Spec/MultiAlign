using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.Runtime.Serialization;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
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
