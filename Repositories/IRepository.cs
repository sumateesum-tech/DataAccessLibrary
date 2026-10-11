using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(object id);
        int Add(T entity);
        int Update(T entity);
        int Delete(object id);
        bool Exists(object id);
    }
}
