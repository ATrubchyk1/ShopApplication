using System.Linq.Expressions;
using ShopApplication_DataAccess.Data.Repository.IRepository;

namespace ShopApplication_DataAccess.Data.Services.IServices;

public interface IService<T> where T : class
{
    public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>,
        IOrderedQueryable<T>>? orderBy = null, string? includes = null, bool isTracking = false);
    public Task<T?> GetByIdAsync(int id);
    public Task<T?> FirstOfDefaultAsync(Expression<Func<T, bool>>? filter = null, string? includes = null, bool isTracking = true);
    public Task AddAsync(T entity);
    public Task DeleteAsync(T entity);
    public Task DeleteRangeAsync(IEnumerable<T> entity);
    public Task SaveChangesAsync();
    
    void Update(T entity);
}