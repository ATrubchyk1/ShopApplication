using System.Linq.Expressions;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository;

public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
{
    private readonly ContextDb _context;
    
    public ApplicationTypeRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public void Update(ApplicationType applicationType)
    {
        var categoryFromDb = _context.ApplicationTypes.FirstOrDefault(x => x.Id == applicationType.Id);
        if (categoryFromDb != null)
        {
            categoryFromDb.Name = applicationType.Name;
        }
    }
}