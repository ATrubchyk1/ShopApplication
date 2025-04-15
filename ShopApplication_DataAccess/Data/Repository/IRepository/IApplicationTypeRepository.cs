using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository.IRepository;

public interface IApplicationTypeRepository : IRepository<ApplicationType>
{
    public void Update(ApplicationType applicationType);
}