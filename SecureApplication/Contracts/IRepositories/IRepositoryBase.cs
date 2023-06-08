using System.Linq.Expressions;
namespace Contracts.IRepositories
{
    public interface IRepositoryBase<T>
    {

        /// <summary>
        /// This function returns all entities of type T from the database as an IQueryable object.
        /// </summary>
        public IQueryable<T> FindAll(bool includeInactive = false);

        /// <summary>
        /// This function returns a queryable object of type T filtered by a given expression.
        /// </summary>
        /// <param name="expression">An expression that represents a condition to filter the entities in
        /// the database.</param>
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool includeInactive = false);

        /// <summary>
        /// This function adds a new entity to the database.
        /// </summary>
        /// <param name="T">T is a generic type parameter that represents the type of entity being /// created.</param>

        void Create(T entity);

        /// <summary>
        /// This function deletes an entity of type T from the repository context.
        /// </summary>
        /// <param name="T">T is a generic type parameter that represents the type of entity being deleted from the database.</param>
        void Delete(T entity);
    }
}