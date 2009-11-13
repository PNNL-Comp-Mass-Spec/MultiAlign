/// <file>IGenericDAO.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.Runtime.Serialization;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
{
    public interface IGenericDAO<T>
    {
        void Add(T t);
        void AddAll(Set<T> tSet);
        void Update(T t);
        void UpdateAll(Set<T> tSet);
        void Delete(T t);
        void DeleteAll(Set<T> tSet);
        T FindById(int id);
        List<T> FindAll();
    }
}
