using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Services;

public class CategoryService : Service<Category>, ICategoryService
{
    
    private readonly ICategoryRepository _categoryRepository;
    
    public CategoryService(ICategoryRepository repository, ICategoryRepository categoryRepository) : base(repository)
    {
        _categoryRepository = categoryRepository;
    }

    public override void Update(Category category)
    {
        _categoryRepository.Update(category);
        _categoryRepository.SaveAsync();
    }
}