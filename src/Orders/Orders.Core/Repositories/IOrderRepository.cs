using Orders.Core.Entities;
using Orders.Core.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orders.Core.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}
