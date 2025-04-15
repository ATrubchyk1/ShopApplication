using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ContextDb _context;
    public CategoryRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public void Update(Category category)
    {
        var categoryFromDb = _context.Category.FirstOrDefault(x => x.Id == category.Id);
        if (categoryFromDb != null)
        {
            categoryFromDb.Name = category.Name;
            categoryFromDb.DisplayOrder = category.DisplayOrder;
        }
    }
}