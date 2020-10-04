using AutoMapper;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Settings;
using MediatR;
using Newtonsoft.Json;
using Orders.Application.Commands;
using Orders.Core.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Orders.API.RabbitMQs
{
    public class EventBusRabbitMQConsumer
    {
        private readonly IRabbitMQConnection connection;
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly IOrderRepository orderRepository;

        public EventBusRabbitMQConsumer(IRabbitMQConnection connection, IMediator mediator, IMapper mapper, IOrderRepository orderRepository)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public void Consume()
        {
            var channel = connection.CreateModel();
            channel.QueueDeclare(
                EventBusSettings.BasketCheckoutQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: EventBusSettings.BasketCheckoutQueue, autoAck: true, consumer: consumer);
        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if(e.RoutingKey == EventBusSettings.BasketCheckoutQueue)
            {
                var message = Encoding.UTF8.GetString(e.Body.Span);
                var checkoutEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);

                var command = mapper.Map<CheckoutOrderCommand>(checkoutEvent);
                var result = await mediator.Send(command);
            }
        }

        public void Disconnect()
        {
            connection.Dispose();
        }
    }
}
