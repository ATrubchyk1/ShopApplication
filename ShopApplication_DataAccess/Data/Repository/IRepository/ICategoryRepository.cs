using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    public void Update(Category category);
}