using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly HospitalDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(HospitalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<T> AddAsync(T entity)
        {
            var entry = await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        public T Update(T entity) => _dbSet.Update(entity).Entity;

        public T Delete(T entity) => _dbSet.Remove(entity).Entity;
    }
}
