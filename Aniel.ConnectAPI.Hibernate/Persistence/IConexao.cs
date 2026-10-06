using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnielTV.Hibernate.Persistence
{
    public interface IConexao<T> where T : class
    {
        Task<long> Max(Expression<Func<T, object>> expression, Expression<Func<T, bool>> where = null);
        Task<List<T>> query(string tableName, string[] campos, string where);
        Task Remove(T obj);
        Task Remove(IList<T> obj);
        Task RemoveAll();
        Task Save(T obj);
        Task Save(List<T> obj);
        Task SaveOrUpdate(T obj);
        Task SaveOrUpdate(List<T> obj);
        Task<IList<T>> select();
        Task<IList<T>> select(Expression<Func<T, bool>> expression);
        Task<IList<T>> selectSql(string consulta);
        Task Update(T obj);
    }
}