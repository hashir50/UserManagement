using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repository;
using UserManagement.Infrastructure.DBContext;

namespace UserManagement.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public UserManagementContext _context = null;
        public DbSet<T> DbSet = null;
        public bool Disposed = false;
        public GenericRepository(UserManagementContext dbContext)
        {
            _context = dbContext;
            DbSet = _context.Set<T>();
        }
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            var query = _context.Set<T>().AsQueryable();
            return query.Any(predicate);
        }
        public void Delete(T obj)
        {
            if (obj is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                baseEntity.ModifiedAt = DateTime.UtcNow;
                baseEntity.ModifiedBy = "";
                DbSet.Attach(obj);
                _context.Entry(obj).State = EntityState.Modified;
            }
        }
        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                if (Disposing)
                {
                    _context.Dispose();
                }
            }
            Disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            T item;
            var query = _context.Set<T>().AsQueryable();
            item = query.FirstOrDefault(predicate);
            return item;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }
        public async Task InsertAsync(T obj)
        {
            if (obj is BaseEntity baseEntity)
                baseEntity.CreatedAt = DateTime.UtcNow;
            await DbSet.AddAsync(obj);
        }
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task Update(T obj)
        {
            var entry =  _context.Entry(obj);
            if (entry.State == EntityState.Detached)
            {
                DbSet.Attach(obj);
            }
            if (obj is BaseEntity baseEntity)
                baseEntity.ModifiedAt = DateTime.UtcNow;

            entry.State = EntityState.Modified;
        }
        public List<T> Where(Expression<Func<T, bool>> predicate, params string[] navigationProperties)
        {
            List<T> list;
            var query = _context.Set<T>().AsQueryable();
            foreach (string navigationProperty in navigationProperties)
                query = query.Include(navigationProperty);
            list = query.Where(predicate).ToList<T>();
            return list;
        }
    }
}
