using MediatR;
using Orders.Application.Commands;
using Orders.Application.Mapper;
using Orders.Application.Responses;
using Orders.Core.Entities;
using Orders.Core.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orders.Application.Handlers
{
    public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, OrderResponse>
    {
        private readonly IOrderRepository orderRepository;

        public CheckoutOrderHandler(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<OrderResponse> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = OrderMapper.Mapper.Map<Order>(request);
            if (orderEntity == null) throw new ApplicationException("Failed to map order entity");

            var newOrder = await orderRepository.AddAsync(orderEntity);

            var orderResponse = OrderMapper.Mapper.Map<OrderResponse>(newOrder);
            return orderResponse;
        }
    }
}
