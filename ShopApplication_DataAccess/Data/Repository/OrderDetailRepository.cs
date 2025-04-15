using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ContextDb _context;
    
    public OrderDetailRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public void Update(OrderDetail orderDetail)
    {
        _context.OrderDetail.Update(orderDetail);
    }
}