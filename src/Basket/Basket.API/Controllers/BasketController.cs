using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer;
using EventBusRabbitMQ.Producer.Interfaces;
using EventBusRabbitMQ.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository repository;
        private readonly IMapper mapper;
        private readonly IEventBusRabbitMQProducer eventBus;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository repository, IMapper mapper, IEventBusRabbitMQProducer eventBus, ILogger<BasketController> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> GetBasket(string userName)
        {
            return Ok(await repository.GetBasket(userName) ?? new BasketCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> UpdateBasket([FromBody] BasketCart basket)
        {
            return Ok(await repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            var basketDeleted = await repository.DeleteBasket(userName);

            if (!basketDeleted)
            {
                logger.LogError($"Failed to delete basket for {userName}.");
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //  get total price of basket
            //  remove the basket
            //  send checkout event to rabbitmq

            var basket = await repository.GetBasket(basketCheckout.UserName);
            if (basket == null) return BadRequest();

            var basketToRemove = await repository.DeleteBasket(basket.UserName);
            if (!basketToRemove) return BadRequest();

            var eventMessage = mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.RequestId = Guid.NewGuid();
            eventMessage.TotalPrice = basket.TotalPrice;

            try
            {
                eventBus.PublishBasketCheckout(EventBusSettings.BasketCheckoutQueue, eventMessage);
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Failed to send basket checkout event");
            }

            return Accepted();
        }
    }
}
