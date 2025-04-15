using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ContextDb _context;
    
    public OrderHeaderRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public void Update(OrderHeader orderHeader)
    {
        _context.OrderHeader.Update(orderHeader);
    }
}