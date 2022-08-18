using BultyBook.DataAccess.Repository.IRepository;
using System.Linq.Expressions;
using BultyBook.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace BultyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public bool Exist(int id)
        {
            return dbSet.Find(id) != null;
        }

        public IEnumerable<T> GetAll(Expression<Func<T,bool>>? filter = null, string? includeProperies = null)
        {
            IQueryable<T> query = dbSet;

            if(filter != null)
                query = query.Where(filter);

            if(includeProperies != null)
            {
                foreach(var property in includeProperies.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.ToList(); 
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperies = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if (includeProperies != null)
            {
                foreach (var property in includeProperies.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

    }
}
