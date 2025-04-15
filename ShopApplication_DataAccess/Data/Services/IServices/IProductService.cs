using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;

namespace ShopApplication_DataAccess.Data.Services.IServices;

public interface IProductService : IService<Product>
{
    public  IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    public Task<ProductVM> GetProductVmAsync(int id);
    public Task<int> UpsertProductAsync(ProductVM productVm, IFormFileCollection files);
    public Task<bool> DeleteProductAsync(int id);
}