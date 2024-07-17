using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Repository
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        Task<int> SaveAsync();
        Task<T> GetByIdAsync(int id); 
        Task<IEnumerable<T>> GetAllAsync();
        Task InsertAsync(T obj);
        Task Update(T obj);
        List<T> Where(Expression<Func<T, bool>> predicate, params string[] navigationProperties);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        void Delete(T obj);


    }
}
