using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Services;

public class ApplicationTypeService : Service<ApplicationType>, IApplicationTypeService
{
    private readonly IApplicationTypeRepository _applicationTypeRepository;
    
    public ApplicationTypeService(IApplicationTypeRepository applicationTypeRepository)
        : base(applicationTypeRepository)
    {
        _applicationTypeRepository = applicationTypeRepository;
    }

    public override void Update(ApplicationType entity)
    {
        _applicationTypeRepository.Update(entity);
        _applicationTypeRepository.SaveAsync();
    }
}