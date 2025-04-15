using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    public void Update(OrderHeader orderHeader);
}