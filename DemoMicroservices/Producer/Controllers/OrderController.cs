using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Messages.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private string PUBSUB_NAME = "pubsub";
        private string TOPIC_NAME = "orders";

        private readonly ILogger<OrderController> _logger;
        private readonly DaprClient _daprClient;

        public OrderController(ILogger<OrderController> logger, DaprClient daprClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInformation("Get Order API");
            return new List<string>();
        }

        [HttpPost]
        public async Task<IActionResult> OrderProduct(Messages.Models.OrderViewModel order)
        {
            _logger.LogInformation("Post Order API");

            //Validate order placeholder
            try
            {
                var orderMessage = new Order {
                    OrderId = Guid.NewGuid(),
                    OrderAmount = order.OrderAmount,
                    OrderNumber = order.OrderNumber,
                    OrderDate = DateTime.UtcNow
                };

                await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, orderMessage);

                _logger.LogInformation(
                    "Send a message with Order ID {orderId}, {orderNumber}",
                    orderMessage.OrderId, orderMessage.OrderNumber);

                return Ok("Your order is processing.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error Send a message, {OrderNumber}", order.OrderNumber);

            }

            return BadRequest();
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrderAsync(Messages.Models.OrderViewModel order, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update Order number={orderNumber}, amount={amount}", order.OrderNumber, order.OrderAmount);

            var orderMessage = new Order
            {
                OrderId = Guid.NewGuid(),
                OrderAmount = order.OrderAmount,
                OrderNumber = order.OrderNumber,
                OrderDate = DateTime.UtcNow
            };

            var invokeRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Post, "checkout", "update", orderMessage);

            var resp = await _daprClient.InvokeMethodWithResponseAsync(invokeRequest, cancellationToken);

            if (resp.IsSuccessStatusCode)
            {
                return Ok(resp);
            }

            _logger.LogError("Failed to update Order number={orderNumber}, amount={amount}", order.OrderNumber, order.OrderAmount);

            return BadRequest(resp);
        }
    }
}