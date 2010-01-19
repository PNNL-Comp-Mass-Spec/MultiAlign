using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.Runtime.Serialization;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
{
    public interface IGenericDAO<T>
    {
        void Add(T t);
        void AddAll(List<T> tList);
        void Update(T t);
		void UpdateAll(List<T> tList);
        void Delete(T t);
		void DeleteAll(List<T> tList);
        T FindById(int id);
        List<T> FindAll();
    }
}
