using Microsoft.AspNetCore.Mvc.Rendering;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    public void Update(Product product);

    public IEnumerable<SelectListItem> GetAllDropdownList(string obj);
}