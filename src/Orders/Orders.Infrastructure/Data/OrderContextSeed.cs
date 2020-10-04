using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(
            OrderContext context,
            ILoggerFactory loggerFactory,
            int retry = 0)
        {
            try
            {
                context.Database.Migrate();

                if (context.Order.Any()) return;

                context.Order.AddRange(GetPreconfiguredOrders());
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if(retry < 3)
                {
                    retry++;
                    var log = loggerFactory.CreateLogger<OrderContextSeed>();
                    log.LogError(e, "Failed to seed database");
                    await SeedAsync(context, loggerFactory, retry);
                }
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>()
            {
                new Order() { UserName = "swn", FirstName = "Mehmet", LastName = "Ozkaya", EmailAddress = "meh@ozk.com", AddressLine = "Bahcelievler", TotalPrice = 5239 },
                new Order() { UserName = "swn", FirstName = "Selim", LastName = "Arslan", EmailAddress ="sel@ars.com", AddressLine = "Ferah", TotalPrice = 3486 }
            };
        }
    }
}
