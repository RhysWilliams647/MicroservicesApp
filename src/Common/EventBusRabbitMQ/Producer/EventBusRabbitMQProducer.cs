using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace EventBusRabbitMQ.Producer
{
    public class EventBusRabbitMQProducer : IEventBusRabbitMQProducer
    {
        private readonly IRabbitMQConnection connection;

        public EventBusRabbitMQProducer(IRabbitMQConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void PublishBasketCheckout(string queueName, BasketCheckoutEvent checkoutEvent)
        {
            using var channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: queueName, 
                durable: false, 
                exclusive: false, 
                autoDelete: false, 
                null);
            var message = JsonConvert.SerializeObject(checkoutEvent);
            var body = Encoding.UTF8.GetBytes(message);

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;

            channel.ConfirmSelect();
            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: true,
                basicProperties: properties,
                body: body);
            channel.WaitForConfirmsOrDie();

            channel.BasicAcks += (sender, eventArgs) =>
            {
                Console.WriteLine("Sent checkout event to RabbitMQ");
                //  implement ack handle
                
            };
            channel.ConfirmSelect();
        }
    }
}
