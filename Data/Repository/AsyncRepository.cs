using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repository
{
    //TODO add Include option in get method
    public class AsyncRepository<TEntity> : IAsyncRepository<TEntity> where TEntity : BaseEntity
    {
        protected internal Context _context;
        protected internal DbSet<TEntity> dbSet;

        public AsyncRepository(Context context)
        {
            _context = context;
            dbSet = context.Set<TEntity>();
        }

        public async virtual Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool asNoTracking = false,
            bool showDeleted = false)
        {
            IQueryable<TEntity> query = Get(filter, includeProperties, asNoTracking, showDeleted);

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async virtual Task<List<TReturn>> Select<TReturn>(
            Func<TEntity, TReturn> select,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool asNoTracking = false,
            bool showDeleted = false)
        {
            IQueryable<TEntity> query = Get(filter, includeProperties, asNoTracking, showDeleted);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.Select(select).ToList();
        }

        public async Task<TEntity> GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "",
            bool asNoTracking = false, 
            bool showDeleted = false)
        {
            IQueryable<TEntity> query = Get(filter, includeProperties, asNoTracking, showDeleted);
            return await query.FirstAsync();
        }

        public async Task<TEntity> GetFirst<TOption>(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity, TOption>> includeProperties = null,
            bool asNoTracking = false,
            bool showDeleted = false)
        {
            IQueryable<TEntity> query = Get(filter, "", asNoTracking, showDeleted);

            if (includeProperties != null)
            {
                query.Include(includeProperties);
            }

            return await query.FirstAsync();
        }

        public async virtual Task<TEntity> GetByID(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async virtual void Insert(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        protected virtual IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter,
            string includeProperties,
            bool asNoTracking,
            bool showDeleted)
        {
            IQueryable<TEntity> query = dbSet;
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (showDeleted)
            {
                query = query.Where(e => !e.IsDeleted);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        //public async virtual void Delete(int id)
        //{
        //    TEntity entityToDelete = dbSet.Find(id);
        //    Delete(entityToDelete);
        //}
        //public async virtual void Update(TEntity entityToUpdate)
        //{
        //    dbSet.Attach(entityToUpdate);
        //    _context.Entry(entityToUpdate).State = EntityState.Modified;
        //}
    }
}
