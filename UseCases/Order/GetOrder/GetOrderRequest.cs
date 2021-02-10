using CqrsFramework;

namespace UseCases.Order.GetOrder
{
    public class GetOrderRequest : IRequest<OrderDto>
    {
        public int Id { get; set; }
    }
}