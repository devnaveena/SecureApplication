using Contracts;
using Contracts.IRepositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext { get; set; }
        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }


        public IQueryable<T> FindAll(bool includeInactive = false)
        {
            IQueryable<T> query = RepositoryContext.Set<T>().AsQueryable().AsNoTracking();

            if (includeInactive)
            {
                var isActiveProperty = typeof(T).GetProperty("IsActive");
                if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
                {
                    query = query.AsEnumerable().Where(e => (bool)isActiveProperty.GetValue(e)!).AsQueryable();
                }
            }

            return query;
        }


        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool includeInactive = false)
        {
            IQueryable<T> query = RepositoryContext.Set<T>().AsQueryable().AsNoTracking();

            if (includeInactive)
            {
                // Exclude inactive entities
                var isActiveProperty = typeof(T).GetProperty("IsActive");
                if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
                {
                    query = query.AsEnumerable().Where(e => (bool)isActiveProperty.GetValue(e)!).AsQueryable();
                }
            }

            return query.Where(expression);
        }

        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);

        public void Delete(T entity)
        {
            var isActiveProperty = typeof(T).GetProperty("IsActive");
            if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
            {
                isActiveProperty.SetValue(entity, false);
                RepositoryContext.Entry(entity).State = EntityState.Modified;
            }
        }

    }
}