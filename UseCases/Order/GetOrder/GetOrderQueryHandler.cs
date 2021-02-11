using System.Threading.Tasks;
using AutoMapper;
using CqrsFramework;
using Infrastructure.Interfaces;

namespace UseCases.Order.GetOrder
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
    {
        private readonly IDbContext dbContext;
        private readonly IMapper mapper;

        public GetOrderQueryHandler(IDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        public async Task<OrderDto> HandleAsync(GetOrderQuery request)
        {
            var order = await this.dbContext.Orders.FindAsync(request.Id);
            return this.mapper.Map<OrderDto>(order);
        }
    }
}