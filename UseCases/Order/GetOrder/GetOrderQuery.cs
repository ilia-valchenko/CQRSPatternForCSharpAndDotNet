using CqrsFramework;
using UseCases.Order.CheckOrder;

namespace UseCases.Order.GetOrder
{
    public class GetOrderQuery : IRequest<OrderDto>, ICheckOrderRequest
    {
        public int Id { get; set; }
    }
}