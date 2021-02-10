using CqrsFramework;

namespace UseCases.Order.UpdateOrder
{
    // The DTO below will be transfered between two layers.
    public class UpdateOrderCommand : IVoidRequest
    {
        public int Id { get; set; }

        public OrderDto Dto { get; set; }
    }
}