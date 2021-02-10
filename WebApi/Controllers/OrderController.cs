using System.Threading.Tasks;
using CqrsFramework;
using Microsoft.AspNetCore.Mvc;
using UseCases.Order;
using UseCases.Order.GetOrder;
using UseCases.Order.UpdateOrder;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IHandlerDispatcher handlerDispatcher;

        public OrderController(IHandlerDispatcher handlerDispatcher)
        {
            this.handlerDispatcher = handlerDispatcher;
        }

        [HttpGet("{id}")]
        public async Task<OrderDto> Get(int id)
        {
            return await this.handlerDispatcher.SendAsync<OrderDto>(new GetOrderRequest { Id = id });
        }

        [HttpPost("{id}")]
        public async Task Update(int id, [FromBody]OrderDto dto, [FromServices] IRequestHandler<UpdateOrderCommand, int> updateOrderCommandHandler)
        {
            var command = new UpdateOrderCommand
            {
                Id = id,
                Dto = dto
            };

            await this.handlerDispatcher.SendAsync<Unit>(command);
        }
    }
}