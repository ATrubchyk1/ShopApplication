using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    public void Update(OrderDetail orderDetail);
}