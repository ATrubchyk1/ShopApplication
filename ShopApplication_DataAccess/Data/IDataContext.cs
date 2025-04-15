using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data;

public interface IDataContext
{
    public IEnumerable<Category> GetCategories();
    public void AddCategory(Category category);
    public Category GetCategoryById(int? id);
    public void UpdateCategory(Category category);
    public int DeleteCategory(int? id);
    
    
    public IEnumerable<ApplicationType> GetApplicationTypes();
    public void AddApplicationType(ApplicationType applicationType);
    public ApplicationType GetApplicationTypeById(int? id);
    public void UpdateApplicationType(ApplicationType applicationType);
    public int DeleteApplicationType(int? id);
    
    
    public IEnumerable<Product> GetProduct();
    public void AddProduct(Product product);
    public Product GetProductById(int? id, int mark = 0);
    public void UpdateProduct(Product product);
    public int DeleteProduct(int? id);
    
    public ApplicationUser? GetApplicationUserById(string claimId);
}