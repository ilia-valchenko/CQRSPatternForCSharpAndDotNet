using System.Threading.Tasks;
using CqrsFramework;
using Microsoft.AspNetCore.Mvc;
using UseCases.Order;
using UseCases.Order.UpdateOrder;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRequestHandler<int, OrderDto> getOrderHandler;
        private readonly IRequestHandler<UpdateOrderCommand, Unit> updateOrderHandler;

        // We don't need to pass all of the handlers in the constructor as we
        // can have a lot of them. We can use ASP .NET Core injection method.
        public OrderController(
            IRequestHandler<int, OrderDto> getOrderHandler,
            IRequestHandler<UpdateOrderCommand, Unit> updateOrderHandler)
        {
            this.getOrderHandler = getOrderHandler;
            this.updateOrderHandler = updateOrderHandler;
        }

        [HttpGet("{id}")]
        public async Task<OrderDto> Get(int id)
        {
            return await this.getOrderHandler.HandleAsync(id);
        }

        [HttpPost("{id}")]
        public async Task Update(int id, [FromBody]OrderDto dto, [FromServices] IRequestHandler<UpdateOrderCommand, int> updateOrderCommandHandler)
        {
            var command = new UpdateOrderCommand
            {
                Id = id,
                Dto = dto
            };

            await updateOrderCommandHandler.HandleAsync(command);
        }
    }
}