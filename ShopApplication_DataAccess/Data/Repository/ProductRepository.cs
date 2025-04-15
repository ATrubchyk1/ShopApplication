using Microsoft.AspNetCore.Mvc.Rendering;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_Models;
using Constants = ShopApplication_Utility.Constants;

namespace ShopApplication_DataAccess.Data.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ContextDb _context;
    
    public ProductRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public  IEnumerable<SelectListItem> GetAllDropdownList(string obj)
    {
        if (obj == Constants.CategoryName)
        {
            return _context.Category.Select(elem => new SelectListItem
            {
                Text = elem.Name,
                Value = elem.Id.ToString()
            });
        }

        if (obj == Constants.ApplicationTypeName)
        {
            return _context.ApplicationTypes.Select(elem => new SelectListItem
            {
                Text = elem.Name,
                Value = elem.Id.ToString()
            });
        }

        return null;
    }
}