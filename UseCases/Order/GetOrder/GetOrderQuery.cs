using CqrsFramework;

namespace UseCases.Order.GetOrder
{
    public class GetOrderQuery : IRequest<OrderDto>
    {
        public int Id { get; set; }
    }
}