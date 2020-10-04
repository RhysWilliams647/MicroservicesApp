using AutoMapper;
using EventBusRabbitMQ.Events;
using Orders.Application.Commands;

namespace Orders.API.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<BasketCheckoutEvent, CheckoutOrderCommand>().ReverseMap();
        }
    }
}
