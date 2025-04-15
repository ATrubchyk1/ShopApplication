using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ShopApplication_DataAccess.Data.Repository.IRepository;

public interface IRepository<T> where T : class
{
    public Task<T?> FindAsync(int id);
    
    public Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includes = null,
        bool isTracking = false
        );
    
    public Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? filter = null,
        string? includes = null,
        bool isTracking = false
    );
    
    public Task AddAsync(T entity);
    
    public void Remove(T entity);
    
    public void RemoveRange(IEnumerable<T> entity);
    
    public Task SaveAsync();

}