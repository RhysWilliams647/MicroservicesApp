using EventBusRabbitMQ.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusRabbitMQ.Producer.Interfaces
{
    public interface IEventBusRabbitMQProducer
    {
        void PublishBasketCheckout(string queueName, BasketCheckoutEvent checkoutEvent);
    }
}
