using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BultyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T - Category (for example)

        T GetFirstOrDefault(Expression<Func<T,bool>> filter, string? includeProperies = null);
        IEnumerable<T> GetAll(string? includeProperies = null);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        bool Exist(int id);
    }
}
