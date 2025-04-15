using System.Linq.Expressions;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ContextDb _context;
    
    public ApplicationUserRepository(ContextDb context) : base(context)
    {
        _context = context;
    }
}