using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShopApplication_DataAccess.Data.Repository.IRepository;

namespace ShopApplication_DataAccess.Data.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ContextDb _context;
    internal DbSet<T> DbSet;

    public Repository(ContextDb context)
    {
        _context = context;
        DbSet = _context.Set<T>();
    }
    
    public async Task<T?> FindAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includes = null, bool isTracking = false)
    {
        IQueryable<T> query = DbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includes != null)
        {
            foreach (var include in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(include);
            }
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (isTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.ToListAsync();
    }

    public async Task<T?>FirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null, string? includes = null, bool isTracking = true)
    {
        IQueryable<T> query = DbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includes != null)
        {
            foreach (var include in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(include);
            }
        }

        if (!isTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.FirstOrDefaultAsync();
    }

    public async Task AddAsync(T entity)
    { 
        await DbSet.AddAsync(entity);
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entity)
    {
        _context.RemoveRange(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}