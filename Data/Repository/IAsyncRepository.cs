using Data.Entities;
using System.Linq.Expressions;

namespace Data.Repository
{
    public interface IAsyncRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool asNoTracking = false,
            bool showDeleted = false);
        Task<List<TReturn>> Select<TReturn>(
            Func<TEntity, TReturn> select,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool asNoTracking = false,
            bool showDeleted = false);
        Task<TEntity> GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "",
            bool asNoTracking = false,
            bool showDeleted = false);
        Task<TEntity> GetFirst<TOption>(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity, TOption>> includeProperties = null,
            bool asNoTracking = false,
            bool showDeleted = false);
        Task<TEntity> GetByID(int id);
        void Insert(TEntity entity);
        void Delete(TEntity entityToDelete);
        //void Delete(int id);
        //void Update(TEntity entityToUpdate);
    }
}
