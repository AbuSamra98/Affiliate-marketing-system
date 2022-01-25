using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository.Core
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        public Repository(DbContext context)
        {
            Context = context;
        }
        public async Task<TEntity> Find(object id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public async Task<int> Add(TEntity entity)
        {
            await Context.Set<TEntity>().AddAsync(entity);

            return await Context.SaveChangesAsync();
        }

        public async Task<int> AddRange(IEnumerable<TEntity> entities)
        {
            await Context.Set<TEntity>().AddRangeAsync(entities);
            return await Context.SaveChangesAsync();
        }

        public async Task<int> Update(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
            return await Context.SaveChangesAsync();
        }
        public async Task<int> UpdateRange(IEnumerable<TEntity> entity)
        {
            Context.Set<TEntity>().UpdateRange(entity);
            return await Context.SaveChangesAsync();
        }


        public async Task<int> Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            return await Context.SaveChangesAsync();
        }

        public async Task<int> RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            return await Context.SaveChangesAsync();
        }


        //public IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate)
        //{
        //    return Context.Set<TEntity>().Where(predicate).AsQueryable();
        //}

    }

}
