using Microsoft.EntityFrameworkCore;
using Orders.Core.Entities;

namespace Orders.Infrastructure.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {

        }

        public DbSet<Order> Order { get; set; }
    }
}
