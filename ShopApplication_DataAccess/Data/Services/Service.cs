using System.Linq.Expressions;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;

namespace ShopApplication_DataAccess.Data.Services;

public class Service<T> : IService<T> where T : class
{
    public readonly IRepository<T> Repository;

    public Service(IRepository<T> repository)
    {
        Repository = repository;
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>,
        IOrderedQueryable<T>>? orderBy = null, string? includes = null, bool isTracking = false)
    {
        return await Repository.GetAllAsync(filter, orderBy, includes, isTracking);
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await Repository.FindAsync(id);
    }

    public async Task<T?> FirstOfDefaultAsync(Expression<Func<T, bool>>? filter = null, string? includes = null, bool isTracking = true)
    {
        return await Repository.FirstOrDefaultAsync(filter, includes, isTracking);
    }

    public async Task AddAsync(T entity)
    {
        await Repository.AddAsync(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        Repository.Remove(entity);
        await SaveChangesAsync();
    }
    
    public async Task DeleteRangeAsync(IEnumerable<T> entity)
    {
        Repository.RemoveRange(entity);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await Repository.SaveAsync();
    }

    public virtual void Update(T entity)
    {
        throw new NotImplementedException("Method has to be implemented in derived classes.");
    }
}