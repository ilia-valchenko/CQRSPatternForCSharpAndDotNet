using System;
using System.Threading.Tasks;
using AutoMapper;
using CqrsFramework;
using Infrastructure.Interfaces;

namespace UseCases.Order.GetOrder
{
    public class GetOrderQueryHandler : IRequestHandler<int, OrderDto>
    {
        private readonly IDbContext dbContext;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public GetOrderQueryHandler(IDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.dbContext = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<OrderDto> HandleAsync(int id)
        {
            var order = await this.dbContext.Orders.FindAsync(id);

            if (order == null)
            {
                throw new Exception("Not found");
            }

            if (order.UserEmail != this.currentUserService.Email)
            {
                throw new Exception("Forbidden");
            }

            return this.mapper.Map<OrderDto>(order);
        }
    }
}