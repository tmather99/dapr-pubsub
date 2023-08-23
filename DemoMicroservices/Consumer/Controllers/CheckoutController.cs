using System;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CheckoutService.controller
{
    [ApiController]
    public class CheckoutServiceController : ControllerBase
    {
        private readonly ILogger<CheckoutServiceController> _logger;
        private const string DAPR_PUBSUB_NAME = "pubsub";
        private const string TOPIC_NAME = "orders";

        public CheckoutServiceController(ILogger<CheckoutServiceController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //Subscribe to a topic
        [Topic(DAPR_PUBSUB_NAME, TOPIC_NAME)]
        [HttpPost("checkout")]
        public void Checkout([FromBody] Messages.Commands.Order order)
        {
            Console.WriteLine("Subscriber received : " + order.OrderId);
            _logger.LogInformation("Received checkout: {OrderId}, {OrderNumber}", order.OrderId, order.OrderNumber);
        }

        [HttpPost("update")]
        public void Update([FromBody] Messages.Commands.Order order)
        {
            _logger.LogInformation("Update order: {OrderId}, {OrderNumber}", order.OrderId, order.OrderNumber);
        }
    }
}