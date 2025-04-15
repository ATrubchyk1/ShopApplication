using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;

namespace ShopApplication_DataAccess.Data.Services;

public class ProductService : Service<Product>, IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    
    public ProductService(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment) : base(productRepository)
    {
        _productRepository = productRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    public override void Update(Product entity)
    {
        _productRepository.Update(entity);
    }

    public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
    {
        return _productRepository.GetAllDropdownList(obj);
    }

    public async Task<ProductVM> GetProductVmAsync(int id)
    {
        var productVm = new ProductVM()
        {
            Product = new Product(),
            CategorySelectList = _productRepository.GetAllDropdownList(Constants.CategoryName),
            ApplicationTypeSelectList = _productRepository.GetAllDropdownList(Constants.ApplicationTypeName)
        };

        var tempProduct = await _productRepository.FindAsync(id);
        if (tempProduct != null)
        {
            productVm.Product = tempProduct;
        }
        return productVm;
    }

    public async Task<int> UpsertProductAsync(ProductVM productVm, IFormFileCollection files)
    {
        var webRootPath = _webHostEnvironment.WebRootPath;
        if (productVm.Product.Id == 0)
        {
            //creating
            var upload = webRootPath + Constants.ImagePath;
            var fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(files[0].FileName);

            using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
            {
                files[0].CopyTo(fileStream);
            }
            
            productVm.Product.Image = fileName + extension;
            await _productRepository.AddAsync(productVm.Product);
            await _productRepository.SaveAsync();
            return 0;
        }

        //updating
        var objectFromDb = await _productRepository.FirstOrDefaultAsync(p => p.Id == productVm.Product.Id, isTracking: false);
        if (files.Count > 0)
        {
            var upload = webRootPath + Constants.ImagePath;
            var fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(files[0].FileName);

            if (objectFromDb != null)
            {
                var oldFile = Path.Combine(upload, objectFromDb.Image);

                if (File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                }
            }

            using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
            {
                files[0].CopyTo(fileStream);
            }
            productVm.Product.Image = fileName + extension;
        }
        else
        {
            if (objectFromDb != null) productVm.Product.Image = objectFromDb.Image;
        }
        _productRepository.Update(productVm.Product);
        await _productRepository.SaveAsync();

        return 1;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.FindAsync(id);
        if (product == null)
        {
            return false;
        }
        
        var upload = _webHostEnvironment.WebRootPath + Constants.ImagePath;
        var oldFile = Path.Combine(upload, product.Image);
        
        if (File.Exists(oldFile))
        {
            File.Delete(oldFile);
        }
        _productRepository.Remove(product);
        await _productRepository.SaveAsync();
        return true;
    }
}