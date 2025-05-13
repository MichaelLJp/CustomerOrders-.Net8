using Microsoft.EntityFrameworkCore;
using CustomerOrders.Domain.Entities;
using CustomerOrders.Domain.Interfaces;
using CustomerOrders.Infrastructure.Data;

namespace CustomerOrders.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task CreateAsync(T entity)
        {
            if (typeof(T) == typeof(Order))
            {
                var order = entity as Order;
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == order!.CustomerId);

                if (!customerExists)
                    throw new InvalidOperationException("El cliente especificado no existe.");
            }

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            if (typeof(T) == typeof(Order))
            {
                var order = entity as Order;
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == order!.CustomerId);

                if (!customerExists)
                    throw new InvalidOperationException("No se puede asignar un cliente inexistente.");
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}