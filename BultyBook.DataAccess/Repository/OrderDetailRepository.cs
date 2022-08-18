using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.DataAccess.Data;
using BultyBook.Models;

namespace BultyBook.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext context;
        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            context.OrderDetails.Update(orderDetail);
        }
    }
}
