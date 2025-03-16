using RepoLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DataLayer.DbContext;

namespace RepoLayer.Implemention
{
    public abstract class BaseRepo<T> : IBaseRepo<T> where T: class
    {
        protected readonly WeLearnContext dbContext;

        public BaseRepo(WeLearnContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual IQueryable<T> GetList()
        {
            return dbContext.Set<T>().AsQueryable();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task CreateAsync(T entity)
        {
            try
            {
                await dbContext.Set<T>().AddAsync(entity);
                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            dbContext.Set<T>().Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(int id)
        {
            T entity = await dbContext.Set<T>().FindAsync(id);
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> IdExistAsync(int id)
        {
            return await dbContext.Set<T>().FindAsync(id) != null;
        }
    }
}
