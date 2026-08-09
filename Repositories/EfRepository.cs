using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MVC_P.Data;

namespace MVC_P.Repositories;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    private readonly DbSet<T> _set;

    public EfRepository(ApplicationDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public Task<T?> GetByIdAsync(int id) => _set.FindAsync(id).AsTask();

    public Task<List<T>> GetAllAsync() => _set.ToListAsync();

    public Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) => _set.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity)
    {
        await _set.AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
