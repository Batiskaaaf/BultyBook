using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.DataAccess.Data;
using BultyBook.Models;

namespace BultyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var order = context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(order != null)
            {
                order.OrderStatus = orderStatus;
                if(paymentStatus != null)
                    order.PaymentStatus = paymentStatus;
            }
        }
    }
}
